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

namespace MDS.Client.MDSServices
{
    /// <summary>
    /// Any MDSService class must has this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MDSServiceAttribute : Attribute
    {
        /// <summary>
        /// A brief description of Service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Service Version. This property can be used to indicate the code version (especially the version of service methods).
        /// This value is sent to user application on an exception, so, user/client application can know that service version is changed.
        /// Default value: NO_VERSION.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Creates a new MDSServiceAttribute object.
        /// </summary>
        public MDSServiceAttribute()
        {
            Version = "NO_VERSION";
        }
    }
}
