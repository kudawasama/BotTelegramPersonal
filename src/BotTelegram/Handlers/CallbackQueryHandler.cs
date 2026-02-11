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
                if (data == "start")
                {
                    await HandleStartCallback(bot, chatId, messageId, ct);
                }
                else if (data == "show_remember_help")
                {
                    await HandleShowRememberHelpCallback(bot, chatId, messageId, ct);
                }
                else if (data == "quick_times")
                {
                    await HandleQuickTimesCallback(bot, chatId, messageId, ct);
                }
                else if (data == "help")
                {
                    await HandleHelpCallback(bot, chatId, messageId, ct);
                }
                else if (data == "list")
                {
                    await HandleListCallback(bot, chatId, messageId, ct);
                }
                else if (data.StartsWith("help_"))
                {
                    await HandleSpecificHelpCallback(bot, chatId, messageId, data, ct);
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
                else if (data.StartsWith("quick_remind:"))
                {
                    await HandleQuickRemindCallback(bot, chatId, messageId, data, ct);
                }
                else if (data.StartsWith("faq_"))
                {
                    await HandleFaqCallback(bot, chatId, messageId, data, ct);
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
            // Crear botones con todas las acciones
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⏰ Crear", "show_remember_help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Lista", "list"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕐 Rápidos", "quick_times")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✏️ Editar", "help_edit"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗑️ Eliminar", "help_delete"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Recurrente", "help_recur")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            var helpText = @"📚 *AYUDA - Bot de Recordatorios*

*✅ CREAR RECORDATORIOS:*
`/remember <texto> en <tiempo>`

*📝 Ejemplos:*
• `/remember Tomar agua en 10 min`
• `/remember Reunión mañana a las 14:30`
• `/remember Viaje en 3 días`
• `/remember Llamar mamá hoy a las 19:00`

*🕐 Tiempos soportados:*
• `en 10 segundos` / `en 5 min`
• `en 2 horas` / `en 3 días`
• `hoy a las 18:00`
• `mañana a las 09:00`

*📋 GESTIONAR:*
• `/list` - Ver todos los recordatorios
• `/delete <id>` - Eliminar uno
• `/edit <id> <texto>` - Modificar
• `/recur <id> <tipo>` - Hacer recurrente

*🎯 Click en los botones abajo para acciones rápidas*";

            await bot.EditMessageText(
                chatId, 
                messageId, 
                helpText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleStartCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⏰ Crear Recordatorio", "show_remember_help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Lista", "list")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕐 Atajos Rápidos", "quick_times"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❓ Ayuda", "help")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "👋 *¡Bienvenido al Bot de Recordatorios!*\n\n" +
                "✨ Soy tu asistente personal para recordatorios.\n" +
                "Nunca más olvidarás algo importante.\n\n" +
                "🎯 *Elige una opción:*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleShowRememberHelpCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕐 Usar Atajos Rápidos", "quick_times")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Lista", "list"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "⏰ *CREAR RECORDATORIO*\n\n" +
                "📝 *Escribe tu recordatorio así:*\n" +
                "`/remember <texto> en <tiempo>`\n\n" +
                "💡 *Ejemplos:*\n" +
                "• `/remember Tomar agua en 10 min`\n" +
                "• `/remember Reunión mañana a las 14:30`\n" +
                "• `/remember Llamar a Juan en 2 horas`\n" +
                "• `/remember Comprar comida hoy a las 19:00`\n" +
                "• `/remember Vacaciones en 30 días`\n\n" +
                "🕐 *Tiempos soportados:*\n" +
                "• `en X segundos/min/horas/días`\n" +
                "• `hoy a las HH:MM`\n" +
                "• `mañana a las HH:MM`\n\n" +
                "⚡ *O usa Atajos Rápidos para tiempos comunes*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleQuickTimesCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔥 5 minutos", "quick_remind:5min"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⏱️ 15 minutos", "quick_remind:15min")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕐 1 hora", "quick_remind:1h"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕑 3 horas", "quick_remind:3h")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Mañana 9 AM", "quick_remind:tomorrow9"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🌙 Hoy 20:00", "quick_remind:today20")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✏️ Escribir manualmente", "show_remember_help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "🕐 *ATAJOS RÁPIDOS*\n\n" +
                "Selecciona un tiempo y luego escribe qué recordar:\n\n" +
                "🔥 Ideal para tareas urgentes\n" +
                "📅 Planifica para mañana\n" +
                "🌙 Recordatorios nocturnos",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleSpecificHelpCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            var helpType = data.Replace("help_", "");
            string helpText = "";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Lista", "list"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❓ Ayuda", "help")
                }
            });

            switch (helpType)
            {
                case "edit":
                    helpText = "✏️ *EDITAR RECORDATORIO*\n\n" +
                              "Para modificar un recordatorio:\n" +
                              "`/edit <id> <nuevo texto>`\n\n" +
                              "📝 *Ejemplo:*\n" +
                              "`/edit abc123 Llamar a María en lugar de Juan`\n\n" +
                              "💡 *Nota:* El ID lo ves con `/list`";
                    break;
                case "delete":
                    helpText = "🗑️ *ELIMINAR RECORDATORIO*\n\n" +
                              "Para eliminar un recordatorio:\n" +
                              "`/delete <id>`\n\n" +
                              "📝 *Ejemplo:*\n" +
                              "`/delete abc123`\n\n" +
                              "💡 *Nota:* También puedes usar el botón 🗑️ en `/list`";
                    break;
                case "recur":
                    helpText = "🔄 *RECORDATORIOS RECURRENTES*\n\n" +
                              "Haz que un recordatorio se repita:\n" +
                              "`/recur <id> <tipo>`\n\n" +
                              "📝 *Tipos disponibles:*\n" +
                              "• `daily` - Todos los días\n" +
                              "• `weekly` - Una vez por semana\n" +
                              "• `monthly` - Una vez al mes\n" +
                              "• `yearly` - Una vez al año\n" +
                              "• `none` - Desactivar recurrencia\n\n" +
                              "💡 *Ejemplo:* `/recur abc123 daily`";
                    break;
                default:
                    helpText = "❓ Ayuda no encontrada";
                    break;
            }

            await bot.EditMessageText(
                chatId,
                messageId,
                helpText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
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

        private static async Task HandleQuickRemindCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            var timeCode = data.Replace("quick_remind:", "");
            string timeText = "";
            string commandExample = "";

            switch (timeCode)
            {
                case "5min":
                    timeText = "5 minutos";
                    commandExample = "/remember Tarea urgente en 5 min";
                    break;
                case "15min":
                    timeText = "15 minutos";
                    commandExample = "/remember Revisar correo en 15 min";
                    break;
                case "1h":
                    timeText = "1 hora";
                    commandExample = "/remember Llamada importante en 1 hora";
                    break;
                case "3h":
                    timeText = "3 horas";
                    commandExample = "/remember Preparar cena en 3 horas";
                    break;
                case "tomorrow9":
                    timeText = "mañana a las 09:00";
                    commandExample = "/remember Reunión proyecto mañana a las 09:00";
                    break;
                case "today20":
                    timeText = "hoy a las 20:00";
                    commandExample = "/remember Ver serie favorita hoy a las 20:00";
                    break;
                default:
                    timeText = "tiempo desconocido";
                    commandExample = "/remember <texto> en <tiempo>";
                    break;
            }

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Otros tiempos", "quick_times"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                $"⏰ *Tiempo seleccionado: {timeText}*\n\n" +
                $"📝 *Ahora escribe tu recordatorio así:*\n" +
                $"`{commandExample}`\n\n" +
                $"💡 *Formato:*\n" +
                $"`/remember <tu texto> en {timeText}`",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleFaqCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            string data,
            CancellationToken ct)
        {
            var faqType = data.Replace("faq_", "");
            string faqText = "";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver a FAQ", "faq_menu"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            switch (faqType)
            {
                case "menu":
                    // Volver al menú principal de FAQ
                    var menuKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⏰ Crear", "faq_crear"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Listar", "faq_listar"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✏️ Editar", "faq_editar")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗑️ Eliminar", "faq_eliminar"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Recurrente", "faq_recurrente"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🕐 Atajos", "faq_atajos")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎯 Modo de Uso General", "faq_general"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                        }
                    });

                    faqText = @"❓ *PREGUNTAS FRECUENTES (FAQ)*

