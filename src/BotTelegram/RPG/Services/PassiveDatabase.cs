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
            },
            
            // ═══════════════════════════════════════════════════════════════
            // FASE 3 - NUEVAS PASIVAS (40 NUEVAS)
            // ═══════════════════════════════════════════════════════════════
            
            // FORTRESS KNIGHT Passives
            new Passive
            {
                Id = "unbreakable_defense",
                Name = "Defensa Inquebrantable",
                Emoji = "🛡️",
                Description = "+50% probabilidad de bloqueo y +30 de Defensa Física.",
                Type = PassiveType.UnbreakableDefense,
                Value = 50
            },
            
            new Passive
            {
                Id = "damage_reflection",
                Name = "Reflejo de Daño",
                Emoji = "🔄",
                Description = "Reflejas el 25% del daño bloqueado al atacante.",
                Type = PassiveType.DamageReflection,
                Value = 25
            },
            
            new Passive
            {
                Id = "shield_mastery",
                Name = "Maestría con Escudos",
                Emoji = "🛡️",
                Description = "Los escudos otorgan +50% de stats adicionales.",
                Type = PassiveType.ShieldMastery,
                Value = 50
            },
            
            // IMMOVABLE MOUNTAIN Passives
            new Passive
            {
                Id = "stone_skin",
                Name = "Piel de Piedra",
                Emoji = "⛰️",
                Description = "Reducción de daño fija de 15 puntos (aplicado antes de DEF).",
                Type = PassiveType.StoneSkin,
                Value = 15
            },
            
            new Passive
            {
                Id = "last_stand",
                Name = "Última Resistencia",
                Emoji = "💪",
                Description = "Al llegar a 1 HP, recuperas 40% HP una vez por combate.",
                Type = PassiveType.LastStand,
                Value = 40
            },
            
            new Passive
            {
                Id = "immovable",
                Name = "Inamovible",
                Emoji = "⚓",
                Description = "Inmune a Stun, Knockback y efectos de movimiento forzado.",
                Type = PassiveType.Immovable,
                Value = 1
            },
            
            // BERSERKER BLOOD RAGE Passives
            new Passive
            {
                Id = "blood_frenzy",
                Name = "Frenesí Sanguinario",
                Emoji = "🩸",
                Description = "+5% de daño por cada 10% de HP perdido (máximo +50%).",
                Type = PassiveType.BloodFrenzy,
                Value = 5
            },
            
            new Passive
            {
                Id = "reckless_abandon",
                Name = "Abandono Imprudente",
                Emoji = "😈",
                Description = "+50% de daño pero -30% de Defensa.",
                Type = PassiveType.RecklessAbandon,
                Value = 50
            },
            
            new Passive
            {
                Id = "killing_spree",
                Name = "Racha Asesina",
                Emoji = "💀",
                Description = "Cada kill otorga +10% daño por 3 turnos (stackeable x5).",
                Type = PassiveType.KillingSpree,
                Value = 10
            },
            
            // ARCANE SIPHONER Passives
            new Passive
            {
                Id = "arcane_overflow",
                Name = "Desbordamiento Arcano",
                Emoji = "🔮",
                Description = "Hechizos que exceden tu MaxMana hacen +50% daño.",
                Type = PassiveType.ArcaneOverflow,
                Value = 50
            },
            
            new Passive
            {
                Id = "mana_burn",
                Name = "Quema de Mana",
                Emoji = "🔥",
                Description = "Puedes castear sin mana consumiendo HP (2 HP = 1 Mana).",
                Type = PassiveType.ManaBurn,
                Value = 2
            },
            
            new Passive
            {
                Id = "spell_amplification",
                Name = "Amplificación Mágica",
                Emoji = "✨",
                Description = "+60% daño mágico pero -30% Defensa Física.",
                Type = PassiveType.SpellAmplification,
                Value = 60
            },
            
            // LIFE WEAVER Passives
            new Passive
            {
                Id = "divine_touch",
                Name = "Toque Divino",
                Emoji = "🌸",
                Description = "Tus curaciones son +100% más efectivas.",
                Type = PassiveType.DivineTouch,
                Value = 100
            },
            
            new Passive
            {
                Id = "regeneration_aura",
                Name = "Aura de Regeneración",
                Emoji = "💚",
                Description = "Recuperas 10% HP cada turno automáticamente.",
                Type = PassiveType.RegenerationAura,
                Value = 10
            },
            
            new Passive
            {
                Id = "life_link",
                Name = "Vínculo Vital",
                Emoji = "❤️",
                Description = "Al morir, revives automáticamente con 60% HP (Cooldown: 1 por combate).",
                Type = PassiveType.LifeLink,
                Value = 60
            },
            
            // PUPPET MASTER Passives
            new Passive
            {
                Id = "master_manipulator",
                Name = "Maestro Manipulador",
                Emoji = "🎭",
                Description = "Efectos de control mental duran +30% más tiempo.",
                Type = PassiveType.MasterManipulator,
                Value = 30
            },
            
            new Passive
            {
                Id = "puppet_strings",
                Name = "Hilos de Títere",
                Emoji = "🎎",
                Description = "Enemigos controlados hacen +50% daño.",
                Type = PassiveType.PuppetStrings,
                Value = 50
            },
            
            new Passive
            {
                Id = "mind_immunity",
                Name = "Inmunidad Mental",
                Emoji = "🧠",
                Description = "Inmune a control mental, confusión y efectos de charme.",
                Type = PassiveType.MindImmunity,
                Value = 1
            },
            
            // TIME BENDER Passives
            new Passive
            {
                Id = "temporal_flux",
                Name = "Flujo Temporal",
                Emoji = "⏰",
                Description = "+50% velocidad base. Actúas más frecuentemente.",
                Type = PassiveType.TemporalFlux,
                Value = 50
            },
            
            new Passive
            {
                Id = "foresight",
                Name = "Previsión",
                Emoji = "👁️",
                Description = "Ves el próximo movimiento del enemigo antes de que ataque.",
                Type = PassiveType.Foresight,
                Value = 1
            },
            
            new Passive
            {
                Id = "time_loop",
                Name = "Bucle Temporal",
                Emoji = "🔄",
                Description = "10% de probabilidad de repetir tu última acción gratis.",
                Type = PassiveType.TimeLoop,
                Value = 10
            },
            
            // ELEMENTAL OVERLORD Passives
            new Passive
            {
                Id = "elemental_fusion",
                Name = "Fusión Elemental",
                Emoji = "🌊🔥",
                Description = "Tus hechizos combinan automáticamente 2 elementos.",
                Type = PassiveType.ElementalFusion,
                Value = 1
            },
            
            new Passive
            {
                Id = "elemental_immunity",
                Name = "Inmunidad Elemental",
                Emoji = "❄️⚡",
                Description = "Completamente inmune a daño de fuego, agua, tierra y aire.",
                Type = PassiveType.ElementalImmunity,
                Value = 100
            },
            
            new Passive
            {
                Id = "primal_force_upgraded",
                Name = "Fuerza Primordial Superior",
                Emoji = "⚡",
                Description = "+80% de daño elemental (mejora de Fuerza Primordial).",
                Type = PassiveType.PrimalForceUpgraded,
                Value = 80
            },
            
            // BEAST LORD Passives
            new Passive
            {
                Id = "beast_army",
                Name = "Ejército Bestial",
                Emoji = "🐲",
                Description = "+2 slots de mascota activa (total 3 mascotas simultáneas).",
                Type = PassiveType.BeastArmy,
                Value = 2
            },
            
            new Passive
            {
                Id = "alpha_dominance",
                Name = "Dominio Alfa",
                Emoji = "👑",
                Description = "Tus mascotas hacen +100% de daño.",
                Type = PassiveType.AlphaDominance,
                Value = 100
            },
            
            new Passive
            {
                Id = "beast_fusion",
                Name = "Fusión Bestial",
                Emoji = "🦁",
                Description = "Puedes fusionar 2 mascotas temporalmente en una criatura poderosa.",
                Type = PassiveType.BeastFusion,
                Value = 1
            },
            
            // LICH KING Passives
            new Passive
            {
                Id = "undead_mastery",
                Name = "Maestría No-muerta",
                Emoji = "💀👑",
                Description = "+3 slots de minion no-muerto (total 5 minions simultáneos).",
                Type = PassiveType.UndeadMastery,
                Value = 3
            },
            
            new Passive
            {
                Id = "death_aura",
                Name = "Aura de Muerte",
                Emoji = "☠️",
                Description = "Los enemigos pierden 5% de su MaxHP cada turno.",
                Type = PassiveType.DeathAura,
                Value = 5
            },
            
            new Passive
            {
                Id = "phylactery",
                Name = "Filacteria",
                Emoji = "💎",
                Description = "Si mueres con >3 minions, revives con 50% HP.",
                Type = PassiveType.Phylactery,
                Value = 50
            },
            
            // VOID SUMMONER Passives
            new Passive
            {
                Id = "eldritch_pact",
                Name = "Pacto Eldritch",
                Emoji = "👁️",
                Description = "Tus invocaciones cuestan HP en vez de mana.",
                Type = PassiveType.EldritchPact,
                Value = 1
            },
            
            new Passive
            {
                Id = "void_touched",
                Name = "Tocado por el Vacío",
                Emoji = "🌀",
                Description = "+100% daño void pero -50% cordura (sanity).",
                Type = PassiveType.VoidTouched,
                Value = 100
            },
            
            new Passive
            {
                Id = "beyond_death",
                Name = "Más Allá de la Muerte",
                Emoji = "👻",
                Description = "Si mueres, revives como aberración (1 vez por día).",
                Type = PassiveType.BeyondDeath,
                Value = 1
            },
            
            // BONUSES ADICIONALES (10 extra para completar 40)
            new Passive
            {
                Id = "fortress",
                Name = "Fortaleza",
                Emoji = "🏰",
                Description = "Completamente inmune a knockback y empujones.",
                Type = PassiveType.Fortress,
                Value = 1
            },
            
            new Passive
            {
                Id = "mana_regeneration",
                Name = "Regeneración de Mana",
                Emoji = "💙",
                Description = "+5% de mana por turno durante combate.",
                Type = PassiveType.ManaRegeneration,
                Value = 5
            },
            
            new Passive
            {
                Id = "blood_pact",
                Name = "Pacto de Sangre",
                Emoji = "🩸",
                Description = "Convierte HP en ATK extra (2 HP = 1 ATK).",
                Type = PassiveType.BloodPact,
                Value = 2
            },
            
            new Passive
            {
                Id = "void_shield",
                Name = "Escudo del Vacío",
                Emoji = "🌌",
                Description = "+30% resistencia a daño void y oscuro.",
                Type = PassiveType.VoidShield,
                Value = 30
            },
            
            new Passive
            {
                Id = "soul_siphon",
                Name = "Sifón de Almas",
                Emoji = "👻",
                Description = "Drena 10% del MaxHP del enemigo al inicio del combate.",
                Type = PassiveType.SoulSiphon,
                Value = 10
            },
            
            new Passive
            {
                Id = "divine_intervention",
                Name = "Intervención Divina",
                Emoji = "🙏",
                Description = "Dios te salva de la muerte 1 vez por combate (quedas con 1 HP).",
                Type = PassiveType.DivineIntervention,
                Value = 1
            },
            
            new Passive
            {
                Id = "time_manipulation",
                Name = "Manipulación Temporal",
                Emoji = "⏱️",
                Description = "Todos tus cooldowns se reducen 20%.",
                Type = PassiveType.TimeManipulation,
                Value = 20
            },
            
            new Passive
            {
                Id = "elemental_resonance",
                Name = "Resonancia Elemental",
                Emoji = "🌈",
                Description = "Hechizos elementales tienen 15% de aplicar status (burn/freeze/shock/poison).",
                Type = PassiveType.ElementalResonance,
                Value = 15
            },
            
            new Passive
            {
                Id = "beast_bond",
                Name = "Vínculo Bestial",
                Emoji = "🤝",
                Description = "+30% bond ganado con mascotas.",
                Type = PassiveType.BeastBond,
                Value = 30
            },
            
            new Passive
            {
                Id = "unholy_regeneration",
                Name = "Regeneración Profana",
                Emoji = "💀",
                Description = "Regeneras 5% de tu MaxHP por cada enemigo que mates.",
                Type = PassiveType.UnholyRegeneration,
                Value = 5
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
