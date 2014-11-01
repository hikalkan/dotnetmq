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
using MDS.Communication.Messages;
using MDS.Settings;

namespace MDS.Organization.Routing
{
    /// <summary>
    /// Base class for distribution strategies.
    /// </summary>
    internal abstract class DistributionStrategyBase
    {
        /// <summary>
        /// Reference to RoutingRule object that uses this distribution strategy to route messages.
        /// </summary>
        protected readonly RoutingRule RoutingRule;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="routingRule">Reference to RoutingRule object that uses this distribution strategy to route messages</param>
        protected DistributionStrategyBase(RoutingRule routingRule)
        {
            RoutingRule = routingRule;
        }

        /// <summary>
        /// Sets the destination of a message.
        /// </summary>
        /// <param name="message">Message to set it's destination</param>
        /// <param name="destination">Destination to set to message</param>
        protected static void SetMessageDestination(MDSDataTransferMessage message, RoutingDestination destination)
        {
            //Sets destination server
            if (!string.IsNullOrEmpty(destination.Server))
            {
                message.DestinationServerName = destination.Server.Equals("this", StringComparison.OrdinalIgnoreCase)
                                                    ? MDSSettings.Instance.ThisServerName
                                                    : destination.Server;
            }

            //Sets destination application
            if (!string.IsNullOrEmpty(destination.Application))
            {
                message.DestinationApplicationName = destination.Application;
            }
        }
    }
}
