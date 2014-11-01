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

namespace MDS.Serialization
{
    /// <summary>
    /// This class is the default serializer of MDS.
    /// The serialized object must be deserialized by MDSDefaultDeserializer.
    /// Only needed serializers designed for MDS.
    /// </summary>
    public class MDSDefaultSerializer : IMDSSerializer
    {
        #region Private fields

        /// <summary>
        /// The stream that is used to write serialized items.
        /// </summary>
        private readonly Stream _stream;

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a new MDSDefaultSerializer object.
        /// </summary>
        /// <param name="stream">The stream that is used to write serialized items</param>
        public MDSDefaultSerializer(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Serializes a byte.
        /// </summary>
        /// <param name="b">byte to serialize</param>
        public void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        /// <summary>
        /// Writes a byte array to serialization stream.
        /// Byte array may be null or empty.
        /// </summary>
        /// <param name="bytes">byte array to write</param>
        public void WriteByteArray(byte[] bytes)
        {
            if (bytes == null)
            {
                WriteInt32(-1);
            }
            else
            {
                WriteInt32(bytes.Length);
                if (bytes.Length > 0)
                {
                    _stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        /// <summary>
        /// Serializes a integer.
        /// </summary>
        /// <param name="number">integer to serialize</param>
        public void WriteInt32(int number)
        {
            _stream.WriteByte((byte)((number >> 24) & 0xFF));
            _stream.WriteByte((byte)((number >> 16) & 0xFF));
            _stream.WriteByte((byte)((number >> 8) & 0xFF));
            _stream.WriteByte((byte)(number & 0xFF));
        }

        /// <summary>
        /// Serializes an unsigned integer.
        /// </summary>
        /// <param name="number">unsigned integer to serialize</param>
        public void WriteUInt32(uint number)
        {
            WriteInt32((int) number);
        }

        /// <summary>
        /// Serializes a long.
        /// </summary>
        /// <param name="number">long to serialize</param>
        public void WriteInt64(long number)
        {
            _stream.WriteByte((byte)((number >> 56) & 0xFF));
            _stream.WriteByte((byte)((number >> 48) & 0xFF));
            _stream.WriteByte((byte)((number >> 40) & 0xFF));
            _stream.WriteByte((byte)((number >> 32) & 0xFF));
            _stream.WriteByte((byte)((number >> 24) & 0xFF));
            _stream.WriteByte((byte)((number >> 16) & 0xFF));
            _stream.WriteByte((byte)((number >> 8) & 0xFF));
            _stream.WriteByte((byte)(number & 0xFF));
        }

        /// <summary>
        /// Serializes a boolean.
        /// </summary>
        /// <param name="b">boolean to serialize</param>
        public void WriteBoolean(bool b)
        {
            _stream.WriteByte((byte) (b ? 1 : 0));
        }

        /// <summary>
        /// Serializes a DateTime object.
        /// </summary>
        /// <param name="dateTime">DateTime to serialize</param>
        public void WriteDateTime(DateTime dateTime)
        {
            WriteInt64(dateTime.Ticks);
        }

        /// <summary>
        /// Serializes a char according to UTF8.
        /// Char may be null or empty.
        /// Note: A better way may be found.
        /// </summary>
        /// <param name="c">char to serialize</param>
        public void WriteCharUTF8(char c)
        {
            WriteByteArray(Encoding.UTF8.GetBytes(c.ToString()));
        }

        /// <summary>
        /// Serializes a string according to UTF8.
        /// String may be null or empty.
        /// </summary>
        /// <param name="text">string to serialize</param>
        public void WriteStringUTF8(string text)
        {
            switch (text)
            {
                case null:
                    WriteInt32(-1);
                    break;
                case "":
                    WriteInt32(0);
                    break;
                default:
                    WriteByteArray(Encoding.UTF8.GetBytes(text));
                    break;
            }
        }

        /// <summary>
        /// Serializes an object that implements IMDSSerializable interface.
        /// Object may be null.
        /// </summary>
        /// <param name="serializableObject">object to serialize</param>
        public void WriteObject(IMDSSerializable serializableObject)
        {
            if (serializableObject == null)
            {
                _stream.WriteByte(0);
                return;
            }

            _stream.WriteByte(1);
            serializableObject.Serialize(this);
        }

        /// <summary>
        /// Serializes an array that all items implements IMDSSerializable interface.
        /// Object array may be null or empty.
        /// </summary>
        /// <param name="serializableObjects">objects to serialize</param>
        public void WriteObjectArray(IMDSSerializable[] serializableObjects)
        {
            if (serializableObjects == null)
            {
                WriteInt32(-1);
            }
            else
            {
                WriteInt32(serializableObjects.Length);
                for (var i = 0; i < serializableObjects.Length; i++)
                {
                    WriteObject(serializableObjects[i]);
                }
            }
        }

        #endregion
    }
}
