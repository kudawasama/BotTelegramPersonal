using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Commands
{
    /// <summary>Comando /gremio — Sistema de Gremio (Fase 10)</summary>
    public class GuildCommand
    {
        private static readonly GuildService _gs = new();

        // ══════════════════════════════════════════════════════════════════
        // ENTRY POINT  /gremio
        // ══════════════════════════════════════════════════════════════════
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var player = new RpgService().GetPlayer(message.Chat.Id);
            if (player is null)
            {
                await bot.SendMessage(message.Chat.Id, "❌ Aún no tienes un perfil RPG. Usa /rpg para comenzar.", cancellationToken: ct);
                return;
            }
            await ShowGuildMenu(bot, message.Chat.Id, player, ct, messageId: null);
        }

        // ══════════════════════════════════════════════════════════════════
        // CALLBACK DISPATCHER (llamado desde CallbackQueryHandler)
        // ══════════════════════════════════════════════════════════════════
        public static async Task HandleCallback(
            ITelegramBotClient bot, CallbackQuery cb, string data, CancellationToken ct)
        {
            var chatId    = cb.Message!.Chat.Id;
            var msgId     = cb.Message.MessageId;
            var rpgSvc    = new RpgService();
            var player    = rpgSvc.GetPlayer(chatId);
            if (player is null)
            {
                await bot.EditMessageText(chatId, msgId, "❌ No tienes perfil RPG. Usa /rpg para comenzar.", cancellationToken: ct);
                return;
            }

            await bot.AnswerCallbackQuery(cb.Id, cancellationToken: ct);

            if (data == "guild_menu")               { await ShowGuildMenu(bot, chatId, player, ct, msgId); return; }
            if (data == "guild_info")               { await ShowGuildInfo(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_members")            { await ShowGuildMembers(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_create")             { await ShowCreateGuildForm(bot, chatId, msgId, ct); return; }
            if (data == "guild_join_list")          { await ShowJoinList(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_leave")              { await ShowLeaveConfirm(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_leave_confirm")      { await DoLeave(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_bank")               { await ShowGuildBank(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_ranking")            { await ShowGuildRanking(bot, chatId, msgId, ct); return; }
            if (data == "guild_disband")            { await ShowDisbandConfirm(bot, chatId, msgId, player, ct); return; }
            if (data == "guild_disband_confirm")    { await DoDisband(bot, chatId, msgId, player, ct); return; }

            if (data.StartsWith("guild_join:"))
            {
                var guildId = data["guild_join:".Length..];
                await DoJoin(bot, chatId, msgId, player, guildId, ct);
                return;
            }
            if (data.StartsWith("guild_kick:") && long.TryParse(data["guild_kick:".Length..], out var kickId))
            {
                await DoKick(bot, chatId, msgId, player, kickId, ct);
                return;
            }
            if (data.StartsWith("guild_promote:") && long.TryParse(data["guild_promote:".Length..], out var promId))
            {
                await DoPromote(bot, chatId, msgId, player, promId, ct);
                return;
            }
            if (data.StartsWith("guild_deposit:"))
            {
                var raw = data["guild_deposit:".Length..];
                if (int.TryParse(raw, out var amt)) await DoDeposit(bot, chatId, msgId, player, amt, ct);
                return;
            }
            if (data.StartsWith("guild_withdraw:"))
            {
                var raw = data["guild_withdraw:".Length..];
                if (int.TryParse(raw, out var amt)) await DoWithdraw(bot, chatId, msgId, player, amt, ct);
                return;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // VISTAS
        // ══════════════════════════════════════════════════════════════════

        public static async Task ShowGuildMenu(
            ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? messageId)
        {
            var guild = _gs.GetPlayerGuild(player);

            string text;
            InlineKeyboardMarkup kb;

            if (guild is null)
            {
                text = $"🏰 **Sistema de Gremio**\n\n" +
                       $"No perteneces a ningún gremio.\n\n" +
                       $"💰 Crear gremio cuesta **{GuildService.CreateCost}** oro.\n" +
                       $"Tu oro actual: **{player.Gold}** 💰";

                kb = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏰 Crear Gremio", "guild_create") },
                    new[] { InlineKeyboardButton.WithCallbackData("🔍 Buscar Gremios", "guild_join_list") },
                    new[] { InlineKeyboardButton.WithCallbackData("🏆 Ranking de Gremios", "guild_ranking") },
                    new[] { InlineKeyboardButton.WithCallbackData("🗺️ Volver al RPG", "rpg_main") }
                });
            }
            else
            {
                text = $"{guild.LevelEmoji} **{guild.Emoji} {guild.Name}** [{guild.Tag}]\n" +
                       $"Nivel {guild.Level} | {guild.Members.Count}/{guild.MaxMembers} miembros\n\n" +
                       $"Tu rol: **{RoleLabel(player.GuildRole)}**\n" +
                       $"Contribución total: **{player.GuildContribution}** 💰";

                var rows = new List<InlineKeyboardButton[]>
                {
                    new[] { InlineKeyboardButton.WithCallbackData("📋 Info del Gremio", "guild_info") },
                    new[] { InlineKeyboardButton.WithCallbackData("👥 Miembros", "guild_members") },
                    new[] { InlineKeyboardButton.WithCallbackData("🏦 Banco del Gremio", "guild_bank") },
                    new[] { InlineKeyboardButton.WithCallbackData("🏆 Ranking de Gremios", "guild_ranking") },
                    new[] { InlineKeyboardButton.WithCallbackData("🚪 Salir del Gremio", "guild_leave") },
                    new[] { InlineKeyboardButton.WithCallbackData("🗺️ Volver al RPG", "rpg_main") }
                };
                kb = new InlineKeyboardMarkup(rows);
            }

            if (messageId.HasValue)
                await bot.EditMessageText(chatId, messageId.Value, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: kb, cancellationToken: ct);
            else
                await bot.SendMessage(chatId, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: kb, cancellationToken: ct);
        }

        private static async Task ShowGuildInfo(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null) { await bot.SendMessage(chatId, "❌ No perteneces a un gremio.", cancellationToken: ct); return; }

            var progressBar = BuildProgressBar(guild.Experience, guild.ExperienceToNextLevel);
            var owner       = guild.GetMember(guild.OwnerId);

            var text = $"🏰 **{guild.Emoji} {guild.Name}** [{guild.Tag}]\n" +
                       $"━━━━━━━━━━━━━━━━━━\n" +
                       $"📝 {guild.Description}\n\n" +
                       $"{guild.LevelEmoji} Nivel **{guild.Level}** | XP: {progressBar}\n" +
                       $"👑 Líder: **{owner?.Name ?? "Desconocido"}**\n" +
                       $"👥 Miembros: **{guild.Members.Count}/{guild.MaxMembers}**\n" +
                       $"🏦 Banco: **{guild.GuildBank}** 💰\n" +
                       $"💎 Contribución total: **{guild.TotalContribution}** 💰\n" +
                       $"📅 Fundado: {guild.CreatedAt:dd/MM/yyyy}";

            var rows = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData("👥 Ver Miembros", "guild_members") },
                new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
            };

            // Si es Owner: botón disolver
            if (guild.IsOwner(player.ChatId))
                rows.Insert(1, new[] { InlineKeyboardButton.WithCallbackData("💥 Disolver Gremio", "guild_disband") });

            await bot.EditMessageText(chatId, msgId, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(rows), cancellationToken: ct);
        }

        private static async Task ShowGuildMembers(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null) { await bot.SendMessage(chatId, "❌ No perteneces a un gremio.", cancellationToken: ct); return; }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"👥 **Miembros de {guild.Emoji} {guild.Name}**\n");

            foreach (var m in guild.Members.OrderByDescending(x => x.Role).ThenByDescending(x => x.Contribution))
            {
                var roleIcon = m.Role == GuildRole.Owner ? "👑" : m.Role == GuildRole.Officer ? "⭐" : "⚔️";
                sb.AppendLine($"{roleIcon} **{m.Name}** — {m.Contribution:N0} 💰");
            }

            var rows = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
            };

            // Opciones de gestión para Owner/Officer
            if (guild.CanManage(player.ChatId))
            {
                var kickable = guild.Members
                    .Where(m => m.ChatId != player.ChatId && m.Role < player.GuildRole)
                    .Take(4)
                    .ToList();

                foreach (var m in kickable)
                    rows.Insert(0, new[] { InlineKeyboardButton.WithCallbackData($"🚫 Expulsar {m.Name}", $"guild_kick:{m.ChatId}") });

                if (guild.IsOwner(player.ChatId))
                {
                    var promotable = guild.Members
                        .Where(m => m.ChatId != player.ChatId && m.Role == GuildRole.Member)
                        .Take(3)
                        .ToList();
                    foreach (var m in promotable)
                        rows.Insert(0, new[] { InlineKeyboardButton.WithCallbackData($"⭐ Promover {m.Name}", $"guild_promote:{m.ChatId}") });
                }
            }

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(rows), cancellationToken: ct);
        }

        private static async Task ShowCreateGuildForm(
            ITelegramBotClient bot, long chatId, int msgId, CancellationToken ct)
        {
            var text = "🏰 **Crear un Gremio**\n\n" +
                       $"Costo: **{GuildService.CreateCost}** 💰 oro\n\n" +
                       "Usa el comando con los parámetros:\n" +
                       "`/gremio crear [nombre] | [tag] | [emoji] | [descripción]`\n\n" +
                       "**Ejemplo:**\n" +
                       "`/gremio crear Los Dragones | DRGN | 🐉 | Gremio de élite`\n\n" +
                       "• Nombre: 3-24 caracteres\n" +
                       "• Tag: 2-5 letras (identificador único)\n" +
                       "• Emoji: cualquier emoji";

            await bot.EditMessageText(chatId, msgId, text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task ShowJoinList(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guilds = _gs.GetJoinableGuilds();
            if (!guilds.Any())
            {
                await bot.EditMessageText(chatId, msgId, "😔 No hay gremios disponibles para unirse ahora.",
                    replyMarkup: new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") } }),
                    cancellationToken: ct);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("🔍 **Gremios Disponibles**\n");
            foreach (var g in guilds)
                sb.AppendLine($"{g.LevelEmoji} **{g.Emoji} {g.Name}** [{g.Tag}] — Nv.{g.Level} | {g.Members.Count}/{g.MaxMembers} miembros");

            var rows = guilds.Select(g =>
                new[] { InlineKeyboardButton.WithCallbackData($"🚪 Unirse a {g.Emoji} {g.Name}", $"guild_join:{g.Id}") }
            ).ToList();
            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") });

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(rows), cancellationToken: ct);
        }

        private static async Task ShowLeaveConfirm(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null) { await bot.SendMessage(chatId, "❌ No estás en un gremio.", cancellationToken: ct); return; }

            var extra = guild.IsOwner(player.ChatId) && guild.Members.Count > 1
                ? "\n⚠️ Eres el líder — el rango pasará al siguiente oficial/miembro."
                : guild.IsOwner(player.ChatId)
                    ? "\n⚠️ Eres el único miembro — el gremio se **disolverá**."
                    : "";

            await bot.EditMessageText(chatId, msgId,
                $"⚠️ ¿Seguro que quieres salir de **{guild.Emoji} {guild.Name}**?{extra}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("✅ Sí, salir", "guild_leave_confirm") },
                    new[] { InlineKeyboardButton.WithCallbackData("❌ Cancelar", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task ShowGuildBank(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null) { await bot.SendMessage(chatId, "❌ No perteneces a un gremio.", cancellationToken: ct); return; }

            var canWithdraw = guild.CanManage(player.ChatId);
            var text = $"🏦 **Banco de {guild.Emoji} {guild.Name}**\n\n" +
                       $"Balance: **{guild.GuildBank}** 💰\n" +
                       $"Tu oro: **{player.Gold}** 💰\n" +
                       $"Tu contribución: **{player.GuildContribution}** 💰";

            var rows = new List<InlineKeyboardButton[]>
            {
                new[] {
                    InlineKeyboardButton.WithCallbackData("💰 +100", "guild_deposit:100"),
                    InlineKeyboardButton.WithCallbackData("💰 +500", "guild_deposit:500"),
                    InlineKeyboardButton.WithCallbackData("💰 +1000", "guild_deposit:1000")
                }
            };

            if (canWithdraw)
                rows.Add(new[] {
                    InlineKeyboardButton.WithCallbackData("💸 -100", "guild_withdraw:100"),
                    InlineKeyboardButton.WithCallbackData("💸 -500", "guild_withdraw:500"),
                    InlineKeyboardButton.WithCallbackData("💸 -1000", "guild_withdraw:1000")
                });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") });

            await bot.EditMessageText(chatId, msgId, text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(rows), cancellationToken: ct);
        }

        private static async Task ShowGuildRanking(
            ITelegramBotClient bot, long chatId, int msgId, CancellationToken ct)
        {
            var top = _gs.GetRanking(10);
            if (!top.Any())
            {
                await bot.EditMessageText(chatId, msgId, "🏆 Aún no hay gremios registrados.",
                    replyMarkup: new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") } }),
                    cancellationToken: ct);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("🏆 **Ranking de Gremios**\n");
            int pos = 1;
            foreach (var g in top)
            {
                var medal = pos == 1 ? "🥇" : pos == 2 ? "🥈" : pos == 3 ? "🥉" : $"#{pos}";
                sb.AppendLine($"{medal} {g.Emoji} **{g.Name}** [{g.Tag}] — Nv.{g.Level} | {g.Members.Count} miembros");
                pos++;
            }

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task ShowDisbandConfirm(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null || !guild.IsOwner(player.ChatId))
            { await bot.SendMessage(chatId, "❌ Solo el líder puede disolver el gremio.", cancellationToken: ct); return; }

            await bot.EditMessageText(chatId, msgId,
                $"💥 ¿Seguro que quieres **disolver** el gremio **{guild.Emoji} {guild.Name}**?\n\n" +
                "⚠️ Todos los miembros serán expulsados y el oro del banco se **perderá**.",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("💥 Disolver definitivamente", "guild_disband_confirm") },
                    new[] { InlineKeyboardButton.WithCallbackData("❌ Cancelar", "guild_menu") }
                }), cancellationToken: ct);
        }

        // ══════════════════════════════════════════════════════════════════
        // ACCIONES
        // ══════════════════════════════════════════════════════════════════

        private static async Task DoJoin(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, string guildId, CancellationToken ct)
        {
            var (ok, msg) = _gs.JoinGuild(player, guildId);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏰 Menú Gremio", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoLeave(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var (ok, msg) = _gs.LeaveGuild(player);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏰 Menú Gremio", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoKick(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, long targetId, CancellationToken ct)
        {
            var (ok, msg) = _gs.KickMember(player, targetId);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("👥 Miembros", "guild_members") },
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoPromote(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, long targetId, CancellationToken ct)
        {
            var (ok, msg) = _gs.PromoteMember(player, targetId);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("👥 Miembros", "guild_members") },
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoDeposit(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, int amount, CancellationToken ct)
        {
            var (ok, msg) = _gs.Deposit(player, amount);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏦 Banco", "guild_bank") },
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoWithdraw(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, int amount, CancellationToken ct)
        {
            var (ok, msg) = _gs.Withdraw(player, amount);
            await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏦 Banco", "guild_bank") },
                    new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "guild_menu") }
                }), cancellationToken: ct);
        }

        private static async Task DoDisband(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var guild = _gs.GetPlayerGuild(player);
            if (guild is null || !guild.IsOwner(player.ChatId))
            { await bot.SendMessage(chatId, "❌ Solo el líder puede disolver.", cancellationToken: ct); return; }

            var rpgSvc = new RpgService();
            // Limpiar todos los miembros
            foreach (var m in guild.Members.ToList())
            {
                var mp = rpgSvc.GetPlayer(m.ChatId);
                if (mp != null) { mp.GuildId = null; mp.GuildRole = GuildRole.Member; rpgSvc.SavePlayer(mp); }
            }
            var guildName = guild.Name;
            new GuildDatabase().Delete(guild.Id);

            await bot.EditMessageText(chatId, msgId,
                $"🏚️ El gremio **{guildName}** ha sido disuelto.",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("🏰 Menú Gremio", "guild_menu") }
                }), cancellationToken: ct);
        }

        // ══════════════════════════════════════════════════════════════════
        // HANDLE /gremio crear [args]  (llamado desde CommandRouter)
        // ══════════════════════════════════════════════════════════════════
        public static async Task HandleCreateCommand(
            ITelegramBotClient bot, Message message, RpgPlayer player, CancellationToken ct)
        {
            // Formato: /gremio crear Nombre | TAG | 🐉 | Descripción
            var args = message.Text ?? "";
            var parts = args.Split('|');
            if (parts.Length < 3)
            {
                await bot.SendMessage(message.Chat.Id,
                    "⚠️ Formato: `/gremio crear Nombre | TAG | 🐉 | Descripción`\nEjemplo: `/gremio crear Los Dragones | DRGN | 🐉 | Gremio de élite`",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct);
                return;
            }

            // Extraer nombre (quitar el prefijo "/gremio crear")
            var rawName = parts[0].Replace("/gremio crear", "", StringComparison.OrdinalIgnoreCase).Trim();
            var tag     = parts[1].Trim();
            var emoji   = parts.Length > 2 ? parts[2].Trim() : "🏰";
            var desc    = parts.Length > 3 ? parts[3].Trim() : "Un gran gremio.";

            var (ok, msg, guild) = _gs.CreateGuild(player, rawName, tag, desc, emoji);
            await bot.SendMessage(message.Chat.Id, ok ? msg : msg,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct);
        }

        // ══════════════════════════════════════════════════════════════════
        // HELPERS UI
        // ══════════════════════════════════════════════════════════════════
        private static string RoleLabel(GuildRole role) => role switch
        {
            GuildRole.Owner   => "👑 Líder",
            GuildRole.Officer => "⭐ Oficial",
            _                 => "⚔️ Miembro"
        };

        private static string BuildProgressBar(int current, int max, int bars = 10)
        {
            var ratio   = max == 0 ? 0 : (double)current / max;
            var filled  = (int)(ratio * bars);
            return $"[{"█".PadLeft(filled + 1, '█').PadRight(bars, '░')}] {current}/{max}";
        }
    }
}
