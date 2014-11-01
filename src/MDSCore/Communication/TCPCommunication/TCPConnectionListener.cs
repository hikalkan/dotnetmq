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
using System.Threading;
using System.Net.Sockets;
using log4net;
using MDS.Threading;

namespace MDS.Communication.TCPCommunication
{
    /// <summary>
    /// This class is used to listen and accept incoming TCP connection requests on given TCP port.
    /// </summary>
    public class TCPConnectionListener : IRunnable
    {
        #region Events

        /// <summary>
        /// When a client successfully connected the server, this event is raised.
        /// </summary>
        public event TCPClientConnectedHandler TCPClientConnected;

        #endregion

        #region Public properties

        /// <summary>
        /// Listening port number
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region Private fields

        /// <summary>
        /// Reference to logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Server socket to listen incoming connection requests.
        /// </summary>
        private TcpListener _listenerSocket;
        
        /// <summary>
        /// The thread to listen socket
        /// </summary>
        private volatile Thread _thread;

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new TCPConnectionListener with given tcp port.
        /// </summary>
        /// <param name="port">Listening TCP port no</param>
        public TCPConnectionListener(int port)
        {
            Port = port;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts listening.
        /// </summary>
        public void Start()
        {
            StartSocket();
            _running = true;
            _thread = new Thread(DoListenAsThread);
            _thread.Start();
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        /// <param name="waitToStop">True, if caller method must wait until running stops.</param>
        public void Stop(bool waitToStop)
        {
            _running = false;
            StopSocket();
            if (waitToStop)
            {
                WaitToStop();
            }
        }

        /// <summary>
        /// Waits until listener finishes it's work, if it is working.
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
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Starts listening socket.
        /// </summary>
        private void StartSocket()
        {
            _listenerSocket = new TcpListener(System.Net.IPAddress.Any, Port);
            _listenerSocket.Start();
        }

        /// <summary>
        /// Stops listening socket.
        /// </summary>
        /// <returns>Indicates the result of operation</returns>
        private void StopSocket()
        {
            _listenerSocket.Stop();
        }

        /// <summary>
        /// Entrance point of the thread.
        /// This method is used by the thread to listen incoming requests.
        /// </summary>
        private void DoListenAsThread()
        {
            while (_running)
            {
                try
                {
                    //Wait and get connected socket
                    var clientSocket = _listenerSocket.AcceptSocket();

                    //Raise TCPClientConnected event
                    if (clientSocket.Connected && (TCPClientConnected != null))
                    {
                        TCPClientConnected(this, new TCPClientConnectedEventArgs {ClientSocket = clientSocket});
                    }
                }
                catch (Exception ex)
                {
                    if (!_running)
                    {
                        return;
                    }

                    //on an exception, close connection, wait for a while and start again
                    Logger.Error(ex.Message, ex);

                    try
                    {
                        StopSocket();
                    }
                    catch (Exception exStop)
                    {
                        Logger.Error(exStop.Message, exStop);
                    }

                    Thread.Sleep(3000); //Wait 3 seconds

                    if (!_running)
                    {
                        return;
                    }

                    try
                    {
                        StartSocket();
                    }
                    catch (Exception exStart)
                    {
                        Logger.Error(exStart.Message, exStart);
                    }
                }
            }

            _thread = null;
        }

        #endregion
    }
}
