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
            
            // Crear menú principal con acciones
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⏰ Crear Recordatorio", "show_remember_help"),
                    InlineKeyboardButton.WithCallbackData("📋 Ver Lista", "list")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🕐 Atajos Rápidos", "quick_times"),
                    InlineKeyboardButton.WithCallbackData("❓ Ayuda", "help")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📚 FAQ / Manual", "faq_menu")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 *¡Bienvenido al Bot de Recordatorios!*\n\n" +
                      "✨ Soy tu asistente personal para recordatorios.\n" +
                      "Nunca más olvidarás algo importante.\n\n" +
                      "🎯 *Elige una opción:*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );
            Console.WriteLine($"   [StartCommand] ✅ Mensaje enviado");
        }
    }
}

