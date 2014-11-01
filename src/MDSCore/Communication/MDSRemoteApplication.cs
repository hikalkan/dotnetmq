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
using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Exceptions;
using MDS.Settings;
using MDS.Threading;

namespace MDS.Communication
{
    /// <summary>
    /// This is a base class for remote application connected to the server as a MDSAdjacentServer,
    /// MDSClientApplication or MDSController.
    /// </summary>
    public abstract partial class MDSRemoteApplication : IRunnable
    {
        #region Events

        /// <summary>
        /// This event is raised when a message is received from remote application.
        /// </summary>
        public event MessageReceivedFromRemoteApplicationHandler MessageReceived;

        /// <summary>
        /// This event is raised when a new communicator connection established correctly.
        /// </summary>
        public event CommunicatorConnectedHandler CommunicatorConnected;

        /// <summary>
        /// This event is raised when a communicator connection closed.
        /// </summary>
        public event CommunicatorDisconnectedHandler CommunicatorDisconnected;

        #endregion

        #region Public properties

        /// <summary>
        /// Reference to settings.
        /// </summary>
        public MDSSettings Settings { set; protected get; }
        
        /// <summary>
        /// An unique ID for this remote application in this server.
        /// </summary>
        public int ApplicationId { get; private set; }

        /// <summary>
        /// Name of the remote application
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Communicator/Application Type for this remote application.
        /// </summary>
        public abstract CommunicatorTypes CommunicatorType { get; }

        /// <summary>
        /// Time of last message received from remote application.
        /// </summary>
        public DateTime LastIncomingMessageTime { get; set; }

        /// <summary>
        /// Time of last message sent to remote application.
        /// </summary>
        public DateTime LastOutgoingMessageTime { get; set; }

        /// <summary>
        /// MessageId of last received and acknowledged message's Id.
        /// This field is used to do not receive/accept same message again.
        /// If a message is send by this remote application with same MessageId,
        /// message is discarded and ACK message is sent to application.
        /// This field is set and used by OrganizationLayer. (Note: It is better to move this field another class in upper layer, because it is used by upper layer only.)
        /// </summary>
        public string LastAcknowledgedMessageId { get; set; }

        /// <summary>
        /// Gets connected (online) communicator count for this remote application.
        /// </summary>
        public int ConnectedCommunicatorCount
        {
            get
            {
                lock (_communicators)
                {
                    return _communicators.Count;
                }
            }
        }        
        
        #endregion

        #region Protected/Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This field is used to determine maximum allowed communicator count.
        /// No more communicator added if communicator count is equal to this number.
        /// For infinit communicator, returns -1;
        /// </summary>
        protected virtual int MaxAllowedCommunicatorCount { get { return -1; } }

        /// <summary>
        /// The connected communication channels associated to this remote application
        /// </summary>
        private readonly LinkedList<ConnectedCommunicator> _communicators;

        /// <summary>
        /// This queue is used to queue MDSMessage objects received from remote application and process them sequentially.
        /// </summary>
        private readonly QueueProcessorThread<MessageReceivedFromCommunicatorEventArgs> _incomingMessageQueue;

        /// <summary>
        /// This object is used to send/deliver messages to remote applications.
        /// </summary>
        private readonly MessageDeliverer _messageDeliverer;

        /// <summary>
        /// This flag is used to start/stop MessageDeliverer.
        /// </summary>
        private volatile bool _running;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Contructor.
        /// </summary>
        protected MDSRemoteApplication(string name, int applicationId)
        {
            Name = name;
            ApplicationId = applicationId;
            LastIncomingMessageTime = DateTime.MinValue;
            LastOutgoingMessageTime = DateTime.MinValue;
            _communicators = new LinkedList<ConnectedCommunicator>();
            _messageDeliverer = new MessageDeliverer(this);
            _incomingMessageQueue = new QueueProcessorThread<MessageReceivedFromCommunicatorEventArgs>();
            _incomingMessageQueue.ProcessItem += IncomingMessageQueue_ProcessItem;
        }

        #endregion

