﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace textadventure_backend.Models
{
    public class NPCs : DefaultModel
    {
        public string Conversation { get; set; }
        public int Risk { get; set; }
        public int WeaponId { get; set; }
        public int ItemId { get; set; }

        public virtual Weapons Weapon { get; set; }
        public virtual Items Item { get; set; }
        public virtual ICollection<Interactions> Interaction { get; set; }

        public NPCs()
        {

        }

        public NPCs(string conversation, int risk, Weapons weapon)
        {
            Conversation = conversation;
            Risk = risk;
            Weapon = weapon;
            WeaponId = weapon.Id;
        }

        public NPCs(string conversation, int risk, Items item)
        {
            Conversation = conversation;
            Risk = risk;
            Item = item;
            ItemId = item.Id;
        }
    }
}
