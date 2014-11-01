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
    /// This class is used to store transmit informations of a message throught a server.
    /// </summary>
    public class ServerTransmitReport : IMDSSerializable
    {
        /// <summary>
        /// Name of the server.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Message arriving time to server.
        /// </summary>
        public DateTime ArrivingTime { get; set; }

        /// <summary>
        /// Message leaving time from server.
        /// </summary>
        public DateTime LeavingTime { get; set; }

        /// <summary>
        /// Creates a new ServerTransmitReport.
        /// </summary>
        public ServerTransmitReport()
        {
            ArrivingTime = DateTime.MinValue;
            LeavingTime = DateTime.MinValue;
        }

        /// <summary>
        /// Serializes this object.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public void Serialize(IMDSSerializer serializer)
        {
            serializer.WriteStringUTF8(ServerName);
            serializer.WriteDateTime(ArrivingTime);
            serializer.WriteDateTime(LeavingTime);
        }

        /// <summary>
        /// Deserializes this object.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public void Deserialize(IMDSDeserializer deserializer)
        {
            ServerName = deserializer.ReadStringUTF8();
            ArrivingTime = deserializer.ReadDateTime();
            LeavingTime = deserializer.ReadDateTime();
        }
    }
}
