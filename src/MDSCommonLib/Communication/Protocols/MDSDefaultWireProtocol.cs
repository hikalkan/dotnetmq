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
using MDS.Exceptions;
using MDS.Serialization;

namespace MDS.Communication.Protocols
{
    /// <summary>
    /// This class is the Default Protocol that is used by MDS to communicate with other applications.
    /// A message frame is sent and received by MDSDefaultWireProtocol:
    /// 
    /// - Protocol type: 4 bytes unsigned integer. 
    ///   Must be MDSDefaultProtocolType for MDSDefaultWireProtocol.
    /// - Message type: 4 bytes integer.
    ///   Must be defined in MDSMessageFactory class.
    /// - Serialized bytes of a MDSMessage object.
    /// </summary>
    public class MDSDefaultWireProtocol : IMDSWireProtocol
    {
        /// <summary>
        /// Specific number that a message must start with.
        /// </summary>
        public const uint MDSDefaultProtocolType = 19180685;

        /// <summary>
        /// Serializes and writes a MDSMessage according to the protocol rules.
        /// </summary>
        /// <param name="serializer">Serializer to serialize message</param>
        /// <param name="message">Message to be serialized</param>
        public void WriteMessage(IMDSSerializer serializer, MDSMessage message)
        {
            //Write protocol type
            serializer.WriteUInt32(MDSDefaultProtocolType);
            
            //Write the message type
            serializer.WriteInt32(message.MessageTypeId);
            
            //Write message
            serializer.WriteObject(message);
        }

        /// <summary>
        /// Reads and constructs a MDSMessage according to the protocol rules.
        /// </summary>
        /// <param name="deserializer">Deserializer to read message</param>
        /// <returns>MDSMessage object that is read</returns>
        public MDSMessage ReadMessage(IMDSDeserializer deserializer)
        {
            //Read protocol type
            var protocolType = deserializer.ReadUInt32();
            if (protocolType != MDSDefaultProtocolType)
            {
                throw new MDSException("Wrong protocol type: " + protocolType + ".");
            }

            //Read message type
            var messageTypeId = deserializer.ReadInt32();

            //Read and return message
            return deserializer.ReadObject(() => MDSMessageFactory.CreateMessageByTypeId(messageTypeId));
        }
    }
}
