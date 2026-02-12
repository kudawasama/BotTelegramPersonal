using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de todas las pasivas desbloqueables
    /// </summary>
    public static class PassiveDatabase
    {
        private static readonly List<Passive> _passives = new()
        {
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS BÁSICAS (Desbloqueadas por acciones simples)
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "beast_whisperer",
                Name = "Susurrador de Bestias",
                Emoji = "🐾",
                Description = "Puedes comunicarte y calmar bestias salvajes. Desbloquea acción 'Domar Bestia'.",
                Type = PassiveType.BeastWhisperer,
                Value = 1
            },
            
            new Passive
            {
                Id = "shadow_step",
                Name = "Paso de Sombra",
                Emoji = "👤",
                Description = "Puedes atacar desde las sombras. +50% chance crítico en primer ataque.",
                Type = PassiveType.ShadowStep,
                Value = 50
            },
            
            new Passive
            {
                Id = "divine_blessing",
                Name = "Bendición Divina",
                Emoji = "✨",
                Description = "Tus curaciones son 50% más efectivas.",
                Type = PassiveType.DivineBlessing,
                Value = 50
            },
            
            new Passive
            {
                Id = "necrotic_touch",
                Name = "Toque Necrótico",
                Emoji = "☠️",
                Description = "Tus ataques causan +20 de daño necrótico adicional.",
                Type = PassiveType.NecroticTouch,
                Value = 20
            },
            
            new Passive
            {
                Id = "elemental_affinity",
                Name = "Afinidad Elemental",
                Emoji = "🌊",
                Description = "+30% resistencia a daño elemental (fuego, hielo, rayo).",
                Type = PassiveType.ElementalAffinity,
                Value = 30
            },
            
            new Passive
            {
                Id = "blade_dancer",
                Name = "Danzante de Hojas",
                Emoji = "⚔️",
                Description = "Tu combo no se resetea al fallar un ataque.",
                Type = PassiveType.BladeDancer,
                Value = 1
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS DE COMBAT (Bonus permanentes)
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "critical_mastery",
                Name = "Maestría Crítica",
                Emoji = "💥",
                Description = "+10% de probabilidad de crítico.",
                Type = PassiveType.CriticalChanceBonus,
                Value = 10
            },
            
            new Passive
            {
                Id = "lethal_strikes",
                Name = "Golpes Letales",
                Emoji = "🗡️",
                Description = "Tus críticos causan +25% más daño.",
                Type = PassiveType.CriticalDamageBonus,
                Value = 25
            },
            
            new Passive
            {
                Id = "berserker_rage",
                Name = "Furia Berserker",
                Emoji = "😈",
                Description = "+15 de daño físico permanente.",
                Type = PassiveType.PhysicalDamageBonus,
                Value = 15
            },
            
            new Passive
            {
                Id = "arcane_power",
                Name = "Poder Arcano",
                Emoji = "🔮",
                Description = "+20 de daño mágico permanente.",
                Type = PassiveType.MagicalDamageBonus,
                Value = 20
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS DE SUPERVIVENCIA
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "iron_skin",
                Name = "Piel de Hierro",
                Emoji = "🛡️",
                Description = "+50 HP máximo.",
                Type = PassiveType.MaxHPBonus,
                Value = 50
            },
            
            new Passive
            {
                Id = "mana_font",
                Name = "Fuente de Mana",
                Emoji = "💠",
                Description = "+30 Mana máximo.",
                Type = PassiveType.MaxManaBonus,
                Value = 30
            },
            
            new Passive
            {
                Id = "tireless",
                Name = "Incansable",
                Emoji = "💪",
                Description = "+20 Stamina máxima.",
                Type = PassiveType.MaxStaminaBonus,
                Value = 20
            },
            
            new Passive
            {
                Id = "second_wind",
                Name = "Segundo Aliento",
                Emoji = "🌟",
                Description = "Auto-resucitas con 30% HP una vez por combate.",
                Type = PassiveType.SecondWind,
                Value = 30
            },
            
            new Passive
            {
                Id = "regeneration",
                Name = "Regeneración",
                Emoji = "❤️",
                Description = "Regeneras 5% HP por turno fuera de combate.",
                Type = PassiveType.Regeneration,
                Value = 5
            },
            
            new Passive
            {
                Id = "meditation_master",
                Name = "Maestro de Meditación",
                Emoji = "🧘",
                Description = "Regeneras 10% Mana por turno fuera de combate.",
                Type = PassiveType.Meditation,
                Value = 10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS DE COMBATE AVANZADO
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "bloodlust",
                Name = "Sed de Sangre",
                Emoji = "🩸",
                Description = "+2% daño por cada 10% HP perdido (max +20%).",
                Type = PassiveType.Bloodlust,
                Value = 2
            },
            
            new Passive
            {
                Id = "counter_master",
                Name = "Maestro del Contraataque",
                Emoji = "🔄",
                Description = "30% chance de contraatacar cuando defiendes.",
                Type = PassiveType.CounterAttack,
                Value = 30
            },
            
            new Passive
            {
                Id = "thorns",
                Name = "Espinas",
                Emoji = "🌵",
                Description = "Devuelves 20% del daño recibido al atacante.",
                Type = PassiveType.Thorns,
                Value = 20
            },
            
            new Passive
            {
                Id = "life_steal",
                Name = "Robo de Vida",
                Emoji = "🧛",
                Description = "Robas 15% del daño físico como HP.",
                Type = PassiveType.LifeSteal,
                Value = 15
            },
            
            new Passive
            {
                Id = "spell_vamp",
                Name = "Vampirismo Mágico",
                Emoji = "🔮",
                Description = "Robas 20% del daño mágico como HP.",
                Type = PassiveType.SpellVamp,
                Value = 20
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS DE RECURSOS/LOOT
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "treasure_hunter",
                Name = "Cazador de Tesoros",
                Emoji = "💰",
                Description = "+25% de probabilidad de encontrar loot.",
                Type = PassiveType.LootDropBonus,
                Value = 25
            },
            
            new Passive
            {
                Id = "gold_magnate",
                Name = "Magnate del Oro",
                Emoji = "🪙",
                Description = "+30% de oro obtenido en combates.",
                Type = PassiveType.GoldFindBonus,
                Value = 30
            },
            
            new Passive
            {
                Id = "fast_learner",
                Name = "Aprendiz Veloz",
                Emoji = "📚",
                Description = "+20% XP obtenido.",
                Type = PassiveType.XPBonus,
                Value = 20
            },
            
            new Passive
            {
                Id = "merchant_friend",
                Name = "Amigo del Mercader",
                Emoji = "🤝",
                Description = "-15% precio en tiendas.",
                Type = PassiveType.MerchantFriend,
                Value = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PASIVAS DE CLASES OCULTAS (otorgadas automáticamente)
            // ═══════════════════════════════════════════════════════════════
            new Passive
            {
                Id = "beast_companion",
                Name = "Compañero Bestial",
                Emoji = "🐺",
                Description = "Tu bestia domada lucha a tu lado (+20% daño total).",
                Type = PassiveType.BeastMastery,
                Value = 20
            },
            
            new Passive
            {
                Id = "beast_empathy",
                Name = "Empatía Bestial",
                Emoji = "🦊",
                Description = "Las bestias salvajes no te atacan al explorar.",
                Type = PassiveType.BeastWhisperer,
                Value = 2
            },
            
            new Passive
            {
                Id = "night_vision",
                Name = "Visión Nocturna",
                Emoji = "👁️",
                Description = "Ves perfectamente en la oscuridad.",
                Type = PassiveType.ShadowStep,
                Value = 2
            },
            
            new Passive
            {
                Id = "silent_movement",
                Name = "Movimiento Silencioso",
                Emoji = "🤫",
                Description = "No haces ruido al moverte. +30% evasión.",
                Type = PassiveType.ShadowStep,
                Value = 3
            },
            
            new Passive
            {
                Id = "holy_aura",
                Name = "Aura Sagrada",
                Emoji = "✨",
                Description = "Regeneras 5% HP por turno durante combate.",
                Type = PassiveType.Regeneration,
                Value = 5
            },
            
            new Passive
            {
                Id = "resurrection",
                Name = "Resurrección",
                Emoji = "⛪",
                Description = "Auto-revives con 50% HP una vez por combate.",
                Type = PassiveType.SecondWind,
                Value = 50
            },
            
            new Passive
            {
                Id = "lichdom",
                Name = "Lichdom",
                Emoji = "💀",
                Description = "Recibes 50% menos daño físico.",
                Type = PassiveType.NecroticTouch,
                Value = 2
            },
            
            new Passive
            {
                Id = "soul_harvest",
                Name = "Cosecha de Almas",
                Emoji = "👻",
                Description = "+20% XP de enemigos derrotados.",
                Type = PassiveType.XPBonus,
                Value = 20
            },
            
            new Passive
            {
                Id = "elemental_mastery",
                Name = "Maestría Elemental",
                Emoji = "🌊",
                Description = "Hechizos cuestan 20% menos mana.",
                Type = PassiveType.ElementalAffinity,
                Value = 2
            },
            
            new Passive
            {
                Id = "primal_force",
                Name = "Fuerza Primordial",
                Emoji = "⚡",
                Description = "+15% de daño mágico.",
                Type = PassiveType.MagicalDamageBonus,
                Value = 15
            },
            
            new Passive
            {
                Id = "flow_state",
                Name = "Estado de Flujo",
                Emoji = "🌀",
                Description = "+5% daño acumulativo por cada hit en combo.",
                Type = PassiveType.BladeDancer,
                Value = 2
            },
            
            new Passive
            {
                Id = "graceful_fighter",
                Name = "Luchador Grácil",
                Emoji = "💃",
                Description = "+20% evasión durante combos activos.",
                Type = PassiveType.BladeDancer,
                Value = 3
            }
        };
        
        public static List<Passive> GetAll() => _passives;
        
        public static Passive? GetById(string id) => _passives.FirstOrDefault(p => p.Id == id);
        
        public static List<Passive> GetByIds(List<string> ids)
        {
            return _passives.Where(p => ids.Contains(p.Id)).ToList();
        }
        
        public static List<Passive> GetUnlockedByPlayer(RpgPlayer player)
        {
            return GetByIds(player.UnlockedPassives);
        }
    }
}
