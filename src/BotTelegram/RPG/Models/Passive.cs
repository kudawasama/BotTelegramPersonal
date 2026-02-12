using System.Collections.Generic;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Define una pasiva permanente que el jugador puede desbloquear
    /// </summary>
    public class Passive
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "✨";
        public PassiveType Type { get; set; }
        public int Value { get; set; } // Valor numérico del bonus
    }

    public enum PassiveType
    {
        // Bonuses permanentes
        CriticalDamageBonus,      // +X% daño crítico
        CriticalChanceBonus,      // +X% chance de crítico
        PhysicalDamageBonus,      // +X daño físico
        MagicalDamageBonus,       // +X daño mágico
        MaxHPBonus,               // +X HP máximo
        MaxManaBonus,             // +X Mana máximo
        MaxStaminaBonus,          // +X Stamina máxima
        GoldFindBonus,            // +X% oro encontrado
        XPBonus,                  // +X% XP ganado
        LootDropBonus,            // +X% drop rate
        
        // Habilidades especiales
        SecondWind,               // Auto-revive 1 vez por combate
        Bloodlust,                // Daño aumenta al bajar HP
        Meditation,               // Regen mana fuera de combate
        Regeneration,             // Regen HP fuera de combate
        CounterAttack,            // Chance de contraatacar al defender
        Thorns,                   // Devuelve daño al ser golpeado
        LifeSteal,                // Roba vida con ataques físicos
        SpellVamp,                // Roba vida con ataques mágicos
        
        // Pasivas de exploración
        BeastWhisperer,           // Puede acariciar/calmar bestias
        MerchantFriend,           // Descuentos en tiendas
        TreasureHunter,           // Encuentra más tesoros
        Survivalist,              // Consume menos recursos al viajar
        
        // Pasivas de clase oculta
        BeastMastery,             // Puede domar bestias
        ShadowStep,               // Puede atacar desde las sombras
        DivineBlessing,           // Heals aumentados
        NecroticTouch,            // Daño oscuro adicional
        ElementalAffinity,        // Resistencia elemental
        BladeDancer              // Combo no se resetea al fallar
    }
