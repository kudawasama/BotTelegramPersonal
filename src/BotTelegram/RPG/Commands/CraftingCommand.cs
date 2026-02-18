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
    /// Comando /herreria — Sistema de Crafteo (Fase 8).
    /// Callbacks: craft_menu, craft_view:{id}, craft_do:{id}
    /// </summary>
    public class CraftingCommand
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
            await ShowCraftMenu(bot, chatId, player, ct);
        }

        // ── Menú principal de Herrería ──────────────────────────────────────
        public static async Task ShowCraftMenu(ITelegramBotClient bot, long chatId, RpgPlayer player,
            CancellationToken ct, int? editMessageId = null)
        {
            var recipes   = CraftingDatabase.GetAvailableFor(player.Level);
            var allRecipes = CraftingDatabase.AllRecipes;
            var locked    = allRecipes.Where(r => r.RequiredLevel > player.Level).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("⚒️ **HERRERÍA**");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"👤 Nivel: **{player.Level}** | 💰 Oro: **{player.Gold}**");
            sb.AppendLine();
            sb.AppendLine("**Materiales disponibles:**");

            var mats = player.Inventory.Where(i => i.Type == ItemType.Material)
                             .GroupBy(i => i.Name)
                             .ToDictionary(g => g.Key, g => g.Count());

            if (mats.Count == 0)
                sb.AppendLine("_(sin materiales — obtén materiales derrotando enemigos)_");
            else
                foreach (var (name, qty) in mats)
                    sb.AppendLine($"• {name} ×{qty}");

            sb.AppendLine();
            sb.AppendLine($"**Recetas disponibles:** {recipes.Count}/{allRecipes.Count}");

            // Botones de recetas disponibles (máx 6 filas)
            var rows = new List<InlineKeyboardButton[]>();
            foreach (var r in recipes.Take(6))
            {
                var (canCraft, _) = CraftingService.CheckIngredients(player, r);
                string icon = canCraft ? "✅" : "🔸";
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"{icon} {r.Emoji} {r.Name} (Lv{r.RequiredLevel})", $"craft_view:{r.Id}")
                });
            }

            if (locked.Any())
            {
                sb.AppendLine();
                sb.AppendLine("🔒 **Recetas bloqueadas:**");
                foreach (var r in locked.Take(3))
                    sb.AppendLine($"• {r.Emoji} {r.Name} (requiere Lv{r.RequiredLevel})");
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Ciudad", "rpg_menu_city") });

            var markup = new InlineKeyboardMarkup(rows);
            await SendOrEdit(bot, chatId, sb.ToString(), markup, editMessageId, ct);
        }

        // ── Detalle de receta ────────────────────────────────────────────────
        public static async Task ShowRecipeDetail(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string recipeId, CancellationToken ct, int? editMessageId = null)
        {
            var recipe = CraftingDatabase.GetById(recipeId);
            if (recipe is null)
            {
                await bot.SendMessage(chatId, "❌ Receta no encontrada.", cancellationToken: ct);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"⚒️ **{recipe.Emoji} {recipe.Name}**");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"_{recipe.Description}_");
            sb.AppendLine();
            sb.AppendLine($"📊 Nivel requerido: **{recipe.RequiredLevel}**");
            sb.AppendLine($"🎯 Resultado: **{recipe.ResultEmoji} {recipe.ResultName}** ({recipe.ResultRarity})");
            if (recipe.ResultHPRestore  > 0) sb.AppendLine($"   ❤️ Restaura {recipe.ResultHPRestore} HP");
            if (recipe.ResultManaRestore > 0) sb.AppendLine($"   💧 Restaura {recipe.ResultManaRestore} Maná");
            sb.AppendLine();
            sb.AppendLine("**Ingredientes:**");
            sb.AppendLine(CraftingService.IngredientStatusText(player, recipe));

            var (canCraft, missing) = CraftingService.CheckIngredients(player, recipe);

            var rows = new List<InlineKeyboardButton[]>();
            if (canCraft)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData($"⚒️ ¡CRAFTEAR {recipe.ResultName}!", $"craft_do:{recipeId}") });
            else
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("❌ Faltan ingredientes", "craft_menu") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Herrería", "craft_menu") });

            var markup = new InlineKeyboardMarkup(rows);
            await SendOrEdit(bot, chatId, sb.ToString(), markup, editMessageId, ct);
        }

        // ── Ejecutar crafteo ────────────────────────────────────────────────
        public static async Task DoCraft(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string recipeId, CancellationToken ct, int? editMessageId = null)
        {
            var rpgService = new RpgService();
            var (success, message, item, equip) = CraftingService.Craft(player, recipeId);

            if (success)
            {
                // Actualizar objetivos de misión de crafteo
                var notifications = QuestService.UpdateCraftObjective(player, recipeId);
                rpgService.SavePlayer(player);

                var sb = new StringBuilder();
                sb.AppendLine(message);
                if (notifications.Any())
                {
                    sb.AppendLine();
                    foreach (var n in notifications) sb.AppendLine(n);
                }

                var markup = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("⚒️ Seguir crafteando", "craft_menu") },
                    new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Ciudad",   "rpg_menu_city") }
                });
                await SendOrEdit(bot, chatId, sb.ToString(), markup, editMessageId, ct);
            }
            else
            {
                var markup = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver a Herrería", "craft_menu") }
                });
                await SendOrEdit(bot, chatId, message, markup, editMessageId, ct);
            }
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
                catch { /* Fallback a SendMessage */ }
            }
            await bot.SendMessage(chatId, text, parseMode: ParseMode.Markdown,
                replyMarkup: markup, cancellationToken: ct);
        }
    }
}
