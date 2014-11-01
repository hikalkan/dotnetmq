/*
DotNetMQ - A Complete Message Broker For .NET
Copyright (C) 2011 Halil ibrahim KALKAN

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using MDS.Exceptions;
using MDS.Settings;
using MDS.Threading;

namespace MDS.Organization
{
    /// <summary>
    /// Represents all servers on network as a graph.
    /// And also stores references to all communicating adjacent servers.
    /// </summary>
    public class MDSServerGraph : IRunnable
    {
        #region Public properties

        /// <summary>
        /// Gets this (current) server node.
        /// </summary>
        public MDSServerNode ThisServerNode { get; private set; }

        /// <summary>
        /// All server nodes on network.
        /// </summary>
        public SortedList<string, MDSServerNode> ServerNodes { get; private set; }

        /// <summary>
        /// A collection that stores communicating adjacent MDS servers to this MDS server.
        /// </summary>
        public SortedList<string, MDSAdjacentServer> AdjacentServers { get; private set; }

        #endregion

        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to settings.
        /// </summary>
        private readonly MDSSettings _settings;

        /// <summary>
        /// This Timer is used to check mds server connections,
        /// send ping messages and reconnect if needed.
        /// </summary>
        private readonly Timer _pingTimer;
        
        /// <summary>
        /// This flag is used to start/stop _pingTimer.
        /// </summary>
        private volatile bool _running;

        #endregion

        #region Constructors

        /// <summary>
        /// Contructor.
        /// </summary>
        public MDSServerGraph()
        {
            _settings = MDSSettings.Instance;
            ServerNodes = new SortedList<string, MDSServerNode>();
            AdjacentServers = new SortedList<string, MDSAdjacentServer>();
            _pingTimer = new Timer(PingTimer_Elapsed, null, Timeout.Infinite, Timeout.Infinite);
            try
            {
                CreateGraph();
            }
            catch (MDSException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MDSException("Can not read settings file. Detail: " + ex.Message, ex);
            }
        }

        #endregion

        #region Public methods

        public void Start()
        {
            foreach (var server in AdjacentServers.Values)
            {
                server.Start();
            }

            _running = true;
            _pingTimer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public void Stop(bool waitToStop)
        {
            lock (_pingTimer)
            {
                _running = false;
                _pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            foreach (var server in AdjacentServers.Values)
            {
                server.Stop(waitToStop);
            }
        }

        public void WaitToStop()
        {
            foreach (var server in AdjacentServers.Values)
            {
                server.WaitToStop();
            }
        }

        /// <summary>
        /// Calculates all next (adjacent) servers for all destination servers. 
        /// </summary>
        /// <returns>
        /// List of "Destination server name - Next server name" pairs.
        /// For example:
        /// If path is "ThisServer - ServerB - ServerC" than Destination server is ServerC and NextServer is ServerB.
        /// </returns>
        public List<KeyValuePair<string, string>> GetNextServersForDestServers()
        {
            var list = new List<KeyValuePair<string, string>>(ServerNodes.Count);
            list.Add(new KeyValuePair<string, string>(ThisServerNode.Name, ThisServerNode.Name));

            foreach (var destNode in ServerNodes.Values)
            {
                if (!ThisServerNode.BestPathsToServers.ContainsKey(destNode.Name))
                {
                    continue;
                }

                var bestPath = ThisServerNode.BestPathsToServers[destNode.Name];
                if (bestPath.Count > 1)
                {
                    list.Add(new KeyValuePair<string, string>(destNode.Name, bestPath[1].Name));
                }
            }

            return list;
        }

        #endregion

        #region Private methods

        #region Initializing methods

        /// <summary>
        /// Creates graph according to settings.
        /// </summary>
        private void CreateGraph()
        {
            CreateServerNodes();
            JoinNodes();
            SetCurrentServer();
            CreateAdjacentServers();
            CalculateShortestPaths();
        }

        /// <summary>
        /// Creates ServerNodes list from _settings.
        /// </summary>
        private void CreateServerNodes()
        {
            foreach (var server in _settings.Servers)
            {
                ServerNodes.Add(server.Name, new MDSServerNode(server.Name));
            }
        }

        /// <summary>
        /// Gets adjacent nodes from settings and joins servers in ServerNodes.
        /// </summary>
        private void JoinNodes()
        {
            //Gets adjacents of all nodes
            var adjacentsOfServers = new SortedList<string, string>();
            foreach (var server in _settings.Servers)
            {
                adjacentsOfServers.Add(server.Name, server.Adjacents);
            }

            //Join adjacents of all nodes
            foreach (var serverName in adjacentsOfServers.Keys)
            {
                //Create adjacent list
                ServerNodes[serverName].Adjacents = new SortedList<string, MDSServerNode>();
                //Get adjacent names
                var adjacents = adjacentsOfServers[serverName].Split(',');
                //Add nodes as adjacent
                foreach (var adjacent in adjacents)
                {
                    var trimmedAdjacentName = adjacent.Trim();
                    if (string.IsNullOrEmpty(trimmedAdjacentName))
                    {
                        continue;
                    }

                    if (!ServerNodes.ContainsKey(trimmedAdjacentName))
                    {
                        throw new MDSException("Adjacent server (" + trimmedAdjacentName + ") of server (" + serverName + ") can not be found in servers list.");
                    }

                    ServerNodes[serverName].Adjacents.Add(trimmedAdjacentName, ServerNodes[trimmedAdjacentName]);
                }
            }
        }
        
        /// <summary>
        /// Sets ThisServerNode field according to _settings and ServerNodes
        /// </summary>
        private void SetCurrentServer()
        {
            if (ServerNodes.ContainsKey(_settings.ThisServerName))
            {
                ThisServerNode = ServerNodes[_settings.ThisServerName];
            }
            else
            {
                throw new MDSException("Current server is not defined in settings file.");
            }
        }

        /// <summary>
        /// Fills AdjacentServers collection according to settings.
        /// </summary>
        private void CreateAdjacentServers()
        {
            //Create MDSAdjacentServer objects to communicate with adjacent servers of this Server
            foreach (var server in _settings.Servers)
            {
                //If the node is this server, get IP and Port informations
                if (server.Name == _settings.ThisServerName)
                {
                    _settings["__ThisServerTCPPort"] = server.Port.ToString();
                }
                //If the node is adjacent to this server, create a MDSAdjacentServer object to communicate
                else if (ThisServerNode.Adjacents.ContainsKey(server.Name))
                {
                    AdjacentServers.Add(server.Name, new MDSAdjacentServer(server.Name, server.IpAddress, server.Port));
                }
            }
        }

        /// <summary>
        /// Calculates all shorted paths from all nodes to other nodes.
        /// </summary>
        private void CalculateShortestPaths()
        {
            //Find shortest paths from all servers to all servers
            foreach (var sourceNodeName in ServerNodes.Keys)
            {
                ServerNodes[sourceNodeName].BestPathsToServers = new SortedList<string, List<MDSServerNode>>();
                foreach (var destinationNodeName in ServerNodes.Keys)
                {
                    //Do not search if source and destination nodes are same
                    if (sourceNodeName == destinationNodeName)
                    {
                        continue;
                    }

                    var shortestPath = FindShortestPath(ServerNodes[sourceNodeName], ServerNodes[destinationNodeName]);
                    if (shortestPath == null)
                    {
                        throw new MDSException("There is no path from server '" + sourceNodeName + "' to server '" + destinationNodeName + "'");
                    }

                    ServerNodes[sourceNodeName].BestPathsToServers[destinationNodeName] = shortestPath;
                }
            }
        }

        /// <summary>
        /// Find one of the shortest paths from given source node to destination node.
        /// </summary>
        /// <param name="sourceServerNode">Source node</param>
        /// <param name="destServerNode">Destination node</param>
        /// <returns>A path from source to destination</returns>
        private static List<MDSServerNode> FindShortestPath(MDSServerNode sourceServerNode, MDSServerNode destServerNode)
        {
            //Find all paths
            var allPaths = new List<List<MDSServerNode>>();
            FindPaths(sourceServerNode, destServerNode, allPaths, new List<MDSServerNode>());

            //Get shortest
            if (allPaths.Count > 0)
            {
                var bestPath = allPaths[0];
                for (var i = 1; i < allPaths.Count; i++)
                {
                    if (bestPath.Count > allPaths[i].Count)
                    {
                        bestPath = allPaths[i];
                    }
                }

                return bestPath;
            }

            //No path from sourceServerNode to destServerNode
            return null;
        }

        /// <summary>
        /// Finds all the paths from currentServerNode to destServerNode as a recursive method.
        /// Passes all nodes, if destination node found, it is added to paths
        /// </summary>
        /// <param name="currentServerNode">Current server node</param>
        /// <param name="destServerNode">Destination server node</param>
        /// <param name="paths">All possible paths are inserted to this list</param>
        /// <param name="passedNodes"></param>
        private static void FindPaths(MDSServerNode currentServerNode, MDSServerNode destServerNode, ICollection<List<MDSServerNode>> paths, ICollection<MDSServerNode> passedNodes)
        {
            //Add current node to passedNodes to prevent multi-pass over same node
            passedNodes.Add(currentServerNode);

            //If current node is destination, then add passed nodes to found paths.
            if (currentServerNode == destServerNode)
            {
                var foundPath = new List<MDSServerNode>();
                foundPath.AddRange(passedNodes);
                paths.Add(foundPath);
            }
            //Else, Jump to adjacents nodes of current node and conitnue searching
            else
            {
                foreach (var adjacentServerNode in currentServerNode.Adjacents.Values)
                {
                    //If passed over this adjacentServerNode before, skip it
                    if (passedNodes.Contains(adjacentServerNode))
                    {
                        continue;
                    }
                    
                    //Search path from this adjacent server to destination (recursive call)
                    FindPaths(adjacentServerNode, destServerNode, paths, passedNodes);
                    //Remove node from passed nodes, because we may pass over this node for searching another path
                    passedNodes.Remove(adjacentServerNode);
                }
            }
        }

        #endregion

        #region Checking server connections periodically

        /// <summary>
        /// This method is called by _pingTimer periodically.
        /// </summary>
        /// <param name="state">Not used argument</param>
        private void PingTimer_Elapsed(object state)
        {
            try
            {
                //Stop ping timer temporary
                _pingTimer.Change(Timeout.Infinite, Timeout.Infinite);

                //Check connection for all adjacent servers
                foreach (var server in AdjacentServers.Values)
                {
                    try
                    {
                        server.CheckConnection();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
            finally
            {
                //Schedule ping timer for next running if MDSServerGraph is still running.
                lock (_pingTimer)
                {
                    if (_running)
                    {
                        _pingTimer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}