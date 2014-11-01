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
using MDS.Communication;

namespace MDS.Client.MDSServices
{
    public class MDSServiceConsumer : IDisposable
    {
        #region Public fields

        /// <summary>
        /// Underlying MDSClient object to send/receive MDS messages.
        /// </summary>
        internal MDSClient MdsClient { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new MDSServiceApplication object with default values to connect to MDS server.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        public MDSServiceConsumer(string applicationName)
        {
            MdsClient = new MDSClient(applicationName, CommunicationConsts.DefaultIpAddress, CommunicationConsts.DefaultMDSPort);
            Initialize();
        }

        /// <summary>
        /// Creates a new MDSServiceApplication object with default port to connect to MDS server.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        /// <param name="ipAddress">IP address of MDS server</param>
        public MDSServiceConsumer(string applicationName, string ipAddress)
        {
            MdsClient = new MDSClient(applicationName, ipAddress, CommunicationConsts.DefaultMDSPort);
        }

        /// <summary>
        /// Creates a new MDSServiceApplication object.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        /// <param name="ipAddress">IP address of MDS server</param>
        /// <param name="port">TCP port of MDS server</param>
        public MDSServiceConsumer(string applicationName, string ipAddress, int port) 
        {
            MdsClient = new MDSClient(applicationName, ipAddress, port);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method connects to MDS server using underlying MDSClient object.
        /// </summary>
        public void Connect()
        {
            MdsClient.Connect();
        }

        /// <summary>
        /// This method disconnects from MDS server using underlying MDSClient object.
        /// </summary>
        public void Disconnect()
        {
            MdsClient.Disconnect();
        }

        /// <summary>
        /// Disposes this object, disposes/closes underlying MDSClient object.
        /// </summary>
        public void Dispose()
        {
            MdsClient.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes this object.
        /// </summary>
        private void Initialize()
        {
            MdsClient.CommunicationWay = CommunicationWays.Send;
        }
        
        #endregion
    }
}
