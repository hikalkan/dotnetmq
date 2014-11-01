namespace DotNetMQ
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DotNetMqProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DotNetMqServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // DotNetMqProcessInstaller
            // 
            this.DotNetMqProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DotNetMqProcessInstaller.Password = null;
            this.DotNetMqProcessInstaller.Username = null;
            // 
            // DotNetMqServiceInstaller
            // 
            this.DotNetMqServiceInstaller.Description = "A Message Broker for integration of applications.";
            this.DotNetMqServiceInstaller.DisplayName = "DotNetMQ";
            this.DotNetMqServiceInstaller.ServiceName = "DotNetMqService";
            this.DotNetMqServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DotNetMqProcessInstaller,
            this.DotNetMqServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DotNetMqProcessInstaller;
        private System.ServiceProcess.ServiceInstaller DotNetMqServiceInstaller;
    }
}