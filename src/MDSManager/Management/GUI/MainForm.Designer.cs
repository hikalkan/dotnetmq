namespace MDS.Management.GUI
{
    partial class MainForm
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
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.mdsServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientApplicationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mdsServersToolStripMenuItem,
            this.clientApplicationsToolStripMenuItem});
            this.mainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(813, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "Main Menu";
            // 
            // mdsServersToolStripMenuItem
            // 
            this.mdsServersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverGraphToolStripMenuItem,
            this.routesToolStripMenuItem});
            this.mdsServersToolStripMenuItem.Name = "mdsServersToolStripMenuItem";
            this.mdsServersToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.mdsServersToolStripMenuItem.Text = "MDS Servers";
            // 
            // serverGraphToolStripMenuItem
            // 
            this.serverGraphToolStripMenuItem.Name = "serverGraphToolStripMenuItem";
            this.serverGraphToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.serverGraphToolStripMenuItem.Text = "Server Graph";
            this.serverGraphToolStripMenuItem.Click += new System.EventHandler(this.ServerGraphToolStripMenuItem_Click);
            // 
            // routesToolStripMenuItem
            // 
            this.routesToolStripMenuItem.Name = "routesToolStripMenuItem";
            this.routesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.routesToolStripMenuItem.Text = "Routes";
            this.routesToolStripMenuItem.Visible = false;
            this.routesToolStripMenuItem.Click += new System.EventHandler(this.RoutesToolStripMenuItem_Click);
            // 
            // clientApplicationsToolStripMenuItem
            // 
            this.clientApplicationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationListToolStripMenuItem});
            this.clientApplicationsToolStripMenuItem.Name = "clientApplicationsToolStripMenuItem";
            this.clientApplicationsToolStripMenuItem.Size = new System.Drawing.Size(117, 20);
            this.clientApplicationsToolStripMenuItem.Text = "Client Applications";
            // 
            // applicationListToolStripMenuItem
            // 
            this.applicationListToolStripMenuItem.Name = "applicationListToolStripMenuItem";
            this.applicationListToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.applicationListToolStripMenuItem.Text = "Application List";
            this.applicationListToolStripMenuItem.Click += new System.EventHandler(this.ApplicationListToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(813, 464);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "MDS Management GUI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem mdsServersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clientApplicationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applicationListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routesToolStripMenuItem;
    }
}

