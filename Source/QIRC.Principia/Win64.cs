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
    /// This is the implementation for the win64 command. It will deliver the Win64 Principia Build when called by
    /// an operator.
    /// </summary>
    public class Win64 : IrcCommand
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
            return "Delivers the Principia Win64 Build when called in #principia";
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
            if (message.Source != "#principia")
            {
                QIRC.SendMessage(client, "This command can only be used in #principia.", message.User, message.Source);
                return;
            }
            if (!File.Exists(Constants.Paths.settings + "principia.txt"))
                File.Create(Constants.Paths.settings + "principia.txt");
            String[] builds = File.ReadAllLines(Constants.Paths.settings + "principia.txt");
            if (builds.Count(s => s.StartsWith("Win32:")) == 1)
                QIRC.SendMessage(client, builds.First(s => s.StartsWith("Win64: ")).Remove(0, "Win64: ".Length), message.User, message.Source, true);
            else
                QIRC.SendMessage(client, "There seems to be no build for Win64!", message.User, message.Source, true);
        }
    }
}
