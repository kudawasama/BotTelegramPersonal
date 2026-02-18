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
    /// Comando /tienda — Tienda del RPG: comprar consumibles/materiales y vender ítems del inventario.
    /// Callbacks: rpg_shop, shop_buy, shop_sell, shop_buy_item:{id}, inv_sell_item:*, inv_sell_equip:*
    /// </summary>
    public class ShopCommand
    {
        // ═══════════════════════════════════════
        // CATÁLOGO DE LA TIENDA
        // ═══════════════════════════════════════
        private static readonly List<ShopEntry> _catalog = new()
        {
            // ─── Pociones HP ───────────────────────────────────────
            new("shop_pocion_menor",    "🧪 Poción Menor",    "Restaura 50 HP",       50,   ItemType.Consumable, 50,  0),
            new("shop_pocion_vida",     "❤️ Poción de Vida",  "Restaura 150 HP",      120,  ItemType.Consumable, 150, 0),
            new("shop_pocion_mayor",    "🍶 Poción Mayor",    "Restaura 300 HP",      250,  ItemType.Consumable, 300, 0),
            new("shop_pocion_suprema",  "🏺 Poción Suprema",  "Restaura HP máximo",   500,  ItemType.Consumable, 9999,0),
            // ─── Pociones Maná ─────────────────────────────────────
            new("shop_pocion_mana",     "💧 Poción de Maná",  "Restaura 50 maná",     80,   ItemType.Consumable, 0,   50),
            new("shop_elixir_mana",     "💎 Elixir de Maná",  "Restaura 200 maná",    200,  ItemType.Consumable, 0,   200),
            // ─── Materiales ────────────────────────────────────────
            new("shop_fragmento",       "🔷 Fragmento Cristal","Material de crafteo", 40,   ItemType.Material, 0, 0),
            new("shop_esencia",         "✨ Esencia Mágica",  "Material raro crafteo",100,  ItemType.Material, 0, 0),
            new("shop_runa",            "🔶 Runa Antigua",    "Scroll de habilidad",  180,  ItemType.Material, 0, 0),
        };

        // ─── Modelo de entrada de tienda ────────────────────────────
        private record ShopEntry(string Id, string Name, string Description, int Price,
            ItemType ItemType, int HPRestore, int ManaRestore);

        // ═══════════════════════════════════════
        // PUNTO DE ENTRADA
        // ═══════════════════════════════════════
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var rpgService = new RpgService();
            var player = rpgService.GetPlayer(chatId);
            if (player == null)
            {
                await bot.SendMessage(chatId, "❌ No tienes personaje. Usa /rpg para crear uno.", cancellationToken: ct);
                return;
            }
            await ShowShopMain(bot, chatId, player, ct);
        }

        // ═══════════════════════════════════════
        // MENÚ PRINCIPAL DE LA TIENDA
        // ═══════════════════════════════════════
        public static async Task ShowShopMain(ITelegramBotClient bot, long chatId, RpgPlayer player,
            CancellationToken ct, int? editMessageId = null)
        {
            var text = new StringBuilder();
            text.AppendLine("🏪 **TIENDA DE AVENTUREROS**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"💰 Tu oro: **{player.Gold}** monedas");
            text.AppendLine();
            text.AppendLine("¿Qué deseas hacer hoy?");
            text.AppendLine();
            text.AppendLine("🛒 **Comprar** — Consumibles y materiales");
            text.AppendLine("💰 **Vender** — Ítems y equipos de tu inventario");

            var markup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("🛒 Comprar", "shop_buy"),
                        InlineKeyboardButton.WithCallbackData("💰 Vender",  "shop_sell") },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main") }
            });

            await SendOrEdit(bot, chatId, text.ToString(), markup, ct, editMessageId);
        }

        // ═══════════════════════════════════════
        // TAB COMPRAR
        // ═══════════════════════════════════════
        public static async Task ShowBuyMenu(ITelegramBotClient bot, long chatId, RpgPlayer player,
            CancellationToken ct, int? editMessageId = null)
        {
            var text = new StringBuilder();
            text.AppendLine("🛒 **CATÁLOGO DE LA TIENDA**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"💰 Tu oro: **{player.Gold}** monedas");
            text.AppendLine();

            foreach (var entry in _catalog)
            {
                var affordIcon = player.Gold >= entry.Price ? "✅" : "❌";
                text.AppendLine($"{affordIcon} {entry.Name} — **{entry.Price}g**");
                text.AppendLine($"    _{entry.Description}_");
            }

            text.AppendLine();
            text.AppendLine("Pulsa un ítem para comprarlo:");

            // Botones de compra (2 por fila, solo los que puede pagar o todos visibles)
            var buttons = _catalog
                .Select(e => InlineKeyboardButton.WithCallbackData(
                    $"{e.Name} ({e.Price}g)",
                    $"shop_buy_item:{e.Id}"))
                .Chunk(2)
                .Select(r => r.ToArray())
                .ToList();

            buttons.Add(new[] {
                InlineKeyboardButton.WithCallbackData("🔙 Volver Tienda", "rpg_shop")
            });

            await SendOrEdit(bot, chatId, text.ToString(), new InlineKeyboardMarkup(buttons), ct, editMessageId);
        }

        // ═══════════════════════════════════════
        // COMPRAR ÍTEM
        // ═══════════════════════════════════════
        public static async Task BuyItem(ITelegramBotClient bot, long chatId, RpgPlayer player,
            string shopItemId, InventoryService invSvc, CancellationToken ct, int? editMessageId = null, string? callbackId = null)
        {
            var entry = _catalog.FirstOrDefault(e => e.Id == shopItemId);
            if (entry == null)
            {
                if (callbackId != null)
                    await bot.AnswerCallbackQuery(callbackId, "❌ Ítem no encontrado.", cancellationToken: ct);
                return;
            }

            if (player.Gold < entry.Price)
            {
                if (callbackId != null)
                    await bot.AnswerCallbackQuery(callbackId, $"❌ Oro insuficiente ({player.Gold}/{entry.Price})", cancellationToken: ct);
                return;
            }

            if (player.Inventory.Count >= 40)
            {
                if (callbackId != null)
                    await bot.AnswerCallbackQuery(callbackId, "❌ Inventario lleno (máx 40 ítems).", cancellationToken: ct);
                return;
            }

            // Descontar oro y agregar ítem
            player.Gold -= entry.Price;

            // Separar emoji y nombre: "🧪 Poción Menor" → emoji="🧪", name="Poción Menor"
            var nameParts = entry.Name.Split(' ', 2);
            var itemEmoji = nameParts.Length > 0 ? nameParts[0] : "📦";
            var itemName  = nameParts.Length > 1 ? nameParts[1] : entry.Name;

            var newItem = new RpgItem
            {
                Id          = Guid.NewGuid().ToString("N")[..8],
                Name        = itemName,
                Emoji       = itemEmoji,
                Description = entry.Description,
                Type        = entry.ItemType,
                Value       = entry.Price,
                HPRestore   = entry.HPRestore,
                ManaRestore = entry.ManaRestore,
                Rarity      = ItemRarity.Common
            };

            invSvc.AddItem(player, newItem);

            if (callbackId != null)
                await bot.AnswerCallbackQuery(callbackId, $"✅ {newItem.Name} comprado por {entry.Price}g", cancellationToken: ct);

            // Refrescar menú de compra
            await ShowBuyMenu(bot, chatId, player, ct, editMessageId);
        }

        // ═══════════════════════════════════════
        // TAB VENDER
        // ═══════════════════════════════════════
        public static async Task ShowSellMenu(ITelegramBotClient bot, long chatId, RpgPlayer player,
            CancellationToken ct, int? editMessageId = null)
        {
            var text = new StringBuilder();
            text.AppendLine("💰 **VENDER ÍTEMS**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"💰 Tu oro: **{player.Gold}** monedas");
            text.AppendLine();

            var buttons = new List<InlineKeyboardButton[]>();

            // ─── Consumibles / Materiales ──────────────────────────
            var sellableItems = player.Inventory
                .Where(i => i.Type != ItemType.Quest)
                .OrderByDescending(i => i.Value)
                .ToList();

            if (sellableItems.Any())
            {
                text.AppendLine("**🎒 Consumibles y Materiales:**");
                foreach (var item in sellableItems.Take(10))
                {
                    var sellVal = Math.Max(1, item.Value / 2);
                    text.AppendLine($"  {item.Emoji} {item.Name} → **{sellVal}g**");
                    buttons.Add(new[] {
                        InlineKeyboardButton.WithCallbackData(
                            $"💰 {item.Emoji}{item.Name} ({sellVal}g)",
                            $"shop_sell_item:{item.Id}")
                    });
                }
                if (sellableItems.Count > 10)
                    text.AppendLine($"  *(y {sellableItems.Count - 10} más en inventario...)*");
                text.AppendLine();
            }

            // ─── Equipamiento ──────────────────────────────────────
            var sellableEquip = player.EquipmentInventory
                .Where(e => e.Id != player.EquippedWeaponNew?.Id && e.Id != player.EquippedArmorNew?.Id && e.Id != player.EquippedAccessoryNew?.Id)
                .OrderByDescending(e => e.Price)
                .ToList();

            if (sellableEquip.Any())
            {
                text.AppendLine("**⚔️ Equipamiento (no equipado):**");
                foreach (var eq in sellableEquip.Take(8))
                {
                    var sellVal = Math.Max(1, eq.Price / 2);
                    text.AppendLine($"  {eq.TypeEmoji} {eq.Name} {eq.RarityEmoji} → **{sellVal}g**");
                    buttons.Add(new[] {
                        InlineKeyboardButton.WithCallbackData(
                            $"💰 {eq.TypeEmoji}{eq.Name} ({sellVal}g)",
                            $"shop_sell_equip:{eq.Id}")
                    });
                }
            }

            if (!sellableItems.Any() && !sellableEquip.Any())
            {
                text.AppendLine("🎒 No tienes ítems para vender.");
                text.AppendLine("*(Consigue ítems en mazmorras y combates)*");
            }

            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver Tienda", "rpg_shop") });

            await SendOrEdit(bot, chatId, text.ToString(), new InlineKeyboardMarkup(buttons), ct, editMessageId);
        }

        // ═══════════════════════════════════════
        // HELPER: SendOrEdit
        // ═══════════════════════════════════════
        private static async Task SendOrEdit(ITelegramBotClient bot, long chatId, string text,
            InlineKeyboardMarkup markup, CancellationToken ct, int? editMessageId)
        {
            if (editMessageId.HasValue)
                await bot.EditMessageText(chatId, editMessageId.Value, text,
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            else
                await bot.SendMessage(chatId, text,
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
        }
    }
}
