using System.Text;
using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /misiones — Sistema de Misiones/Quests (Fase 9).
    /// Callbacks: quest_menu, quest_view:{id}, quest_accept:{id}, quest_complete:{id}
    /// </summary>
    public class QuestCommand
    {
        // ── Punto de entrada ────────────────────────────────────────────────
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var rpgService = new RpgService();
            var player = rpgService.GetPlayer(chatId);
            if (player is null)
            {
                await bot.SendMessage(chatId, "❌ No tienes personaje. Usa /rpg para crear uno.", cancellationToken: ct);
                return;
            }
            await ShowQuestMenu(bot, chatId, player, ct);
        }

        // ── Menú principal de Misiones ──────────────────────────────────────
        public static async Task ShowQuestMenu(ITelegramBotClient bot, long chatId, RpgPlayer player,
            CancellationToken ct, int? editMessageId = null)
        {
            var available = QuestDatabase.GetAvailableFor(player);
            var active    = player.ActiveQuests;

            var sb = new StringBuilder();
            sb.AppendLine("🏛️ **TABLÓN DE MISIONES**");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"👤 Nivel: **{player.Level}** | ✅ Completadas: **{player.CompletedQuestIds.Count}**");
            sb.AppendLine();

            var rows = new List<InlineKeyboardButton[]>();

            // Misiones activas
            if (active.Any())
            {
                sb.AppendLine("⚡ **MISIONES ACTIVAS:**");
                foreach (var pq in active)
                {
                    var def = QuestDatabase.GetById(pq.QuestId);
                    if (def is null) continue;

                    int completedObj = pq.Objectives.Count(o => o.IsCompleted);
                    bool allDone = pq.Objectives.All(o => o.IsCompleted);
                    string statusIcon = allDone ? "🏆" : "⚡";
                    string progressBar = $"{completedObj}/{pq.Objectives.Count} obj";

                    sb.AppendLine($"{statusIcon} **{def.Emoji} {def.Name}** [{progressBar}]");
                    var btnLabel = allDone ? $"🏆 Entregar: {def.Name}" : $"📋 Ver: {def.Name}";
                    rows.Add(new[] { InlineKeyboardButton.WithCallbackData(btnLabel, $"quest_view:{def.Id}") });
                }
                sb.AppendLine();
            }

            // Misiones disponibles
            if (available.Any())
            {
                sb.AppendLine("📋 **MISIONES DISPONIBLES:**");
                foreach (var def in available.Take(5))
                {
                    sb.AppendLine($"• {def.Emoji} **{def.Name}** (Lv{def.RequiredLevel}+) — {def.NPCName}");
                    rows.Add(new[] { InlineKeyboardButton.WithCallbackData($"📋 {def.Emoji} {def.Name}", $"quest_view:{def.Id}") });
                }
            }
            else if (!active.Any())
            {
                sb.AppendLine("_(No hay misiones disponibles para tu nivel actual)_");
            }

            // Misiones completadas
            if (player.CompletedQuestIds.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"🏅 Completadas: {player.CompletedQuestIds.Count} misiones");
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Ciudad", "rpg_menu_city") });

            var markup = new InlineKeyboardMarkup(rows);
            await SendOrEdit(bot, chatId, sb.ToString(), markup, editMessageId, ct);
        }

        // ── Detalle de misión ────────────────────────────────────────────────
        public static async Task ShowQuestDetail(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string questId, CancellationToken ct, int? editMessageId = null)
        {
            var def = QuestDatabase.GetById(questId);
            if (def is null)
            {
                await bot.SendMessage(chatId, "❌ Misión no encontrada.", cancellationToken: ct);
                return;
            }

            var activeQuest = player.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
            bool isActive    = activeQuest != null;
            bool isCompleted = player.CompletedQuestIds.Contains(questId);
            bool allDone     = isActive && activeQuest!.Objectives.All(o => o.IsCompleted);

            var sb = new StringBuilder();
            sb.AppendLine($"{def.Emoji} **{def.Name}**");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"🗣️ NPC: {def.NPCName}");
            sb.AppendLine($"📊 Nivel mínimo: {def.RequiredLevel}");
            sb.AppendLine($"_{def.Description}_");
            sb.AppendLine();

            // Objetivos
            sb.AppendLine("**Objetivos:**");
            if (isActive)
            {
                foreach (var obj in activeQuest!.Objectives)
                {
                    string check = obj.IsCompleted ? "✅" : "🔸";
                    string progress = obj.Type != QuestType.Collect ? $" ({obj.Current}/{obj.Required})" : "";
                    sb.AppendLine($"{check} {obj.Description}{progress}");
                }
            }
            else
            {
                foreach (var obj in def.Objectives)
                    sb.AppendLine($"🔸 {obj.Description} (×{obj.Required})");
            }

            // Recompensa
            sb.AppendLine();
            sb.AppendLine("**Recompensa:**");
            sb.AppendLine($"• 💰 {def.Reward.GoldReward} oro");
            sb.AppendLine($"• ✨ {def.Reward.XPReward} XP");
            if (!string.IsNullOrEmpty(def.Reward.ItemRewardName))
                sb.AppendLine($"• 🎁 {def.Reward.ItemRewardName}");
            if (!string.IsNullOrEmpty(def.Reward.EquipId))
            {
                var equip = EquipmentDatabase.GetById(def.Reward.EquipId);
                if (equip != null)
                    sb.AppendLine($"• ⚔️ {equip.Name} ({equip.Rarity})");
            }

            var rows = new List<InlineKeyboardButton[]>();

            if (isCompleted && !def.IsRepeatable)
            {
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("✅ Misión completada", "quest_menu") });
            }
            else if (allDone)
            {
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🏆 ¡ENTREGAR MISIÓN!", $"quest_complete:{questId}") });
            }
            else if (isActive)
            {
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("⚡ Misión en progreso...", "quest_menu") });
            }
            else
            {
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("✅ Aceptar misión", $"quest_accept:{questId}") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Misiones", "quest_menu") });

            var markup = new InlineKeyboardMarkup(rows);
            await SendOrEdit(bot, chatId, sb.ToString(), markup, editMessageId, ct);
        }

        // ── Aceptar misión ──────────────────────────────────────────────────
        public static async Task AcceptQuest(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string questId, CancellationToken ct, int? editMessageId = null)
        {
            var rpgService = new RpgService();
            var (success, msg) = QuestService.AcceptQuest(player, questId);
            if (success) rpgService.SavePlayer(player);

            var markup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("📋 Ver mis misiones", "quest_menu") },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Ciudad",  "rpg_menu_city") }
            });
            await SendOrEdit(bot, chatId, msg, markup, editMessageId, ct);
        }

        // ── Completar/Entregar misión ────────────────────────────────────────
        public static async Task CompleteQuest(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string questId, CancellationToken ct, int? editMessageId = null)
        {
            var rpgService = new RpgService();
            var (success, msg) = QuestService.CompleteQuest(player, questId);
            if (success) rpgService.SavePlayer(player);

            var markup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("📋 Ver más misiones", "quest_menu") },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Ciudad",  "rpg_menu_city") }
            });
            await SendOrEdit(bot, chatId, msg, markup, editMessageId, ct);
        }

        // ── Helper SendOrEdit ───────────────────────────────────────────────
        private static async Task SendOrEdit(ITelegramBotClient bot, long chatId, string text,
            InlineKeyboardMarkup markup, int? editMessageId, CancellationToken ct)
        {
            if (editMessageId.HasValue)
            {
                try
                {
                    await bot.EditMessageText(chatId, editMessageId.Value, text,
                        parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
                    return;
                }
                catch { /* Fallback */ }
            }
            await bot.SendMessage(chatId, text, parseMode: ParseMode.Markdown,
                replyMarkup: markup, cancellationToken: ct);
        }
    }
}
