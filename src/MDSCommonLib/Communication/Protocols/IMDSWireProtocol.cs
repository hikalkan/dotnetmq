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
using MDS.Serialization;

namespace MDS.Communication.Protocols
{
    /// <summary>
    /// This interface is used to Write/Read messages according to a Wire/Communication Protocol.
    /// </summary>
    public interface IMDSWireProtocol
    {
        /// <summary>
        /// Serializes and writes a MDSMessage according to the protocol rules.
        /// </summary>
        /// <param name="serializer">Serializer to serialize message</param>
        /// <param name="message">Message to be serialized</param>
        void WriteMessage(IMDSSerializer serializer, MDSMessage message);

        /// <summary>
        /// Reads and constructs a MDSMessage according to the protocol rules.
        /// </summary>
        /// <param name="deserializer">Deserializer to read message</param>
        /// <returns>MDSMessage object that is read</returns>
        MDSMessage ReadMessage(IMDSDeserializer deserializer);
    }
}
