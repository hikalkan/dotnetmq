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

namespace MDS.Settings
{
    /// <summary>
    /// Represents a Route information in settings.
    /// </summary>
    public class RouteInfoItem
    {
        /// <summary>
        /// Name of the Route.
        /// It does not effect routing.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Distribution type/strategy. Must be one of following values:
        /// Sequential: To use Sequential Distribution strategy.
        /// Random: To use Random Distribution strategy.
        /// </summary>
        public string DistributionType { get; set; }

        /// <summary>
        /// Routing filters.
        /// If a message matches one of this filters, it is routed by this route rule.
        /// </summary>
        public List<FilterInfoItem> Filters { get; private set; }

        /// <summary>
        /// Routing destinations.
        /// A messages that is filtered by this rule is redirected one of this destinations according to current distribution strategy.
        /// </summary>
        public List<DestinationInfoItem> Destinations { get; private set; }

        /// <summary>
        /// Creates a new RouteInfoItem object.
        /// </summary>
        public RouteInfoItem()
        {
            Filters = new List<FilterInfoItem>();
            Destinations = new List<DestinationInfoItem>();
        }

        /// <summary>
        /// Represents a Filter information of a Route in settings.
        /// </summary>
        public class FilterInfoItem
        {
            /// <summary>
            /// Source server name for the filter. Must be one of following values:
            /// Empty string or null: No filter on this property.
            /// this: For this/current server.
            /// A server name: For a specified server name.
            /// </summary>
            public string SourceServer { get; set; }

            /// <summary>
            /// Source application name for the filter. Must be one of following values:
            /// Empty string or null: No filter on this property.
            /// An application name: For a specified application name.
            /// </summary>
            public string SourceApplication { get; set; }
            
            /// <summary>
            /// Destination server name for the filter. Must be one of following values:
            /// Empty string or null: No filter on this property.
            /// this: For this/current server.
            /// A server name: For a specified server name.
            /// </summary>
            public string DestinationServer { get; set; }
            
            /// <summary>
            /// Destination application name for the filter. Must be one of following values:
            /// Empty string or null: No filter on this property.
            /// An application name: For a specified application name.
            /// </summary>
            public string DestinationApplication { get; set; }
            
            /// <summary>
            /// Transmit rule for the filter. Must be one of following values:
            /// Empty string or null: No filter on this property.
            /// An element of MessageTransmitRules enum.
            /// </summary>
            public string TransmitRule { get; set; }
        }

        /// <summary>
        /// Represents a Destination information of a Route in settings.
        /// </summary>
        public class DestinationInfoItem
        {
            /// <summary>
            /// Destination server name. Must be one of following values:
            /// Empty string or null: Don't change destination server.
            /// this: Change to this/current server.
            /// A server name: Change to a specified server name.
            /// </summary>
            public string Server { get; set; }

            /// <summary>
            /// Destination application name. Must be one of following values:
            /// Empty string or null: Don't change destination application.
            /// A application name: Change to a specified application name.
            /// </summary>
            public string Application { get; set; }

            /// <summary>
            /// Route factor.
            /// Must be 1 or greater.
            /// </summary>
            public int RouteFactor { get; set; }
        }
    }
}
