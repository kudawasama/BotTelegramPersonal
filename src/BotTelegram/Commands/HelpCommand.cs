using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;

namespace BotTelegram.Commands
{
    public class HelpCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            // Crear botones con todas las acciones
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⏰ Crear", "show_remember_help"),
                    InlineKeyboardButton.WithCallbackData("📋 Lista", "list"),
                    InlineKeyboardButton.WithCallbackData("🕐 Rápidos", "quick_times")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✏️ Editar", "help_edit"),
                    InlineKeyboardButton.WithCallbackData("🗑️ Eliminar", "help_delete"),
                    InlineKeyboardButton.WithCallbackData("🔄 Recurrente", "help_recur")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text:
@"📚 *AYUDA - Bot de Recordatorios*

*✅ CREAR RECORDATORIOS:*
`/remember <texto> en <tiempo>`

*📝 Ejemplos:*
• `/remember Tomar agua en 10 min`
• `/remember Reunión mañana a las 14:30`
• `/remember Viaje en 3 días`
• `/remember Llamar mamá hoy a las 19:00`

*🕐 Tiempos soportados:*
• `en 10 segundos` / `en 5 min`
• `en 2 horas` / `en 3 días`
• `hoy a las 18:00`
• `mañana a las 09:00`

*📋 GESTIONAR:*
• `/list` - Ver todos los recordatorios
• `/delete <id>` - Eliminar uno
• `/edit <id> <texto>` - Modificar
• `/recur <id> <tipo>` - Hacer recurrente

*🎯 Click en los botones abajo para acciones rápidas*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );
        }
    }
}

