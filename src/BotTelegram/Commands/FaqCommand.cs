using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.Commands
{
    public class FaqCommand
    {
        public static async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⏰ Crear", "faq_crear"),
                    InlineKeyboardButton.WithCallbackData("📋 Listar", "faq_listar"),
                    InlineKeyboardButton.WithCallbackData("✏️ Editar", "faq_editar")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗑️ Eliminar", "faq_eliminar"),
                    InlineKeyboardButton.WithCallbackData("🔄 Recurrente", "faq_recurrente"),
                    InlineKeyboardButton.WithCallbackData("🕐 Atajos", "faq_atajos")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🎯 Modo de Uso General", "faq_general"),
                    InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            var faqText = @"❓ *PREGUNTAS FRECUENTES (FAQ)*

Selecciona un tema para ver información detallada:

🔹 *Funciones Principales:*
• ⏰ **Crear** - Cómo crear recordatorios
• 📋 **Listar** - Ver tus recordatorios
• ✏️ **Editar** - Modificar recordatorios
• 🗑️ **Eliminar** - Borrar recordatorios
• 🔄 **Recurrente** - Repetir recordatorios
• 🕐 **Atajos** - Tiempos rápidos

🔹 *General:*
• 🎯 **Modo de Uso** - Guía completa

👇 *Haz clic en cualquier botón para más info*";

            await bot.SendMessage(
                chatId,
                faqText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);

            Console.WriteLine($"   [FaqCommand] ✅ FAQ enviado al chat {chatId}");
        }
    }
}
