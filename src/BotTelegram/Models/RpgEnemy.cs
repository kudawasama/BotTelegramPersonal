namespace BotTelegram.Models
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
        
        // Loot drops (opcional)
        public List<RpgItem>? PossibleLoot { get; set; }
    }
    
    public enum EnemyDifficulty
    {
        Easy,
        Medium,
        Hard,
        Boss
    }
}
