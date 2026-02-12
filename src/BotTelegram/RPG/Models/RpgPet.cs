using System.Text.Json.Serialization;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Representa una mascota domada por el jugador
    /// </summary>
    public class RpgPet
    {
        // Identificación
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Species { get; set; } = ""; // "Wolf", "Bear", "Dragon", etc.
        public PetRarity Rarity { get; set; } = PetRarity.Common;
        
        // Nivel y Progresión
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        
        // Sistema de Bond (Vínculo)
        public int Bond { get; set; } = 0; // 0-1000
        public PetLoyalty Loyalty { get; set; } = PetLoyalty.Neutral;
        
        // Stats de Combate
        public int HP { get; set; } = 30;
        public int MaxHP { get; set; } = 30;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int Speed { get; set; } = 5;
        public int MagicPower { get; set; } = 0;
        
        // Habilidades
        public List<string> Abilities { get; set; } = new(); // IDs de habilidades
        public List<StatusEffect> StatusEffects { get; set; } = new(); // Buffs/debuffs activos
        
        // Evolución
        public int EvolutionStage { get; set; } = 1; // 1=Basic, 2=Advanced, 3=Ultimate
        public int EvolutionXP { get; set; } = 0;
        public bool CanEvolve { get; set; } = false;
        
        // Comportamiento en Combate
        public PetBehavior Behavior { get; set; } = PetBehavior.Balanced;
        
        // Estadísticas de Progreso
        public int TotalKills { get; set; } = 0;
        public int BossKills { get; set; } = 0;
        public int TotalDamageDealt { get; set; } = 0;
        public int TimesRevived { get; set; } = 0;
        
        // Timestamps
        public DateTime TamedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastFed { get; set; } = DateTime.UtcNow;
        
        // Propiedades calculadas
        [JsonIgnore]
        public int XPNeeded => Level * 50; // Pets suben más rápido que jugadores
        
        [JsonIgnore]
        public double BondPercentage => (Bond / 1000.0) * 100;
        
        [JsonIgnore]
        public string LoyaltyEmoji => Loyalty switch
        {
            PetLoyalty.Hostile => "💢",
            PetLoyalty.Neutral => "😐",
            PetLoyalty.Friendly => "😊",
            PetLoyalty.Loyal => "💙",
            PetLoyalty.Devoted => "💖",
            _ => "❓"
        };
        
        [JsonIgnore]
        public string RarityEmoji => Rarity switch
        {
            PetRarity.Common => "⚪",
            PetRarity.Uncommon => "🟢",
            PetRarity.Rare => "🔵",
            PetRarity.Epic => "🟣",
            PetRarity.Legendary => "🟡",
            PetRarity.Mythical => "🔴",
            _ => "⚪"
        };
        
        [JsonIgnore]
        public double LoyaltyStatBonus => Loyalty switch
        {
            PetLoyalty.Hostile => -0.30,  // -30% stats si te odia
            PetLoyalty.Neutral => 0.0,     // Sin bonus
            PetLoyalty.Friendly => 0.20,   // +20%
            PetLoyalty.Loyal => 0.50,      // +50%
            PetLoyalty.Devoted => 1.00,    // +100% (DOBLE stats!)
            _ => 0.0
        };
        
        /// <summary>
        /// Calcula el ATK efectivo considerando bond
        /// </summary>
        [JsonIgnore]
        public int EffectiveAttack => (int)(Attack * (1 + LoyaltyStatBonus));
        
        /// <summary>
        /// Calcula la DEF efectiva considerando bond
        /// </summary>
        [JsonIgnore]
        public int EffectiveDefense => (int)(Defense * (1 + LoyaltyStatBonus));
        
        /// <summary>
        /// Actualiza Loyalty basado en Bond actual
        /// </summary>
        public void UpdateLoyalty()
        {
            if (Bond < 200) Loyalty = PetLoyalty.Hostile;
            else if (Bond < 400) Loyalty = PetLoyalty.Neutral;
            else if (Bond < 600) Loyalty = PetLoyalty.Friendly;
            else if (Bond < 800) Loyalty = PetLoyalty.Loyal;
            else Loyalty = PetLoyalty.Devoted;
        }
        
        /// <summary>
        /// Aumenta bond con la mascota
        /// </summary>
        public void IncreaseBond(int amount)
        {
            Bond = Math.Min(1000, Bond + amount);
            UpdateLoyalty();
        }
        
        /// <summary>
        /// Reduce bond con la mascota
        /// </summary>
        public void DecreaseBond(int amount)
        {
            Bond = Math.Max(0, Bond - amount);
            UpdateLoyalty();
        }
        
        /// <summary>
        /// Verifica si la mascota puede evolucionar
        /// </summary>
        public bool CheckEvolution(int bondRequired, int killsRequired, int bossKillsRequired)
        {
            if (EvolutionStage >= 3) return false; // Ya está en etapa final
            
            CanEvolve = Level >= GetRequiredLevelForEvolution() 
                       && Bond >= bondRequired 
                       && TotalKills >= killsRequired
                       && BossKills >= bossKillsRequired;
            
            return CanEvolve;
        }
        
        /// <summary>
        /// Nivel requerido para evolución según etapa
        /// </summary>
        public int GetRequiredLevelForEvolution()
        {
            return EvolutionStage switch
            {
                1 => 15, // Basic → Advanced: Nivel 15
                2 => 35, // Advanced → Ultimate: Nivel 35
                _ => 99
            };
        }
    }
    
    public enum PetLoyalty
    {
        Hostile = 0,    // 0-199 bond - Puede atacarte o desobedecerte
        Neutral = 1,    // 200-399 bond - Obedece órdenes básicas
        Friendly = 2,   // 400-599 bond - Obedece bien, +20% stats
        Loyal = 3,      // 600-799 bond - Obedece siempre, +50% stats
        Devoted = 4     // 800-1000 bond - Sacrificaría su vida, +100% stats
    }
    
    public enum PetBehavior
    {
        Aggressive,     // Siempre ataca al enemigo con más HP
        Defensive,      // Protege al jugador, ataca al que atacó al owner
        Balanced,       // Mix de ataque y protección (default)
        Supportive,     // Prioriza habilidades de buff/heal en vez de atacar
        Smart           // IA avanzada (ataca debilidades, usa habilidades óptimas)
    }
    
    public enum PetRarity
    {
        Common,         // ⚪ Fácil de encontrar
        Uncommon,       // 🟢 Poco común
        Rare,           // 🔵 Raro
        Epic,           // 🟣 Épico
        Legendary,      // 🟡 Legendario
        Mythical        // 🔴 Mítico (úni eventos especiales)
    }
}
