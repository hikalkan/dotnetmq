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

using System.Collections.Generic;

namespace MDS.Organization
{
    /// <summary>
    /// Represents a MDS server on network.
    /// </summary>
    public class MDSServerNode
    {
        /// <summary>
        /// Name of the remote application
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adjacent server nodes of this node.
        /// </summary>
        public SortedList<string, MDSServerNode> Adjacents { get; set; }

        /// <summary>
        /// Stores best paths to the all server nodes from this node
        /// </summary>
        public SortedList<string, List<MDSServerNode>> BestPathsToServers { get; set; }

        /// <summary>
        /// Constructur.
        /// </summary>
        /// <param name="name">name of server</param>
        public MDSServerNode(string name)
        {
            Name = name;
        }
    }
}
