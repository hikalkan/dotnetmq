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
using log4net;
using MDS.Communication.Messages;
using MDS.Exceptions;
using MDS.MDSAppWebServiceRef;
using MDS.Serialization;
using MDS.Threading;

namespace MDS.Communication.WebServiceCommunication
{
    /// <summary>
    /// This class is used to communicate with a ASP.NET Web Service.
    /// </summary>
    public class WebServiceCommunicator : CommunicatorBase
    {
        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// URL of web service.
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// This queue is used to make web service calls sequentially.
        /// </summary>
        private readonly QueueProcessorThread<MDSDataTransferMessage> _outgoingMessageQueue;

        /// <summary>
        /// Creates a new WebServiceCommunicator object.
        /// </summary>
        /// <param name="url">URL of web service</param>
        /// <param name="comminicatorId">Communicator Id</param>
        public WebServiceCommunicator(string url, long comminicatorId)
            : base(comminicatorId)
        {
            _url = url;
            CommunicationWay = CommunicationWays.SendAndReceive;
            _outgoingMessageQueue = new QueueProcessorThread<MDSDataTransferMessage>();
            _outgoingMessageQueue.ProcessItem += OutgoingMessageQueue_ProcessItem;
        }

        /// <summary>
        /// Waits communicator to stop.
        /// </summary>
        public override void WaitToStop()
        {
            _outgoingMessageQueue.WaitToStop();
        }

        /// <summary>
        /// Prepares communication objects and starts outgoing message queue.
        /// </summary>
        protected override void StartCommunicaiton()
        {
            _outgoingMessageQueue.Start();
        }

        /// <summary>
        /// Starts outgoing message queue.
        /// </summary>
        /// <param name="waitToStop">True, to block caller thread until this object stops</param>
        protected override void StopCommunicaiton(bool waitToStop)
        {
            _outgoingMessageQueue.Stop(waitToStop);
        }

        /// <summary>
        /// This method is used to add a message to outgoing messages queue.
        /// It is called by CommunicatorBase.
        /// </summary>
        /// <param name="message">Message to send</param>
        protected override void SendMessageInternal(MDSMessage message)
        {
            if (message.MessageTypeId != MDSMessageFactory.MessageTypeIdMDSDataTransferMessage)
            {
                return;
            }

            _outgoingMessageQueue.Add(message as MDSDataTransferMessage);
        }

        /// <summary>
        /// This method is called to process a outgoing message.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void OutgoingMessageQueue_ProcessItem(object sender, ProcessQueueItemEventArgs<MDSDataTransferMessage> e)
        {
            try
            {
                SendMessageToWebService(e.ProcessItem);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Makes web service call, receives result and raises MessageReceived event.
        /// </summary>
        /// <param name="message"></param>
        private void SendMessageToWebService(MDSDataTransferMessage message)
        {
            using (var appService = new MDSAppService())
            {
                appService.Url = _url;
                var responseMessageBytes = appService.ReceiveMDSMessage(MDSSerializationHelper.SerializeToByteArray(message));
                if (responseMessageBytes == null)
                {
                    throw new MDSException("Response byte array from web service call is null.");
                }

                var responseMessage = MDSSerializationHelper.DeserializeFromByteArray(() => new MDSDataTransferResponseMessage(), responseMessageBytes);
                if (responseMessage.Result != null)
                {
                    OnMessageReceived(responseMessage.Result);
                }

                if (responseMessage.Message != null)
                {
                    OnMessageReceived(responseMessage.Message);
                }
            }
        }
    }
}
