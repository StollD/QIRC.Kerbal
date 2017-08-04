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
using System.Collections.Generic;
using System.Linq;
using QIRC.Serialization;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;

namespace QIRC.Kountdown
{
    /// <summary>
    /// This is the implementation for the kountdown command.
    /// </summary>
    public class Kountdown : IrcCommand
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
            return "kountdown";
        }

        /// <summary>
        /// Alternative names
        /// </summary>
        public override String[] GetAlternativeNames()
        {
            return new[] { "kd" };
        }

        /// <summary>
        /// Returns a description of the command
        /// </summary>
        public override String GetDescription()
        {
            return "Prints out the details for Kountdown Events";
        }

        /// <summary>
        /// Whether the command can be used in serious channels.
        /// </summary>
        public override Boolean IsSerious()
        {
            return true;
        }

        /// <summary>
        /// The Parameters of the Command
        /// </summary>
        public override String[] GetParameters()
        {
            return new[]
            {
                "add", "Add Kountdown event. Syntax: " + Settings.Read<String>("control") + "kountdown -add name|description|time.",
                "list", "List pending Kountdown Events or subscribers",
                "remove", "Delete Kountdown by id.",
                "edit", "Edits a Kountdown by id.",
                "subscribe", "Subscribe yourself or a channel to Kountdown.",
                "unsubscribe", "Unsubscribes yourself or a channel from the Kountdown."
            };
        }

        /// <summary>
        /// An example for using the command.
        /// </summary>
        /// <returns></returns>
        public override String GetExample()
        {
            return Settings.Read<String>("control") + GetName() + " 1";
        }

        /// <summary>
        /// Here we run the command and evaluate the parameters
        /// </summary>
        public override void RunCommand(IrcClient client, ProtoIrcMessage message)
        {
            // Add event
            if (StartsWithParam("add", message.Message))
            {
                if (BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                {
                    String msg = message.Message;
                    StripParam("add", ref msg);
                    String[] data = msg.Split('|');
                    if (data.Length != 3)
                    {
                        BotController.SendMessage(client, "Invalid syntax. The correct syntax would be name|description|time.", message.User, message.Source);
                        return;
                    }
                    if (!DateTime.TryParse(data[2], out DateTime time))
                    {
                        BotController.SendMessage(client, "Invalid time syntax! Please use correct DateTime syntax.", message.User, message.Source);
                        return;
                    }
                    Event e = Event.Query.Insert(data[0], data[1], time);
                    BotController.SendMessage(client, "Added event #" + e.ID, message.User, message.Source);
                    return;
                }
                else
                {
                    BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.ADMIN + " can use this command! You are " + message.level + ".", message.User, message.Source);
                    return;
                }
            }

            // List
            if (StartsWithParam("list", message.Message))
            {
                String msg = message.Message;
                String type = StripParam("list", ref msg);
                if (type == "events")
                {
                    String content = "";
                    foreach (Event e in Event.Query)
                    {
                        content += $"[{e.ID}] {e.Name} - {e.Description} - {e.Time.ToString()}\n";
                    }
                    using (WebClient wc = new WebClient())
                    {
                        wc.Encoding = Encoding.UTF8;
                        String response = JObject.Parse(wc.UploadString("https://hastebin.com/documents", "POST", content))["key"].ToString();
                        BotController.SendMessage(client, "Events: https://hastebin.com/" + response, message.User, message.User, true);
                    }
                    if (message.IsChannelMessage) BotController.SendMessage(client, "I sent you a list of all Kountdown events.", message.User, message.Source);
                    return;
                }
                else if (type == "subs")
                {
                    if (BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                    {
                        String content = "";
                        foreach (SubscriberData data in SubscriberData.Query)
                        {
                            content += data.Name + "\n";
                        }
                        using (WebClient wc = new WebClient())
                        {
                            wc.Encoding = Encoding.UTF8;
                            String response = JObject.Parse(wc.UploadString("https://hastebin.com/documents", "POST", content))["key"].ToString();
                            BotController.SendMessage(client, "Subscribers: https://hastebin.com/" + response, message.User, message.User, true);
                        }
                        if (message.IsChannelMessage) BotController.SendMessage(client, "I sent you a list of all Kountdown subscribers.", message.User, message.Source);
                        return;
                    }
                    else
                    {
                        BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.ADMIN + " can use this command! You are " + message.level + ".", message.User, message.Source);
                        return;
                    }
                }
                else
                {
                    BotController.SendMessage(client, "Invalid list argument! Valid arguments are 'events' and 'subs'.", message.User, message.Source);
                    return;
                }
            }

            // Remove
            if (StartsWithParam("remove", message.Message))
            {
                if (BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                {
                    String msg = message.Message;
                    String sid = StripParam("add", ref msg);
                    if (!Int32.TryParse(sid, out Int32 id))
                    {
                        BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                        return;
                    }
                    Event e = Event.Query.FirstOrDefault(ev => ev.ID == id);
                    if (e == null)
                    {
                        BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                        return;
                    }
                    Event.Query.Delete(ev => ev.ID == id);
                    BotController.SendMessage(client, "Removed event #" + id, message.User, message.Source);
                    return;
                }
                else
                {
                    BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.ADMIN + " can use this command! You are " + message.level + ".", message.User, message.Source);
                    return;
                }
            }

            // Edit 
            if (StartsWithParam("edit", message.Message))
            {
                if (BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                {
                    String msg = message.Message;
                    String sid = StripParam("edit", ref msg);
                    if (!Int32.TryParse(sid, out Int32 id))
                    {
                        BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                        return;
                    }
                    Event e = Event.Query.FirstOrDefault(ev => ev.ID == id);
                    if (e == null)
                    {
                        BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                        return;
                    }
                    if (msg.StartsWith("name "))
                    {
                        msg = msg.Replace("name ", "");
                        e.Name = msg;
                    }
                    if (msg.StartsWith("description "))
                    {
                        msg = msg.Replace("description ", "");
                        e.Description = msg;
                    }
                    if (msg.StartsWith("time "))
                    {
                        msg = msg.Replace("time ", "");
                        if (!DateTime.TryParse(msg, out DateTime time))
                        {
                            BotController.SendMessage(client, "Invalid time syntax! Please use correct DateTime syntax.", message.User, message.Source);
                            return;
                        }
                        e.Time = time;
                        KountdownPlugin.queue = e.RebuildQueue(KountdownPlugin.queue);
                    }
                    BotController.Database.Update(e);
                    BotController.SendMessage(client, $"Updated event #{e.ID}: {e.Name} - {e.Description} - {e.Time.ToString()}", message.User, message.Source);
                    return;
                }
                else
                {
                    BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.ADMIN + " can use this command! You are " + message.level + ".", message.User, message.Source);
                    return;
                }
            }

            // Subscribe
            if (StartsWithParam("subscribe", message.Message))
            {
                String msg = message.Message;
                String target = StripParam("subscribe", ref msg);
                if (String.IsNullOrWhiteSpace(target))
                {
                    // Subscribe ourselves
                    SubscriberData.Query.Insert(message.User);
                    BotController.SendMessage(client, "Added you to the Kountdown subscribers.", message.User, message.Source);
                    return;
                }
                else if (target.StartsWith("#"))
                {
                    // Its a channel
                    if (Settings.Read<List<ProtoIrcChannel>>("channels").Count(c => String.Equals(c.name, target, StringComparison.InvariantCultureIgnoreCase)) == 0)
                    {
                        BotController.SendMessage(client, "I am not in the channel " + target + ".", message.User, message.Source);
                        return;
                    }

                    // We know the channel
                    IrcChannel channel = client.Channels[target];
                    IrcUser user = client.Users[message.User];
                    if (user.ChannelModes[channel] == 'o' || user.ChannelModes[channel] == 'O' || BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                    {
                        // User is an operator
                        SubscriberData.Query.Insert(target);
                        BotController.SendMessage(client, "Added the channel to the Kountdown subscribers.", message.User, message.Source);
                        return;
                    }
                    else
                    {
                        BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.OPERATOR + " in " + target + " can use this command!", message.User, message.Source);
                        return;
                    }
                }
                else
                {
                    BotController.SendMessage(client, "Invalid channel name!", message.User, message.Source);
                    return;
                }
            }

            // Unsubscribe
            if (StartsWithParam("unsubscribe", message.Message))
            {
                String msg = message.Message;
                String target = StripParam("unsubscribe", ref msg);
                if (String.IsNullOrWhiteSpace(target))
                {
                    // Unsubscribe ourselves
                    String name = message.User;
                    if (SubscriberData.Query.Count(s => s.Name == name) > 0)
                    {
                        SubscriberData.Query.Delete(s => s.Name == name);
                        BotController.SendMessage(client, "Removed you from the Kountdown subscribers.", message.User, message.Source);
                        return;
                    }
                    else
                    {
                        BotController.SendMessage(client, "You are not subscribed to Kountdown events.", message.User, message.Source);
                        return;
                    }
                }
                else if (target.StartsWith("#"))
                {
                    // Its a channel
                    if (SubscriberData.Query.Count(s => s.Name == target) > 0)
                    {
                        // We know the channel
                        IrcChannel channel = client.Channels[target];
                        IrcUser user = client.Users[message.User];
                        if (user.ChannelModes[channel] == 'o' || user.ChannelModes[channel] == 'O' || BotController.CheckPermission(AccessLevel.ADMIN, message.level))
                        {
                            // User is an operator
                            SubscriberData.Query.Delete(s => s.Name == target);
                            BotController.SendMessage(client, "Removed the channel from the Kountdown subscribers.", message.User, message.Source);
                            return;
                        }
                        else
                        {
                            BotController.SendMessage(client, "You don't have the permission to use this command! Only " + AccessLevel.OPERATOR + " in " + target + " can use this command!", message.User, message.Source);
                            return;
                        }
                    }
                    else
                    {
                        BotController.SendMessage(client, "This channel is not subscribed to Kountdown events.", message.User, message.Source);
                        return;
                    }
                }
                else
                {
                    BotController.SendMessage(client, "Invalid channel name!", message.User, message.Source);
                    return;
                }
            }

            // No params
            if (!Int32.TryParse(message.Message, out Int32 ID))
            {
                BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                return;
            }
            Event evnt = Event.Query.FirstOrDefault(ev => ev.ID == ID);
            if (evnt == null)
            {
                BotController.SendMessage(client, "Invalid ID!", message.User, message.Source);
                return;
            }
            BotController.SendMessage(client, $"ID: {evnt.ID} | Name: {evnt.Name} | Time: {evnt.Time.ToString()} | Unixtime: {evnt.Time.Ticks} | Left: {(evnt.Time - DateTime.UtcNow).ToString()}", message.User, message.Source);
            BotController.SendMessage(client, $"Description: {evnt.Description}", message.User, message.Source);
        }
    }
}

