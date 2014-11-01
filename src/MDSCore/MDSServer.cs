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

using MDS.Communication;
using MDS.Organization;
using MDS.Organization.Routing;
using MDS.Settings;
using MDS.Storage;
using MDS.Threading;

namespace MDS
{
    /// <summary>
    /// Represents a MDS server. This is the main class to construct and run a MDS server.
    /// </summary>
    public class MDSServer : IRunnable
    {
        /// <summary>
        /// Settings.
        /// </summary>
        private readonly MDSSettings _settings;

        /// <summary>
        /// Storage manager used for database operations.
        /// </summary>
        private readonly IStorageManager _storageManager;

        /// <summary>
        /// Routing table.
        /// </summary>
        private readonly RoutingTable _routingTable;

        /// <summary>
        /// A Graph consist of server nodes. It also holds references to MDSAdjacentServer objects.
        /// </summary>
        private readonly MDSServerGraph _serverGraph;

        /// <summary>
        /// Reference to all MDS Managers. It contains communicators to all instances of MDS manager.
        /// </summary>
        private readonly MDSController _mdsManager;

        /// <summary>
        /// List of applications
        /// </summary>
        private readonly MDSClientApplicationList _clientApplicationList;

        /// <summary>
        /// Communication layer.
        /// </summary>
        private readonly CommunicationLayer _communicationLayer;

        /// <summary>
        /// Organization layer.
        /// </summary>
        private readonly OrganizationLayer _organizationLayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MDSServer()
        {
            _settings = MDSSettings.Instance;
            _serverGraph = new MDSServerGraph();
            _clientApplicationList = new MDSClientApplicationList();
            _mdsManager = new MDSController("MDSController");
            _storageManager = StorageManagerFactory.CreateStorageManager();
            _routingTable = new RoutingTable();
            _communicationLayer = new CommunicationLayer();
            _organizationLayer = new OrganizationLayer(_communicationLayer, _storageManager, _routingTable, _serverGraph, _clientApplicationList, _mdsManager);
            _mdsManager.OrganizationLayer = _organizationLayer;
        }

        /// <summary>
        /// Starts the MDS server.
        /// </summary>
        public void Start()
        {
            _storageManager.Start();
            CorrectDatabase();
            _communicationLayer.Start();
            _organizationLayer.Start();
        }

        /// <summary>
        /// Stops the MDS server.
        /// </summary>
        /// <param name="waitToStop">True, if caller thread must be blocked until MDS server stops.</param>
        public void Stop(bool waitToStop)
        {
            _communicationLayer.Stop(waitToStop);
            _organizationLayer.Stop(waitToStop);
            _storageManager.Stop(waitToStop);
        }

        /// <summary>
        /// Waits stopping of MDS server.
        /// </summary>
        public void WaitToStop()
        {
            _communicationLayer.WaitToStop();
            _organizationLayer.WaitToStop();
            _storageManager.WaitToStop();
        }

        /// <summary>
        /// Checks and corrects database records if needed.
        /// </summary>
        private void CorrectDatabase()
        {
            if (_settings["CheckDatabaseOnStartup"] != "true")
            {
                return;
            }

            //If Server graph is changed, records in storage engine (database) may be wrong, therefore, they must be updated 
            var nextServersList = _serverGraph.GetNextServersForDestServers();
            foreach (var nextServerItem in nextServersList)
            {
                _storageManager.UpdateNextServer(nextServerItem.Key, nextServerItem.Value);
            }
        }
    }
}
