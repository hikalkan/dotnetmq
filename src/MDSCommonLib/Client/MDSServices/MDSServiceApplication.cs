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
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MDS.Communication;
using MDS.Communication.Messages;
using MDS.Exceptions;

namespace MDS.Client.MDSServices
{
    /// <summary>
    /// This class ensures to use a class that is derived from MDSService class, as a service on MDS.
    /// </summary>
    public class MDSServiceApplication : IDisposable
    {
        #region Private fields

        /// <summary>
        /// Underlying MDSClient object to send/receive MDS messages.
        /// </summary>
        private readonly MDSClient _mdsClient;

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The service object that handles all method invokes.
        /// </summary>
        private SortedList<string, ServiceObject> _serviceObjects;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new MDSServiceApplication object with default values to connect to MDS server.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        public MDSServiceApplication(string applicationName)
        {
            _mdsClient = new MDSClient(applicationName, CommunicationConsts.DefaultIpAddress, CommunicationConsts.DefaultMDSPort);
            Initialize();
        }

        /// <summary>
        /// Creates a new MDSServiceApplication object with default port to connect to MDS server.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        /// <param name="ipAddress">IP address of MDS server</param>
        public MDSServiceApplication(string applicationName, string ipAddress)
        {
            _mdsClient = new MDSClient(applicationName, ipAddress, CommunicationConsts.DefaultMDSPort);
            Initialize();
        }

