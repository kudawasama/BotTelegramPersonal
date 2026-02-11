using System.Text.Json.Serialization;

namespace BotTelegram.RPG.Models
{
    public class RpgEnemy
    {
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "👹";
        public int Level { get; set; } = 1;
        public int HP { get; set; } = 30;
        public int MaxHP { get; set; } = 30;
        public int Attack { get; set; } = 8;
        public int MagicPower { get; set; } = 5;
        public int PhysicalDefense { get; set; } = 2;
        public int MagicResistance { get; set; } = 1;
        public int Accuracy { get; set; } = 10;  // Precisión para golpear
        public int Evasion { get; set; } = 5;    // Probabilidad de esquivar
        public int XPReward { get; set; } = 20;
        public int GoldReward { get; set; } = 15;
        public EnemyDifficulty Difficulty { get; set; } = EnemyDifficulty.Easy;
        public EnemyType Type { get; set; } = EnemyType.Beast;
        
        // Special abilities
        public bool CanPoison { get; set; } = false;
        public bool CanStun { get; set; } = false;
        public bool CanHeal { get; set; } = false;
        
        // Combat avanzado
        public List<StatusEffect> StatusEffects { get; set; } = new();
        
        // Loot drops (opcional)
        public List<RpgItem>? PossibleLoot { get; set; }
        
        // Legacy property (compatibilidad)
        [JsonIgnore]
        public int Defense
        {
            get => PhysicalDefense;
            set => PhysicalDefense = value;
        }
    }
    
    public enum EnemyDifficulty
    {
        Easy,
        Medium,
        Hard,
        Elite,
        Boss,
        WorldBoss
    }
    
    public enum EnemyType
    {
        Beast,       // Animales (lobos, osos)
        Humanoid,    // Goblins, orcos, bandidos
        Undead,      // Esqueletos, zombies
        Demon,       // Demonios
        Dragon,      // Dragones
        Elemental,   // Elementales
        Aberration,  // Criaturas místicas
        Construct    // Golems
    }
}
