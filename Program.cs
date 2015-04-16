using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Ini;

namespace Better_CSharp_IRC_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            List<string> serverList = new List<string>();
            List<Server> serverConnections = new List<Server>();
            string appLocation = AppDomain.CurrentDomain.BaseDirectory;
            IniFile serverFile;
            if (!File.Exists(appLocation + "servers.ini"))
            {
                serverFile = new IniFile(appLocation + "servers.ini");
                int port;
                string server, nick, owner, name;
                //assume first run, start application setup.
                Console.WriteLine("Welcome to Better CSharp IRC Bot! It seems the server INI file is missing, so let's get a server set up for you.");
                Console.WriteLine();
                Console.Write("Enter the address of the server: ");
                server = Console.ReadLine();
                Console.Write("Enter a port for the bot (Usually 6667):");
                port = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter a nick for the bot: ");
                nick = Console.ReadLine();
                Console.Write("Enter an owner for the bot (This is you!): ");
                owner = Console.ReadLine();
                Console.Write("Enter a real name for the bot (OPTIONAL): ");
                name = Console.ReadLine();
                if (name == "") name = nick;

                //All done, now to save this information to the disk
                serverFile.IniWriteValue("Server", "Server0", server);
                Directory.CreateDirectory(appLocation + server);
                IniFile firstServer = new IniFile(appLocation + "\\" +  server + "\\config.ini");
                firstServer.IniWriteValue("Config", "Address", server);
                firstServer.IniWriteValue("Config", "Port", port.ToString());
                firstServer.IniWriteValue("Config", "Nick", nick);
                firstServer.IniWriteValue("Config", "Owner", owner);
                firstServer.IniWriteValue("Config", "RealName", name);
            }
            serverFile = new IniFile(appLocation + "servers.ini");
            i = 0;
            while (serverFile.IniReadValue("Server", "Server" + i.ToString()) != "")
            {
                serverList.Add(serverFile.IniReadValue("Server", "Server" + i.ToString()));
                i++;
            }

            foreach (string s in serverList)
            {
                Server serv;
                IniFile connectServer = new IniFile(appLocation + "\\" + s + "\\config.ini");
                serverConnections.Add(serv = new Server(connectServer.IniReadValue("Config", "Nick"),
                    connectServer.IniReadValue("Config", "Address"),
                    connectServer.IniReadValue("Config", "RealName"),
                    Convert.ToInt32(connectServer.IniReadValue("Config", "Port")), connectServer.IniReadValue("Config", "Owner")));
            }
            //At this point, many foreground threads are made for each server and channel, so the main thread can close and allow the server and
            //channel threads to operate independently.
        }
    }
}
