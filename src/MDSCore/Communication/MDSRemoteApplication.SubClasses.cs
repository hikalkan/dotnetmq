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
using System.Threading;
using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Exceptions;
using MDS.Threading;

namespace MDS.Communication
{
    /// <summary>
    /// This file contains subclasses of MDSRemoteApplication.
    /// It is created to reduce complexity of MDSRemoteApplication class.
    /// </summary>
    public abstract partial class MDSRemoteApplication
    {
        #region MessageDeliverer class
        
        /// <summary>
        /// This class is used to deliver messages to remote application.
        /// It is designed to seperate message sending/delivering process from other processes of MDSRemoteApplication class.
        /// </summary>
        private class MessageDeliverer :IRunnable
        {
            #region Private fields

            /// <summary>
            /// Reference to the MDSRemoteApplication object that is used with this MessageDeliverer.
            /// </summary>
            private readonly MDSRemoteApplication _remoteApplication;

            /// <summary>
            /// This Timer is used to check if a ACK/Reject timeout occured for a message.
            /// Normally, when a communicator disconnected, if it was processing a message but not acknowledged yet,
            /// the message is turns back to the queue to be sent to another communicator (see RemoveReceiver method).
            /// But sometimes a communicator that processing messages may crash and can not send TCP close signal, so communicator may be supposed running..
            /// In this situation this timer periodically checks if a certain time passed but not ACK/Reject received from a communicator.
            /// </summary>
            private readonly Timer _asynchronMessageControlTimer;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="remoteApplication">Reference to the MDSRemoteApplication object that is used with this MessageDeliverer</param>
            public MessageDeliverer(MDSRemoteApplication remoteApplication)
            {
                _remoteApplication = remoteApplication;
                _asynchronMessageControlTimer = new Timer(AsynchronMessageControlTimer_Elapsed, null, Timeout.Infinite, Timeout.Infinite);
            }

            #endregion

            #region Public methods
            
