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

using MDS.Communication.Messages;

namespace MDS.Organization.Routing
{
    /// <summary>
    /// Represents a filter of a routing.
    /// </summary>
    public class RoutingFilter
    {
        /// <summary>
        /// Source server name for the filter. Must be one of following values:
        /// Empty string or null: No filter on this property.
        /// this: For this/current server.
        /// A server name: For a specified server name.
        /// </summary>
        public string SourceServer { get; set; }

        /// <summary>
        /// Source application name for the filter. Must be one of following values:
        /// Empty string or null: No filter on this property.
        /// An application name: For a specified application name.
        /// </summary>
        public string SourceApplication { get; set; }

        /// <summary>
        /// Destination server name for the filter. Must be one of following values:
        /// Empty string or null: No filter on this property.
        /// this: For this/current server.
        /// A server name: For a specified server name.
        /// </summary>
        public string DestinationServer { get; set; }

        /// <summary>
        /// Destination application name for the filter. Must be one of following values:
        /// Empty string or null: No filter on this property.
        /// An application name: For a specified application name.
        /// </summary>
        public string DestinationApplication { get; set; }

        /// <summary>
        /// Transmit rule for the filter. Must be one of following values:
        /// Empty string or null: No filter on this property.
        /// An element of MessageTransmitRules enum.
        /// </summary>
        public string TransmitRule { get; set; }

        /// <summary>
        /// Creates a new RoutingFilter object.
        /// </summary>
        public RoutingFilter()
        {
            SourceServer = "";
            SourceApplication = "";
            DestinationServer = "";
            DestinationApplication = "";
            TransmitRule = "";
        }

        /// <summary>
        /// Checks if a message matches with this filter.
        /// </summary>
        /// <param name="message">Message to check</param>
        /// <returns>True, if message matches with this rule</returns>
        public bool Matches(MDSDataTransferMessage message)
        {
            if ((string.IsNullOrEmpty(SourceServer) || SourceServer == message.SourceServerName) &&
                (string.IsNullOrEmpty(SourceApplication) || SourceApplication == message.SourceApplicationName) &&
                (string.IsNullOrEmpty(DestinationServer) || DestinationServer == message.DestinationServerName) &&
                (string.IsNullOrEmpty(DestinationApplication) || DestinationApplication == message.DestinationApplicationName) &&
                (string.IsNullOrEmpty(TransmitRule) || TransmitRule == message.TransmitRule.ToString()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a string that presents a brief information about RoutingFilter object.
        /// </summary>
        /// <returns>Brief information about RoutingFilter object</returns>
        public override string ToString()
        {
            return string.Format("SourceServer: {0}, SourceApplication: {1}, DestinationServer: {2}, DestinationApplication: {3}, TransmitRule: {4}", SourceServer, SourceApplication, DestinationServer, DestinationApplication, TransmitRule);
        }
    }
}
