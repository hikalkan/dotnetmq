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
using MDS.Serialization;

namespace MDS.Communication.Messages
{
    /// <summary>
    /// Abstract class of all message classes.
    /// All messages transmiting on MDS must be derrived from this class.
    /// </summary>
    public abstract class MDSMessage : IMDSSerializable
    {
        /// <summary>
        /// MessageTypeId of message.
        /// It is used to serialize/deserialize message.
        /// </summary>
        public abstract int MessageTypeId { get; }
        
        /// <summary>
        /// Unique ID for this message.
        /// Thiss will be a GUID if it is not set.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// If this message is a reply for another message then RepliedMessageId contains first message's MessageId
        /// else RepliedMessageId is null default.
        /// </summary>
        public string RepliedMessageId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected MDSMessage()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Serializes this message.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public virtual void Serialize(IMDSSerializer serializer)
        {
            serializer.WriteStringUTF8(MessageId);
            serializer.WriteStringUTF8(RepliedMessageId);
        }

        /// <summary>
        /// Deserializes this message.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public virtual void Deserialize(IMDSDeserializer deserializer)
        {
            MessageId = deserializer.ReadStringUTF8();
            RepliedMessageId = deserializer.ReadStringUTF8();
        }
    }
}
