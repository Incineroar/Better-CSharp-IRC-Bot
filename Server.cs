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
        System.IO.TextReader input;
        System.IO.TextWriter output;
        CommandManager cm;
        bool connections = false;
        System.Net.Sockets.TcpClient sock;

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
            cm = new CommandManager(nick, owner);
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
            cm = new CommandManager(nick, owner);
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
            sock = new System.Net.Sockets.TcpClient();
            sock.Connect(server, port);
            if (!sock.Connected)
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Failed to connect: " + server + ":" + port.ToString() + ". Abandoning connection to server!");
                return;
            }
            else Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Successfully connected to server " + server + ":" + port.ToString() + ".");

            input = new System.IO.StreamReader(sock.GetStream());
            output = new System.IO.StreamWriter(sock.GetStream());
            //sendText("NOTICE " + owner + " " + nick + " is now LOGGED ON.");
            //Establish connection using nick.
            sendText("USER " + nick + " * 0 :" + realName);
            sendText("NICK " + nick);
            string buf;
            try
            {
                for (buf = input.ReadLine(); ; buf = input.ReadLine())
                {
                    Console.WriteLine(buf);
                    if (!buf.Equals(""))
                    {
                        if (buf.Split(' ')[1] == "001" && !connections)
                        {
                            Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Got the 001 command, joining channels...");
                            connections = true;
                            foreach (string s in chans)
                            {
                                connectToChannel(s);
                            }
                        }
                    }
                    if (buf.StartsWith("PING"))
                    {
                        Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Replied to a PING request sent from the server.");
                        sendText(buf.Replace("PING", "PONG"));
                    }
                    string response = cm.parse(buf);
                    if (response != null)
                    {
                        if (response.Contains("93J13QjAQiaxBvrMCpk3"))
                        {
                            string tmp = response.Remove(0, 21);
                            shutdown(tmp);
                            return;
                        }
                    }
                    if (response != null) sendText(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "ERROR: " + ex.ToString());
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Abandoning connection!");
                shutdown("An error has occured and the application needs to close.");
                //Console.Beep();
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Sends a string of text to the IRC server.
        /// </summary>
        /// <param name="s">The text to write to the server. DOES NOT include return characters</param>
        private void sendText(string s)
        {
            output.Write(s + "\r\n");
            output.Flush();
        }

        /// <summary>
        /// Connects to a defined channel.
        /// </summary>
        /// <param name="s">The channel to connect to. # symbol for channel is optional.</param>
        private void connectToChannel(string s)
        {
            if (s.StartsWith("#"))
            {
                try
                {
                    sendText("JOIN " + s);
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Successfully joined " + s + ".");
                }
                catch
                {
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Failed to connect to channel " + s + ". Quitting the attempt...");
                }
            }
            else
            {
                try
                {
                    sendText("JOIN #" + s);
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Successfully joined #" + s + ".");
                }
                catch
                {
                    Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Failed to connect to channel #" + s + ". Quitting the attempt...");
                    return;
                }
            }
        }

        public void shutdown(string reason)
        {
            string x = reason;
            foreach (string s in chans)
            {
                if (s.StartsWith("#"))
                {
                    try
                    {
                        sendText("PART " + s + " :" + x);
                    }
                    catch
                    {
                        
                    }
                }
                else
                {
                    try
                    {
                        sendText("PART #" + s + " :" + x);
                    }
                    catch
                    {
                        
                    }
                }
            }
            sendText("QUIT");
            sock.Close();
        }
    }
}
