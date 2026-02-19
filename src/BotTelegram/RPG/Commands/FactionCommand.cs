using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotTelegram.RPG.Services;
using System.Text;

namespace BotTelegram.RPG.Commands
{
    public class FactionCommand
    {
        private readonly RpgService _rpgService;
        private readonly FactionService _factionService;
        
        public FactionCommand()
        {
            _rpgService = new RpgService();
            _factionService = new FactionService();
        }
        
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var player = _rpgService.GetPlayer(message.Chat.Id);
            if (player == null)
            {
                await bot.SendMessage(message.Chat.Id, "❌ Usa /rpg primero.", cancellationToken: ct);
                return;
            }
            
            var sb = new StringBuilder();
            sb.AppendLine("🏛️ **TUS REPUTACIONES**\n");
            
            var factions = FactionDatabase.GetAllFactions();
            foreach (var faction in factions.Take(8))
            {
                var rep = _factionService.GetReputation(player, faction.Id);
                var repValue = rep?.Reputation ?? 0;
                var tier = FactionDatabase.GetTierFromReputation(repValue);
                var tierName = FactionDatabase.GetTierName(tier);
                var emoji = FactionDatabase.GetTierEmoji(tier);
                
                sb.AppendLine($"{faction.Emoji} **{faction.Name}**");
                sb.AppendLine($"{emoji} {tierName} ({repValue:+#;-#;0})");
                
                if (rep != null)
                {
                    var progress = rep.GetProgressToNextTier();
                    sb.AppendLine($"📊 {progress:F0}% al siguiente tier\n");
                }
                else
                {
                    sb.AppendLine($"📊 Sin reputación\n");
                }
            }
            
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("💡 Gana reputación completando quests y derrotando enemigos de facciones enemigas.");
            
            await bot.SendMessage(message.Chat.Id, sb.ToString(), parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
