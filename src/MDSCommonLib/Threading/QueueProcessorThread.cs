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

using System.Collections;
using System.Threading;

namespace MDS.Threading
{
    /// <summary>
    /// A threaded queue that process only one item in a time and keeps others in a queue.
    /// </summary>
    /// <typeparam name="T">Type of the processing item</typeparam>
    public class QueueProcessorThread<T> : IRunnable
    {
        /// <summary>
        /// This event is used to process get and process an item from queue. When an item inserted this
        /// queue, ProcessItem event is raised.
        /// </summary>
        public event ProcessQueueItemHandler<T> ProcessItem;

        /// <summary>
        /// Queue object to store items.
        /// </summary>
        private readonly Queue _queue;

        /// <summary>
        /// Running thread.
        /// </summary>
        private Thread _thread;
        
        /// <summary>
        /// Thread control flag.
        /// </summary>
        private volatile bool _running;

        /// <summary>
        /// Construnctor.
        /// </summary>
        public QueueProcessorThread()
        {
            _queue = Queue.Synchronized(new Queue());
        }

        /// <summary>
        /// Starts the processing of items. Thread runs, listens and process items on queue.
        /// </summary>
        public void Start()
        {
            lock (_queue.SyncRoot)
            {
                if (_running)
                {
                    return;
                }

                _running = true;
                _thread = new Thread(DoProcess);
                _thread.Start();
            }
        }

        /// <summary>
        /// Stops the processing of items and stops the thread.
        /// </summary>
        /// <param name="waitToStop">True, if caller method must wait until running stops.</param>
        public void Stop(bool waitToStop)
        {
            lock (_queue.SyncRoot)
            {
                if (_running)
                {
                    _running = false;
                    Monitor.PulseAll(_queue.SyncRoot);
                }
            }

            if(waitToStop)
            {
                WaitToStop();
            }
        }

        /// <summary>
        /// Waits stopping of thread, thus waits end of execution of currently processing item.
        /// </summary>
        public void WaitToStop()
        {
            if (_thread == null)
            {
                return;
            }

            try
            {
                _thread.Join();
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// Adds given item to queue to process.
        /// </summary>
        /// <param name="queueItem"></param>
        public void Add(T queueItem)
        {
            lock (_queue.SyncRoot)
            {
                _queue.Enqueue(queueItem);
                Monitor.PulseAll(_queue.SyncRoot);
            }
        }

        /// <summary>
        /// Thread's running method. Listens queue and processes items.
        /// </summary>
        private void DoProcess()
        {
            while (_running)
            {
                var queueItem = default(T);
                var remainingItemCount = 0;
                lock (_queue.SyncRoot)
                {
                    if (_queue.Count > 0)
                    {
                        queueItem = (T)_queue.Dequeue();
                        remainingItemCount = _queue.Count;
                    }
                    else
                    {
                        Monitor.Wait(_queue.SyncRoot);
                    }
                }

                if (!Equals(queueItem, default(T)))
                {
                    OnProcessItem(queueItem, remainingItemCount);
                }
            }

            _thread = null;
        }

        /// <summary>
        /// This method is used to raise ProcessItem event.
        /// </summary>
        /// <param name="queueItem">The item that must be processed</param>
        /// <param name="remainingItemCount">Waiting item count on queue except this one</param>
        protected virtual void OnProcessItem(T queueItem, int remainingItemCount)
        {
            if (ProcessItem == null)
            {
                return;
            }

            try
            {
                ProcessItem(this, new ProcessQueueItemEventArgs<T>(queueItem, remainingItemCount));
            }
            catch
            {

            }
        }
    }
}
