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

namespace MDS.Communication
{
    /// <summary>
    /// This class stores some consts used in MDS.
    /// </summary>
    public sealed class CommunicationConsts
    {
        /// <summary>
        /// Default IP address to connect to MDS server.
        /// </summary>
        public const string DefaultIpAddress = "127.0.0.1";

        /// <summary>
        /// Default listening port of MDS server.
        /// </summary>
        public const int DefaultMDSPort = 10905;

        /// <summary>
        /// Maximum message data length.
        /// </summary>
        public const uint MaxMessageSize = 52428800; //50 MegaBytes
    }
}
