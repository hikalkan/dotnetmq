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
using MDS.Communication.Messages;
using MDS.Serialization;

namespace MDS.Client.WebServices
{
    /// <summary>
    /// This class is used by MDS Web Services to serialize/deserialize/create messages.
    /// </summary>
    public static class WebServiceHelper
    {
        #region Public methods

        /// <summary>
        /// Deserializes an incoming message for Web Service from MDS server.
        /// </summary>
        /// <param name="bytesOfMessage">Message as byte array</param>
        /// <returns>Deserialized message</returns>
        public static IWebServiceIncomingMessage DeserializeMessage(byte[] bytesOfMessage)
        {
            var dataMessage = MDSSerializationHelper.DeserializeFromByteArray(() => new MDSDataTransferMessage(), bytesOfMessage);
            return new IncomingDataMessage(dataMessage);
        }

        /// <summary>
        /// Serializes a message to send to MDS server from Web Service.
        /// </summary>
        /// <param name="responseMessage">Message to serialize</param>
        /// <returns>Serialized message</returns>
        public static byte[] SerializeMessage(IWebServiceResponseMessage responseMessage)
        {
            CheckResponseMessage(responseMessage);
            var response = ((ResponseMessage) responseMessage).CreateDataTransferResponseMessage();
            return MDSSerializationHelper.SerializeToByteArray(response);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Checks a response message whether it is a valid response message
        /// </summary>
        /// <param name="responseMessage">Message to check</param>
        private static void CheckResponseMessage(IWebServiceResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException("responseMessage", "responseMessage can not be null.");
            }

            if (!(responseMessage is ResponseMessage))
            {
                throw new Exception("responseMessage parameter is not known type.");
            }

            if (responseMessage.Result == null)
            {
                throw new ArgumentNullException("responseMessage", "responseMessage.Result can not be null.");
            }

            if( !(responseMessage.Result is ResultMessage))
            {
                throw new Exception("responseMessage.Result is not known type.");
            }

            if(responseMessage.Message != null && !(responseMessage.Message is OutgoingDataMessage))
            {
                throw new Exception("responseMessage.Message is not known type.");
            }
        }

        #endregion

        #region Sub classes

        /// <summary>
        /// Implements IWebServiceIncomingMessage to be used by MDS web service.
        /// </summary>
        private class IncomingDataMessage : MDSDataTransferMessage, IWebServiceIncomingMessage
        {
            /// <summary>
            /// Creates a new IncomingDataMessage object from a MDSDataTransferMessage object.
            /// </summary>
            /// <param name="message">MDSDataTransferMessage object to create IncomingDataMessage</param>
            public IncomingDataMessage(MDSDataTransferMessage message)
            {
                DestinationApplicationName = message.DestinationApplicationName;
                DestinationCommunicatorId = message.DestinationCommunicatorId;
                DestinationServerName = message.DestinationServerName;
                MessageData = message.MessageData;
                MessageId = message.MessageId;
                PassedServers = message.PassedServers;
                RepliedMessageId = message.RepliedMessageId;
                SourceApplicationName = message.SourceApplicationName;
                SourceCommunicatorId = message.SourceCommunicatorId;
                SourceServerName = message.SourceServerName;
                TransmitRule = message.TransmitRule;
            }

            /// <summary>
            /// Creates IWebServiceResponseMessage using this incoming message, to return from web service to MDS server.
            /// </summary>
            /// <returns>Response message to this message</returns>
            public IWebServiceResponseMessage CreateResponseMessage()
            {
                return new ResponseMessage { Result = new ResultMessage { RepliedMessageId = MessageId } };
            }

            /// <summary>
            /// Creates IWebServiceOutgoingMessage using this incoming message, to return from web service to MDS server.
            /// </summary>
            /// <returns>Response message to this message</returns>
            public IWebServiceOutgoingMessage CreateResponseDataMessage()
            {
                return new OutgoingDataMessage
                {
                    DestinationApplicationName = SourceApplicationName,
                    DestinationCommunicatorId = SourceCommunicatorId,
                    DestinationServerName = SourceServerName,
                    RepliedMessageId = MessageId,
                    TransmitRule = TransmitRule
                };
            }
        }

        /// <summary>
        /// Implements IWebServiceOutgoingMessage to be used by MDS web service.
        /// </summary>
        private class OutgoingDataMessage : MDSDataTransferMessage, IWebServiceOutgoingMessage
        {
            //No data or method
        }
        
        /// <summary>
        /// Implements IWebServiceResponseMessage to be used by MDS web service.
        /// </summary>
        private class ResponseMessage : IWebServiceResponseMessage
        {
            public IWebServiceOperationResultMessage Result { get; set; }

            public IWebServiceOutgoingMessage Message { get; set; }

            public MDSDataTransferResponseMessage CreateDataTransferResponseMessage()
            {
                return new MDSDataTransferResponseMessage
                       {
                           Message = (MDSDataTransferMessage) Message,
                           Result = (MDSOperationResultMessage) Result
                       };
            }
        }

        /// <summary>
        /// Implements IWebServiceOperationResultMessage to be used by MDS web service.
        /// </summary>
        private class ResultMessage : MDSOperationResultMessage, IWebServiceOperationResultMessage
        {
            //No data or method            
        }

        #endregion
    }
}
