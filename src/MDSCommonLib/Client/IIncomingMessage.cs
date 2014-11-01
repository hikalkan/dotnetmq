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
    /// Represents an incoming data message for a MDS client.
    /// </summary>
    public interface IIncomingMessage
    {
        #region Properties

        /// <summary>
        /// Acknowledgment state of the message.
        /// </summary>
        MessageAckStates AckState { get; }

        /// <summary>
        /// Gets the unique identifier for this message.
        /// </summary>
        string MessageId { get; }

        /// <summary>
        /// If this message is a reply for another message then RepliedMessageId contains first message's MessageId
        /// else RepliedMessageId is null as default.
        /// </summary>
        string RepliedMessageId { get; }

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
        /// Destination communication channel's (Communicator's) Id.
        /// This field is used by MDS to deliver message to a spesific communicator.
        /// When more than one communicator of an application is connected same MDS server
        /// at the same time, this field may be used to indicate a spesific communicator as receiver of message.
        /// If it is set to 0 (zero), message may be delivered to any connected communicator.
        /// </summary>
        long DestinationCommunicatorId { get; }

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

        /// <summary>
        /// Used to Acknowledge this message.
        /// Confirms that the message is received safely by client application.
        /// A message must be acknowledged by client application to remove message from message queue.
        /// MDS server doesn't send next message to the client application until the message is acknowledged.
        /// Also, MDS server sends same message again if the message is not acknowledged in a certain time.
        /// </summary>
        void Acknowledge();

        /// <summary>
        /// Used to reject (to not acknowledge) this message.
        /// Indicates that the message can not received correctly or can not handled the message, and the message 
        /// must be sent to client application later again.
        /// If MDS server gets reject for a message,
        /// it doesn't send any message to the client application instance for a while.
        /// If message is persistent, than it is sent to another instance of application or to same application instance again. 
        /// If message is not persistent, it is deleted.
        /// </summary>
        void Reject();

        /// <summary>
        /// Used to reject (to not acknowledge) this message.
        /// Indicates that the message can not received correctly or can not handled the message, and the message 
        /// must be sent to client application later again.
        /// If MDS server gets reject for a message,
        /// it doesn't send any message to the client application instance for a while.
        /// If message is persistent, than it is sent to another instance of application or to same application instance again. 
        /// If message is not persistent, it is deleted.
        /// </summary>
        /// <param name="reason">Reject reason</param>
        void Reject(string reason);

        /// <summary>
        /// Creates a empty response message for this message.
        /// </summary>
        /// <returns>Response message object</returns>
        IOutgoingMessage CreateResponseMessage();

        #endregion
    }
}
