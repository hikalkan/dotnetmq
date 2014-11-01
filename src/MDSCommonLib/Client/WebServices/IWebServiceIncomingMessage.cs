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

using MDS.Communication.Messages;

namespace MDS.Client.WebServices
{
    /// <summary>
    /// Represents an incoming data message to a MDS Web Service from MDS server.
    /// </summary>
    public interface IWebServiceIncomingMessage
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for this message.
        /// </summary>
        string MessageId { get; }

        /// <summary>
        /// Name of the first source server of the message.
        /// </summary>
        string SourceServerName { get; }

        /// <summary>
        /// Name of the sender application of the message.
        /// </summary>
        string SourceApplicationName { get; }

        /// <summary>
        /// The source communication channel's (Communicator's) unique Id.
        /// When more than one communicator of an application is connected same MDS server
        /// at the same time, this field may be used to indicate a spesific communicator.
        /// This field is set by MDS automatically.
        /// </summary>
        long SourceCommunicatorId { get; }

        /// <summary>
        /// Name of the final destination server of the message.
        /// </summary>
        string DestinationServerName { get; }

        /// <summary>
        /// Name of the final destination application of the message.
        /// </summary>
        string DestinationApplicationName { get; }

        /// <summary>
        /// Passed servers of message until now, includes source and destination servers.
        /// </summary>
        ServerTransmitReport[] PassedServers { get; }

        /// <summary>
        /// Essential application message data that is received.
        /// </summary>
        byte[] MessageData { get; }

        /// <summary>
        /// Transmit rule of message.
        /// This is important because it determines persistence and transmit time of message.
        /// Default: StoreAndForward.
        /// </summary>
        MessageTransmitRules TransmitRule { get; }

        #endregion

        #region Methods

        IWebServiceResponseMessage CreateResponseMessage();

        IWebServiceOutgoingMessage CreateResponseDataMessage();

        #endregion
    }
}
