using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace Interrupt.IrcBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            IrcBotService service = new IrcBotService();
            if (Debugger.IsAttached) service.OnStart();
            else if (args.Contains("/console")) service.OnStart();
            else ServiceBase.Run(service);
        }
    }
}
