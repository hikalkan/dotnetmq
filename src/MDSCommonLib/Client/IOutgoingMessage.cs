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

namespace MDS.Client
{
    /// <summary>
    /// Represents an outgoing data message for a MDS client.
    /// </summary>
    public interface IOutgoingMessage
    {
        #region Properties

        /// <summary>
        /// Unique ID for this message.
        /// This will be a GUID if it is not set.
        /// It is recommended to leave this field as default.
        /// </summary>
        string MessageId { get; set; }

        /// <summary>
        /// If this message is a reply for another message then RepliedMessageId contains first message's MessageId
        /// else RepliedMessageId is null default.
        /// </summary>
        string RepliedMessageId { get; set; }

        /// <summary>
        /// Name of the first source server of the message.
        /// If this field is leaved null/empty, it is set by MDS server automatically.
        /// </summary>
        string SourceServerName { get; set; }

        /// <summary>
        /// Name of the first sender application of the message.
        /// If this field is leaved null/empty, it is set by MDS server as sender application's name automatically.
        /// </summary>
        string SourceApplicationName { get; set; }

        /// <summary>
        /// Name of the final destination server of the message.
        /// If this field is leaved null/empty, it is set by MDS server as
        /// sender application's server's name automatically.
        /// It may be leaved null to send a message to an application on the same server.
        /// </summary>
        string DestinationServerName { get; set; }

        /// <summary>
        /// Name of the final destination application of the message.
        /// If this field is leaved null/empty, it is set by MDS server as sender application's name automatically.
        /// It may be leaved null to send a message to same application.
        /// </summary>
        string DestinationApplicationName { get; set; }

        /// <summary>
        /// Destination communication channel's (Communicator's) Id.
        /// This field is used by MDS to deliver message to a spesific communicator.
        /// When more than one communicator of an application is connected same MDS server
        /// at the same time, this field may be used to indicates a spesific communicator as receiver of message.
        /// If it is set to 0 (zero), message may be delivered to any connected communicator.
        /// If there is no communicator with DestinationCommunicatorId, message can not be delivered, so,
        /// this field can only be used to send non-persistent messages (Syncronous messages).
        /// </summary>
        long DestinationCommunicatorId { get; set; }

        /// <summary>
        /// Essential application message data to be sent.
        /// </summary>
        byte[] MessageData { get; set; }

        /// <summary>
        /// Transmit rule of message.
        /// This is important because it determines persistence and transmit time of message.
        /// Default: StoreAndForward.
        /// </summary>
        MessageTransmitRules TransmitRule { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the message to the MDS server.
        /// If this method does not throw an exception,
        /// message is correctly delivered to MDS server (persistent message)
        /// or to the destination application (non persistent message).
        /// </summary>
        void Send();

        /// <summary>
        /// Sends the message to the MDS server.
        /// If this method does not throw an exception,
        /// message is correctly delivered to MDS server (persistent message)
        /// or to the destination application (non persistent message).
        /// </summary>
        /// <param name="timeoutMilliseconds">Timeout to send message as milliseconds</param>
        void Send(int timeoutMilliseconds);

        /// <summary>
        /// Sends the message and waits for an incoming message for that message.
        /// MDS can be used for Request/Response type messaging with this method.
        /// Default timeout value: 5 minutes.
        /// </summary>
        /// <returns>Response message</returns>
        IIncomingMessage SendAndGetResponse();

        /// <summary>
        /// Sends the message and waits for an incoming message for that message.
        /// MDS can be used for Request/Response type messaging with this method.
        /// </summary>
        /// <param name="timeoutMilliseconds">Timeout to get response message</param>
        /// <returns>Response message</returns>
        IIncomingMessage SendAndGetResponse(int timeoutMilliseconds);

        #endregion
    }
}
