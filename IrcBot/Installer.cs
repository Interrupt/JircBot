using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Interrupt.IrcBot
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        private void MyServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void MyServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
