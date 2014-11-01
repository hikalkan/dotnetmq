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
using System.Net;
using MDS.Communication;
using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Communication.TCPCommunication;
using MDS.Exceptions;
using MDS.Storage;

namespace MDS.Organization
{
    /// <summary>
    /// An MDSAsjacentServer is a server in MDSServerGraph that is an adjacent of this server
    /// and directly communicates with this server. 
    /// </summary>
    public class MDSAdjacentServer : MDSPersistentRemoteApplicationBase
    {
        #region Public properties

        /// <summary>
        /// IP Address of Server on network
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
        }
        private readonly string _ipAddress;

        /// <summary>
        /// Listening port number of MDS on Server.
        /// </summary>
        public int Port
        {
            get { return _port; }
        }
        private readonly int _port;
        
        /// <summary>
        /// Communicator Type for MDS server
        /// </summary>
        public override CommunicatorTypes CommunicatorType
        {
            get { return CommunicatorTypes.MdsServer; }
        }

        /// <summary>
        /// This field is used to determine maximum allowed communicator count.
        /// No more communicator added if communicator count is equal to this number.
        /// For infinit communicator, returns -1;
        /// 
        /// Only 1 communicator is allowed in any time for a MDSAdjacentServer.
        /// </summary>
        protected override int MaxAllowedCommunicatorCount
        {
            get { return 1; }
        }

        #endregion

        #region Private fields

        /// <summary>
        /// The time last connection attempt to remote MDS server.
        /// </summary>
        private DateTime _lastConnectionAttemptTime;

        /// <summary>
        /// Consecutive error count on trying to connect to remote MDS server.
        /// </summary>
        private int _connectionErrorCount;

