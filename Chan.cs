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
        System.IO.TextReader input;
        System.IO.TextWriter output;

        public Chan(string channel, TextReader tr, TextWriter tw, string nick)
        {
            this.channel = channel;
            this.input = tr;
            this.output = tw;
            this.nick = nick;
            startConnection();
        }

        private void startConnection()
        {
            ThreadStart job = new ThreadStart(() => channelConnect(channel, nick)); //use a lambda expression so it stops complaining.
            Thread thread = new Thread(job);
            //thread.IsBackground = true;
            thread.Start();
        }

        private void channelConnect(string channel, string nick)
        {
            string buf;
            for (buf = input.ReadLine(); ; buf = input.ReadLine())
            {
                try
                {
                    if (buf.Split(' ')[1] == "001") //if 001 is sent, join channel.
                    {
                        if (channel != "")
                        {
                            output.Write("JOIN " + channel + "\r\n");
                            output.Flush();
                            Console.WriteLine("Successfully joined " + channel + ".");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to connect to channel " + channel + ". Quitting the attempt...");
                    break;
                }

                //reply to any PING requests from the server. (UNKNOWN IF WILL WORK)
                if (buf.StartsWith("PING ")) { output.Write(buf.Replace("PING", "PONG") + "\r\n"); output.Flush(); }

                if (buf.Contains(channel + " :" + nick)) { output.Write("PRIVMSG " + channel + " :" + "meep. Got me!\r\n"); output.Flush(); }
            }
        }
    }
}
