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

namespace MDS.Organization.Routing
{
    /// <summary>
    /// Represents a Destination of a Routing.
    /// </summary>
    public class RoutingDestination
    {
        /// <summary>
        /// Destination server name. Must be one of following values:
        /// Empty string or null: Don't change destination server.
        /// this: Change to this/current server.
        /// A server name: Change to a specified server name.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Destination application name. Must be one of following values:
        /// Empty string or null: Don't change destination application.
        /// A application name: Change to a specified application name.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Route factor.
        /// Must be 1 or greater.
        /// </summary>
        public int RouteFactor { get; set; }

        /// <summary>
        /// Creates a new RoutingDestination object.
        /// </summary>
        public RoutingDestination()
        {
            Server = "";
            Application = "";
            RouteFactor = 1;
        }
        
        /// <summary>
        /// Returns a string that presents a brief information about RoutingFilter object.
        /// </summary>
        /// <returns>Brief information about RoutingFilter object</returns>
        public override string ToString()
        {
            return string.Format("Server: {0}, Application: {1}, RouteFactor: {2}", Server, Application, RouteFactor);
        }
    }
}
