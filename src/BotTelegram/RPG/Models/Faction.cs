namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Facción del mundo con la que los jugadores pueden ganar reputación
    /// </summary>
    public class Faction
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "⚔️";
        
        /// <summary>
        /// Región principal donde opera esta facción
        /// </summary>
        public string PrimaryRegionId { get; set; } = "";
        
        /// <summary>
        /// Facción enemiga (ganar reputación aquí reduce reputación con el enemigo)
        /// </summary>
        public string? EnemyFactionId { get; set; }
        
        /// <summary>
        /// Facciones aliadas (ganar reputación con aliados da bonus menor aquí)
        /// </summary>
        public List<string> AlliedFactionIds { get; set; } = new();
        
        /// <summary>
        /// Recompensas por tier de reputación
        /// </summary>
        public Dictionary<FactionTier, FactionReward> Rewards { get; set; } = new();
    }
    
    /// <summary>
    /// Niveles de reputación con una facción
    /// </summary>
    public enum FactionTier
    {
        Hated = -2,      // < -3000
        Hostile = -1,    // -3000 a -1000
        Neutral = 0,     // -1000 a +1000
        Friendly = 1,    // +1000 a +3000
        Honored = 2,     // +3000 a +6000
        Revered = 3,     // +6000 a +10000
        Exalted = 4      // +10000+
    }
    
    /// <summary>
    /// Recompensa por alcanzar un tier de reputación
    /// </summary>
    public class FactionReward
    {
        public int GoldReward { get; set; }
        public int XPReward { get; set; }
        public string? UnlockedZoneId { get; set; }
        public string? UnlockedRecipeId { get; set; }
        public string? UnlockedQuestId { get; set; }
        public string? UnlockedTitle { get; set; }
        public int ShopDiscountPercent { get; set; }
    }
    
    /// <summary>
    /// Reputación del jugador con una facción específica
    /// </summary>
    public class PlayerFactionReputation
    {
        public string FactionId { get; set; } = "";
        public int Reputation { get; set; } = 0; // Valor numérico (-10000 a +15000)
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Calcula el tier actual basado en reputación
        /// </summary>
        public FactionTier GetTier()
        {
            return Reputation switch
            {
                <= -3000 => FactionTier.Hated,
                <= -1000 => FactionTier.Hostile,
                <= 1000 => FactionTier.Neutral,
                <= 3000 => FactionTier.Friendly,
                <= 6000 => FactionTier.Honored,
                <= 10000 => FactionTier.Revered,
                _ => FactionTier.Exalted
            };
        }
        
        /// <summary>
        /// Obtiene el progreso hacia el siguiente tier (0-100%)
        /// </summary>
        public double GetProgressToNextTier()
        {
            var currentTier = GetTier();
            if (currentTier == FactionTier.Exalted) return 100.0;
            
            var (min, max) = currentTier switch
            {
                FactionTier.Hated => (-10000, -3000),
                FactionTier.Hostile => (-3000, -1000),
                FactionTier.Neutral => (-1000, 1000),
                FactionTier.Friendly => (1000, 3000),
                FactionTier.Honored => (3000, 6000),
                FactionTier.Revered => (6000, 10000),
                _ => (10000, 15000)
            };
            
            return ((double)(Reputation - min) / (max - min)) * 100.0;
        }
    }
}
