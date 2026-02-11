using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;

namespace BotTelegram.Commands
{
    public class DeleteCommand
    {
        private readonly ReminderService _service = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [DeleteCommand] Procesando: {message.Text}");
            var input = message.Text!.Replace("/delete", "").Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Uso:\n/delete <id>\n\nUsa /list para ver los IDs",
                    cancellationToken: ct);
                return;
            }

            var reminders = _service.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == input && r.ChatId == message.Chat.Id);

            if (reminder == null)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    $"❌ No encontré un recordatorio con ID: {input}",
                    cancellationToken: ct);
                return;
            }

            // Mostrar confirmación con botones
            var text = $"⚠️ ¿Estás seguro de eliminar este recordatorio?\n\n📝 {reminder.Text}\n⏰ {reminder.DueAt:dd/MM HH:mm}";
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✅ Sí, eliminar", $"confirm_delete:{input}"),
                    InlineKeyboardButton.WithCallbackData("❌ Cancelar", $"cancel_delete:{input}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            await bot.SendMessage(
                message.Chat.Id,
                text,
                replyMarkup: keyboard,
                cancellationToken: ct);

            Console.WriteLine($"   [DeleteCommand] ⚠️ Solicitando confirmación para eliminar {input}");
        }
    }
}
