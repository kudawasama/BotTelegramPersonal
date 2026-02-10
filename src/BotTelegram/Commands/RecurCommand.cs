using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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

            // Si solo viene el ID, mostrar opciones con botones
            var parts = input.Split(' ');
            if (parts.Length == 1 && !string.IsNullOrWhiteSpace(parts[0]))
            {
                var reminderId = parts[0];
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

                // Mostrar opciones de recurrencia con botones
                var text = $"🔄 Selecciona la recurrencia para:\n\n📝 {reminder.Text}\n⏰ {reminder.DueAt:dd/MM HH:mm}\n\n🔁 Recurrencia actual: {reminder.Recurrence}";
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("📅 Diaria", $"set_recur:{reminderId}:Daily"),
                        InlineKeyboardButton.WithCallbackData("📆 Semanal", $"set_recur:{reminderId}:Weekly")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("📊 Mensual", $"set_recur:{reminderId}:Monthly"),
                        InlineKeyboardButton.WithCallbackData("🗓️ Anual", $"set_recur:{reminderId}:Yearly")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("❌ Sin recurrencia", $"set_recur:{reminderId}:None")
                    }
                });

                await bot.SendMessage(
                    message.Chat.Id,
                    text,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // Formato: /recur <id> <daily|weekly|monthly|yearly|none>
            if (parts.Length < 2)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Uso:\n/recur <id> [daily|weekly|monthly|yearly|none]\n\nEjemplo: /recur abc123 daily\n\nO simplemente: /recur <id> para ver opciones",
                    cancellationToken: ct);
                return;
            }

            var recReminderId = parts[0];
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

            var allReminders = _service.GetAll();
            var targetReminder = allReminders.FirstOrDefault(r => r.Id == recReminderId && r.ChatId == message.Chat.Id);

            if (targetReminder == null)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    $"❌ No encontré un recordatorio con ID: {recReminderId}",
                    cancellationToken: ct);
                return;
            }

            targetReminder.Recurrence = recurrence;
            _service.UpdateAll(allReminders);

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
                $"✅ Recurrencia actualizada:\n🔄 Tipo: {recurStr}\n📝 {targetReminder.Text}",
                cancellationToken: ct);

            Console.WriteLine($"   [RecurCommand] ✅ Recurrencia de {recReminderId} establecida a {recurrence}");
        }
    }
}
