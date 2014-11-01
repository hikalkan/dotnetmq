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
using System.Windows.Forms;
using MDS.GUI;

namespace MDS.Management.GUI.ClientApplications
{
    public partial class AddNewApplicationForm : Form
    {
        public string ApplicationName { get; set; }

        public AddNewApplicationForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var appName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(appName))
            {
                MDSGuiHelper.ShowWarningMessage("Application name can not be empty!");
                return;
            }

            ApplicationName = appName;
            Close();
        }

        private void AddNewApplicationForm_Load(object sender, EventArgs e)
        {
            PrepareForm();
        }

        /// <summary>
        /// Prepares form.
        /// </summary>
        private void PrepareForm()
        {
            Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
        }
    }
}
