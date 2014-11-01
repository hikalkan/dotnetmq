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

namespace MDS.Communication.Messages
{
    /// <summary>
    /// This class is used to send Ping messages to check if remote application is connected and working.
    /// MDS Servers send Ping messages to other MDS servers and gets response.
    /// Client applications send Ping messages to MDS servers and gets response.
    /// If there is no Ping message from a remote application for a while, connection is closed and
    /// reconnected if needed.
    /// </summary>
    public class MDSPingMessage : MDSMessage
    {
        /// <summary>
        /// MessageTypeId of message.
        /// It is used to serialize/deserialize message.
        /// </summary>
        public override int MessageTypeId
        {
            get { return MDSMessageFactory.MessageTypeIdMDSPingMessage; }
        }
    }
}
