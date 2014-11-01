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

namespace MDS.Communication.Messages.ControllerMessages
{
    /// <summary>
    /// This message is used to get an operation result message.
    /// </summary>
    public class OperationResultMessage : ControlMessage
    {        
        /// <summary>
        /// Gets MessageTypeId for OperationResultMessage.
        /// </summary>
        public override int MessageTypeId
        {
            get { return ControlMessageFactory.MessageTypeIdOperationResultMessage; }
        }

        /// <summary>
        /// True, if operation is successfully executed.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// If Success = True then "Success.", else error message.
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// Serializes this message.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public override void Serialize(IMDSSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteBoolean(Success);
            serializer.WriteStringUTF8(ResultMessage);
        }

        /// <summary>
        /// Deserializes this message.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public override void Deserialize(IMDSDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            Success = deserializer.ReadBoolean();
            ResultMessage = deserializer.ReadStringUTF8();
        }
    }
}
