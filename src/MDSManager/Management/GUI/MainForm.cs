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
using System.Windows.Forms;
using log4net;
using MDS.Communication.Messages.ControllerMessages;
using MDS.Exceptions;
using MDS.GUI;
using MDS.Management.GUI.ClientApplications;
using MDS.Management.GUI.MDSServers;

namespace MDS.Management.GUI
{
    /// <summary>
    /// This is the main form of the application.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Private fields

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// All communication with MDS server is performed using this object.
        /// </summary>
        private readonly MDSController _controller;

        #region References to the open forms

        private ServerGraphForm _serverGraphForm;

        private ApplicationListForm _applicationListForm;

        private RoutesForm _routesForm;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public MainForm(MDSController controller)
        {
            InitializeComponent();
            _controller = controller;
            _controller.ControlMessageReceived += Controller_ControlMessageReceived;
        }

        #endregion

        #region Private methods

        #region Sub form open/close

        #region ApplicationListForm

        private void ApplicationListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_applicationListForm == null)
            {
                var form = new ApplicationListForm(_controller) {MdiParent = this};
                form.FormClosed += ApplicationList_FormClosed;
                form.Show();
                _applicationListForm = form;
            }
            else
            {
                _applicationListForm.Activate();
            }
        }

        private void ApplicationList_FormClosed(object sender, FormClosedEventArgs e)
        {
            _applicationListForm = null;
        }

        #endregion

        #region ServerGraphForm

        private void ServerGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_serverGraphForm == null)
            {
                _serverGraphForm = new ServerGraphForm(_controller) { MdiParent = this };
                _serverGraphForm.FormClosed += ServerGraphForm_FormClosed;
                _serverGraphForm.Show();
            }
            else
            {
                _serverGraphForm.Activate();
            }
        }

        private void ServerGraphForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _serverGraphForm = null;
        }

        #endregion

        #region RoutesForm

        private void RoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_routesForm == null)
            {
                _routesForm = new RoutesForm() { MdiParent = this };
                _routesForm.FormClosed += RoutesForm_FormClosed;
                _routesForm.Show();
            }
            else
            {
                _routesForm.Activate();
            }
        }

        private void RoutesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _routesForm = null;
        }

        #endregion

        #endregion

        #region MainForm event handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _controller.Connect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Can not connected to MDS Server. Detail: " + ex.Message);
                Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _controller.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }
        }

        #endregion

        #region Incoming message handling and processing

        /// <summary>
        /// This method handles ControlMessageReceived event of _controller object.
        /// It calls appropriate method to process message according to message's type.
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event arguments</param>
        private void Controller_ControlMessageReceived(object sender, ControlMessageReceivedEventArgs e)
        {
            try
            {
                switch (e.Message.MessageTypeId)
                {
                    case ControlMessageFactory.MessageTypeIdClientApplicationRefreshEventMessage:
                        ProcessClientApplicationRefreshEventMessage(e.Message as ClientApplicationRefreshEventMessage);
                        break;
                    case ControlMessageFactory.MessageTypeIdClientApplicationRemovedEventMessage:
                        ProcessClientApplicationRemovedEventMessage(e.Message as ClientApplicationRemovedEventMessage);
                        break;
                    default:
                        throw new MDSException("Undefined MessageTypeId for ControlMessage: " + e.Message.MessageTypeId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Processes a ClientApplicationRefreshEventMessage message.
        /// </summary>
        /// <param name="message">Message to process</param>
        private void ProcessClientApplicationRefreshEventMessage(ClientApplicationRefreshEventMessage message)
        {
            if (_applicationListForm != null)
            {
                _applicationListForm.GetClientApplicationRefreshEventMessage(message);
            }
        }

        /// <summary>
        /// Processes a ClientApplicationRemovedEventMessage message.
        /// </summary>
        /// <param name="message">Message to process</param>
        private void ProcessClientApplicationRemovedEventMessage(ClientApplicationRemovedEventMessage message)
        {
            if (_applicationListForm != null)
            {
                _applicationListForm.GetClientApplicationRemovedEventMessage(message);
            }
        }

        #endregion
        
        #endregion
    }
}
