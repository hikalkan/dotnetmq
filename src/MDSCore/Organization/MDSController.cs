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
using System.IO;
using MDS.Communication.Events;
using MDS.Communication.Messages.ControllerMessages;
using MDS.Exceptions;
using MDS.Serialization;
using MDS.Communication;
using MDS.Communication.Messages;
using MDS.Settings;

namespace MDS.Organization
{
    /// <summary>
    /// Represents a MDS controller that can monitor/manage to this server.
    /// </summary>
    public class MDSController : MDSRemoteApplication
    {
        #region Public fiedls

        /// <summary>
        /// Communicator type for Controllers.
        /// </summary>
        public override CommunicatorTypes CommunicatorType
        {
            get { return CommunicatorTypes.Controller; }
        }

        /// <summary>
        /// Reference to Organization Layer.
        /// </summary>
        public OrganizationLayer OrganizationLayer { private get; set; }

        #endregion

        #region Private fields

        /// <summary>
        /// Reference to Settings.
        /// </summary>
        private readonly MDSSettings _settings;

        /// <summary>
        /// Reference to Design Settings.
        /// </summary>
        private readonly MDSDesignSettings _designSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new MDSController object.
        /// </summary>
        /// <param name="name">Name of the controller</param>
        public MDSController(string name)
            : base(name, CommunicationLayer.CreateApplicationId())
        {
            _settings = MDSSettings.Instance;
            _designSettings = MDSDesignSettings.Instance;
            MessageReceived += MDSController_MessageReceived;
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            base.Start();

            //Register events of remote applications
            var applicationList = OrganizationLayer.GetClientApplications();
            foreach (var clientApplication in applicationList)
            {
                clientApplication.CommunicatorConnected += ClientApplication_CommunicatorConnected;
                clientApplication.CommunicatorDisconnected += ClientApplication_CommunicatorDisconnected;
            }
        }

        #endregion

        #region Private methods

        #region Message handling and processing mehods

