using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;

namespace BotTelegram.Handlers
{
    public static class MessageHandler
    {
        public static async Task Handle(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            // Null check crítico: evita crashes con fotos/stickers/videos
            if (message.Text == null)
            {
                Console.WriteLine($"   [MessageHandler] ⚠️ Mensaje sin texto (media) de ChatId {message.Chat.Id}");
                return;
            }

            Console.WriteLine($"   [MessageHandler] Mensaje de ChatId {message.Chat.Id}: {message.Text}");

            if (AIService.IsChatMode(message.Chat.Id) && !message.Text.StartsWith("/"))
            {
                var aiService = new AIService();

                await bot.SendChatAction(
                    message.Chat.Id,
                    Telegram.Bot.Types.Enums.ChatAction.Typing,
                    cancellationToken: ct);

                var response = await aiService.Chat(message.Chat.Id, message.Text);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🔄 Reiniciar chat", "clear_chat"),
                        InlineKeyboardButton.WithCallbackData("🚪 Salir del chat", "exit_chat")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                    }
                });

                // Visual feedback: indicador verde para modo chat activo
                await bot.SendMessage(
                    message.Chat.Id,
                    $"🟢 *Modo Chat*\n\n{response}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // Comandos normales
            await CommandHandler.Handle(bot, message, ct);
        }
    }
}

    