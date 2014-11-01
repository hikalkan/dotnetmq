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

namespace MDS.Client.AppService
{
    /// <summary> 
    /// This class is used as base class for classes that are processing messages of an application concurrently. Thus,
    /// an application can process more than one message in a time.
    /// MDS creates an instance of this class for every incoming message to process it.
    /// Maximum limit of messages that are being processed at the same time is configurable for individual applications.
    /// </summary>
    public abstract class MDSMessageProcessor : MDSAppServiceBase
    {
        /// <summary>
        /// Used to get/set if messages are auto acknowledged.
        /// If AutoAcknowledgeMessages is true, then messages are automatically acknowledged after MessageReceived event,
        /// if they are not acknowledged/rejected before by application.
        /// Default: true.
        /// </summary>
        protected bool AutoAcknowledgeMessages
        {
            get { return _autoAcknowledgeMessages; }
            set { _autoAcknowledgeMessages = value; }
        }
        private bool _autoAcknowledgeMessages = true;

        /// <summary>
        /// This method is called by MDS server to process the message, when a message is sent to this application.
        /// </summary>
        /// <param name="message">Message to process</param>
        public abstract void ProcessMessage(IIncomingMessage message);
    }
}
