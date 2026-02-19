namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// NPC (Non-Player Character) con diálogos e interacciones
    /// </summary>
    public class NPC
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "👤";
        
        /// <summary>
        /// ID de la zona donde se encuentra este NPC
        /// </summary>
        public string ZoneId { get; set; } = "";
        
        /// <summary>
        /// Facción a la que pertenece (null si es neutral)
        /// </summary>
        public string? FactionId { get; set; }
        
        /// <summary>
        /// Tipo de NPC (vendedor, quest giver, entrenador, etc.)
        /// </summary>
        public NPCType Type { get; set; } = NPCType.Generic;
        
        /// <summary>
        /// Nivel mínimo de reputación con su facción para hablar con él (-10000 a +15000)
        /// </summary>
        public int RequiredReputation { get; set; } = -10000;
        
        /// <summary>
        /// IDs de diálogos disponibles con este NPC
        /// </summary>
        public List<string> DialogueIds { get; set; } = new();
        
        /// <summary>
        /// IDs de quests que este NPC puede dar
        /// </summary>
        public List<string> QuestIds { get; set; } = new();
        
        /// <summary>
        /// IDs de ítems/equipos que este NPC vende (si es vendedor)
        /// </summary>
        public List<string> ShopInventoryIds { get; set; } = new();
        
        /// <summary>
        /// Descuento en tienda según reputación del jugador (%)
        /// </summary>
        public int ShopDiscountPercent { get; set; } = 0;
    }
    
    /// <summary>
    /// Tipos de NPC según su función
    /// </summary>
    public enum NPCType
    {
        Generic,         // NPC genérico sin función especial
        Merchant,        // Vendedor de ítems
        QuestGiver,      // Da misiones
        Trainer,         // Entrena habilidades
        BankKeeper,      // Acceso al banco/guild
        Innkeeper,       // Posada (restaurar HP/Mana, guardar progreso)
        Blacksmith,      // Crafteo/reparación de equipo
        Enchanter,       // Encantar/mejorar equipos
        FactionLeader,   // Líder de facción (quests importantes)
        StoryNPC         // NPC con historia (solo diálogo)
    }
    
    /// <summary>
    /// Resultado de interacción con un NPC
    /// </summary>
    public class NPCInteractionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public NPC? NPC { get; set; }
        public Dialogue? CurrentDialogue { get; set; }
        public List<string> AvailableOptions { get; set; } = new();
    }
}
