using System.Text.Json.Serialization;

namespace BotTelegram.RPG.Models
{
    public class RpgPlayer
    {
        public long ChatId { get; set; }
        public string Name { get; set; } = "";
        public CharacterClass Class { get; set; } = CharacterClass.Warrior;
        
        // Stats
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        
        // ═══════════════════════════════════════
        // STATS PRIMARIOS (Fijos - crecen con nivel)
        // ═══════════════════════════════════════
        public int Strength { get; set; } = 10;       // Daño físico, carga
        public int Intelligence { get; set; } = 10;   // Daño mágico, mana
        public int Dexterity { get; set; } = 10;      // Críticos, evasión, precisión
        public int Constitution { get; set; } = 10;   // HP, defensa física
        public int Wisdom { get; set; } = 10;         // Resistencia mágica, regen mana
        public int Charisma { get; set; } = 10;       // Críticos mejorados, comercio
        
        // ═══════════════════════════════════════
        // STATS VARIABLES (Recursos)
        // ═══════════════════════════════════════
        public int HP { get; set; } = 100;
        public int MaxHP { get; set; } = 100;
        
        public int Mana { get; set; } = 0;
        public int MaxMana { get; set; } = 0;
        
        public int Stamina { get; set; } = 50;        // Para habilidades físicas
        public int MaxStamina { get; set; } = 50;
        
        // Resources
        public int Gold { get; set; } = 50;
        public List<RpgItem> Inventory { get; set; } = new();
        
        // Legacy equipment (mantener RpgItem por compatibilidad)
        public RpgItem? EquippedWeapon { get; set; }
        public RpgItem? EquippedArmor { get; set; }
        public RpgItem? EquippedAccessory { get; set; }
        
        // ═══════════════════════════════════════
        // EQUIPMENT SYSTEM (Nuevo)
        // ═══════════════════════════════════════
        public RpgEquipment? EquippedWeaponNew { get; set; }
        public RpgEquipment? EquippedArmorNew { get; set; }
        public RpgEquipment? EquippedAccessoryNew { get; set; }
        public List<RpgEquipment> EquipmentInventory { get; set; } = new();
        
        // ═══════════════════════════════════════
        // ACTION COUNTERS (Sistema de progreso)
        // ═══════════════════════════════════════
        public Dictionary<string, int> ActionCounters { get; set; } = new();
        
        // ═══════════════════════════════════════
        // SKILLS SYSTEM (Habilidades desbloqueables)
        // ═══════════════════════════════════════
        public List<string> UnlockedSkills { get; set; } = new(); // IDs de skills desbloqueadas
        public Dictionary<string, int> SkillCooldowns { get; set; } = new(); // Cooldowns activos
        
        // Legacy skills (deprecado - usar UnlockedSkills)
        public List<RpgSkill> Skills { get; set; } = new();
        
        // ═══════════════════════════════════════
        // PASSIVES SYSTEM (Habilidades pasivas permanentes)
        // ═══════════════════════════════════════
        public List<string> UnlockedPassives { get; set; } = new(); // IDs de pasivas desbloqueadas
        
        // ═══════════════════════════════════════
        // HIDDEN CLASSES (Clases ocultas desbloqueables)
        // ═══════════════════════════════════════
        public List<string> UnlockedHiddenClasses { get; set; } = new(); // IDs de clases ocultas desbloqueadas
        public string? ActiveHiddenClass { get; set; } // Clase oculta activa (opcional, puede no usar ninguna)
        
        // ═══════════════════════════════════════
        // PET SYSTEM (Sistema de Mascotas)
        // ═══════════════════════════════════════
        public List<RpgPet> ActivePets { get; set; } = new(); // Mascotas en combate (máx 1-3 según clase)
        public List<RpgPet> PetInventory { get; set; } = new(); // Todas las mascotas domadas
        
        [JsonIgnore]
        public int MaxActivePets => ActiveHiddenClass switch
        {
            "beast_tamer" => 1,
            "beast_lord" => 3,
            "necromancer_lord" => 3, // Para minions no-muertos
            "lich_king" => 5,
            _ => 1
        };
        
        // ═══════════════════════════════════════
        // MINION SYSTEM (Sistema de Invocaciones)
        // ═══════════════════════════════════════
        public List<Minion> ActiveMinions { get; set; } = new(); // Invocaciones activas en combate
        
        [JsonIgnore]
        public int MaxActiveMinions => ActiveHiddenClass switch
        {
            "necromancer_lord" => 3,
            "lich_king" => 5,
            "elemental_overlord" => 4,
            "void_summoner" => 2,
            _ => 1 // Por defecto todos pueden invocar 1
        };
        
