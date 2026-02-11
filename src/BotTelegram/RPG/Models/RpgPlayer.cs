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
        public RpgItem? EquippedWeapon { get; set; }
        public RpgItem? EquippedArmor { get; set; }
        public RpgItem? EquippedAccessory { get; set; }
        
        // ═══════════════════════════════════════
        // HABILIDADES
        // ═══════════════════════════════════════
        public List<RpgSkill> Skills { get; set; } = new();
        
        [JsonIgnore]
        public List<RpgSkill> AvailableSkills => Skills.Where(s => 
            s.CurrentCooldown == 0 && 
            s.ManaCost <= Mana && 
            s.StaminaCost <= Stamina).ToList();
        
        // Progress
        public string CurrentLocation { get; set; } = "Taberna de Puerto Esperanza";
        public DateTime LastActionTime { get; set; } = DateTime.UtcNow;
        
        // Combat
        public bool IsInCombat { get; set; } = false;
        public RpgEnemy? CurrentEnemy { get; set; }
        
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
        
        // ═══════════════════════════════════════
        // STATS DERIVADOS (Calculados)
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Ataque Físico = STR + (STR * bonuses de clase) + arma
        /// </summary>
        [JsonIgnore]
        public int PhysicalAttack => GetPhysicalAttack();
        
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
        public int XPNeeded => Level * 100;
        
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
            var baseAttack = Strength;
            
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
            
            // Bonus de nivel
            baseAttack += Level / 2;
            
            // Bonus de equipamiento
            return baseAttack + (EquippedWeapon?.AttackBonus ?? 0);
        }
        
        private int GetMagicalAttack()
        {
            var baseAttack = Intelligence;
            
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
            
            // Bonus de nivel
            baseAttack += Level / 2;
            
            // Bonus de equipamiento magic
            return baseAttack + (EquippedWeapon?.MagicBonus ?? 0);
        }
        
        private int GetPhysicalDefense()
        {
            var baseDefense = Constitution + (Dexterity / 2);
            
            // Scaling por clase
            var classBonus = Class switch
            {
                CharacterClass.Paladin => Level,
                CharacterClass.Warrior => Level / 2,
                CharacterClass.Monk => Wisdom / 2,
                _ => 0
            };
            
            baseDefense += classBonus;
            
            // Bonus de armadura y accesorios
            return baseDefense + 
                   (EquippedArmor?.DefenseBonus ?? 0) + 
                   (EquippedAccessory?.DefenseBonus ?? 0);
        }
        
        private int GetMagicResistance()
        {
            var baseResist = Wisdom + (Intelligence / 3);
            
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
            
            // Bonus de accesorios mágicos
            return baseResist + (EquippedAccessory?.MagicResistanceBonus ?? 0);
        }
        
        private double GetCriticalChance()
        {
            // Base: DEX/2 + CHA/3
            var baseCrit = (Dexterity / 2.0) + (Charisma / 3.0);
            
            // Bonus de clase
            var classBonus = Class switch
            {
                CharacterClass.Assassin => 15.0,
                CharacterClass.Rogue => 10.0,
                CharacterClass.Ranger => 8.0,
                CharacterClass.Bard => 5.0,
                _ => 0.0
            };
            
            // Caps: 5% min, 80% max
            var total = baseCrit + classBonus + (Level * 0.5);
            return Math.Clamp(total, 5.0, 80.0);
        }
        
        private double GetEvasion()
        {
            // Base: DEX + Level/2
            var baseEvasion = Dexterity + (Level / 2.0);
            
            // Bonus de clase
            var classBonus = Class switch
            {
                CharacterClass.Rogue => 15.0,
                CharacterClass.Assassin => 12.0,
                CharacterClass.Monk => 10.0,
                CharacterClass.Ranger => 8.0,
                _ => 0.0
            };
            
            // Caps: 0% min, 70% max
            var total = baseEvasion + classBonus;
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
