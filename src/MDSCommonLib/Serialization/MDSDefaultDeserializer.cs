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
using System.IO;
using System.Text;
using MDS.Exceptions;

namespace MDS.Serialization
{
    /// <summary>
    /// This class is the default deserializer of MDS.
    /// The deserializing object must be serialized by MDSDefaultSerializer.
    /// Only needed deserializers designed for MDS.
    /// </summary>
    public class MDSDefaultDeserializer : IMDSDeserializer
    {
        #region Private fields

        /// <summary>
        /// The stream that is used to read serialized items for deserializing.
        /// </summary>
        private readonly Stream _stream;

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a new MDSDefaultDeserializer object.
        /// </summary>
        /// <param name="stream">The stream that is used to read serialized items for deserializing</param>
        public MDSDefaultDeserializer(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Deserializes and returns a serialized byte.
        /// </summary>
        /// <returns>Deserialized byte</returns>
        public byte ReadByte()
        {
            var b = _stream.ReadByte();
            if (b == -1)
            {
                ThrowEndOfStreamException();
            }

            return (byte)b;
        }

        /// <summary>
        /// Reads a byte array from deserializing stream.
        /// Created byte array may be null or empty.
        /// </summary>
        /// <returns>Deserialized string</returns>
        public byte[] ReadByteArray()
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }

            if (length == 0)
            {
                return new byte[0];
            }

            return ReadByteArray(length);
        }

        /// <summary>
        /// Deserializes and returns a serialized integer.
        /// </summary>
        /// <returns>Deserialized integer</returns>
        public int ReadInt32()
        {
            return ((ReadByte() << 24) |
                    (ReadByte() << 16) |
                    (ReadByte() << 8) |
                    (ReadByte())
                   );
        }

        /// <summary>
        /// Deserializes and returns a serialized unsigned integer.
        /// </summary>
        /// <returns>Deserialized unsigned integer</returns>
        public uint ReadUInt32()
        {
            return (uint) ReadInt32();
        }

        /// <summary>
        /// Deserializes and returns a serialized long.
        /// </summary>
        /// <returns>Deserialized long</returns>
        public long ReadInt64()
        {
            return (((long)ReadByte() << 56) |
                    ((long)ReadByte() << 48) |
                    ((long)ReadByte() << 40) |
                    ((long)ReadByte() << 32) |
                    ((long)ReadByte() << 24) |
                    ((long)ReadByte() << 16) |
                    ((long)ReadByte() << 8) |
                    ((long)ReadByte())
                   );
        }

        /// <summary>
        /// Deserializes and returns a serialized boolean.
        /// </summary>
        /// <returns>Deserialized boolean</returns>
        public bool ReadBoolean()
        {
            return (ReadByte() == 1);
        }

        /// <summary>
        /// Deserializes and returns a serialized DateTime object.
        /// </summary>
        /// <returns>Deserialized DateTime object</returns>
        public DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }

        /// <summary>
        /// Deserializes and returns a serialized char using UTF8.
        /// Note: A better way may be found.
        /// </summary>
        /// <returns>Deserialized char</returns>
        public char ReadCharUTF8()
        {
            return ReadStringUTF8()[0];
        }

        /// <summary>
        /// Deserializes and returns a serialized string using UTF8.
        /// Created string may be null or empty.
        /// </summary>
        /// <returns>Deserialized string</returns>
        public string ReadStringUTF8()
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }

            if (length == 0)
            {
                return "";
            }

            return Encoding.UTF8.GetString(ReadByteArray(length), 0, length);
        }

        /// <summary>
        /// Deserializes and returns an object that implements IMDSSerializable.
        /// Object creation method is passed as parameter and used to create empty object.
        /// Created object may be null.
        /// </summary>
        /// <typeparam name="T">A class that implements IMDSSerializable</typeparam>
        /// <param name="createObjectHandler">A function that creates an empty T object</param>
        /// <returns>Deserialized object</returns>
        public T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : IMDSSerializable
        {
            if (ReadByte() == 0)
            {
                return default(T);
            }

            var serializableObject = createObjectHandler();
            serializableObject.Deserialize(this);
            return serializableObject;
        }

        /// <summary>
        /// Deserializes and returns an array of objects that implements IMDSSerializable.
        /// Object creation method is passed as parameter and used to create empty object.
        /// Created array may be null or empty.
        /// </summary>
        /// <typeparam name="T">A class that implements IMDSSerializable</typeparam>
        /// <param name="createObjectHandler">A function that creates an empty T object</param>
        /// <returns>Deserialized object</returns>
        public T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : IMDSSerializable
        {
            var length = ReadInt32();
            if (length < 0)
            {
                return null;
            }

            if (length == 0)
            {
                return new T[0];
            }

            var objects = new T[length];
            for (var i = 0; i < length; i++)
            {
                objects[i] =  ReadObject(createObjectHandler);
            }

            return objects;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reads a byte array with spesified length.
        /// </summary>
        /// <param name="length">Length of the byte array to read</param>
        /// <returns>Read byte array</returns>
        private byte[] ReadByteArray(int length)
        {
            var buffer = new byte[length];
            var totalRead = 0;
            while (totalRead < length)
            {
                var read = _stream.Read(buffer, totalRead, length - totalRead);
                if (read <= 0)
                {
                    ThrowEndOfStreamException();
                }

                totalRead += read;
            }

            return buffer;
        }

        /// <summary>
        /// Throws an exception for
        /// </summary>
        private static void ThrowEndOfStreamException()
        {
            throw new MDSSerializationException("Can not read from stream! Input stream is closed.");
        }

        #endregion
    }
}
