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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using MDS.Exceptions;

namespace MDS.Storage
{
    /// <summary>
    /// This class adds fault tolerance improvements to a Storage Manager.
    /// Tries database operations more than once until specified timeout occurs. 
    /// </summary>
    public class FaultToleratedStorageManagerWrapper : IStorageManager
    {
        #region Public properties

        /// <summary>
        /// Timeout value to cancel trying operation again (as milliseconds).
        /// Default: 90 seconds (90000 ms).
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// This value determines th time to wait before retry on an exception (as milliseconds).
        /// Default: 1 seconds (1000 ms).
        /// </summary>
        public int WaitTimeBeforeRetry { get; set; }

        /// <summary>
        /// If this is true, StorageManager is restarted (Stop, Start) after an error, before next try.
        /// Default: true
        /// </summary>
        public bool RestartStorageManagerOnException { get; set; }
        
        #endregion

        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Original Storage Manager to perform database operations.
        /// </summary>
        private readonly IStorageManager _storageManager;

        /// <summary>
        /// The last restart time of storage manager.
        /// This is used to prevent very frequently restarts.
        /// </summary>
        private DateTime _lastRestartTime;
        
        /// <summary>
        /// This object is used to synchronizing threads.
        /// </summary>
        private readonly object _syncObj = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new FaultToleratedStorageManagerWrapper, wraps a IStorageManager.
        /// </summary>
        /// <param name="storageManager">Original Storage Manager to perform database operations</param>
        public FaultToleratedStorageManagerWrapper(IStorageManager storageManager)
        {
            if (storageManager == null)
            {
                throw new ArgumentNullException("storageManager", "storageManager parameter can not be null.");
            }

            _storageManager = storageManager;
            _lastRestartTime = DateTime.MinValue;
            TimeOut = 90000;
            WaitTimeBeforeRetry = 1000;
            RestartStorageManagerOnException = true;
        }

        #endregion

        #region IStorageManager members

        public void Start()
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    _storageManager.Start();
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public void Stop(bool waitToStop)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    _storageManager.Stop(waitToStop);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public void WaitToStop()
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    _storageManager.WaitToStop();
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public int StoreMessage(MDSMessageRecord messageRecord)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.StoreMessage(messageRecord);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public List<MDSMessageRecord> GetWaitingMessagesOfApplication(string nextServer, string destApplication, int minId, int maxCount)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.GetWaitingMessagesOfApplication(nextServer, destApplication, minId, maxCount);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public int GetMaxWaitingMessageIdOfApplication(string nextServer, string destApplication)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.GetMaxWaitingMessageIdOfApplication(nextServer, destApplication);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public List<MDSMessageRecord> GetWaitingMessagesOfServer(string nextServer, int minId, int maxCount)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.GetWaitingMessagesOfServer(nextServer, minId, maxCount);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public int GetMaxWaitingMessageIdOfServer(string nextServer)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.GetMaxWaitingMessageIdOfServer(nextServer);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public int RemoveMessage(int id)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    return _storageManager.RemoveMessage(id);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        public void UpdateNextServer(string destServer, string nextServer)
        {
            var startTime = DateTime.Now;
            Exception lastException = null;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < TimeOut)
            {
                try
                {
                    _storageManager.UpdateNextServer(destServer, nextServer);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    lastException = ex;
                    Thread.Sleep(WaitTimeBeforeRetry);
                    RestartStorageManager();
                }
            }

            throw new MDSDatabaseException("Can not performed a database operation.", lastException);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Restarts Storage Manager if RestartStorageManagerOnException is true.
        /// </summary>
        private void RestartStorageManager()
        {
            if (!RestartStorageManagerOnException)
            {
                return;
            }

            //Locked _syncObj to make restart by only one thread in a time
            lock (_syncObj)
            {
                //Do not restart if already restarted in last 1 minute
                if(DateTime.Now.Subtract(_lastRestartTime).TotalSeconds < 60)
                {
                    return;
                }

                try
                {
                    _storageManager.Stop(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                }

                try
                {
                    _storageManager.Start();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                }

                //Update last restart time
                _lastRestartTime = DateTime.Now;
            }
        }

        #endregion
    }
}