            /// <summary>
            /// Starts MessageDeliverer.
            /// </summary>
            public void Start()
            {
                _asynchronMessageControlTimer.Change(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
            }

            /// <summary>
            /// Stops MessageDeliverer.
            /// </summary>
            /// <param name="waitToStop">Wait stopping of MessageDeliverer</param>
            public void Stop(bool waitToStop)
            {
                lock (_asynchronMessageControlTimer)
                {
                    _asynchronMessageControlTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }

            /// <summary>
            /// Waits stopping of MessageDeliverer.
            /// </summary>
            public void WaitToStop()
            {
                //No action
            }

            /// <summary>
            /// This method is used to send a MDSDataTransferMessage to an available communicator of application.
            /// It just blocks caller thread until a communicator comes available and message is sent or until timeout,
            /// but receives result (ACK/Reject) message asynchronous. It sends result (ACK/Reject) message to OnResponseReceived() method.
            /// </summary>
            /// <param name="message">Message to send</param>
            /// <param name="timeOut">Timeout value to wait if all receivers are busy. Set 0 to do not use timeout.</param>
            /// <returns>True, if message sent to communicator.</returns>
            public void SendDataMessage(MDSDataTransferMessage message, int timeOut)
            {
                //Get a free/available communicator from _receiverCommunicators list
                ConnectedCommunicator receiver = null;
                lock (_remoteApplication._communicators)
                {
                    var startTime = DateTime.Now;
                    var passedTime = 0;
                    while (receiver == null)
                    {
                        //If no receiver is connected to server, throw exception
                        if (_remoteApplication._communicators.Count <= 0)
                        {
                            throw new MDSNoCommunicatorException("There is no communicator for remote application '" + _remoteApplication.Name + "' to send message.");
                        }

                        //If timeout is set and timeout occurs, throw an exception
                        if ((timeOut > 0) && ((passedTime = (int)DateTime.Now.Subtract(startTime).TotalMilliseconds) >= timeOut))
                        {
                            throw new MDSTimeoutException("All communicators for remote application '" + _remoteApplication.Name + "' are busy. Waited for " + timeOut + " ms.");
                        }

                        //Get a free communicator that can receive message
                        //TODO: This check is working but not proper for here, move in the future.
                        receiver = message.DestinationServerName == _remoteApplication.Settings.ThisServerName
                                       ? GetFreeReceiverCommunicator(message.DestinationCommunicatorId)
                                       : GetFreeReceiverCommunicator(0);

                        //If no communicator is free, than wait for a free communicator..
                        if (receiver == null)
                        {
                            if (timeOut > 0)
                            {
                                //If there is not free communicator, wait until a communicator is available
                                Monitor.Wait(_remoteApplication._communicators, timeOut - passedTime);
                            }
                            else
                            {
                                //If there is not free communicator, wait until a communicator is available
                                Monitor.Wait(_remoteApplication._communicators);
                            }
                        }
                        else
                        {
                            receiver.ProcessingMessage = message;
                            receiver.ProcessingMessageExpireDate = DateTime.Now.AddMilliseconds(_remoteApplication.Settings.MessageResponseTimeout);
                        }
                    }
                }

                //Send message to communicator
                SetLeavingTime(message);
                SendMessage(message, receiver.Communicator);
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
                if(communicator == null)
                {
                    lock (_remoteApplication._communicators)
                    {
                        var receiverCommunicator = GetAnyReceiverCommunicator();
                        //If no receiver is connected to server, throw exception
                        if (receiverCommunicator == null)
                        {
                            throw new MDSNoCommunicatorException("There is no communicator for remote application '" + _remoteApplication.Name + "' to send message.");
                        }

                        communicator = receiverCommunicator.Communicator;
                    }
                }

                communicator.SendMessage(message);
                _remoteApplication.LastOutgoingMessageTime = DateTime.Now;
            }

            /// <summary>
            /// This method is used to get MDSOperationResultMessage objects (ACK/Reject messages) by MessageDeliverer class.
            /// It is called by MDSRemoteApplication's Communicator_MessageReceived method.
            /// </summary>
            /// <param name="e">Event arguments from Communicator_MessageReceived method</param>
            /// <returns>True, if message is handled by this method</returns>
            public bool HandleOperationResultMessage(MessageReceivedFromCommunicatorEventArgs e)
            {
                lock (_remoteApplication._communicators)
                {
                    var connectedCommunicator = _remoteApplication.FindCommunicator(e.Communicator);
                    if (connectedCommunicator != null && connectedCommunicator.ProcessingMessage != null && connectedCommunicator.ProcessingMessage.MessageId == e.Message.RepliedMessageId)
                    {
                        //Set communicator as free
                        connectedCommunicator.ProcessingMessage = null;

                        //Send receiver to end of the list
                        _remoteApplication._communicators.Remove(connectedCommunicator);
                        _remoteApplication._communicators.AddLast(connectedCommunicator);

                        //Suspend communicator if it rejected the message
                        var resultMessage = e.Message as MDSOperationResultMessage;
                        if (resultMessage != null && !resultMessage.Success)
                        {
                            connectedCommunicator.IsSuspended = true;
                            connectedCommunicator.SuspendExpireDate = DateTime.Now.AddSeconds(15);
                        }
                        else
                        {
                            //Pulse threads that are waiting for a free communicator
                            Monitor.PulseAll(_remoteApplication._communicators);
                        }

                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// This method is used to know if any receiver exists.
            /// </summary>
            /// <returns>True, if at least one receiver connected</returns>
            public bool IsThereAnyReceiver()
            {
                foreach (var communicator in _remoteApplication._communicators)
                {
                    if (communicator.Communicator.CommunicationWay == CommunicationWays.SendAndReceive)
                    {
                        return true;
                    }
                }

                return false;
            }

            #endregion

            /// <summary>
            /// This method is called by _asynchronMessageControlTimer periodically.
            /// See definition of _asynchronMessageControlTimer object.
            /// </summary>
            /// <param name="state">Not used argument</param>
            private void AsynchronMessageControlTimer_Elapsed(object state)
            {
                try
                {
                    //Stop timer during running of this method
                    _asynchronMessageControlTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    lock (_remoteApplication._communicators)
                    {
                        //If no communicator do not check anything..
                        if (_remoteApplication._communicators.Count <= 0)
                        {
                            return;
                        }

                        var pulseCommunicators = false;
                        var now = DateTime.Now;
                        foreach (var connectedCommunicator in _remoteApplication._communicators)
                        {
                            //Check if receiver is suspended and suspend date expired..
                            if (connectedCommunicator.IsSuspended && DateTime.Now.Subtract(connectedCommunicator.SuspendExpireDate).TotalSeconds >= 0)
                            {
                                //Reset Suspend flag
                                connectedCommunicator.IsSuspended = false;
                                //Pulse threads that are waiting for a free communicator
                                pulseCommunicators = true;
                            }

                            //If (communicator is not processing any message) OR (processing message is not timed out) then skip communicator..
                            if ((connectedCommunicator.ProcessingMessage == null) || (connectedCommunicator.ProcessingMessageExpireDate.Subtract(now).TotalMilliseconds > 0))
                            {
                                continue;
                            }

                            //Send communicator to end of the _receiverCommunicators list.
                            _remoteApplication._communicators.Remove(connectedCommunicator);
                            _remoteApplication._communicators.AddLast(connectedCommunicator);

                            //Pulse threads that are waiting for a free communicator
                            pulseCommunicators = true;

                            //Send Reject message for processing message, because communicator didn't send result message before ProcessingMessageExpireDate
                            _remoteApplication.OnResponseReceived(
                                null, new MDSOperationResultMessage
                                          {
                                              RepliedMessageId = connectedCommunicator.ProcessingMessage.MessageId,
                                              ResultText = "Remote application did not send ACK/Reject. Timeout occured.",
                                              Success = false
                                          });
                            connectedCommunicator.ProcessingMessage = null;
                        }

                        //Pulse threads that are waiting for a free communicator.
                        if(pulseCommunicators)
                        {
                            Monitor.PulseAll(_remoteApplication._communicators);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                }
                finally
                {
                    //Schedule ping timer for next running if organization layer is still running.
                    lock (_asynchronMessageControlTimer)
                    {
                        if (_remoteApplication._running)
                        {
                            _asynchronMessageControlTimer.Change(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
                        }
                    }
                }
            }

            /// <summary>
            /// Gets a free Receiver Communicator if exists, else returns null.
            /// </summary>
            /// <param name="communicatorId">
            /// Communicator's Id to get. 
            /// If it is smaller or equal to 0, returns any available communicator.
            /// </param>
            /// <returns>Free receiver communicator</returns>
            private ConnectedCommunicator GetFreeReceiverCommunicator(long communicatorId)
            {
                var communicatorExists = false;
                foreach (var communicator in _remoteApplication._communicators)
                {
                    if ((communicatorId == 0L) || (communicatorId == communicator.Communicator.ComminicatorId))
                    {
                        communicatorExists = true;
                    }

                    //Check if receiver is suspended..
                    if ((communicatorId == 0L) && communicator.IsSuspended)
                    {
                        //if it is, check if suspend date expired..
                        if (DateTime.Now.Subtract(communicator.SuspendExpireDate).TotalSeconds >= 0)
                        {
                            //Reset Suspend flag
                            communicator.IsSuspended = false;
                        }
                        else
                        {
                            //Skip communicator, it is suspended
                            continue;
                        }
                    }

                    //If wants to get a spesific communicator, and this communicator is not that, skip it..
                    if ((communicatorId > 0L) && (communicatorId != communicator.Communicator.ComminicatorId))
                    {
                        continue;
                    }

                    //If communicator is not busy and (it is requested communicator or a receiver) than get this communicator.
                    if ((communicator.ProcessingMessage == null) && ((communicatorId > 0L) || (communicator.Communicator.CommunicationWay == CommunicationWays.SendAndReceive)))
                    {
                        return communicator;
                    }
                }

                //If there is no communicator..
                if (!communicatorExists)
                {
                    if (communicatorId > 0)
                    {
                        throw new MDSNoCommunicatorException("There is no communicator for remote application '" +
                                                             _remoteApplication.Name + "' with communicatorId '" +
                                                             communicatorId);
                    }

                    throw new MDSNoCommunicatorException("There is no communicator for remote application '" +
                                                         _remoteApplication.Name);
                }

                //No available (free) communicator to get
                return null;
            }

            /// <summary>
            /// Gets any Receiver Communicator. It does not metter wheter it is busy or free.
            /// </summary>
            /// <returns>A receiver communicator</returns>
            private ConnectedCommunicator GetAnyReceiverCommunicator()
            {
                foreach (var communicator in _remoteApplication._communicators)
                {
                    if (communicator.Communicator.CommunicationWay == CommunicationWays.SendAndReceive)
                    {
                        return communicator;
                    }
                }

                return null;
            }
            
            /// <summary>
            /// Sets Leaving time for last passed server (this server) of message.
            /// </summary>
            /// <param name="message">Message object</param>
            private static void SetLeavingTime(MDSDataTransferMessage message)
            {
                if (message.PassedServers == null || message.PassedServers.Length <= 0)
                {
                    return;
                }

                message.PassedServers[message.PassedServers.Length - 1].LeavingTime = DateTime.Now;
            }
        }

        #endregion

        #region ConnectedCommunicator class

        /// <summary>
        /// This class is used to store connected communicators of this applications and their states/activities.
        /// </summary>
        protected class ConnectedCommunicator
        {
            /// <summary>
            /// Reference to the communicator object.
            /// </summary>
            public ICommunicator Communicator { get; private set; }

            /// <summary>
            /// If this communicator is processing a message (that means a message is sent to this communicator and response (ACK/Reject) message is not received yet)
            /// this field stores a reference to that message. This indicates that the communicator is busy, so we do not send and message if it is busy.
            /// If this field is null that means the communicator is free and can receive messages, so we can send messages to it.
            /// </summary>
            public MDSDataTransferMessage ProcessingMessage { get; set; }

            /// <summary>
            /// Indicates when the ProcessingMessage is assumed to rejected (not acknowledged) by communicator if any ACK/Reject is received.
            /// </summary>
            public DateTime ProcessingMessageExpireDate { get; set; }

            /// <summary>
            /// If a communicator rejects a message, it is suspended for a while.
            /// This value is true, if communicator is suspended now.
            /// </summary>
            public bool IsSuspended { get; set; }

            /// <summary>
            /// If IsSuspended is true, this value is the expire date of suspend state.
            /// </summary>
            public DateTime SuspendExpireDate { get; set; }

            /// <summary>
            /// Creates a new ReceiverCommunicator object.
            /// </summary>
            /// <param name="communicator">The communicator</param>
            public ConnectedCommunicator(ICommunicator communicator)
            {
                IsSuspended = false;
                SuspendExpireDate = DateTime.MinValue;
                ProcessingMessageExpireDate = DateTime.MaxValue;
                Communicator = communicator;
            }
        }

        #endregion
    }
}
