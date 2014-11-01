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
    /// Sequential distribution strategy.
    /// According to this strategy, a message is routed one of available destinations sequentially according to destination's RouteFactor.
    /// For example, if,
    /// 
    /// - Destination-A has a RouteFactor of 4
    /// - Destination-B has a RouteFactor of 1
    /// 
    /// Then, 4 messages are sent to Destination-A, 1 message is sent to Destination-B sequentially.
    /// </summary>
    internal class SequentialDistribution : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// Total count of all RouteFactors of Destinations and calculated by Reset method.
        /// </summary>
        private int _totalCount;

        /// <summary>
        /// Current routing number. It is used to determine the next routing destination.
        /// For example, if,
        /// 
        /// - Destination-A has a RouteFactor of 4
        /// - Destination-B has a RouteFactor of 3
        /// - Destination-C has a RouteFactor of 3
        /// 
        /// and _currentNumber is 5, then destination is Destination-B. 
        /// </summary>
        private int _currentNumber;

        /// <summary>
        /// Creates a new SequentialDistribution object.
        /// </summary>
        /// <param name="routingRule">Reference to RoutingRule object that uses this distribution strategy to route messages</param>
        public SequentialDistribution(RoutingRule routingRule)
            : base(routingRule)
        {
            Reset();
        }

        /// <summary>
        /// Initializes and Resets distribution strategy.
        /// </summary>
        public void Reset()
        {
            _totalCount = 0;
            foreach (var destination in RoutingRule.Destinations)
            {
                _totalCount += destination.RouteFactor;
            }
        }

        /// <summary>
        /// Sets the destination of a message according to distribution strategy.
        /// </summary>
        /// <param name="message">Message to set it's destination</param>
        public void SetDestination(MDSDataTransferMessage message)
        {
            //Return, if no destination exists
            if (_totalCount == 0 || RoutingRule.Destinations.Length <= 0)
            {
                return;
            }

            //Find next destination and set
            var currentTotal = 0;
            foreach (var destination in RoutingRule.Destinations)
            {
                currentTotal += destination.RouteFactor;
                if (_currentNumber < currentTotal)
                {
                    SetMessageDestination(message, destination);
                    break;
                }
            }

            //Increase _currentNumber
            if ((++_currentNumber) >= _totalCount)
            {
                _currentNumber = 0;
            }
        }
    }
}
