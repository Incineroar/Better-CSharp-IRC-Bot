using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Better_CSharp_IRC_Bot
{
    class CommandManager
    {
        private string channel; //for commands in a channel
        private string nick; //for commands in query
        private string ownNick; //in the event that a command requires use of the bot's nick

        public CommandManager(string nick)
        {
            ownNick = nick;
        }

        public string parse(string message)
        {
            channel = getChannel(message);
            nick = getUserNick(message);
            if (nick == null && channel == null)
            {
                return null; //No channel or nick information, so we'll do nothing.
            }
            if (channel == null)
            {
                return processQueryMessage(message);
            }
            else
            {
                return processChannelMessage(message);
            }
        }

        /// <summary>
        /// Returns the channel that is in a raw message.
        /// </summary>
        /// <param name="msg">The message that contains a channel.</param>
        /// <returns>The channel in which a message was sent, if found.</returns>
        private string getChannel(string msg)
        {
            try // Use a try / catch here. If this line fails, that means that a channel string is not present and therefore must be a query.
            {
                return msg.Substring(msg.IndexOf('#'), msg.IndexOf(" :") - msg.IndexOf('#'));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nickname of the user that sent the message.
        /// </summary>
        /// <param name="msg">The message that contains a user's nick.</param>
        /// <returns>The nickname of the user that sent the message</returns>
        private string getUserNick(string msg)
        {
            try
            {
                return msg.Remove(0, 1).Remove(msg.IndexOf('!') - 1);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Processes messages intended for channels.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>A message to be sent to the IRC server for the channel intended.</returns>
        private string processChannelMessage(string message)
        {
            if (message.StartsWith("PING"))
            {
                Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "] " + "Replied to a PING request (Server class).");
                return message.Replace("PING", "PONG");
            }
            else if (message.Contains(" :.lenny"))
            {
                return "PRIVMSG " + channel + " :( ͡° ͜ʖ ͡°)";
            }
            else return null;
        }

        /// <summary>
        /// Processses messages intended for queries.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>A message to be sent to the IRC server for the user intended.</returns>
        private string processQueryMessage(string message)
        {
            if (message.Contains(" :ADMIN"))
            {
                return "PRIVMSG " + nick + " :ADMIN MODE ACTIVATED.";
            }
            else return null;
        }
    }
}
