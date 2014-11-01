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
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using MDS.Communication.Events;
using MDS.Exceptions;
using MDS.Organization.Routing;
using MDS.Settings;
using MDS.Storage;
using MDS.Threading;
using MDS.Communication;
using MDS.Communication.Messages;

namespace MDS.Organization
{
    /// <summary>
    /// This class represents organization layer of MDS Server. It handles, stores, routes and delivers messages.
    /// </summary>
    public class OrganizationLayer : IRunnable
    {
        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to settings.
        /// </summary>
        private readonly MDSSettings _settings;

        /// <summary>
        /// Reference to communication layer.
        /// </summary>
        private readonly CommunicationLayer _communicationLayer;

        /// <summary>
        /// Reference to storage manager.
        /// </summary>
        private readonly IStorageManager _storageManager;

        /// <summary>
        /// Routing table.
        /// </summary>
        private readonly RoutingTable _routingTable;

        /// <summary>
        /// Reference to server graph.
        /// </summary>
        private readonly MDSServerGraph _serverGraph;

        /// <summary>
        /// Reference to application list.
        /// </summary>
        private readonly MDSClientApplicationList _clientApplicationList;

        /// <summary>
        /// Reference to all MDS Manager. It contains communicators to all instances of MDS manager.
        /// So, there is only one MDSController object in MDS.
        /// </summary>
        private readonly MDSController _mdsManager;

        /// <summary>
        /// This collection is used to send message and get response in SendMessageDirectly method.
        /// SendMessageDirectly method must wait until response received. It waits using this collection.
        /// Key: Message ID to wait response.
        /// Value: ManualResetEvent to wait thread until response received.
        /// </summary>
        private readonly SortedList<string, WaitingMessage> _waitingMessages;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="communicationLayer">Reference to the Communication Layer</param>
        /// <param name="storageManager">Reference to the Storage Manager</param>
        /// <param name="routingTable">Reference to the routing table</param>
        /// <param name="serverGraph">Reference to server graph</param>
        /// <param name="clientApplicationList">Reference to application list</param>
        /// <param name="mdsManager">Reference to MDS Manager object</param>
        public OrganizationLayer(CommunicationLayer communicationLayer, IStorageManager storageManager, RoutingTable routingTable, MDSServerGraph serverGraph, MDSClientApplicationList clientApplicationList, MDSController mdsManager)
        {
            _settings = MDSSettings.Instance;
            _communicationLayer = communicationLayer;
            _storageManager = storageManager;
            _routingTable = routingTable;
            _serverGraph = serverGraph;
            _clientApplicationList = clientApplicationList;
            _mdsManager = mdsManager;
            _waitingMessages = new SortedList<string, WaitingMessage>();
            PrepareCommunicationLayer();
        }

        #endregion

        #region Public methods

        #region Starting / Stopping to Organization layer

        /// <summary>
        /// Starts the organization layer.
        /// </summary>
        public void Start()
        {
            _clientApplicationList.Start();
            _serverGraph.Start();
            _mdsManager.Start();
        }

        /// <summary>
        /// Stops the organization layer.
        /// </summary>
        /// <param name="waitToStop">True, if caller thread must be blocked until organization layer stops.</param>
        public void Stop(bool waitToStop)
        {
            _mdsManager.Stop(waitToStop);
            _serverGraph.Stop(waitToStop);
            _clientApplicationList.Stop(waitToStop);
        }

        /// <summary>
        /// Waits to stop of organization layer.
        /// </summary>
        public void WaitToStop()
        {
            _mdsManager.WaitToStop();
            _serverGraph.WaitToStop();
            _clientApplicationList.WaitToStop();
        }

        #endregion

        #region Other public methods

        #region Client Application related methods

        /// <summary>
        /// Gets a list of all client applications as an array.
        /// </summary>
        /// <returns>Client applications array</returns>
        public MDSClientApplication[] GetClientApplications()
        {
            lock (_clientApplicationList.Applications)
            {
                return _clientApplicationList.Applications.Values.ToArray();                
            }
        }

        /// <summary>
        /// This method is used to add a new client application to MDS while MDS is running.
        /// Used by MDSController to allow user to add a new application from MDSManager GUI.
        /// It does all necessary tasks to add new application (Updates XML file, adds application to needed
        /// collections of system...).
        /// </summary>
        /// <param name="name">Name of the new application</param>
        public MDSClientApplication AddApplication(string name)
        {
            //Add to settings
            lock (_settings.Applications)
            {
                _settings.Applications.Add(
                    new ApplicationInfoItem
                        {
                            Name = name
                        });
                _settings.SaveToXml();
            }

            //Add to applications list
            lock (_clientApplicationList.Applications)
            {
                var newApplication = new MDSClientApplication(name);
                newApplication.Settings = _settings;
                newApplication.StorageManager = _storageManager;
                newApplication.MessageReceived += RemoteApplication_MessageReceived;

                _clientApplicationList.Applications.Add(name, newApplication);
                _communicationLayer.AddRemoteApplication(newApplication);

                newApplication.Start();

                return newApplication;
            }
        }

