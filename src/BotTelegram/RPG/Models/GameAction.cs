namespace BotTelegram.RPG.Models
{
    public class GameAction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long PlayerId { get; set; }
        public ActionType Type { get; set; }
        public string Location { get; set; } = "";
        public DateTime StartsAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletesAt { get; set; }
        public int EnergyCost { get; set; } = 10;
        public bool IsCompleted { get; set; } = false;
        public ActionResult? Result { get; set; }
    }
    
    public enum ActionType
    {
        Explore,
        Train,
        Rest,
        Work,
        Shop,
        Quest,
        Dungeon,      // Nuevo
        PvP,          // Nuevo
        Craft         // Nuevo
    }
    
    public class ActionResult
    {
        public bool Success { get; set; } = true;
        public int XPGained { get; set; } = 0;
        public int GoldGained { get; set; } = 0;
        public int EnergyRestored { get; set; } = 0;
        public string Narrative { get; set; } = "";
        public RpgEnemy? EncounteredEnemy { get; set; }
        public List<RpgItem> LootFound { get; set; } = new();
    }
}
