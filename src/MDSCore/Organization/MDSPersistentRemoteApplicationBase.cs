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
using System.Threading;
using MDS.Communication;
using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Exceptions;
using MDS.Storage;

namespace MDS.Organization
{
    /// <summary>
    /// This class extends MDSRemoteApplication, adds functionality to send/receive persistent messages.
    /// </summary>
    public abstract class MDSPersistentRemoteApplicationBase : MDSRemoteApplication
    {
        #region Consts
        
        /// <summary>
        /// Maximum message count can be stored in _waitingMessages.
        /// </summary>
        private const int MaxMessagesInQueue = 50;

        /// <summary>
        /// If message count in _waitingMessages decrees under this count and there are new messages on database, new messages are received to queue.
        /// This value must be smaller (or equal but not recommended) than MaxMessagesInQueue.
        /// </summary>
        private const int MinMessageCountToGetFromDatabase = 5;

        /// <summary>
        /// In an database, network... error, thread first waits FirstWaitingTimeOnError milliseconds before next try.
        /// </summary>
        private const int FirstWaitingTimeOnError = 1000;

        /// <summary>
        /// In a database, network... error, thread maximum waits MaxWaitingTimeOnError milliseconds before next try.
        /// </summary>
        private const int MaxWaitingTimeOnError = 60000;

        #endregion

        #region Public fields

        /// <summary>
        /// Reference to the Store manager in MDS server.
        /// </summary>
        public IStorageManager StorageManager { get; set; }

        #endregion

        #region Private fields

        /// <summary>
        /// Waiting message list that contains cached messages to be sent to this remote application.
        /// New messages are inserted to storage manager (database) and also added to this queue. Thus, when message is sent to remote application,
        /// it is gooten from this queue instead of database. This improves performance.
        /// When this list has item less than MinMessageCountToGetFromDatabase and database has messages, the list is filled from database.
        /// </summary>
        private readonly LinkedList<WaitingMessage> _waitingMessages;

        /// <summary>
        /// Biggest Id in database that is waiting to be sent to this remote application.
        /// If no records exists, this is last sent message's Id.
        /// </summary>
        private int _biggestWaitingMessageId;

        /// <summary>
        /// Biggest Id in _waitingMessages (Last item's Id).
        /// </summary>
        private int _biggestWaitingMessageIdInList;

        /// <summary>
        /// Runs on ProcessWaitingMessageRecordsAsThread method.
        /// Used to process messages in _waitingMessages.
        /// </summary>
        private Thread _waitingMessageProcessThread;

        /// <summary>
        /// Indicates that _waitingMessageProcessThread is running or not.
        /// </summary>
        private volatile bool _waitingMessageProcessRunning;

        /// <summary>
        /// Waiting time on an error situation.
        /// It is used with FirstWaitingTimeOnError and MaxWaitingTimeOnError to wait before next try when an error occured.
        /// </summary>
        private int _waitingTimeOnError = FirstWaitingTimeOnError;

        /// <summary>
        /// This is true if _waitingMessageProcessThread is waiting because of an error situation.
        /// In this case, it can not be pulsed and waits to finish it's timeout (except service is stopping).
        /// </summary>
        private volatile bool _waitingForAnError;
     
        #endregion

