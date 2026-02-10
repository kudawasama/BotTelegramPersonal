using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;

namespace BotTelegram.Commands
{
    public class StartCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [StartCommand] Enviando mensaje de bienvenida");
            
            // Crear botones inline para acceso rápido
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📋 Ver mis recordatorios", "list"),
                    InlineKeyboardButton.WithCallbackData("❓ Ayuda", "help")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 ¡Bienvenido al Bot de Recordatorios!\n\n" +
                      "Puedo ayudarte a crear y gestionar recordatorios.\n\n" +
                      "🚀 Acciones rápidas:",
                replyMarkup: keyboard,
                cancellationToken: ct
            );
            Console.WriteLine($"   [StartCommand] ✅ Mensaje enviado");
        }
    }
}

