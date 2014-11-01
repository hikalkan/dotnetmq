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
    /// This message is response to MDS Manager for GetApplicationWebServicesMessage.
    /// </summary>
    public class GetApplicationWebServicesResponseMessage : ControlMessage
    {
        /// <summary>
        /// Gets MessageTypeId for GetApplicationWebServicesMessage.
        /// </summary>
        public override int MessageTypeId
        {
            get { return ControlMessageFactory.MessageTypeIdGetApplicationWebServicesResponseMessage; }
        }

        /// <summary>
        /// True, if operation is success and no error occured.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Detailed information about operation result. Error text, if any error occured.
        /// </summary>
        public string ResultText { get; set; }

        /// <summary>
        /// Web service communicators of application.
        /// </summary>
        public ApplicationWebServiceInfo[] WebServices { get; set; }

        /// <summary>
        /// Serializes this message.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public override void Serialize(IMDSSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteBoolean(Success);
            serializer.WriteStringUTF8(ResultText);
            serializer.WriteObjectArray(WebServices);
        }

        /// <summary>
        /// Deserializes this message.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public override void Deserialize(IMDSDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            Success = deserializer.ReadBoolean();
            ResultText = deserializer.ReadStringUTF8();
            WebServices = deserializer.ReadObjectArray(() => new ApplicationWebServiceInfo());
        }
    }
}
