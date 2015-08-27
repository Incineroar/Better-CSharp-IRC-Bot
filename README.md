# Better C# IRC Bot
This is a simple IRC bot written in C#. Functions:
- Handles multiple server connections at once
- Handles multiple channels on each server
- Incredibly well-threaded as to ensure all servers don't lose connection (That is, each server is on its own dedicated thread)

## To be added:
- More commands (.lenny, .shia, and URL parsing are some sample commands, as well as an admin backend via query)
- Better code optimisation
- More admin commands (Such as restart server, server statistics)
- Error checking
- Nickserv authentication for the bot (To prevent stealing of the bot's nick, password being set by admin of the bot)
- More?

## Known Bugs
- Logging is in the process of being repaired
- Initial setup does not ask for any channels. Users must manually add a chans.ini file with channels defined in it (Fixed for later publishing)

## To use this bot
To run the bot on your own server, simply build and start the application and follow the simple startup guide in the command prompt window. After the initial setup, it will save your settings to the application's directory, then hook up to the server that you defined. You must be running .NET 4 at minimum (Although if you adjust this in the Properties, you may be able to get it to build with an older version of .NET, although this hasn't been tested as of yet).

## Credits
The original code that this bot is based off of was shared on many forums, so I can't directly credit it until I find the source of the code.

The INI class that's in the application was written by BLaZiNiX (http://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C)
