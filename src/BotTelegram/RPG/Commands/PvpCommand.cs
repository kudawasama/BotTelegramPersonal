using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Commands
{
    /// <summary>Comando /arena — Sistema PvP (Fase 11)</summary>
    public class PvpCommand
    {
        private static readonly PvpService _pvp    = new();
        private static readonly RpgService _rpgSvc = new();

        // ══════════════════════════════════════════════════════════════════
        // ENTRY POINT  /arena | /pvp
        // ══════════════════════════════════════════════════════════════════
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var player = _rpgSvc.GetPlayer(message.Chat.Id);
            if (player is null)
            {
                await bot.SendMessage(message.Chat.Id, "❌ Usa /rpg para crear tu personaje primero.", cancellationToken: ct);
                return;
            }
            await ShowArenaMenu(bot, message.Chat.Id, player, ct, null);
        }

        // ══════════════════════════════════════════════════════════════════
        // CALLBACK DISPATCHER
        // ══════════════════════════════════════════════════════════════════
        public static async Task HandleCallback(
            ITelegramBotClient bot, CallbackQuery cb, string data, CancellationToken ct)
        {
            var chatId = cb.Message!.Chat.Id;
            var msgId  = cb.Message.MessageId;
            var player = _rpgSvc.GetPlayer(chatId);

            await bot.AnswerCallbackQuery(cb.Id, cancellationToken: ct);

            if (player is null)
            {
                await bot.EditMessageText(chatId, msgId, "❌ No tienes perfil RPG. Usa /rpg.", cancellationToken: ct);
                return;
            }

            if (data == "pvp_menu")           { await ShowArenaMenu(bot, chatId, player, ct, msgId); return; }
            if (data == "pvp_ranking")        { await ShowRanking(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_history")        { await ShowHistory(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_profile")        { await ShowPvpProfile(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_challenge_menu") { await ShowChallengeMenu(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_accept")         { await AcceptChallenge(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_decline")        { await DeclineChallenge(bot, chatId, msgId, player, ct); return; }
            if (data == "pvp_cancel")         { await CancelChallenge(bot, chatId, msgId, player, ct); return; }

            if (data.StartsWith("pvp_fight:"))
            {
                if (long.TryParse(data["pvp_fight:".Length..], out var targetId))
                    await StartFight(bot, chatId, msgId, player, targetId, 0, ct);
                return;
            }
            if (data.StartsWith("pvp_bet:"))
            {
                var parts = data["pvp_bet:".Length..].Split(':');
                if (parts.Length == 2 && long.TryParse(parts[0], out var tid) && int.TryParse(parts[1], out var amt))
                {
                    var (ok, msg, chal) = _pvp.SendChallenge(player, tid, amt);
                    await bot.EditMessageText(chatId, msgId, ok ? msg : $"❌ {msg}",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: BackToArena(), cancellationToken: ct);
                }
                return;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // VISTAS
        // ══════════════════════════════════════════════════════════════════

        private static async Task ShowArenaMenu(
            ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? msgId)
        {
            var tier = PvpService.GetTier(player.PvpRating);
            var pendingChallenge = _pvp.GetPendingChallenge(chatId);
            var sentChallenge    = _pvp.GetSentChallenge(chatId);

            bool onCooldown = _pvp.IsOnCooldown(player, out var rem);
            var cdText = onCooldown ? $"\n⏳ Cooldown: {rem.Minutes}m {rem.Seconds}s" : "";

            var text = $"⚔️ **Arena PvP**\n\n" +
                       $"{tier}\n" +
                       $"Rating: **{player.PvpRating}** pts\n" +
                       $"Victorias: {player.PvpWins} | Derrotas: {player.PvpLosses} | Empates: {player.PvpDraws}\n" +
                       cdText;

            var rows = new List<InlineKeyboardButton[]>();

            // Reto pendiente para este jugador
            if (pendingChallenge != null)
            {
                text += $"\n\n🔔 **{pendingChallenge.ChallengerName}** te ha retado" +
                        (pendingChallenge.BetAmount > 0 ? $" (apuesta: {pendingChallenge.BetAmount} 💰)" : "") + "!";
                rows.Add(new[] {
                    InlineKeyboardButton.WithCallbackData("✅ Aceptar reto", "pvp_accept"),
                    InlineKeyboardButton.WithCallbackData("❌ Rechazar", "pvp_decline")
                });
            }

            // Reto enviado esperando
            if (sentChallenge != null && !sentChallenge.Expired)
            {
                text += $"\n\n⏳ Esperando respuesta de **{sentChallenge.ChallengedName}**...";
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🚫 Cancelar reto", "pvp_cancel") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("⚔️ Retar a alguien", "pvp_challenge_menu") });
            rows.Add(new[] {
                InlineKeyboardButton.WithCallbackData("🏆 Ranking", "pvp_ranking"),
                InlineKeyboardButton.WithCallbackData("📋 Historial", "pvp_history")
            });
            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("👤 Mi perfil PvP", "pvp_profile") });
            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("🗺️ Volver al RPG", "rpg_main") });

            var kb = new InlineKeyboardMarkup(rows);

            if (msgId.HasValue)
                await bot.EditMessageText(chatId, msgId.Value, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: kb, cancellationToken: ct);
            else
                await bot.SendMessage(chatId, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: kb, cancellationToken: ct);
        }

        private static async Task ShowChallengeMenu(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            // Listar jugadores activos (distinto de uno mismo, últimos 48h)
            var candidates = _rpgSvc.GetAllPlayers()
                .Where(p => p.ChatId != chatId)
                .OrderByDescending(p => p.LastPlayedAt)
                .Take(8)
                .ToList();

            if (!candidates.Any())
            {
                await bot.EditMessageText(chatId, msgId,
                    "😔 No hay otros jugadores disponibles ahora.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("⚔️ **Selecciona a tu rival:**\n");
            foreach (var c in candidates)
                sb.AppendLine($"• **{c.Name}** — Nv.{c.Level} | Rating: {c.PvpRating} ({PvpService.GetTier(c.PvpRating)})");

            var rows = candidates
                .Select(c => new[] { InlineKeyboardButton.WithCallbackData(
                    $"⚔️ Retar a {c.Name} (Nv.{c.Level})", $"pvp_fight:{c.ChatId}") })
                .ToList();
            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver", "pvp_menu") });

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(rows), cancellationToken: ct);
        }

        private static async Task ShowRanking(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var top = _pvp.GetRanking(10);
            if (!top.Any())
            {
                await bot.EditMessageText(chatId, msgId, "🏆 Aún no hay combates PvP registrados.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("🏆 **Ranking PvP**\n");
            int pos = 1;
            foreach (var e in top)
            {
                var medal = pos == 1 ? "🥇" : pos == 2 ? "🥈" : pos == 3 ? "🥉" : $"#{pos}";
                var you   = e.ChatId == chatId ? " ← tú" : "";
                sb.AppendLine($"{medal} **{e.Name}** — {e.Rating} pts {e.Tier}");
                sb.AppendLine($"   {e.Wins}V / {e.Losses}D / {e.Draws}E ({e.WinRate}%){you}");
                pos++;
            }

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: BackToArena(), cancellationToken: ct);
        }

        private static async Task ShowHistory(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var hist = _pvp.GetHistory(chatId, 5);
            if (!hist.Any())
            {
                await bot.EditMessageText(chatId, msgId, "📋 No tienes combates PvP aún.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("📋 **Historial PvP (últimos 5)**\n");
            foreach (var m in hist)
            {
                var youWon  = m.WinnerId == chatId;
                var isDraw  = m.Result == PvpMatchResult.Draw;
                var icon    = isDraw ? "🤝" : youWon ? "✅" : "❌";
                var rival   = m.PlayerA == chatId ? m.NameB : m.NameA;
                var myElo   = m.PlayerA == chatId ? m.RatingChangeA : m.RatingChangeB;
                var eloSign = myElo >= 0 ? "+" : "";
                sb.AppendLine($"{icon} vs **{rival}** — {(isDraw ? "Empate" : youWon ? "Victoria" : "Derrota")} ({eloSign}{myElo} pts)");
                sb.AppendLine($"   {m.PlayedAt:dd/MM HH:mm} UTC | {m.TurnsPlayed} turnos");
            }

            await bot.EditMessageText(chatId, msgId, sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: BackToArena(), cancellationToken: ct);
        }

        private static async Task ShowPvpProfile(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var tier = PvpService.GetTier(player.PvpRating);
            int total = player.PvpWins + player.PvpLosses + player.PvpDraws;
            double wr = total == 0 ? 0 : Math.Round(100.0 * player.PvpWins / total, 1);

            var text = $"👤 **Perfil PvP de {player.Name}**\n\n" +
                       $"{tier}\n" +
                       $"⭐ Rating: **{player.PvpRating}** pts\n\n" +
                       $"✅ Victorias: **{player.PvpWins}**\n" +
                       $"❌ Derrotas: **{player.PvpLosses}**\n" +
                       $"🤝 Empates:  **{player.PvpDraws}**\n" +
                       $"📊 Win Rate: **{wr}%** ({total} combates)\n\n" +
                       $"⚔️ Ataque: {player.PhysicalAttack} | 🛡️ Defensa: {player.Constitution / 4}\n" +
                       $"❤️ HP: {player.MaxHP} | 🎯 Dex: {player.Dexterity}";

            await bot.EditMessageText(chatId, msgId, text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: BackToArena(), cancellationToken: ct);
        }

        // ══════════════════════════════════════════════════════════════════
        // ACCIONES
        // ══════════════════════════════════════════════════════════════════

        private static async Task StartFight(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player,
            long targetId, int betAmount, CancellationToken ct)
        {
            if (_pvp.IsOnCooldown(player, out var rem))
            {
                await bot.EditMessageText(chatId, msgId,
                    $"⏳ Debes esperar **{rem.Minutes}m {rem.Seconds}s** antes de otro combate.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            var rival = _rpgSvc.GetPlayer(targetId);
            if (rival is null)
            {
                await bot.EditMessageText(chatId, msgId, "❌ Rival no encontrado.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            var (match, narrative) = _pvp.SimulatePvp(player, rival);
            var (goldA, goldB)     = _pvp.ApplyMatchResult(player, rival, match, betAmount);

            var goldText = "";
            if (betAmount > 0)
                goldText = goldA > 0 ? $"\n💰 Ganaste **{goldA}** oro de apuesta!"
                         : goldA < 0 ? $"\n💸 Perdiste **{Math.Abs(goldA)}** oro de apuesta."
                         : "\n🤝 Apuesta devuelta (empate).";

            await bot.EditMessageText(chatId, msgId,
                narrative + goldText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("⚔️ Volver a la Arena", "pvp_menu") },
                    new[] { InlineKeyboardButton.WithCallbackData("🏆 Ver Ranking", "pvp_ranking") }
                }), cancellationToken: ct);

            // Notificar al rival
            try
            {
                var rivalPlayer = _rpgSvc.GetPlayer(targetId); // re-leer con stats actualizados
                var eloRival    = match.PlayerA == chatId ? match.RatingChangeB : match.RatingChangeA;
                var eloSign     = eloRival >= 0 ? "+" : "";
                var resultMsg   = match.WinnerId == targetId ? "✅ ¡Ganaste!" : match.Result == PvpMatchResult.Draw ? "🤝 Empate" : "❌ Perdiste";
                await bot.SendMessage(targetId,
                    $"⚔️ **{player.Name}** te retó en la Arena!\n\n{resultMsg}\n📊 Rating: {eloSign}{eloRival} pts\nUsa /arena para ver tu historial.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct);
            }
            catch { /* El rival puede tener el bot bloqueado */ }
        }

        private static async Task AcceptChallenge(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var chal = _pvp.GetPendingChallenge(chatId);
            if (chal is null || chal.Expired)
            {
                await bot.EditMessageText(chatId, msgId, "⚠️ El reto ya expiró o fue cancelado.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }
            _pvp.CancelChallenge(chal.ChallengerId); // borrar reto

            var challenger = _rpgSvc.GetPlayer(chal.ChallengerId);
            if (challenger is null)
            {
                await bot.EditMessageText(chatId, msgId, "❌ El retador ya no existe.",
                    replyMarkup: BackToArena(), cancellationToken: ct);
                return;
            }

            await StartFight(bot, chatId, msgId, player, challenger.ChatId, chal.BetAmount, ct);
        }

        private static async Task DeclineChallenge(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            var chal = _pvp.GetPendingChallenge(chatId);
            if (chal != null) _pvp.CancelChallenge(chal.ChallengerId);

            await bot.EditMessageText(chatId, msgId, "🚫 Reto rechazado.",
                replyMarkup: BackToArena(), cancellationToken: ct);

            // Notificar al retador
            try
            {
                var msg = $"😔 **{player.Name}** rechazó tu reto de combate.";
                await bot.SendMessage(chal?.ChallengerId ?? chatId, msg,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, cancellationToken: ct);
            }
            catch { }
        }

        private static async Task CancelChallenge(
            ITelegramBotClient bot, long chatId, int msgId, RpgPlayer player, CancellationToken ct)
        {
            _pvp.CancelChallenge(chatId);
            await bot.EditMessageText(chatId, msgId, "✅ Reto cancelado.",
                replyMarkup: BackToArena(), cancellationToken: ct);
        }

        // ══════════════════════════════════════════════════════════════════
        // HELPERS
        // ══════════════════════════════════════════════════════════════════
        private static InlineKeyboardMarkup BackToArena() =>
            new(new[] { new[] { InlineKeyboardButton.WithCallbackData("◀️ Volver a la Arena", "pvp_menu") } });
    }
}
