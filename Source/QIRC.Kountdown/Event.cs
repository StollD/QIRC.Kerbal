/** 
 * .NET Bot for Internet Relay Chat (IRC)
 * Copyright (c) Dorian Stoll 2017
 * QIRC is licensed under the MIT License
 */

using QIRC.Serialization;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QIRC.Kountdown
{
    /// <summary>
    /// A class to store kountdown events
    /// </summary>
    public class Event : Storage<Event>
    {
        [PrimaryKey, Unique, NotNull, AutoIncrement]
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime Time { get; set; }

        private static Int32[] times = new Int32[]
        {
                10 * 24 * 3600,
                7 * 24 * 3600,
                5 * 24 * 3600,
                4 * 24 * 3600,
                3 * 24 * 3600,
                2 * 24 * 3600,
                36 * 3600,
                24 * 3600,
                18 * 3600,
                12 *3600,
                9 * 3600,
                6 * 3600,
                4 * 3600,
                3 * 3600,
                2 * 3600,
                3600,
                30 * 60,
                10 * 60,
                5 * 30,
                0
        };

        public Event()
        {

        }

        public Event(String name, String description, DateTime time)
        {
            Name = name;
            Description = description;
            Time = time;
            KountdownPlugin.queue = RebuildQueue(KountdownPlugin.queue);
        }

        public List<Tuple<Int32, DateTime>> RebuildQueue(List<Tuple<Int32, DateTime>> queue)
        {
            lock (KountdownPlugin.lockQueue)
            {
                List<Tuple<Int32, DateTime>> newQueue = new List<Tuple<Int32, DateTime>>();
                foreach (Tuple<Int32, DateTime> e in queue)
                {
                    if (e.Item1 != ID)
                    {
                        newQueue.Add(e);
                    }
                }
                foreach (Int32 t in times)
                {
                    TimeSpan span = new TimeSpan(0, 0, t);
                    if (Time - span >= DateTime.UtcNow)
                    {
                        newQueue.Add(new Tuple<Int32, DateTime>(ID, Time - span));
                    }
                }
                newQueue.OrderBy(t => t.Item2.Ticks);
                return newQueue;
            }
        }

        public static List<Tuple<Int32, DateTime>> BuildQueue()
        {
            lock (KountdownPlugin.lockQueue)
            {
                List<Tuple<Int32, DateTime>> queue = new List<Tuple<Int32, DateTime>>();
                foreach (Event e in Query)
                {
                    if (e.Time < DateTime.UtcNow)
                    {
                        continue;
                    }
                    foreach (Int32 t in times)
                    {
                        TimeSpan span = new TimeSpan(0, 0, t);
                        if (e.Time - span >= DateTime.UtcNow)
                        {
                            queue.Add(new Tuple<Int32, DateTime>(e.ID, e.Time - span));
                        }
                    }
                }
                queue.OrderBy(t => t.Item2.Ticks);
                return queue;
            }
        }
    }
}
