/** 
 * .NET Bot for Internet Relay Chat (IRC)
 * Copyright (c) Dorian Stoll 2017
 * QIRC is licensed under the MIT License
 */

using System;
using QIRC.Serialization;
using SQLite;

namespace QIRC.Kountdown
{
    /// <summary>
    /// Contains all data for one kountdown subscriber
    /// </summary>
    public class SubscriberData : Storage<SubscriberData>
    {
        [PrimaryKey, Unique, NotNull]
        public String Name { get; set; }

        public SubscriberData() { }

        public SubscriberData(String name) { Name = name; }
    }
}