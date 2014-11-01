namespace MDS.Tools.ProxyGenerator
{
    partial class ProxyGeneratorForm
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTargetFolderBrowse = new System.Windows.Forms.Button();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbClasses = new System.Windows.Forms.ComboBox();
            this.btnBrowseAssembly = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAssemblyPath = new System.Windows.Forms.TextBox();
            this.lblApplicationListHeader = new System.Windows.Forms.Label();
            this.AssemblyBrowseDialog = new System.Windows.Forms.OpenFileDialog();
            this.TargetFolderBrowseDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Lavender;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.groupBox2);
            this.mainPanel.Controls.Add(this.btnGenerateCode);
            this.mainPanel.Controls.Add(this.groupBox1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(844, 212);
            this.mainPanel.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnTargetFolderBrowse);
            this.groupBox2.Controls.Add(this.txtTargetFolder);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtNamespace);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(8, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(823, 78);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy Class Generation";
            // 
            // btnTargetFolderBrowse
            // 
            this.btnTargetFolderBrowse.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnTargetFolderBrowse.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnTargetFolderBrowse.ForeColor = System.Drawing.Color.White;
            this.btnTargetFolderBrowse.Location = new System.Drawing.Point(668, 45);
            this.btnTargetFolderBrowse.Name = "btnTargetFolderBrowse";
            this.btnTargetFolderBrowse.Size = new System.Drawing.Size(149, 28);
            this.btnTargetFolderBrowse.TabIndex = 17;
            this.btnTargetFolderBrowse.Text = "Browse...";
            this.btnTargetFolderBrowse.UseVisualStyleBackColor = false;
            this.btnTargetFolderBrowse.Click += new System.EventHandler(this.btnTargetFolderBrowse_Click);
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTargetFolder.Location = new System.Drawing.Point(158, 48);
            this.txtTargetFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTargetFolder.MaxLength = 50;
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(504, 23);
            this.txtTargetFolder.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LightSlateGray;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 23);
            this.label3.TabIndex = 15;
            this.label3.Text = "Target Folder:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNamespace
            // 
            this.txtNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNamespace.Location = new System.Drawing.Point(158, 23);
            this.txtNamespace.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNamespace.MaxLength = 50;
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(504, 23);
            this.txtNamespace.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSlateGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 23);
            this.label1.TabIndex = 13;
            this.label1.Text = "Namespace:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGenerateCode.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnGenerateCode.ForeColor = System.Drawing.Color.White;
            this.btnGenerateCode.Location = new System.Drawing.Point(461, 174);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(209, 28);
            this.btnGenerateCode.TabIndex = 4;
            this.btnGenerateCode.Text = "Generate Code";
            this.btnGenerateCode.UseVisualStyleBackColor = false;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbClasses);
            this.groupBox1.Controls.Add(this.btnBrowseAssembly);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtAssemblyPath);
            this.groupBox1.Controls.Add(this.lblApplicationListHeader);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(823, 78);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Class Selection";
            // 
            // cmbClasses
            // 
            this.cmbClasses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClasses.FormattingEnabled = true;
            this.cmbClasses.Location = new System.Drawing.Point(158, 47);
            this.cmbClasses.Name = "cmbClasses";
            this.cmbClasses.Size = new System.Drawing.Size(504, 24);
            this.cmbClasses.TabIndex = 12;
            // 
            // btnBrowseAssembly
            // 
            this.btnBrowseAssembly.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnBrowseAssembly.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnBrowseAssembly.ForeColor = System.Drawing.Color.White;
            this.btnBrowseAssembly.Location = new System.Drawing.Point(668, 19);
            this.btnBrowseAssembly.Name = "btnBrowseAssembly";
            this.btnBrowseAssembly.Size = new System.Drawing.Size(149, 28);
            this.btnBrowseAssembly.TabIndex = 7;
            this.btnBrowseAssembly.Text = "Browse...";
            this.btnBrowseAssembly.UseVisualStyleBackColor = false;
            this.btnBrowseAssembly.Click += new System.EventHandler(this.btnBrowseAssembly_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LightSlateGray;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 23);
            this.label2.TabIndex = 11;
            this.label2.Text = "Assembly:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAssemblyPath
            // 
            this.txtAssemblyPath.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtAssemblyPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssemblyPath.Location = new System.Drawing.Point(158, 22);
            this.txtAssemblyPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAssemblyPath.MaxLength = 50;
            this.txtAssemblyPath.Name = "txtAssemblyPath";
            this.txtAssemblyPath.ReadOnly = true;
            this.txtAssemblyPath.Size = new System.Drawing.Size(504, 23);
            this.txtAssemblyPath.TabIndex = 1;
            // 
            // lblApplicationListHeader
            // 
            this.lblApplicationListHeader.BackColor = System.Drawing.Color.LightSlateGray;
            this.lblApplicationListHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblApplicationListHeader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblApplicationListHeader.ForeColor = System.Drawing.Color.White;
            this.lblApplicationListHeader.Location = new System.Drawing.Point(6, 47);
            this.lblApplicationListHeader.Name = "lblApplicationListHeader";
            this.lblApplicationListHeader.Size = new System.Drawing.Size(150, 24);
            this.lblApplicationListHeader.TabIndex = 2;
            this.lblApplicationListHeader.Text = "Class:";
            this.lblApplicationListHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AssemblyBrowseDialog
            // 
            this.AssemblyBrowseDialog.FileName = "openFileDialog1";
            this.AssemblyBrowseDialog.Filter = "Executable files|*.exe|Dll files|*.dll|All files|*.*";
            // 
            // ProxyGeneratorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(844, 212);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProxyGeneratorForm";
            this.Text = "MDS Service Proxy Generator";
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAssemblyPath;
        private System.Windows.Forms.Label lblApplicationListHeader;
        private System.Windows.Forms.Button btnBrowseAssembly;
        private System.Windows.Forms.ComboBox cmbClasses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTargetFolderBrowse;
        private System.Windows.Forms.OpenFileDialog AssemblyBrowseDialog;
        private System.Windows.Forms.FolderBrowserDialog TargetFolderBrowseDialog;
    }
}