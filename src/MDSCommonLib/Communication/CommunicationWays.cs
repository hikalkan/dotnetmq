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

namespace MDS.Communication
{
    /// <summary>
    /// Communication ways.
    /// A client application may just send messages from communication channel or it can send and receive messages.
    /// </summary>
    public enum CommunicationWays: byte
    {
        /// <summary>
        /// Application can only send messages to MDS server.
        /// </summary>
        Send = 1,

        /// <summary>
        /// Application can send and receive messages to/from MDS server.
        /// </summary>
        SendAndReceive = 2,
    }
}