Selecciona un tema para ver información detallada:

🔹 *Funciones Principales:*
• ⏰ **Crear** - Cómo crear recordatorios
• 📋 **Listar** - Ver tus recordatorios
• ✏️ **Editar** - Modificar recordatorios
• 🗑️ **Eliminar** - Borrar recordatorios
• 🔄 **Recurrente** - Repetir recordatorios
• 🕐 **Atajos** - Tiempos rápidos

🔹 *General:*
• 🎯 **Modo de Uso** - Guía completa

👇 *Haz clic en cualquier botón para más info*";

                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        faqText,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: menuKeyboard,
                        cancellationToken: ct);
                    return;

                case "crear":
                    faqText = @"⏰ *FAQ: CREAR RECORDATORIOS*

*🎯 ¿Qué hace este botón?*
Te permite crear recordatorios de forma flexible usando lenguaje natural.

*📝 ¿Cómo usarlo?*
Escribe: `/remember <texto> en <tiempo>`

*✨ Ejemplos prácticos:*
• `/remember Tomar agua en 10 min`
• `/remember Reunión con cliente mañana a las 14:30`
• `/remember Pagar renta en 5 días`
• `/remember Llamar doctor hoy a las 18:00`

*🕐 Formatos de tiempo aceptados:*
✅ `en X segundos/min/horas/días`
✅ `hoy a las HH:MM`
✅ `mañana a las HH:MM`

