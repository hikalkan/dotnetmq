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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;
using MDS.Communication.Messages.ControllerMessages;
using MDS.Exceptions;
using MDS.GUI;

namespace MDS.Management.GUI.ClientApplications
{
    public partial class ApplicationListForm : Form
    {
        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to MDSController object.
        /// All communication with MDS server is performed using this object.
        /// </summary>
        private readonly MDSController _controller;

        /// <summary>
        /// This list is used as DataSource of data grid.
        /// It stores client applications.
        /// </summary>
        private readonly List<ApplicationListItem> _applicationList;

        /// <summary>
        /// This list is used to set data grid's datasource to clear items.
        /// </summary>
        private readonly List<ApplicationListItem> _emptyList;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ApplicationListForm object.
        /// </summary>
        /// <param name="controller">Reference to MDSController object</param>
        public ApplicationListForm(MDSController controller)
        {
            _controller = controller;
            _applicationList = new List<ApplicationListItem>();
            _emptyList = new List<ApplicationListItem>();
            InitializeComponent();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method is called by MainForm when a ClientApplicationRefreshEventMessage received from MDS Server.
        /// </summary>
        /// <param name="message">Incoming message</param>
        public void GetClientApplicationRefreshEventMessage(ClientApplicationRefreshEventMessage message)
        {
            RefreshClientApplication(message);
        }

        /// <summary>
        /// This method is called by MainForm when a ClientApplicationRemovedEventMessage received from MDS Server.
        /// </summary>
        /// <param name="message"></param>
        public void GetClientApplicationRemovedEventMessage(ClientApplicationRemovedEventMessage message)
        {
            lock (_applicationList)
            {
                RemoveApplicationFromList(message.ApplicationName);
            }
        }

        #endregion

        #region Private methods

        #region Form events

        private void ApplicationList_Load(object sender, EventArgs e)
        {
            PrepareForm();
            GetApplicationsFromServer();
        }

        private void btnAddApplication_Click(object sender, EventArgs e)
        {
            var addApplicationForm = new AddNewApplicationForm();
            addApplicationForm.ShowDialog();
            if (string.IsNullOrEmpty(addApplicationForm.ApplicationName))
            {
                return;
            }

            try
            {
                _controller.SendMessage(
                    new AddNewApplicationMessage
                        {
                            ApplicationName = addApplicationForm.ApplicationName
                        });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Application can not be added. Detail: " + ex.Message);
            }
        }
        
        private void btnRemoveApplication_Click(object sender, EventArgs e)
        {
            lock (_applicationList)
            {
                //Check if user selected any application to remove
                if (gwApplicationList.SelectedRows.Count <= 0)
                {
                    return;
                }

                var selectedIndex = gwApplicationList.SelectedRows[0].Index;
                if ((selectedIndex < 0) || (selectedIndex >= _applicationList.Count))
                {
                    return;
                }

                //Get confirmation from user to remove application.
                var applicationName = _applicationList[selectedIndex].ApplicationName;
                var dialogResult =
                    MDSGuiHelper.ShowQuestionDialog("Are you sure to remove '" + applicationName + "' application from MDS",
                                                    "Attention! You are removing an application",
                                                    MessageBoxDefaultButton.Button2);
                if (dialogResult != DialogResult.Yes)
                {
                    return;
                }

                try
                {
                    //Send RemoveApplicationMessage to MDS server
                    var message = _controller.SendMessageAndGetResponse(
                        new RemoveApplicationMessage
                        {
                            ApplicationName = applicationName
                        });

                    //Check response message
                    if (message.MessageTypeId != ControlMessageFactory.MessageTypeIdRemoveApplicationResponseMessage)
                    {
                        throw new MDSException("Response message to RemoveApplicationMessage must be as RemoveApplicationResponseMessage");
                    }

                    var responseMessage = message as RemoveApplicationResponseMessage;
                    if (responseMessage == null)
                    {
                        throw new MDSException("Incorrect type. MessageTypeId = " + message.MessageTypeId + ", but Type of object: " + message.GetType().Name);
                    }

                    //Evaluate response message
                    if (responseMessage.Removed)
                    {
                        RemoveApplicationFromList(responseMessage.ApplicationName);
                    }
                    else
                    {
                        MDSGuiHelper.ShowWarningMessage(responseMessage.ResultMessage, applicationName + " application can not be removed!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    MDSGuiHelper.ShowErrorMessage(applicationName + " application can not be removed. Detail: " + ex.Message);
                }
            }
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            GetApplicationsFromServer();
        }
        
        private void editWebServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string applicationName;
            lock (_applicationList)
            {
                //Check if user selected any application to remove
                if (gwApplicationList.SelectedRows.Count <= 0)
                {
                    return;
                }

                var selectedIndex = gwApplicationList.SelectedRows[0].Index;
                if ((selectedIndex < 0) || (selectedIndex >= _applicationList.Count))
                {
                    return;
                }

                //Get confirmation from user to remove application.
                applicationName = _applicationList[selectedIndex].ApplicationName;
            }

            var editWebServiceForm = new EditApplicationWebServicesForm(_controller, applicationName);
            editWebServiceForm.ShowDialog();
        }

        #endregion

        #region Other private methods

        /// <summary>
        /// Gets all applications from server and fills into list.
        /// </summary>
        private void GetApplicationsFromServer()
        {
            try
            {
                //Send a message to MDS server to get list of client applications, get response and fill data grid.
                var message = _controller.SendMessageAndGetResponse(new GetApplicationListMessage());
                if (message.MessageTypeId != ControlMessageFactory.MessageTypeIdGetApplicationListResponseMessage)
                {
                    throw new MDSException("Response message to GetApplicationListMessage must be a GetApplicationListResponseMessage");
                }

                var applicationListMessage = message as GetApplicationListResponseMessage;
                if (applicationListMessage == null)
                {
                    throw new MDSException("Incorrect message type. MessageTypeId = " + message.MessageTypeId + ", but Type of object: " + message.GetType().Name);
                }

                FillApplicationList(applicationListMessage.ClientApplications);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Application list can not received from MDS Server. Detail: " + ex.Message);
            }
        }

        /// <summary>
        /// Fills data grid (gwApplicationList) by list of applications.
        /// </summary>
        /// <param name="applicationInfos">Application list</param>
        private void FillApplicationList(IEnumerable<GetApplicationListResponseMessage.ClientApplicationInfo> applicationInfos)
        {
            lock (_applicationList)
            {
                EmptyList();
                _applicationList.Clear();
                foreach (var applicationInfo in applicationInfos)
                {
                    _applicationList.Add(
                        new ApplicationListItem
                            {
                                ApplicationName = applicationInfo.Name,
                                ConnectedClients = applicationInfo.CommunicatorCount
                            });
                }

                RefreshList();
            }
        }

        /// <summary>
        /// Processes a ClientApplicationRefreshEventMessage.
        /// </summary>
        /// <param name="message">Message to process</param>
        private void RefreshClientApplication(ClientApplicationRefreshEventMessage message)
        {
            lock (_applicationList)
            {
                try
                {
                    EmptyList();

                    //Find application in list and change properties
                    foreach (var application in _applicationList)
                    {
                        if (message.Name.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase))
                        {
                            application.ConnectedClients = message.CommunicatorCount;
                            return;
                        }
                    }

                    //Add to list if it is not in list
                    _applicationList.Add(
                        new ApplicationListItem
                        {
                            ApplicationName = message.Name,
                            ConnectedClients = message.CommunicatorCount
                        });
                }
                finally
                {
                    RefreshList();
                }
            }
        }

        /// <summary>
        /// Removes application from shown application list.
        /// </summary>
        /// <param name="name">Name of the application</param>
        private void RemoveApplicationFromList(string name)
        {
            //Find application in list
            ApplicationListItem application = null;
            foreach (var applicationListItem in _applicationList)
            {
                if (name.Equals(applicationListItem.ApplicationName, StringComparison.OrdinalIgnoreCase))
                {
                    application = applicationListItem;
                    break;
                }
            }

            //No action if application is not in list.
            if (application == null)
            {
                return;
            }

            EmptyList();
            _applicationList.Remove(application);
            RefreshList();
        }

        /// <summary>
        /// Empties data grid.
        /// </summary>
        private void EmptyList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(EmptyList));
            }
            else
            {
                gwApplicationList.DataSource = _emptyList;
            }
        }

        /// <summary>
        /// Refreshes data grid.
        /// </summary>
        private void RefreshList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshList));
            }
            else
            {
                gwApplicationList.DataSource = _emptyList;
                gwApplicationList.DataSource = _applicationList;
            }
        }

        /// <summary>
        /// Prepares form. This method is called while form is opening.
        /// </summary>
        private void PrepareForm()
        {
            Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
        }

        #endregion

        #endregion

        #region Sub classes

        /// <summary>
        /// Represents an item in application list.
        /// </summary>
        private class ApplicationListItem
        {
            /// <summary>
            /// Name of the application.
            /// </summary>
            public string ApplicationName { get; set; }

            /// <summary>
            /// Currently connected communicator count.
            /// </summary>
            public int ConnectedClients { get; set; }
        }

        #endregion
    }
}