        // ═══════════════════════════════════════
        // LOCATION SYSTEM (Sistema de Zonas)
        // ═══════════════════════════════════════
        public string CurrentZone { get; set; } = "puerto_esperanza"; // ID de la zona actual
        public List<string> UnlockedZones { get; set; } = new() { "puerto_esperanza" }; // Zonas desbloqueadas
        
        // ═══════════════════════════════════════
        // DUNGEON SYSTEM (FASE 3)
        // ═══════════════════════════════════════
        public Dungeon? CurrentDungeon { get; set; } // Mazmorra activa (null si no está en mazmorra)
        public List<DungeonKey> DungeonKeys { get; set; } = new(); // Llaves de mazmorra disponibles
        public Dictionary<string, int> DungeonsCompleted { get; set; } = new(); // dungeonId -> veces completado
        public int TotalDungeonsCompleted { get; set; } = 0;
        public int TotalDungeonFloorsCleaned { get; set; } = 0;
        
        // Progress
        public string CurrentLocation { get; set; } = "Taberna de Puerto Esperanza";
        public DateTime LastActionTime { get; set; } = DateTime.UtcNow;
        
        // Combat
        public bool IsInCombat { get; set; } = false;
        public RpgEnemy? CurrentEnemy { get; set; }
        
        // Fase 5.2: Single Message Interaction - MessageId del mensaje de combate actual
        public int? ActiveCombatMessageId { get; set; }
        
        // Combat avanzado
        public int ComboCount { get; set; } = 0; // Ataques consecutivos exitosos
        public int CombatTurnCount { get; set; } = 0; // Turno actual del combate
        public List<StatusEffect> StatusEffects { get; set; } = new();
        
        [JsonIgnore]
        public List<CombatLogEntry> CombatLog { get; set; } = new(); // No persistir en JSON
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
        
        // Stats avanzados
        public int TotalKills { get; set; } = 0;
        public int TotalDeaths { get; set; } = 0;
        public int TotalGoldEarned { get; set; } = 0;
        public int BossKills { get; set; } = 0;
        public long TotalDamageDealt { get; set; } = 0;
        public string? Username { get; set; } // Telegram username para leaderboard
        
        // ═══════════════════════════════════════
        // STATS DERIVADOS (Calculados)
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Ataque Físico = Base STR + Equipment Bonuses + Class Bonuses
        /// </summary>
        [JsonIgnore]
        public int PhysicalAttack => GetPhysicalAttack();
        
        /// <summary>
        /// STR Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveStrength => Strength + 
            (EquippedWeaponNew?.BonusStrength ?? 0) + 
            (EquippedArmorNew?.BonusStrength ?? 0) + 
            (EquippedAccessoryNew?.BonusStrength ?? 0);
        
        /// <summary>
        /// INT Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveIntelligence => Intelligence + 
            (EquippedWeaponNew?.BonusIntelligence ?? 0) + 
            (EquippedArmorNew?.BonusIntelligence ?? 0) + 
            (EquippedAccessoryNew?.BonusIntelligence ?? 0);
        
        /// <summary>
        /// DEX Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveDexterity => Dexterity + 
            (EquippedWeaponNew?.BonusDexterity ?? 0) + 
            (EquippedArmorNew?.BonusDexterity ?? 0) + 
            (EquippedAccessoryNew?.BonusDexterity ?? 0);
        
        /// <summary>
        /// CON Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveConstitution => Constitution + 
            (EquippedWeaponNew?.BonusConstitution ?? 0) + 
            (EquippedArmorNew?.BonusConstitution ?? 0) + 
            (EquippedAccessoryNew?.BonusConstitution ?? 0);
        
        /// <summary>
        /// WIS Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveWisdom => Wisdom + 
            (EquippedWeaponNew?.BonusWisdom ?? 0) + 
            (EquippedArmorNew?.BonusWisdom ?? 0) + 
            (EquippedAccessoryNew?.BonusWisdom ?? 0);
        
        /// <summary>
        /// CHA Activo = Base + Equipment
        /// </summary>
        [JsonIgnore]
        public int ActiveCharisma => Charisma + 
            (EquippedWeaponNew?.BonusCharisma ?? 0) + 
            (EquippedArmorNew?.BonusCharisma ?? 0) + 
            (EquippedAccessoryNew?.BonusCharisma ?? 0);
        
        /// <summary>
        /// Ataque Mágico = INT + (INT * bonuses de clase) + arma mágica
        /// </summary>
        [JsonIgnore]
        public int MagicalAttack => GetMagicalAttack();
        
        /// <summary>
        /// Defensa Física = CON + DEX/2 + armadura (reduce daño físico)
        /// </summary>
        [JsonIgnore]
        public int PhysicalDefense => GetPhysicalDefense();
        
