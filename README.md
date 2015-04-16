# Better C# IRC Bot
This is a simple IRC bot written in C#. Functions:
- Handles multiple server connections at once
- Handles multiple channels on each server
- Incredibly well-threaded as to ensure all servers don't lose connection

## To be added:
- More commands (Currently only responds to its own name as a quick test)
- Better code optimisation
- Admin commands (Such as restart server, kill server, server statistics)
- Error checking
- Nickserv authentication for the bot (To prevent stealing of the bot's nick, password being set by admin of the bot)
- More?

## To use this bot
To run the bot on your own server, simply build and start the application and follow the simple startup guide in the command prompt window. After the initial setup, it will save your settings to the application's directory, then hook up to the server that you defined. You must be running .NET 4 at minimum (Although if you adjust this in the Properties, you may be able to get it to build with an older version of .NET).

## Credits
The original code that this bot is based off of was shared on many forums, so I can't directly credit it until I find the source of the code.

The INI class that's in the application was written by BLaZiNiX (http://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C)