        /// <summary>
        /// This method is used to delete a client application from MDS while MDS is running.
        /// Used by MDSController to allow user to remove an application from MDSManager GUI.
        /// It does all necessary tasks to remove application (Updates XML file, removes application from needed
        /// collections of system...).
        /// </summary>
        /// <param name="name">Name of the application to remove</param>
        /// <returns>Removed client application</returns>
        public MDSClientApplication RemoveApplication(string name)
        {
            //Remove from application list and communicaton layer
            MDSClientApplication clientApplication = null;
            lock (_clientApplicationList.Applications)
            {
                if (_clientApplicationList.Applications.ContainsKey(name))
                {
                    clientApplication = _clientApplicationList.Applications[name];
                    if (clientApplication.ConnectedCommunicatorCount > 0)
                    {
                        throw new MDSException("Client application can not be removed. It has " +
                                               clientApplication.ConnectedCommunicatorCount +
                                               " communicators connected. Please stop theese communicators first.");
                    }

                    clientApplication.MessageReceived -= RemoteApplication_MessageReceived;

                    _communicationLayer.RemoveRemoteApplication(clientApplication);
                    _clientApplicationList.Applications.Remove(name);
                }
            }

            if (clientApplication == null)
            {
                throw new MDSException("There is no client application with name " + name);
            }
            
            clientApplication.Stop(true);

            //Remove from settings
            lock (_settings.Applications)
            {
                for (var i = 0; i < _settings.Applications.Count;i++ )
                {
                    if (_settings.Applications[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        _settings.Applications.RemoveAt(i);
                        _settings.SaveToXml();
                        break;
                    }
                }
            }

            return clientApplication;
        }

        #endregion

        #endregion

        #endregion

        #region Private methods

        #region Initializing part

        /// <summary>
        /// Adds server and applications to communication layer and registers their MessageReceived events.
        /// </summary>
        private void PrepareCommunicationLayer()
        {
            //Add MDS servers to communication layer, set needed objects and register events
            foreach (var adjacentServer in _serverGraph.AdjacentServers.Values)
            {
                adjacentServer.Settings = _settings;
                _communicationLayer.AddRemoteApplication(adjacentServer);
                adjacentServer.StorageManager = _storageManager;
                adjacentServer.MessageReceived += RemoteApplication_MessageReceived;
            }

            //Add applications to communication layer, set needed objects and register events
            foreach (var clientApplication in _clientApplicationList.Applications.Values)
            {
                clientApplication.Settings = _settings;
                _communicationLayer.AddRemoteApplication(clientApplication);
                clientApplication.StorageManager = _storageManager;
                clientApplication.MessageReceived += RemoteApplication_MessageReceived;
            }

            _mdsManager.Settings = _settings;
            _communicationLayer.AddRemoteApplication(_mdsManager);
        }

        #endregion

        #region Incoming message handling

        #region Incoming message handling from MDS Serves / Client Applications

        /// <summary>
        /// This method is handles all adjacent server's and client application's MessageReceived events.
        /// </summary>
        /// <param name="sender">Creator of event</param>
        /// <param name="e">Event arguments</param>
        private void RemoteApplication_MessageReceived(object sender, MessageReceivedFromRemoteApplicationEventArgs e)
        {
            try
            {
                //Incoming data transfer message request
                if (e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSDataTransferMessage)
                {
                    //Get, check and process message
                    var dataTransferMessage = e.Message as MDSDataTransferMessage;
                    ProcessDataTransferMessage(e.Application as MDSPersistentRemoteApplicationBase, e.Communicator, dataTransferMessage);
                }
                //Incoming delivery result (ACK/Reject message)
                else if ((e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSOperationResultMessage) && 
                    (!string.IsNullOrEmpty(e.Message.RepliedMessageId)))
                {
                    //Find and send signal/pulse to waiting thread for this message
                    WaitingMessage waitingMessage = null;
                    lock (_waitingMessages)
                    {
                        if (_waitingMessages.ContainsKey(e.Message.RepliedMessageId))
                        {
                            waitingMessage = _waitingMessages[e.Message.RepliedMessageId];
                        }
                    }

                    if(waitingMessage != null)
                    {
                        waitingMessage.ResponseMessage = e.Message as MDSOperationResultMessage;
                        waitingMessage.WaitEvent.Set();
                    }
                }
                //Incoming Ping message
                else if ((e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSPingMessage) && 
                    string.IsNullOrEmpty(e.Message.RepliedMessageId))
                {
                    //Reply ping message
                    e.Application.SendMessage(new MDSPingMessage {RepliedMessageId = e.Message.MessageId},
                                              e.Communicator);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #endregion

        #region Processing MDSDataTransferMessage message

        /// <summary>
        /// This method is used to process a MDSDataTransferMessage that is gotten from a mds server or client application.
        /// Message is sent to destination or next server in one of these three conditions:
        /// - Destination server is this server and application exists on this server
        /// - Destination server is an adjacent server of this server
        /// - Destination server in server graph and there is a path from this server to destination server
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="message">Message</param>
        private void ProcessDataTransferMessage(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSDataTransferMessage message)
        {
            //Check for duplicate messages
            if (senderApplication.LastAcknowledgedMessageId == message.MessageId)
            {
                SendOperationResultMessage(senderApplication, senderCommunicator, message, true, "Duplicate message.");
                return;
            }

            try
            {
                AddThisServerToPassedServerList(message);

                FillEmptyMessageFields(message, senderApplication, senderCommunicator);

                _routingTable.ApplyRouting(message);

                //If Destination server is this server then deliver message to the destination application
                if (message.DestinationServerName.Equals(_settings.ThisServerName, StringComparison.OrdinalIgnoreCase))
                {
                    SentToClientApplication(senderApplication, senderCommunicator, message);
                }
                //Else, if destination server is an adjacent of this server (so they can communicate directly)
                else if (_serverGraph.AdjacentServers.ContainsKey(message.DestinationServerName))
                {
                    SentToAdjacentServer(senderApplication, senderCommunicator, message);
                }
                //Else, if destination server is not adjacent but in server graph (so, send message to next server)
                else if (_serverGraph.ServerNodes.ContainsKey(message.DestinationServerName))
                {
                    SendToNextServer(senderApplication, senderCommunicator, message);
                }
                else
                {
                    //return error to sender
                    SendOperationResultMessage(senderApplication, senderCommunicator, message, false, "Destination does not exists.");
                }
            }
            catch (Exception ex)
            {
                SendOperationResultMessage(senderApplication, senderCommunicator, message, false, ex.Message);
            }
        }
        
        /// <summary>
        /// Adds this server to the list of passed servers of message.
        /// </summary>
        /// <param name="message">Message object</param>
        private void AddThisServerToPassedServerList(MDSDataTransferMessage message)
        {
            //Create new transmit report for this server
            var transmitReport = new ServerTransmitReport
            {
                ArrivingTime = DateTime.Now,
                ServerName = _settings.ThisServerName
            };
            if (message.PassedServers == null)
            {
                //Create array
                message.PassedServers = new ServerTransmitReport[1];
            }
            else
            {
                //Create new array (that has item one more than original array)
                var newArray = new ServerTransmitReport[message.PassedServers.Length + 1];
                //Copy old items to new array
                if (message.PassedServers.Length > 1)
                {
                    Array.Copy(message.PassedServers, 0, newArray, 0, message.PassedServers.Length);
                }

                //Replace old array by new array 
                message.PassedServers = newArray;
            }

            //Add transmit report to array
            message.PassedServers[message.PassedServers.Length - 1] = transmitReport;
        }

        /// <summary>
        /// Checks a MDSDataTransferMessage and fills it's empty fields by default values.
        /// </summary>
        /// <param name="dataTransferMessage">Message</param>
        /// <param name="senderApplication">Sender application</param>
        /// <param name="communicator">Sender communicator of application</param>
        private void FillEmptyMessageFields(MDSDataTransferMessage dataTransferMessage, MDSRemoteApplication senderApplication, ICommunicator communicator)
        {
            //Default SourceApplicationName: Name of the sender application.
            if (string.IsNullOrEmpty(dataTransferMessage.SourceApplicationName))
            {
                dataTransferMessage.SourceApplicationName = senderApplication.Name;
            }

            //Default SourceServerName: Name of this server.
            if (string.IsNullOrEmpty(dataTransferMessage.SourceServerName))
            {
                dataTransferMessage.SourceServerName = _settings.ThisServerName;
            }

            //Default DestinationApplicationName: Name of the sender application.
            if (string.IsNullOrEmpty(dataTransferMessage.DestinationApplicationName))
            {
                dataTransferMessage.DestinationApplicationName = senderApplication.Name;
            }

            //Default DestinationServerName: Name of this server.
            if (string.IsNullOrEmpty(dataTransferMessage.DestinationServerName))
            {
                dataTransferMessage.DestinationServerName = _settings.ThisServerName;
            }

            if (dataTransferMessage.SourceServerName == _settings.ThisServerName)
            {
                //Sender communicator id is being set.
                dataTransferMessage.SourceCommunicatorId = communicator.ComminicatorId;
            }
        }

        /// <summary>
        /// This method is called by ProcessDataTransferMessage when a message must be sent to a aclient application
        /// that is running on this server.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="message">Message</param>
        private void SentToClientApplication(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSDataTransferMessage message)
        {
            MDSClientApplication destinationApplication = null;

            //If application exists on this server, get it
            lock (_clientApplicationList.Applications)
            {
                if (_clientApplicationList.Applications.ContainsKey(message.DestinationApplicationName))
                {
                    destinationApplication = _clientApplicationList.Applications[message.DestinationApplicationName];
                }
            }
            
            //If application doesn't exist on this server...
            if (destinationApplication == null)
            {
                SendOperationResultMessage(senderApplication, senderCommunicator, message, false, "Application does not exists on this server (" + _settings.ThisServerName + ").");
                return;
            }

            //Send message according TransmitRule
            switch (message.TransmitRule)
            {
                case MessageTransmitRules.DirectlySend:
                    SendMessageDirectly(
                        senderApplication,
                        senderCommunicator,
                        destinationApplication,
                        message
                        );
                    break;
                default:
                    // case MessageTransmitRules.StoreAndForward:
                    // case MessageTransmitRules.NonPersistent:
                    EnqueueMessage(
                        senderApplication,
                        senderCommunicator,
                        destinationApplication,
                        message
                        );
                    break;
            }
        }

        /// <summary>
        /// This method is called by ProcessDataTransferMessage when a message must be sent to an adjacent server of this server.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="message">Message</param>
        private void SentToAdjacentServer(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSDataTransferMessage message)
        {
            /* On one of these conditions, message is stored:
             * - TransmitRule = StoreAndForward
             * - (TransmitRule = StoreOnSource OR StoreOnEndPoints) AND (This server is the source server)
             */
            if (message.TransmitRule == MessageTransmitRules.StoreAndForward || 
                message.TransmitRule == MessageTransmitRules.NonPersistent)
            {
                EnqueueMessage(
                    senderApplication,
                    senderCommunicator,
                    _serverGraph.AdjacentServers[message.DestinationServerName],
                    message
                    );
            }
            /* Else, message is not stored in these conditions:
             * - TransmitRule = DirectlySend OR StoreOnDestination (this server can not be destination because message is being sent to another server right now)
             * - All Other conditions
             */
            else
            {
                SendMessageDirectly(
                    senderApplication,
                    senderCommunicator,
                    _serverGraph.AdjacentServers[message.DestinationServerName],
                    message
                    );
            }
        }
        
        /// <summary>
        /// This method is called by ProcessDataTransferMessage when a message must be sent to a server
        /// that is not an adjacent of this server. Message is forwarded to next server.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="message">Message</param>
        private void SendToNextServer(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSDataTransferMessage message)
        {
            //If there is a path from this server to destination server...
            if (_serverGraph.ThisServerNode.BestPathsToServers.ContainsKey(message.DestinationServerName))
            {
                //Find best path to destination server 
                var bestPath = _serverGraph.ThisServerNode.BestPathsToServers[message.DestinationServerName];
                //If path is regular (a path must consist of 2 nodes at least)...
                if (bestPath.Count > 1)
                {
                    //Next server
                    var nextServerName = bestPath[1].Name;

                    /* On one of these conditions, message is stored:
                     * - TransmitRule = StoreAndForward
                     * - (TransmitRule = StoreOnSource OR StoreOnEndPoints) AND (This server is the source server)
                     */
                    if (message.TransmitRule == MessageTransmitRules.StoreAndForward ||
                        message.TransmitRule == MessageTransmitRules.NonPersistent)
                    {
                        EnqueueMessage(
                            senderApplication,
                            senderCommunicator,
                            _serverGraph.AdjacentServers[nextServerName],
                            message
                            );
                    }
                    /* Else, message is not stored in these conditions:
                     * - TransmitRule = DirectlySend OR StoreOnDestination (this server can not be destination because message is being sent to another server right now)
                     * - All Other conditions
                     */
                    else
                    {
                        SendMessageDirectly(
                            senderApplication,
                            senderCommunicator,
                            _serverGraph.AdjacentServers[nextServerName],
                            message
                            );
                    }
                }
                //Server graph may be wrong (this is just for checking case, normally this situation must not become)
                else
                {
                    SendOperationResultMessage(senderApplication, senderCommunicator, message, false, "Server graph is wrong.");
                }
            }
            //No path from this server to destination server
            else
            {
                SendOperationResultMessage(senderApplication, senderCommunicator, message, false, "There is no path from this server to destination.");
            }
        }

        /// <summary>
        /// Sends message directly to application (not stores) and waits ACK.
        /// This method adds message to queue by MDSPersistentRemoteApplicationBase.AddMessageToHeadOfQueue method
        /// and waits a signal/pulse from RemoteApplication_MessageReceived method to get ACK/Reject.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="destApplication">Destination application/server</param>
        /// <param name="message">Message</param>
        private void SendMessageDirectly(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSPersistentRemoteApplicationBase destApplication, MDSDataTransferMessage message)
        {
            //Create a WaitingMessage to wait and get ACK/Reject message and add it to waiting messages
            var waitingMessage = new WaitingMessage();
            lock (_waitingMessages)
            {
                _waitingMessages[message.MessageId] = waitingMessage;
            }

            try
            {
                //Add message to head of queue of remote application
                destApplication.AddMessageToHeadOfQueue(message);

                //Wait until thread is signalled by another thread to get response (Signalled by RemoteApplication_MessageReceived method)
                waitingMessage.WaitEvent.WaitOne((int) (_settings.MessageResponseTimeout*1.2));

                //Evaluate response
                if (waitingMessage.ResponseMessage.Success)
                {
                    SendOperationResultMessage(senderApplication, senderCommunicator, message, true, "Success.");
                }
                else
                {
                    SendOperationResultMessage(senderApplication, senderCommunicator, message, false, "Message is not acknowledged. Reason: " + waitingMessage.ResponseMessage.ResultText);
                }
            }
            finally
            {
                //Remove message from waiting messages
                lock (_waitingMessages)
                {
                    _waitingMessages.Remove(message.MessageId);
                }
            }
        }
        
        /// <summary>
        /// Adds message to destination's send queue.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="senderCommunicator">Sender communicator</param>
        /// <param name="destApplication">Destination application/server</param>
        /// <param name="message">Message</param>
        private static void EnqueueMessage(MDSRemoteApplication senderApplication, ICommunicator senderCommunicator, MDSPersistentRemoteApplicationBase destApplication, MDSDataTransferMessage message)
        {
            destApplication.EnqueueMessage(message);
            SendOperationResultMessage(senderApplication, senderCommunicator, message, true, "Success.");
        }

        /// <summary>
        /// To send a MDSOperationResultMessage to remote application's spesific communicator.
        /// </summary>
        /// <param name="senderApplication">Sender application/server</param>
        /// <param name="communicator">Communicator to send message</param>
        /// <param name="repliedMessage">Replied Message</param>
        /// <param name="success">Operation result</param>
        /// <param name="resultText">Details</param>
        private static void SendOperationResultMessage(MDSRemoteApplication senderApplication, ICommunicator communicator, MDSDataTransferMessage repliedMessage, bool success, string resultText)
        {
            try
            {
                if (success)
                {
                    //Save MessageId of acknowledged message to do not receive same message again
                    senderApplication.LastAcknowledgedMessageId = repliedMessage.MessageId;
                }

                senderApplication.SendMessage(new MDSOperationResultMessage
                {
                    RepliedMessageId = repliedMessage.MessageId,
                    Success = success,
                    ResultText = resultText
                }, communicator);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #endregion

        #region Sub classes

        /// <summary>
        /// This class is used as item in _waitingMessages collection.
        /// Key: Message ID to wait response.
        /// Value: ManualResetEvent to wait thread until response received.
        /// </summary>
        /// </summary>
        private class WaitingMessage
        {
            /// <summary>
            /// ManualResetEvent to wait thread until response received.
            /// </summary>
            public ManualResetEvent WaitEvent { get; private set; }

            /// <summary>
            /// Response message received as ACK/Reject for sent message
            /// </summary>
            public MDSOperationResultMessage ResponseMessage { get; set; }

            /// <summary>
            /// Creates a new WaitingMessage.
            /// </summary>
            public WaitingMessage()
            {
                WaitEvent = new ManualResetEvent(false);
            }
        }

        #endregion
    }
}
