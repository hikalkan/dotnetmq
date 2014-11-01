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

using MDS.Serialization;

namespace MDS.Communication.Messages
{
    /// <summary>
    /// This message is used to acknowledge/reject a message and to send a MDSDataTransferMessage in same message object.
    /// It is used in web services.
    /// </summary>
    public class MDSDataTransferResponseMessage : MDSMessage
    {
        /// <summary>
        /// MessageTypeId of message.
        /// It is used to serialize/deserialize message.
        /// </summary>
        public override int MessageTypeId
        {
            get { return MDSMessageFactory.MessageTypeIdMDSDataTransferResponseMessage; }
        }

        /// <summary>
        /// This field is used to acknowledge/reject to an incoming message.
        /// </summary>
        public MDSOperationResultMessage Result { get; set; }

        /// <summary>
        /// This field is used to send a new message.
        /// </summary>
        public MDSDataTransferMessage Message { get; set; }

        /// <summary>
        /// Serializes this message.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public override void Serialize(IMDSSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteObject(Result);
            serializer.WriteObject(Message);
        }

        /// <summary>
        /// Deserializes this message.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public override void Deserialize(IMDSDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            Result = deserializer.ReadObject(() => new MDSOperationResultMessage());
            Message = deserializer.ReadObject(() => new MDSDataTransferMessage());
        }
    }
}
