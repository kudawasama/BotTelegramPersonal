using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Lógica del sistema de misiones (Fase 9)</summary>
    public static class QuestService
    {
        // ── Aceptar misión ──────────────────────────────────────────────────
        public static (bool Success, string Message) AcceptQuest(RpgPlayer player, string questId)
        {
            var def = QuestDatabase.GetById(questId);
            if (def is null) return (false, "❌ Misión no encontrada.");

            if (player.Level < def.RequiredLevel)
                return (false, $"❌ Necesitas nivel {def.RequiredLevel} para esta misión.");

            if (player.ActiveQuests.Any(q => q.QuestId == questId))
                return (false, "⚠️ Ya tienes esta misión activa.");

            if (!def.IsRepeatable && player.CompletedQuestIds.Contains(questId))
                return (false, "✅ Ya completaste esta misión.");

            // Deep copy de los objetivos
            var playerQuest = new PlayerQuest
            {
                QuestId    = questId,
                Status     = QuestStatus.Active,
                StartedAt  = DateTime.UtcNow,
                Objectives = def.Objectives.Select(o => new QuestObjective
                {
                    Description = o.Description,
                    Type        = o.Type,
                    TargetId    = o.TargetId,
                    Required    = o.Required,
                    Current     = 0
                }).ToList()
            };

            player.ActiveQuests.Add(playerQuest);
            return (true, $"✅ Misión aceptada: **{def.Emoji} {def.Name}**");
        }

        // ── Actualizar progreso (Kill) ──────────────────────────────────────
        /// <summary>Llamar tras derrotar un enemigo. Devuelve mensajes de misión completada si los hay.</summary>
        public static List<string> UpdateKillObjective(RpgPlayer player, string enemyName, int enemyLevel = 0)
        {
            var notifications = new List<string>();
            foreach (var quest in player.ActiveQuests.ToList())
            {
                var def = QuestDatabase.GetById(quest.QuestId);
                if (def is null) continue;

                foreach (var obj in quest.Objectives.Where(o => o.Type == QuestType.Kill && !o.IsCompleted))
                {
                    bool matches = enemyName.Contains(obj.TargetId, StringComparison.OrdinalIgnoreCase)
                        || (obj.TargetId == "boss_any" && enemyLevel >= 10)
                        || obj.TargetId.Equals("any", StringComparison.OrdinalIgnoreCase);

                    if (matches)
                    {
                        obj.Current++;
                        if (obj.IsCompleted)
                            notifications.Add($"🎯 Objetivo completado: {obj.Description} ({def.Emoji} {def.Name})");
                    }
                }

                // Verificar si toda la misión está completa
                if (quest.Objectives.All(o => o.IsCompleted))
                {
                    notifications.Add($"🏆 ¡Misión lista para entregar: **{def.Emoji} {def.Name}**! Habla con {def.NPCName}.");
                }
            }
            return notifications;
        }

        // ── Actualizar progreso (Collect) ───────────────────────────────────
        /// <summary>Llamar al obtener ítems. Verifica el inventario del jugador.</summary>
        public static List<string> UpdateCollectObjective(RpgPlayer player)
        {
            var notifications = new List<string>();
            foreach (var quest in player.ActiveQuests)
            {
                var def = QuestDatabase.GetById(quest.QuestId);
                if (def is null) continue;

                foreach (var obj in quest.Objectives.Where(o => o.Type == QuestType.Collect && !o.IsCompleted))
                {
                    int count = player.Inventory.Count(i =>
                        i.Name.Equals(obj.TargetId, StringComparison.OrdinalIgnoreCase));
                    obj.Current = Math.Min(count, obj.Required);

                    if (obj.IsCompleted)
                        notifications.Add($"🎯 Objetivo completado: {obj.Description} ({def.Emoji} {def.Name})");
                }

                if (quest.Objectives.All(o => o.IsCompleted))
                    notifications.Add($"🏆 ¡Misión lista para entregar: **{def.Emoji} {def.Name}**!");
            }
            return notifications;
        }

        // ── Actualizar progreso (Craft) ─────────────────────────────────────
        /// <summary>Llamar al craftear un ítem/equipo.</summary>
        public static List<string> UpdateCraftObjective(RpgPlayer player, string recipeId)
        {
            var notifications = new List<string>();
            foreach (var quest in player.ActiveQuests)
            {
                var def = QuestDatabase.GetById(quest.QuestId);
                if (def is null) continue;

                foreach (var obj in quest.Objectives.Where(o => o.Type == QuestType.Craft && !o.IsCompleted))
                {
                    bool matches = obj.TargetId == recipeId
                        || obj.TargetId == "potion_any" && (recipeId == "pocion_mayor" || recipeId == "elixir_mana" || recipeId == "pocion_suprema" || recipeId == "tonico_fuerza");

                    if (matches)
                    {
                        obj.Current++;
                        if (obj.IsCompleted)
                            notifications.Add($"🎯 Objetivo crafteo completado ({def.Emoji} {def.Name})");
                    }
                }

                if (quest.Objectives.All(o => o.IsCompleted))
                    notifications.Add($"🏆 ¡Misión lista para entregar: **{def.Emoji} {def.Name}**!");
            }
            return notifications;
        }

        // ── Actualizar progreso (Dungeon) ───────────────────────────────────
        public static List<string> UpdateExploreObjective(RpgPlayer player, string dungeonId)
        {
            var notifications = new List<string>();
            foreach (var quest in player.ActiveQuests)
            {
                var def = QuestDatabase.GetById(quest.QuestId);
                if (def is null) continue;

                foreach (var obj in quest.Objectives.Where(o => o.Type == QuestType.Explore && !o.IsCompleted))
                {
                    bool matches = obj.TargetId == dungeonId || obj.TargetId == "dungeon_any";
                    if (matches)
                    {
                        obj.Current++;
                        if (obj.IsCompleted)
                            notifications.Add($"🎯 Objetivo exploración completado ({def.Emoji} {def.Name})");
                    }
                }

                if (quest.Objectives.All(o => o.IsCompleted))
                    notifications.Add($"🏆 ¡Misión lista para entregar: **{def.Emoji} {def.Name}**!");
            }
            return notifications;
        }

        // ── Verificar si una misión es entregable ───────────────────────────
        public static bool IsCompletable(RpgPlayer player, string questId)
        {
            var pq = player.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
            return pq != null && pq.Objectives.All(o => o.IsCompleted);
        }

        // ── Completar misión y dar recompensas ──────────────────────────────
        public static (bool Success, string Message) CompleteQuest(RpgPlayer player, string questId)
        {
            var pq = player.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
            if (pq is null) return (false, "❌ No tienes esa misión activa.");

            if (!pq.Objectives.All(o => o.IsCompleted))
                return (false, "⚠️ Aún no has completado todos los objetivos.");

            var def = QuestDatabase.GetById(questId);
            if (def is null) return (false, "❌ Misión no encontrada.");

            // Dar recompensas
            player.Gold += def.Reward.GoldReward;
            player.XP   += def.Reward.XPReward;

            var rewardLines = new List<string>
            {
                $"+{def.Reward.GoldReward} 💰 Oro",
                $"+{def.Reward.XPReward} ✨ XP"
            };

            if (!string.IsNullOrEmpty(def.Reward.ItemRewardName))
            {
                var bonus = new RpgItem
                {
                    Name        = def.Reward.ItemRewardName,
                    Emoji       = "🎁",
                    Description = "Recompensa de misión",
                    Type        = ItemType.Consumable,
                    Rarity      = ItemRarity.Uncommon,
                    HPRestore   = 100,
                    Value       = 100
                };
                player.Inventory.Add(bonus);
                rewardLines.Add($"+1 🎁 {def.Reward.ItemRewardName}");
            }

            if (!string.IsNullOrEmpty(def.Reward.EquipId))
            {
                var equip = EquipmentDatabase.GetById(def.Reward.EquipId);
                if (equip != null)
                {
                    player.EquipmentInventory.Add(equip);
                    rewardLines.Add($"+1 ⚔️ {equip.Name}");
                }
            }

            // Level up check
            int xpNeeded = player.Level * 100;
            string lvlMsg = "";
            while (player.XP >= xpNeeded)
            {
                player.XP    -= xpNeeded;
                player.Level++;
                player.MaxHP  += 20;
                player.MaxMana += 10;
                player.HP =  player.MaxHP;
                player.Mana = player.MaxMana;
                xpNeeded = player.Level * 100;
                lvlMsg = $"\n🎉 ¡Subiste al nivel **{player.Level}**!";
            }

            // Mover misión a completadas
            pq.Status      = QuestStatus.Completed;
            pq.CompletedAt = DateTime.UtcNow;
            player.ActiveQuests.Remove(pq);
            if (!player.CompletedQuestIds.Contains(questId))
                player.CompletedQuestIds.Add(questId);

            return (true,
                $"🏆 **¡Misión completada: {def.Emoji} {def.Name}!**\n\n"
                + $"**Recompensas:**\n• {string.Join("\n• ", rewardLines)}{lvlMsg}");
        }
    }
}