*💡 Consejo:*
Si necesitas tiempos comunes (5min, 1h, etc.), usa el botón *🕐 Atajos Rápidos* del menú principal.

*📋 Después de crear:*
Verás 4 botones de acción:
• 🔄 Hacer Recurrente
• 📋 Ver Todos
• ➕ Crear Otro
• 🏠 Menú Principal";
                    break;

                case "listar":
                    faqText = @"📋 *FAQ: VER LISTA DE RECORDATORIOS*

*🎯 ¿Qué hace este botón?*
Muestra todos tus recordatorios organizados en dos categorías:

*📌 Recordatorios Pendientes:*
• Se muestran con ⏰ y tiempo restante
• Ejemplo: `⏰ Tomar agua (ID: abc123) - en 5 min`
• Puedes ver el ID para editarlo o eliminarlo

*✅ Recordatorios Completados:*
• Últimos 5 enviados
• Se muestran tachados: ~~Texto~~
• Con marcador 🔔 para indicar que fueron notificados

*💡 Comandos útiles:*
• `/list` - Ver todos los recordatorios
• Desde la lista puedes hacer clic en 🗑️ para eliminar

*🎯 Interpretación del tiempo:*
• `en 2 min` - Faltan 2 minutos
• `en 1 hora` - Falta 1 hora
• `vence pronto` - Menos de 1 minuto
• `vencido` - Ya pasó la hora (lo notificaremos pronto)";
                    break;

                case "editar":
                    faqText = @"✏️ *FAQ: EDITAR RECORDATORIOS*

*🎯 ¿Qué hace este comando?*
Permite modificar el texto de un recordatorio existente sin cambiar la hora programada.

*📝 ¿Cómo usarlo?*
Escribe: `/edit <id> <nuevo texto>`

*✨ Ejemplo paso a paso:*

1️⃣ Primero, ve tu lista:
   `/list`

2️⃣ Identifica el ID (ej: `abc123`)

3️⃣ Edita el texto:
   `/edit abc123 Nuevo texto del recordatorio`

*⚠️ Limitaciones:*
• Solo cambia el TEXTO
• NO cambia la fecha/hora
• Si quieres cambiar la hora, debes:
  1. Eliminar el recordatorio (`/delete abc123`)
  2. Crear uno nuevo con `/remember`

*💡 Caso de uso:*
Escribiste mal algo y quieres corregirlo sin perder la programación horaria.";
                    break;

                case "eliminar":
                    faqText = @"🗑️ *FAQ: ELIMINAR RECORDATORIOS*

*🎯 ¿Qué hace este comando?*
Borra permanentemente un recordatorio de la base de datos.

*📝 ¿Cómo usarlo?*
Método 1 (Comando directo):
`/delete <id>`

Método 2 (Desde lista - RECOMENDADO):
1. Escribe `/list`
2. Haz clic en el botón 🗑️ junto al recordatorio
3. Confirma con ✅ o cancela con ❌

*✨ Ejemplo:*
```
/delete abc123
```

*🔒 Seguridad:*
• Desde `/list` te pediremos confirmación
• Desde comando directo se borra inmediatamente
• NO se puede recuperar después de borrar

*💡 Recomendación:*
Usa el método de botones desde `/list` para evitar borrar por error. Tendrás una confirmación visual antes de eliminar.";
                    break;

                case "recurrente":
                    faqText = @"🔄 *FAQ: RECORDATORIOS RECURRENTES*

*🎯 ¿Qué hace esta función?*
Convierte un recordatorio en una tarea que se repite automáticamente.

