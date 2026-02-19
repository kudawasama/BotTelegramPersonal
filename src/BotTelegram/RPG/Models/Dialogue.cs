namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Sistema de diálogo con NPCs (árbol de opciones)
    /// </summary>
    public class Dialogue
    {
        public string Id { get; set; } = "";
        public string NPCId { get; set; } = "";
        
        /// <summary>
        /// Texto que dice el NPC
        /// </summary>
        public string Text { get; set; } = "";
        
        /// <summary>
        /// Tipo de diálogo
        /// </summary>
        public DialogueType Type { get; set; } = DialogueType.Greeting;
        
        /// <summary>
        /// Requisitos para que este diálogo aparezca
        /// </summary>
        public DialogueRequirement? Requirement { get; set; }
        
        /// <summary>
        /// Opciones de respuesta del jugador
        /// </summary>
        public List<DialogueOption> Options { get; set; } = new();
        
        /// <summary>
        /// Recompensa al completar este diálogo (opcional)
        /// </summary>
        public DialogueReward? Reward { get; set; }
    }
    
    /// <summary>
    /// Tipos de diálogo
    /// </summary>
    public enum DialogueType
    {
        Greeting,       // Saludo inicial
        Story,          // Historia/lore
        QuestOffer,     // Ofrecer misión
        QuestProgress,  // Progreso de misión
        QuestComplete,  // Completar misión
        Trade,          // Comercio
        Training,       // Entrenamiento
        Farewell        // Despedida
    }
    
    /// <summary>
    /// Requisito para que un diálogo esté disponible
    /// </summary>
    public class DialogueRequirement
    {
        public int? MinLevel { get; set; }
        public int? MinReputation { get; set; }
        public string? RequiredQuestCompleted { get; set; }
        public string? RequiredQuestActive { get; set; }
        public string? RequiredItemId { get; set; }
        public string? RequiredClass { get; set; }
    }
    
    /// <summary>
    /// Opción de respuesta del jugador en el diálogo
    /// </summary>
    public class DialogueOption
    {
        public string Id { get; set; } = "";
        public string Text { get; set; } = "";
        
        /// <summary>
        /// ID del siguiente diálogo al elegir esta opción (null = termina conversación)
        /// </summary>
        public string? NextDialogueId { get; set; }
        
        /// <summary>
        /// Acción que ejecuta esta opción
        /// </summary>
        public DialogueAction? Action { get; set; }
    }
    
    /// <summary>
    /// Acción que ejecuta una opción de diálogo
    /// </summary>
    public class DialogueAction
    {
        public DialogueActionType Type { get; set; }
        public string? TargetId { get; set; }
        public int? Value { get; set; }
    }
    
    /// <summary>
    /// Tipos de acción de diálogo
    /// </summary>
    public enum DialogueActionType
    {
        None,                   // Sin acción
        OpenShop,               // Abrir tienda del NPC
        StartQuest,             // Iniciar quest
        CompleteQuest,          // Completar quest
        GainReputation,         // Ganar reputación
        LoseReputation,         // Perder reputación
        GiveItem,               // Dar ítem al jugador
        TakeItem,               // Quitar ítem al jugador
        GiveGold,               // Dar oro
        TakeGold,               // Quitar oro
        Teleport,               // Teletransportar a zona
        UnlockZone,             // Desbloquear zona
        StartTraining           // Abrir entrenamiento
    }
    
    /// <summary>
    /// Recompensa al completar un diálogo
    /// </summary>
    public class DialogueReward
    {
        public int Gold { get; set; }
        public int XP { get; set; }
        public int Reputation { get; set; }
        public string? FactionId { get; set; }
    }
}
