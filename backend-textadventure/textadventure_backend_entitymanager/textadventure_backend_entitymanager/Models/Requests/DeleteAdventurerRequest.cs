﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace textadventure_backend_entitymanager.Models.Requests
{
    public class DeleteAdventurerRequest
    {
        [Required]
        public int adventurerId { get; set; }
    }
}
