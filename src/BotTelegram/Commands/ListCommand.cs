using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Services;

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
            var reminders = _service.GetAll()
                .Where(r => r.ChatId == message.Chat.Id && !r.Notified)
                .OrderBy(r => r.DueAt)
                .ToList();

            if (!reminders.Any())
            {
                await bot.SendMessage(message.Chat.Id, "📭 No tienes recuerdos pendientes.", cancellationToken: ct);
                return;
            }

            var text = "📝 Tus recuerdos:\n\n";
            foreach (var r in reminders)
            {
                var recurrenceStr = r.Recurrence != BotTelegram.Models.RecurrenceType.None ? $" [🔄 {r.Recurrence}]" : "";
                text += $"🔹 `{r.Id}`\n⏰ {r.DueAt:dd/MM HH:mm} - {r.Text}{recurrenceStr}\n";
            }

            text += "\n💡 Usa /delete <id> para eliminar\n💡 Usa /edit <id> <nuevo texto> para modificar";

            await bot.SendMessage(message.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
