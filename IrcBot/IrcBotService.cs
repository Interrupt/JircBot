using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Interrupt.IrcBot.Properties;
using System.Threading;

namespace Interrupt.IrcBot
{
    public delegate void CommandReceived( string command );

    public partial class IrcBotService : WorkerServiceBase
    {
        public event CommandReceived eventReceivedCommand;

        private TcpClient ircConnection;
        private NetworkStream ircStream;
        private StreamReader ircReader;
        private StreamWriter ircWriter;

        private bool doDisconnect = false;
        private bool logging = true;

        public IrcBotService()
        {
            InitializeComponent();
        }

        public void OnStart()
        {
            OnStart(null);
        }

        /// <summary>
        /// Occurs when the service is started
        /// </summary>
        protected override void OnStart(string[] args)
        {
            try
            {
                // hookup the command event
                eventReceivedCommand += new CommandReceived(IrcBotService_eventReceivedCommand);

                connect();

                CreateThreads(1, workerFunc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when starting: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when the service is stopped
        /// </summary>
        protected override void OnStop()
        {
            // close the connection when stopped
            if(ircConnection.Connected)
                ircConnection.Close();
        }

        /// <summary>
        /// Main thread of the service
        /// </summary>
        protected void workerFunc()
        {
            try
            {
                // try to reconnect if disconnected
                if (!ircConnection.Connected) connect();

                string incoming = ircReader.ReadLine();
                if (!String.IsNullOrEmpty(incoming))
                {
                    if (eventReceivedCommand != null) this.eventReceivedCommand(incoming);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                System.Threading.Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Connects to the IRC server, and joins the designated channel
        /// </summary>
        protected void connect()
        {
            Console.WriteLine("Connecting...");

            // connect to the server, set the streams
            ircConnection = new TcpClient(Settings.Default.server, 6667);
            ircStream = ircConnection.GetStream();
            ircReader = new StreamReader(ircStream);
            ircWriter = new StreamWriter(ircStream);

            // authenticate with the server
            IrcWrite(String.Format("USER {0} {1} * :{2}", Settings.Default.user, false, "Interrupt.IrcBot"));
            IrcWrite(String.Format("PASS {0}", Settings.Default.password));
            IrcWrite(String.Format("NICK {0}", Settings.Default.nick));
            IrcWrite(String.Format("JOIN {0}", Settings.Default.channel));

            // announce to the log that logging has begun
            WriteToHtmlLog(String.Format("------- Started Logging at {0} ------", DateTime.Now.ToShortTimeString()));
        }

        /// <summary>
        /// Writes raw text to the IRC socket
        /// </summary>
        protected void IrcWrite(string writeString)
        {
            ircWriter.WriteLine(writeString);
            ircWriter.Flush();
        }

        /// <summary>
        /// Occurs when a new event is received
        /// </summary>
        void IrcBotService_eventReceivedCommand(string command)
        {
            Console.WriteLine(command);
            string[] line = command.Split(' ');

            // keep connected to the server, respond to pings
            if (line[0] == "PING")
            {
                IrcWrite("PONG");
            }

            string hReadable = getReadableMessage(line);
            if (logging)
            {
                string logText = filter(line[1], line[2], line[0], getReadableMessage(line));
                if (logText != null) WriteToHtmlLog(logText);
            }

            if (hReadable.StartsWith("!")) doCommand(hReadable);
        }

        /// <summary>
        /// Filters out only PRIVMSG events
        /// </summary>
        protected string filter(string type, string channel, string nick, string message)
        {
            if (type != "PRIVMSG") return null;

            string filteredNick = nick.TrimStart(':');
            string[] fNick = filteredNick.Split('!');
            if (fNick.Count() > 0) filteredNick = fNick[0];

            string filteredMessage = message.TrimStart(':');

            return String.Format("<span class='date'>{0}</span> <span class='dec'>&lt;</span> <span class='name'>{1}</span> <span class='dec'>&gt;</span> <span class='msg'>{2}</span>", DateTime.Now.ToShortTimeString(), filteredNick, message);
        }

        /// <summary>
        /// Returns a human readable representation of a PRIVMSG
        /// </summary>
        protected static string getReadableMessage(string[] message)
        {
            string final = "";
            for (int i = 3; i < message.Count(); i++)
            {
                if (i == 3) message[i] = message[i].TrimStart(':');
                final += message[i] + " ";
            }
            return final;
        }

        /// <summary>
        /// Executes commands destined for the bot
        /// </summary>
        /// <param name="cmd">The command string to parse</param>
        protected void doCommand(string cmd)
        {
            string[] args = cmd.Split(' ');
            if (args[0] == "!say")
            {
                string message = "";
                for (int i = 1; i < args.Count(); i++)
                {
                    message += args[i] + " ";
                }

                Say(message);
            }
        }

        /// <summary>
        /// Sends a PRIVMSG command to the channel
        /// </summary>
        /// <param name="message">The message the bot will say.</param>
        protected void Say(string message)
        {
            IrcWrite(String.Format("PRIVMSG {0} :{1}", Settings.Default.channel, message));
        }

        protected void WriteToHtmlLog(string htmlLine)
        {
            try
            {
                string fileName = Settings.Default.logDirectory + "\\" + DateTime.Now.ToString("MM-dd-yy") + "-log.html";

                // check if the stylesheet include needs to be added to a new file
                bool addStylesheet = !File.Exists(fileName);

                // create the writer, write to the file
                TextWriter writer = new StreamWriter(fileName, true);

                // write the stylesheet to the beginning, if needed
                if (addStylesheet) writer.WriteLine("<head><link rel='stylesheet' type='text/css' href='styles.css' /></head>");

                // write the line
                writer.WriteLine("<div class='line'>" + htmlLine + "</div>");
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    } 
}
