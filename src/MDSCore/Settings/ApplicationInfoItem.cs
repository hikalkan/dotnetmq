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

using System.Collections.Generic;
using System.Collections.Specialized;

namespace MDS.Settings
{
    /// <summary>
    /// Represents a Client Application's informations in settings.
    /// </summary>
    public class ApplicationInfoItem
    {
        /// <summary>
        /// Name of this server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Predefined communication channels.
        /// </summary>
        public List<CommunicationChannelInfoItem> CommunicationChannels { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApplicationInfoItem()
        {
            CommunicationChannels = new List<CommunicationChannelInfoItem>();
        }

        /// <summary>
        /// Represents a predefined Communication channel for an Application.
        /// </summary>
        public class CommunicationChannelInfoItem
        {
            /// <summary>
            /// Type of communicaton. Can be WebService.
            /// </summary>
            public string CommunicationType { get; set; }

            /// <summary>
            /// Settings for communication. For example, includes Url info if CommunicationType is WebService.
            /// </summary>
            public NameValueCollection CommunicationSettings { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public CommunicationChannelInfoItem()
            {
                CommunicationSettings = new NameValueCollection();
            }
        }
    }
}
