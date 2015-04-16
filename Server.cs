using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ini;

namespace Better_CSharp_IRC_Bot
{
    class Server
    {
        protected string nick, server, realName, owner;
        protected int port;
        List<string> chans = new List<string>();

        /// <summary>
        /// Start a new connection to a server.
        /// </summary>
        /// <param name="nick">Nickname to use on the server.</param>
        /// <param name="server">Address to access the server by. Can be link or IP address.</param>
        /// <param name="port">Port to connect to. Usually is 6667.</param>
        public Server(string nick, string server, int port, string owner)
        {
            this.nick = nick;
            this.server = server;
            this.port = port;
            realName = nick; //Set this as a default value.
            this.owner = owner;
            startConnection();
        }

        /// <summary>
        /// Start a new connection to a server, including a realname.
        /// </summary>
        /// <param name="nick">Nickname to use on the server.</param>
        /// <param name="server">Address to access the server by. Can be link or IP address.</param>
        /// <param name="realName">Define the realname to connect with.</param>
        /// <param name="port">Port to connect to. Usually is 6667.</param>
        public Server(string nick, string server, string realName, int port, string owner)
        {
            this.nick = nick;
            this.server = server;
            this.port = port;
            this.realName = realName;
            this.owner = owner;
            startConnection();
        }

        protected void startConnection()
        {
            int i = 0;
            IniFile chanFile = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\" + server + "\\chans.ini");
            while (chanFile.IniReadValue("Channels", "Channel" + i.ToString()) != "")
            {
                chans.Add(chanFile.IniReadValue("Channels", "Channel" + i.ToString()));
                i++;
            }
            ThreadStart job = new ThreadStart(() => connectToServer(nick, server, realName, port)); //use a lambda expression so it stops complaining.
            Thread thread = new Thread(job);
            //thread.IsBackground = true;
            thread.Start();
        }

        protected void connectToServer(string nick, string server, string realName, int port)
        {
            System.Net.Sockets.TcpClient sock = new System.Net.Sockets.TcpClient();
            System.IO.TextReader input;
            System.IO.TextWriter output;
            Chan c;

            sock.Connect(server, port);
            if (!sock.Connected)
            {
                Console.WriteLine("Failed to connect: " + server + ":" + port.ToString() + ". Abandoning connection to server!");
                return;
            }
            else Console.WriteLine("Successfully connected to server " + server + ":" + port.ToString() + ".");

            input = new System.IO.StreamReader(sock.GetStream());
            output = new System.IO.StreamWriter(sock.GetStream());
            output.Write("NOTICE " + owner + " :" + nick + " is now LOGGED ON." + "\r\n");
            //Establish connection using nick.
            output.Write("USER " + nick + " * 0 :" + realName + "\r\n" + "NICK " + nick + "\r\n");
            output.Flush();

            foreach (string s in chans)
            {
                c = new Chan(s, input, output, nick);
            }
        }
    }
}
