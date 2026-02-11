using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;

namespace BotTelegram.Commands
{
    public class ChatCommand
    {
        private readonly AIService _aiService = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var userMessage = message.Text!.Replace("/chat", "").Trim();

            Console.WriteLine($"[ChatCommand] 💬 Procesando chat de ChatId {chatId}");

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("💡 Ver ejemplos", "show_chat_help"),
                        InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                    }
                });

                await bot.SendMessage(
                    chatId,
                    "💬 *Modo Chat IA*\n\n" +
                    "Escribe algo después de /chat para conversar conmigo.\n\n" +
                    "📝 *Ejemplos:*\n" +
                    "• `/chat Hola, ¿cómo estás?`\n" +
                    "• `/chat ¿Qué tengo pendiente hoy?`\n" +
                    "• `/chat Explícame cómo usar el bot`\n" +
                    "• `/chat Tengo reunión mañana`\n\n" +
                    "💡 *Tip:* Recuerdo el contexto de nuestra conversación.\n" +
                    "Para reiniciar escribe: `/chat reiniciar`",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            try
            {
                // Mostrar indicador "escribiendo..."
                await bot.SendChatAction(
                    chatId,
                    Telegram.Bot.Types.Enums.ChatAction.Typing,
                    cancellationToken: ct);

                Console.WriteLine($"[ChatCommand] 🔄 Enviando a AIService: '{userMessage.Substring(0, Math.Min(50, userMessage.Length))}...'");

                // Obtener respuesta de la IA
                var response = await _aiService.Chat(chatId, userMessage);

                Console.WriteLine($"[ChatCommand] ✅ Respuesta recibida, enviando a usuario");

                // Botón de menú principal
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🔄 Reiniciar chat", "clear_chat"),
                        InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                    }
                });

                // Enviar respuesta
                await bot.SendMessage(
                    chatId,
                    response,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);

                Console.WriteLine($"[ChatCommand] ✅ Chat completado para ChatId {chatId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatCommand] ❌ Error: {ex.Message}");
                
                await bot.SendMessage(
                    chatId,
                    "❌ *Error al procesar tu mensaje*\n\n" +
                    "Hubo un problema al comunicarme con el servicio de IA.\n" +
                    "Por favor intenta de nuevo en un momento.\n\n" +
                    "Si el problema persiste, usa los comandos tradicionales como `/list`, `/remember`, etc.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct);
            }
        }
    }
}
