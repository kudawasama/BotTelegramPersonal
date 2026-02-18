namespace BotTelegram.RPG.Models
{
    // ─────────────────────────────────────────────────────────────────────────
    // CRAFTEO: Receta e Ingrediente
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Un ingrediente necesario para una receta de crafteo.</summary>
    public class CraftIngredient
    {
        public string ItemName { get; set; } = "";   // Nombre del item (Material)
        public int Quantity   { get; set; } = 1;
    }

    /// <summary>Resultado del crafteo: item consumible o equipo.</summary>
    public enum CraftResultType { Item, Equipment }

    /// <summary>Receta de crafteo completa.</summary>
    public class CraftRecipe
    {
        public string Id          { get; set; } = "";
        public string Name        { get; set; } = "";
        public string Emoji       { get; set; } = "⚒️";
        public string Description { get; set; } = "";

        public int RequiredLevel { get; set; } = 1;

        /// <summary>Materiales del inventario necesarios.</summary>
        public List<CraftIngredient> Ingredients { get; set; } = new();

        /// <summary>Nombre del equipo o item resultante.</summary>
        public string ResultName      { get; set; } = "";
        public CraftResultType ResultType { get; set; } = CraftResultType.Item;

        /// <summary>ID al que apunta en EquipmentDatabase (si es Equipment) o plantilla de RpgItem.</summary>
        public string? ResultEquipmentId { get; set; }

        // Para cuando el resultado es un RpgItem consumible directamente:
        public int ResultHPRestore   { get; set; }
        public int ResultManaRestore { get; set; }
        public int ResultValue       { get; set; } = 50;
        public ItemRarity ResultRarity { get; set; } = ItemRarity.Uncommon;
        public string ResultEmoji    { get; set; } = "📦";
        public string ResultDesc     { get; set; } = "";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MISIONES / QUESTS
    // ─────────────────────────────────────────────────────────────────────────

    public enum QuestStatus { Available, Active, Completed, Failed }
    public enum QuestType   { Kill, Collect, Explore, Craft, Talk }

    /// <summary>Objetivo individual de una misión.</summary>
    public class QuestObjective
    {
        public string Description  { get; set; } = "";
        public QuestType Type      { get; set; }
        public string TargetId     { get; set; } = "";   // enemyId / itemName / zoneId
        public int Required        { get; set; } = 1;
        public int Current         { get; set; } = 0;
        public bool IsCompleted    => Current >= Required;
    }

    /// <summary>Recompensas de una misión completada.</summary>
    public class QuestReward
    {
        public int GoldReward  { get; set; }
        public int XPReward    { get; set; }
        public string? ItemRewardName { get; set; }   // Nombre descriptivo
        public string? EquipId       { get; set; }   // ID en EquipmentDatabase
    }

    /// <summary>Definición estática de una misión (template).</summary>
    public class QuestDefinition
    {
        public string Id          { get; set; } = "";
        public string Name        { get; set; } = "";
        public string Emoji       { get; set; } = "📜";
        public string Description { get; set; } = "";
        public string NPCName     { get; set; } = "Aldeano";
        public int RequiredLevel  { get; set; } = 1;
        public List<QuestObjective> Objectives { get; set; } = new();
        public QuestReward Reward { get; set; } = new();
        public bool IsRepeatable  { get; set; } = false;
    }

    /// <summary>Estado de una misión activa/completada del jugador.</summary>
    public class PlayerQuest
    {
        public string QuestId  { get; set; } = "";
        public QuestStatus Status { get; set; } = QuestStatus.Active;
        public List<QuestObjective> Objectives { get; set; } = new();
        public DateTime StartedAt   { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
