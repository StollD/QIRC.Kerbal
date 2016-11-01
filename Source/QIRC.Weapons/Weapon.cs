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
using QIRC.Serialization;

namespace QIRC.Commands
{
    /// <summary>
    /// This is the implementation for the weapon command. Gives you weapons
    /// </summary>
    public class Weapon : IrcCommand
    {
        public static SerializeableList<String> weapons = new SerializeableList<String>("weapons");
        public static SerializeableList<String> adjectives = new SerializeableList<String>("adjectives");

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
            return "weapon";
        }

        /// <summary>
        /// Alternative names
        /// </summary>
        public override String[] GetAlternativeNames()
        {
            return new[] {"wpn"};
        }

        /// <summary>
        /// Returns a description of the command
        /// </summary>
        public override String GetDescription()
        {
            return "Creates weapons from a list of objects and adjectives";
        }

        /// <summary>
        /// Whether the command can be used in serious channels.
        /// </summary>
        public override Boolean IsSerious()
        {
            return false;
        }

        /// <summary>
        /// The Parameters of the Command
        /// </summary>
        public override String[] GetParameters()
        {
            return new String[]
            {
                "add", "Adds objects or adjectives",
                "remove", "Removes objects or adjectives",
                "stats", "Shows data about the weapons"
            };
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
            String msg = message.Message;
            if (StartsWithParam("add", message.Message))
            {
                String type = StripParam("add", ref msg);
                if (type == "wpn")
                {
                    if (weapons.Contains(msg))
                        QIRC.SendMessage(client, "Weapon already added!", message.User, message.Source);
                    else
                    {
                        weapons.Add(msg);
                        QIRC.SendMessage(client, "Weapon added!", message.User, message.Source);
                    }
                }
                else if (type == "adj")
                {
                    if (adjectives.Contains(msg))
                        QIRC.SendMessage(client, "Adjective already added!", message.User, message.Source);
                    else
                    {
                        adjectives.Add(msg);
                        QIRC.SendMessage(client, "Adjective added!", message.User, message.Source);
                    }
                }
                else
                {
                    QIRC.SendMessage(client, "Invalid type", message.User, message.Source);
                }
                return;
            }
            if (StartsWithParam("remove", message.Message))
            {
                String type = StripParam("remove", ref msg);
                if (type == "wpn")
                {
                    if (!weapons.Contains(msg))
                        QIRC.SendMessage(client, "Weapon doesn't exist!", message.User, message.Source);
                    else
                        weapons.Remove(msg);
                }
                else if (type == "adj")
                {
                    if (!adjectives.Contains(msg))
                        QIRC.SendMessage(client, "Adjective doesn't exist!", message.User, message.Source);
                    else
                        adjectives.Remove(msg);
                }
                else
                {
                    QIRC.SendMessage(client, "Invalid type", message.User, message.Source);
                }
                return;
            }
            if (StartsWithParam("stats", message.Message))
            {
                Int32 weaponsnum = weapons.Count;
                Int32 adjsnum = adjectives.Count;
                Int64 combospossible = (weaponsnum) + (weaponsnum * adjsnum) + (4 * weaponsnum * weaponsnum * adjsnum) + (weaponsnum * adjsnum * adjsnum) + (4 * weaponsnum * weaponsnum * adjsnum * adjsnum);
                QIRC.SendMessage(client, $"Total weapons: {weaponsnum}. Total adjectives: {adjsnum}. Total possible combinations: {combospossible}.", message.User, message.Source);
                return;
            }
            Random r = new Random();
            String name = String.IsNullOrWhiteSpace(message.Message) ? message.User : message.Message;
            String weapon = weapons[r.Next(0, weapons.Count)];
            String adjective = adjectives[r.Next(0, adjectives.Count)];

            Int32 extraweapon = r.Next(0, 20); // roll a d20; if it comes up 1-4, do something silly with an extra weapon entry.
            if (extraweapon == 1) // weapon/weapon hybrid
                weapon += "/" + weapons[r.Next(0, weapons.Count)] + " hybrid";
            if (extraweapon == 2) // weapon with a weapon attachment
            {
                String wpn2 = weapons[r.Next(0, weapons.Count)];
                if (new[] {"a", "e", "i", "o", "u"}.Contains(wpn2.ToLower().Substring(0, 1)) && wpn2.ToLower().Substring(0, 2) != "eu")
                    weapon += " with an ";
                else
                    weapon += " with a ";
                weapon += wpn2 + " attachment";
            }
            if (extraweapon == 3) // weapon-like weapon
            {
                String wpn2 = weapons[r.Next(0, weapons.Count)];
                if (!wpn2.Contains(" "))
                    weapon = wpn2 + "-like " + weapon;
            }
            if (extraweapon == 4) // weapon which strongly/vaguely resembles a weapon
            {
                String wpn2 = weapons[r.Next(0, weapons.Count)];
                if (r.Next(0, 2) == 0) // pick strong/vague resemblance with a coin toss
                {
                    if (new[] {"a", "e", "i", "o", "u"}.Contains(wpn2.ToLower().Substring(0, 1)) && wpn2.ToLower().Substring(0, 2) != "eu")
                        weapon += " which vaguely resembles an " + wpn2;
                    else
                        weapon += " which vaguely resembles a " + wpn2;
                }
                else
                {
                    if (new[] {"a", "e", "i", "o", "u"}.Contains(wpn2.ToLower().Substring(0, 1)) && wpn2.ToLower().Substring(0, 2) != "eu")
                        weapon += " which strongly resembles an " + wpn2;
                    else
                        weapon += " which strongly resembles a " + wpn2;
                }
            }
            if (r.Next(0, 11) == 4) // roll a d10, if it comes up 4, give up on adjectives and return a plain old weapon.
                adjective = ""; // why 4? I don't know, go ask a psychologist. (And I dont know too)
            else
            {
                if (!adjective.EndsWith(">"))
                    adjective += " ";
                else
                    adjective = adjective.Substring(0, adjective.Length - 1);

                if (r.Next(0, 6) == 3) // roll a d5, if it comes up 3, use more adjectives! I do realize that this is actually a d6 because of that 0...
                {
                    String extraadj = adjectives[r.Next(0, adjectives.Count)]; // ...but I don't really care, to be honest.
                    if (!extraadj.EndsWith(">"))
                        extraadj += " ";
                    else
                        extraadj = adjective.Substring(0, adjective.Length - 1);

                    if (adjective.Length > 3) // more stuff. mostly a/an detection.
                    {
                        if (adjective.EndsWith(" a ") && new[] {"a", "e", "i", "o", "u"}.Contains(extraadj.ToLower().Substring(0, 1)) && extraadj.ToLower().Substring(0, 2) != "eu")
                            adjective = adjective.Substring(0, adjective.Length - 1) + "n ";
                    }
                    adjective += extraadj;
                }
            }
            if (adjective.Length > 3) // more stuff. mostly a/an detection.
            {
                if (adjective.EndsWith(" a ") && new[] {"a", "e", "i", "o", "u"}.Contains(weapon.ToLower().Substring(0, 1)) && weapon.ToLower().Substring(0, 2) != "eu")
                    adjective = adjective.Substring(0, adjective.Length - 1) + "n ";
            }
            weapon = adjective + weapon;
            if (new[] {"a", "e", "i", "o", "u"}.Contains(weapon.ToLower().Substring(0, 1)) && weapon.ToLower().Substring(0, 2) != "eu")
                weapon = " an " + weapon;
            else
                weapon = "a " + weapon;
            QIRC.SendAction(client, $"gives {name} {weapon}", message.Source);
        }
    }
}