        #region Public methods

        #region Starting / Stopping methods

        /// <summary>
        /// Starts processing of incoming messages.
        /// </summary>
        public virtual void Start()
        {
            _running = true;
            StartCommunicators();
            _messageDeliverer.Start();
            _incomingMessageQueue.Start();
        }

        /// <summary>
        /// Stops processing incoming messages
        /// </summary>
        /// <param name="waitToStop">True, to wait until stopped</param>
        public virtual void Stop(bool waitToStop)
        {
            _running = false;

            //Stop processing new incoming messages
            _incomingMessageQueue.Stop(waitToStop);

            //Stop communicator objects
            StopCommunicators();

            //Pulses WaitUntilReceiverCommunicatorConnected (waiting for a communicator)
            //and MessageDeliverer.SendDataMessage (waiting for available receiver) methods.
            //They must no wait anymore, since MDS is shutting down.
            lock (_communicators)
            {
                Monitor.PulseAll(_communicators);
            }

            //Stop message deliverer
            _messageDeliverer.Stop(waitToStop);
        }

        /// <summary>
        /// Waits until this object is stopped correctly.
        /// </summary>
        public virtual void WaitToStop()
        {
            _incomingMessageQueue.WaitToStop();
            _messageDeliverer.WaitToStop();
        }

        #endregion

        #region Communicator methods

        /// <summary>
        /// Adds a new communication channel for this application.
        /// </summary>
        /// <param name="communicator">new communicator</param>
        public void AddCommunicator(ICommunicator communicator)
        {
            lock (_communicators)
            {
                //Check if communicator count reached to maximum
                var maxAllowedCommunicatorCount = MaxAllowedCommunicatorCount;
                if((maxAllowedCommunicatorCount >=0) && (_communicators.Count >= maxAllowedCommunicatorCount))
                {
                    throw new MDSException("New communicator connection is not allowed, because Communicator count reached to maximum communicator count (" + maxAllowedCommunicatorCount + ")");
                }

                var connectedCommunicator = FindCommunicator(communicator);
                if (connectedCommunicator != null)
                {
                    return;
                }

                //Add to list
                _communicators.AddLast(new ConnectedCommunicator(communicator));

                //Update incoming/outgoing message dates
                LastIncomingMessageTime = DateTime.Now;
                LastOutgoingMessageTime = DateTime.Now;

                //Register to events of communicator
                communicator.MessageReceived += Communicator_MessageReceived;
                communicator.StateChanged += Communicator_StateChanged;

                //Pulse threads that are waiting for a communicator
                Monitor.PulseAll(_communicators);
                
                Logger.Info("Connection established with Remote Application: " + Name);
            }

            OnCommunicatorConnected(communicator);
        }

