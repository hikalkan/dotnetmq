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
using MDS.Communication.Messages.ControllerMessages;

namespace MDS.Management
{
    /// <summary>
    /// A delegate to create events when a control message received from MDS server.
    /// </summary>
    /// <param name="sender">The object which raises event</param>
    /// <param name="e">Event arguments</param>
    public delegate void ControlMessageReceivedHandler(object sender, ControlMessageReceivedEventArgs e);

    /// <summary>
    /// Stores message informations.
    /// </summary>
    public class ControlMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Received message from MDS server.
        /// </summary>
        public ControlMessage Message { get; set; }

        /// <summary>
        /// Creates a ControlMessageReceivedEventArgs object.
        /// </summary>
        /// <param name="message">Received message from MDS server</param>
        public ControlMessageReceivedEventArgs(ControlMessage message)
        {
            Message = message;
        }
    }
}
