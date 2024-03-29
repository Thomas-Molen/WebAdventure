﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace textadventure_backend_entitymanager.Models.Responses
{
    public class LeaderboardResponse
    {
        public int Position { get; set; }
        public string User { get; set; }
        public string Adventurer { get; set; }
        public int Level { get; set; }
        public int Rooms { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
    }
}
