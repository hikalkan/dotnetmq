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
    /// This class represents a message that is being transmitted between MDS server and a Controller (MDS Manager).
    /// </summary>
    public class MDSControllerMessage : MDSMessage
    {
        /// <summary>
        /// MessageTypeId for MDSControllerMessage.
        /// </summary>
        public override int MessageTypeId
        {
            get { return MDSMessageFactory.MessageTypeIdMDSControllerMessage; }
        }

        /// <summary>
        /// MessageTypeId of ControllerMessage.
        /// This field is used to deserialize MessageData.
        /// All types defined in ControlMessageFactory class.
        /// </summary>
        public int ControllerMessageTypeId { get; set; }

        /// <summary>
        /// Essential message data.
        /// This is a serialized object of a class in MDS.Communication.Messages.ControllerMessages namespace.
        /// </summary>
        public byte[] MessageData { get; set; }

        public override void Serialize(IMDSSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteInt32(ControllerMessageTypeId);
            serializer.WriteByteArray(MessageData);
        }

        public override void Deserialize(IMDSDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ControllerMessageTypeId = deserializer.ReadInt32();
            MessageData = deserializer.ReadByteArray();
        }
    }
}
