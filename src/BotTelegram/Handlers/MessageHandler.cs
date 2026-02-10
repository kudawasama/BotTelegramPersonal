using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTelegram.Handlers
{
    public static class MessageHandler
    {
        public static async Task Handle(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [MessageHandler] Mensaje de ChatId {message.Chat.Id}: {message.Text}");
            // Por ahora todo pasa por el CommandHandler
            await CommandHandler.Handle(bot, message, ct);
        }
    }
}

    