JircBot is a simple IRC channel logger bot, built as a windows service and written in C#.

JircBot can be run from the command line by running the built binary with the '/console' flag, or it can be installed installutil to run as a Windows service.

Connection and output settings can be changed in the Properties/Settings file in the solution. The settings are as follows:

server		IRC server to connect to, using port 6667.
user		Name the bot will give when logging in.
nick		Nickname of the bot on the irc server.
channel		Channel to join and start logging.
password	Password to use when logging into the server.
logDirectory	Directory log files will be written to.