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
    /// Interface of a distribution strategy.
    /// A distribution strategy is a way of redirecting a message to one of available destinations.
    /// </summary>
    internal interface IDistributionStrategy
    {
        /// <summary>
        /// Initializes and Resets distribution strategy.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the destination of a message according to distribution strategy.
        /// </summary>
        /// <param name="message">Message to set it's destination</param>
        void SetDestination(MDSDataTransferMessage message);
    }
}
