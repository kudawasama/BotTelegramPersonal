namespace BotTelegram.Models
{
    public class RpgItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "📦";
        public string Description { get; set; } = "";
        public ItemType Type { get; set; } = ItemType.Consumable;
        public int Value { get; set; } = 10; // Precio en oro
        
        // Stats (para armas/armadura)
        public int AttackBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;
        public int HPBonus { get; set; } = 0;
        
        // Para consumibles
        public int HPRestore { get; set; } = 0;
        public int EnergyRestore { get; set; } = 0;
        
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    }
    
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Quest,
        Material
    }
    
    public enum ItemRarity
    {
        Common,    // Gris
        Uncommon,  // Verde
        Rare,      // Azul
        Epic,      // Púrpura
        Legendary  // Naranja
    }
}
