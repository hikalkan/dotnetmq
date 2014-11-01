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
using System.Reflection;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using log4net;
using MDS.Communication.Messages;
using MDS.Communication.Protocols;
using MDS.Exceptions;
using MDS.Serialization;

namespace MDS.Communication.TCPCommunication
{
    /// <summary>
    /// This class represents an communication channel with a Remote Application via TCP sockets.
    /// </summary>
    public class TCPCommunicator : CommunicatorBase
    {
        #region Private fields

        /// <summary>
        /// Reference to logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The TCP socket to the remote application.
        /// </summary>
        private readonly Socket _socket;

        /// <summary>
        /// The main stream wraps socket to send/receive data.
        /// </summary>
        private NetworkStream _networkStream;

        /// <summary>
        /// This object is used to send/receive messages as byte array.
        /// </summary>
        private readonly IMDSWireProtocol _wireProtocol;

        /// <summary>
        /// The thread that listens incoming data.
        /// </summary>
        private Thread _thread;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="socket">Open TCP socket connection to the communicator.</param>
        /// <param name="comminicatorId">Unique identifier for this communicator.</param>
        public TCPCommunicator(Socket socket, long comminicatorId)
            : base(comminicatorId)
        {
            _socket = socket;
            _socket.NoDelay = true;
            CommunicationWay = CommunicationWays.SendAndReceive;
            _wireProtocol = new MDSDefaultWireProtocol();           
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Waits communicator to stop.
        /// </summary>
        public override void WaitToStop()
        {
            if (_thread == null)
            {
                return;
            }

            try
            {
                _thread.Join();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Prepares communication objects and starts data listening thread.
        /// </summary>
        protected override void StartCommunicaiton()
        {
            if (!_socket.Connected)
            {
                throw new MDSException("Tried to start communication with a TCP socket that is not connected.");
            }

            _networkStream = new NetworkStream(_socket);

            _thread = new Thread(DoCommunicateAsThread);
            _thread.Start();
        }

        /// <summary>
        /// Closes the socket and stops the thread.
        /// </summary>
        /// <param name="waitToStop">True, to block caller thread until this object stops</param>
        protected override void StopCommunicaiton(bool waitToStop)
        {
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Send);
                _socket.Close();
            }

            if (waitToStop)
            {
                WaitToStop();
            }
        }

        /// <summary>
        /// Sends a message to the TCP communicator according Communication type
        /// </summary>
        /// <param name="message">Message to send</param>
        protected override void SendMessageInternal(MDSMessage message)
        {
            if(State != CommunicationStates.Connected)
            {
                throw new MDSException("Communicator's state is not connected. It can not send message.");
            }

            SendMessageToSocket(message);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Entrance point of the thread.
        /// This method run by thread to listen incoming data from communicator.
        /// </summary>
        private void DoCommunicateAsThread()
        {
            Logger.Debug("TCPCommunicator thread is started. CommunicatorId=" + ComminicatorId);
            
            while (State == CommunicationStates.Connected || State == CommunicationStates.Connecting)
            {
                try
                {
                    //Read a message from _networkStream (socket) and raise MessageReceived event
                    var message = _wireProtocol.ReadMessage(new MDSDefaultDeserializer(_networkStream));
                    Logger.Debug("Message received by communicator " + ComminicatorId + ": " + message.GetType().Name);
                    OnMessageReceived(message);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    break; //Stop listening
                }
            }

            //if socket is still connected, then close it
            try
            {
                Stop(false);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            Logger.Debug("TCPCommunicator is stopped. CommunicatorId=" + ComminicatorId);

            _thread = null;
        }

        /// <summary>
        /// Sends MDSMessage object to the socket.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        private void SendMessageToSocket(MDSMessage message)
        {
            Logger.Debug("Message is being sent to communicator " + ComminicatorId + ": " + message.GetType().Name);

            //Create MemoryStream to write message to a byte array
            var memoryStream = new MemoryStream();

            //Write message
            _wireProtocol.WriteMessage(new MDSDefaultSerializer(memoryStream), message);

            //Check the length of message data
            if (memoryStream.Length > CommunicationConsts.MaxMessageSize)
            {
                throw new Exception("Message is too big to send.");
            }

            //SendMessage message (contents of created memory stream)
            var sendBuffer = memoryStream.ToArray();
            var length = sendBuffer.Length;
            var totalSent = 0;
            while (totalSent < length)
            {
                var sent = _socket.Send(sendBuffer, totalSent, length - totalSent, SocketFlags.None);
                if (sent <= 0)
                {
                    throw new Exception("Message can not be sent via TCP socket. Only " + totalSent + " bytes of " + length + " bytes are sent.");
                }

                totalSent += sent;
            }

            Logger.Debug("Message is sent to communicator " + ComminicatorId + ": " + message.GetType().Name);
        }

        #endregion
    }
}
