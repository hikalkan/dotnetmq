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
    /// Message transmit rules.
    /// All messages are persistent except 'DirectlySend'.
    /// If a server doesn't stores message and transmiting it directly,
    /// it transmits this message before than a stored (persistent) message.
    /// </summary>
    public enum MessageTransmitRules : byte
    {
        /// <summary>
        /// Not persistent message.
        /// Message may be lost in an error.
        /// Message is not stored on any server. 
        /// Message is not guarantied to be delivered.
        /// This rule may be used if both of source and destination applications must be run at the same time.
        /// If no exception received while sending message,
        /// that means message delivered to and acknowledged by destination application correctly.
        /// This rule blocks sender application until destination application sends ACK for message.
        /// </summary>
        DirectlySend = 0,
        
        /// <summary>
        /// Persistent Message.
        /// Message can not be lost and it is being stored in all passing servers.
        /// Message is guarantied to be delivered and it will be delivered as ordered (FIFO).
        /// This is the slowest but most reliable rule.
        /// This rule blocks sender application until source (first) MDS server stores message.
        /// </summary>
        StoreAndForward,

        /// <summary>
        /// Non-persistent message.
        /// Message will be lost if MDS server which has message shuts down.
        /// Message is not guarantied to be delivered.
        /// </summary>
        NonPersistent
    }
}
