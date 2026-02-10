using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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
                await bot.SendMessage(message.Chat.Id, "📭 No tienes recordatorios pendientes.", cancellationToken: ct);
                return;
            }

            var text = "📝 Tus recordatorios:\n\n";
            var buttons = new List<List<InlineKeyboardButton>>();

            foreach (var r in reminders)
            {
                var recurrenceStr = r.Recurrence != BotTelegram.Models.RecurrenceType.None ? $" [🔄 {r.Recurrence}]" : "";
                text += $"🔹 `{r.Id}`\n⏰ {r.DueAt:dd/MM HH:mm} - {r.Text}{recurrenceStr}\n\n";

                // Agregar botones para este recordatorio
                buttons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData($"🗑️ Eliminar {r.Id}", $"delete:{r.Id}"),
                    InlineKeyboardButton.WithCallbackData($"🔄 Recurrencia", $"recur:{r.Id}")
                });
            }

            var keyboard = new InlineKeyboardMarkup(buttons);

            await bot.SendMessage(
                message.Chat.Id, 
                text, 
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
    }
}
