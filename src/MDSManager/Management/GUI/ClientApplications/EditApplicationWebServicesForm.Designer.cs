namespace MDS.Management.GUI.ClientApplications
{
    partial class EditApplicationWebServicesForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gwWebServices = new System.Windows.Forms.DataGridView();
            this.cApplicationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Application = new System.Windows.Forms.GroupBox();
            this.lblApplicationListHeader = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtServiceUrl = new System.Windows.Forms.TextBox();
            this.WebServicesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeWebServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gwWebServices)).BeginInit();
            this.Application.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.WebServicesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Lavender;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.btnSaveChanges);
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.Application);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(633, 507);
            this.mainPanel.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gwWebServices);
            this.groupBox2.Location = new System.Drawing.Point(11, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(608, 308);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Web Services Of This Application";
            // 
            // gwWebServices
            // 
            this.gwWebServices.AllowUserToAddRows = false;
            this.gwWebServices.AllowUserToDeleteRows = false;
            this.gwWebServices.AllowUserToResizeRows = false;
            this.gwWebServices.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.gwWebServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gwWebServices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cApplicationName});
            this.gwWebServices.ContextMenuStrip = this.WebServicesMenu;
            this.gwWebServices.Location = new System.Drawing.Point(6, 22);
            this.gwWebServices.MultiSelect = false;
            this.gwWebServices.Name = "gwWebServices";
            this.gwWebServices.ReadOnly = true;
            this.gwWebServices.RowHeadersVisible = false;
            this.gwWebServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gwWebServices.Size = new System.Drawing.Size(596, 280);
            this.gwWebServices.TabIndex = 10;
            // 
            // cApplicationName
            // 
            this.cApplicationName.DataPropertyName = "Url";
            this.cApplicationName.HeaderText = "Web Service URL";
            this.cApplicationName.Name = "cApplicationName";
            this.cApplicationName.ReadOnly = true;
            this.cApplicationName.Width = 576;
            // 
            // Application
            // 
            this.Application.Controls.Add(this.lblApplicationListHeader);
            this.Application.Controls.Add(this.txtAppName);
            this.Application.Location = new System.Drawing.Point(11, 6);
            this.Application.Name = "Application";
            this.Application.Size = new System.Drawing.Size(608, 49);
            this.Application.TabIndex = 8;
            this.Application.TabStop = false;
            this.Application.Text = "Application";
            // 
            // lblApplicationListHeader
            // 
            this.lblApplicationListHeader.BackColor = System.Drawing.Color.LightSlateGray;
            this.lblApplicationListHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblApplicationListHeader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblApplicationListHeader.ForeColor = System.Drawing.Color.White;
            this.lblApplicationListHeader.Location = new System.Drawing.Point(6, 19);
            this.lblApplicationListHeader.Name = "lblApplicationListHeader";
            this.lblApplicationListHeader.Size = new System.Drawing.Size(120, 23);
            this.lblApplicationListHeader.TabIndex = 5;
            this.lblApplicationListHeader.Text = "Name:";
            this.lblApplicationListHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAppName
            // 
            this.txtAppName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtAppName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAppName.Location = new System.Drawing.Point(127, 19);
            this.txtAppName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAppName.MaxLength = 50;
            this.txtAppName.Name = "txtAppName";
            this.txtAppName.ReadOnly = true;
            this.txtAppName.Size = new System.Drawing.Size(242, 23);
            this.txtAppName.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.txtServiceUrl);
            this.groupBox1.Location = new System.Drawing.Point(11, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(608, 87);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New Web Service";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSlateGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "URL:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAdd.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(447, 53);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(155, 28);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add Web Service";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtServiceUrl
            // 
            this.txtServiceUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServiceUrl.Location = new System.Drawing.Point(127, 23);
            this.txtServiceUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtServiceUrl.MaxLength = 50;
            this.txtServiceUrl.Name = "txtServiceUrl";
            this.txtServiceUrl.Size = new System.Drawing.Size(475, 23);
            this.txtServiceUrl.TabIndex = 6;
            // 
            // WebServicesMenu
            // 
            this.WebServicesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeWebServiceToolStripMenuItem});
            this.WebServicesMenu.Name = "WebServicesMenu";
            this.WebServicesMenu.Size = new System.Drawing.Size(185, 26);
            // 
            // removeWebServiceToolStripMenuItem
            // 
            this.removeWebServiceToolStripMenuItem.Name = "removeWebServiceToolStripMenuItem";
            this.removeWebServiceToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.removeWebServiceToolStripMenuItem.Text = "Remove Web Service";
            this.removeWebServiceToolStripMenuItem.Click += new System.EventHandler(this.removeWebServiceToolStripMenuItem_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSaveChanges.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSaveChanges.ForeColor = System.Drawing.Color.White;
            this.btnSaveChanges.Location = new System.Drawing.Point(458, 468);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(155, 28);
            this.btnSaveChanges.TabIndex = 10;
            this.btnSaveChanges.Text = "Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // EditApplicationWebServicesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(633, 507);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Name = "EditApplicationWebServicesForm";
            this.Text = "Edit Web Services Of Application";
            this.Load += new System.EventHandler(this.EditApplicationWebServicesForm_Load);
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gwWebServices)).EndInit();
            this.Application.ResumeLayout(false);
            this.Application.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.WebServicesMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox Application;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceUrl;
        private System.Windows.Forms.Label lblApplicationListHeader;
        private System.Windows.Forms.TextBox txtAppName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView gwWebServices;
        private System.Windows.Forms.ContextMenuStrip WebServicesMenu;
        private System.Windows.Forms.ToolStripMenuItem removeWebServiceToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn cApplicationName;
        private System.Windows.Forms.Button btnSaveChanges;
    }
}