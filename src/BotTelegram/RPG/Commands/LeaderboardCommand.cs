using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;
using BotTelegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Commands;

public class LeaderboardCommand
{
    private readonly LeaderboardService _leaderboardService;
    private readonly RpgService _rpgService;

    public LeaderboardCommand()
    {
        _rpgService = new RpgService();
        _leaderboardService = new LeaderboardService(_rpgService);
    }

    public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;
        await ShowMainLeaderboard(bot, chatId, ct);
    }

    public async Task ShowMainLeaderboard(ITelegramBotClient bot, long chatId, CancellationToken ct)
    {
        var globalStats = _leaderboardService.GetGlobalStats();
        
        var text = "🏆 **RANKINGS GLOBALES**\n\n";
        text += "📊 **Estadísticas del Reino:**\n";
        text += $"👥 Aventureros registrados: **{globalStats.TotalPlayers}**\n";
        text += $"⚡ Activos (24h): **{globalStats.ActivePlayers}**\n";
        text += $"💰 Oro en circulación: **{globalStats.TotalGoldCirculating:N0}**\n";
        text += $"⚔️ Enemigos derrotados: **{globalStats.TotalEnemiesDefeated:N0}**\n";
        text += $"👹 Jefes eliminados: **{globalStats.TotalBossesDefeated:N0}**\n\n";
        text += "━━━━━━━━━━━━━━━\n\n";
        text += "📋 **Selecciona un ranking:**";

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⭐ Por Nivel", "leaderboard_level"),
                InlineKeyboardButton.WithCallbackData("💰 Por Oro", "leaderboard_gold")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⚔️ Por Kills", "leaderboard_kills"),
                InlineKeyboardButton.WithCallbackData("👹 Por Jefes", "leaderboard_boss_kills")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("💥 Por Daño", "leaderboard_damage"),
                InlineKeyboardButton.WithCallbackData("🐾 Por Mascotas", "leaderboard_pets")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✨ Por Skills", "leaderboard_skills"),
                InlineKeyboardButton.WithCallbackData("👤 Mi Perfil", "leaderboard_my_profile")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
            }
        });

        await bot.SendMessage(
            chatId,
            text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: ct
        );
    }

    public static string FormatLeaderboard(List<LeaderboardEntry> entries, string title, string valueLabel)
    {
        var text = $"🏆 **{title}**\n\n";

        if (!entries.Any())
        {
            text += "❌ No hay datos todavía.\n";
            return text;
        }

        foreach (var entry in entries)
        {
            var medal = entry.Rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => $"{entry.Rank}."
            };

            var classEmoji = entry.Class switch
            {
                "Warrior" => "⚔️",
                "Mage" => "🔮",
                "Ranger" => "🏹",
                "Cleric" => "✨",
                _ => "🎮"
            };

            text += $"{medal} **{entry.PlayerName}** {classEmoji} Lv.{entry.Level}\n";
            text += $"   {valueLabel}: **{entry.Value:N0}**";
            
            if (!string.IsNullOrEmpty(entry.Username))
            {
                text += $" • @{entry.Username}";
            }
            
            text += "\n\n";
        }

        return text;
    }
}
