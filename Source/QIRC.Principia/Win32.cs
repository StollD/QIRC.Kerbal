/** 
 * .NET Bot for Internet Relay Chat (IRC)
 * Copyright (c) ThomasKerman 2016
 * QIRC is licensed under the MIT License
 */

using ChatSharp;
using QIRC.Configuration;
using QIRC.IRC;
using QIRC.Plugins;
using System;
using System.IO;
using System.Linq;
using PathIO = System.IO.Path;

namespace QIRC.Commands
{
    /// <summary>
    /// This is the implementation for the win32 command. It will deliver the Win32 Principia Build when called by
    /// an operator.
    /// </summary>
    public class Win32 : IrcCommand
    {
        /// <summary>
        /// The Access Level that is needed to execute the command
        /// </summary>
        public override AccessLevel GetAccessLevel()
        {
            return AccessLevel.NORMAL;
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public override String GetName()
        {
            return "win32";
        }

        /// <summary>
        /// Returns a description of the command
        /// </summary>
        public override String GetDescription()
        {
            return "Delivers the Principia Win32 Build when called in #principia";
        }

        /// <summary>
        /// Whether the command can be used in serious channels.
        /// </summary>
        public override Boolean IsSerious()
        {
            return true;
        }

        /// <summary>
        /// An example for using the command.
        /// </summary>
        /// <returns></returns>
        public override String GetExample()
        {
            return Settings.Read<String>("control") + GetName();
        }

        /// <summary>
        /// Here we run the command and evaluate the parameters
        /// </summary>
        public override void RunCommand(IrcClient client, ProtoIrcMessage message)
        {
            BotController.SendMessage(client, "Win32 is no longer supported. Please use the x64 build, available through !win64, or get a new bird.", message.User, message.Source, true);
        }
    }
}
