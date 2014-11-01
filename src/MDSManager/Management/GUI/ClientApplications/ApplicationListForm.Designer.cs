namespace MDS.Management.GUI.ClientApplications
{
    partial class ApplicationListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnRemoveApplication = new System.Windows.Forms.Button();
            this.btnAddApplication = new System.Windows.Forms.Button();
            this.lblApplicationListHeader = new System.Windows.Forms.Label();
            this.gwApplicationList = new System.Windows.Forms.DataGridView();
            this.cApplicationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cConnectedClients = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ApplicationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editWebServicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gwApplicationList)).BeginInit();
            this.ApplicationMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Lavender;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.btnRefreshList);
            this.mainPanel.Controls.Add(this.btnRemoveApplication);
            this.mainPanel.Controls.Add(this.btnAddApplication);
            this.mainPanel.Controls.Add(this.lblApplicationListHeader);
            this.mainPanel.Controls.Add(this.gwApplicationList);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(557, 362);
            this.mainPanel.TabIndex = 0;
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnRefreshList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRefreshList.ForeColor = System.Drawing.Color.White;
            this.btnRefreshList.Location = new System.Drawing.Point(3, 329);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(137, 28);
            this.btnRefreshList.TabIndex = 4;
            this.btnRefreshList.Text = "Refresh List";
            this.btnRefreshList.UseVisualStyleBackColor = false;
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // btnRemoveApplication
            // 
            this.btnRemoveApplication.BackColor = System.Drawing.Color.Crimson;
            this.btnRemoveApplication.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRemoveApplication.ForeColor = System.Drawing.Color.White;
            this.btnRemoveApplication.Location = new System.Drawing.Point(146, 329);
            this.btnRemoveApplication.Name = "btnRemoveApplication";
            this.btnRemoveApplication.Size = new System.Drawing.Size(200, 28);
            this.btnRemoveApplication.TabIndex = 3;
            this.btnRemoveApplication.Text = "Remove Application";
            this.btnRemoveApplication.UseVisualStyleBackColor = false;
            this.btnRemoveApplication.Click += new System.EventHandler(this.btnRemoveApplication_Click);
            // 
            // btnAddApplication
            // 
            this.btnAddApplication.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAddApplication.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnAddApplication.ForeColor = System.Drawing.Color.White;
            this.btnAddApplication.Location = new System.Drawing.Point(352, 329);
            this.btnAddApplication.Name = "btnAddApplication";
            this.btnAddApplication.Size = new System.Drawing.Size(200, 28);
            this.btnAddApplication.TabIndex = 2;
            this.btnAddApplication.Text = "Add new Application";
            this.btnAddApplication.UseVisualStyleBackColor = false;
            this.btnAddApplication.Click += new System.EventHandler(this.btnAddApplication_Click);
            // 
            // lblApplicationListHeader
            // 
            this.lblApplicationListHeader.BackColor = System.Drawing.Color.DodgerBlue;
            this.lblApplicationListHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblApplicationListHeader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblApplicationListHeader.ForeColor = System.Drawing.Color.White;
            this.lblApplicationListHeader.Location = new System.Drawing.Point(3, 3);
            this.lblApplicationListHeader.Name = "lblApplicationListHeader";
            this.lblApplicationListHeader.Size = new System.Drawing.Size(550, 23);
            this.lblApplicationListHeader.TabIndex = 1;
            this.lblApplicationListHeader.Text = "All Applications";
            this.lblApplicationListHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gwApplicationList
            // 
            this.gwApplicationList.AllowUserToAddRows = false;
            this.gwApplicationList.AllowUserToDeleteRows = false;
            this.gwApplicationList.AllowUserToResizeRows = false;
            this.gwApplicationList.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.gwApplicationList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gwApplicationList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cApplicationName,
            this.cConnectedClients});
            this.gwApplicationList.ContextMenuStrip = this.ApplicationMenu;
            this.gwApplicationList.Location = new System.Drawing.Point(3, 25);
            this.gwApplicationList.MultiSelect = false;
            this.gwApplicationList.Name = "gwApplicationList";
            this.gwApplicationList.ReadOnly = true;
            this.gwApplicationList.RowHeadersVisible = false;
            this.gwApplicationList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gwApplicationList.Size = new System.Drawing.Size(550, 300);
            this.gwApplicationList.TabIndex = 0;
            // 
            // cApplicationName
            // 
            this.cApplicationName.DataPropertyName = "ApplicationName";
            this.cApplicationName.HeaderText = "Application Name";
            this.cApplicationName.Name = "cApplicationName";
            this.cApplicationName.ReadOnly = true;
            this.cApplicationName.Width = 350;
            // 
            // cConnectedClients
            // 
            this.cConnectedClients.DataPropertyName = "ConnectedClients";
            this.cConnectedClients.HeaderText = "Connected Clients";
            this.cConnectedClients.Name = "cConnectedClients";
            this.cConnectedClients.ReadOnly = true;
            this.cConnectedClients.Width = 180;
            // 
            // ApplicationMenu
            // 
            this.ApplicationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editWebServicesToolStripMenuItem});
            this.ApplicationMenu.Name = "ApplicationMenu";
            this.ApplicationMenu.Size = new System.Drawing.Size(172, 26);
            // 
            // editWebServicesToolStripMenuItem
            // 
            this.editWebServicesToolStripMenuItem.Name = "editWebServicesToolStripMenuItem";
            this.editWebServicesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.editWebServicesToolStripMenuItem.Text = "Edit Web Services";
            this.editWebServicesToolStripMenuItem.Click += new System.EventHandler(this.editWebServicesToolStripMenuItem_Click);
            // 
            // ApplicationListForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(557, 362);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ApplicationListForm";
            this.Text = "Client Application List";
            this.Load += new System.EventHandler(this.ApplicationList_Load);
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gwApplicationList)).EndInit();
            this.ApplicationMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.DataGridView gwApplicationList;
        private System.Windows.Forms.Label lblApplicationListHeader;
        private System.Windows.Forms.Button btnAddApplication;
        private System.Windows.Forms.DataGridViewTextBoxColumn cApplicationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cConnectedClients;
        private System.Windows.Forms.Button btnRemoveApplication;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.ContextMenuStrip ApplicationMenu;
        private System.Windows.Forms.ToolStripMenuItem editWebServicesToolStripMenuItem;

    }
}