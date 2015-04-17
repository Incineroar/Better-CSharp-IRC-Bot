using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Ini;

namespace Better_CSharp_IRC_Bot
{
    class Server
    {
        protected string nick, server, realName, owner;
        protected int port;
        List<string> chans = new List<string>();
        List<Chan> chanList = new List<Chan>();
        System.IO.TextReader input;
        System.IO.TextWriter output;
        bool connections = false;
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
            thread.Start();
        }

        protected void connectToServer(string nick, string server, string realName, int port)
        {
            System.Net.Sockets.TcpClient sock = new System.Net.Sockets.TcpClient();

            sock.Connect(server, port);
            if (!sock.Connected)
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Failed to connect: " + server + ":" + port.ToString() + ". Abandoning connection to server!");
                return;
            }
            else Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Successfully connected to server " + server + ":" + port.ToString() + ".");

            input = new System.IO.StreamReader(sock.GetStream());
            output = new System.IO.StreamWriter(sock.GetStream());
            sendText("NOTICE " + owner + " " + nick + " is now LOGGED ON." + "\r\n");
            //Establish connection using nick.
            sendText("USER " + nick + " * 0 :" + realName + "\r\n" + "NICK " + nick + "\r\n");
            
            string buf;
            try
            {
                for (buf = input.ReadLine(); ; buf = input.ReadLine())
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + server + "\\LOG.txt"))
                    {
                        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + server + "\\logs\\"))
                        {
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + server + "\\logs\\");
                        }
                        File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\" + server + "\\logs\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", "[CHANNEL CLASS] [" + System.DateTime.Now.ToShortTimeString() + "] " + buf + Environment.NewLine);
                        
                    }
                    if (!buf.Equals(""))
                    {
                        if (buf.Split(' ')[1] == "001" && !connections)
                        {
                            connections = true;
                            foreach (string s in chans)
                            {
                                chanList.Add(new Chan(s, input, output, nick, server));
                            }
                        }
                    }
                    if (buf.Contains("PING")) { sendText(buf.Replace("PING", "PONG") + "\r\n"); Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + 
                        "Replied to a PING request (Server class)."); }
                }
            }
            catch (Exception ex)
                {
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "ERROR: " + ex.ToString());
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Abandoning connection!");
                }
        }

        /// <summary>
        /// Sends a string of text to the IRC server.
        /// </summary>
        /// <param name="s">The text to write to the server. DOES NOT include return characters</param>
        private void sendText(string s)
        {
            output.Write(s);
            output.Flush();
        }
    }
}
