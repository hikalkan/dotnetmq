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

using System.IO;

namespace MDS.Serialization
{
    /// <summary>
    /// This class is used to simplify serialization/deserialization with MDS serialization classes.
    /// </summary>
    public static class MDSSerializationHelper
    {
        /// <summary>
        /// Serializes an object that implements IMDSSerializable and returns serialized byte array.
        /// </summary>
        /// <param name="serializableObject">Object to serialize</param>
        /// <returns>Serialized object as byte array</returns>
        public static byte[] SerializeToByteArray(IMDSSerializable serializableObject)
        {
            var stream = new MemoryStream();
            new MDSDefaultSerializer(stream).WriteObject(serializableObject);
            return stream.ToArray();
        }

        /// <summary>
        /// Serializes an object that implements IMDSSerializable to a Stream.
        /// </summary>
        /// <param name="stream">Stream to write serialized object</param>
        /// <param name="serializableObject">Object to serialize</param>
        public static void SerializeToStream(Stream stream, IMDSSerializable serializableObject)
        {
            new MDSDefaultSerializer(stream).WriteObject(serializableObject);
        }

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <typeparam name="T">Type of object. This type must implement IMDSSerializable interface</typeparam>
        /// <param name="createObjectHandler">A function that creates an instance of that object (T)</param>
        /// <param name="bytesOfObject">Byte array</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeFromByteArray<T>(CreateSerializableObjectHandler<T> createObjectHandler, byte[] bytesOfObject) where T : IMDSSerializable
        {
            return new MDSDefaultDeserializer(new MemoryStream(bytesOfObject) {Position = 0}).ReadObject(createObjectHandler);
        }

        /// <summary>
        /// Deserializes an object via reading from a stream.
        /// </summary>
        /// <typeparam name="T">Type of object. This type must implement IMDSSerializable interface</typeparam>
        /// <param name="createObjectHandler">A function that creates an instance of that object (T)</param>
        /// <param name="stream">Deserialized object</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeFromStream<T>(CreateSerializableObjectHandler<T> createObjectHandler, Stream stream) where T : IMDSSerializable
        {
            return new MDSDefaultDeserializer(stream).ReadObject(createObjectHandler);
        }
    }
}
