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
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MDS.Communication.Messages.ControllerMessages;
using MDS.Exceptions;
using MDS.GUI;

namespace MDS.Management.GUI.ClientApplications
{
    /// <summary>
    /// This form is used to edit web service communicators of an application.
    /// </summary>
    public partial class EditApplicationWebServicesForm : Form
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
        /// It stores web services of application.
        /// </summary>
        private readonly BindingList<WebServiceListItem> _webServicesList;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">Reference to MDSController object</param>
        /// <param name="applicationName">Name of the application</param>
        public EditApplicationWebServicesForm(MDSController controller, string applicationName)
        {
            InitializeComponent();
            _controller = controller;
            txtAppName.Text = applicationName;
            _webServicesList = new BindingList<WebServiceListItem>();
        }

        #endregion

        #region Events of Form and controls

        private void EditApplicationWebServicesForm_Load(object sender, EventArgs e)
        {
            PrepareForm();
            GetWebServiceList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var webServiceUrl = txtServiceUrl.Text.Trim();
            if (string.IsNullOrEmpty(webServiceUrl))
            {
                MDSGuiHelper.ShowWarningMessage("Web Service URL can not be empty!");
                return;
            }

            _webServicesList.Add(new WebServiceListItem { Url = webServiceUrl });
            gwWebServices.DataSource = _webServicesList;
        }

        private void removeWebServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if user selected any web service to remove
            if (gwWebServices.SelectedRows.Count <= 0)
            {
                return;
            }

            var selectedIndex = gwWebServices.SelectedRows[0].Index;
            if ((selectedIndex < 0) || (selectedIndex >= _webServicesList.Count))
            {
                return;
            }

            //Get selected web service item
            var selectedItem = _webServicesList[selectedIndex];

            //Confirm removing
            var dialogResult = MDSGuiHelper.ShowQuestionDialog(
                "Are you sure to remove web service: " + selectedItem.Url, "Comfirm removing?");
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            _webServicesList.RemoveAt(selectedIndex);
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SendChangesToServer();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Prepares form.
        /// </summary>
        private void PrepareForm()
        {
            Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
        }

        /// <summary>
        /// Gets web service list of application from web service.
        /// </summary>
        private void GetWebServiceList()
        {
            try
            {
                //Send a message to MDS server to get list of web services of application, get response and fill data grid.
                var responseMessage = _controller.SendMessageAndGetResponse(new GetApplicationWebServicesMessage {ApplicationName = txtAppName.Text});
                if (responseMessage.MessageTypeId != ControlMessageFactory.MessageTypeIdGetApplicationWebServicesResponseMessage)
                {
                    throw new MDSException("Response message to GetApplicationWebServicesMessage must be a GetApplicationWebServicesResponseMessage");
                }

                var webServicesResponseMessage = responseMessage as GetApplicationWebServicesResponseMessage;
                if (webServicesResponseMessage == null)
                {
                    throw new MDSException("Incorrect message type. MessageTypeId = " + responseMessage.MessageTypeId + ", but Type of object: " + responseMessage.GetType().Name);
                }

                //Check result
                if (!webServicesResponseMessage.Success)
                {
                    MDSGuiHelper.ShowWarningMessage(webServicesResponseMessage.ResultText);
                    return;
                }

                //Fill data grid
                FillWebServiceList(webServicesResponseMessage.WebServices);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Error occured while getting web service list from server. Error detail: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Fills data grid (gwWebServices) by list of web services of application.
        /// </summary>
        /// <param name="webServiceInfos">Web service list</param>
        private void FillWebServiceList(IEnumerable<ApplicationWebServiceInfo> webServiceInfos)
        {
            foreach (var webServiceInfo in webServiceInfos)
            {
                _webServicesList.Add(new WebServiceListItem {Url = webServiceInfo.Url});
            }

            gwWebServices.DataSource = _webServicesList;
        }

        /// <summary>
        /// Sends new web service list to server.
        /// </summary>
        private void SendChangesToServer()
        {
            try
            {
                var wsList = new ApplicationWebServiceInfo[_webServicesList.Count];
                for (var i = 0; i < _webServicesList.Count; i++)
                {
                    wsList[i] = new ApplicationWebServiceInfo {Url = _webServicesList[i].Url};
                }

                var responseMessage = _controller.SendMessageAndGetResponse(new UpdateApplicationWebServicesMessage {ApplicationName = txtAppName.Text, WebServices = wsList});
                if (responseMessage.MessageTypeId != ControlMessageFactory.MessageTypeIdOperationResultMessage)
                {
                    throw new MDSException("Response message to UpdateApplicationWebServicesMessage must be a OperationResultMessage");
                }

                var operationResultMessage = responseMessage as OperationResultMessage;
                if (operationResultMessage == null)
                {
                    throw new MDSException("Incorrect message type. MessageTypeId = " + responseMessage.MessageTypeId + ", but Type of object: " + responseMessage.GetType().Name);
                }

                //Check result
                if (!operationResultMessage.Success)
                {
                    MDSGuiHelper.ShowWarningMessage(operationResultMessage.ResultMessage);
                    return;
                }

                //Success
                MDSGuiHelper.ShowInfoDialog("Updated web services for application '" + txtAppName.Text + "'.", "Success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Error occured while sending web service list to server. Error detail: " + ex.Message);
            }
        }

        #endregion

        #region Sub classes

        /// <summary>
        /// Represents an item in web services list.
        /// </summary>
        private class WebServiceListItem
        {
            /// <summary>
            /// Url of web service.
            /// </summary>
            public string Url { get; set; }
        }

        #endregion
    }
}
