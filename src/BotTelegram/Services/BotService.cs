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

            // Procesar mensajes
            if (update.Type == UpdateType.Message)
            {
                if (update.Message!.Text == null)
                {
                    Console.WriteLine($"   [BotService] Message sin texto");
                    return;
                }

                Console.WriteLine($"   [BotService] Enviando a MessageHandler: {update.Message.Text}");
                await MessageHandler.Handle(bot, update.Message, ct);
                return;
            }

            // Procesar callbacks de botones inline
            if (update.Type == UpdateType.CallbackQuery)
            {
                Console.WriteLine($"   [BotService] Enviando a CallbackQueryHandler: {update.CallbackQuery!.Data}");
                await CallbackQueryHandler.Handle(bot, update.CallbackQuery!, ct);
                return;
            }

            Console.WriteLine($"   [BotService] Update ignorado (tipo no soportado)");
        }
    }
}

