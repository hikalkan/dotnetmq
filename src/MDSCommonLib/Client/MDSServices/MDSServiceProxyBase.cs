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

namespace MDS.Client.MDSServices
{
    /// <summary>
    /// This is the base class for proxy classes that is used to make remote invocation to a MDSService.
    /// </summary>
    public abstract class MDSServiceProxyBase
    {
        /// <summary>
        /// Reference to the underlying MDSServiceConsumer object to send/receive MDS messages.
        /// </summary>
        private readonly MDSServiceConsumer _serviceConsumer;

        /// <summary>
        /// MDS Address of remote application.
        /// </summary>
        public MDSRemoteAppEndPoint RemoteApplication { get; private set; }

        /// <summary>
        /// Transmit rule of underlying messages.
        /// Using this peoperty, connection between applications can be changes from tight coupled to loose coupled and vice versa.
        /// Just methods whose return type is void, can use other transmit rule than DirectlySend. So, that methods may be loose coupled by setting transmit rule.
        /// Methods that has return value always use DirectlySend transmit rule, even if it is set by user to another rule. So, that methods must be tight coupled.
        /// Default value: DirectlySend.
        /// </summary>
        public MessageTransmitRules TransmitRule { get; set; }

        /// <summary>
        /// Timeout for service method calls as milliseconds.
        /// Default: 300000 (5 minutes).
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Name of the service class. 
        /// </summary>
        private readonly string _serviceClassName;

        /// <summary>
        /// Initializes MDSServiceProxyBase.
        /// </summary>
        /// <param name="serviceConsumer">Reference to a MDSServiceConsumer object to send/receive MDS messages</param>
        /// <param name="remoteEndPoint">Address of remote application</param>
        /// <param name="serviceClassName">Name of the service class</param>
        protected MDSServiceProxyBase(MDSServiceConsumer serviceConsumer, MDSRemoteAppEndPoint remoteEndPoint, string serviceClassName)
        {
            if (string.IsNullOrEmpty(serviceClassName))
            {
                throw new ArgumentNullException("serviceClassName");
            }

            if (remoteEndPoint == null)
            {
                remoteEndPoint = new MDSRemoteAppEndPoint();
            }

            _serviceConsumer = serviceConsumer;
            RemoteApplication = remoteEndPoint;
            _serviceClassName = serviceClassName;
            TransmitRule = MessageTransmitRules.DirectlySend;
            Timeout = 300000;
        }

        /// <summary>
        /// Sends remote method invocation message to the remote application and gets result.
        /// This simplifies remove method invocation like calling a method locally.
        /// It throws Exception if any Exception occured on remote application's method.
        /// </summary>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="args">Method parameters</param>
        /// <returns>Return value of remote method</returns>
        protected object InvokeRemoteMethodAndGetResult(string methodName, params object[] args)
        {
            //Create MDSRemoteInvokeMessage object that contains invocation informations
            var invokeMessage = new MDSRemoteInvokeMessage { ServiceClassName = _serviceClassName, MethodName = methodName, Parameters = args };

            //Create MDS message to transmit MDSRemoteInvokeMessage.
            var outgoingMessage = _serviceConsumer.MdsClient.CreateMessage();
            outgoingMessage.DestinationServerName = RemoteApplication.ServerName;
            outgoingMessage.DestinationApplicationName = RemoteApplication.ApplicationName;
            outgoingMessage.DestinationCommunicatorId = RemoteApplication.CommunicatorId;
            outgoingMessage.TransmitRule = MessageTransmitRules.DirectlySend;
            outgoingMessage.MessageData = GeneralHelper.SerializeObject(invokeMessage);

            //Send message and get response
            var incomingMessage = outgoingMessage.SendAndGetResponse(Timeout);
            incomingMessage.Acknowledge();

            //Deserialize and check return value
            var invokeReturnMessage = (MDSRemoteInvokeReturnMessage) GeneralHelper.DeserializeObject(incomingMessage.MessageData);
            if (invokeReturnMessage.RemoteException != null)
            {
                throw invokeReturnMessage.RemoteException;
            }

            //Success
            return invokeReturnMessage.ReturnValue;
        }

        /// <summary>
        /// Sends remote method invocation message to the remote application and gets result.
        /// This simplifies remove method invocation like calling a method locally.
        /// It throws Exception if any Exception occured on remote application's method.
        /// </summary>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="args">Method parameters</param>
        protected void InvokeRemoteMethod(string methodName, params object[] args)
        {
            //Create MDSRemoteInvokeMessage object that contains invocation informations
            var invokeMessage = new MDSRemoteInvokeMessage
                                {
                                    ServiceClassName = _serviceClassName,
                                    MethodName = methodName,
                                    Parameters = args
                                };

            //Create MDS message to transmit MDSRemoteInvokeMessage.
            var outgoingMessage = _serviceConsumer.MdsClient.CreateMessage();
            outgoingMessage.DestinationServerName = RemoteApplication.ServerName;
            outgoingMessage.DestinationApplicationName = RemoteApplication.ApplicationName;
            outgoingMessage.DestinationCommunicatorId = RemoteApplication.CommunicatorId;
            outgoingMessage.TransmitRule = TransmitRule;
            outgoingMessage.MessageData = GeneralHelper.SerializeObject(invokeMessage);

            if (TransmitRule == MessageTransmitRules.DirectlySend)
            {
                //Send message and get response
                var incomingMessage = outgoingMessage.SendAndGetResponse(Timeout);
                incomingMessage.Acknowledge();

                //Deserialize and check return value
                var invokeReturnMessage = (MDSRemoteInvokeReturnMessage)GeneralHelper.DeserializeObject(incomingMessage.MessageData);
                if (invokeReturnMessage.RemoteException != null)
                {
                    throw invokeReturnMessage.RemoteException;
                }
            }
            else
            {
                //Just send message
                outgoingMessage.Send();
            }
        }
    }
}
