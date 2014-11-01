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

namespace MDS.Settings
{
    /// <summary>
    /// Represents a Server's informations in settings.
    /// </summary>
    public class ServerInfoItem
    {
        /// <summary>
        /// Name of this server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IP address of this server.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// TCP Port number that is listened by this server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// List of adjacent servers of this server that are splitted by , or ;
        /// </summary>
        public string Adjacents { get; set; }
    }
}