        /// <summary>
        /// Creates a new MDSServiceApplication object.
        /// </summary>
        /// <param name="applicationName">Name of the application</param>
        /// <param name="ipAddress">IP address of MDS server</param>
        /// <param name="port">TCP port of MDS server</param>
        public MDSServiceApplication(string applicationName, string ipAddress, int port) 
        {
            _mdsClient = new MDSClient(applicationName, ipAddress, port);
            Initialize();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method connects to MDS server using underlying MDSClient object.
        /// </summary>
        public void Connect()
        {
            _mdsClient.Connect();
        }

        /// <summary>
        /// This method disconnects from MDS server using underlying MDSClient object.
        /// </summary>
        public void Disconnect()
        {
            _mdsClient.Disconnect();
        }
        
        /// <summary>
        /// Adds a new MDSService for this service application.
        /// </summary>
        /// <param name="service">Service to add</param>
        public void AddService(MDSService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var type = service.GetType();
            var attributes = type.GetCustomAttributes(typeof (MDSServiceAttribute), true);
            if(attributes.Length <= 0)
            {
                throw new MDSException("Service class must has MDSService attribute to be added.");
            }

            if (_serviceObjects.ContainsKey(type.Name))
            {
                throw new MDSException("Service '" + type.Name + "' is already added.");
            }

            _serviceObjects.Add(type.Name, new ServiceObject(service, (MDSServiceAttribute)attributes[0]));
        }

        /// <summary>
        /// Removes a MDSService from this service application.
        /// </summary>
        /// <param name="service">Service to add</param>
        public void RemoveService(MDSService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var type = service.GetType();
            if (!_serviceObjects.ContainsKey(type.Name))
            {
                return;
            }

            _serviceObjects.Remove(type.Name);
        }

        /// <summary>
        /// Disposes this object, disposes/closes underlying MDSClient object.
        /// </summary>
        public void Dispose()
        {
            _mdsClient.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes this object.
        /// </summary>
        private void Initialize()
        {
            _serviceObjects = new SortedList<string, ServiceObject>();
            _mdsClient.MessageReceived += MdsClient_MessageReceived;
        }
        
        /// <summary>
        /// This method handles all incoming messages from MDS server.
        /// </summary>
        /// <param name="sender">Creator object of method (MDSClient object)</param>
        /// <param name="e">Message event arguments</param>
        private void MdsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Deserialize message
            MDSRemoteInvokeMessage invokeMessage;
            try
            {
                invokeMessage = (MDSRemoteInvokeMessage)GeneralHelper.DeserializeObject(e.Message.MessageData);
            }
            catch (Exception ex)
            {
                AcknowledgeMessage(e.Message);
                SendException(e.Message, new MDSRemoteException("Incoming message can not be deserialized to MDSRemoteInvokeMessage.", ex));
                return;
            }
            
            //Check service class name
            if (!_serviceObjects.ContainsKey(invokeMessage.ServiceClassName))
            {
                AcknowledgeMessage(e.Message);
                SendException(e.Message, new MDSRemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
                return;
            }

            //Get service object
            var serviceObject = _serviceObjects[invokeMessage.ServiceClassName];
            
            //Invoke service method and get return value
            object returnValue;
            try
            {
                //Set request variables to service object and invoke method
                serviceObject.Service.IncomingMessage = e.Message;
                returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);
            }
            catch (Exception ex)
            {
                SendException(e.Message,
                              new MDSRemoteException(
                                  ex.Message + Environment.NewLine + "Service Class: " +
                                  invokeMessage.ServiceClassName + " " + Environment.NewLine +
                                  "Service Version: " + serviceObject.ServiceAttribute.Version, ex));
                return;
            }

            //Send return value to sender application
            SendReturnValue(e.Message, returnValue);
        }


        /// <summary>
        /// Sends an Exception to the remote application that invoked a service method
        /// </summary>
        /// <param name="incomingMessage">Incoming invoke message from remote application</param>
        /// <param name="exception">Exception to send</param>
        private static void SendException(IIncomingMessage incomingMessage, MDSRemoteException exception)
        {
            if (incomingMessage.TransmitRule != MessageTransmitRules.DirectlySend)
            {
                return;
            }

            try
            {
                //Create return message
                var returnMessage = new MDSRemoteInvokeReturnMessage { RemoteException = exception };
                //Create response message and send
                var responseMessage = incomingMessage.CreateResponseMessage();
                responseMessage.MessageData = GeneralHelper.SerializeObject(returnMessage);
                responseMessage.Send();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends return value to the remote application that invoked a service method.
        /// </summary>
        /// <param name="incomingMessage">Incoming invoke message from remote application</param>
        /// <param name="returnValue">Return value to send</param>
        private static void SendReturnValue(IIncomingMessage incomingMessage, object returnValue)
        {
            if (incomingMessage.TransmitRule != MessageTransmitRules.DirectlySend)
            {
                return;
            }

            try
            {
                //Create return message
                var returnMessage = new MDSRemoteInvokeReturnMessage { ReturnValue = returnValue };
                //Create response message and send
                var responseMessage = incomingMessage.CreateResponseMessage();
                responseMessage.MessageData = GeneralHelper.SerializeObject(returnMessage);
                responseMessage.Send();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Acknowledges a message.
        /// </summary>
        /// <param name="message">Message to acknowledge</param>
        private static void AcknowledgeMessage(IIncomingMessage message)
        {
            try
            {
                message.Acknowledge();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        #endregion

        #region Sub classes

        /// <summary>
        /// Represents a MDSService object.
        /// </summary>
        private class ServiceObject
        {
            /// <summary>
            /// The service object that is used to invoke methods on.
            /// </summary>
            public MDSService Service { get; private set; }

            /// <summary>
            /// MDSService attribute of Service object's class.
            /// </summary>
            public MDSServiceAttribute ServiceAttribute { get; private set; }

            /// <summary>
            /// Name of the Service object's class.
            /// </summary>
            private readonly string _serviceClassName;
            
            /// <summary>
            /// This collection stores a list of all methods of T, if that can be invoked from remote applications or not.
            /// Key: Method name
            /// Value: True, if it can be invoked from remote application. 
            /// </summary>
            private readonly SortedList<string, bool> _methods;
            
            /// <summary>
            /// Creates a new ServiceObject.
            /// </summary>
            /// <param name="service">The service object that is used to invoke methods on</param>
            /// <param name="serviceAttribute">MDSService attribute of service object's class</param>
            public ServiceObject(MDSService service, MDSServiceAttribute serviceAttribute)
            {
                Service = service;
                ServiceAttribute = serviceAttribute;

                _serviceClassName = service.GetType().Name;

                //Find all methods
                _methods = new SortedList<string, bool>();
                foreach (var methodInfo in Service.GetType().GetMethods())
                {
                    var attributes = methodInfo.GetCustomAttributes(typeof(MDSServiceMethodAttribute), true);
                    _methods.Add(methodInfo.Name, attributes.Length > 0);
                }
            }

            /// <summary>
            /// Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters)
            {
                //Check if there is a method with name methodName
                if (!_methods.ContainsKey(methodName))
                {
                    AcknowledgeMessage(Service.IncomingMessage);
                    throw new MethodAccessException("There is not a method with name '" + methodName + "' in service '" + _serviceClassName + "'.");
                }

                //Check if method has MDSServiceMethod attribute
                if (!_methods[methodName])
                {
                    AcknowledgeMessage(Service.IncomingMessage);
                    throw new MethodAccessException("Method '" + methodName + "' has not MDSServiceMethod attribute. It can not be invoked from remote applications.");
                }

                //Set object properties before method invocation
                Service.RemoteApplication =
                    new MDSRemoteAppEndPoint(Service.IncomingMessage.SourceServerName,
                                             Service.IncomingMessage.SourceApplicationName,
                                             Service.IncomingMessage.SourceCommunicatorId);

                //If Transmit rule is DirectlySend than message must be acknowledged now.
                //If any exception occurs on method invocation, exception will be sent to
                //remote application in a MDSRemoteInvokeReturnMessage.
                if (Service.IncomingMessage.TransmitRule == MessageTransmitRules.DirectlySend)
                {
                    AcknowledgeMessage(Service.IncomingMessage);
                }

                //Invoke method and get return value
                try
                {
                    var returnValue = Service.GetType().GetMethod(methodName).Invoke(Service, parameters);
                    if (Service.IncomingMessage.TransmitRule != MessageTransmitRules.DirectlySend)
                    {
                        AcknowledgeMessage(Service.IncomingMessage);
                    }

                    return returnValue;
                }
                catch (Exception ex)
                {
                    if (Service.IncomingMessage.TransmitRule != MessageTransmitRules.DirectlySend)
                    {
                        RejectMessage(Service.IncomingMessage, ex.Message);
                    }

                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }

                    throw;
                }
            }

            /// <summary>
            /// Rejects a message.
            /// </summary>
            /// <param name="message">Message to reject</param>
            /// <param name="reason">Reject reason</param>
            private static void RejectMessage(IIncomingMessage message, string reason)
            {
                try
                {
                    message.Reject(reason);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }
        }

        #endregion
    }
}
