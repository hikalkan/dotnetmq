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

using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Threading;

namespace MDS.Communication
{
    /// <summary>
    /// Communicators is used by upper layers by this interface,
    /// all communicator classes must implement it.
    /// </summary>
    public interface ICommunicator : IRunnable
    {
        /// <summary>
        /// This event is raised when the state of the communicator changes.
        /// </summary>
        event CommunicatorStateChangedHandler StateChanged;

        /// <summary>
        /// This event is raised when a MdsMessage received.
        /// </summary>
        event MessageReceivedFromCommunicatorHandler MessageReceived;

        /// <summary>
        ///  Unique identifier for this communicator in this server.
        /// </summary>
        long ComminicatorId { get; }

        /// <summary>
        /// Connection state of communicator.
        /// </summary>
        CommunicationStates State { get; }

        /// <summary>
        /// Communication way for this communicator.
        /// </summary>
        CommunicationWays CommunicationWay { get; set; }

        /// <summary>
        /// Sends a message to the communicator.
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(MDSMessage message);
    }
}
