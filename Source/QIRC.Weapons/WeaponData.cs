/** 
 * .NET Bot for Internet Relay Chat (IRC)
 * Copyright (c) ThomasKerman 2016
 * QIRC is licensed under the MIT License
 */

using System;
using QIRC.Serialization;
using SQLite;

namespace QIRC.Commands
{
    /// <summary>
    /// Contains all Weapons and Adjectives
    /// </summary>
    public class WeaponData : Storage<WeaponData>
    {
        [PrimaryKey, NotNull, Unique, AutoIncrement]
        public Int32 Index { get; set; }

        public String Content { get; set; }
        public Boolean wpn { get; set; }

        public WeaponData() { }
    }
}