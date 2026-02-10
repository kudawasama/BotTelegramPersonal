using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Services;

namespace BotTelegram.Handlers
{
    public static class CallbackQueryHandler
    {
        private static readonly ReminderService _reminderService = new();

        public static async Task Handle(
            ITelegramBotClient bot,
            CallbackQuery callbackQuery,
            CancellationToken ct)
        {
            Console.WriteLine($"   [CallbackQueryHandler] Callback recibido: {callbackQuery.Data}");

            if (string.IsNullOrWhiteSpace(callbackQuery.Data) || callbackQuery.Message == null)
                return;

            var data = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            try
            {
                // Responder al callback para quitar el loading
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);

                // Procesar diferentes tipos de callbacks
                if (data == "help")
                {
                    await HandleHelpCallback(bot, chatId, messageId, ct);
                }
                else if (data == "list")
                {
                    await HandleListCallback(bot, chatId, messageId, ct);
                }
                else if (data.StartsWith("delete:"))
                {
                    await HandleDeleteCallback(bot, chatId, messageId, data, ct);
                }
                else if (data.StartsWith("confirm_delete:"))
                {
                    await HandleConfirmDeleteCallback(bot, callbackQuery, data, ct);
                }
                else if (data.StartsWith("cancel_delete:"))
                {
                    await HandleCancelDeleteCallback(bot, chatId, messageId, data, ct);
                }
                else if (data.StartsWith("recur:"))
                {
                    await HandleRecurCallback(bot, chatId, messageId, data, ct);
                }
                else if (data.StartsWith("set_recur:"))
                {
                    await HandleSetRecurCallback(bot, callbackQuery, data, ct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [CallbackQueryHandler] Error: {ex.Message}");
                await bot.SendMessage(chatId, "❌ Ocurrió un error procesando tu solicitud.", cancellationToken: ct);
            }
        }

        private static async Task HandleHelpCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var helpText = @"📌 Comandos disponibles:

✅ Crear recordatorios:
/remember <texto> en <tiempo> - Crear recordatorio
  Ejemplos: /remember Tomar agua en 10 min
           /remember Reunión mañana a las 14:30
           /remember Viaje en 3 días

📋 Ver y gestionar:
/list - Listar recordatorios pendientes
/delete <id> - Eliminar un recordatorio
/edit <id> <nuevo texto> - Modificar un recordatorio

❓ Otros:
/start - Iniciar el bot
/help - Ver este mensaje

🕐 Formatos de tiempo soportados:
- en 10 segundos / en 5 min / en 2 horas / en 3 días
- hoy a las 18:00
- mañana a las 09:00";

            await bot.EditMessageText(chatId, messageId, helpText, cancellationToken: ct);
        }

        private static async Task HandleListCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var reminders = _reminderService.GetAll()
                .Where(r => r.ChatId == chatId && !r.Notified)
                .OrderBy(r => r.DueAt)
                .ToList();

            if (!reminders.Any())
            {
                await bot.EditMessageText(chatId, messageId, "📭 No tienes recordatorios pendientes.", cancellationToken: ct);
                return;
            }

            var text = "📝 Tus recordatorios:\n\n";
            var buttons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();

