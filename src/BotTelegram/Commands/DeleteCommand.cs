using Telegram.Bot;
using Telegram.Bot.Types;
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

            reminders.Remove(reminder);
            _service.UpdateAll(reminders);

            await bot.SendMessage(
                message.Chat.Id,
                $"✅ Recordatorio eliminado:\n📝 {reminder.Text}",
                cancellationToken: ct);

            Console.WriteLine($"   [DeleteCommand] ✅ Recordatorio {input} eliminado");
        }
    }
}
