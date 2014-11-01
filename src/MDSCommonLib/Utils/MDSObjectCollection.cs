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

namespace MDS.Utils
{
    /// <summary>
    /// Represents a thread-safe string-key based object collection.
    /// </summary>
    public class MDSObjectCollection
    {
        /// <summary>
        /// All objects are stored in this collection.
        /// </summary>
        private readonly SortedDictionary<string, object> _objects;

        /// <summary>
        /// Gets the synchronization object that is used by 
        /// </summary>
        public object SyncObj { get; private set; }

        /// <summary>
        /// Contructor.
        /// </summary>
        public MDSObjectCollection()
        {
            SyncObj = new object();
            _objects = new SortedDictionary<string, object>();
        }

        /// <summary>
        /// Gets/sets an object with given key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Object with given key, or null if key does not exists</returns>
        public object this[string key]
        {
            get 
            {
                lock (SyncObj)
                {
                    return _objects.ContainsKey(key) ? _objects[key] : null;
                }
            }

            set
            {
                lock (SyncObj)
                {
                    _objects[key] = value;
                }
            }
        }

        /// <summary>
        /// Removes an object from collection.
        /// </summary>
        /// <param name="key">Key of object to remove</param>
        public void Remove(string key)
        {
            lock (SyncObj)
            {
                _objects.Remove(key);
            }
        }

        /// <summary>
        /// Rewmoves all keys/values from collection.
        /// </summary>
        public void Clear()
        {
            lock (SyncObj)
            {
                _objects.Clear();
            }
        }
    }
}
