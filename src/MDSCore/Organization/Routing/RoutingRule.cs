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

using MDS.Communication.Messages;

namespace MDS.Organization.Routing
{
    /// <summary>
    /// Represents a routing rule.
    /// </summary>
    public class RoutingRule
    {
        /// <summary>
        /// Name of the Route.
        /// It does not effect routing.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets Route distribution type.
        /// Sequential: To use SequentialDistribution strategy.
        /// Random: To use RandomDistribution strategy.
        /// </summary>
        public string DistributionType { get; set; }

        /// <summary>
        /// Routing filters.
        /// If a message matches one of this filters, it is routed by this RouteRule.
        /// </summary>
        public RoutingFilter[] Filters { get; set; }

        /// <summary>
        /// Routing destinations.
        /// A messages that is filtered by this rule is redirected one of this destinations according to current distribution strategy.
        /// </summary>
        public RoutingDestination[] Destinations { get; set; }

        /// <summary>
        /// Gets the current distribution strategy.
        /// </summary>
        private IDistributionStrategy DistributionStrategy
        {
            get
            {
                if (_distributionStrategy == null)
                {
                    switch (DistributionType)
                    {
                        case "Random":
                            _distributionStrategy = new RandomDistribution(this);
                            break;
                        //case "Sequential":
                        default:
                            _distributionStrategy = new SequentialDistribution(this);
                            break;
                    }
                }

                return _distributionStrategy;
            }
        }
        private IDistributionStrategy _distributionStrategy;

        /// <summary>
        /// Tries to appliy this rule to a data transfer message.
        /// </summary>
        /// <param name="message">Message to apply rule</param>
        /// <returns>True, if rule applied</returns>
        public bool ApplyRule(MDSDataTransferMessage message)
        {
            for (var i = 0; i < Filters.Length; i++)
            {
                if (Filters[i].Matches(message))
                {
                    DistributionStrategy.SetDestination(message);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a string that presents a brief information about RoutingRule object.
        /// </summary>
        /// <returns>Brief information about RoutingRule object</returns>
        public override string ToString()
        {
            return string.Format("Rule: {0} (DistributionType: {1}, Filter Count: {2}, Destination Count: {3}).", Name, DistributionType, Filters.Length, Destinations.Length);
        }
    }
}
