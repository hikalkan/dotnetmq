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

namespace MDS.Threading
{
    /// <summary>
    /// This interface is used for a class that can startable and stoppable.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// This method is used to start running.
        /// </summary>
        void Start();

        /// <summary>
        /// This method is used to stop running.
        /// </summary>
        /// <param name="waitToStop">Indicates that caller thread waits stopping of module</param>
        void Stop(bool waitToStop);

        /// <summary>
        /// Joins module's thread until it stops.
        /// </summary>
        void WaitToStop();
    }
}
