using System.Text.Json.Serialization;

namespace BotTelegram.Models
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
        
        public int Strength { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        
        // Resources
        public int Gold { get; set; } = 50;
        public List<RpgItem> Inventory { get; set; } = new();
        public RpgItem? EquippedWeapon { get; set; }
        public RpgItem? EquippedArmor { get; set; }
        
        // Progress
        public string CurrentLocation { get; set; } = "Taberna de Puerto Esperanza";
        public DateTime LastActionTime { get; set; } = DateTime.UtcNow;
        
        // Combat
        public bool IsInCombat { get; set; } = false;
        public RpgEnemy? CurrentEnemy { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
        
        // Computed properties
        [JsonIgnore]
        public int TotalAttack => Strength + (EquippedWeapon?.AttackBonus ?? 0);
        
        [JsonIgnore]
        public int TotalDefense => Dexterity / 2 + (EquippedArmor?.DefenseBonus ?? 0);
        
        [JsonIgnore]
        public int XPNeeded => Level * 100;
    }
    
    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Cleric
    }
}
