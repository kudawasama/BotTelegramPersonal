using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Services;
using BotTelegram.Models;
using System.Text.RegularExpressions;

namespace BotTelegram.Commands
{
    public class RememberCommand
    {
        private readonly ReminderService _service = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [RememberCommand] Procesando: {message.Text}");
            var input = message.Text!.Replace("/remember", "").Trim();

            // Validación de entrada
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"   [RememberCommand] ❌ Entrada vacía");
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Uso:\n/remember Tomar agua en 10 min\n/remember Reunión mañana a las 14:30\n/remember Llamar mamá hoy a las 19:00",
                    cancellationToken: ct);
                return;
            }
            
            // Validación: longitud mínima y máxima
            if (input.Length < 3)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ El recordatorio debe tener al menos 3 caracteres.",
                    cancellationToken: ct);
                return;
            }
            
            if (input.Length > 500)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    $"❌ El recordatorio es demasiado largo ({input.Length} caracteres). Máximo: 500 caracteres.",
                    cancellationToken: ct);
                return;
            }

            Console.WriteLine($"   [RememberCommand] Parseando: '{input}'");
            var (taskText, dueAt) = ParseReminder(input);
            Console.WriteLine($"   [RememberCommand] Task: '{taskText}' | DueAt: {dueAt:dd/MM/yyyy HH:mm:ss}");

            if (dueAt < DateTimeOffset.Now)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ La fecha no puede ser en el pasado",
                    cancellationToken: ct);
                return;
            }

            var reminder = new 
            Reminder
            {
                ChatId = message.Chat.Id,
                Text = taskText,
                DueAt = dueAt
            };

            _service.Save(reminder);

            var formattedTime = dueAt.ToString("dd/MM/yyyy HH:mm");
            
            // Botones de acciones post-creación
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Hacer Recurrente", $"recur:{reminder.Id}"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Todos", "list")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➕ Crear Otro", "show_remember_help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });
            
            await bot.SendMessage(
                message.Chat.Id,
                $"✅ *Recordatorio creado!*\n\n" +
                $"📝 {taskText}\n" +
                $"⏰ {formattedTime}\n" +
                $"🆔 `{reminder.Id}`\n\n" +
                $"💡 *¿Qué quieres hacer ahora?*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private (string taskText, DateTimeOffset dueAt) ParseReminder(string input)
        {
            // Patrones soportados:
            // "en 10 seg/segundos", "en 10 min/minutos", "en 1 hora", "en 2 días", "mañana", "hoy a las 14:30", etc.

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

            // Patrón: "mañana a las HH:mm" o solo "mañana"
            if (Regex.IsMatch(input, @"mañana", RegexOptions.IgnoreCase))
            {
                var timeMatch = Regex.Match(input, @"a\s+las\s+(\d{1,2}):(\d{2})", RegexOptions.IgnoreCase);
                var task = Regex.Replace(input, @"mañana\s*(a\s+las\s+\d{1,2}:\d{2})?", "", RegexOptions.IgnoreCase).Trim();

                if (timeMatch.Success && int.TryParse(timeMatch.Groups[1].Value, out var hour) && 
                    int.TryParse(timeMatch.Groups[2].Value, out var minute))
                {
                    var tomorrow = now.AddDays(1).Date.AddHours(hour).AddMinutes(minute);
                    return (task, tomorrow);
                }

                // Mañana a las 9:00 (default)
                return (task, now.AddDays(1).Date.AddHours(9));
            }

            // Patrón: "hoy a las HH:mm"
            if (Regex.IsMatch(input, @"hoy", RegexOptions.IgnoreCase))
            {
                var timeMatch = Regex.Match(input, @"a\s+las\s+(\d{1,2}):(\d{2})", RegexOptions.IgnoreCase);
                var task = Regex.Replace(input, @"hoy\s*(a\s+las\s+\d{1,2}:\d{2})?", "", RegexOptions.IgnoreCase).Trim();

                if (timeMatch.Success && int.TryParse(timeMatch.Groups[1].Value, out var hour) && 
                    int.TryParse(timeMatch.Groups[2].Value, out var minute))
                {
                    var today = now.Date.AddHours(hour).AddMinutes(minute);
                    return (task, today);
                }

                return (task, now.AddHours(1)); // Por defecto en 1 hora
            }

            // Patrón: "YYYY-MM-DD HH:mm" (compatibilidad con formato anterior)
            if (DateTime.TryParse(input, out var parsedDate))
            {
                return (input, parsedDate);
            }

            // Por defecto: en 1 hora
            return (input, now.AddHours(1));
        }
    }
}

