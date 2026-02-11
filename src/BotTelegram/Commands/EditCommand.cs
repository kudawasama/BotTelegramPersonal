using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;
using BotTelegram.Models;
using System.Text.RegularExpressions;

namespace BotTelegram.Commands
{
    public class EditCommand
    {
        private readonly ReminderService _service = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [EditCommand] Procesando: {message.Text}");
            var input = message.Text!.Replace("/edit", "").Trim();

            // Formato: /edit <id> <nuevo texto> en <tiempo>
            var spaceIndex = input.IndexOf(' ');
            if (spaceIndex < 0)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Uso:\n/edit <id> <nuevo texto> en <tiempo>\n\nEjemplo: /edit abc123 Tomar agua en 10 min",
                    cancellationToken: ct);
                return;
            }

            var reminderId = input.Substring(0, spaceIndex);
            var newInput = input.Substring(spaceIndex + 1);

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

            // Parsear el nuevo tiempo
            var (taskText, newDueAt) = ParseReminder(newInput);

            if (newDueAt < DateTimeOffset.Now)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ La fecha no puede ser en el pasado",
                    cancellationToken: ct);
                return;
            }

            reminder.Text = taskText;
            reminder.DueAt = newDueAt;
            reminder.Notified = false;

            _service.UpdateAll(reminders);

            var formattedTime = newDueAt.ToString("dd/MM/yyyy HH:mm");
            
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Lista", "list"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });
            
            await bot.SendMessage(
                message.Chat.Id,
                $"✅ Recordatorio actualizado:\n📝 {taskText}\n⏰ {formattedTime}",
                replyMarkup: keyboard,
                cancellationToken: ct);

            Console.WriteLine($"   [EditCommand] ✅ Recordatorio {reminderId} actualizado");
        }

        private (string taskText, DateTimeOffset dueAt) ParseReminder(string input)
        {
            var now = DateTimeOffset.Now;

            // Patrón: "en X seg/segundo/segundos" O solo "X segundos"
            var secMatch = Regex.Match(input, @"(?:en\s+)?(\d+)\s+(seg|segundo|segundos)", RegexOptions.IgnoreCase);
            if (secMatch.Success && int.TryParse(secMatch.Groups[1].Value, out var seconds))
            {
                var task = input.Replace(secMatch.Value, "").Trim();
                return (task, now.AddSeconds(seconds));
            }

            // Patrón: "en X min/minutos" O solo "X minutos"
            var minMatch = Regex.Match(input, @"(?:en\s+)?(\d+)\s+(min|minutos?)", RegexOptions.IgnoreCase);
            if (minMatch.Success && int.TryParse(minMatch.Groups[1].Value, out var minutes))
            {
                var task = input.Replace(minMatch.Value, "").Trim();
                return (task, now.AddMinutes(minutes));
            }

            // Patrón: "en X hora/horas"
            var hourMatch = Regex.Match(input, @"en\s+(\d+)\s+(h|hora|horas)", RegexOptions.IgnoreCase);
            if (hourMatch.Success && int.TryParse(hourMatch.Groups[1].Value, out var hours))
            {
                var task = input.Replace(hourMatch.Value, "").Trim();
                return (task, now.AddHours(hours));
            }

            // Patrón: "en X día/días"
            var dayMatch = Regex.Match(input, @"en\s+(\d+)\s+(día|días)", RegexOptions.IgnoreCase);
            if (dayMatch.Success && int.TryParse(dayMatch.Groups[1].Value, out var days))
            {
                var task = input.Replace(dayMatch.Value, "").Trim();
                return (task, now.AddDays(days));
            }

            // Por defecto: en 1 hora
            return (input, now.AddHours(1));
        }
    }
}