        /// <summary>
        /// Searches throught _communicators list and checks if it contains a record with a spesified communicator object.
        /// If it contains, returns the ConnectedCommunicator object, else returns null.
        /// </summary>
        /// <param name="communicator">Communicator object to search</param>
        /// <returns>ConnectedCommunicator object, if _communicators contains communicator object</returns>
        private ConnectedCommunicator FindCommunicator(ICommunicator communicator)
        {
            lock (_communicators)
            {
                foreach (var communicatorItem in _communicators)
                {
                    if (communicatorItem.Communicator == communicator)
                    {
                        return communicatorItem;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Message sending methods

        /// <summary>
        /// This method is used to send a MDSDataTransferMessage to an available communicator of application.
        /// It just blocks caller thread until a communicator comes available and message is sent or until timeout,
        /// but receives result (ACK/Reject) message asynchronous. It sends result (ACK/Reject) message to OnResponseReceived() method.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="timeOut">Timeout value to wait if all receivers are busy. Set 0 to do not use timeout.</param>
        public void SendDataMessage(MDSDataTransferMessage message, int timeOut)
        {
            _messageDeliverer.SendDataMessage(message, timeOut);
        }

        /// <summary>
        /// Sends a MDSMessage to this application.
        /// This method does not block calling thread to wait an ACK for sending message.
        /// This method is just an overload for SendMessage(MDSMessage, ICommunicator) method as communicator is null.
        /// </summary>
        /// <param name="message">outgoing message</param>
        public void SendMessage(MDSMessage message)
        {
            SendMessage(message, null);
        }

        /// <summary>
        /// Sends a MDSMessage to a spesific communicator of this application.
        /// This method does not block calling thread to wait an ACK for sending message.
        /// If communicator is null, then it sends message first communicator of receiver communicators.
        /// </summary>
        /// <param name="message">outgoing message</param>
        /// <param name="communicator">Communicator to send message (may be null)</param>
        public void SendMessage(MDSMessage message, ICommunicator communicator)
        {
            _messageDeliverer.SendMessage(message, communicator);
        }

        #endregion

        #endregion

        #region Protected methods

        /// <summary>
        /// This method is used to wait a thread until a receiver communicatior connection established.
        /// </summary>
        protected void WaitUntilReceiverCommunicatorConnected()
        {
            lock (_communicators)
            {
                if (_running && (!_messageDeliverer.IsThereAnyReceiver()))
                {
                    Monitor.Wait(_communicators);
                }
            }
        }

        /// <summary>
        /// Checks if there is any connected communicator exists (at least 1 connected communicator).
        /// </summary>
        /// <returns>True, if there is one communicator at least</returns>
        protected bool IsThereCommunicator()
        {
            lock (_communicators)
            {
                return (_communicators.Count > 0);
            }
        }

        /// <summary>
        /// Gets a list of all connected receiver communicators.
        /// </summary>
        /// <returns>All receiver communicators</returns>
        protected List<ICommunicator> GetAllReceiverCommunicators()
        {
            var receivers = new List<ICommunicator>();
            lock (_communicators)
            {
                foreach (var connectedCommunicator in _communicators)
                {
                    if (connectedCommunicator.Communicator.CommunicationWay == CommunicationWays.SendAndReceive)
                    {
                        receivers.Add(connectedCommunicator.Communicator);
                    }
                }
            }

            return receivers;
        }

        #region Virtual methods

        protected virtual void OnResponseReceived(ICommunicator communicator, MDSOperationResultMessage operationResultMessage)
        {
            //No action
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        /// When a communicator's state is changed, this method handles event..
        /// </summary>
        /// <param name="sender">Creator of event</param>
        /// <param name="e">Event arguments</param>
        private void Communicator_StateChanged(object sender, CommunicatorStateChangedEventArgs e)
        {
            //Process only CommunicationStates.Closed state
            if (e.Communicator.State != CommunicationStates.Closed)
            {
                return;
            }

            lock (_communicators)
            {
                var connectedCommunicator = FindCommunicator(e.Communicator);
                if (connectedCommunicator == null)
                {
                    return;
                }

                _communicators.Remove(connectedCommunicator);

                //Send Reject message for processing message, because communicator is disconnected
                if (connectedCommunicator.ProcessingMessage != null)
                {
                    OnResponseReceived(
                        null,
                        new MDSOperationResultMessage
                            {
                                RepliedMessageId = connectedCommunicator.ProcessingMessage.MessageId,
                                Success = false,
                                ResultText = "Communicator of remote application disconnected from server."
                            });
                }

                Logger.Info("A connection closed with remote application: " + Name);
            }

            OnCommunicatorDisconnected(e.Communicator);
        }

        /// <summary>
        /// When a communicator is received a message, this method handles event..
        /// </summary>
        /// <param name="sender">Creator of event</param>
        /// <param name="e">Event arguments</param>
        private void Communicator_MessageReceived(object sender, MessageReceivedFromCommunicatorEventArgs e)
        {
            //Update last incoming message time
            LastIncomingMessageTime = DateTime.Now;

            //Check if this is an ACK/Reject message for a data transfer message
            if ((e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSOperationResultMessage) && 
                (!string.IsNullOrEmpty(e.Message.RepliedMessageId)))
            {
                ProcessOperationResultMessage(e);
                return;
            }

            //Check if this is an MDSChangeCommunicationWayMessage
            if (e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSChangeCommunicationWayMessage)
            {
                ProcessChangeCommunicationWayMessage(e);
                return;
            }

            //Add message to incoming message queue to process as ordered
            _incomingMessageQueue.Add(e);
        }

        /// <summary>
        /// Processes a MDSOperationResultMessage message.
        /// </summary>
        /// <param name="e">Event arguments from Communicator_MessageReceived method</param>
        private void ProcessOperationResultMessage(MessageReceivedFromCommunicatorEventArgs e)
        {
            //Send message to message deliverer to process
            var handled = _messageDeliverer.HandleOperationResultMessage(e);
            /* If message is handled, OnResponseReceived event is raised, 
             * thus, caller of SendDataMessage (MDSPersistentRemoteApplicationBase.ProcessWaitingMessage) method 
             * gets response. */
            if (handled)
            {
                OnResponseReceived(e.Communicator, e.Message as MDSOperationResultMessage);
            }
        }

        /// <summary>
        /// Processes a MDSChangeCommunicationWayMessage message.
        /// </summary>
        /// <param name="e">Event arguments from Communicator_MessageReceived method</param>
        private static void ProcessChangeCommunicationWayMessage(MessageReceivedFromCommunicatorEventArgs e)
        {
            var message = e.Message as MDSChangeCommunicationWayMessage;
            if (message == null)
            {
                return;
            }

            //Change communication way
            e.Communicator.CommunicationWay = message.NewCommunicationWay;
        }

        /// <summary>
        /// This method is called for each incoming message by _incomingMessageQueue to process incoming messages as ordered.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void IncomingMessageQueue_ProcessItem(object sender, ProcessQueueItemEventArgs<MessageReceivedFromCommunicatorEventArgs> e)
        {
            OnMessageReceived(this, e.ProcessItem);
        }

        /// <summary>
        /// This method is used to start static communicators (like Web Service communicators).
        /// </summary>
        private void StartCommunicators()
        {
            foreach (var connectedCommunicator in _communicators)
            {
                connectedCommunicator.Communicator.Start();
            }
        }

        /// <summary>
        /// This method is used to stop communicators that are not first connected to this server but this server opened connection to them.
        /// It also stops web service communicators.
        /// Because they are not contained in Communication layer, connection between them is closed by this method.
        /// </summary>
        private void StopCommunicators()
        {
            while (_communicators.Count > 0)
            {
                try
                {
                    _communicators.First.Value.Communicator.Stop(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                }
            }
        }

        #region Event raising methods
        
        /// <summary>
        /// Raises a MessageReceived event.
        /// </summary>
        /// <param name="sender">Creator of event</param>
        /// <param name="e">Event arguments</param>
        protected void OnMessageReceived(object sender, MessageReceivedFromCommunicatorEventArgs e)
        {
            try
            {
                if (MessageReceived != null)
                {
                    MessageReceived(sender, new MessageReceivedFromRemoteApplicationEventArgs
                    {
                        Application = this,
                        Communicator = e.Communicator,
                        Message = e.Message
                    });
                }
                else
                {
                    //Disconnect communicator if there is not listener for incoming messages from this communicator.
                    e.Communicator.Stop(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        /// <summary>
        /// Raises a CommunicatorConnected event.
        /// </summary>
        /// <param name="communicator">New connected communicator</param>
        private void OnCommunicatorConnected(ICommunicator communicator)
        {
            if (CommunicatorConnected == null)
            {
                return;
            }

            try
            {
                CommunicatorConnected(this, new CommunicatorConnectedEventArgs { Communicator = communicator });
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        /// <summary>
        /// Raises a CommunicatorConnected event.
        /// </summary>
        /// <param name="communicator">New connected communicator</param>
        private void OnCommunicatorDisconnected(ICommunicator communicator)
        {
            if (CommunicatorConnected == null)
            {
                return;
            }

            try
            {
                CommunicatorDisconnected(this, new CommunicatorDisconnectedEventArgs {Communicator = communicator});
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #endregion
    }
}
