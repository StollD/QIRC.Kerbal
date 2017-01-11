/// --------------------------------------
/// .NET Bot for Internet Relay Chat (IRC)
/// Copyright (c) ThomasKerman 2016
/// QIRC is licensed under the MIT License
/// --------------------------------------

/// IRC
using ChatSharp;
using ChatSharp.Events;

/// QIRC
using QIRC;
using QIRC.Configuration;
using QIRC.IRC;
using QIRC.Plugins;

/// System
using System;
using System.IO;
using System.Linq;
using PathIO = System.IO.Path;

/// <summary>
/// Here's everything that is an IrcCommand
/// </summary>
namespace QIRC.Commands
{
    /// <summary>
    /// This is the implementation for the Linux command. It will deliver the Linux Principia Build when called by
    /// an operator.
    /// </summary>
    public class Linux : IrcCommand
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
            return "linux";
        }

        /// <summary>
        /// Returns a description of the command
        /// </summary>
        public override String GetDescription()
        {
            return "Delivers the Principia Linux Build when called in #principia";
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
                BotController.SendMessage(client, "This command can only be used in #principia.", message.User, message.Source);
                return;
            }
            if (!File.Exists(Constants.Paths.settings + "principia.txt"))
                File.Create(Constants.Paths.settings + "principia.txt");
            String[] builds = File.ReadAllLines(Constants.Paths.settings + "principia.txt");
            if (builds.Count(s => s.StartsWith("Linux:")) == 1)
                BotController.SendMessage(client, builds.First(s => s.StartsWith("Linux: ")).Remove(0, "Linux: ".Length), message.User, message.Source, true);
            else
                BotController.SendMessage(client, "There seems to be no build for Linux!", message.User, message.Source, true);
        }
    }
}
