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

namespace MDS.Threading
{
    /// <summary>
    /// A delegate to used by QueueProcessorThread to raise processing event
    /// </summary>
    /// <typeparam name="T">Type of the item to process</typeparam>
    /// <param name="sender">The object which raises event</param>
    /// <param name="e">Event arguments</param>
    public delegate void ProcessQueueItemHandler<T>(object sender, ProcessQueueItemEventArgs<T> e);

    /// <summary>
    /// Stores processing item and some informations about queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProcessQueueItemEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The item to process.
        /// </summary>
        public T ProcessItem { get; set; }

        /// <summary>
        /// The item count waiting for processing on queue (after this one).
        /// </summary>
        public int QueuedItemCount { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processItem">The item to process</param>
        /// <param name="queuedItemCount">The item count waiting for processing on queue (after this one)</param>
        public ProcessQueueItemEventArgs(T processItem, int queuedItemCount)
        {
            ProcessItem = processItem;
            QueuedItemCount = queuedItemCount;
        }
    }
}