        /// <summary>
        /// Resistencia Mágica = WIS + INT/3 + accesorios (reduce daño mágico)
        /// </summary>
        [JsonIgnore]
        public int MagicResistance => GetMagicResistance();
        
        /// <summary>
        /// Probabilidad de crítico = DEX/2 + CHA/3 + bonuses (0-100%)
        /// </summary>
        [JsonIgnore]
        public double CriticalChance => GetCriticalChance();
        
        /// <summary>
        /// Probabilidad de evasión = DEX + Level/2 (0-100%)
        /// </summary>
        [JsonIgnore]
        public double Evasion => GetEvasion();
        
        /// <summary>
        /// Precisión = DEX + Level (aumenta hit chance)
        /// </summary>
        [JsonIgnore]
        public int Accuracy => Dexterity + Level;
        
        // Legacy properties (compatibilidad)
        [JsonIgnore]
        public int TotalAttack => PhysicalAttack;
        
        [JsonIgnore]
        public int TotalDefense => PhysicalDefense;
        
        [JsonIgnore]
        public int TotalMagicPower => MagicalAttack;
        
        [JsonIgnore]
        public int Energy
        {
            get => Stamina;
            set => Stamina = value;
        }
        
        [JsonIgnore]
        public int MaxEnergy
        {
            get => MaxStamina;
            set => MaxStamina = value;
        }
        
        [JsonIgnore]
        public int XPNeeded => (int)(100 * Math.Pow(1.15, Level - 1)); // Fórmula exponencial para progresión más lenta
        
        [JsonIgnore]
        public string ClassEmoji => Class switch
        {
            CharacterClass.Warrior => "⚔️",
            CharacterClass.Mage => "🔮",
            CharacterClass.Rogue => "🗡️",
            CharacterClass.Cleric => "✨",
            CharacterClass.Paladin => "🛡️",
            CharacterClass.Ranger => "🏹",
            CharacterClass.Warlock => "🔥",
            CharacterClass.Monk => "🥋",
            CharacterClass.Bard => "🎵",
            CharacterClass.Druid => "🌿",
            CharacterClass.Necromancer => "💀",
            CharacterClass.Assassin => "🗡️",
            CharacterClass.Berserker => "⚡",
            CharacterClass.Sorcerer => "🌟",
            _ => "👤"
        };
        
        // ═══════════════════════════════════════
        // MÉTODOS DE CÁLCULO DE STATS
        // ═══════════════════════════════════════
        
        private int GetPhysicalAttack()
        {
            // Usar STR activo (con bonos de equipo)
            var baseAttack = ActiveStrength;
            
            // Scaling por clase (multiplicador de STR)
            var classMultiplier = Class switch
            {
                CharacterClass.Warrior => 1.5,
                CharacterClass.Berserker => 1.8,
                CharacterClass.Paladin => 1.3,
                CharacterClass.Rogue => 1.2,
                CharacterClass.Assassin => 1.4,
                CharacterClass.Monk => 1.2,
                CharacterClass.Ranger => 1.1,
                _ => 1.0
            };
            
            baseAttack = (int)(baseAttack * classMultiplier);
            
            // Bonus de nivel (reducido para aumentar dificultad)
            baseAttack += Level / 3;
            
            // Bonus directo de arma (nuevo sistema prioritario, fallback a legacy)
            baseAttack += (EquippedWeaponNew?.BonusAttack ?? EquippedWeapon?.AttackBonus ?? 0);
            
            return baseAttack;
        }
        
        private int GetMagicalAttack()
        {
            // Usar INT activo (con bonos de equipo)
            var baseAttack = ActiveIntelligence;
            
            // Scaling por clase (multiplicador de INT)
            var classMultiplier = Class switch
            {
                CharacterClass.Mage => 1.8,
                CharacterClass.Sorcerer => 2.0,
                CharacterClass.Warlock => 1.6,
                CharacterClass.Cleric => 1.3,
                CharacterClass.Druid => 1.4,
                CharacterClass.Necromancer => 1.7,
                CharacterClass.Bard => 1.2,
                _ => 1.0
            };
            
            baseAttack = (int)(baseAttack * classMultiplier);
            
            // Bonus de nivel (reducido para aumentar dificultad)
            baseAttack += Level / 3;
            
            // Bonus directo de magia (nuevo sistema prioritario, fallback a legacy)
            baseAttack += (EquippedWeaponNew?.BonusMagicPower ?? EquippedWeapon?.MagicBonus ?? 0);
            
            return baseAttack;
        }
        