        /// <summary>
        /// This communicator object is temporary used to reconnect to remote MDS server.
        /// After connection it is added to communicators list and set to null.
        /// </summary>
        private TCPCommunicator _reconnectingCommunicator;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructur.
        /// </summary>
        /// <param name="name">name of server</param>
        /// <param name="ipAddress">IP Address of server</param>
        /// <param name="port">Listening TCP port of server</param>
        public MDSAdjacentServer(string name, string ipAddress, int port)
            : base(name)
        {
            _ipAddress = ipAddress;
            _port = port;
            _lastConnectionAttemptTime = DateTime.MinValue;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method is responsible to ensure connection with communicating MDS server.
        /// Checks connection, reconnects if disconnected and sends ping message.
        /// </summary>
        public void CheckConnection()
        {
            try
            {
                if (IsThereCommunicator())
                {
                    SendPingMessageIfNeeded();
                }

                CheckConnectionAndReConnectIfNeeded();
            }
            catch (Exception ex)
            {
                Logger.Warn("Can not connected to MDS Server: " + Name);
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #region Protected / Overrive methods

        /// <summary>
        /// Gets messages from database to be sent to this server.
        /// </summary>
        /// <param name="minId">Minimum Id of message record to get (minId included)</param>
        /// <param name="maxCount">Maximum number of records to get</param>
        /// <returns>List of messages</returns>
        protected override List<MDSMessageRecord> GetWaitingMessages(int minId, int maxCount)
        {
            return StorageManager.GetWaitingMessagesOfServer(Name, minId, maxCount);
        }

        /// <summary>
        /// Gets Id of last incoming message that will be sent to this server.
        /// </summary>
        /// <returns>Id of last incoming message</returns>
        protected override int GetMaxWaitingMessageId()
        {
            return StorageManager.GetMaxWaitingMessageIdOfServer(Name);
        }

        /// <summary>
        /// Finds Next server for a message.
        /// </summary>
        /// <returns>Next server</returns>
        protected override string GetNextServerForMessage(MDSDataTransferMessage message)
        {
            //Next server is this MDSAdjacentServer because message is being sent to that now.
            return Name;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Checks connection and reconnects to MDS server if needed.
        /// </summary>
        private void CheckConnectionAndReConnectIfNeeded()
        {
            var now = DateTime.Now;

            if (IsThereCommunicator())
            {
                // If any (dublex) communication made within last 90 seconds, then do not create connection.
                if (now.Subtract(LastIncomingMessageTime).TotalSeconds <= 90 &&
                    now.Subtract(LastOutgoingMessageTime).TotalSeconds <= 90)
                {
                    return;
                }
            }

            //Wait 1 second more on every failed connection attempt (Maximum 30 seconds).
            var waitSeconds = Math.Min(++_connectionErrorCount, 30);
            if (now.Subtract(_lastConnectionAttemptTime).TotalSeconds < waitSeconds)
            {
                return;
            }

            Logger.Info("Connecting remote MDS Server: " + Name + " (Attempt: " + _connectionErrorCount + ")");
            _lastConnectionAttemptTime = DateTime.Now;
            ConnectToServer();
            _connectionErrorCount = 0;
        }

        /// <summary>
        /// Sends a Ping message to MDS server if 60 seconds passed after last communication.
        /// </summary>
        private void SendPingMessageIfNeeded()
        {
            var now = DateTime.Now;
            if (now.Subtract(LastIncomingMessageTime).TotalSeconds < 60 &&
                now.Subtract(LastOutgoingMessageTime).TotalSeconds < 60)
            {
                return;
            }

            try
            {
                SendMessage(new MDSPingMessage());
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates a new TCPCommunicator to communicate with MDS server.
        /// </summary>
        private void ConnectToServer()
        {
            var ip = IPAddress.Parse(_ipAddress);
            if (ip == null)
            {
                throw new MDSException("IP address is not valid: " + _ipAddress);
            }

            var socket = GeneralHelper.ConnectToServerWithTimeout(new IPEndPoint(ip, _port), 10000); //10 seconds
            if (!socket.Connected)
            {
                throw new MDSException("TCP connection can not be established.");
            }

            //Create communicator object.
            _reconnectingCommunicator = new TCPCommunicator(socket, CommunicationLayer.CreateCommunicatorId());

            //Register MessageReceived event to receive response of Register message
            _reconnectingCommunicator.MessageReceived += Communicator_MessageReceived;

            //Start communicator and send a register message
            _reconnectingCommunicator.Start();
            _reconnectingCommunicator.SendMessage(new MDSRegisterMessage
                                               {
                                                   CommunicationWay = CommunicationWays.SendAndReceive,
                                                   CommunicatorType = CommunicatorTypes.MdsServer,
                                                   Name = Settings.ThisServerName,
                                                   Password = "" //Not implemented yet
                                               });
        }

        /// <summary>
        /// This method is just used to make a new connection with MDS server.
        /// It receives response of register message and adds communicator to Communicators if successfuly registered.
        /// </summary>
        /// <param name="sender">Creator object of event</param>
        /// <param name="e">Event arguments</param>
        private void Communicator_MessageReceived(object sender, MessageReceivedFromCommunicatorEventArgs e)
        {
            try
            {
                //Message must be MDSOperationResultMessage
                var message = e.Message as MDSOperationResultMessage;
                if (message == null)
                {
                    throw new MDSException("First message must be MDSOperationResultMessage");
                }

                //Check if remote MDS server accepted connection
                if(!message.Success)
                {
                    throw new MDSException("Remote MDS server did not accept connection.");
                }

                //Unregister from event and add communicator to Communicators list.
                e.Communicator.MessageReceived -= Communicator_MessageReceived;
                try
                {
                    AddCommunicator(e.Communicator);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    e.Communicator.Stop(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Can not connected to remote MDS server: '" + Name + "'. Connection is being closed.");
                Logger.Warn(ex.Message, ex);
                try
                {
                    e.Communicator.Stop(false);
                }
                catch (Exception ex2)
                {
                    Logger.Warn(ex2.Message, ex2);                    
                }
            }
            finally
            {
                _reconnectingCommunicator = null;
            }
        }

        #endregion
    }
}
