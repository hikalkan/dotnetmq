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

namespace MDS.Serialization
{
    /// <summary>
    /// This interface is used to deserialize primitives and objects.
    /// Only needed Read methods designed for MDS.
    /// </summary>
    public interface IMDSDeserializer
    {
        /// <summary>
        /// Deserializes and returns a serialized byte.
        /// </summary>
        /// <returns>Deserialized byte</returns>
        byte ReadByte();

        /// <summary>
        /// Reads a byte array from deserializing stream.
        /// Created byte array may be null or empty.
        /// </summary>
        /// <returns>Deserialized string</returns>
        byte[] ReadByteArray();
        
        /// <summary>
        /// Deserializes and returns a serialized integer.
        /// </summary>
        /// <returns>Deserialized integer</returns>
        int ReadInt32();

        /// <summary>
        /// Deserializes and returns a serialized unsigned integer.
        /// </summary>
        /// <returns>Deserialized unsigned integer</returns>
        uint ReadUInt32();

        /// <summary>
        /// Deserializes and returns a serialized long.
        /// </summary>
        /// <returns>Deserialized long</returns>
        long ReadInt64();

        /// <summary>
        /// Deserializes and returns a serialized boolean.
        /// </summary>
        /// <returns>Deserialized boolean</returns>
        bool ReadBoolean();

        /// <summary>
        /// Deserializes and returns a serialized DateTime object.
        /// </summary>
        /// <returns>Deserialized DateTime object</returns>
        DateTime ReadDateTime();

        /// <summary>
        /// Deserializes and returns a serialized char using UTF8.
        /// </summary>
        /// <returns>Deserialized char</returns>
        char ReadCharUTF8();

        /// <summary>
        /// Deserializes and returns a serialized string using UTF8.
        /// Created string may be null or empty.
        /// </summary>
        /// <returns>Deserialized string</returns>
        string ReadStringUTF8();

        /// <summary>
        /// Deserializes and returns an object that implements IMDSSerializable.
        /// Object creation method is passed as parameter and used to create empty object.
        /// Created object may be null.
        /// </summary>
        /// <typeparam name="T">A class that implements IMDSSerializable</typeparam>
        /// <param name="createObjectHandler">A function that creates an empty T object</param>
        /// <returns>Deserialized object</returns>
        T ReadObject<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : IMDSSerializable;

        /// <summary>
        /// Deserializes and returns an array of objects that implements IMDSSerializable.
        /// Object creation method is passed as parameter and used to create empty object.
        /// Created array may be null or empty.
        /// </summary>
        /// <typeparam name="T">A class that implements IMDSSerializable</typeparam>
        /// <param name="createObjectHandler">A function that creates an empty T object</param>
        /// <returns>Deserialized object</returns>
        T[] ReadObjectArray<T>(CreateSerializableObjectHandler<T> createObjectHandler) where T : IMDSSerializable;
    }
}
