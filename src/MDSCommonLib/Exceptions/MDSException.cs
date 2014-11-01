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
using System.Runtime.Serialization;

namespace MDS.Exceptions
{
    /// <summary>
    /// Represents a MDS Exception.
    /// This is the base class for exceptions that are thrown by MDS system.
    /// </summary>
    [Serializable]
    public class MDSException : Exception
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        public MDSException()
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        public MDSException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public MDSException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public MDSException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
