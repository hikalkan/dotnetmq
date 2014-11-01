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


using MDS.Communication.Events;

namespace MDS.Communication
{
    /// <summary>
    /// Base class for communicator managers.
    /// </summary>
    public abstract class CommunicationManagerBase : ICommunicationManager
    {
        /// <summary>
        /// This event is raised when a communicator connection established.
        /// </summary>
        public event CommunicatorConnectedHandler CommunicatorConnected;

        /// <summary>
        /// Starts communication manager.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops communication manager.
        /// </summary>
        /// <param name="waitToStop">Indicates that caller thread waits stopping of manager</param>
        public abstract void Stop(bool waitToStop);

        /// <summary>
        /// Waits stopping of communication manager.
        /// </summary>
        public abstract void WaitToStop();

        /// <summary>
        /// Raises CommunicatorConnected event.
        /// </summary>
        /// <param name="communicator">Communicator</param>
        protected void OnCommunicatorConnected(ICommunicator communicator)
        {
            if (CommunicatorConnected != null)
            {
                CommunicatorConnected(this, new CommunicatorConnectedEventArgs {Communicator = communicator});
            }
        }
    }
}