*📝 ¿Cómo usarlo?*
Método 1 (Comando):
`/recur <id> <tipo>`

Método 2 (Desde lista):
1. Escribe `/list`
2. Haz clic en 🔄 junto al recordatorio
3. Selecciona el tipo de recurrencia

*🔁 Tipos de recurrencia:*
• `daily` (diario) - Se repite cada día
• `weekly` (semanal) - Se repite cada semana
• `monthly` (mensual) - Se repite cada mes
• `yearly` (anual) - Se repite cada año
• `none` (ninguno) - Desactiva la recurrencia

*✨ Ejemplo:*
```
# Recordatorio diario para tomar agua
/remember Tomar agua en 10 min
/recur abc123 daily

# Pago de renta mensual
/remember Pagar renta mañana a las 09:00
/recur xyz789 monthly
```

*⚙️ ¿Cómo funciona?*
Después de que el recordatorio se envía, automáticamente se programa de nuevo para la siguiente ocurrencia según el tipo.

*💡 Casos de uso:*
• Medicamentos diarios
• Reportes semanales
• Pagos mensuales
• Cumpleaños anuales";
                    break;

                case "atajos":
                    faqText = @"🕐 *FAQ: ATAJOS RÁPIDOS*

*🎯 ¿Qué hace este botón?*
Ofrece tiempos pre-configurados para crear recordatorios más rápido sin escribir fechas/horas.

*⚡ Atajos disponibles:*
• 🔥 *5 minutos* - Tareas muy urgentes
• ⏱️ *15 minutos* - Tareas a corto plazo
• 🕐 *1 hora* - Planificación cercana
• 🕑 *3 horas* - Tareas del día
• 📅 *Mañana 9 AM* - Planificación siguiente día
• 🌙 *Hoy 20:00* - Recordatorios nocturnos

*📝 ¿Cómo usar?*
1. Haz clic en *🕐 Atajos Rápidos* del menú
2. Selecciona un tiempo (ej: 15 minutos)
3. El bot te mostrará el formato exacto
4. Escribe tu recordatorio:
   `/remember Revisar correo en 15 min`

*💡 Ventajas:*
• No necesitas calcular la hora
• Formatos validados
• Ejemplos visuales
• Más rápido que escribir fechas

*🎯 Caso de uso:*
Perfecto cuando necesitas recordatorios rápidos sin pensar en formatos de tiempo complejos.";
                    break;

                case "general":
                    faqText = @"🎯 *FAQ: MODO DE USO GENERAL*

*🤖 ¿Qué es este bot?*
Un asistente personal de recordatorios que te ayuda a nunca olvidar tareas importantes.

*📱 ¿Cómo empezar?*
1. Escribe `/start` para ver el menú principal
2. Usa `/faq` para abrir este manual
3. Crea tu primer recordatorio con `/remember`

*🎮 Flujo básico de trabajo:*

*Paso 1: Crear*
`/remember Tomar agua en 10 min`
✅ Recordatorio creado con ID único

*Paso 2: Gestionar*
`/list` - Ver todos tus recordatorios
🗑️ Eliminar desde la lista
✏️ Editar texto
🔄 Hacer recurrente

*Paso 3: Recibir*
El bot te enviará un mensaje cuando llegue la hora:
`🔔 ¡RECORDATORIO! Tomar agua`

*🛠️ Comandos principales:*
• `/start` - Menú principal con botones
• `/remember <texto> en <tiempo>` - Crear
• `/list` - Ver todos
• `/delete <id>` - Eliminar
• `/edit <id> <texto>` - Modificar
• `/recur <id> <tipo>` - Hacer recurrente
• `/help` - Ayuda rápida con acciones
• `/faq` - Este manual completo

*💡 Consejos de uso:*
✅ Usa lenguaje natural: ""en 10 min"", ""mañana a las 14:00""
✅ Revisa tu lista regularmente con `/list`
✅ Usa atajos rápidos para tiempos comunes
✅ Haz recurrentes los recordatorios repetitivos

*🔐 Privacidad:*
• Tus recordatorios son privados
• Solo tú puedes verlos y modificarlos
• Se guardan de forma segura en el servidor

*❓ ¿Necesitas más ayuda?*
Escribe `/help` para ver la ayuda rápida con botones de acción.";
                    break;

                default:
                    faqText = "❓ Tema de FAQ no encontrado.";
                    break;
            }

            await bot.EditMessageText(
                chatId,
                messageId,
                faqText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
    }
}
