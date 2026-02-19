using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Base de datos de misiones/quests (Fase 9)</summary>
    public static class QuestDatabase
    {
        private static readonly List<QuestDefinition> _quests = new()
        {
            // ── Misiones de Eliminación ─────────────────────────────────────
            new QuestDefinition
            {
                Id           = "quest_wolf_hunt",
                Name         = "Cacería de Lobos",
                Emoji        = "🐺",
                Description  = "Los lobos plaga los alrededores de la ciudad. Elimínalos.",
                NPCName      = "Guardia Ryon",
                RequiredLevel = 1,
                IsRepeatable  = true,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Eliminar lobos",
                        Type        = QuestType.Kill,
                        TargetId    = "lobo",
                        Required    = 5,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 200, XPReward = 150, ReputationReward = 25, FactionId = "guardianes_amanecer" },
                FactionId = "guardianes_amanecer"
            },
            new QuestDefinition
            {
                Id           = "quest_goblin_raid",
                Name         = "Purgar la Guarida Goblin",
                Emoji        = "👺",
                Description  = "Los goblins atacan las aldeas cercanas. Ponles fin.",
                NPCName      = "Comandante Sera",
                RequiredLevel = 3,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Eliminar goblins",
                        Type        = QuestType.Kill,
                        TargetId    = "goblin",
                        Required    = 10,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 350, XPReward = 300, ReputationReward = 50, FactionId = "guardianes_amanecer" },
                FactionId = "guardianes_amanecer"
            },
            new QuestDefinition
            {
                Id           = "quest_dragon_champion",
                Name         = "Cazador de Dragones",
                Emoji        = "🐉",
                Description  = "Un dragón ha sido avistado en las montañas. ¡Demuestra tu valor!",
                NPCName      = "Oráculo Ziven",
                RequiredLevel = 8,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Derrotar al dragón",
                        Type        = QuestType.Kill,
                        TargetId    = "dragón",
                        Required    = 1,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 800, XPReward = 700, EquipId = "weapon_runic_blade", ReputationReward = 150, FactionId = "orden_llama" },
                FactionId = "orden_llama",
                RequiredReputation = 1000
            },
            new QuestDefinition
            {
                Id           = "quest_boss_slay",
                Name         = "El Último Jefe",
                Emoji        = "💀",
                Description  = "Una criatura de nivel 10 o superior amenaza el reino.",
                NPCName      = "Rey Aldran",
                RequiredLevel = 10,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Derrotar un enemigo de nivel 10+",
                        Type        = QuestType.Kill,
                        TargetId    = "boss_any",
                        Required    = 1,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 1000, XPReward = 900, EquipId = "armor_shadow_cloak", ReputationReward = 100, FactionId = "culto_abismo" },
                FactionId = "culto_abismo"  // Fase 12: Combate contra el mal
            },

            // ── Misiones de Recolección ─────────────────────────────────────
            new QuestDefinition
            {
                Id           = "quest_herb_gather",
                Name         = "Recolección de Esencias",
                Emoji        = "✨",
                Description  = "El alquimista necesita esencias mágicas para sus experimentos.",
                NPCName      = "Alquimista Mira",
                RequiredLevel = 1,
                IsRepeatable  = true,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Recolectar Esencias Mágicas",
                        Type        = QuestType.Collect,
                        TargetId    = "Esencia Mágica",
                        Required    = 3,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 150, XPReward = 100, ItemRewardName = "Poción de Vida", ReputationReward = 15, FactionId = "druidas_eternos" },
                FactionId = "druidas_eternos"  // Fase 12: Recolección de naturaleza
            },
            new QuestDefinition
            {
                Id           = "quest_crystal_mine",
                Name         = "La Mina de Cristal",
                Emoji        = "🔷",
                Description  = "El herrero necesita fragmentos de cristal para forjar armas.",
                NPCName      = "Herrero Boran",
                RequiredLevel = 2,
                IsRepeatable  = true,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Recolectar Fragmentos de Cristal",
                        Type        = QuestType.Collect,
                        TargetId    = "Fragmento de Cristal",
                        Required    = 5,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 180, XPReward = 120, ReputationReward = 20, FactionId = "viajeros_salvajes" },
                FactionId = "viajeros_salvajes"  // Fase 12: Exploradores recolectores
            },
            new QuestDefinition
            {
                Id           = "quest_rune_search",
                Name         = "Runas Perdidas",
                Emoji        = "🔶",
                Description  = "Las runas antiguas contienen poder arcano. Consíguelas.",
                NPCName      = "Sabio Elvan",
                RequiredLevel = 4,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Recolectar Runas Antiguas",
                        Type        = QuestType.Collect,
                        TargetId    = "Runa Antigua",
                        Required    = 3,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 300, XPReward = 250, ItemRewardName = "Elixir de Maná", ReputationReward = 40, FactionId = "custodios_desierto" },
                FactionId = "custodios_desierto"  // Fase 12: Guardianes del conocimiento antiguo
            },

            // ── Misiones de Crafteo ─────────────────────────────────────────
            new QuestDefinition
            {
                Id           = "quest_craft_potion",
                Name         = "El Arte del Alquimista",
                Emoji        = "⚗️",
                Description  = "Demuestra tu habilidad con la alquimia crafteando una poción.",
                NPCName      = "Maestra Alquimista",
                RequiredLevel = 3,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Craftear cualquier poción",
                        Type        = QuestType.Craft,
                        TargetId    = "potion_any",
                        Required    = 1,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 250, XPReward = 200, ReputationReward = 30, FactionId = "guardianes_amanecer" },
                FactionId = "guardianes_amanecer"  // Fase 12: Protectores que ayudan
            },

            // ── Misiones de Mazmorra ────────────────────────────────────────
            new QuestDefinition
            {
                Id           = "quest_dungeon_run",
                Name         = "Las Catacumbas",
                Emoji        = "🏚️",
                Description  = "Completa al menos una mazmorra para demostrar tu valor.",
                NPCName      = "Explorador Dax",
                RequiredLevel = 5,
                IsRepeatable  = false,
                Objectives   = new()
                {
                    new QuestObjective
                    {
                        Description = "Completar una mazmorra",
                        Type        = QuestType.Explore,
                        TargetId    = "dungeon_any",
                        Required    = 1,
                        Current     = 0
                    }
                },
                Reward = new QuestReward { GoldReward = 600, XPReward = 500, EquipId = "armor_reinforced", ReputationReward = 80, FactionId = "legion_hielo" },
                FactionId = "legion_hielo"  // Fase 12: Guerreros valientes
            },
        };

        public static IReadOnlyList<QuestDefinition> AllQuests => _quests;

        public static QuestDefinition? GetById(string id) =>
            _quests.FirstOrDefault(q => q.Id == id);

        public static List<QuestDefinition> GetAvailableFor(RpgPlayer player) =>
            _quests
                .Where(q => q.RequiredLevel <= player.Level)
                .Where(q => q.IsRepeatable || !player.CompletedQuestIds.Contains(q.Id))
                .Where(q => !player.ActiveQuests.Any(aq => aq.QuestId == q.Id))
                .ToList();
    }
}
