using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;
using BotTelegram.Services;

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
            
            // 🎯 LOG: Registrar comando /start
            TelegramLogger.LogUserAction(
                chatId: message.Chat.Id,
                username: message.From?.Username ?? "unknown",
                action: "/start",
                details: "Menu principal desplegado"
            );
            
            // Menú principal reorganizado por categorías
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📅 RECORDATORIOS", "menu_reminders")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🤖 INTELIGENCIA ARTIFICIAL", "menu_ai")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("ℹ️ AYUDA E INFORMACIÓN", "menu_info")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 *¡Bienvenido al Bot Multifuncional!*\n\n" +
                      "✨ Tu asistente personal todo-en-uno:\n" +
                      "• Recordatorios inteligentes\n" +
                      "• Chat con IA avanzada\n" +
                      "• Juego RPG inmersivo\n\n" +
                      "🎯 *Selecciona una categoría:*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );
            Console.WriteLine($"   [StartCommand] ✅ Mensaje enviado");
        }
    }
}

