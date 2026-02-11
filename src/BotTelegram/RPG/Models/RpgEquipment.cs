namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Tipos de equipo
    /// </summary>
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Accessory
    }
    
    /// <summary>
    /// Rareza del equipo
    /// </summary>
    public enum EquipmentRarity
    {
        Common,      // +10% stats
        Uncommon,    // +25% stats
        Rare,        // +50% stats
        Epic,        // +100% stats
        Legendary    // +200% stats
    }
    
    /// <summary>
    /// Modelo de equipo (armas, armaduras, accesorios)
    /// </summary>
    public class RpgEquipment
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public EquipmentType Type { get; set; }
        public EquipmentRarity Rarity { get; set; }
        public int RequiredLevel { get; set; }
        
        // Bonificaciones de estadísticas base
        public int BonusStrength { get; set; }
        public int BonusIntelligence { get; set; }
        public int BonusDexterity { get; set; }
        public int BonusConstitution { get; set; }
        public int BonusWisdom { get; set; }
        public int BonusCharisma { get; set; }
        
        // Bonificaciones derivadas
        public int BonusAttack { get; set; }
        public int BonusMagicPower { get; set; }
        public int BonusDefense { get; set; }
        public int BonusMagicResistance { get; set; }
        public int BonusHP { get; set; }
        public int BonusMana { get; set; }
        public int BonusStamina { get; set; }
        public int BonusAccuracy { get; set; }
        public int BonusEvasion { get; set; }
        public int BonusCritChance { get; set; }
        public int BonusCritDamage { get; set; }
        
        // Precio
        public int Price { get; set; }
        
        /// <summary>
        /// Obtiene el emoji según la rareza
        /// </summary>
        public string RarityEmoji => Rarity switch
        {
            EquipmentRarity.Common => "⚪",
            EquipmentRarity.Uncommon => "🟢",
            EquipmentRarity.Rare => "🔵",
            EquipmentRarity.Epic => "🟣",
            EquipmentRarity.Legendary => "🟡",
            _ => "⚪"
        };
        
        /// <summary>
        /// Obtiene el emoji según el tipo
        /// </summary>
        public string TypeEmoji => Type switch
        {
            EquipmentType.Weapon => "⚔️",
            EquipmentType.Armor => "🛡️",
            EquipmentType.Accessory => "💍",
            _ => "❓"
        };
        
        /// <summary>
        /// Obtiene el multiplicador de rareza
        /// </summary>
        public double RarityMultiplier => Rarity switch
        {
            EquipmentRarity.Common => 1.0,
            EquipmentRarity.Uncommon => 1.25,
            EquipmentRarity.Rare => 1.5,
            EquipmentRarity.Epic => 2.0,
            EquipmentRarity.Legendary => 3.0,
            _ => 1.0
        };
        
        /// <summary>
        /// Crea una copia del equipo
        /// </summary>
        public RpgEquipment Clone()
        {
            return new RpgEquipment
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Type = Type,
                Rarity = Rarity,
                RequiredLevel = RequiredLevel,
                BonusStrength = BonusStrength,
                BonusIntelligence = BonusIntelligence,
                BonusDexterity = BonusDexterity,
                BonusConstitution = BonusConstitution,
                BonusWisdom = BonusWisdom,
                BonusCharisma = BonusCharisma,
                BonusAttack = BonusAttack,
                BonusMagicPower = BonusMagicPower,
                BonusDefense = BonusDefense,
                BonusMagicResistance = BonusMagicResistance,
                BonusHP = BonusHP,
                BonusMana = BonusMana,
                BonusStamina = BonusStamina,
                BonusAccuracy = BonusAccuracy,
                BonusEvasion = BonusEvasion,
                BonusCritChance = BonusCritChance,
                BonusCritDamage = BonusCritDamage,
                Price = Price
            };
        }
        
        /// <summary>
        /// Obtiene descripción completa del equipo
        /// </summary>
        public string GetFullDescription()
        {
            var desc = $"{RarityEmoji} {TypeEmoji} **{Name}** ({Rarity})\n{Description}\n\n";
            desc += $"**Nivel requerido:** {RequiredLevel}\n";
            desc += $"**Precio:** {Price} 💰\n\n";
            desc += "**Bonificaciones:**\n";
            
            if (BonusStrength > 0) desc += $"• +{BonusStrength} STR\n";
            if (BonusIntelligence > 0) desc += $"• +{BonusIntelligence} INT\n";
            if (BonusDexterity > 0) desc += $"• +{BonusDexterity} DEX\n";
            if (BonusConstitution > 0) desc += $"• +{BonusConstitution} CON\n";
            if (BonusWisdom > 0) desc += $"• +{BonusWisdom} WIS\n";
            if (BonusCharisma > 0) desc += $"• +{BonusCharisma} CHA\n";
            if (BonusAttack > 0) desc += $"• +{BonusAttack} Ataque\n";
            if (BonusMagicPower > 0) desc += $"• +{BonusMagicPower} Poder Mágico\n";
            if (BonusDefense > 0) desc += $"• +{BonusDefense} Defensa\n";
            if (BonusMagicResistance > 0) desc += $"• +{BonusMagicResistance} Resistencia Mágica\n";
            if (BonusHP > 0) desc += $"• +{BonusHP} HP\n";
            if (BonusMana > 0) desc += $"• +{BonusMana} Mana\n";
            if (BonusStamina > 0) desc += $"• +{BonusStamina} Stamina\n";
            if (BonusAccuracy > 0) desc += $"• +{BonusAccuracy} Precisión\n";
            if (BonusEvasion > 0) desc += $"• +{BonusEvasion} Evasión\n";
            if (BonusCritChance > 0) desc += $"• +{BonusCritChance}% Prob. Crítico\n";
            if (BonusCritDamage > 0) desc += $"• +{BonusCritDamage}% Daño Crítico\n";
            
            return desc;
        }
    }
}
