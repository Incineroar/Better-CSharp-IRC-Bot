using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Better_CSharp_IRC_Bot
{
    class CommandManager
    {
        protected string channel;
        protected string nick;
        public static string parse(string message)
        {
            if (message.Contains("PING"))
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Replied to a PING request (Server class).");
                return message.Replace("PING", "PONG");
            }
            else if (message.Contains(" :.lenny"))
            {
                return "PRIVMSG #rhm :" + "( ͡° ͜ʖ ͡°)";
            }
            else return null;
        }
    }
}
