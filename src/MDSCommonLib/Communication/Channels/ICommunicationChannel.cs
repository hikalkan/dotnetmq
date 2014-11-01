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

namespace MDS.Communication.Channels
{
    /// <summary>
    /// All Communication channels implements this interface.
    /// It is used by MDSClient and MDSController classes to communicate with MDS server.
    /// </summary>
    public interface ICommunicationChannel
    {
        /// <summary>
        /// This event is raised when the state of the communication channel changes.
        /// </summary>
        event CommunicationStateChangedHandler StateChanged;

        /// <summary>
        /// This event is raised when a MDSMessage object is received from MDS server.
        /// </summary>
        event MessageReceivedHandler MessageReceived;
        
        /// <summary>
        /// Unique identifier for this communicator in connected MDS server.
        /// This field is not set by communication channel,
        /// it is set by another classes (MDSClient) that are using
        /// communication channel. 
        /// </summary>
        long ComminicatorId { get; set; }

        /// <summary>
        /// Gets the state of communication channel.
        /// </summary>
        CommunicationStates State { get; }

        /// <summary>
        /// Communication way for this channel.
        /// This field is not set by communication channel,
        /// it is set by another classes (MDSClient) that are using
        /// communication channel. 
        /// </summary>
        CommunicationWays CommunicationWay { get; set; }
        
        /// <summary>
        /// Connects to MDS server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from MDS server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends a MDSMessage to the MDS server
        /// </summary>
        /// <param name="message">Message to be sent</param>
        void SendMessage(MDSMessage message);
    }
}
