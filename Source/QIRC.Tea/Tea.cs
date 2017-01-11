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
using System.Linq;

namespace QIRC.Commands
{
    /// <summary>
    /// This is the implementation for the tea command. The bot will grab a random word from a list
    /// and pass it to another bot called teabot, which will do funny things with it.
    /// </summary>
    public class Tea : IrcCommand
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
            return "tea";
        }

        /// <summary>
        /// Returns a description of the command
        /// </summary>
        public override String GetDescription()
        {
            return "Passes a random word to a bot called teabot.";
        }

        /// <summary>
        /// Whether the command can be used in serious channels.
        /// </summary>
        public override Boolean IsSerious()
        {
            return false;
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
            if (!message.IsChannelMessage)
                return;

            if (client.Channels[message.Source].Users.Contains("teabot"))
            {
                String[] words = Words.words.Split('\n');
                Boolean noTea = new Random().Next(0, 100) == 1;
                Func<String, Boolean> predicate = s => s.Contains("te") || s.Contains("ti") || s.Contains("ty");
                String[] select = words.Where(s => noTea ? !predicate(s) : predicate(s)).ToArray();
                String word = select[new Random().Next(0, select.Length)];
                BotController.SendMessage(client, "teabot: " + word, message.User, message.Source, true);
            }
            else
            {
                BotController.SendMessage(client, "No teabot!", message.User, message.Source);
            }
        }
    }
}
