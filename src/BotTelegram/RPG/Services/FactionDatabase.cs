using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de facciones del mundo
    /// </summary>
    public static class FactionDatabase
    {
        private static readonly List<Faction> _factions = new();
        
        static FactionDatabase()
        {
            InitializeFactions();
        }
        
        public static List<Faction> GetAllFactions() => _factions;
        public static Faction? GetFaction(string factionId) => _factions.FirstOrDefault(f => f.Id == factionId);
        
        /// <summary>
        /// Obtiene el tier de reputación según el valor numérico
        /// </summary>
        public static FactionTier GetTierFromReputation(int reputation)
        {
            return reputation switch
            {
                <= -3000 => FactionTier.Hated,
                <= -1000 => FactionTier.Hostile,
                <= 1000 => FactionTier.Neutral,
                <= 3000 => FactionTier.Friendly,
                <= 6000 => FactionTier.Honored,
                <= 10000 => FactionTier.Revered,
                _ => FactionTier.Exalted
            };
        }
        
        /// <summary>
        /// Obtiene el nombre localizado de un tier
        /// </summary>
        public static string GetTierName(FactionTier tier)
        {
            return tier switch
            {
                FactionTier.Hated => "Odiado",
                FactionTier.Hostile => "Hostil",
                FactionTier.Neutral => "Neutral",
                FactionTier.Friendly => "Amistoso",
                FactionTier.Honored => "Honorable",
                FactionTier.Revered => "Venerado",
                FactionTier.Exalted => "Exaltado",
                _ => "Desconocido"
            };
        }
        
        /// <summary>
        /// Obtiene el emoji de un tier
        /// </summary>
        public static string GetTierEmoji(FactionTier tier)
        {
            return tier switch
            {
                FactionTier.Hated => "💀",
                FactionTier.Hostile => "⚔️",
                FactionTier.Neutral => "⚪",
                FactionTier.Friendly => "🟢",
                FactionTier.Honored => "🔵",
                FactionTier.Revered => "🟣",
                FactionTier.Exalted => "🟡",
                _ => "❓"
            };
        }
        
        private static void InitializeFactions()
        {
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 1: GUARDIANES DEL AMANECER (Islas del Amanecer)
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "guardianes_amanecer",
                Name = "Guardianes del Amanecer",
                Emoji = "🏝️",
                Description = "Protectores pacíficos de las islas iniciales. Ayudan a nuevos aventureros.",
                PrimaryRegionId = "islas_amanecer",
                EnemyFactionId = "culto_abismo",
                AlliedFactionIds = new() { "viajeros_salvajes", "orden_llama" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 100, XPReward = 500, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 300, XPReward = 1500, UnlockedRecipeId = "guardian_shield", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 1000, XPReward = 5000, UnlockedZoneId = "fortaleza_amanecer", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 10000, UnlockedTitle = "Campeón del Amanecer", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 2: VIAJEROS DE LAS TIERRAS SALVAJES
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "viajeros_salvajes",
                Name = "Viajeros de las Tierras Salvajes",
                Emoji = "🌲",
                Description = "Exploradores y cazadores que dominan los bosques. Enemigos de bandidos.",
                PrimaryRegionId = "tierras_salvajes",
                EnemyFactionId = "hermandad_sombras",
                AlliedFactionIds = new() { "guardianes_amanecer", "druidas_eternos" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 150, XPReward = 750, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 400, XPReward = 2000, UnlockedRecipeId = "hunter_bow", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 1500, XPReward = 7000, UnlockedQuestId = "quest_beast_king", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 15000, UnlockedTitle = "Maestro Cazador", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 3: ORDEN DE LA LLAMA ETERNA (Montañas de Ceniza)
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "orden_llama",
                Name = "Orden de la Llama Eterna",
                Emoji = "🔥",
                Description = "Caballeros elementales que protegen las montañas de cultistas oscuros.",
                PrimaryRegionId = "montanas_ceniza",
                EnemyFactionId = "culto_abismo",
                AlliedFactionIds = new() { "guardianes_amanecer", "legion_hielo" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 250, XPReward = 1200, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 600, XPReward = 3500, UnlockedRecipeId = "flame_blade", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 2000, XPReward = 10000, UnlockedZoneId = "forja_dragon", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 20000, UnlockedTitle = "Campeón de la Llama", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 4: CUSTODIOS DEL DESIERTO (Desierto Olvidado)
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "custodios_desierto",
                Name = "Custodios del Desierto",
                Emoji = "🏜️",
                Description = "Guardias de tumbas antiguas. Luchan contra necromantes que profanan las arenas.",
                PrimaryRegionId = "desierto_olvidado",
                EnemyFactionId = "orden_muertos",
                AlliedFactionIds = new() { "orden_llama" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 400, XPReward = 2000, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 800, XPReward = 5000, UnlockedRecipeId = "desert_armor", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 3000, XPReward = 15000, UnlockedQuestId = "quest_pharaoh_curse", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 30000, UnlockedTitle = "Guardián de los Ancestros", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 5: DRUIDAS DEL BOSQUE ETERNO
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "druidas_eternos",
                Name = "Druidas del Bosque Eterno",
                Emoji = "🌳",
                Description = "Protectores místicos de la naturaleza antigua. Hostiles a destructores del bosque.",
                PrimaryRegionId = "bosque_eterno",
                EnemyFactionId = "hermandad_sombras",
                AlliedFactionIds = new() { "viajeros_salvajes" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 600, XPReward = 3000, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 1200, XPReward = 7000, UnlockedRecipeId = "nature_staff", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 4000, XPReward = 20000, UnlockedZoneId = "arbol_ancestral", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 40000, UnlockedTitle = "Archidruida", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 6: LEGIÓN DEL HIELO ETERNO (Tierras Heladas)
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "legion_hielo",
                Name = "Legión del Hielo Eterno",
                Emoji = "❄️",
                Description = "Guerreros de las tierras heladas. Protegen el norte de invasores y gigantes.",
                PrimaryRegionId = "tierras_heladas",
                EnemyFactionId = "culto_abismo",
                AlliedFactionIds = new() { "orden_llama", "centinelas_cielo" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 800, XPReward = 4000, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 1600, XPReward = 10000, UnlockedRecipeId = "frost_hammer", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 5000, XPReward = 25000, UnlockedQuestId = "quest_winter_king", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 50000, UnlockedTitle = "Señor del Invierno", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 7: CULTO DEL ABISMO (El Abismo) - ENEMIGA
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "culto_abismo",
                Name = "Culto del Abismo",
                Emoji = "🕳️",
                Description = "Cultistas demónicos que buscan corromper el mundo. Enemigos de todas las facciones de luz.",
                PrimaryRegionId = "abismo",
                EnemyFactionId = "centinelas_cielo",
                AlliedFactionIds = new() { "hermandad_sombras", "orden_muertos" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 1000, XPReward = 5000, UnlockedRecipeId = "shadow_dagger", ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 2000, XPReward = 12000, UnlockedZoneId = "ciudadela_demonio", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 6000, XPReward = 30000, UnlockedQuestId = "quest_demon_lord", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 60000, UnlockedTitle = "Heraldo del Abismo", ShopDiscountPercent = 20 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIÓN 8: CENTINELAS DEL CIELO ETÉREO
            // ═══════════════════════════════════════════════════════════════
            _factions.Add(new Faction
            {
                Id = "centinelas_cielo",
                Name = "Centinelas del Cielo Etéreo",
                Emoji = "☁️",
                Description = "Ángeles caídos y dragones sabios. Guardianes del equilibrio entre luz y oscuridad.",
                PrimaryRegionId = "cielo_etereo",
                EnemyFactionId = "culto_abismo",
                AlliedFactionIds = new() { "legion_hielo", "druidas_eternos" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 1500, XPReward = 8000, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 3000, XPReward = 20000, UnlockedRecipeId = "celestial_sword", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 10000, XPReward = 50000, UnlockedZoneId = "trono_supremo", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 100000, UnlockedTitle = "Ascendido", ShopDiscountPercent = 25 } }
                }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // FACCIONES NEUTRALES/OSCURAS (no principales, para quests futuras)
            // ═══════════════════════════════════════════════════════════════
            
            _factions.Add(new Faction
            {
                Id = "hermandad_sombras",
                Name = "Hermandad de Sombras",
                Emoji = "🗡️",
                Description = "Asesinos y ladrones que operan en las sombras. Enemigos de guardias y druidas.",
                PrimaryRegionId = "tierras_salvajes",
                EnemyFactionId = "viajeros_salvajes",
                AlliedFactionIds = new() { "culto_abismo" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 500, XPReward = 2500, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 1000, XPReward = 6000, UnlockedRecipeId = "assassin_blade", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 3500, XPReward = 17000, UnlockedQuestId = "quest_shadow_guild", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 35000, UnlockedTitle = "Maestro de Sombras", ShopDiscountPercent = 20 } }
                }
            });
            
            _factions.Add(new Faction
            {
                Id = "orden_muertos",
                Name = "Orden de los Muertos",
                Emoji = "💀",
                Description = "Necromantes que dominan los muertos vivientes del desierto.",
                PrimaryRegionId = "desierto_olvidado",
                EnemyFactionId = "custodios_desierto",
                AlliedFactionIds = new() { "culto_abismo" },
                Rewards = new()
                {
                    { FactionTier.Friendly, new FactionReward { GoldReward = 700, XPReward = 3500, ShopDiscountPercent = 5 } },
                    { FactionTier.Honored, new FactionReward { GoldReward = 1400, XPReward = 8000, UnlockedRecipeId = "necro_staff", ShopDiscountPercent = 10 } },
                    { FactionTier.Revered, new FactionReward { GoldReward = 4500, XPReward = 22000, UnlockedZoneId = "tumba_lich", ShopDiscountPercent = 15 } },
                    { FactionTier.Exalted, new FactionReward { XPReward = 45000, UnlockedTitle = "Señor de los Muertos", ShopDiscountPercent = 20 } }
                }
            });
        }
    }
}
