namespace BotTelegram.RPG.Services
{
    using BotTelegram.RPG.Models;
    
    /// <summary>
    /// Base de datos de equipamiento balanceado
    /// </summary>
    public static class EquipmentDatabase
    {
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Obtiene todas las armas disponibles
        /// </summary>
        public static List<RpgEquipment> GetWeapons()
        {
            return new List<RpgEquipment>
            {
                // ═══════════════════════════════════════
                // ARMAS COMUNES (Nivel 1-5)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "weapon_rusty_sword",
                    Name = "Espada Oxidada",
                    Description = "Una espada desgastada por el tiempo.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusStrength = 2,
                    BonusAttack = 8,
                    Price = 50
                },
                new RpgEquipment
                {
                    Id = "weapon_wooden_staff",
                    Name = "Bastón de Madera",
                    Description = "Un simple bastón tallado.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusIntelligence = 2,
                    BonusMagicPower = 10,
                    Price = 50
                },
                new RpgEquipment
                {
                    Id = "weapon_iron_dagger",
                    Name = "Daga de Hierro",
                    Description = "Pequeña pero mortal.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusDexterity = 3,
                    BonusAttack = 6,
                    BonusCritChance = 5,
                    Price = 60
                },
                
                // ═══════════════════════════════════════
                // ARMAS POCO COMUNES (Nivel 5-10)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "weapon_steel_sword",
                    Name = "Espada de Acero",
                    Description = "Bien forjada y balanceada.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 5,
                    BonusStrength = 5,
                    BonusAttack = 18,
                    BonusAccuracy = 3,
                    Price = 250
                },
                new RpgEquipment
                {
                    Id = "weapon_apprentice_wand",
                    Name = "Varita de Aprendiz",
                    Description = "Canaliza la energía mágica eficientemente.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 5,
                    BonusIntelligence = 6,
                    BonusMagicPower = 22,
                    BonusMana = 20,
                    Price = 270
                },
                
                // ═══════════════════════════════════════
                // ARMAS RARAS (Nivel 10-15)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "weapon_flameblade",
                    Name = "Hoja de Llamas",
                    Description = "Envuelta en fuego eterno.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 10,
                    BonusStrength = 8,
                    BonusIntelligence = 4,
                    BonusAttack = 30,
                    BonusMagicPower = 15,
                    Price = 800
                },
                new RpgEquipment
                {
                    Id = "weapon_shadow_daggers",
                    Name = "Dagas de las Sombras",
                    Description = "Se mueven como la oscuridad misma.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 12,
                    BonusDexterity = 12,
                    BonusAttack = 25,
                    BonusCritChance = 15,
                    BonusCritDamage = 25,
                    BonusEvasion = 8,
                    Price = 900
                },
                