            foreach (var r in reminders)
            {
                var recurrenceStr = r.Recurrence != Models.RecurrenceType.None ? $" [🔄 {r.Recurrence}]" : "";
                text += $"🔹 `{r.Id}`\n⏰ {r.DueAt:dd/MM HH:mm} - {r.Text}{recurrenceStr}\n\n";

                // Agregar botones para este recordatorio
                buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"🗑️ Eliminar {r.Id}", $"delete:{r.Id}"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"🔄 Recurrencia", $"recur:{r.Id}")
                });
            }

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);
            await bot.EditMessageText(
                chatId, 
                messageId, 
                text, 
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleDeleteCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            var reminderId = data.Replace("delete:", "");
            var reminders = _reminderService.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == reminderId && r.ChatId == chatId);

            if (reminder == null)
            {
                await bot.EditMessageText(chatId, messageId, $"❌ No encontré un recordatorio con ID: {reminderId}", cancellationToken: ct);
                return;
            }

            // Mostrar confirmación
            var text = $"⚠️ ¿Estás seguro de eliminar este recordatorio?\n\n📝 {reminder.Text}\n⏰ {reminder.DueAt:dd/MM HH:mm}";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí, eliminar", $"confirm_delete:{reminderId}"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Cancelar", $"cancel_delete:{reminderId}")
                }
            });

            await bot.EditMessageText(chatId, messageId, text, replyMarkup: keyboard, cancellationToken: ct);
        }

        private static async Task HandleConfirmDeleteCallback(
            ITelegramBotClient bot,
            CallbackQuery callbackQuery,
            string data,
            CancellationToken ct)
        {
            var reminderId = data.Replace("confirm_delete:", "");
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            var reminders = _reminderService.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == reminderId && r.ChatId == chatId);

            if (reminder == null)
            {
                await bot.EditMessageText(chatId, messageId, $"❌ No encontré un recordatorio con ID: {reminderId}", cancellationToken: ct);
                return;
            }

            reminders.Remove(reminder);
            _reminderService.UpdateAll(reminders);

            await bot.EditMessageText(
                chatId,
                messageId,
                $"✅ Recordatorio eliminado:\n📝 {reminder.Text}",
                cancellationToken: ct);

            // Mostrar un mensaje de feedback con animación
            await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Recordatorio eliminado", cancellationToken: ct);

            Console.WriteLine($"   [CallbackQueryHandler] ✅ Recordatorio {reminderId} eliminado");
        }

        private static async Task HandleCancelDeleteCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            await bot.EditMessageText(chatId, messageId, "❌ Eliminación cancelada. Usa /list para ver tus recordatorios.", cancellationToken: ct);
        }

        private static async Task HandleRecurCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            var reminderId = data.Replace("recur:", "");
            var reminders = _reminderService.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == reminderId && r.ChatId == chatId);

            if (reminder == null)
            {
                await bot.EditMessageText(chatId, messageId, $"❌ No encontré un recordatorio con ID: {reminderId}", cancellationToken: ct);
                return;
            }

            // Mostrar opciones de recurrencia
            var text = $"🔄 Selecciona la recurrencia para:\n\n📝 {reminder.Text}\n⏰ {reminder.DueAt:dd/MM HH:mm}\n\n🔁 Recurrencia actual: {reminder.Recurrence}";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Diaria", $"set_recur:{reminderId}:Daily"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📆 Semanal", $"set_recur:{reminderId}:Weekly")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Mensual", $"set_recur:{reminderId}:Monthly"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗓️ Anual", $"set_recur:{reminderId}:Yearly")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Sin recurrencia", $"set_recur:{reminderId}:None")
                }
            });

            await bot.EditMessageText(chatId, messageId, text, replyMarkup: keyboard, cancellationToken: ct);
        }

        private static async Task HandleSetRecurCallback(
            ITelegramBotClient bot,
            CallbackQuery callbackQuery,
            string data,
            CancellationToken ct)
        {
            var parts = data.Replace("set_recur:", "").Split(':');
            if (parts.Length != 2)
            {
                await bot.SendMessage(callbackQuery.Message!.Chat.Id, "❌ Error procesando la recurrencia.", cancellationToken: ct);
                return;
            }

            var reminderId = parts[0];
            var recurrenceType = parts[1];
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            var reminders = _reminderService.GetAll();
            var reminder = reminders.FirstOrDefault(r => r.Id == reminderId && r.ChatId == chatId);

            if (reminder == null)
            {
                await bot.EditMessageText(chatId, messageId, $"❌ No encontré un recordatorio con ID: {reminderId}", cancellationToken: ct);
                return;
            }

            // Actualizar recurrencia
            if (Enum.TryParse<Models.RecurrenceType>(recurrenceType, out var recurrence))
            {
                reminder.Recurrence = recurrence;
                _reminderService.UpdateAll(reminders);

                var recurrenceIcon = recurrence switch
                {
                    Models.RecurrenceType.Daily => "📅",
                    Models.RecurrenceType.Weekly => "📆",
                    Models.RecurrenceType.Monthly => "📊",
                    Models.RecurrenceType.Yearly => "🗓️",
                    _ => "❌"
                };

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    $"✅ Recurrencia actualizada: {recurrenceIcon} {recurrence}\n\n📝 {reminder.Text}\n⏰ {reminder.DueAt:dd/MM HH:mm}",
                    cancellationToken: ct);

                await bot.AnswerCallbackQuery(callbackQuery.Id, $"✅ Recurrencia: {recurrence}", cancellationToken: ct);

                Console.WriteLine($"   [CallbackQueryHandler] ✅ Recurrencia actualizada para {reminderId}: {recurrence}");
            }
            else
            {
                await bot.EditMessageText(chatId, messageId, "❌ Tipo de recurrencia inválido.", cancellationToken: ct);
            }
        }
    }
}