        /// <summary>
        /// Handles MessageReceived event.
        /// All messages received from all controllers comes to this method.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void MDSController_MessageReceived(object sender, MessageReceivedFromRemoteApplicationEventArgs e)
        {
            try
            {
                //Response to Ping messages
                if ((e.Message.MessageTypeId == MDSMessageFactory.MessageTypeIdMDSPingMessage) && string.IsNullOrEmpty(e.Message.RepliedMessageId))
                {
                    //Reply ping message
                    SendMessage(new MDSPingMessage { RepliedMessageId = e.Message.MessageId }, e.Communicator);
                    return;
                }

                //Do not process messages other than MDSControllerMessage
                if (e.Message.MessageTypeId != MDSMessageFactory.MessageTypeIdMDSControllerMessage)
                {
                    return;
                }

                //Cast message to MDSControllerMessage
                var controllerMessage = e.Message as MDSControllerMessage;
                if (controllerMessage == null)
                {
                    return;
                }

                //Create (deserialize) ControlMessage from MessageData of controllerMessage object
                var controlMessage = MDSSerializationHelper.DeserializeFromByteArray(
                    () =>
                    ControlMessageFactory.CreateMessageByTypeId(controllerMessage.ControllerMessageTypeId),
                    controllerMessage.MessageData);

                //Process message
                ProcessControllerMessage(e.Communicator, controllerMessage, controlMessage);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        /// <summary>
        /// This methods checks type of message (MessageTypeId) and calls appropriate method to process message.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="controllerMessage">MDSControllerMessage object that includes controlMessage</param>
        /// <param name="controlMessage">The message to be processed</param>
        private void ProcessControllerMessage(ICommunicator communicator, MDSControllerMessage controllerMessage , ControlMessage controlMessage)
        {
            switch (controlMessage.MessageTypeId)
            {
                case ControlMessageFactory.MessageTypeIdGetApplicationListMessage:
                    ProcessGetApplicationListMessage(communicator, controllerMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdAddNewApplicationMessage:
                    ProcessAddNewApplicationMessage(controlMessage as AddNewApplicationMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdRemoveApplicationMessage:
                    ProcessRemoveApplicationMessage(communicator, controlMessage as RemoveApplicationMessage, controllerMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdGetServerGraphMessage:
                    ProcessGetServerGraphMessage(communicator, controllerMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdUpdateServerGraphMessage:
                    ProcessUpdateServerGraphMessage(communicator, controlMessage as UpdateServerGraphMessage, controllerMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdGetApplicationWebServicesMessage:
                    ProcessGetApplicationWebServicesMessage(communicator, controlMessage as GetApplicationWebServicesMessage, controllerMessage);
                    break;
                case ControlMessageFactory.MessageTypeIdUpdateApplicationWebServicesMessage:
                    ProcessUpdateApplicationWebServicesMessage(communicator, controlMessage as UpdateApplicationWebServicesMessage, controllerMessage);
                    break;
                default:
                    throw new MDSException("Undefined MessageTypeId for ControlMessage: " + controlMessage.MessageTypeId);
            }
        }

        #region GetApplicationListMessage

        /// <summary>
        /// Processes GetApplicationListMessage.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="controllerMessage">MDSControllerMessage object that includes controlMessage</param>
        private void ProcessGetApplicationListMessage(ICommunicator communicator, MDSControllerMessage controllerMessage)
        {
            //Get all client applications
            var applicationList = OrganizationLayer.GetClientApplications();

            //Create ClientApplicationInfo array
            var clientApplications = new GetApplicationListResponseMessage.ClientApplicationInfo[applicationList.Length];
            for (var i = 0; i < applicationList.Length; i++)
            {
                clientApplications[i] = new GetApplicationListResponseMessage.ClientApplicationInfo
                                         {
                                             Name = applicationList[i].Name,
                                             CommunicatorCount = applicationList[i].ConnectedCommunicatorCount
                                         };
            }

            //Send response message
            ReplyMessageToCommunicator(
                communicator,
                new GetApplicationListResponseMessage
                    {
                        ClientApplications = clientApplications
                    },
                controllerMessage
                );
        }

        #endregion

        #region AddNewApplicationMessage

        /// <summary>
        /// Processes AddNewApplicationMessage.
        /// </summary>
        /// <param name="controlMessage">The message to be processed</param>
        private void ProcessAddNewApplicationMessage(AddNewApplicationMessage controlMessage)
        {
            var addedApplication = OrganizationLayer.AddApplication(controlMessage.ApplicationName);
            addedApplication.CommunicatorConnected += ClientApplication_CommunicatorConnected;
            addedApplication.CommunicatorDisconnected += ClientApplication_CommunicatorDisconnected;
            SendMessageToAllReceivers(
                new ClientApplicationRefreshEventMessage
                    {
                        Name = addedApplication.Name,
                        CommunicatorCount = addedApplication.ConnectedCommunicatorCount
                    });
        }

        #endregion

        #region RemoveApplicationMessage

        /// <summary>
        /// Processes RemoveApplicationMessage.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="controlMessage">The message to be processed</param>
        /// <param name="controllerMessage">MDSControllerMessage object that includes controlMessage</param>
        private void ProcessRemoveApplicationMessage(ICommunicator communicator, RemoveApplicationMessage controlMessage, MDSControllerMessage controllerMessage)
        {
            try
            {
                var removedApplication = OrganizationLayer.RemoveApplication(controlMessage.ApplicationName);
                removedApplication.CommunicatorConnected -= ClientApplication_CommunicatorConnected;
                removedApplication.CommunicatorDisconnected -= ClientApplication_CommunicatorDisconnected;

                ReplyMessageToCommunicator(
                    communicator,
                    new RemoveApplicationResponseMessage
                        {
                            ApplicationName = controlMessage.ApplicationName,
                            Removed = true,
                            ResultMessage = "Success."
                        },
                    controllerMessage
                    );

                SendMessageToAllReceivers(
                    new ClientApplicationRemovedEventMessage
                        {
                            ApplicationName = removedApplication.Name
                        });
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                ReplyMessageToCommunicator(
                    communicator,
                    new RemoveApplicationResponseMessage
                        {
                            ApplicationName = controlMessage.ApplicationName,
                            Removed = false,
                            ResultMessage = ex.Message
                        },
                    controllerMessage
                    );
            }
        }

        #endregion

        #region GetServerGraphMessage

        /// <summary>
        /// Processes GetServerGraphMessage.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="controllerMessage">MDSControllerMessage object that includes controlMessage</param>
        private void ProcessGetServerGraphMessage(ICommunicator communicator, MDSControllerMessage controllerMessage)
        {
            //Create response message
            var responseMessage =
                new GetServerGraphResponseMessage
                    {
                        ServerGraph =
                            new ServerGraphInfo
                                {
                                    ThisServerName = _settings.ThisServerName,
                                    Servers = new ServerGraphInfo.ServerOnGraph[_settings.Servers.Count]
                                }
                    };

            //Fill server settings
            for (var i = 0; i < _settings.Servers.Count; i++)
            {
                responseMessage.ServerGraph.Servers[i] = new ServerGraphInfo.ServerOnGraph
                                                             {
                                                                 Name = _settings.Servers[i].Name,
                                                                 IpAddress = _settings.Servers[i].IpAddress,
                                                                 Port = _settings.Servers[i].Port,
                                                                 Adjacents = _settings.Servers[i].Adjacents
                                                             };
            }

            //Fill server design settings
            for (var i = 0; i < responseMessage.ServerGraph.Servers.Length; i++)
            {
                foreach (var serverDesignItem in _designSettings.Servers)
                {
                    if (responseMessage.ServerGraph.Servers[i].Name == serverDesignItem.Name)
                    {
                        responseMessage.ServerGraph.Servers[i].Location = serverDesignItem.Location;
                        break;
                    }
                }
            }

            //Send response message
            ReplyMessageToCommunicator(
                communicator,
                responseMessage,
                controllerMessage
                );
        }

        #endregion

        #region UpdateServerGraphMessage

        /// <summary>
        /// Processes UpdateServerGraphMessage.
        /// </summary>
        /// <param name="communicator">Communicator that sent message</param>
        /// <param name="controlMessage">The message to be processed</param>
        /// <param name="controllerMessage">MDSControllerMessage object that includes controlMessage</param>
        private void ProcessUpdateServerGraphMessage(ICommunicator communicator, UpdateServerGraphMessage controlMessage, MDSControllerMessage controllerMessage)
        {
            try
            {
                var newSettings = new MDSSettings(Path.Combine(GeneralHelper.GetCurrentDirectory(), "MDSSettings.xml"));
                var newDesignSettings = new MDSDesignSettings(Path.Combine(GeneralHelper.GetCurrentDirectory(), "MDSSettings.design.xml"));

                //Clear existing server lists
                newSettings.Servers.Clear();
                newDesignSettings.Servers.Clear();

                //Add servers from UpdateServerGraphMessage
                newSettings.ThisServerName = controlMessage.ServerGraph.ThisServerName;
                foreach (var server in controlMessage.ServerGraph.Servers)
                {
                    //Settings
                    newSettings.Servers.Add(
                        new ServerInfoItem
                        {
                            Name = server.Name,
                            IpAddress = server.IpAddress,
                            Port = server.Port,
                            Adjacents = server.Adjacents
                        });
                    //Design settings
                    newDesignSettings.Servers.Add(
                        new ServerDesignItem
                        {
                            Name = server.Name,
                            Location = server.Location
                        });
                }

                //Save settings
                newSettings.SaveToXml();
                newDesignSettings.SaveToXml();
            }
            catch (Exception ex)
            {
                //Send fail message
                ReplyMessageToCommunicator(
                    communicator,
                    new OperationResultMessage {Success = false, ResultMessage = ex.Message},
                    controllerMessage
                    );
                return;
            }

            //Send success message
            ReplyMessageToCommunicator(
                communicator,
                new OperationResultMessage {Success = true, ResultMessage = "Success"},
                controllerMessage
                );
        }

        #endregion

        #region GetApplicationWebServicesMessage

        private void ProcessGetApplicationWebServicesMessage(ICommunicator communicator, GetApplicationWebServicesMessage message, MDSControllerMessage controllerMessage)
        {
            try
            {
                //Find application
                ApplicationInfoItem application = null;
                foreach (var applicationInfoItem in _settings.Applications)
                {
                    if(applicationInfoItem.Name == message.ApplicationName)
                    {
                        application = applicationInfoItem;
                    }
                }

                if(application == null)
                {
                    //Send message
                    ReplyMessageToCommunicator(
                        communicator,
                        new GetApplicationWebServicesResponseMessage
                            {
                                WebServices = null,
                                Success = false,
                                ResultText = "No application found with name '" + message.ApplicationName + "'."
                            },
                        controllerMessage
                        );

                    return;
                }

                var webServiceList = new List<ApplicationWebServiceInfo>();
                foreach (var channel in application.CommunicationChannels)
                {
                    if ("WebService".Equals(channel.CommunicationType, StringComparison.OrdinalIgnoreCase))
                    {
                        webServiceList.Add(new ApplicationWebServiceInfo {Url = channel.CommunicationSettings["Url"]});
                    }
                }

                //Send web service list
                ReplyMessageToCommunicator(
                    communicator,
                    new GetApplicationWebServicesResponseMessage
                        {
                            WebServices = webServiceList.ToArray(),
                            Success = true,
                            ResultText = "Success."
                        },
                    controllerMessage
                    );
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        #endregion

        #region UpdateApplicationWebServicesMessage

        private void ProcessUpdateApplicationWebServicesMessage(ICommunicator communicator, UpdateApplicationWebServicesMessage message, MDSControllerMessage controllerMessage)
        {
            try
            {
                //Find application
                ApplicationInfoItem application = null;
                foreach (var applicationInfoItem in _settings.Applications)
                {
                    if (applicationInfoItem.Name == message.ApplicationName)
                    {
                        application = applicationInfoItem;
                    }
                }

                if (application == null)
                {
                    //Send message
                    ReplyMessageToCommunicator(
                        communicator,
                        new OperationResultMessage()
                        {
                            Success = false,
                            ResultMessage = "No application found with name '" + message.ApplicationName + "'."
                        },
                        controllerMessage
                        );
                    return;
                }

                //Delete old service list
                application.CommunicationChannels.Clear();

                //Add new services
                if (message.WebServices != null && message.WebServices.Length > 0)
                {
                    foreach (var webServiceInfo in message.WebServices)
                    {
                        var channelInfo = new ApplicationInfoItem.CommunicationChannelInfoItem { CommunicationType = "WebService" };
                        channelInfo.CommunicationSettings["Url"] = webServiceInfo.Url;
                        application.CommunicationChannels.Add(channelInfo);
                    }
                }

                try
                {
                    //Save settings
                    _settings.SaveToXml();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    ReplyMessageToCommunicator(
                        communicator,
                        new OperationResultMessage()
                        {
                            Success = false,
                            ResultMessage = "Can not save XML configuration file (MDSSettings.xml)."
                        },
                        controllerMessage
                        );
                    return;
                }

                //Send success message
                ReplyMessageToCommunicator(
                    communicator,
                    new OperationResultMessage()
                    {
                        Success = true,
                        ResultMessage = "Success."
                    },
                    controllerMessage
                    );
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                ReplyMessageToCommunicator(
                    communicator,
                    new OperationResultMessage()
                    {
                        Success = false,
                        ResultMessage = ex.Message
                    },
                    controllerMessage
                    );
                return;
            }
        }

        #endregion

        #endregion

        #region Other Event handling methods

        /// <summary>
        /// Handles CommunicatorConnected event of all client applications.
        /// </summary>
        /// <param name="sender">Creates of event (application)</param>
        /// <param name="e">Event arguments</param>
        private void ClientApplication_CommunicatorConnected(object sender, CommunicatorConnectedEventArgs e)
        {
            var application = sender as MDSRemoteApplication;
            if (application == null)
            {
                return;
            }

            SendMessageToAllReceivers(new ClientApplicationRefreshEventMessage
                                          {
                                              Name = application.Name,
                                              CommunicatorCount = application.ConnectedCommunicatorCount
                                          });
        }

        /// <summary>
        /// Handles CommunicatorDisconnected event of all client applications.
        /// </summary>
        /// <param name="sender">Creates of event (application)</param>
        /// <param name="e">Event arguments</param>
        void ClientApplication_CommunicatorDisconnected(object sender, CommunicatorDisconnectedEventArgs e)
        {
            var application = sender as MDSRemoteApplication;
            if (application == null)
            {
                return;
            }

            SendMessageToAllReceivers(new ClientApplicationRefreshEventMessage
                                          {
                                              Name = application.Name,
                                              CommunicatorCount = application.ConnectedCommunicatorCount
                                          });
        }

        #endregion

        #region Other private methods

        /// <summary>
        /// Sends a ControlMessage to all connected MDSController instances.
        /// </summary>
        /// <param name="message">Message to send</param>
        private void SendMessageToAllReceivers(ControlMessage message)
        {
            var outgoingMessage = new MDSControllerMessage
            {
                ControllerMessageTypeId = message.MessageTypeId,
                MessageData = MDSSerializationHelper.SerializeToByteArray(message)
            };

            var receivers = GetAllReceiverCommunicators();
            foreach (var receiver in receivers)
            {
                try
                {
                    SendMessage(outgoingMessage, receiver);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Sends a message to a spesific communicator as a reply to an incoming message.
        /// </summary>
        /// <param name="communicator">Communicator to send message</param>
        /// <param name="message">Message to send</param>
        /// <param name="incomingMessage">Incoming message which is being replied</param>
        private void ReplyMessageToCommunicator(ICommunicator communicator, ControlMessage message, MDSControllerMessage incomingMessage)
        {
            //Create MDSControllerMessage that includes serialized GetApplicationListResponseMessage message
            var outgoingMessage = new MDSControllerMessage
            {
                ControllerMessageTypeId = message.MessageTypeId,
                MessageData = MDSSerializationHelper.SerializeToByteArray(message),
                RepliedMessageId = incomingMessage.MessageId
            };
            //Send message to communicator that sent to message
            SendMessage(outgoingMessage, communicator);
        }

        #endregion

        #endregion
    }
}
