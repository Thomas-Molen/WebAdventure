﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace textadventure_backend.Models
{
    public class Adventurers : DefaultModel
    {
        public int Experience { get; set; } = 0;
        public int Health { get; set; } = 20;

        public string Name { get; set; } = "Adventurer";
        public int UserId { get; set; }
        public int DungeonId { get; set; }
        public int? RoomId { get; set; }

        [JsonIgnore]
        public virtual Users User { get; set; }
        [JsonIgnore]
        public virtual Dungeons Dungeon { get; set; }
        [JsonIgnore]
        public virtual Rooms Room { get; set; } = null;

        public virtual ICollection<AdventurerMaps> AdventurerMaps { get; set; }
        public virtual ICollection<Weapons> Weapons { get; set; }
        public virtual ICollection<Items> Items { get; set; }

        public Adventurers()
        {
            AdventurerMaps = new HashSet<AdventurerMaps>();
            Weapons = new HashSet<Weapons>();
            Items = new HashSet<Items>();
        }
    }
}
