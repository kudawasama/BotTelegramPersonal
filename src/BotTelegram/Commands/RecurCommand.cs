using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Services;
using BotTelegram.Models;

namespace BotTelegram.Commands
{
    public class RecurCommand
    {
        private readonly ReminderService _service = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [RecurCommand] Procesando: {message.Text}");
            var input = message.Text!.Replace("/recur", "").Trim();

            // Formato: /recur <id> <daily|weekly|monthly|yearly|none>
            var parts = input.Split(' ');
            if (parts.Length < 2)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Uso:\n/recur <id> <daily|weekly|monthly|yearly|none>\n\nEjemplo: /recur abc123 daily",
                    cancellationToken: ct);
                return;
            }

            var reminderId = parts[0];
            var recurrenceStr = parts[1].ToLower();

            var recurrence = recurrenceStr switch
            {
                "daily" => RecurrenceType.Daily,
                "weekly" => RecurrenceType.Weekly,
                "monthly" => RecurrenceType.Monthly,
                "yearly" => RecurrenceType.Yearly,
                "none" => RecurrenceType.None,
                _ => RecurrenceType.None
            };

            var reminders = _service.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == reminderId && r.ChatId == message.Chat.Id);

            if (reminder == null)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    $"❌ No encontré un recordatorio con ID: {reminderId}",
                    cancellationToken: ct);
                return;
            }

            reminder.Recurrence = recurrence;
            _service.UpdateAll(reminders);

            var recurStr = recurrence switch
            {
                RecurrenceType.Daily => "Diario",
                RecurrenceType.Weekly => "Semanal",
                RecurrenceType.Monthly => "Mensual",
                RecurrenceType.Yearly => "Anual",
                _ => "Una sola vez"
            };

            await bot.SendMessage(
                message.Chat.Id,
                $"✅ Recurrencia actualizada:\n🔄 Tipo: {recurStr}\n📝 {reminder.Text}",
                cancellationToken: ct);

            Console.WriteLine($"   [RecurCommand] ✅ Recurrencia de {reminderId} establecida a {recurrence}");
        }
    }
}