        #region Constructors
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the remote application</param>
        protected MDSPersistentRemoteApplicationBase(string name) :
            base(name, CommunicationLayer.CreateApplicationId())
        {
            _waitingMessages = new LinkedList<WaitingMessage>();
            _biggestWaitingMessageId = 0;
            _biggestWaitingMessageIdInList = 0;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts this remote application.
        /// </summary>
        public override void Start()
        {
            //Lock collection to be synchronized.
            lock (_waitingMessages)
            {
                //Get biggest Id value from database.
                _biggestWaitingMessageId = GetMaxWaitingMessageId();

                //Start _waitingMessageProcessThread
                _waitingMessageProcessRunning = true;
                _waitingMessageProcessThread = new Thread(ProcessWaitingMessageRecordsAsThread);
                _waitingMessageProcessThread.Start();

                //Start base class
                base.Start();
            }
        }

        /// <summary>
        /// Stops this remote application.
        /// </summary>
        /// <param name="waitToStop">This is set to true if called thread wants to wait until this application completely stops</param>
        public override void Stop(bool waitToStop)
        {
            lock (_waitingMessages)
            {
                //Stop base class
                base.Stop(waitToStop);

                //Stop _waitingMessageProcessThread
                _waitingMessageProcessRunning = false;
                Monitor.PulseAll(_waitingMessages);
            }

            if(waitToStop)
            {
                WaitToStop();
            }
        }

        /// <summary>
        /// Waits until this application completely stops.
        /// No action if it is already stopped.
        /// </summary>
        public override void WaitToStop()
        {
            //Wait to stop of base class
            base.WaitToStop();

            //If _waitingMessageProcessThread is null, that means it is already stopped.
            if (_waitingMessageProcessThread == null)
            {
                return;
            }

            try
            {
                //Wait to stop of _waitingMessageProcessThread
                _waitingMessageProcessThread.Join();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method is called when a MDSDataTransferMessage sent to this application.
        /// Stores message and adds to queue to send to remote application.
        /// </summary>
        /// <param name="message">Message</param>
        public void EnqueueMessage(MDSDataTransferMessage message)
        {
            //Create MDSMessageRecord from MDSDataTransferMessage to save message to database
            var messageRecord = new MDSMessageRecord(message) {NextServer = GetNextServerForMessage(message)};

            //Lock collection to be synchronized.
            lock (_waitingMessages)
            {
                if (message.TransmitRule == MessageTransmitRules.StoreAndForward)
                {
                    //Save message to database
                    StorageManager.StoreMessage(messageRecord);
                    /* If these conditions are true, then message also added to _waitingMessages 
                     * and message is sent to appliction from this queue instead of read again from database (for performance reasons):
                     * - _waitingMessages's message count is smaller than a maximum count (_waitingMessages.Count < MaxMessagesInQueue).
                     * - All messages in database is also in _waitingMessages (_biggestWaitingMessageIdInList >= _biggestWaitingMessageId) 
                     *   That means there is no message that is in database but not in _waitingMessages list.
                     */
                    if (_waitingMessages.Count < MaxMessagesInQueue &&
                        _biggestWaitingMessageIdInList >= _biggestWaitingMessageId)
                    {
                        //Add message to queue.
                        _waitingMessages.AddLast(new WaitingMessage(messageRecord));
                        //This message's Id is new biggest id on queue.
                        _biggestWaitingMessageIdInList = messageRecord.Id;
                    }

                    //This message's id is new biggest id in database, so update _biggestWaitingMessageId value
                    _biggestWaitingMessageId = messageRecord.Id;
                }
                else
                {
                    //Add message to queue.
                    _waitingMessages.AddFirst(new WaitingMessage(messageRecord));
                }

                //Pulse waiting thread that is in wait state because of no message to process.
                if (!_waitingForAnError)
                {
                    Monitor.PulseAll(_waitingMessages);
                }

                Logger.Debug("EnqueueMessage - WaitingMessages.Count = " + _waitingMessages.Count + ", Application = " + Name);
            }
        }

        /// <summary>
        /// This method is called when a MDSDataTransferMessage sent to this application.
        /// It does not store message, adds it as first item of sending queue.
        /// </summary>
        /// <param name="message">Message</param>
        public void AddMessageToHeadOfQueue(MDSDataTransferMessage message)
        {
            //Lock collection to be synchronized.
            lock (_waitingMessages)
            {
                //Add message to queue.
                _waitingMessages.AddFirst(
                    new WaitingMessage(
                        new MDSMessageRecord(message)
                            {
                                NextServer = GetNextServerForMessage(message),
                                Id = -1
                            }));

                //Pulse waiting thread that is in wait state because of no message to process.
                if (!_waitingForAnError)
                {
                    Monitor.PulseAll(_waitingMessages);
                }              

                Logger.Debug("AddMessageToHeadOfQueue - WaitingMessages.Count = " + _waitingMessages.Count + ", Application = " + Name);
            }
        }

        #endregion

        #region Protected methods
        
        /// <summary>
        /// This method handles ACK/Reject messages from remote application for a data transfer message.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="operationResultMessage">Response message</param>
        protected override void OnResponseReceived(ICommunicator communicator, MDSOperationResultMessage operationResultMessage)
        {
            base.OnResponseReceived(communicator, operationResultMessage);
            EvaluateResponse(communicator, operationResultMessage, null);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method is run by _waitingMessageProcessThread and ensures persistence with EnqueueMessage method.
        /// </summary>
        private void ProcessWaitingMessageRecordsAsThread()
        {
            Logger.Debug("MDSPersistentRemoteApplicationBase - ProcessWaitingMessage thread is started. ApplicationId = " + ApplicationId);

            //Loop until this remote application stops (by Stop method)
            while (_waitingMessageProcessRunning)
            {
                try
                {
                    ProcessWaitingMessageRecords();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }

            Logger.Debug("MDSPersistentRemoteApplicationBase - ProcessWaitingMessage thread is stopped. ApplicationId = " + ApplicationId);

            _waitingMessageProcessThread = null;
        }

        /// <summary>
        /// This is the main method that is called in ProcessWaitingMessageRecordsAsThread method.
        /// In basic, it gets a record from queue/database and sends it to the remote application.
        /// </summary>
        private void ProcessWaitingMessageRecords()
        {
            try
            {
                WaitingMessage waitingMessage;
                
                //Lock collection to be synchronized.
                lock (_waitingMessages)
                {
                    GetMessagesFromDatabaseIfNeeded();
                    waitingMessage = GetFirstReadyMessageToSend();
                    if (waitingMessage == null)
                    {
                        //Wait for a new incoming message pulse (by StoreAndEnqueue or AddMessageToHeadOfQueue methods) or undelivered message pulse (by OnResponseReceived method)
                        Monitor.Wait(_waitingMessages);
                    }

                    Logger.Debug("ProcessWaitingMessageRecords - WaitingMessages.Count = " + _waitingMessages.Count + ", Application = " + Name);
                }

                //If a message is gotten from queue then process it
                if (waitingMessage != null)
                {
                    ProcessWaitingMessage(waitingMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                WaitOnError();
                return;
            }
        }

        /// <summary>
        /// Gets a message from _waitingMessages that is ready to send (not processing by a communicator and waiting for ACK). 
        /// </summary>
        /// <returns>A message that is ready to send. Returns null if there is no message to send</returns>
        private WaitingMessage GetFirstReadyMessageToSend()
        {
            for (var waitingMessage = _waitingMessages.First; waitingMessage != null; waitingMessage = waitingMessage.Next )
            {
                if (waitingMessage.Value.State == WaitingMessage.WaitingMessageStates.ReadyToSend)
                {
                    return waitingMessage.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets messages from database if needed.
        /// </summary>
        private void GetMessagesFromDatabaseIfNeeded()
        {
            /* Messages are gotten from database if both of these conditions are true:
             * - Messages exists in database that are not in (_waitingMessages) queue (_biggestWaitingMessageId > _biggestWaitingMessageIdInList)
             * - Message count in (_waitingMessages) list is smaller than a minimum amount (_waitingMessages.Count < MinMessageCountToGetFromDatabase)
             */
            if (_biggestWaitingMessageId > _biggestWaitingMessageIdInList && _waitingMessages.Count < MinMessageCountToGetFromDatabase)
            {
                //Get messages from database
                var newRecords = GetWaitingMessages(_biggestWaitingMessageIdInList + 1, MaxMessagesInQueue - _waitingMessages.Count);
                foreach (var newRecord in newRecords)
                {
                    //Add message to queue
                    _waitingMessages.AddLast(new WaitingMessage(newRecord));
                    //Update biggest Id on queue
                    _biggestWaitingMessageIdInList = newRecord.Id;
                }
            }
        }

        /// <summary>
        /// Process one MDSMessageRecord from queue.
        /// Called by ProcessWaitingMessageRecords method for each message.
        /// If waitingMessage.MessageRecord.Id > 0 that means the message is asyncronous (added to queue with EnqueueMessage and stored to database),
        /// else the message is syncronous (added to queue with AddMessageToHeadOfQueue method).
        /// Asyncronous messages will be tried again and again until it is delivered, 
        /// but syncronsous messages is tried only one time to send, because sender application waits response for it.
        /// </summary>
        /// <param name="waitingMessage">Message to process</param>
        private void ProcessWaitingMessage(WaitingMessage waitingMessage)
        {
            try
            {
                waitingMessage.State = WaitingMessage.WaitingMessageStates.WaitingForAcknowledgment;
                //If message is stored to database (waitingMessage.MessageRecord.Id > 0)...
                if (waitingMessage.MessageRecord.Id >= 0)
                {
                    //Set DestinationCommunicatorId = 0, because this field can not be used on persistent messages.
                    waitingMessage.MessageRecord.Message.DestinationCommunicatorId = 0;
                    //No timeout value, so, wait a free communicator for infinity
                    SendDataMessage(waitingMessage.MessageRecord.Message, 0);
                }
                else
                {
                    SendDataMessage(waitingMessage.MessageRecord.Message, Settings.MessageResponseTimeout);
                }
            }
            catch (MDSNoCommunicatorException ex)
            {
                Logger.Warn(ex.Message, ex);
                EvaluateResponse(null, new MDSOperationResultMessage
                                           {
                                               RepliedMessageId = waitingMessage.MessageRecord.MessageId,
                                               ResultText = ex.Message,
                                               Success = false
                                           }, waitingMessage);
                //If it was stored message, stop processing messages until a receiver communicator is connected
                if (waitingMessage.MessageRecord.Id >= 0)
                {
                    WaitUntilReceiverCommunicatorConnected();
                }
            }
            catch (MDSTimeoutException ex)
            {
                Logger.Warn(ex.Message, ex);
                EvaluateResponse(null, new MDSOperationResultMessage
                                           {
                                               RepliedMessageId = waitingMessage.MessageRecord.MessageId,
                                               ResultText = ex.Message,
                                               Success = false
                                           }, waitingMessage);
                //If it was stored message, wait a while before processing messages
                if (waitingMessage.MessageRecord.Id >= 0)
                {
                    WaitOnError();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                EvaluateResponse(null, new MDSOperationResultMessage
                                           {
                                               RepliedMessageId = waitingMessage.MessageRecord.MessageId,
                                               ResultText = ex.Message,
                                               Success = false
                                           }, waitingMessage);
                //If it was stored message, wait a while before processing messages
                if (waitingMessage.MessageRecord.Id >= 0)
                {
                    WaitOnError();
                }
            }
        }

        /// <summary>
        /// This method evaluates ACK/Reject messages from remote application for a data transfer message.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="operationResultMessage">Response message</param>
        /// <param name="waitingMessage">Waiting message for this response (may be null, if it is null, it is searched in _waitingMessages list)</param>
        private void EvaluateResponse(ICommunicator communicator, MDSOperationResultMessage operationResultMessage, WaitingMessage waitingMessage)
        {
            lock (_waitingMessages)
            {
                //If waitingMessage argument is not supplied, find waiting message from _waitingMessages list
                if (waitingMessage == null)
                {
                    foreach (var wMessage in _waitingMessages)
                    {
                        if (wMessage.State == WaitingMessage.WaitingMessageStates.WaitingForAcknowledgment &&
                            wMessage.MessageRecord.MessageId == operationResultMessage.RepliedMessageId)
                        {
                            waitingMessage = wMessage;
                            break;
                        }
                    }

                    //If there is not message that waits for this response, just exit from method..
                    if (waitingMessage == null)
                    {
                        return;
                    }
                }

                //If message is stored to database in this server
                if (waitingMessage.MessageRecord.Id >= 0)
                {
                    if (operationResultMessage.Success)
                    {
                        //If message is persistent (Stored on database), remove from database.
                        if (waitingMessage.MessageRecord.Id > 0)
                        {
                            StorageManager.RemoveMessage(waitingMessage.MessageRecord.Id);
                        }

                        //Remove from list
                        _waitingMessages.Remove(waitingMessage);
                        //Reset _waitingTimeOnError (because this is a success situation)
                        _waitingTimeOnError = FirstWaitingTimeOnError;
                    }
                    else
                    {
                        //Set message state as 'ready to send' again. Thus, message is added to send queue again.
                        waitingMessage.State = WaitingMessage.WaitingMessageStates.ReadyToSend;
                        //Pulse if a thread waiting in ProcessWaitingMessageRecords method
                        Monitor.PulseAll(_waitingMessages);
                    }
                }
                //If message is not stored to database in this server
                else
                {
                    //Remove from list
                    _waitingMessages.Remove(waitingMessage);
                }

                Logger.Debug("EvaluateResponse - WaitingMessages.Count = " + _waitingMessages.Count + ", Application = " + Name);
            }

            //If it is not stored message..
            if (waitingMessage.MessageRecord.Id < 0)
            {
                //Create event to send response message to upper layers
                OnMessageReceived(this, new MessageReceivedFromCommunicatorEventArgs
                                        {
                                            Communicator = communicator,
                                            Message = operationResultMessage
                                        });
            }
        }
        
        /// <summary>
        /// Stops _waitingMessageProcessThread thread for a while on an error situation.
        /// </summary>
        private void WaitOnError()
        {
            _waitingForAnError = true;
            try
            {
                lock (_waitingMessages)
                {
                    //Calculating waiting time
                    var waitingTime = Math.Min(_waitingTimeOnError, MaxWaitingTimeOnError);
                    //Calculate waiting time on next error
                    _waitingTimeOnError = Math.Min(waitingTime * 2, MaxWaitingTimeOnError);
                    //Wait..
                    Monitor.Wait(_waitingMessages, waitingTime);
                }
            }
            finally
            {
                _waitingForAnError = false;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Gets messages from database to be sent to this remote application.
        /// </summary>
        /// <param name="minId">Minimum Id of message record to get (minId included)</param>
        /// <param name="maxCount">Maximum number of records to get</param>
        /// <returns>List of messages</returns>
        protected abstract List<MDSMessageRecord> GetWaitingMessages(int minId, int maxCount);

        /// <summary>
        /// Gets Id of last incoming message that will be sent to this remote application.
        /// </summary>
        /// <returns>Id of last incoming message</returns>
        protected abstract int GetMaxWaitingMessageId();

        /// <summary>
        /// Finds Next server for a message.
        /// This method is designed, because it is different to get next server's name for client applications and mds servers.
        /// </summary>
        /// <returns>Next server</returns>
        protected abstract string GetNextServerForMessage(MDSDataTransferMessage message);

        #endregion

        #region Sub classes

        /// <summary>
        /// This class represents a message in _waitingMessages list.
        /// </summary>
        private class WaitingMessage
        {
            /// <summary>
            /// Message record in storage manager.
            /// </summary>
            public MDSMessageRecord MessageRecord { get; private set; }

            /// <summary>
            /// State of the message.
            /// </summary>
            public WaitingMessageStates State { get; set; }

            /// <summary>
            /// States of a messages.
            /// </summary>
            public enum WaitingMessageStates
            {
                /// <summary>
                /// This message is waiting to be sent to remote application.
                /// </summary>
                ReadyToSend,

                /// <summary>
                /// This message is sent to remote application and waiting an ACK message to be removed from _waitingMessages list.
                /// </summary>
                WaitingForAcknowledgment
            }

            /// <summary>
            /// Creates a new WaitingMessage object.
            /// </summary>
            /// <param name="messageRecord">Message record in storage manager</param>
            public WaitingMessage(MDSMessageRecord messageRecord)
            {
                MessageRecord = messageRecord;
                State = WaitingMessageStates.ReadyToSend;
            }
        }

        #endregion
    }
}
