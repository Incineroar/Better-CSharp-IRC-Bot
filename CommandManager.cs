using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Better_CSharp_IRC_Bot
{
    class CommandManager
    {
        private string channel; //for commands in a channel
        private string nick; //for commands in query
        private string ownNick; //in the event that a command requires use of the bot's nick
        private string ownerNick;

        public CommandManager(string nick, string owner)
        {
            ownNick = nick;
            ownerNick = owner;
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
            
            if (message.Contains(" :.lenny"))
            {
                try
                {
                    string x = System.Text.RegularExpressions.Regex.Match(message, @" :.lenny \d+").Value;
                    string x2 = x.Substring(x.LastIndexOf(' '), x.Length - x.LastIndexOf(' ')).Trim();
                    int tmp = int.Parse(x2);
                    string lenny = getLenny(tmp);
                    return "PRIVMSG " + channel + " :" + nick + ": " + lenny;
                }
                catch
                {
                    return "PRIVMSG " + channel + " :" + nick + ": ( ͡° ͜ʖ ͡°)";
                }
            }
            else if (message.Contains(" :.shia"))
            {
                string tmp = getShiaLine();
                return "PRIVMSG " + channel + " :" + nick + ": " + tmp;
            }
            else if (message.Contains("http://") || message.Contains("https://") && nick != null)
            {
                if (message.Contains("332"))
                {
                    if (message.IndexOf("332") < message.IndexOf(" :"))
                        return null;
                }
                string urlstring = message;
                string urlstring2 = "";
                for (; ; )
                {
                    if (urlstring.Contains("http://"))
                    {
                        urlstring2 = urlstring.Remove(0, urlstring.IndexOf("http://"));
                        break;
                    }
                    else if (urlstring.Contains("https://"))
                    {
                        urlstring2 = urlstring.Remove(0, urlstring.IndexOf("https://"));
                        break;
                    }
                    else if (urlstring.Contains("www."))
                    {
                        urlstring2 = urlstring.Remove(0, urlstring.IndexOf("www."));
                        break;
                    }
                    else return null;
                }
                string final;
                try
                {
                    final = urlstring2.Remove(urlstring2.IndexOf(' '), urlstring2.Length - urlstring2.IndexOf(' '));
                }
                catch
                {
                    final = urlstring2;
                }
                string result = GetWebPageTitle(final);
                if (result != null)//check to see if there actually is a title found or not.
                {
                    if (result != "")
                    {
                        return "PRIVMSG " + channel + " :" + nick + ": " + result;
                    }
                }
                return null;
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
            if (message.Contains(" :QUIT") && ownerNick.Equals(nick))
            {
                try
                {
                    string reason = message.Substring(message.IndexOf(" :QUIT") + 7, message.Length - message.IndexOf(" :QUIT") - 7);
                    return "93J13QjAQiaxBvrMCpk3 " + reason; //use a random value here so it doesn't get confused if trying to return a message and accidentally shuts down. Change this for your server as the default is a security risk!
                }
                catch
                {
                    return "93J13QjAQiaxBvrMCpk3 Kill command issued by " + ownerNick;
                }
            }
            else return null;
        }

        private string getLenny(int numberOfLenny)
        {
            switch (numberOfLenny)
            {
                case 1: return "( ͡° ͜ʖ ͡°)";
                case 2: return "( ͠° ͟ʖ ͡°)";
                case 3: return "ᕦ( ͡° ͜ʖ ͡°)ᕤ";
                case 4: return "( ͡~ ͜ʖ ͡°)";
                case 5: return "( ͡o ͜ʖ ͡o)";
                case 6: return "͡° ͜ʖ ͡ -";
                case 7: return "( ͡͡ ° ͜ ʖ ͡ °)﻿";
                case 8: return "( ͡ ͡° ͡°  ʖ ͡° ͡°)";
                case 9: return "(ง ͠° ͟ل͜ ͡°)ง";
                case 10: return "( ͡° ͜ʖ ͡ °)";
                case 11: return "(ʖ ͜° ͜ʖ)";
                case 12: return "[ ͡° ͜ʖ ͡°]";
                case 13: return "ヽ༼ຈل͜ຈ༽ﾉ";
                case 14: return "( ͡o ͜ʖ ͡o)";
                case 15: return "{ ͡• ͜ʖ ͡•}";
                case 16: return "( ͡° ͜V ͡°)";
                case 17: return "( ͡^ ͜ʖ ͡^)";
                case 18: return "( ‾ʖ̫‾)";
                case 19: return "( ͡°╭͜ʖ╮͡° )";
                case 20: return "ᕦ( ͡°╭͜ʖ╮͡° )ᕤ";
                default: return "I don't know what you chose! Lennies used are taken from http://www.alexdantas.net/lenny/ at the top of the page, in order from left to right, top to bottom, numbered 1 to 20.";
            }
        }

        public static string GetWebPageTitle(string url)
        {
            // Create a request to the url
            if (url.StartsWith("www.") == true)//check if it only starts with www. so http:// can be added to it.
            {
                string temp = url;
                url = temp.Insert(0, "http://");
            }

            string title = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest.Create(url) as HttpWebRequest);
                HttpWebResponse response = (request.GetResponse() as HttpWebResponse);

                using (Stream stream = response.GetResponseStream())
                {
                    // compiled regex to check for <title></title> block
                    Regex titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    int bytesToRead = 8092;
                    byte[] buffer = new byte[bytesToRead];
                    string contents = "";
                    int length = 0;
                    while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                    {
                        // convert the byte-array to a string and add it to the rest of the
                        // contents that have been downloaded so far
                        contents += Encoding.UTF8.GetString(buffer, 0, length);
                        contents = WebUtility.HtmlDecode(contents);

                        Match m = titleCheck.Match(contents);
                        if (m.Success)
                        {
                            // we found a <title></title> match =]
                            title = m.Groups[1].Value.ToString();
                            return title;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            // reached end of head-block; no title found =[
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null; //why must this be here .-.
        }

        public string getShiaLine()
        {
            Random a = new Random();
            int x = a.Next(0, 7);
            switch(x)
            {
                case 0:
                    return "DO IT! JUST DO IT!";
                case 1:
                    return "Don't let your dreams be dreams.";
                case 2:
                    return "If you're tired of starting over, STOP GIVING UP.";
                case 3:
                    return "Yes you can. Just do it.";
                case 4:
                    return "Some people dream of success, while you're gonna wake up and work hard at it. NOTHING IS IMPOSSIBLE.";
                case 5:
                    return "You should get to the point where anyone else would quit, and you're not gonna stop there!";
                case 6:
                    return "No... What are you waiting for? JUST... DO IT!";
                default:
                    return "JUST DO IT! Yes, you can!";
            }
        }
    }
}
