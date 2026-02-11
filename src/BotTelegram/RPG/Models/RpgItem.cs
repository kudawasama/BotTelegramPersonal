namespace BotTelegram.RPG.Models
{
    public class RpgItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "📦";
        public string Description { get; set; } = "";
        public ItemType Type { get; set; } = ItemType.Consumable;
        public int Value { get; set; } = 10; // Precio en oro
        public int RequiredLevel { get; set; } = 1;
        
        // Stats (para armas/armadura)
        public int AttackBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;
        public int MagicResistanceBonus { get; set; } = 0;
        public int HPBonus { get; set; } = 0;
        public int ManaBonus { get; set; } = 0;
        public int MagicBonus { get; set; } = 0;
        
        // Para consumibles
        public int HPRestore { get; set; } = 0;
        public int ManaRestore { get; set; } = 0;
        public int StaminaRestore { get; set; } = 0;
        
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        
        // Special effects
        public bool GrantsCriticalBonus { get; set; } = false;
        public int CriticalChanceBonus { get; set; } = 0; // Porcentaje (0-100)
        public bool GrantsLifeSteal { get; set; } = false;
        public int LifeStealPercent { get; set; } = 0;
    }
    
    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,    // Nuevo: anillos, amuletos
        Consumable,
        Quest,
        Material
    }
    
    public enum ItemRarity
    {
        Common,    // Gris - Drop 60%
        Uncommon,  // Verde - Drop 25%
        Rare,      // Azul - Drop 10%
        Epic,      // Púrpura - Drop 4%
        Legendary  // Naranja - Drop 1%
    }
}
