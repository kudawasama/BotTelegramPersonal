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
        public int Defense { get; set; } = 2;
        public int XPReward { get; set; } = 20;
        public int GoldReward { get; set; } = 15;
        public EnemyDifficulty Difficulty { get; set; } = EnemyDifficulty.Easy;
        public EnemyType Type { get; set; } = EnemyType.Beast;
        
        // Special abilities
        public bool CanPoison { get; set; } = false;
        public bool CanStun { get; set; } = false;
        public bool CanHeal { get; set; } = false;
        public int MagicResistance { get; set; } = 0;
        public int PhysicalResistance { get; set; } = 0;
        
        // Combat avanzado
        public List<StatusEffect> StatusEffects { get; set; } = new();
        
        // Loot drops (opcional)
        public List<RpgItem>? PossibleLoot { get; set; }
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
