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
        BladeDancer,              // Combo no se resetea al fallar
        
        // ═══════════════════════════════════════════════════════════════
        // FASE 3 - NUEVAS PASIVAS (40 nuevos tipos)
        // ═══════════════════════════════════════════════════════════════
        
        // Fortress Knight passives
        UnbreakableDefense,       // +50% block chance, +30 Physical Defense
        DamageReflection,         // 25% daño bloqueado se refleja
        ShieldMastery,            // Escudos +50% stats
        
        // Immovable Mountain passives
        StoneSkin,                // Reducción fija 15 daño
        LastStand,                // Recupera 40% HP al llegar a 1 HP
        Immovable,                // Inmune Stun/Knockback
        
        // Berserker Blood Rage passives
        BloodFrenzy,              // +5% daño por 10% HP perdido
        RecklessAbandon,          // +50% daño, -30% DEF
        KillingSpree,             // +10% daño por 3 turnos post-kill
        
        // Arcane Siphoner passives
        ArcaneOverflow,           // Spells que exceden MaxMana +50% daño
        ManaBurn,                 // Spells consumen HP si no hay mana
        SpellAmplification,       // +60% daño mágico, -30% Physical Defense
        
        // Life Weaver passives
        DivineTouch,              // Heals +100%
        RegenerationAura,         // Recupera 10% HP cada turno
        LifeLink,                 // Revive con 60% HP al morir
        
        // Puppet Master passives
        MasterManipulator,        // +30% duración control mental
        PuppetStrings,            // Enemigos controlados +50% daño
        MindImmunity,             // Inmune control mental/confusión
        
        // Time Bender passives
        TemporalFlux,             // +50% velocidad base
        Foresight,                // Ve próximo movimiento enemigo
        TimeLoop,                 // 10% chance repetir acción gratis
        
        // Elemental Overlord passives
        ElementalFusion,          // Spells combinan 2 elementos
        ElementalImmunity,        // Inmune daño elemental
        PrimalForceUpgraded,      // Daño elemental +80%
        
        // Beast Lord passives
        BeastArmy,                // +2 slots mascota (total 3)
        AlphaDominance,           // Mascotas +100% daño
        BeastFusion,              // Fusionar 2 mascotas temporalmente
        
        // Lich King passives
        UndeadMastery,            // +3 slots minion (total 5)
        DeathAura,                // Enemigos pierden 5% MaxHP por turno
        Phylactery,               // Revive con 50% HP si mueres con >3 minions
        
        // Void Summoner passives
        EldritchPact,             // Invocaciones cuestan HP no mana
        VoidTouched,              // +100% daño void, -50% sanity
        BeyondDeath,              // Revive como aberración 1 vez por día
        
        // Bonuses adicionales (10 extra para completar 40)
        Fortress,                 // Inmune a knockback
        ManaRegeneration,         // +5% mana por turno en combate
        BloodPact,                // Convierte HP en ATK
        VoidShield,               // +30% resistencia void
        SoulSiphon,               // Drena 10% MaxHP enemigo
        DivineIntervention,       // Evita muerte 1 vez (Dios salva)
        TimeManipulation,         // Cooldowns -20%
        ElementalResonance,       // Spells elementales tienen 15% de aplicar status
        BeastBond,                // +30% bond con mascotas
        UnholyRegeneration        // Regenera HP matando (5% MaxHP por kill)
    }
}
