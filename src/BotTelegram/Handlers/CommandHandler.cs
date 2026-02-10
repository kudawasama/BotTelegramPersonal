using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Core;

namespace BotTelegram.Handlers
{
    public static class CommandHandler
    {
        public static async Task Handle(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            await CommandRouter.Route(bot, message, ct);
        }
    }
}

