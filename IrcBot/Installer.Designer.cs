namespace Interrupt.IrcBot
{
    partial class Installer
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
            this.MyServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.MyServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // MyServiceInstaller
            // 
            this.MyServiceInstaller.Description = "This component is used to keep a log of a set channel on an IRC sever.";
            this.MyServiceInstaller.DisplayName = "IrcLoggerBot";
            this.MyServiceInstaller.ServiceName = "IrcLoggerBot";
            this.MyServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.MyServiceInstaller_AfterInstall);
            // 
            // MyServiceProcessInstaller
            // 
            this.MyServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.MyServiceProcessInstaller.Password = null;
            this.MyServiceProcessInstaller.Username = null;
            this.MyServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.MyServiceProcessInstaller_AfterInstall);
            // 
            // Installer
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MyServiceInstaller,
            this.MyServiceProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller MyServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller MyServiceProcessInstaller;
    }
}