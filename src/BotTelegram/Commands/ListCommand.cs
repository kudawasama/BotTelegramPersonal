using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;
using System.Text;

namespace BotTelegram.Commands
{
    public class ListCommand
    {
        private readonly ReminderService _service = new();

        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            try
            {
                Console.WriteLine($"[ListCommand] Obteniendo recordatorios para ChatId {message.Chat.Id}");
                
                var allReminders = _service.GetAll()
                    .Where(r => r.ChatId == message.Chat.Id)
                    .OrderBy(r => r.DueAt)
                    .ToList();

                Console.WriteLine($"[ListCommand] Total encontrados: {allReminders.Count}");

                if (!allReminders.Any())
                {
                    await bot.SendMessage(
                        message.Chat.Id,
                        "📭 No tienes recordatorios guardados.\n\nUsa /remember para crear uno.",
                        cancellationToken: ct);
                    return;
                }

                var pendientes = allReminders.Where(r => !r.Notified).ToList();
                var completados = allReminders.Where(r => r.Notified).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("📋 *TUS RECORDATORIOS*\n");

                // Crear lista de botones para los pendientes
                var buttons = new List<List<InlineKeyboardButton>>();

                if (pendientes.Any())
                {
                    sb.AppendLine("⏰ *PENDIENTES:*");
                    foreach (var r in pendientes)
                    {
                        var timeLeft = r.DueAt - DateTimeOffset.Now;
                        var timeStr = FormatTimeLeft(timeLeft);
                        var recurrenceStr = r.Recurrence != BotTelegram.Models.RecurrenceType.None ? $" 🔄 {r.Recurrence}" : "";
                        
                        sb.AppendLine($"• `{r.Id}` - {r.Text}");
                        sb.AppendLine($"  ⏰ {r.DueAt:dd/MM HH:mm} ({timeStr}){recurrenceStr}");
                        sb.AppendLine();

                        // Agregar botones para cada recordatorio
                        buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData($"🗑️ {r.Id}", $"delete:{r.Id}"),
                            InlineKeyboardButton.WithCallbackData($"🔄 Recurrente", $"recur:{r.Id}")
                        });
                    }
                }
                else
                {
                    sb.AppendLine("✅ No hay recordatorios pendientes\n");
                }

                if (completados.Any())
                {
                    sb.AppendLine("\n✅ *COMPLETADOS (últimos 5):*");
                    foreach (var r in completados.TakeLast(5))
                    {
                        sb.AppendLine($"• ~~{r.Text}~~");
                        sb.AppendLine($"  ✓ {r.DueAt:dd/MM HH:mm}");
                    }
                }

                sb.AppendLine("\n💡 *Comandos útiles:*");
                sb.AppendLine("`/delete <id>` - Eliminar");
                sb.AppendLine("`/edit <id> <texto>` - Modificar");
                sb.AppendLine("`/recur <id> <tipo>` - Recurrencia");

                // Agregar botón de menú principal al final
                buttons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                });

                var keyboard = new InlineKeyboardMarkup(buttons);

                await bot.SendMessage(
                    message.Chat.Id,
                    sb.ToString(),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);

                Console.WriteLine("[ListCommand] ✅ Lista enviada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ListCommand] ❌ Error: {ex.Message}");
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Error al obtener recordatorios. Intenta de nuevo.",
                    cancellationToken: ct);
            }
        }

        private string FormatTimeLeft(TimeSpan timeLeft)
        {
            if (timeLeft.TotalDays >= 1)
                return $"en {(int)timeLeft.TotalDays} días";
            if (timeLeft.TotalHours >= 1)
                return $"en {(int)timeLeft.TotalHours}h";
            if (timeLeft.TotalMinutes >= 1)
                return $"en {(int)timeLeft.TotalMinutes} min";
            if (timeLeft.TotalSeconds > 0)
                return $"en {(int)timeLeft.TotalSeconds}s";
            return "vencido";
        }
    }
}
