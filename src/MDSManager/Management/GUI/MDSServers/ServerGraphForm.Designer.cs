using MDS.Management.GUI.Tools;

namespace MDS.Management.GUI.MDSServers
{
    partial class ServerGraphForm
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
            this.pnlDesign = new MDS.Management.GUI.Tools.DoubleBufferedPanel();
            this.cmsDesignAreaRightMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveGraph = new System.Windows.Forms.Button();
            this.cmsServerRightMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAsThisServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel.SuspendLayout();
            this.cmsDesignAreaRightMenu.SuspendLayout();
            this.cmsServerRightMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Lavender;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.pnlDesign);
            this.mainPanel.Controls.Add(this.btnSaveGraph);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(894, 572);
            this.mainPanel.TabIndex = 2;
            // 
            // pnlDesign
            // 
            this.pnlDesign.AutoScroll = true;
            this.pnlDesign.AutoScrollMargin = new System.Drawing.Size(5, 5);
            this.pnlDesign.BackColor = System.Drawing.Color.AliceBlue;
            this.pnlDesign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDesign.ContextMenuStrip = this.cmsDesignAreaRightMenu;
            this.pnlDesign.Location = new System.Drawing.Point(3, 3);
            this.pnlDesign.Name = "pnlDesign";
            this.pnlDesign.Size = new System.Drawing.Size(886, 528);
            this.pnlDesign.TabIndex = 4;
            this.pnlDesign.Paint += new System.Windows.Forms.PaintEventHandler(this.DesignPanel_Paint);
            this.pnlDesign.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DesignPanel_MouseDown);
            // 
            // cmsDesignAreaRightMenu
            // 
            this.cmsDesignAreaRightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewServerToolStripMenuItem});
            this.cmsDesignAreaRightMenu.Name = "cmsDesignAreaRightMenu";
            this.cmsDesignAreaRightMenu.Size = new System.Drawing.Size(159, 26);
            // 
            // addNewServerToolStripMenuItem
            // 
            this.addNewServerToolStripMenuItem.Name = "addNewServerToolStripMenuItem";
            this.addNewServerToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.addNewServerToolStripMenuItem.Text = "Add New Server";
            this.addNewServerToolStripMenuItem.Click += new System.EventHandler(this.DesignArea_AddNewServer_Click);
            // 
            // btnSaveGraph
            // 
            this.btnSaveGraph.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSaveGraph.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSaveGraph.ForeColor = System.Drawing.Color.White;
            this.btnSaveGraph.Location = new System.Drawing.Point(689, 536);
            this.btnSaveGraph.Name = "btnSaveGraph";
            this.btnSaveGraph.Size = new System.Drawing.Size(200, 28);
            this.btnSaveGraph.TabIndex = 3;
            this.btnSaveGraph.Text = "Save && Update Graph";
            this.btnSaveGraph.UseVisualStyleBackColor = false;
            this.btnSaveGraph.Click += new System.EventHandler(this.btnSaveGraph_Click);
            // 
            // cmsServerRightMenu
            // 
            this.cmsServerRightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeServerToolStripMenuItem,
            this.setAsThisServerToolStripMenuItem,
            this.serverPropertiesToolStripMenuItem});
            this.cmsServerRightMenu.Name = "cmsServerRightMenu";
            this.cmsServerRightMenu.Size = new System.Drawing.Size(175, 70);
            // 
            // removeServerToolStripMenuItem
            // 
            this.removeServerToolStripMenuItem.Name = "removeServerToolStripMenuItem";
            this.removeServerToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.removeServerToolStripMenuItem.Text = "Remove Server";
            this.removeServerToolStripMenuItem.Click += new System.EventHandler(this.ServerRightMenu_RemoveServer_Click);
            // 
            // setAsThisServerToolStripMenuItem
            // 
            this.setAsThisServerToolStripMenuItem.Name = "setAsThisServerToolStripMenuItem";
            this.setAsThisServerToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.setAsThisServerToolStripMenuItem.Text = "Set As This Server";
            this.setAsThisServerToolStripMenuItem.Click += new System.EventHandler(this.ServerRightMenu_SetAsThisServer_Click);
            // 
            // serverPropertiesToolStripMenuItem
            // 
            this.serverPropertiesToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.serverPropertiesToolStripMenuItem.Name = "serverPropertiesToolStripMenuItem";
            this.serverPropertiesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.serverPropertiesToolStripMenuItem.Text = "Server Properties";
            this.serverPropertiesToolStripMenuItem.Click += new System.EventHandler(this.ServerRightMenu_ServerProperties_Click);
            // 
            // ServerGraphForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(894, 572);
            this.Controls.Add(this.mainPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ServerGraphForm";
            this.Text = "DotNetMQ Server Graph";
            this.Load += new System.EventHandler(this.ServerGraphForm_Load);
            this.Resize += new System.EventHandler(this.ServerGraphForm_Resize);
            this.mainPanel.ResumeLayout(false);
            this.cmsDesignAreaRightMenu.ResumeLayout(false);
            this.cmsServerRightMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button btnSaveGraph;
        private DoubleBufferedPanel pnlDesign;
        private System.Windows.Forms.ContextMenuStrip cmsServerRightMenu;
        private System.Windows.Forms.ToolStripMenuItem removeServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverPropertiesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsDesignAreaRightMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAsThisServerToolStripMenuItem;
    }
}