using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotTelegram.Handlers;

namespace BotTelegram.Services
{
    public static class BotService
    {
        public static async Task HandleUpdate(
            ITelegramBotClient bot,
            Update update,
            CancellationToken ct)
        {
            Console.WriteLine($"   [BotService] Tipo de update: {update.Type}");

            if (update.Type != UpdateType.Message)
            {
                Console.WriteLine($"   [BotService] Update ignorado (no es Message)");
                return;
            }

            if (update.Message!.Text == null)
            {
                Console.WriteLine($"   [BotService] Message sin texto");
                return;
            }

            Console.WriteLine($"   [BotService] Enviando a MessageHandler: {update.Message.Text}");
            await MessageHandler.Handle(bot, update.Message, ct);
        }
    }
}