        private int GetPhysicalDefense()
        {
            var baseDefense = ActiveConstitution + (ActiveDexterity / 2);
            
            // Scaling por clase
            var classBonus = Class switch
            {
                CharacterClass.Paladin => Level,
                CharacterClass.Warrior => Level / 2,
                CharacterClass.Monk => ActiveWisdom / 2,
                _ => 0
            };
            
            baseDefense += classBonus;
            
            // Bonus directo de equipamiento (nuevo sistema prioritario, fallback a legacy)
            baseDefense += (EquippedWeaponNew?.BonusDefense ?? 0);
            baseDefense += (EquippedArmorNew?.BonusDefense ?? EquippedArmor?.DefenseBonus ?? 0);
            baseDefense += (EquippedAccessoryNew?.BonusDefense ?? EquippedAccessory?.DefenseBonus ?? 0);
            
            return baseDefense;
        }
        
        private int GetMagicResistance()
        {
            var baseResist = ActiveWisdom + (ActiveIntelligence / 3);
            
            // Scaling por clase
            var classBonus = Class switch
            {
                CharacterClass.Mage => Level / 2,
                CharacterClass.Sorcerer => Level / 2,
                CharacterClass.Cleric => Level / 3,
                CharacterClass.Warlock => Level / 2,
                _ => 0
            };
            
            baseResist += classBonus;
            
            // Bonus directo de equipamiento (nuevo sistema prioritario, fallback a legacy)
            baseResist += (EquippedWeaponNew?.BonusMagicResistance ?? 0);
            baseResist += (EquippedArmorNew?.BonusMagicResistance ?? 0);
            baseResist += (EquippedAccessoryNew?.BonusMagicResistance ?? EquippedAccessory?.MagicResistanceBonus ?? 0);
            
            return baseResist;
        }
        
        private double GetCriticalChance()
        {
            // Base: DEX/2 + CHA/3 (usando stats activos)
            var baseCrit = (ActiveDexterity / 2.0) + (ActiveCharisma / 3.0);
            
            // Bonus de clase
            var classBonus = Class switch
            {
                CharacterClass.Assassin => 15.0,
                CharacterClass.Rogue => 10.0,
                CharacterClass.Ranger => 8.0,
                CharacterClass.Bard => 5.0,
                _ => 0.0
            };
            
            // Bonus de equipamiento (solo nuevo sistema)
            var equipBonus = (EquippedWeaponNew?.BonusCritChance ?? 0) + 
                            (EquippedArmorNew?.BonusCritChance ?? 0) + 
                            (EquippedAccessoryNew?.BonusCritChance ?? 0);
            
            // Caps: 5% min, 80% max
            var total = baseCrit + classBonus + (Level * 0.5) + equipBonus;
            return Math.Clamp(total, 5.0, 80.0);
        }
        
        private double GetEvasion()
        {
            // Base: DEX + Level/2 (usando stats activos)
            var baseEvasion = ActiveDexterity + (Level / 2.0);
            
            // Bonus de clase
            var classBonus = Class switch
            {
                CharacterClass.Rogue => 15.0,
                CharacterClass.Assassin => 12.0,
                CharacterClass.Monk => 10.0,
                CharacterClass.Ranger => 8.0,
                _ => 0.0
            };
            
            // Bonus de equipamiento (solo nuevo sistema)
            var equipBonus = (EquippedWeaponNew?.BonusEvasion ?? 0) + 
                            (EquippedArmorNew?.BonusEvasion ?? 0) + 
                            (EquippedAccessoryNew?.BonusEvasion ?? 0);
            
            // Caps: 0% min, 70% max
            var total = baseEvasion + classBonus + equipBonus;
            return Math.Clamp(total, 0.0, 70.0);
        }
    }
    
    public enum CharacterClass
    {
        // Básicas (Tier 1)
        Warrior,      // Combate físico, tanque
        Mage,         // Magia elemental
        Rogue,        // Sigilo y críticos
        Cleric,       // Curación y soporte
        
        // Avanzadas (Tier 2 - Evoluciones)
        Paladin,      // Warrior → Paladin (Nv.10) - Tank mágico
        Ranger,       // Rogue → Ranger (Nv.10) - Arquero
        Warlock,      // Mage → Warlock (Nv.10) - Magia oscura
        Monk,         // Cleric → Monk (Nv.10) - Combate cuerpo a cuerpo
        
        // Épicas (Tier 3 - Especializaciones)
        Berserker,    // Warrior → Berserker (Nv.20) - DPS puro
        Assassin,     // Rogue → Assassin (Nv.20) - Críticos letales
        Sorcerer,     // Mage → Sorcerer (Nv.20) - Multielemento
        Druid,        // Cleric → Druid (Nv.20) - Naturaleza
        
        // Legendarias (Tier 4 - Híbridas)
        Necromancer,  // Warlock + Experiencia (Nv.30) - Invocación
        Bard          // Hybrid support (Nv.30) - Buffs de equipo
    }
}
