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
using System.Text;
using System.Windows.Forms;
using MDS.Communication.Messages.ControllerMessages;
using MDS.GUI;

namespace MDS.Management.GUI.MDSServers
{
    /// <summary>
    /// This form is used to view/change properties of a server in graph.
    /// </summary>
    public partial class EditServerForm : Form
    {
        /// <summary>
        /// True, if user accepted changes by pressing Update button.
        /// </summary>
        public bool Updated { get; private set; }

        /// <summary>
        /// Server to edit.
        /// </summary>
        private readonly ServerGraphInfo.ServerOnGraph _server;

        /// <summary>
        /// True, if adding new server, else updating exists server.
        /// </summary>
        private readonly bool _addNew;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Server to edit</param>
        /// <param name="addNew"></param>
        public EditServerForm(ServerGraphInfo.ServerOnGraph server, bool addNew)
        {
            InitializeComponent();
            _server = server;
            _addNew = addNew;
            txtName.Text = _server.Name;
            txtIPAddress.Text = _server.IpAddress;
            txtPort.Text = _server.Port.ToString();
        }

        private void EditServerForm_Load(object sender, EventArgs e)
        {
            PrepareForm();
        }

        /// <summary>
        /// Prepares form. This method is called while form is opening.
        /// </summary>
        private void PrepareForm()
        {
            Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
            if (_addNew)
            {
                Text = "Add New Server";
                btnUpdate.Text = "Add Server";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var name = txtName.Text;
            var ipAddress = txtIPAddress.Text;
            var port = txtPort.Text;

            if (string.IsNullOrEmpty(name))
            {
                MDSGuiHelper.ShowWarningMessage("Server name can not be empty.", "Server name is invalid!");
                return;
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                MDSGuiHelper.ShowWarningMessage("IP address can not be empty.", "IP address is invalid!");
                return;
            }

            if (string.IsNullOrEmpty(port))
            {
                MDSGuiHelper.ShowWarningMessage("TCP Port can not be empty.", "TCP Port is invalid!");
                return;
            }

            int portNo;
            try
            {
                portNo = Convert.ToInt32(port);
            }
            catch
            {
                MDSGuiHelper.ShowWarningMessage("TCP Port must be numeric.", "TCP Port is invalid!");
                return;
            }

            if(portNo <= 0)
            {
                MDSGuiHelper.ShowWarningMessage("TCP Port must be a positive number.", "TCP Port is invalid!");
                return;
            }

            _server.Name = name;
            _server.IpAddress = ipAddress;
            _server.Port = portNo;

            Updated = true;

            Close();
        }
    }
}
