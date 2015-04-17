using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Better_CSharp_IRC_Bot
{
    class Chan
    {
        protected string channel;
        protected string nick;
        protected string server;
        System.IO.TextReader input;
        System.IO.TextWriter output;

        public Chan(string channel, TextReader tr, TextWriter tw, string nick, string server)
        {
            this.channel = channel;
            this.input = tr;
            this.output = tw;
            this.nick = nick;
            this.server = server;
            startConnection();
        }

        private void startConnection()
        {
            ThreadStart job = new ThreadStart(() => channelConnect(channel, nick)); //use a lambda expression so it stops complaining.
            Thread thread = new Thread(job);
            thread.Start();
        }

        private void channelConnect(string channel, string nick)
        {
            string buf;
            try
            {
                sendText("JOIN " + channel + "\r\n");
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Successfully joined " + channel + ".");
            }
            catch
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Failed to connect to channel " + channel + ". Quitting the attempt...");
                return;
            }
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
                    if (buf.Contains("PING")) { sendText(buf.Replace("PING", "PONG") + "\r\n"); Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Replied to a PING request (Chan class)."); }
                    if (buf.Contains(channel + " :!meep")) { sendText("PRIVMSG " + channel + " :" + "meep. Got me!\r\n"); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "ERROR: " + ex.ToString());
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Abandoning channel " + channel + "!");
                sendText("PRIVMSG " + channel + " :" + "An error has occured! Leaving channel!\r\n");
                sendText("PART " + channel + " An error in the application requires this thread be closed. The bot will have to be restarted to rejoin this channel.\r\n");
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
