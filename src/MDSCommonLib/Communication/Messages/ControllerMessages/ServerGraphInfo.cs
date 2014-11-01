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
    /// Stores all MDS server's and server graph's informations.
    /// </summary>
    public class ServerGraphInfo : IMDSSerializable
    {
        /// <summary>
        /// Reference to this server on graph (This server is also in Servers list).
        /// </summary>
        public string ThisServerName { get; set; }

        /// <summary>
        /// All servers on graph.
        /// </summary>
        public ServerOnGraph[] Servers { get; set; }

        /// <summary>
        /// Serializes this object.
        /// </summary>
        /// <param name="serializer">Serializer used to serialize objects</param>
        public void Serialize(IMDSSerializer serializer)
        {
            serializer.WriteStringUTF8(ThisServerName);
            serializer.WriteObjectArray(Servers);
        }

        /// <summary>
        /// Deserializes this object.
        /// </summary>
        /// <param name="deserializer">Deserializer used to deserialize objects</param>
        public void Deserialize(IMDSDeserializer deserializer)
        {
            ThisServerName = deserializer.ReadStringUTF8();
            Servers = deserializer.ReadObjectArray(() => new ServerOnGraph());
        }

        #region Sub classes

        public class ServerOnGraph : IMDSSerializable
        {
            /// <summary>
            /// Name of this server.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// IP address of this server.
            /// </summary>
            public string IpAddress { get; set; }

            /// <summary>
            /// TCP Port number that is listened by this server.
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// List of adjacent servers of this server that are splitted by , or ;
            /// </summary>
            public string Adjacents { get; set; }

            /// <summary>
            /// Location of server (Left (X) and Top (Y) properties in design area, seperated by comma (,)).
            /// </summary>
            public string Location { get; set; }

            /// <summary>
            /// Serializes this object.
            /// </summary>
            /// <param name="serializer">Serializer used to serialize objects</param>
            public void Serialize(IMDSSerializer serializer)
            {
                serializer.WriteStringUTF8(Name);
                serializer.WriteStringUTF8(IpAddress);
                serializer.WriteInt32(Port);
                serializer.WriteStringUTF8(Adjacents);
                serializer.WriteStringUTF8(Location);
            }

            /// <summary>
            /// Deserializes this object.
            /// </summary>
            /// <param name="deserializer">Deserializer used to deserialize objects</param>
            public void Deserialize(IMDSDeserializer deserializer)
            {
                Name = deserializer.ReadStringUTF8();
                IpAddress = deserializer.ReadStringUTF8();
                Port = deserializer.ReadInt32();
                Adjacents = deserializer.ReadStringUTF8();
                Location = deserializer.ReadStringUTF8();
            }
        }

        #endregion
    }
}
