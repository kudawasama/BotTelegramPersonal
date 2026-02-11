namespace BotTelegram.RPG.Services
{
    using BotTelegram.RPG.Models;
    
    /// <summary>
    /// Base de datos de habilidades desbloqueables
    /// </summary>
    public static class SkillDatabase
    {
        /// <summary>
        /// Obtiene todas las habilidades disponibles
        /// </summary>
        public static List<RpgSkill> GetAllSkills()
        {
            return new List<RpgSkill>
            {
                // ═══════════════════════════════════════
                // COMBAT SKILLS
                // ═══════════════════════════════════════
                new RpgSkill
                {
                    Id = "skill_power_strike",
                    Name = "Golpe Poderoso",
                    Description = "Un ataque devastador que ignora parte de la defensa enemiga.",
                    Category = SkillCategory.Combat,
                    RequiredLevel = 5,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "physical_attack", Count = 50, Description = "Ataques físicos" },
                        new SkillRequirement { ActionType = "heavy_attack", Count = 20, Description = "Ataques pesados" }
                    },
                    ManaCost = 0,
                    StaminaCost = 30,
                    Cooldown = 3,
                    DamageMultiplier = 200,
                    IgnoresDefense = true,
                    DamageType = DamageType.Bludgeoning
                },
                new RpgSkill
                {
                    Id = "skill_whirlwind",
                    Name = "Torbellino",
                    Description = "Giras tu arma golpeando múltiples veces.",
                    Category = SkillCategory.Combat,
                    RequiredLevel = 8,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "physical_attack", Count = 100, Description = "Ataques físicos" },
                        new SkillRequirement { ActionType = "charge_attack", Count = 30, Description = "Envestidas" }
                    },
                    ManaCost = 0,
                    StaminaCost = 40,
                    Cooldown = 4,
                    DamageMultiplier = 80,
                    MultiHit = true,
                    HitCount = 4,
                    DamageType = DamageType.Slashing
                },
                new RpgSkill
                {
                    Id = "skill_assassinate",
                    Name = "Asesinar",
                    Description = "Un golpe preciso y letal dirigido a puntos vitales.",
                    Category = SkillCategory.Combat,
                    RequiredLevel = 10,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "precise_attack", Count = 40, Description = "Ataques precisos" },
                        new SkillRequirement { ActionType = "critical_hit", Count = 25, Description = "Golpes críticos" }
                    },
                    ManaCost = 0,
                    StaminaCost = 35,
                    Cooldown = 5,
                    DamageMultiplier = 250,
                    CanStun = true,
                    StunChance = 30,
                    DamageType = DamageType.Piercing
                },
                
                // ═══════════════════════════════════════
                // MAGIC SKILLS
                // ═══════════════════════════════════════
                new RpgSkill
                {
                    Id = "skill_fireball",
                    Name = "Bola de Fuego",
                    Description = "Lanzas una esfera de fuego explosiva.",
                    Category = SkillCategory.Magic,
                    RequiredLevel = 4,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "magic_attack", Count = 40, Description = "Ataques mágicos" }
                    },
                    ManaCost = 25,
                    StaminaCost = 0,
                    Cooldown = 2,
                    DamageMultiplier = 180,
                    DamageType = DamageType.Fire
                },
                new RpgSkill
                {
                    Id = "skill_lightning_bolt",
                    Name = "Rayo",
                    Description = "Invocas un rayo que ignora defensas.",
                    Category = SkillCategory.Magic,
                    RequiredLevel = 7,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "magic_attack", Count = 80, Description = "Ataques mágicos" },
                        new SkillRequirement { ActionType = "meditate", Count = 20, Description = "Meditaciones" }
                    },
                    ManaCost = 35,
                    StaminaCost = 0,
                    Cooldown = 3,
                    DamageMultiplier = 220,
                    IgnoresDefense = true,
                    DamageType = DamageType.Lightning
                },
                new RpgSkill
                {
                    Id = "skill_meteor_storm",
                    Name = "Tormenta de Meteoros",
                    Description = "Invocas meteoritos ardientes desde el cielo.",
                    Category = SkillCategory.Magic,
                    RequiredLevel = 15,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "magic_attack", Count = 200, Description = "Ataques mágicos" },
                        new SkillRequirement { ActionType = "skill_used", Count = 50, Description = "Habilidades usadas" }
                    },
                    ManaCost = 60,
                    StaminaCost = 0,
                    Cooldown = 6,
                    DamageMultiplier = 150,
                    MultiHit = true,
                    HitCount = 5,
                    DamageType = DamageType.Fire
                },
                
                // ═══════════════════════════════════════
                // DEFENSE SKILLS
                // ═══════════════════════════════════════
                new RpgSkill
                {
                    Id = "skill_iron_wall",
                    Name = "Muro de Hierro",
                    Description = "Te vuelves inmóvil pero casi invulnerable.",
                    Category = SkillCategory.Defense,
                    RequiredLevel = 6,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "block", Count = 50, Description = "Bloqueos" },
                        new SkillRequirement { ActionType = "damage_taken", Count = 500, Description = "Daño recibido" }
                    },
                    ManaCost = 0,
                    StaminaCost = 25,
                    Cooldown = 5,
                    BuffDuration = 3,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Defense", 50 },
                        { "MagicResistance", 30 }
                    }
                },
                new RpgSkill
                {
                    Id = "skill_riposte",
                    Name = "Respuesta",
                    Description = "Devuelves el daño recibido multiplicado.",
                    Category = SkillCategory.Defense,
                    RequiredLevel = 9,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "counter", Count = 30, Description = "Contraataques" },
                        new SkillRequirement { ActionType = "block", Count = 40, Description = "Bloqueos" }
                    },
                    ManaCost = 15,
                    StaminaCost = 20,
                    Cooldown = 4,
                    DamageMultiplier = 150,
                    DamageType = DamageType.Physical
                },
                
                // ═══════════════════════════════════════
                // MOVEMENT SKILLS
                // ═══════════════════════════════════════
                new RpgSkill
                {
                    Id = "skill_shadow_step",
                    Name = "Paso de Sombra",
                    Description = "Te mueves como las sombras, evadiendo todo.",
                    Category = SkillCategory.Movement,
                    RequiredLevel = 7,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "dodge", Count = 40, Description = "Esquivas" },
                        new SkillRequirement { ActionType = "retreat", Count = 20, Description = "Retrocesos" }
                    },
                    ManaCost = 20,
                    StaminaCost = 15,
                    Cooldown = 3,
                    BuffDuration = 2,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Evasion", 40 },
                        { "Dexterity", 15 }
                    }
                },
                new RpgSkill
                {
                    Id = "skill_blitz",
                    Name = "Blitz",
                    Description = "Te lanzas hacia el enemigo con velocidad sobrehumana.",
                    Category = SkillCategory.Movement,
                    RequiredLevel = 10,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "advance", Count = 30, Description = "Avances" },
                        new SkillRequirement { ActionType = "charge_attack", Count = 25, Description = "Envestidas" }
                    },
                    ManaCost = 0,
                    StaminaCost = 35,
                    Cooldown = 4,
                    DamageMultiplier = 170,
                    CanStun = true,
                    StunChance = 20,
                    DamageType = DamageType.Physical
                },
                
                // ═══════════════════════════════════════
                // SPECIAL SKILLS
                // ═══════════════════════════════════════
                new RpgSkill
                {
                    Id = "skill_battle_cry",
                    Name = "Grito de Guerra",
                    Description = "Inspiras terror en tus enemigos y vigor en ti mismo.",
                    Category = SkillCategory.Special,
                    RequiredLevel = 8,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "enemy_defeated", Count = 10, Description = "Enemigos derrotados" },
                        new SkillRequirement { ActionType = "combat_survived", Count = 15, Description = "Combates sobrevividos" }
                    },
                    ManaCost = 15,
                    StaminaCost = 15,
                    Cooldown = 5,
                    BuffDuration = 4,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Attack", 20 },
                        { "Defense", 15 },
                        { "CritChance", 10 }
                    }
                },
                new RpgSkill
                {
                    Id = "skill_life_drain",
                    Name = "Drenaje de Vida",
                    Description = "Absorbes la vida del enemigo.",
                    Category = SkillCategory.Special,
                    RequiredLevel = 12,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "magic_attack", Count = 100, Description = "Ataques mágicos" },
                        new SkillRequirement { ActionType = "low_hp_combat", Count = 20, Description = "Combates con HP baja" }
                    },
                    ManaCost = 30,
                    StaminaCost = 0,
                    Cooldown = 5,
                    DamageMultiplier = 140,
                    HealAmount = 0, // Se calcula como % del daño hecho
                    DamageType = DamageType.Dark
                },
                new RpgSkill
                {
                    Id = "skill_divine_protection",
                    Name = "Protección Divina",
                    Description = "Invocas protección celestial.",
                    Category = SkillCategory.Special,
                    RequiredLevel = 14,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "wait", Count = 30, Description = "Esperas" },
                        new SkillRequirement { ActionType = "observe", Count = 25, Description = "Observaciones" },
                        new SkillRequirement { ActionType = "meditate", Count = 40, Description = "Meditaciones" }
                    },
                    ManaCost = 50,
                    StaminaCost = 0,
                    Cooldown = 7,
                    HealAmount = 80,
                    BuffDuration = 3,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Defense", 30 },
                        { "MagicResistance", 30 },
                        { "HP", 50 }
                    }
                },
                new RpgSkill
                {
                    Id = "skill_berserker_rage",
                    Name = "Furia Berserker",
                    Description = "Sacrificas defensa por poder destructivo.",
                    Category = SkillCategory.Special,
                    RequiredLevel = 16,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "heavy_attack", Count = 50, Description = "Ataques pesados" },
                        new SkillRequirement { ActionType = "low_hp_combat", Count = 30, Description = "Combates con HP baja" },
                        new SkillRequirement { ActionType = "damage_dealt", Count = 2000, Description = "Daño total infligido" }
                    },
                    ManaCost = 0,
                    StaminaCost = 50,
                    Cooldown = 6,
                    BuffDuration = 5,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Attack", 50 },
                        { "CritChance", 25 },
                        { "CritDamage", 50 },
                        { "Defense", -20 } // Penalización
                    }
                },
                new RpgSkill
                {
                    Id = "skill_time_warp",
                    Name = "Distorsión Temporal",
                    Description = "Manipulas el tiempo para actuar dos veces.",
                    Category = SkillCategory.Special,
                    RequiredLevel = 20,
                    Requirements = new List<SkillRequirement>
                    {
                        new SkillRequirement { ActionType = "skill_used", Count = 100, Description = "Habilidades usadas" },
                        new SkillRequirement { ActionType = "perfect_dodge", Count = 30, Description = "Esquivas perfectas" },
                        new SkillRequirement { ActionType = "combo_5plus", Count = 20, Description = "Combos de 5+" }
                    },
                    ManaCost = 80,
                    StaminaCost = 40,
                    Cooldown = 10,
                    BuffDuration = 1,
                    StatBuffs = new Dictionary<string, int>
                    {
                        { "Dexterity", 50 },
                        { "Evasion", 30 }
                    }
                }
            };
        }
        
        /// <summary>
        /// Obtiene habilidad por ID
        /// </summary>
        public static RpgSkill? GetById(string id)
        {
            return GetAllSkills().FirstOrDefault(s => s.Id == id);
        }
        
        /// <summary>
        /// Obtiene habilidades desbloqueables para un jugador
        /// </summary>
        public static List<RpgSkill> GetUnlockableSkills(RpgPlayer player)
        {
            var all = GetAllSkills();
            var unlocked = player.UnlockedSkills;
            
            // Retornar solo las que cumplen requisitos y no están desbloqueadas
            return all.Where(s => !unlocked.Contains(s.Id) && s.MeetsRequirements(player)).ToList();
        }
        
        /// <summary>
        /// Obtiene habilidades próximas a desbloquear (falta poco)
        /// </summary>
        public static List<RpgSkill> GetNearlyUnlockableSkills(RpgPlayer player, int threshold = 10)
        {
            var all = GetAllSkills();
            var unlocked = player.UnlockedSkills;
            
            return all.Where(s =>
            {
                if (unlocked.Contains(s.Id) || player.Level < s.RequiredLevel)
                    return false;
                
                // Verificar si está cerca de cumplir requisitos
                foreach (var req in s.Requirements)
                {
                    var current = player.ActionCounters.ContainsKey(req.ActionType)
                        ? player.ActionCounters[req.ActionType]
                        : 0;
                    
                    // Si falta menos del threshold, está cerca
                    if (req.Count - current <= threshold && req.Count - current > 0)
                        return true;
                }
                
                return false;
            }).ToList();
        }
        
        /// <summary>
        /// Intenta desbloquear habilidades automáticamente
        /// </summary>
        public static List<RpgSkill> CheckAndUnlockSkills(RpgPlayer player)
        {
            var newlyUnlocked = new List<RpgSkill>();
            var unlockable = GetUnlockableSkills(player);
            
            foreach (var skill in unlockable)
            {
                if (!player.UnlockedSkills.Contains(skill.Id))
                {
                    player.UnlockedSkills.Add(skill.Id);
                    newlyUnlocked.Add(skill);
                }
            }
            
            return newlyUnlocked;
        }
    }
}
