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

namespace MDS.Serialization
{
    /// <summary>
    /// This delegate is used with IMDSDeserializer to deserialize an object.
    /// It is used by IMDSDeserializer to create an instance of deserializing object.
    /// So, user of MDS serialization must supply a method that creates an empty T object.
    /// This is needed for performance reasons. Because it is slower to create object by reflection.
    /// </summary>
    /// <typeparam name="T">Type of the object to be deserialized</typeparam>
    /// <returns>An object from type T</returns>
    public delegate T CreateSerializableObjectHandler<T>() where T : IMDSSerializable;
}