                // ═══════════════════════════════════════
                // ARMAS ÉPICAS (Nivel 15-20)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "weapon_dragonslayer",
                    Name = "Matadragones",
                    Description = "Forjada con escamas de dragón.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Epic,
                    RequiredLevel = 18,
                    BonusStrength = 15,
                    BonusConstitution = 8,
                    BonusAttack = 50,
                    BonusDefense = 10,
                    BonusHP = 50,
                    Price = 2500
                },
                new RpgEquipment
                {
                    Id = "weapon_staff_of_archmage",
                    Name = "Báculo del Archimago",
                    Description = "Resonante con poder ancestral.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Epic,
                    RequiredLevel = 20,
                    BonusIntelligence = 18,
                    BonusWisdom = 10,
                    BonusMagicPower = 60,
                    BonusMana = 80,
                    BonusMagicResistance = 15,
                    Price = 3000
                },
                
                // ═══════════════════════════════════════
                // ARMAS LEGENDARIAS (Nivel 25+)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "weapon_excalibur",
                    Name = "Excalibur",
                    Description = "La espada de los reyes legendarios.",
                    Type = EquipmentType.Weapon,
                    Rarity = EquipmentRarity.Legendary,
                    RequiredLevel = 30,
                    BonusStrength = 25,
                    BonusCharisma = 15,
                    BonusAttack = 100,
                    BonusAccuracy = 20,
                    BonusCritChance = 20,
                    BonusHP = 100,
                    Price = 10000
                }
            };
        }
        
        /// <summary>
        /// Obtiene todas las armaduras disponibles
        /// </summary>
        public static List<RpgEquipment> GetArmors()
        {
            return new List<RpgEquipment>
            {
                // ═══════════════════════════════════════
                // ARMADURAS COMUNES (Nivel 1-5)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "armor_leather_tunic",
                    Name = "Túnica de Cuero",
                    Description = "Protección básica ligera.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusConstitution = 2,
                    BonusDefense = 5,
                    BonusHP = 15,
                    Price = 40
                },
                new RpgEquipment
                {
                    Id = "armor_cloth_robe",
                    Name = "Túnica de Tela",
                    Description = "Cómoda y mágicamente receptiva.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusWisdom = 2,
                    BonusMagicResistance = 4,
                    BonusMana = 15,
                    Price = 40
                },
                
                // ═══════════════════════════════════════
                // ARMADURAS POCO COMUNES (Nivel 5-10)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "armor_chainmail",
                    Name = "Cota de Malla",
                    Description = "Anillos entrelazados de acero.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 5,
                    BonusConstitution = 5,
                    BonusDefense = 15,
                    BonusHP = 40,
                    Price = 200
                },
                new RpgEquipment
                {
                    Id = "armor_enchanted_robe",
                    Name = "Túnica Encantada",
                    Description = "Tejida con hilos mágicos.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 6,
                    BonusIntelligence = 4,
                    BonusWisdom = 4,
                    BonusMagicResistance = 12,
                    BonusMana = 45,
                    Price = 230
                },
                
                // ═══════════════════════════════════════
                // ARMADURAS RARAS (Nivel 10-15)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "armor_plate_armor",
                    Name = "Armadura de Placas",
                    Description = "Protección completa de acero macizo.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 12,
                    BonusStrength = 5,
                    BonusConstitution = 12,
                    BonusDefense = 35,
                    BonusHP = 80,
                    Price = 750
                },
                new RpgEquipment
                {
                    Id = "armor_robe_of_magi",
                    Name = "Túnica de los Magos",
                    Description = "Usada por maestros de las artes arcanas.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 13,
                    BonusIntelligence = 10,
                    BonusWisdom = 8,
                    BonusMagicPower = 20,
                    BonusMagicResistance = 25,
                    BonusMana = 70,
                    Price = 800
                },
                
                // ═══════════════════════════════════════
                // ARMADURAS ÉPICAS (Nivel 15-20)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "armor_dragon_scale",
                    Name = "Armadura de Escamas de Dragón",
                    Description = "Impenetrable como la piel de un dragón.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Epic,
                    RequiredLevel = 20,
                    BonusStrength = 10,
                    BonusConstitution = 20,
                    BonusDefense = 60,
                    BonusMagicResistance = 30,
                    BonusHP = 150,
                    Price = 3500
                },
                
                // ═══════════════════════════════════════
                // ARMADURAS LEGENDARIAS (Nivel 25+)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "armor_celestial_aegis",
                    Name = "Égida Celestial",
                    Description = "Forjada en los cielos mismos.",
                    Type = EquipmentType.Armor,
                    Rarity = EquipmentRarity.Legendary,
                    RequiredLevel = 28,
                    BonusConstitution = 30,
                    BonusWisdom = 20,
                    BonusDefense = 100,
                    BonusMagicResistance = 80,
                    BonusHP = 250,
                    BonusMana = 100,
                    Price = 12000
                }
            };
        }
        
        /// <summary>
        /// Obtiene todos los accesorios disponibles
        /// </summary>
        public static List<RpgEquipment> GetAccessories()
        {
            return new List<RpgEquipment>
            {
                // ═══════════════════════════════════════
                // ACCESORIOS COMUNES (Nivel 1-5)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "accessory_bronze_ring",
                    Name = "Anillo de Bronce",
                    Description = "Un simple anillo decorativo.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusStrength = 1,
                    BonusAttack = 3,
                    Price = 30
                },
                new RpgEquipment
                {
                    Id = "accessory_leather_gloves",
                    Name = "Guantes de Cuero",
                    Description = "Mejoran el agarre.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Common,
                    RequiredLevel = 1,
                    BonusDexterity = 2,
                    BonusAccuracy = 2,
                    Price = 35
                },
                
                // ═══════════════════════════════════════
                // ACCESORIOS POCO COMUNES (Nivel 5-10)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "accessory_silver_amulet",
                    Name = "Amuleto de Plata",
                    Description = "Protege contra la magia oscura.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 5,
                    BonusWisdom = 4,
                    BonusMagicResistance = 8,
                    BonusMana = 25,
                    Price = 180
                },
                new RpgEquipment
                {
                    Id = "accessory_belt_of_strength",
                    Name = "Cinturón de Fuerza",
                    Description = "Otorga poder físico.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Uncommon,
                    RequiredLevel = 6,
                    BonusStrength = 5,
                    BonusConstitution = 3,
                    BonusHP = 30,
                    BonusStamina = 15,
                    Price = 200
                },
                
                // ═══════════════════════════════════════
                // ACCESORIOS RAROS (Nivel 10-15)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "accessory_ring_of_haste",
                    Name = "Anillo de Celeridad",
                    Description = "Acelera los reflejos.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 10,
                    BonusDexterity = 10,
                    BonusEvasion = 12,
                    BonusAccuracy = 8,
                    BonusCritChance = 8,
                    Price = 650
                },
                new RpgEquipment
                {
                    Id = "accessory_mana_crystal",
                    Name = "Cristal de Maná",
                    Description = "Almacena energía mágica pura.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Rare,
                    RequiredLevel = 12,
                    BonusIntelligence = 8,
                    BonusWisdom = 6,
                    BonusMagicPower = 15,
                    BonusMana = 100,
                    Price = 700
                },
                
                // ═══════════════════════════════════════
                // ACCESORIOS ÉPICOS (Nivel 15-20)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "accessory_cloak_of_shadows",
                    Name = "Capa de las Sombras",
                    Description = "Dificulta ser detectado.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Epic,
                    RequiredLevel = 18,
                    BonusDexterity = 15,
                    BonusEvasion = 25,
                    BonusCritChance = 15,
                    BonusCritDamage = 30,
                    Price = 2800
                },
                new RpgEquipment
                {
                    Id = "accessory_dragon_heart",
                    Name = "Corazón de Dragón",
                    Description = "Late con poder dracónico.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Epic,
                    RequiredLevel = 20,
                    BonusStrength = 12,
                    BonusIntelligence = 12,
                    BonusConstitution = 12,
                    BonusHP = 100,
                    BonusMana = 100,
                    BonusStamina = 50,
                    Price = 3200
                },
                
                // ═══════════════════════════════════════
                // ACCESORIOS LEGENDARIOS (Nivel 25+)
                // ═══════════════════════════════════════
                new RpgEquipment
                {
                    Id = "accessory_crown_of_gods",
                    Name = "Corona de los Dioses",
                    Description = "Otorga poder divino.",
                    Type = EquipmentType.Accessory,
                    Rarity = EquipmentRarity.Legendary,
                    RequiredLevel = 30,
                    BonusStrength = 15,
                    BonusIntelligence = 15,
                    BonusDexterity = 15,
                    BonusConstitution = 15,
                    BonusWisdom = 15,
                    BonusCharisma = 20,
                    BonusAttack = 30,
                    BonusMagicPower = 30,
                    BonusDefense = 30,
                    BonusMagicResistance = 30,
                    Price = 15000
                }
            };
        }
        
        /// <summary>
        /// Obtiene todo el equipamiento disponible
        /// </summary>
        public static List<RpgEquipment> GetAllEquipment()
        {
            var all = new List<RpgEquipment>();
            all.AddRange(GetWeapons());
            all.AddRange(GetArmors());
            all.AddRange(GetAccessories());
            return all;
        }
        
        /// <summary>
        /// Obtiene equipment por ID
        /// </summary>
        public static RpgEquipment? GetById(string id)
        {
            return GetAllEquipment().FirstOrDefault(e => e.Id == id);
        }
        
        /// <summary>
        /// Obtiene equipment aleatorio por nivel del jugador
        /// </summary>
        public static RpgEquipment GetRandomByLevel(int playerLevel, EquipmentType? type = null)
        {
            var allEquipment = GetAllEquipment();
            
            // Filtrar por tipo si se especifica
            if (type.HasValue)
            {
                allEquipment = allEquipment.Where(e => e.Type == type.Value).ToList();
            }
            
            // Filtrar por nivel (±3 niveles del jugador)
            var suitable = allEquipment.Where(e => 
                e.RequiredLevel <= playerLevel + 3 && 
                e.RequiredLevel >= Math.Max(1, playerLevel - 3)
            ).ToList();
            
            if (suitable.Count == 0)
            {
                // Fallback: cualquier equipment que el jugador pueda usar
                suitable = allEquipment.Where(e => e.RequiredLevel <= playerLevel).ToList();
            }
            
            if (suitable.Count == 0)
            {
                // Último fallback: el equipo de nivel 1
                suitable = allEquipment.Where(e => e.RequiredLevel == 1).ToList();
            }
            
            return suitable[_random.Next(suitable.Count)].Clone();
        }
        
        /// <summary>
        /// Genera loot random para un enemigo derrotado
        /// </summary>
        public static RpgEquipment? GenerateLoot(int playerLevel, double dropChance = 0.15)
        {
            // 15% chance base de drop
            if (_random.NextDouble() > dropChance)
                return null;
            
            // Determinar rareza (más difícil conseguir items raros)
            var roll = _random.NextDouble();
            EquipmentRarity rarity;
            
            if (roll < 0.50) rarity = EquipmentRarity.Common;       // 50%
            else if (roll < 0.75) rarity = EquipmentRarity.Uncommon; // 25%
            else if (roll < 0.90) rarity = EquipmentRarity.Rare;     // 15%
            else if (roll < 0.98) rarity = EquipmentRarity.Epic;     // 8%
            else rarity = EquipmentRarity.Legendary;                 // 2%
            
            // Ajustar por nivel (no dropear legendarios a nivel 1)
            if (rarity == EquipmentRarity.Legendary && playerLevel < 25)
                rarity = EquipmentRarity.Epic;
            if (rarity == EquipmentRarity.Epic && playerLevel < 15)
                rarity = EquipmentRarity.Rare;
            
            // Filtrar por rareza y nivel
            var candidates = GetAllEquipment()
                .Where(e => e.Rarity == rarity && e.RequiredLevel <= playerLevel + 5)
                .ToList();
            
            if (candidates.Count == 0)
                return null;
            
            return candidates[_random.Next(candidates.Count)].Clone();
        }
    }
}
