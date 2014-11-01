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

namespace MDS.Communication.Events
{
    /// <summary>
    /// A delegate to create events when a message received from a remote application.
    /// </summary>
    /// <param name="sender">The object which raises event</param>
    /// <param name="e">Event arguments</param>
    public delegate void MessageReceivedFromRemoteApplicationHandler(object sender, MessageReceivedFromRemoteApplicationEventArgs e);

    /// <summary>
    /// Stores informations about received message
    /// </summary>
    public class MessageReceivedFromRemoteApplicationEventArgs
    {
        /// <summary>
        /// Remote Application.
        /// </summary>
        public MDSRemoteApplication Application { get; set; }

        /// <summary>
        /// Communicator.
        /// </summary>
        public ICommunicator Communicator { get; set; }

        /// <summary>
        /// Received message from communicator.
        /// </summary>
        public MDSMessage Message { get; set; }
    }
}
