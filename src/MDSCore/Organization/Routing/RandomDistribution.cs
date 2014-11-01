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
using MDS.Communication.Messages;

namespace MDS.Organization.Routing
{
    /// <summary>
    /// Random distribution strategy.
    /// According to this strategy, a message is routed one of available destinations randomly according to destination's RouteFactor.
    /// For example, if,
    /// 
    /// - Destination-A has a RouteFactor of 4
    /// - Destination-B has a RouteFactor of 1
    /// 
    /// Then, probability of routing a message to destinations:
    /// 
    /// - Destination-A -> 80%
    /// - Destination-B -> 20%
    /// </summary>
    internal class RandomDistribution : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// A Random object to create random numbers.
        /// </summary>
        private readonly Random _rnd;

        /// <summary>
        /// Maximum count to create random numbers.
        /// This is the total count of all RouteFactors of Destinations and calculated by Reset method.
        /// </summary>
        private int _maxCount;

        /// <summary>
        /// Creates a new RandomDistribution object.
        /// </summary>
        /// <param name="routingRule">Reference to RoutingRule object that uses this distribution strategy to route messages</param>
        public RandomDistribution(RoutingRule routingRule)
            : base(routingRule)
        {
            _rnd = new Random();
            Reset();
        }

        /// <summary>
        /// Initializes and Resets distribution strategy.
        /// </summary>
        public void Reset()
        {
            _maxCount = 0;
            foreach (var destination in RoutingRule.Destinations)
            {
                _maxCount += destination.RouteFactor;
            }
        }

        /// <summary>
        /// Sets the destination of a message according to distribution strategy.
        /// </summary>
        /// <param name="message">Message to set it's destination</param>
        public void SetDestination(MDSDataTransferMessage message)
        {
            //Return, if no destination exists
            if (_maxCount == 0 || RoutingRule.Destinations.Length <= 0)
            {
                return;
            }

            //Create a random number
            var randomNumber = _rnd.Next(_maxCount);

            //Find destination according to random number and set the destination.
            var currentTotal = 0;
            foreach (var destination in RoutingRule.Destinations)
            {
                currentTotal += destination.RouteFactor;
                if (randomNumber < currentTotal)
                {
                    SetMessageDestination(message, destination);
                    return;
                }
            }
        }
    }
}
