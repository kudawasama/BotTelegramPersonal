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
        public int HP { get; set; } = 100;
        public int MaxHP { get; set; } = 100;
        public int Energy { get; set; } = 50;
        public int MaxEnergy { get; set; } = 50;
        public int Mana { get; set; } = 0; // Para clases mágicas
        public int MaxMana { get; set; } = 0;
        
        public int Strength { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10; // Nuevo stat
        public int Wisdom { get; set; } = 10; // Nuevo stat
        public int Charisma { get; set; } = 10; // Nuevo stat
        
        // Resources
        public int Gold { get; set; } = 50;
        public List<RpgItem> Inventory { get; set; } = new();
        public RpgItem? EquippedWeapon { get; set; }
        public RpgItem? EquippedArmor { get; set; }
        public RpgItem? EquippedAccessory { get; set; } // Nuevo slot
        
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
        
        // Computed properties
        [JsonIgnore]
        public int TotalAttack => GetTotalAttack();
        
        [JsonIgnore]
        public int TotalDefense => GetTotalDefense();
        
        [JsonIgnore]
        public int TotalMagicPower => Intelligence + (EquippedWeapon?.MagicBonus ?? 0);
        
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
        
        private int GetTotalAttack()
        {
            var baseAttack = Strength;
            
            // Bonus según clase
            baseAttack += Class switch
            {
                CharacterClass.Warrior => Level / 2,
                CharacterClass.Berserker => Level,
                CharacterClass.Paladin => Level / 3,
                CharacterClass.Assassin => Dexterity / 2,
                _ => 0
            };
            
            return baseAttack + (EquippedWeapon?.AttackBonus ?? 0);
        }
        
        private int GetTotalDefense()
        {
            var baseDefense = Dexterity / 2 + Constitution / 3;
            
            // Bonus según clase
            baseDefense += Class switch
            {
                CharacterClass.Paladin => Level / 2,
                CharacterClass.Monk => Wisdom / 2,
                _ => 0
            };
            
            return baseDefense + (EquippedArmor?.DefenseBonus ?? 0) + (EquippedAccessory?.DefenseBonus ?? 0);
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
