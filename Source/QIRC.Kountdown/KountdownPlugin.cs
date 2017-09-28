/** 
 * .NET Bot for Internet Relay Chat (IRC)
 * Copyright (c) Dorian Stoll 2017
 * QIRC is licensed under the MIT License
 */

using QIRC.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using System.Threading;

namespace QIRC.Kountdown
{
    /// <summary>
    /// The countdown IrcPlugin. Here we check the list of kountdowns every x minutes and display a notification.
    /// </summary>
    public class KountdownPlugin : IrcPlugin
    {
        /// <summary>
        /// The thread that is running in the background
        /// </summary>
        public Thread messageThread { get; set; }

        public static List<Tuple<Int32, DateTime>> queue;
        public static Object lockQueue = new Object();

        /// <summary>
        /// When the connection to the server was successfull, start the thread
        /// </summary>
        /// <param name="client"></param>
        public override void OnConnectionComplete(IrcClient client)
        {
            // build the queue
            queue = Event.BuildQueue();

            messageThread = new Thread(MessageWorker);
            messageThread.IsBackground = true;
            messageThread.Start(client);
        }

        public void MessageWorker(Object _client)
        {
            IrcClient client = (IrcClient)_client;

            while (BotController.isConnected)
            {
                Thread.Sleep(5000);
                if (!queue.Any())
                {
                    continue;
                }
                lock (lockQueue)
                {
                    Tuple<Int32, DateTime> item = queue[0];
                    if (item.Item2 <= DateTime.UtcNow)
                    {
                        Event evt = null;
                        try
                        {
                            Int32 ID = item.Item1;
                            evt = Event.Query.First(e => e.ID == ID);
                        }
                        catch (Exception e)
                        {
                            queue.RemoveAt(0);
                            continue;
                        }
                        String mpref = $"{(evt.Time - item.Item2).ToString("d'd 'h'h 'm'm 's's'")} left to event #{evt.ID}: {evt.Name}";
                        String privm = $"<< ! >> {mpref} ({evt.Description}) at {evt.Time.ToString("yyyy-MM-dd HH:mm:ss")} [unixtime {(evt.Time - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds}]";
                        String chanm = $"{mpref} [at {evt.Time.ToString("yyyy-MM-dd HH:mm:ss")}]. Say '!kountdown {evt.ID}' for details";
                        foreach (SubscriberData data in SubscriberData.Query)
                        {
                            if (data.Name.StartsWith("#"))
                            {
                                client.SendNotice(chanm, data.Name);
                            }
                            else
                            {
                                client.SendMessage(privm, data.Name);
                            }
                        }
                        queue.RemoveAt(0);
                        if (evt.Time == item.Item2)
                        {
                            // Event has expired
                            queue = Event.BuildQueue();
                        } 
                    }
                }
            }
        }
    }
}