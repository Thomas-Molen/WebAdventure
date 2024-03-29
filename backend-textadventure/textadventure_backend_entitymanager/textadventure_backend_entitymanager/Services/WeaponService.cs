﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using textadventure_backend_entitymanager.Context;
using textadventure_backend_entitymanager.Models.Entities;

namespace textadventure_backend_entitymanager.Services
{
    public interface IWeaponService
    {
        Task EquipWeapon(int adventurerId, int weaponId);
        Task<Weapons> GenerateWeapon(int adventurerId);
        Task<List<Weapons>> GetAllWeapons(int adventurerId);
    }

    public class WeaponService : IWeaponService
    {
        private readonly IDbContextFactory<TextadventureDBContext> contextFactory;

        public WeaponService(IDbContextFactory<TextadventureDBContext> _contextFactory)
        {
            contextFactory = _contextFactory;
        }

        public async Task<Weapons> GenerateWeapon(int adventurerId)
        {
            using (var db = contextFactory.CreateDbContext())
            {
                var adventurer = await db.Adventurers
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(a => a.Id == adventurerId);

                if (adventurer == null)
                {
                    throw new ArgumentException("No User found with given adventurerId");
                }

                var weapon = new Weapons(adventurer.Experience);
                weapon.AdventurerId = adventurerId;

                await db.AddAsync(weapon);
                await db.SaveChangesAsync();

                return weapon;
            }
        }

        public async Task<List<Weapons>> GetAllWeapons(int adventurerId)
        {
            using (var db = contextFactory.CreateDbContext())
            {
                var adventurer = await db.Adventurers
                    .OrderByDescending(x => x.Id)
                    .Include(a => a.Weapons
                    .Where(w => w.Durability > 0 || w.Equiped))
                    .FirstOrDefaultAsync(a => a.Id == adventurerId);

                if (adventurer == null)
                {
                    throw new ArgumentException("No User found with given adventurerId");
                }

                return adventurer.Weapons.ToList();
            }
        }

        public async Task EquipWeapon(int adventurerId, int weaponId)
        {
            using (var db = contextFactory.CreateDbContext())
            {
                var adventurer = await db.Adventurers
                    .OrderByDescending(x => x.Id)
                    .Include(a => a.Weapons)
                    .FirstOrDefaultAsync(a => a.Id == adventurerId);

                if (adventurer == null)
                {
                    throw new ArgumentException("No User found with given adventurerId");
                }

                var currentlyEquipedWeapon = adventurer.Weapons.ToList().Find(w => w.Equiped);
                var weaponToEquip = adventurer.Weapons.ToList().Find(w => w.Id == weaponId);
                if (currentlyEquipedWeapon != null)
                {
                    currentlyEquipedWeapon.Equiped = false;
                }

                if (weaponToEquip == null)
                {
                    throw new ArgumentException("No Weapon found with given weaponId");
                }
                weaponToEquip.Equiped = true;

                db.Update(adventurer);
                await db.SaveChangesAsync();
            }
        }
    }
}
