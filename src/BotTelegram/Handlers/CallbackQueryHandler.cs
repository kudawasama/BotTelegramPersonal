using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Services;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;
using BotTelegram.RPG.Commands;

namespace BotTelegram.Handlers
{
    public static class CallbackQueryHandler
    {
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
            var username = callbackQuery.From.Username ?? "unknown";

            // 🎯 LOG: Registrar TODA interacción de callback
            TelegramLogger.LogUserAction(
                chatId: chatId,
                username: username,
                action: $"callback:{data}",
                details: $"MessageId: {messageId}"
            );

            try
            {
                // Responder al callback genérico SOLO si NO es RPG/Pets/Guild/PvP/Train/Quest
                // Esos handlers tienen sus propios AnswerCallbackQuery internos
                if (!data.StartsWith("rpg_") && !data.StartsWith("pets_") &&
                    !data.StartsWith("guild_") && !data.StartsWith("pvp_") &&
                    !data.StartsWith("train_") && !data.StartsWith("quest_"))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                }

                // Procesar diferentes tipos de callbacks
                if (data == "start")
                {
                    await HandleStartCallback(bot, chatId, messageId, ct);
                }
                else if (data == "menu_ai")
                {
                    await HandleAIMenuCallback(bot, chatId, messageId, ct);
                }
                else if (data == "menu_info")
                {
                    await HandleInfoMenuCallback(bot, chatId, messageId, ct);
                }
                else if (data == "help")
                {
                    await HandleHelpCallback(bot, chatId, messageId, ct);
                }
                else if (data.StartsWith("faq_"))
                {
                    await HandleFaqCallback(bot, chatId, messageId, data, ct);
                }
                else if (data == "show_chat_help")
                {
                    await HandleShowChatHelpCallback(bot, chatId, messageId, ct);
                }
                else if (data == "clear_chat")
                {
                    await HandleClearChatCallback(bot, chatId, messageId, ct);
                }
                else if (data == "exit_chat")
                {
                    await HandleExitChatCallback(bot, chatId, messageId, ct);
                }
                // Pet System Callbacks (FASE 2)
                else if (data.StartsWith("pets_"))
                {
                    await HandlePetsCallback(bot, callbackQuery, data, ct);
                }
                // Dungeon System Callbacks (FASE 3)
                else if (data.StartsWith("dungeon_"))
                {
                    await HandleDungeonCallback(bot, callbackQuery, data, ct);
                }
                // Class System Callbacks (FASE 4)
                else if (data.StartsWith("class_") || data == "classes_menu" || data == "class_progress")
                {
                    await HandleClassCallback(bot, callbackQuery, data, ct);
                }
                // Leaderboard Callbacks
                else if (data.StartsWith("leaderboard_"))
                {
                    await HandleLeaderboardCallback(bot, callbackQuery, data, ct);
                }
                // Guild Callbacks (FASE 10)
                else if (data.StartsWith("guild_"))
                {
                    await BotTelegram.RPG.Commands.GuildCommand.HandleCallback(bot, callbackQuery, data, ct);
                }
                // PvP Callbacks (FASE 11)
                else if (data.StartsWith("pvp_"))
                {
                    await BotTelegram.RPG.Commands.PvpCommand.HandleCallback(bot, callbackQuery, data, ct);
                }
                // RPG Callbacks (incluye inv_ para inventario/equipamiento)
                else if (data == "rpg_main" || data.StartsWith("rpg_") ||
                         data.StartsWith("shop_") || data == "shop_buy" || data == "shop_sell" ||
                         data == "shop_buy_equip_menu" ||
                         data.StartsWith("craft_") ||
                         data.StartsWith("quest_") ||
                         data.StartsWith("inv_") ||
                         data.StartsWith("train_"))
                {
                    await HandleRpgCallback(bot, callbackQuery, data, ct);
                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiEx) when (apiEx.Message.Contains("message is not modified"))
            {
                // Error esperado: mensaje no modificado (silenciar)
                Console.WriteLine($"⚠️ [CallbackQueryHandler] Mensaje no modificado (mismo contenido)");
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [CallbackQueryHandler] Error: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                // Intentar responder sin mensaje visible al usuario
                try
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "⚠️ Error procesando acción", showAlert: false, cancellationToken: ct);
                }
                catch
                {
                    // Si falla el callback, enviar mensaje
                    await bot.SendMessage(chatId, "❌ Ocurrió un error inesperado. Intenta de nuevo.", cancellationToken: ct);
                }
            }
        }

        private static async Task HandleHelpCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            // Crear botones con acciones principales
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Juego RPG", "rpg_main"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💬 Chat IA", "rpg_ai_chat")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏆 Rankings", "leaderboard_main"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Mascotas", "pets_main")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            var helpText = @"📚 *AYUDA - Bot RPG con IA*

*🎮 JUEGO RPG:*
`/rpg` - Inicia tu aventura
• Explora mazmorras
• Combate enemigos
• Sube de nivel
• Desbloquea habilidades
• Doma mascotas
• Mejora tu equipo

*💬 CHAT CON IA:*
`/chat <mensaje>` - Conversa con la IA
• Pregunta lo que quieras
• Obtén ayuda en el juego
• Descubre secretos

*🏆 SISTEMA SOCIAL:*
`/leaderboard` o `/rankings` - Rankings globales
• Top jugadores por nivel
• Rankings de oro, kills, jefes
• Perfil personal con estadísticas

*🐾 MASCOTAS:*
`/pets` - Gestiona tus mascotas
• Ve tus compañeros
• Entrena y mejora
• Lleva a combate

*🎯 Click en los botones abajo para acceder rápidamente*";

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
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 JUEGO RPG", "menu_ai")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("ℹ️ AYUDA E INFORMACIÓN", "menu_info")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "👋 *¡Bienvenido al Bot RPG con IA!*\n\n" +
                "✨ Tu aventura épica comienza aquí:\n" +
                "• Juego RPG inmersivo con combate por turnos\n" +
                "• Chat con IA avanzada\n" +
                "• Sistema de mascotas y habilidades\n" +
                "• Rankings globales y competencia\n\n" +
                "🎯 *Selecciona una categoría:*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleAIMenuCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💬 Chat con IA", "show_chat_help")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Juego RPG", "rpg_main")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏆 Rankings", "leaderboard_main"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Mascotas", "pets_main")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛡️ Gremio", "guild_menu"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Arena PvP", "pvp_menu")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "🤖 *COTANO RPG*\n\n" +
                "Potenciado por Groq (Llama 3.1 8B):\n\n" +
                "💬 *Chat con IA*\n" +
                "   Conversaciones naturales e inteligentes\n" +
                "   Rate limit: 10 consultas/minuto\n\n" +
                "🎮 *Juego RPG - Leyenda del Void*\n" +
                "   Aventura épica generada con IA\n" +
                "   14 clases, combate por turnos, narrativas dinámicas\n\n" +
                "🏆 *Sistema Social*\n" +
                "   Rankings globales y competencia\n" +
                "   Sistema de mascotas y habilidades\n\n" +
                "⚡ *Características:*\n" +
                "• Respuestas contextuales\n" +
                "• Memoria de conversación\n" +
                "• Generación de narrativas RPG",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleInfoMenuCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❓ Ayuda", "help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📚 FAQ", "faq_menu")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "ℹ️ *AYUDA E INFORMACIÓN*\n\n" +
                "Todo lo que necesitas saber:\n\n" +
                "❓ *Ayuda*\n" +
                "   Guía de comandos y uso básico\n\n" +
                "📚 *FAQ / Manual*\n" +
                "   Preguntas frecuentes\n" +
                "   Ejemplos detallados\n" +
                "   Solución de problemas\n\n" +
                "💡 *Comandos disponibles:*\n" +
                "`/start` - Menú principal\n" +
                "`/help` - Ayuda rápida\n" +
                "`/rpg` - Juego RPG\n" +
                "`/pets` - Gestionar mascotas\n" +
                "`/leaderboard` - Rankings\n" +
                "`/gremio` - Sistema de Gremios\n" +
                "`/arena` - Arena PvP\n" +
                "`/chat` - IA conversacional",
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

        private static async Task HandleShowChatHelpCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            BotTelegram.Services.AIService.SetChatMode(chatId, true);

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Reiniciar chat", "clear_chat"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🚪 Salir del chat", "exit_chat")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            var helpText = @"🤖 *CHAT CON INTELIGENCIA ARTIFICIAL*

✅ *Modo chat activado.*
Ahora puedes escribir normalmente y responderé como asistente.
Los comandos `/list`, `/remember`, etc. siguen funcionando.

📝 *¿Cómo usar?*
Escribe libremente o usa: `/chat <tu mensaje>`

💬 *Ejemplos de conversación:*
• `/chat Hola, ¿cómo estás?`
• `/chat ¿Qué tengo pendiente hoy?`
• `/chat Explícame cómo crear recordatorios`
• `/chat Tengo reunión mañana a las 10`
• `/chat ¿Cómo se hace café espresso?`
• `/chat Dame consejos de productividad`

🧠 *Capacidades:*
✅ Recuerdo el contexto de nuestra conversación
✅ Te ayudo a organizar tu día
✅ Respondo consultas generales
✅ Sugiero recordatorios cuando es apropiado
✅ Explico cómo usar el bot

🔄 *Reiniciar conversación:*
Si quieres que olvide el contexto anterior:
`/chat reiniciar` o botón *Reiniciar chat*

💡 *Consejos:*
• Puedo ayudarte a crear recordatorios de forma conversacional
• Pregúntame sobre tus tareas pendientes
• Pide sugerencias para organizarte mejor
• Úsame para consultas rápidas

⚡ *Potenciado por:* Groq AI (Llama 3.1)";

            await bot.EditMessageText(
                chatId,
                messageId,
                helpText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }

        private static async Task HandleClearChatCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            try
            {
                // Limpiar conversación
                var aiService = new BotTelegram.Services.AIService();
                aiService.ClearConversation(chatId);

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💡 Ver ejemplos", "show_chat_help"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🚪 Salir del chat", "exit_chat")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                    }
                });

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🔄 *Conversación reiniciada*\n\n" +
                    "He limpiado el historial de nuestra conversación.\n" +
                    "Ahora puedes empezar una nueva conversación desde cero.\n\n" +
                    "Escribe normalmente o usa `/chat <mensaje>` para comenzar.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);

                Console.WriteLine($"[CallbackQueryHandler] 🔄 Chat reiniciado para ChatId {chatId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CallbackQueryHandler] ❌ Error al reiniciar chat: {ex.Message}");
                await bot.SendMessage(chatId, "❌ Error al reiniciar la conversación.", cancellationToken: ct);
            }
        }

        private static async Task HandleExitChatCallback(
            ITelegramBotClient bot,
            long chatId,
            int messageId,
            CancellationToken ct)
        {
            BotTelegram.Services.AIService.ClearChatMode(chatId);

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🤖 Activar chat", "show_chat_help"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                }
            });

            await bot.EditMessageText(
                chatId,
                messageId,
                "🚪 *Modo chat desactivado*\n\n" +
                "Si quieres volver a conversar con la IA, pulsa *Activar chat* o escribe `/chat`.",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);

            Console.WriteLine($"[CallbackQueryHandler] 🚪 Chat desactivado para ChatId {chatId}");
        }
        
        // ============================================
        // RPG GAME CALLBACKS
        // ============================================
        
        private static async Task HandleRpgCallback(
            ITelegramBotClient bot,
            Telegram.Bot.Types.CallbackQuery callbackQuery,
            string data,
            CancellationToken ct)
        {
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;
            var rpgCommand = new BotTelegram.RPG.Commands.RpgCommand();
            var rpgService = new BotTelegram.RPG.Services.RpgService();
            var combatService = new BotTelegram.RPG.Services.RpgCombatService();
            var explorationService = new BotTelegram.RPG.Services.ExplorationService(rpgService);
            var mapCommand = new BotTelegram.RPG.Commands.MapCommand();
            var travelCommand = new BotTelegram.RPG.Commands.TravelCommand();
            
            Console.WriteLine($"[RPG] Callback: {data}");
            
            // Helper: Generate OFFICIAL combat menu (usa el menú de RpgCommand)
            var GetCombatKeyboard = () => rpgCommand.GetCombatKeyboard();

            var BuildEquipmentMenuText = (BotTelegram.RPG.Models.RpgPlayer player) =>
            {
                var text = "🎒 **EQUIPMENT**\n\n";
                
                var weapon = player.EquippedWeaponNew;
                var armor = player.EquippedArmorNew;
                var accessory = player.EquippedAccessoryNew;
                
                text += "⚔️ **Arma:** ";
                text += weapon != null ? $"{weapon.Name} {weapon.RarityEmoji}" : "*Sin arma*";
                text += "\n";
                
                text += "🛡️ **Armadura:** ";
                text += armor != null ? $"{armor.Name} {armor.RarityEmoji}" : "*Sin armadura*";
                text += "\n";
                
                text += "💍 **Accesorio:** ";
                text += accessory != null ? $"{accessory.Name} {accessory.RarityEmoji}" : "*Sin accesorio*";
                text += "\n\n";
                
                var totalItems = player.EquipmentInventory?.Count ?? 0;
                var weapons = player.EquipmentInventory?.Count(e => e.Type == BotTelegram.RPG.Models.EquipmentType.Weapon) ?? 0;
                var armors = player.EquipmentInventory?.Count(e => e.Type == BotTelegram.RPG.Models.EquipmentType.Armor) ?? 0;
                var accessories = player.EquipmentInventory?.Count(e => e.Type == BotTelegram.RPG.Models.EquipmentType.Accessory) ?? 0;
                
                text += $"📦 **Inventario:** {totalItems} items\n";
                text += $"   ⚔️ {weapons} | 🛡️ {armors} | 💍 {accessories}\n\n";
                text += "💡 Derrota enemigos para obtener loot o compra equipment en la tienda.\n";
                
                return text;
            };
            
            var BuildEquipmentMenuKeyboard = (BotTelegram.RPG.Models.RpgPlayer player) =>
            {
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Armas", "rpg_equipment_list_weapon"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛡️ Armaduras", "rpg_equipment_list_armor")
                });
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💍 Accesorios", "rpg_equipment_list_accessory"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏪 Tienda", "rpg_shop")
                });
                
                var unequipButtons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                if (player.EquippedWeaponNew != null)
                {
                    unequipButtons.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Desequipar arma", "rpg_unequip_weapon"));
                }
                if (player.EquippedArmorNew != null)
                {
                    unequipButtons.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Desequipar armadura", "rpg_unequip_armor"));
                }
                if (player.EquippedAccessoryNew != null)
                {
                    unequipButtons.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Desequipar accesorio", "rpg_unequip_accessory"));
                }
                
                if (unequipButtons.Count == 1)
                {
                    rows.Add(new[] { unequipButtons[0] });
                }
                else if (unequipButtons.Count == 2)
                {
                    rows.Add(new[] { unequipButtons[0], unequipButtons[1] });
                }
                else if (unequipButtons.Count == 3)
                {
                    rows.Add(new[] { unequipButtons[0], unequipButtons[1] });
                    rows.Add(new[] { unequipButtons[2] });
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                });
                
                return new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows);
            };

            var BuildShopMenu = (BotTelegram.RPG.Models.RpgPlayer player) =>
            {
                var items = BotTelegram.RPG.Services.EquipmentDatabase.GetShopItems(player.Level, 6);
                var text = "🏪 **TIENDA DE EQUIPMENT**\n\n";
                text += $"💰 Oro: **{player.Gold}**\n\n";
                
                if (items.Count == 0)
                {
                    text += "❌ No hay items disponibles ahora mismo.\n";
                }
                else
                {
                    foreach (var item in items)
                    {
                        text += $"{item.TypeEmoji} **{item.Name}** {item.RarityEmoji}\n";
                        text += $"   Lv.{item.RequiredLevel} | 💰 {item.Price} oro\n\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                foreach (var item in items)
                {
                    rows.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"Comprar {item.Name}", $"rpg_shop_buy_{item.Id}")
                    });
                }
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_equipment")
                });
                
                return (text, new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows));
            };
            
            // Main menu
            if (data == "rpg_main")
            {
                await bot.DeleteMessage(chatId, messageId, ct);
                var player = rpgService.GetPlayer(chatId);
                
                if (player == null)
                {
                    await rpgCommand.Execute(bot, callbackQuery.Message, ct);
                }
                else
                {
                    await rpgCommand.ShowMainMenu(bot, chatId, player, ct);
                }
                return;
            }
            
            // Fase 5 - Hierarchical Menu Navigation
            if (data == "rpg_menu_adventure")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null) return;
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ Aventura", cancellationToken: ct);
                
                var hpPct  = (double)player.HP / player.MaxHP * 100;
                var engPct = (double)player.Stamina / player.MaxStamina * 100;
                var hpEmoji  = hpPct  > 70 ? "💚" : hpPct  > 30 ? "💛" : "❤️";
                var engEmoji = engPct > 70 ? "⚡" : engPct > 30 ? "🔋" : "🪫";
                var dungeonStatus = player.CurrentDungeon != null && player.CurrentDungeon.IsActive
                    ? $"\n🏰 Mazmorra activa: **{player.CurrentDungeon.Name}** (Piso {player.CurrentDungeon.CurrentFloor}/{player.CurrentDungeon.TotalFloors})"
                    : "";
                
                var text = $"⚔️ **AVENTURA**\n\n" +
                           $"{hpEmoji} HP: {player.HP}/{player.MaxHP} | {engEmoji} Energía: {player.Stamina}/{player.MaxStamina}\n" +
                           $"📍 _{player.CurrentLocation}_{dungeonStatus}\n\n" +
                           $"**¿A dónde quieres ir?**\n\n" +
                           $"🗺️ **Explorar** — Combate, recursos, tesoros, bestias\n" +
                           $"🏰 **Mazmorras** — Desafía dungeons y obtén llaves\n" +
                           $"🎲 **Aventura Rápida** — Evento aleatorio inmediato\n" +
                           $"😴 **Descansar** — Recupera HP y energía\n" +
                           $"💼 **Trabajar** — Gana oro con trabajos\n" +
                           $"🗺️ **Mapa** — Viaja entre regiones";
                
                await bot.EditMessageText(chatId, messageId, text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: rpgCommand.GetAdventureMenu(),
                    cancellationToken: ct);
                return;
            }
            
            if (data == "rpg_menu_character")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null) return;
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "👤 Personaje", cancellationToken: ct);
                
                var classEmoji = player.Class switch
                {
                    BotTelegram.RPG.Models.CharacterClass.Warrior => "⚔️",
                    BotTelegram.RPG.Models.CharacterClass.Mage => "🔮",
                    BotTelegram.RPG.Models.CharacterClass.Rogue => "🗡️",
                    BotTelegram.RPG.Models.CharacterClass.Cleric => "✨",
                    _ => "👤"
                };
                
                var text = $@"👤 **PERSONAJE**

{classEmoji} **{player.Name}** - {player.Class} Nv.{player.Level}
💰 **Oro:** {player.Gold}
📍 **Ubicación:** {player.CurrentLocation}

📊 **Stats:** Consulta tus estadísticas detalladas
🎒 **Inventario:** Equipamiento y consumibles
✨ **Skills:** Habilidades especiales de combate
🐾 **Mascotas:** Gestiona tus compañeros
🎭 **Clases:** Cambiar o desbloquear clases ocultas
💎 **Pasivas:** Beneficios permanentes";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: rpgCommand.GetCharacterMenu(),
                    cancellationToken: ct);
                return;
            }
            
            if (data == "rpg_menu_city")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null) return;
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🏘️ Ciudad", cancellationToken: ct);
                
                var text = $@"🏘️ **CIUDAD**

Bienvenido a {player.CurrentLocation}

🏪 **Tienda:** Compra equipamiento y consumibles
⚒️ **Herrería:** Craftea objetos y equipos
🏛️ **Misiones:** Acepta quests y gana recompensas
🏆 **Rankings:** Tabla de líderes globales
🛡️ **Entrenar:** Mejora tus estadísticas
🌟 **Progreso:** Consulta tus logros
🛡️ **Gremio:** Únete o crea un gremio
⚔️ **Arena PvP:** Desafía a otros jugadores";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: rpgCommand.GetCityMenu(),
                    cancellationToken: ct);
                return;
            }
            
            if (data == "rpg_menu_help")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null) return;
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚙️ Ayuda", cancellationToken: ct);
                
                var text = @"⚙️ **AYUDA Y CONFIGURACIÓN**

📖 **Guía:** Tutorial completo del juego
💬 **Chat IA:** Pregunta sobre mecánicas o estrategia
🎯 **Tutorial:** Paso a paso para principiantes
⚙️ **Opciones:** Exportar/importar personaje
📊 **Comandos:** Lista de todos los comandos
🐛 **Reportar Bug:** Informa de errores o sugerencias

**Comandos básicos:**
/rpg - Abrir menú principal
/map - Ver mapa de zonas
/pets - Gestionar mascotas
/stats - Ver estadísticas";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: rpgCommand.GetHelpMenu(),
                    cancellationToken: ct);
                return;
            }
            
            // New game
            if (data == "rpg_new_game")
            {
                // Activar estado de espera de nombre
                BotTelegram.RPG.Services.RpgService.SetAwaitingName(chatId, true);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "✨ **CREACIÓN DE PERSONAJE**\n\n" +
                    "¿Cuál es tu nombre, héroe?\n\n" +
                    "Escribe tu nombre en el chat (3-20 caracteres)",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_cancel_creation")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Cancelar creación
            if (data == "rpg_cancel_creation")
            {
                BotTelegram.RPG.Services.RpgService.SetAwaitingName(chatId, false);
                await rpgCommand.Execute(bot, callbackQuery.Message!, ct);
                return;
            }
            
            // FASE 4: Handler rpg_class_ ELIMINADO
            // Los jugadores ya no eligen clase inicial, todos inician como Adventurer
            // Las clases se desbloquean jugando (ver ClassUnlockDatabase + ActionTrackerService)
            
            var currentPlayer = rpgService.GetPlayer(chatId);
            if (currentPlayer == null)
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Necesitas crear un personaje primero", cancellationToken: ct);
                return;
            }
            
            // FASE 6: Sincronizar estado FSM con la realidad del jugador
            StateManager.SyncState(currentPlayer);
            
            // Stats (detailed)
            if (data == "rpg_stats")
            {
                await bot.DeleteMessage(chatId, messageId, ct);
                var statsCommand = new BotTelegram.RPG.Commands.RpgStatsCommand();
                await statsCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
            
            // Skills menu
            if (data == "rpg_skills" || data.StartsWith("rpg_skills:"))
            {
                int page = 1;
                if (data.Contains(":"))
                {
                    int.TryParse(data.Split(':')[1], out page);
                }
                
                var allSkills = BotTelegram.RPG.Services.SkillDatabase.GetAllSkills();
                var unlockedSkills = allSkills.Where(s => currentPlayer.UnlockedSkills.Contains(s.Id)).ToList();
                var lockedSkills = allSkills.Where(s => !currentPlayer.UnlockedSkills.Contains(s.Id)).ToList();
                
                var text = $"✨ **SKILLS** ({unlockedSkills.Count}/{allSkills.Count} desbloqueadas)\n\n";
                
                if (unlockedSkills.Count == 0)
                {
                    text += "❌ Aún no has desbloqueado ninguna skill.\n\n";
                    text += "Completa acciones para desbloquear skills:\n";
                    text += "• Ataca enemigos\n";
                    text += "• Usa skills de combate\n";
                    text += "• Medita para recuperar mana\n";
                    text += "• Explora dungeons\n";
                    text += "• ¡Y mucho más!\n";
                    
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        text,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                    return;
                }
                
                // Agrupar skills desbloqueadas por categoría
                var combatSkills = unlockedSkills.Where(s => s.Category == BotTelegram.RPG.Models.SkillCategory.Combat).ToList();
                var magicSkills = unlockedSkills.Where(s => s.Category == BotTelegram.RPG.Models.SkillCategory.Magic).ToList();  
                var defenseSkills = unlockedSkills.Where(s => s.Category == BotTelegram.RPG.Models.SkillCategory.Defense).ToList();
                var movementSkills = unlockedSkills.Where(s => s.Category == BotTelegram.RPG.Models.SkillCategory.Movement).ToList();
                var specialSkills = unlockedSkills.Where(s => s.Category == BotTelegram.RPG.Models.SkillCategory.Special).ToList();
                
                // Sistema de paginación - 6 skills por página
                const int perPage = 6;
                var allGroups = new List<(string title, List<BotTelegram.RPG.Models.RpgSkill> items)>();
                
                if (combatSkills.Any()) allGroups.Add(("⚔️ **COMBATE:**", combatSkills));
                if (magicSkills.Any()) allGroups.Add(("🔮 **MAGIA:**", magicSkills));
                if (defenseSkills.Any()) allGroups.Add(("🛡️ **DEFENSA:**", defenseSkills));
                if (movementSkills.Any()) allGroups.Add(("💨 **MOVIMIENTO:**", movementSkills));
                if (specialSkills.Any()) allGroups.Add(("🌟 **ESPECIALES:**", specialSkills));
                
                var flatList = allGroups.SelectMany(g => g.items.Select(i => (g.title, item: i))).ToList();
                var totalPages = (int)Math.Ceiling(flatList.Count / (double)perPage);
                var pageItems = flatList.Skip((page - 1) * perPage).Take(perPage).ToList();
                
                string lastTitle = "";
                foreach (var (title, skill) in pageItems)
                {
                    if (title != lastTitle)
                    {
                        text += $"\n{title}\n";
                        lastTitle = title;
                    }
                    
                    var cooldown = currentPlayer.SkillCooldowns.ContainsKey(skill.Id) && currentPlayer.SkillCooldowns[skill.Id] > 0
                        ? $" ⏱️ {currentPlayer.SkillCooldowns[skill.Id]}T"
                        : "";
                    
                    text += $"  **{skill.Name}** (Lv.{skill.RequiredLevel}){cooldown}\n";
                    
                    // Costos
                    var costs = new List<string>();
                    if (skill.ManaCost > 0) costs.Add($"💧 {skill.ManaCost}");
                    if (skill.StaminaCost > 0) costs.Add($"⚡ {skill.StaminaCost}");
                    if (costs.Any())
                        text += $"     {string.Join(" | ", costs)}";
                    
                    if (skill.Cooldown > 0)
                        text += $" | CD: {skill.Cooldown}T";
                    
                    text += $"\n     {skill.Description}\n";
                }
                
                text += $"\n📊 **Página {page}/{totalPages}** | Total: {unlockedSkills.Count} skills";
                
                // Botones de navegación
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (totalPages > 1)
                {
                    var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    if (page > 1)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_skills:{page - 1}"));
                    if (page < totalPages)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_skills:{page + 1}"));
                    if (navRow.Any())
                        buttons.Add(navRow.ToArray());
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📈 Counters", "rpg_counters")
                });
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_skills"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Combo Skills menu (FASE 4)
            if (data == "rpg_combo_skills")
            {
                var text = "🌟 **HABILIDADES COMBINADAS**\n\n";
                text += "Estas skills se desbloquean automáticamente al completar combinaciones de acciones.\n\n";
                
                var allRequirements = BotTelegram.RPG.Services.SkillUnlockDatabase.GetAll();
                var unlocked = currentPlayer.UnlockedSkills;
                
                // Skills desbloqueadas
                var unlockedCombo = allRequirements.Where(r => unlocked.Contains(r.SkillId)).ToList();
                if (unlockedCombo.Any())
                {
                    text += "✅ **DESBLOQUEADAS:**\n";
                    foreach (var req in unlockedCombo.Take(10))
                    {
                        var skill = BotTelegram.RPG.Services.SkillDatabase.GetById(req.SkillId);
                        if (skill != null)
                        {
                            text += $"• {skill.Name}\n";
                        }
                    }
                    text += "\n";
                }
                
                // Skills cerca de desbloquear (>60% progreso)
                var nearUnlock = new List<(string skillId, double progress)>();
                foreach (var req in allRequirements)
                {
                    if (unlocked.Contains(req.SkillId)) continue;
                    
                    var progressDict = BotTelegram.RPG.Services.SkillUnlockDatabase.GetProgressTowards(currentPlayer, req.SkillId);
                    if (!progressDict.Any()) continue;
                    
                    var totalProgress = progressDict.Average(p => (double)p.Value.current / p.Value.required);
                    if (totalProgress >= 0.6)
                    {
                        nearUnlock.Add((req.SkillId, totalProgress));
                    }
                }
                
                if (nearUnlock.Any())
                {
                    text += "🔜 **CERCA DE DESBLOQUEAR:**\n";
                    foreach (var (skillId, progress) in nearUnlock.OrderByDescending(x => x.progress).Take(5))
                    {
                        var skill = BotTelegram.RPG.Services.SkillDatabase.GetById(skillId);
                        if (skill != null)
                        {
                            var percent = (int)(progress * 100);
                            var bar = GetProgressBar(progress, 10);
                            text += $"• {skill.Name} {bar} {percent}%\n";
                        }
                    }
                    text += "\n";
                }
                
                // Stats generales
                text += $"📊 **ESTADÍSTICAS:**\n";
                text += $"• Total desbloqueadas: {unlockedCombo.Count}/{allRequirements.Count}\n";
                text += $"• Progreso global: {(unlockedCombo.Count * 100 / allRequirements.Count)}%\n\n";
                text += "💡 Usa `/rpg_counters` para ver tu progreso en acciones.";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Todas", "rpg_combo_skills_all"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Requisitos", "rpg_combo_skills_req")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver al Menú", "rpg_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }
            
            // Ver todas las combo skills
            if (data == "rpg_combo_skills_all" || data.StartsWith("rpg_combo_skills_all:"))
            {
                int page = 1;
                if (data.Contains(":"))
                    int.TryParse(data.Split(':')[1], out page);
                
                var allRequirements = BotTelegram.RPG.Services.SkillUnlockDatabase.GetAll();
                var unlocked = currentPlayer.UnlockedSkills;
                
                // Paginación
                const int perPage = 6;
                var totalPages = (int)Math.Ceiling(allRequirements.Count / (double)perPage);
                page = Math.Max(1, Math.Min(page, totalPages));
                
                var pageRequirements = allRequirements
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .ToList();
                
                var text = "📜 **TODAS LAS HABILIDADES COMBINADAS**\n\n";
                
                foreach (var req in pageRequirements)
                {
                    var skill = BotTelegram.RPG.Services.SkillDatabase.GetById(req.SkillId);
                    if (skill == null) continue;
                    
                    var status = unlocked.Contains(req.SkillId) ? "✅" : "🔒";
                    text += $"{status} **{skill.Name}**\n";
                    text += $"   {skill.Description}\n\n";
                }
                
                text += $"━━━━━━━━━━━━━━━━━━━━━━\n";
                text += $"📄 Página **{page}/{totalPages}** | Total: {allRequirements.Count} skills\n";
                
                // Construir teclado con navegación
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (totalPages > 1)
                {
                    var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    if (page > 1)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_combo_skills_all:{page - 1}"));
                    if (page < totalPages)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_combo_skills_all:{page + 1}"));
                    if (navRow.Any())
                        buttons.Add(navRow.ToArray());
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combo_skills")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Ver requisitos de combo skills
            if (data == "rpg_combo_skills_req" || data.StartsWith("rpg_combo_skills_req:"))
            {
                int page = 1;
                if (data.Contains(":"))
                    int.TryParse(data.Split(':')[1], out page);
                
                var allRequirements = BotTelegram.RPG.Services.SkillUnlockDatabase.GetAll();
                var unlocked = currentPlayer.UnlockedSkills;
                
                var lockedSkills = allRequirements.Where(r => !unlocked.Contains(r.SkillId)).ToList();
                
                if (lockedSkills.Count == 0)
                {
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        "🎉 **¡Felicidades!**\n\n" +
                        "Has desbloqueado todas las habilidades combinadas disponibles.",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combo_skills") }
                        }),
                        cancellationToken: ct);
                    return;
                }
                
                // Paginación
                const int perPage = 4;
                var totalPages = (int)Math.Ceiling(lockedSkills.Count / (double)perPage);
                page = Math.Max(1, Math.Min(page, totalPages));
                
                var pageSkills = lockedSkills
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .ToList();
                
                var text = "📊 **REQUISITOS DE HABILIDADES**\n\n";
                text += "Progreso hacia skills bloqueadas:\n\n";
                
                foreach (var req in pageSkills)
                {
                    var skill = BotTelegram.RPG.Services.SkillDatabase.GetById(req.SkillId);
                    if (skill == null) continue;
                    
                    text += $"🔒 **{skill.Name}**\n";
                    
                    var progressDict = BotTelegram.RPG.Services.SkillUnlockDatabase.GetProgressTowards(currentPlayer, req.SkillId);
                    foreach (var (actionId, (current, required)) in progressDict)
                    {
                        var actionName = GetActionName(actionId);
                        var progress = (double)current / required;
                        var bar = GetProgressBar(progress, 8);
                        text += $"  • {actionName}: {bar} {current}/{required}\n";
                    }
                    text += "\n";
                }
                
                text += $"━━━━━━━━━━━━━━━━━━━━━━\n";
                text += $"📄 Página **{page}/{totalPages}** | {lockedSkills.Count} skills bloqueadas\n";
                
                // Construir teclado con navegación
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (totalPages > 1)
                {
                    var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    if (page > 1)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_combo_skills_req:{page - 1}"));
                    if (page < totalPages)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_combo_skills_req:{page + 1}"));
                    if (navRow.Any())
                        buttons.Add(navRow.ToArray());
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combo_skills")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Counters menu
            if (data == "rpg_counters" || data.StartsWith("rpg_counters:"))
            {
                int page = 1;
                if (data.Contains(":"))
                    int.TryParse(data.Split(':')[1], out page);
                    
                if (currentPlayer == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Primero crea un personaje", cancellationToken: ct);
                    return;
                }
                
                // Preparar todos los contadores con sus categorías
                var allCounters = new List<(string category, string action, int count)>();
                
                // Definir categorías y sus acciones
                var combatActions = new[] { "physical_attack", "magic_attack", "charge_attack", "precise_attack", "heavy_attack", "light_attack", "reckless_attack", "defensive_attack" };
                var defensiveActions = new[] { "block", "dodge", "counter", "perfect_dodge", "defend" };
                var movementActions = new[] { "jump", "retreat", "advance", "approach_enemy" };
                var specialActions = new[] { "meditate", "observe", "wait", "use_item", "berserk" };
                var eventCounters = new[] { "critical_hit", "damage_dealt", "damage_taken", "combat_survived", "low_hp_combat", "enemy_defeated", "combo_5plus", "combo_10plus", "battles_won", "battles_fled", "kill_enemy", "take_damage", "survive_low_hp", "spell_cast", "heal", "pet_death", "pet_action", "tame_creature", "summon_skeleton", "summon_zombie", "summon_ghost", "summon_lich", "summon_elemental", "skill_used" };
                
                // Recolectar TODOS los contadores (mostrar 0 si no existe)
                foreach (var action in combatActions)
                {
                    int count = currentPlayer.ActionCounters.ContainsKey(action) ? currentPlayer.ActionCounters[action] : 0;
                    allCounters.Add(("⚔️ Ataque", action, count));
                }
                
                foreach (var action in defensiveActions)
                {
                    int count = currentPlayer.ActionCounters.ContainsKey(action) ? currentPlayer.ActionCounters[action] : 0;
                    allCounters.Add(("🛡️ Defensa", action, count));
                }
                
                foreach (var action in movementActions)
                {
                    int count = currentPlayer.ActionCounters.ContainsKey(action) ? currentPlayer.ActionCounters[action] : 0;
                    allCounters.Add(("💨 Movimiento", action, count));
                }
                
                foreach (var action in specialActions)
                {
                    int count = currentPlayer.ActionCounters.ContainsKey(action) ? currentPlayer.ActionCounters[action] : 0;
                    allCounters.Add(("✨ Especial", action, count));
                }
                
                foreach (var counter in eventCounters)
                {
                    int count = currentPlayer.ActionCounters.ContainsKey(counter) ? currentPlayer.ActionCounters[counter] : 0;
                    allCounters.Add(("📈 Eventos", counter, count));
                }
                
                // Skills usadas (top skills only)
                var skillCounters = currentPlayer.ActionCounters
                    .Where(kvp => kvp.Key.StartsWith("skill_") && kvp.Key != "skill_used")
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(10);
                
                foreach (var skill in skillCounters)
                    allCounters.Add(("🎯 Skills", skill.Key, skill.Value));
                
                // Paginación
                const int perPage = 12;
                var totalPages = (int)Math.Ceiling(allCounters.Count / (double)perPage);
                page = Math.Max(1, Math.Min(page, totalPages));
                
                var pageCounters = allCounters
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .ToList();
                
                // Construir mensaje
                var text = $"📊 **CONTADORES DE ACCIÓN**\n\n";
                text += $"Estas estadísticas rastrean tus acciones en combate.\n\n";
                
                string lastCategory = "";
                foreach (var (category, action, count) in pageCounters)
                {
                    // Mostrar categoría si cambió
                    if (category != lastCategory)
                    {
                        if (lastCategory != "") text += "\n";
                        text += $"**{category}**\n";
                        lastCategory = category;
                    }
                    
                    // Formatear nombre de acción
                    string displayName = GetActionName(action);
                    if (action.StartsWith("skill_"))
                    {
                        var skillId = action.Replace("skill_", "");
                        var skillInfo = SkillDatabase.GetById(skillId);
                        displayName = skillInfo?.Name ?? skillId;
                    }
                    
                    text += $"  • {displayName}: **{count:N0}**\n";
                }
                
                // Total
                var totalActions = allCounters.Sum(c => c.count);
                text += $"\n━━━━━━━━━━━━━━━━━━━━━━\n";
                text += $"📊 Total: **{totalActions:N0}** acciones\n";
                text += $"📄 Página **{page}/{totalPages}**\n";
                
                // Construir teclado
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                // Navegación
                if (totalPages > 1)
                {
                    var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    if (page > 1)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_counters:{page - 1}"));
                    if (page < totalPages)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_counters:{page + 1}"));
                    if (navRow.Any())
                        buttons.Add(navRow.ToArray());
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✨ Ver Skills", "rpg_skills"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats")
                });
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_counters"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Equipment menu (placeholder for now)
            if (data == "rpg_equipment")
            {
                var equipmentText = BuildEquipmentMenuText(currentPlayer);
                var keyboard = BuildEquipmentMenuKeyboard(currentPlayer);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    equipmentText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            if (data.StartsWith("rpg_equipment_list_"))
            {
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🎒 Cargando inventario...", cancellationToken: ct);
                
                var typeKey = data.Replace("rpg_equipment_list_", "");
                var type = typeKey switch
                {
                    "weapon" => BotTelegram.RPG.Models.EquipmentType.Weapon,
                    "armor" => BotTelegram.RPG.Models.EquipmentType.Armor,
                    "accessory" => BotTelegram.RPG.Models.EquipmentType.Accessory,
                    _ => BotTelegram.RPG.Models.EquipmentType.Weapon
                };
                
                var items = rpgService.GetEquipmentInventory(currentPlayer, type);
                var listText = "🎒 **INVENTARIO DE EQUIPMENT**\n\n";
                listText += $"Tipo: **{type}**\n\n";
                
                if (!items.Any())
                {
                    listText += "❌ No tienes items de este tipo.\n\n";
                }
                else
                {
                    foreach (var equip in items.Take(8))
                    {
                        listText += $"{equip.TypeEmoji} **{equip.Name}** {equip.RarityEmoji}\n";
                        listText += $"   Lv.{equip.RequiredLevel} | {equip.Type}\n";
                        
                        var bonuses = new List<string>();
                        if (equip.BonusAttack > 0) bonuses.Add($"+{equip.BonusAttack} Atk");
                        if (equip.BonusMagicPower > 0) bonuses.Add($"+{equip.BonusMagicPower} MP");
                        if (equip.BonusDefense > 0) bonuses.Add($"+{equip.BonusDefense} Def");
                        if (equip.BonusMagicResistance > 0) bonuses.Add($"+{equip.BonusMagicResistance} MR");
                        if (equip.BonusHP > 0) bonuses.Add($"+{equip.BonusHP} HP");
                        if (equip.BonusMana > 0) bonuses.Add($"+{equip.BonusMana} Mana");
                        if (equip.BonusStamina > 0) bonuses.Add($"+{equip.BonusStamina} Stamina");
                        
                        if (bonuses.Any())
                            listText += $"   {string.Join(", ", bonuses)}\n";
                        
                        listText += "\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                foreach (var equip in items.Take(8))
                {
                    rows.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"✅ Equipar {equip.Name}", $"rpg_equip_{equip.Id}")
                    });
                }
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_equipment")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    listText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            if (data.StartsWith("rpg_equip_"))
            {
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚙️ Equipando...", cancellationToken: ct);
                
                var equipmentId = data.Replace("rpg_equip_", "");
                var result = rpgService.EquipItem(currentPlayer, equipmentId);
                rpgService.SavePlayer(currentPlayer);
                var equipmentText = BuildEquipmentMenuText(currentPlayer);
                var keyboard = BuildEquipmentMenuKeyboard(currentPlayer);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    equipmentText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }
            
            if (data.StartsWith("rpg_unequip_"))
            {
                var typeKey = data.Replace("rpg_unequip_", "");
                var type = typeKey switch
                {
                    "weapon" => BotTelegram.RPG.Models.EquipmentType.Weapon,
                    "armor" => BotTelegram.RPG.Models.EquipmentType.Armor,
                    "accessory" => BotTelegram.RPG.Models.EquipmentType.Accessory,
                    _ => BotTelegram.RPG.Models.EquipmentType.Weapon
                };
                
                var result = rpgService.UnequipItem(currentPlayer, type);
                rpgService.SavePlayer(currentPlayer);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, result.Message, showAlert: false, cancellationToken: ct);
                var equipmentText = BuildEquipmentMenuText(currentPlayer);
                var keyboard = BuildEquipmentMenuKeyboard(currentPlayer);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    equipmentText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // ═══════════════════════════════════════════════════════════════
            // TIENDA — ShopCommand (rpg_shop, shop_buy, shop_sell, shop_buy_item:, shop_sell_item:, shop_sell_equip:)
            // ═══════════════════════════════════════════════════════════════
            if (data == "rpg_shop")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowShopMain(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            if (data == "shop_buy")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowBuyMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            if (data == "shop_sell")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowSellMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            if (data.StartsWith("shop_buy_item:"))
            {
                var shopItemId = data["shop_buy_item:".Length..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                await BotTelegram.RPG.Commands.ShopCommand.BuyItem(bot, chatId, currentPlayer, shopItemId, invSvc, ct, messageId, callbackQuery.Id);
                return;
            }

            if (data == "shop_buy_equip_menu")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowBuyEquipMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            if (data.StartsWith("shop_buy_equip:"))
            {
                var equipId = data["shop_buy_equip:".Length..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                await BotTelegram.RPG.Commands.ShopCommand.BuyEquip(bot, chatId, currentPlayer, equipId, invSvc, ct, messageId, callbackQuery.Id);
                return;
            }

            if (data.StartsWith("shop_sell_item:"))
            {
                var itemId = data["shop_sell_item:".Length..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.SellItem(currentPlayer, itemId);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? "💰 Vendido" : "❌ Error", cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowSellMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            if (data.StartsWith("shop_sell_equip:"))
            {
                var equipId = data["shop_sell_equip:".Length..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.SellEquipment(currentPlayer, equipId);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? "💰 Vendido" : "❌ Error", cancellationToken: ct);
                await BotTelegram.RPG.Commands.ShopCommand.ShowSellMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }

            // ─── legacy rpg_shop_buy_ (equipo de EquipmentDatabase) ──────
            if (data.StartsWith("rpg_shop_buy_"))
            {
                var itemId = data.Replace("rpg_shop_buy_", "");
                var item = BotTelegram.RPG.Services.EquipmentDatabase.GetById(itemId);
                
                if (item == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Item no encontrado", cancellationToken: ct);
                    return;
                }
                if (currentPlayer.Level < item.RequiredLevel)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, $"❌ Necesitas nivel {item.RequiredLevel}", cancellationToken: ct);
                    return;
                }
                if (currentPlayer.Gold < item.Price)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Oro insuficiente", cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE (caso éxito)
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💰 Comprando...", cancellationToken: ct);
                
                currentPlayer.Gold -= item.Price;
                currentPlayer.EquipmentInventory.Add(item.Clone());
                rpgService.SavePlayer(currentPlayer);
                var (shopText, shopKeyboard) = BuildShopMenu(currentPlayer);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    shopText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: shopKeyboard,
                    cancellationToken: ct);
                return;
            }
            
            // Inventory (legacy)
            // ═══════════════════════════════════════════════════════════════
            // CRAFTEO — CraftingCommand (Fase 8)
            // craft_menu, craft_view:{id}, craft_do:{id}
            // ═══════════════════════════════════════════════════════════════
            if (data == "craft_menu")
            {
                // VALIDAR ESTADO: Solo permitido en Idle
                if (!StateManager.IsActionAllowed(currentPlayer, data))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes craftear durante el combate o viaje", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚒️ Herrería", cancellationToken: ct);
                await BotTelegram.RPG.Commands.CraftingCommand.ShowCraftMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }
            if (data.StartsWith("craft_view:"))
            {
                var recipeId = data["craft_view:".Length..];
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await BotTelegram.RPG.Commands.CraftingCommand.ShowRecipeDetail(bot, chatId, currentPlayer, recipeId, ct, messageId);
                return;
            }
            if (data.StartsWith("craft_do:"))
            {
                var recipeId = data["craft_do:".Length..];
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚒️ Crafteando...", cancellationToken: ct);
                await BotTelegram.RPG.Commands.CraftingCommand.DoCraft(bot, chatId, currentPlayer, recipeId, ct, messageId);
                return;
            }

            // ═══════════════════════════════════════════════════════════════
            // MISIONES — QuestCommand (Fase 9)
            // quest_menu, quest_view:{id}, quest_accept:{id}, quest_complete:{id}
            // ═══════════════════════════════════════════════════════════════
            if (data == "quest_menu")
            {
                // VALIDAR ESTADO: Solo permitido en Idle
                if (!StateManager.IsActionAllowed(currentPlayer, data))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes ver misiones durante el combate o viaje", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                try
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "🏛️ Misiones", cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CallbackQueryHandler] ⚠️ No se pudo responder callback: {ex.Message}");
                }
                await BotTelegram.RPG.Commands.QuestCommand.ShowQuestMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }
            if (data.StartsWith("quest_view:"))
            {
                var questId = data["quest_view:".Length..];
                try
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CallbackQueryHandler] ⚠️ No se pudo responder callback: {ex.Message}");
                }
                await BotTelegram.RPG.Commands.QuestCommand.ShowQuestDetail(bot, chatId, currentPlayer, questId, ct, messageId);
                return;
            }
            if (data.StartsWith("quest_accept:"))
            {
                var questId = data["quest_accept:".Length..];
                try
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Aceptando misión...", cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CallbackQueryHandler] ⚠️ No se pudo responder callback: {ex.Message}");
                }
                await BotTelegram.RPG.Commands.QuestCommand.AcceptQuest(bot, chatId, currentPlayer, questId, ct, messageId);
                return;
            }
            if (data.StartsWith("quest_complete:"))
            {
                var questId = data["quest_complete:".Length..];
                
                // Responder inmediatamente antes de procesar
                try
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "🏆 Entregando misión...", cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CallbackQueryHandler] ⚠️ No se pudo responder callback (probablemente expirado): {ex.Message}");
                }
                
                await BotTelegram.RPG.Commands.QuestCommand.CompleteQuest(bot, chatId, currentPlayer, questId, ct, messageId);
                return;
            }

            // ═══════════════════════════════════════════════════════════════
            // INVENTARIO UNIFICADO - Consumibles + Equipamiento + Equipar/Usar/Vender
            // ═══════════════════════════════════════════════════════════════
            if (data == "rpg_inventory" || data.StartsWith("rpg_inventory:") ||
                data == "rpg_equipment"  || data.StartsWith("rpg_equipment:"))
            {
                // Parsear tab y página: "rpg_inventory:tab:page"
                // tab: "consumables" | "equipment" | "equipped"
                var parts = data.Split(':');
                string tab  = (parts.Length >= 2) ? parts[1] : "consumables";
                int page = 1;
                if (parts.Length >= 3) int.TryParse(parts[2], out page);
                if (page < 1) page = 1;
                if (data == "rpg_equipment") { tab = "equipment"; page = 1; }
                
                var invService = new BotTelegram.RPG.Services.InventoryService(rpgService);
                
                // ─── Vista: EQUIPO EQUIPADO ───────────────────────────────
                if (tab == "equipped")
                {
                    var eqText = BotTelegram.RPG.Services.InventoryService.GetEquippedSummary(currentPlayer);
                    var equippedBtns = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Arma",    "rpg_unequip:weapon"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛡️ Armadura","rpg_unequip:armor"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💍 Acc.",    "rpg_unequip:accessory")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Consumibles",  "rpg_inventory:consumables:1"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Equipamiento", "rpg_inventory:equipment:1")
                        },
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_character") }
                    };
                    await bot.EditMessageText(chatId, messageId, eqText,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(equippedBtns),
                        cancellationToken: ct);
                    return;
                }
                
                // ─── Vista: CONSUMIBLES / MATERIALES ─────────────────────
                if (tab == "consumables")
                {
                    var allItems = currentPlayer.Inventory
                        .OrderBy(i => i.Type)
                        .ThenBy(i => i.Rarity)
                        .ToList();
                    
                    const int perPage = 5;
                    var totalPages = Math.Max(1, (int)Math.Ceiling(allItems.Count / (double)perPage));
                    page = Math.Min(page, totalPages);
                    var pageItems = allItems.Skip((page - 1) * perPage).Take(perPage).ToList();
                    
                    var invText = $"🎒 **INVENTARIO** — Consumibles & Materiales\n";
                    invText += $"📦 {allItems.Count}/40 slots | Pág {page}/{totalPages}\n\n";
                    
                    if (allItems.Count == 0)
                    {
                        invText += "❌ _Sin ítems consumibles o materiales._\n\n";
                        invText += "💡 Explora y combate para obtener pociones y materiales.\n";
                    }
                    else
                    {
                        int idx = (page - 1) * perPage + 1;
                        foreach (var item in pageItems)
                        {
                            var rarityColor = item.Rarity switch
                            {
                                BotTelegram.RPG.Models.ItemRarity.Common    => "⚪",
                                BotTelegram.RPG.Models.ItemRarity.Uncommon  => "🟢",
                                BotTelegram.RPG.Models.ItemRarity.Rare      => "🔵",
                                BotTelegram.RPG.Models.ItemRarity.Epic      => "🟣",
                                BotTelegram.RPG.Models.ItemRarity.Legendary => "🟡",
                                _ => "⚪"
                            };
                            invText += $"**{idx}.** {item.Emoji} {rarityColor} **{item.Name}**\n";
                            invText += $"   _{item.Description}_\n";
                            if (item.HPRestore > 0)   invText += $"   ❤️ +{item.HPRestore} HP  ";
                            if (item.ManaRestore > 0)  invText += $"   💙 +{item.ManaRestore} Maná  ";
                            invText += $"   💰 {item.Value / 2}oro venta\n\n";
                            idx++;
                        }
                    }
                    
                    var consumBtns = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                    
                    // Botones por ítem (usar / vender)
                    int btnIdx = (page - 1) * perPage;
                    foreach (var item in pageItems)
                    {
                        var row = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                        if (item.Type == BotTelegram.RPG.Models.ItemType.Consumable)
                            row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"✅ Usar {item.Emoji}", $"inv_use:{item.Id}"));
                        row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"💰 Vender ({item.Value/2})", $"inv_sell_item:{item.Id}"));
                        consumBtns.Add(row.ToArray());
                    }
                    
                    // Navegación
                    if (totalPages > 1)
                    {
                        var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                        if (page > 1) navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️", $"rpg_inventory:consumables:{page-1}"));
                        if (page < totalPages) navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️", $"rpg_inventory:consumables:{page+1}"));
                        if (navRow.Any()) consumBtns.Add(navRow.ToArray());
                    }
                    
                    consumBtns.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Equipamiento", "rpg_inventory:equipment:1"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗡️ Equipado",     "rpg_inventory:equipped:1")
                    });
                    consumBtns.Add(new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_character") });
                    
                    await bot.EditMessageText(chatId, messageId, invText,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(consumBtns),
                        cancellationToken: ct);
                    return;
                }
                
                // ─── Vista: EQUIPAMIENTO (armas/armaduras/accesorios) ─────
                if (tab == "equipment")
                {
                    var allEq = currentPlayer.EquipmentInventory
                        .OrderByDescending(e => e.Rarity)
                        .ThenBy(e => e.Type)
                        .ToList();
                    
                    const int perPage = 4;
                    var totalPages = Math.Max(1, (int)Math.Ceiling(allEq.Count / (double)perPage));
                    page = Math.Min(page, totalPages);
                    var pageEq = allEq.Skip((page - 1) * perPage).Take(perPage).ToList();
                    
                    var eqText = $"⚔️ **EQUIPAMIENTO** — Inventario de Equipo\n";
                    eqText += $"📦 {allEq.Count}/30 slots | Pág {page}/{totalPages}\n\n";
                    
                    if (allEq.Count == 0)
                    {
                        eqText += "❌ _Sin equipamiento en el inventario._\n\n";
                        eqText += "💡 El equipo se obtiene al vencer enemigos (8% chance).\n";
                    }
                    else
                    {
                        int idx = (page - 1) * perPage + 1;
                        foreach (var eq in pageEq)
                        {
                            eqText += $"**{idx}.** {eq.TypeEmoji} {eq.RarityEmoji} **{eq.Name}**\n";
                            eqText += $"   Nivel req: {eq.RequiredLevel} | Tipo: {eq.Type}\n";
                            var bonuses = new List<string>();
                            if (eq.BonusAttack > 0)      bonuses.Add($"⚔️+{eq.BonusAttack}");
                            if (eq.BonusDefense > 0)     bonuses.Add($"🛡️+{eq.BonusDefense}");
                            if (eq.BonusMagicPower > 0)  bonuses.Add($"🔮+{eq.BonusMagicPower}");
                            if (eq.BonusHP > 0)          bonuses.Add($"❤️+{eq.BonusHP}");
                            if (eq.BonusMana > 0)        bonuses.Add($"💙+{eq.BonusMana}");
                            if (bonuses.Any()) eqText += $"   {string.Join(" | ", bonuses)}\n";
                            eqText += $"   💰 Valor venta: {eq.Price / 2} oro\n\n";
                            idx++;
                        }
                    }
                    
                    var eqBtns = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                    
                    // Botones por ítem (equipar / vender)
                    foreach (var eq in pageEq)
                    {
                        var canEquip = eq.RequiredLevel <= currentPlayer.Level;
                        var row = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                        if (canEquip)
                            row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"✅ Equipar {eq.TypeEmoji}", $"inv_equip:{eq.Id}"));
                        row.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"💰 Vender ({eq.Price/2})", $"inv_sell_eq:{eq.Id}"));
                        eqBtns.Add(row.ToArray());
                    }
                    
                    if (totalPages > 1)
                    {
                        var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                        if (page > 1) navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️", $"rpg_inventory:equipment:{page-1}"));
                        if (page < totalPages) navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️", $"rpg_inventory:equipment:{page+1}"));
                        if (navRow.Any()) eqBtns.Add(navRow.ToArray());
                    }
                    
                    eqBtns.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧪 Consumibles", "rpg_inventory:consumables:1"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗡️ Equipado",    "rpg_inventory:equipped:1")
                    });
                    eqBtns.Add(new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_character") });
                    
                    await bot.EditMessageText(chatId, messageId, eqText,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(eqBtns),
                        cancellationToken: ct);
                    return;
                }
                
                // Fallback: redirigir a consumibles
                goto inventoryConsumables;
                inventoryConsumables:
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🎒 Abriendo inventario...", cancellationToken: ct);
                // Reenviar como consumables tab via EditMessage simple
                {
                    var allItemsFb = currentPlayer.Inventory.OrderBy(i => i.Type).ToList();
                    var fbText = $"🎒 **INVENTARIO** — Consumibles & Materiales\n📦 {allItemsFb.Count}/40 slots\n\n";
                    fbText += allItemsFb.Count == 0 ? "❌ _Sin ítems._\n\n💡 Explora y combate para obtener pociones." : "";
                    var fbBtns = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧪 Consumibles",  "rpg_inventory:consumables:1"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Equipamiento", "rpg_inventory:equipment:1"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗡️ Equipado",     "rpg_inventory:equipped:1")
                        },
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_character") }
                    });
                    await bot.EditMessageText(chatId, messageId, fbText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: fbBtns, cancellationToken: ct);
                }
                return;
            }
            
            // ─── USAR ÍTEM ────────────────────────────────────────────────
            if (data.StartsWith("inv_use:"))
            {
                var itemId = data[8..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.UseItem(currentPlayer, itemId);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? "✅ Ítem usado" : "❌ Error", cancellationToken: ct);
                await bot.EditMessageText(chatId, messageId,
                    $"{msg}\n\n❤️ HP: {currentPlayer.HP}/{currentPlayer.MaxHP} | 💙 Maná: {currentPlayer.Mana}/{currentPlayer.MaxMana}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Volver al Inventario", "rpg_inventory:consumables:1") }
                    }), cancellationToken: ct);
                return;
            }
            
            // ─── VENDER ÍTEM CONSUMIBLE ───────────────────────────────────
            if (data.StartsWith("inv_sell_item:"))
            {
                var itemId = data[14..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.SellItem(currentPlayer, itemId);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? $"💰 Vendido" : "❌ Error", cancellationToken: ct);
                await bot.EditMessageText(chatId, messageId,
                    $"{msg}\n\n💰 Oro total: **{currentPlayer.Gold}**",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Volver al Inventario", "rpg_inventory:consumables:1") }
                    }), cancellationToken: ct);
                return;
            }
            
            // ─── EQUIPAR ITEM ─────────────────────────────────────────────
            if (data.StartsWith("inv_equip:"))
            {
                var equipId = data[10..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg, displaced) = invSvc.EquipItem(currentPlayer, equipId);
                var answer = ok ? "✅ Equipado" : "❌ Error";
                await bot.AnswerCallbackQuery(callbackQuery.Id, answer, cancellationToken: ct);
                var dispMsg = displaced != null ? $"\n🔄 {displaced.Name} movido al inventario." : "";
                await bot.EditMessageText(chatId, messageId,
                    $"{msg}{dispMsg}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Ver Equipo",       "rpg_inventory:equipped:1") },
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Ver Equipamiento", "rpg_inventory:equipment:1") }
                    }), cancellationToken: ct);
                return;
            }
            
            // ─── VENDER EQUIPO ────────────────────────────────────────────
            if (data.StartsWith("inv_sell_eq:"))
            {
                var equipId = data[12..];
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.SellEquipment(currentPlayer, equipId);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? "💰 Vendido" : "❌ Error", cancellationToken: ct);
                await bot.EditMessageText(chatId, messageId,
                    $"{msg}\n\n💰 Oro total: **{currentPlayer.Gold}**",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Volver al Inventario", "rpg_inventory:equipment:1") }
                    }), cancellationToken: ct);
                return;
            }
            
            // ─── DESEQUIPAR ───────────────────────────────────────────────
            if (data.StartsWith("rpg_unequip:"))
            {
                var slotStr = data[12..];
                var slot = slotStr switch
                {
                    "weapon"    => BotTelegram.RPG.Models.EquipmentType.Weapon,
                    "armor"     => BotTelegram.RPG.Models.EquipmentType.Armor,
                    "accessory" => BotTelegram.RPG.Models.EquipmentType.Accessory,
                    _ => BotTelegram.RPG.Models.EquipmentType.Weapon
                };
                var invSvc = new BotTelegram.RPG.Services.InventoryService(rpgService);
                var (ok, msg) = invSvc.UnequipItem(currentPlayer, slot);
                await bot.AnswerCallbackQuery(callbackQuery.Id, ok ? "🔓 Desequipado" : "❌ " + msg, cancellationToken: ct);
                if (ok)
                    await bot.EditMessageText(chatId, messageId, msg,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗡️ Ver Equipo Activo", "rpg_inventory:equipped:1") }
                        }), cancellationToken: ct);
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MENÚ DE PROGRESO (Clases Ocultas)
            // ═══════════════════════════════════════════════════════════════
            // Progress menu (paginado para clases ocultas)
            if (data == "rpg_progress" || data.StartsWith("rpg_progress:"))
            {
                int page = 1;
                if (data.Contains(":"))
                {
                    int.TryParse(data.Split(':')[1], out page);
                }
                
                var tracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                var allClasses = BotTelegram.RPG.Services.HiddenClassDatabase.GetAll();
                
                var text = "🌟 **PROGRESO DE CLASES OCULTAS**\n\n";
                
                if (currentPlayer.UnlockedHiddenClasses.Count > 0)
                {
                    text += "✅ **Clases Desbloqueadas:**\n";
                    foreach (var classId in currentPlayer.UnlockedHiddenClasses)
                    {
                        var hClass = BotTelegram.RPG.Services.HiddenClassDatabase.GetById(classId);
                        if (hClass != null)
                        {
                            var isActive = currentPlayer.ActiveHiddenClass == classId;
                            text += $"{hClass.Emoji} **{hClass.Name}** {(isActive ? "⚡ ACTIVA" : "")}\n";
                        }
                    }
                    text += "\n";
                }
                
                text += "📈 **Progreso hacia Nuevas Clases:**\n\n";
                
                var availableClasses = allClasses.Where(c => !currentPlayer.UnlockedHiddenClasses.Contains(c.Id)).ToList();
                
                if (availableClasses.Count == 0)
                {
                    text += "🎉 ¡Has desbloqueado todas las clases ocultas!\n\n";
                }
                else
                {
                    // Paginación - 3 clases por página
                    const int perPage = 3;
                    var totalPages = (int)Math.Ceiling(availableClasses.Count / (double)perPage);
                    var pageClasses = availableClasses.Skip((page - 1) * perPage).Take(perPage).ToList();
                    
                    foreach (var hClass in pageClasses)
                    {
                        var progress = tracker.GetClassProgress(currentPlayer, hClass.Id);
                        var percentage = tracker.GetClassProgressPercentage(currentPlayer, hClass.Id);
                        
                        text += $"{hClass.Emoji} **{hClass.Name}** [{percentage:F0}%]\n";
                        
                        // Mostrar primeros 3 requisitos
                        var reqCount = 0;
                        foreach (var (actionId, requiredCount) in hClass.RequiredActions.Take(3))
                        {
                            var currentCount = progress.CurrentProgress.GetValueOrDefault(actionId, 0);
                            var met = currentCount >= requiredCount;
                            var actionName = GetActionName(actionId);
                            text += $"  {(met ? "✅" : "🔸")} {actionName}: {currentCount}/{requiredCount}\n";
                            reqCount++;
                        }
                        
                        if (hClass.RequiredActions.Count > 3)
                        {
                            text += $"  ... y {hClass.RequiredActions.Count - 3} más\n";
                        }
                        text += "\n";
                    }
                    
                    text += $"📊 **Página {page}/{totalPages}** | {availableClasses.Count} clases disponibles";
                }
                
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                // Botones de navegación si hay múltiples páginas
                if (availableClasses.Count > 3)
                {
                    var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                    var totalPages = (int)Math.Ceiling(availableClasses.Count / 3.0);
                    if (page > 1)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_progress:{page - 1}"));
                    if (page < totalPages)
                        navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_progress:{page + 1}"));
                    if (navRow.Any())
                        buttons.Add(navRow.ToArray());
                }
                
                // Botón para ver clases desbloqueadas
                if (currentPlayer.UnlockedHiddenClasses.Count > 0)
                {
                    buttons.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🌟 Mis Clases", "rpg_my_classes")
                    });
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Mis Clases (activar/desactivar)
            if (data == "rpg_my_classes")
            {
                var text = "🌟 **MIS CLASES OCULTAS**\n\n";
                
                if (currentPlayer.UnlockedHiddenClasses.Count == 0)
                {
                    text += "❌ Aún no has desbloqueado ninguna clase oculta.\n\n";
                    text += "Completa acciones específicas para desbloquearlas.";
                }
                else
                {
                    text += "Clases que puedes activar:\n\n";
                    
                    foreach (var classId in currentPlayer.UnlockedHiddenClasses)
                    {
                        var hClass = BotTelegram.RPG.Services.HiddenClassDatabase.GetById(classId);
                        if (hClass != null)
                        {
                            var isActive = currentPlayer.ActiveHiddenClass == classId;
                            text += $"{hClass.Emoji} **{hClass.Name}** {(isActive ? "⚡" : "")}\n";
                            text += $"   {hClass.Description}\n\n";
                            text += "   **Bonuses:**\n";
                            if (hClass.StrengthBonus != 0) text += $"   • STR: +{hClass.StrengthBonus}\n";
                            if (hClass.IntelligenceBonus != 0) text += $"   • INT: +{hClass.IntelligenceBonus}\n";
                            if (hClass.DexterityBonus != 0) text += $"   • DEX: +{hClass.DexterityBonus}\n";
                            if (hClass.ConstitutionBonus != 0) text += $"   • CON: +{hClass.ConstitutionBonus}\n";
                            if (hClass.WisdomBonus != 0) text += $"   • WIS: +{hClass.WisdomBonus}\n";
                            if (hClass.CharismaBonus != 0) text += $"   • CHA: +{hClass.CharismaBonus}\n";
                            text += "\n";
                        }
                    }
                }
                
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                // Botones para activar/desactivar cada clase
                foreach (var classId in currentPlayer.UnlockedHiddenClasses)
                {
                    var hClass = BotTelegram.RPG.Services.HiddenClassDatabase.GetById(classId);
                    if (hClass != null)
                    {
                        var isActive = currentPlayer.ActiveHiddenClass == classId;
                        var buttonText = isActive ? $"❌ Desactivar {hClass.Name}" : $"⚡ Activar {hClass.Name}";
                        buttons.Add(new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(buttonText, $"rpg_toggle_class_{classId}")
                        });
                    }
                }
                
                buttons.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Progreso", "rpg_progress")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Activar/Desactivar clase
            if (data.StartsWith("rpg_toggle_class_"))
            {
                var classId = data.Replace("rpg_toggle_class_", "");
                var tracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                if (currentPlayer.ActiveHiddenClass == classId)
                {
                    // Desactivar
                    tracker.DeactivateHiddenClass(currentPlayer);
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Clase desactivada", cancellationToken: ct);
                }
                else
                {
                    // Activar
                    var success = tracker.ActivateHiddenClass(currentPlayer, classId);
                    if (success)
                    {
                        var hClass = BotTelegram.RPG.Services.HiddenClassDatabase.GetById(classId);
                        await bot.AnswerCallbackQuery(callbackQuery.Id, $"⚡ {hClass?.Name} activada!", cancellationToken: ct);
                    }
                    else
                    {
                        await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Error al activar clase", cancellationToken: ct);
                    }
                }
                
                // Refresh menu
                await bot.DeleteMessage(chatId, messageId, ct);
                var tempMsg = await bot.SendMessage(chatId, "Actualizando...", cancellationToken: ct);
                await Task.Delay(100);
                await bot.DeleteMessage(chatId, tempMsg.MessageId, ct);
                
                // Re-mostrar menú using callback
                var newCallback = new CallbackQuery
                {
                    Id = callbackQuery.Id,
                    From = callbackQuery.From,
                    Message = callbackQuery.Message,
                    Data = "rpg_my_classes"
                };
                await Handle(bot, newCallback, ct);
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MENÚ DE PASIVAS
            // ═══════════════════════════════════════════════════════════════
            // Passives menu (paginado)
            if (data == "rpg_passives" || data.StartsWith("rpg_passives:"))
            {
                int page = 1;
                if (data.Contains(":"))
                {
                    int.TryParse(data.Split(':')[1], out page);
                }
                
                var text = "💎 **PASIVAS ACTIVAS**\n\n";
                
                if (currentPlayer.UnlockedPassives.Count == 0)
                {
                    text += "❌ Aún no has desbloqueado ninguna pasiva.\n\n";
                    text += "Completa acciones para desbloquear pasivas permanentes:\n";
                    text += "• 100 críticos → Critical Mastery\n";
                    text += "• 200 enemigos → Life Steal\n";
                    text += "• 50 meditaciones → Regeneration\n";
                    text += "• ¡Y muchas más!\n";
                }
                else
                {
                    var passives = BotTelegram.RPG.Services.PassiveDatabase.GetUnlockedByPlayer(currentPlayer);
                    
                    // Agrupar por tipo
                    var combatPassives = passives.Where(p => 
                        p.Type == BotTelegram.RPG.Models.PassiveType.CriticalChanceBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.CriticalDamageBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.PhysicalDamageBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.MagicalDamageBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.Bloodlust ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.LifeSteal ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.SpellVamp
                    ).ToList();
                    
                    var survivalPassives = passives.Where(p => 
                        p.Type == BotTelegram.RPG.Models.PassiveType.MaxHPBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.MaxManaBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.MaxStaminaBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.SecondWind ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.Regeneration ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.Meditation
                    ).ToList();
                    
                    var utilityPassives = passives.Where(p => 
                        p.Type == BotTelegram.RPG.Models.PassiveType.GoldFindBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.XPBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.LootDropBonus ||
                        p.Type == BotTelegram.RPG.Models.PassiveType.MerchantFriend
                    ).ToList();
                    
                    var specialPassives = passives.Except(combatPassives).Except(survivalPassives).Except(utilityPassives).ToList();
                    
                    // Sistema de paginación - mostrar 8 por página
                    const int perPage = 8;
                    var allGroups = new List<(string title, List<BotTelegram.RPG.Models.Passive> items)>
                    {
                        ("⚔️ **COMBATE:**", combatPassives),
                        ("🛡️ **SUPERVIVENCIA:**", survivalPassives),
                        ("💰 **UTILIDAD:**", utilityPassives),
                        ("🌟 **ESPECIALES:**", specialPassives)
                    };
                    
                    var flatList = allGroups.SelectMany(g => g.items.Select(i => (g.title, item: i))).ToList();
                    var totalPages = (int)Math.Ceiling(flatList.Count / (double)perPage);
                    var pageItems = flatList.Skip((page - 1) * perPage).Take(perPage).ToList();
                    
                    string lastTitle = "";
                    foreach (var (title, passive) in pageItems)
                    {
                        if (title != lastTitle)
                        {
                            text += $"\n{title}\n";
                            lastTitle = title;
                        }
                        text += $"  {passive.Emoji} {passive.Name}\n";
                        text += $"     {passive.Description}\n";
                    }
                    
                    text += $"\n📊 **Página {page}/{totalPages}** | Total: {passives.Count} pasivas";
                    
                    // Botones de navegación
                    var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                    if (totalPages > 1)
                    {
                        var navRow = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                        if (page > 1)
                            navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Anterior", $"rpg_passives:{page - 1}"));
                        if (page < totalPages)
                            navRow.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➡️ Siguiente", $"rpg_passives:{page + 1}"));
                        buttons.Add(navRow.ToArray());
                    }
                    buttons.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                    });
                    
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        text,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                        cancellationToken: ct);
                    return;
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MENÚ DE ACCIONES ESPECIALES
            // ═══════════════════════════════════════════════════════════════
            if (data == "rpg_actions")
            {
                var text = "🧘 **ACCIONES ESPECIALES**\n\n";
                text += "Realiza acciones para progresar hacia clases ocultas:\n\n";
                
                text += "🧘 **Meditar** (Costo: 0 Energía)\n";
                text += "   Recupera mana y progresa hacia varias clases.\n\n";
                
                text += "😴 **Descansar** (Costo: 0 Energía)\n";
                text += "   Recupera HP y Stamina completamente.\n\n";
                
                // Acciones de bestias (solo si tiene pasiva)
                if (currentPlayer.UnlockedPassives.Contains("beast_whisperer"))
                {
                    text += "🐾 **Interactuar con Bestias**\n";
                    text += "   Puedes acariciar, calmar y domar bestias.\n";
                    text += "   Disponible durante exploración.\n\n";
                }
                
                text += "⚡ **Entrenar**\n";
                text += "   Practica habilidades de combate.\n\n";
                
                text += "💼 **Trabajar**\n";
                text += "   Gana oro honradamente.\n\n";
                
                var buttons = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧘 Meditar", "rpg_action_meditate"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("😴 Descansar", "rpg_rest")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚡ Entrenar", "rpg_train"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💼 Trabajar", "rpg_work")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                    }
                };
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Acción: Meditar
            if (data == "rpg_action_meditate")
            {
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🧘 Meditando...", cancellationToken: ct);
                
                var tracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var manaBeforeVar = currentPlayer.Mana;
                
                // Recuperar mana (50% del máximo)
                var manaRecovered = (int)(currentPlayer.MaxMana * 0.5);
                currentPlayer.Mana = Math.Min(currentPlayer.Mana + manaRecovered, currentPlayer.MaxMana);
                
                // Trackear acción
                tracker.TrackAction(currentPlayer, "meditation");
                
                rpgService.SavePlayer(currentPlayer);
                
                // Calcular progreso del mana
                var manaProgress = (double)currentPlayer.Mana / currentPlayer.MaxMana;
                var manaBar = GetProgressBar(manaProgress, 15);
                
                var text = "🧘 **MEDITACIÓN**\n\n";
                text += "_Te sientas en posición de loto, cierras los ojos y sientes la energía mágica fluir a través de tu cuerpo..._\n\n";
                
                text += "💠 **MANA RECUPERADO**\n";
                text += $"{manaBar}\n";
                text += $"🔸 Antes: **{manaBeforeVar}** / Ahora: **{currentPlayer.Mana}** / Máx: **{currentPlayer.MaxMana}**\n";
                text += $"✨ +**{manaRecovered}** mana recuperado\n\n";
                
                // Mostrar progreso hacia clases
                var meditationCount = tracker.GetActionCount(currentPlayer, "meditation");
                text += $"📊 **Sesiones totales:** {meditationCount}\n\n";
                
                // Mostrar qué clases requieren meditación
                var classesNeedingMeditation = BotTelegram.RPG.Services.HiddenClassDatabase.GetAll()
                    .Where(c => !currentPlayer.UnlockedHiddenClasses.Contains(c.Id) && 
                                c.RequiredActions.ContainsKey("meditation"))
                    .Take(3);
                
                if (classesNeedingMeditation.Any())
                {
                    text += "🌟 **PROGRESO HACIA CLASES OCULTAS:**\n";
                    foreach (var hClass in classesNeedingMeditation)
                    {
                        var required = hClass.RequiredActions["meditation"];
                        var classProgress = Math.Min((double)meditationCount / required, 1.0);
                        var classBar = GetProgressBar(classProgress, 10);
                        text += $"{hClass.Emoji} {hClass.Name}\n";
                        text += $"   {classBar} {meditationCount}/{required}\n";
                    }
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧘 Meditar de nuevo", "rpg_action_meditate"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Ver Progreso", "rpg_progress")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_actions")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Dungeon redirect desde menú RPG
            if (data == "rpg_dungeon")
            {
                var dungeonCommand = new DungeonCommand();
                await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                return;
            }
            
            // Classes redirect desde menú RPG (legacy rpg_hidden_classes también)
            if (data == "rpg_classes" || data == "rpg_hidden_classes")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await ClassesCommand.ShowClassMenu(bot, chatId, currentPlayer, ct, messageId);
                return;
            }
            
            // Explore
            // Explore Menu
            if (data == "rpg_explore_menu")
            {
                var energyBar = currentPlayer.Stamina >= 80 ? "⚡⚡⚡⚡⚡" :
                                currentPlayer.Stamina >= 60 ? "⚡⚡⚡⚡░" :
                                currentPlayer.Stamina >= 40 ? "⚡⚡⚡░░" :
                                currentPlayer.Stamina >= 20 ? "⚡⚡░░░" :
                                currentPlayer.Stamina >= 10 ? "⚡░░░░" : "░░░░░";
                
                await bot.EditMessageText(
                    chatId, messageId,
                    $"🗺️ **EXPLORACIÓN**\n\n" +
                    $"⚡ Energía: **{currentPlayer.Stamina}/{currentPlayer.MaxStamina}** {energyBar}\n" +
                    $"📍 Zona: _{currentPlayer.CurrentLocation}_\n\n" +
                    $"**¿Qué deseas hacer?**\n\n" +
                    $"⚔️ **Buscar Combate** — Encuentra enemigos _(15 energía)_\n" +
                    $"🏞️ **Recolectar** — Obtén materiales e ítems _(10 energía)_\n" +
                    $"💎 **Buscar Tesoro** — Cofres ocultos _(25 energía)_\n" +
                    $"🐾 **Bestias** — Busca mascotas domables _(30 energía)_\n" +
                    $"👥 **NPCs** — Habla con personajes\n" +
                    $"🎲 **Evento** — Sorpresa aleatoria _(15 energía)_",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Combate",    "rpg_explore"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏞️ Recursos",   "rpg_gather")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💎 Tesoro",     "rpg_treasure"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Bestias",    "rpg_search_beast")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("👥 NPCs",       "rpg_show_npcs"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎲 Evento",     "rpg_random_event")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("😴 Descansar",  "rpg_rest"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_adventure")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Explore - FORZAR combate (el jugador eligió ⚔️ Combate explícitamente)
            if (data == "rpg_explore")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 15))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 15)", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ Buscando enemigo...", cancellationToken: ct);
                
                rpgService.ConsumeEnergy(currentPlayer, 15);
                
                // Generar enemigo proporcional al nivel del jugador
                var rand = new Random();
                var diffRoll = rand.Next(100);
                var difficulty = diffRoll < 50 ? EnemyDifficulty.Easy
                               : diffRoll < 85 ? EnemyDifficulty.Medium
                               : EnemyDifficulty.Hard;
                
                var enemy = rpgService.GenerateEnemy(currentPlayer.Level, difficulty);
                
                currentPlayer.IsInCombat = true;
                currentPlayer.CurrentEnemy = enemy;
                StateManager.TransitionTo(currentPlayer, GameState.InCombat, enemy.Name);
                
                TelegramLogger.LogRpgEvent(chatId, currentPlayer.Name, "combat_search",
                    $"Enemy: {enemy.Name} (Lv.{enemy.Level}, {difficulty}). Energy left: {currentPlayer.Energy}");
                
                await bot.DeleteMessage(chatId, messageId, ct);
                var combatMessage = await bot.SendMessage(
                    chatId,
                    combatService.GenerateCombatMessage(null, currentPlayer, enemy),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCombatKeyboard(),
                    cancellationToken: ct);
                
                currentPlayer.ActiveCombatMessageId = combatMessage.MessageId;
                rpgService.SavePlayer(currentPlayer);
                return;
            }
            
            // Show NPCs in current zone
            if (data == "rpg_show_npcs")
            {
                var npcs = BotTelegram.RPG.Services.NPCDatabase.GetNPCsInZone(currentPlayer.CurrentZone);
                
                if (!npcs.Any())
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "No hay NPCs en esta zona.", cancellationToken: ct);
                    return;
                }
                
                var text = "👥 **NPCs EN LA ZONA**\n\n";
                text += $"📍 {currentPlayer.CurrentLocation}\n\n";
                
                var buttons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                
                foreach (var npc in npcs)
                {
                    text += $"{npc.Emoji} **{npc.Name}**\n";
                    text += $"_{npc.Description}_\n\n";
                    
                    buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            $"{npc.Emoji} {npc.Name}", 
                            $"rpg_talk_{npc.Id}")
                    });
                }
                
                buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_explore_menu")
                });
                
                await bot.EditMessageText(
                    chatId, messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Talk to NPC
            if (data.StartsWith("rpg_talk_"))
            {
                var npcId = data.Replace("rpg_talk_", "");
                var npcService = new BotTelegram.RPG.Services.NPCInteractionService();
                var result = npcService.StartConversation(currentPlayer, npcId);
                
                if (!result.Success)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, result.Message, cancellationToken: ct);
                    return;
                }
                
                rpgService.SavePlayer(currentPlayer);
                
                var buttons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                
                if (result.CurrentDialogue != null)
                {
                    for (int i = 0; i < result.CurrentDialogue.Options.Count; i++)
                    {
                        var option = result.CurrentDialogue.Options[i];
                        buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                                option.Text,
                                $"rpg_dialogue_{npcId}:{result.CurrentDialogue.Id}:{option.Id}")
                        });
                    }
                }
                else
                {
                    buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_show_npcs")
                    });
                }
                
                await bot.EditMessageText(
                    chatId, messageId,
                    result.Message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Process dialogue option
            if (data.StartsWith("rpg_dialogue_"))
            {
                var parts = data.Replace("rpg_dialogue_", "").Split(':');
                if (parts.Length != 3) return;
                
                var npcId = parts[0];
                var dialogueId = parts[1];
                var optionId = parts[2];
                
                var npcService = new BotTelegram.RPG.Services.NPCInteractionService();
                var result = npcService.ProcessDialogueOption(currentPlayer, npcId, dialogueId, optionId);
                
                rpgService.SavePlayer(currentPlayer);
                
                var buttons = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                
                if (result.CurrentDialogue != null)
                {
                    for (int i = 0; i < result.CurrentDialogue.Options.Count; i++)
                    {
                        var option = result.CurrentDialogue.Options[i];
                        buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                                option.Text,
                                $"rpg_dialogue_{npcId}:{result.CurrentDialogue.Id}:{option.Id}")
                        });
                    }
                }
                else
                {
                    buttons.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver NPCs", "rpg_show_npcs"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú", "rpg_menu")
                    });
                }
                
                await bot.EditMessageText(
                    chatId, messageId,
                    result.Message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
                    cancellationToken: ct);
                return;
            }
            
            // Map - Show world map
            if (data == "rpg_map")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🗺️ Cargando mapa...", cancellationToken: ct);
                
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.SendMessage(chatId, "❌ No tienes un personaje creado.", cancellationToken: ct);
                    return;
                }
                
                await bot.DeleteMessage(chatId, messageId, ct);
                
                // Reuse existing map display logic
                var mapMsg = new Telegram.Bot.Types.Message
                {
                    Chat = callbackQuery.Message.Chat,
                    Text = "/map"
                };
                
                await mapCommand.Execute(bot, mapMsg, ct);
                return;
            }
            
            // Travel - Move to another zone
            if (data.StartsWith("rpg_travel_"))
            {
                var targetZoneId = data.Replace("rpg_travel_", "");
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🚶 Viajando...", cancellationToken: ct);
                
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.SendMessage(chatId, "❌ No tienes un personaje creado.", cancellationToken: ct);
                    return;
                }
                
                await bot.DeleteMessage(chatId, messageId, ct);
                await travelCommand.ExecuteTravelById(bot, chatId, player, targetZoneId, ct);
                return;
            }
            
            // Zones List - Show all available zones
            if (data == "rpg_zones_list")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "📋 Cargando zonas...", cancellationToken: ct);
                
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.SendMessage(chatId, "❌ No tienes un personaje creado.", cancellationToken: ct);
                    return;
                }
                
                await bot.DeleteMessage(chatId, messageId, ct);
                
                var travelMsg = new Telegram.Bot.Types.Message
                {
                    Chat = callbackQuery.Message.Chat,
                    Text = "/travel"  // Without args shows zone list
                };
                
                await travelCommand.Execute(bot, travelMsg, ct);
                return;
            }
            
            // Adventure - Random event
            if (data == "rpg_adventure")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 20))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 20)", cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🗺️ Aventurando...", cancellationToken: ct);
                
                rpgService.ConsumeEnergy(currentPlayer, 20);
                
                var rand = new Random();
                var roll = rand.Next(100);
                
                string message = "🗺️ **AVENTURA**\n\n";
                
                if (roll < 40) // 40% - Enemigo fácil
                {
                    var enemy = rpgService.GenerateEnemy(currentPlayer.Level, EnemyDifficulty.Easy);
                    currentPlayer.IsInCombat = true;
                    currentPlayer.CurrentEnemy = enemy;
                    StateManager.TransitionTo(currentPlayer, GameState.InCombat, enemy.Name); // FSM
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.DeleteMessage(chatId, messageId, ct);
                    await bot.SendMessage(
                        chatId,
                        $"⚔️ **¡ENCUENTRO!**\n\n" +
                        $"Durante tu aventura, te topas con:\n\n" +
                        $"{enemy.Emoji} **{enemy.Name}** (Nv.{enemy.Level})\n" +
                        $"❤️ {enemy.HP}/{enemy.MaxHP} HP\n\n" +
                        $"¿Qué haces?",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                    return;
                }
                else if (roll < 65) // 25% - Enemigo medio
                {
                    var enemy = rpgService.GenerateEnemy(currentPlayer.Level, EnemyDifficulty.Medium);
                    currentPlayer.IsInCombat = true;
                    currentPlayer.CurrentEnemy = enemy;
                    StateManager.TransitionTo(currentPlayer, GameState.InCombat, enemy.Name); // FSM
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.DeleteMessage(chatId, messageId, ct);
                    await bot.SendMessage(
                        chatId,
                        $"⚔️ **¡PELIGRO!**\n\n" +
                        $"¡Un enemigo peligroso aparece!\n\n" +
                        $"{enemy.Emoji} **{enemy.Name}** (Nv.{enemy.Level})\n" +
                        $"❤️ {enemy.HP}/{enemy.MaxHP} HP\n\n" +
                        $"¡Prepárate para el combate!",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                    return;
                }
                else if (roll < 80) // 15% - Enemigo difícil
                {
                    var enemy = rpgService.GenerateEnemy(currentPlayer.Level, EnemyDifficulty.Hard);
                    currentPlayer.IsInCombat = true;
                    currentPlayer.CurrentEnemy = enemy;
                    StateManager.TransitionTo(currentPlayer, GameState.InCombat, enemy.Name); // FSM
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.DeleteMessage(chatId, messageId, ct);
                    await bot.SendMessage(
                        chatId,
                        $"💀 **¡ENEMIGO PODEROSO!**\n\n" +
                        $"¡Una criatura formidable bloquea tu camino!\n\n" +
                        $"{enemy.Emoji} **{enemy.Name}** (Nv.{enemy.Level})\n" +
                        $"❤️ {enemy.HP}/{enemy.MaxHP} HP\n\n" +
                        $"⚠️ ¡Este enemigo es muy peligroso!",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                    return;
                }
                else if (roll < 90) // 10% - Cofre de tesoro
                {
                    var goldFound = rand.Next(50, 201);
                    currentPlayer.Gold += goldFound;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"💎 **¡Cofre encontrado!**\n\n" +
                              $"Encuentras un cofre oculto entre los arbustos.\n" +
                              $"Dentro hay **{goldFound} oro**!\n\n" +
                              $"💰 Oro total: {currentPlayer.Gold}";
                }
                else if (roll < 95) // 5% - Comerciante viajero
                {
                    message += $"🎒 **¡Comerciante viajero!**\n\n" +
                              $"Un comerciante amigable te saluda.\n\n" +
                              $"_\"¡Saludos, aventurero! Tengo productos especiales...\"_\n\n" +
                              $"💡 Visita la tienda para ver sus ofertas.";
                }
                else // 5% - Evento especial
                {
                    var bonusXP = currentPlayer.Level * 10;
                    currentPlayer.XP += bonusXP;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"✨ **¡Evento especial!**\n\n" +
                              $"Ayudas a un anciano en problemas.\n" +
                              $"Como recompensa, te enseña un antiguo secreto.\n\n" +
                              $"📚 +{bonusXP} XP de experiencia!";
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Gather resources
            if (data == "rpg_gather")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 10))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 10)", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🏞️ Buscando recursos...", cancellationToken: ct);
                
                rpgService.ConsumeEnergy(currentPlayer, 10);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var rand = new Random();
                var roll = rand.Next(100);
                
                string message = "🏞️ **BUSCAR RECURSOS**\n\n";
                string itemMsg = "";
                
                if (roll < 30) // 30% - Poción menor
                {
                    var item = new BotTelegram.RPG.Models.RpgItem
                    {
                        Name = "Hierba Medicinal",
                        Emoji = "🌿",
                        Description = "Hierba con propiedades curativas",
                        Type = BotTelegram.RPG.Models.ItemType.Material,
                        Value = 10,
                        Rarity = BotTelegram.RPG.Models.ItemRarity.Common
                    };
                    if (currentPlayer.Inventory.Count < 40) { currentPlayer.Inventory.Add(item); itemMsg = $"\n\n**{item.Emoji} {item.Name}** añadida al inventario."; }
                    message += $"🌿 **¡Hierbas encontradas!**\n\nRecolectas hierbas medicinales útiles para pociones.{itemMsg}";
                    actionTracker.TrackAction(currentPlayer, "gather_herbs");
                }
                else if (roll < 50) // 20% - Fragmento de cristal
                {
                    var item = new BotTelegram.RPG.Models.RpgItem
                    {
                        Name = "Fragmento de Cristal",
                        Emoji = "🔷",
                        Description = "Material de crafteo básico",
                        Type = BotTelegram.RPG.Models.ItemType.Material,
                        Value = 30,
                        Rarity = BotTelegram.RPG.Models.ItemRarity.Common
                    };
                    if (currentPlayer.Inventory.Count < 40) { currentPlayer.Inventory.Add(item); itemMsg = $"\n\n**{item.Emoji} {item.Name}** añadido al inventario."; }
                    message += $"⛏️ **¡Mineral encontrado!**\n\nEncuentras un fragmento de cristal útil para herrero.{itemMsg}";
                    actionTracker.TrackAction(currentPlayer, "mine_ore");
                }
                else if (roll < 75) // 25% - Oro
                {
                    var goldFound = rand.Next(10, 31);
                    currentPlayer.Gold += goldFound;
                    message += $"🪵 **¡Materiales encontrados!**\n\nEncuentras materiales básicos y los vendes por **+{goldFound} oro**.\n💰 Oro total: {currentPlayer.Gold}";
                }
                else // 25% - Nada
                {
                    message += "❌ **No encontraste nada**\n\nBuscas durante un rato pero no encuentras recursos útiles.";
                }
                
                rpgService.SavePlayer(currentPlayer);
                
                await bot.EditMessageText(chatId, messageId, message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏞️ Buscar más", "rpg_gather"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎒 Inventario", "rpg_inventory:consumables:1")
                        },
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_explore_menu") }
                    }), cancellationToken: ct);
                return;
            }
            
            // Treasure hunt
            if (data == "rpg_treasure")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 25))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 25)", cancellationToken: ct);
                    return;
                }
                
                rpgService.ConsumeEnergy(currentPlayer, 25);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var rand = new Random();
                var roll = rand.Next(100);
                
                string message = "💎 **BÚSQUEDA DE TESORO**\n\n";
                
                if (roll < 50) // 50% - Nada
                {
                    message += $"❌ **Sin suerte**\n\n" +
                              $"Buscas cuidadosamente pero no encuentras ningún tesoro.\n\n" +
                              $"🗺️ Sigue explorando...";
                }
                else if (roll < 80) // 30% - Oro pequeño
                {
                    var goldFound = rand.Next(20, 51);
                    currentPlayer.Gold += goldFound;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"💰 **¡Bolsa de oro!**\n\n" +
                              $"Encuentras una pequeña bolsa con **{goldFound} oro**.\n\n" +
                              $"💰 Oro total: {currentPlayer.Gold}";
                    actionTracker.TrackAction(currentPlayer, "treasure_hunt");
                }
                else if (roll < 95) // 15% - Oro medio
                {
                    var goldFound = rand.Next(50, 101);
                    currentPlayer.Gold += goldFound;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"💰 **¡Cofre de oro!**\n\n" +
                              $"¡Encuentras un cofre lleno de monedas!\n" +
                              $"**+{goldFound} oro**\n\n" +
                              $"💰 Oro total: {currentPlayer.Gold}";
                    actionTracker.TrackAction(currentPlayer, "treasure_hunt");
                }
                else if (roll < 99) // 4% - Ítem raro
                {
                    message += $"✨ **¡Ítem raro!**\n\n" +
                              $"¡Encuentras un objeto poco común!\n\n" +
                              $"💡 Se ha agregado a tu inventario.";
                    actionTracker.TrackAction(currentPlayer, "treasure_hunt");
                }
                else // 1% - Ítem legendario
                {
                    message += $"🌟 **¡ÍTEM LEGENDARIO!**\n\n" +
                              $"¡¡¡INCREÍBLE!!! ¡Has encontrado un tesoro legendario!\n\n" +
                              $"✨ _Las leyendas son reales..._";
                    actionTracker.TrackAction(currentPlayer, "treasure_hunt");
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Search for beasts/pets
            if (data == "rpg_search_beast")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 30))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 30)", cancellationToken: ct);
                    return;
                }
                
                rpgService.ConsumeEnergy(currentPlayer, 30);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var rand = new Random();
                var roll = rand.Next(100);
                
                string message = "🐾 **BÚSQUEDA DE MASCOTAS**\n\n";
                
                if (roll < 60) // 60% - Nada
                {
                    message += $"❌ **No encontraste bestias**\n\n" +
                              $"Buscas durante un rato pero no encuentras ninguna criatura domable.\n\n" +
                              $"🐾 Intenta de nuevo más tarde.";
                    actionTracker.TrackAction(currentPlayer, "search_beast");
                }
                else if (roll < 85) // 25% - Bestia común
                {
                    var beasts = new[] { "🐺 Lobo", "🐱 Gato Salvaje", "🐍 Serpiente" };
                    var beast = beasts[rand.Next(beasts.Length)];
                    
                    message += $"🐾 **¡Bestia encontrada!**\n\n" +
                              $"Encuentras un **{beast}** salvaje.\n\n" +
                              $"💡 Usa '⚔️ Explorar' para encontrarlo en combate y domarlo.";
                    actionTracker.TrackAction(currentPlayer, "search_beast");
                }
                else if (roll < 95) // 10% - Bestia rara
                {
                    var beasts = new[] { "🐻 Oso", "🦅 Águila", "🐗 Jabalí" };
                    var beast = beasts[rand.Next(beasts.Length)];
                    
                    message += $"✨ **¡Bestia rara!**\n\n" +
                              $"¡Encuentras un **{beast}** poco común!\n\n" +
                              $"💡 Estas criaturas son más poderosas.";
                    actionTracker.TrackAction(currentPlayer, "search_beast");
                }
                else if (roll < 99) // 4% - Bestia épica
                {
                    message += $"🌟 **¡Bestia épica!**\n\n" +
                              $"¡¡Has avistado un **🐉 Dragón Joven**!!\n\n" +
                              $"💡 ¡Domarlo será un gran desafío!";
                    actionTracker.TrackAction(currentPlayer, "search_beast");
                }
                else // 1% - Bestia legendaria
                {
                    message += $"💫 **¡BESTIA LEGENDARIA!**\n\n" +
                              $"¡¡¡INCREÍBLE!!! Has encontrado una criatura de leyenda...\n\n" +
                              $"🌌 _La criatura desaparece en las sombras..._\n\n" +
                              $"¿Qué era eso?";
                    actionTracker.TrackAction(currentPlayer, "search_beast");
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Random event
            if (data == "rpg_random_event")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 15))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 15)", cancellationToken: ct);
                    return;
                }
                
                rpgService.ConsumeEnergy(currentPlayer, 15);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var rand = new Random();
                var roll = rand.Next(100);
                
                string message = "🎲 **EVENTO ALEATORIO**\n\n";
                
                if (roll < 30) // 30% - Comerciante
                {
                    message += $"🎒 **Comerciante viajero**\n\n" +
                              $"_\"¡Hola, aventurero! ¿Te interesa ver mis productos?\"_\n\n" +
                              $"Un comerciante te ofrece sus mercancías.";
                }
                else if (roll < 55) // 25% - NPC con quest
                {
                    var questReward = rand.Next(30, 71);
                    currentPlayer.Gold += questReward;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"🗣️ **Misión rápida**\n\n" +
                              $"Un aldeano te pide ayuda con una tarea simple.\n" +
                              $"Lo ayudas y te recompensa con **{questReward} oro**.\n\n" +
                              $"💰 Oro total: {currentPlayer.Gold}";
                    actionTracker.TrackAction(currentPlayer, "random_event");
                }
                else if (roll < 75) // 20% - Puzzle
                {
                    var bonusXP = currentPlayer.Level * 15;
                    currentPlayer.XP += bonusXP;
                    rpgService.SavePlayer(currentPlayer);
                    
                    message += $"🧩 **Puzzle antiguo**\n\n" +
                              $"Encuentras un acertijo tallado en piedra.\n" +
                              $"Lo resuelves correctamente y una luz te envuelve.\n\n" +
                              $"📚 +{bonusXP} XP de experiencia!";
                    actionTracker.TrackAction(currentPlayer, "random_event");
                }
                else if (roll < 90) // 15% - Bendición
                {
                    message += $"✨ **Bendición divina**\n\n" +
                              $"Una luz celestial te rodea.\n" +
                              $"Te sientes más fuerte temporalmente.\n\n" +
                              $"💪 +5% stats por 10 combates (próximamente)";
                    actionTracker.TrackAction(currentPlayer, "random_event");
                }
                else // 10% - Misterio
                {
                    message += $"🌀 **Encuentro misterioso**\n\n" +
                              $"Una figura encapuchada te observa desde lejos.\n" +
                              $"Antes de que puedas acercarte, desaparece...\n\n" +
                              $"_¿Quién era?_";
                    actionTracker.TrackAction(currentPlayer, "random_event");
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Combat - Attack
            if (data == "rpg_combat_attack")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                // Feedback inmediato
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ Atacando...", showAlert: false, cancellationToken: ct);
                
                // Guardar referencia al enemigo ANTES de que sea null
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PlayerAttack(currentPlayer, enemy);
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                rpgService.SavePlayer(currentPlayer);
                
                TelegramLogger.LogRpgEvent(
                    chatId,
                    currentPlayer.Name,
                    "combat_attack",
                    $"Attacked {enemy.Name}. Hit: {result.PlayerHit}. Damage: {result.PlayerDamage}. Enemy HP: {enemy.HP}");
                
                // Fase 5.2: Usar MessageId guardado o fallback al actual
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    // Limpiar MessageId de combate
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        narrative,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    // Limpiar MessageId de combate
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else
                {
                    // Fase 5.2: Actualizar vista unificada de combate
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Menu
            if (data == "rpg_combat_menu")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var enemy = currentPlayer.CurrentEnemy;
                var text = $"⚔️ **¡COMBATE!**\n\n" +
                           $"{enemy.Emoji} **{enemy.Name}** (Nv.{enemy.Level})\n" +
                           $"❤️ {enemy.HP}/{enemy.MaxHP} HP\n" +
                           $"⚔️ Ataque: {enemy.Attack} | 🔮 Magia: {enemy.MagicPower}\n" +
                           $"🛡️ Def.Física: {enemy.PhysicalDefense} | 🌀 Def.Mágica: {enemy.MagicResistance}\n\n" +
                           $"👤 **{currentPlayer.Name}**\n" +
                           $"❤️ {currentPlayer.HP}/{currentPlayer.MaxHP} HP | ⚡ {currentPlayer.Stamina}/{currentPlayer.MaxStamina} Stamina | 🔮 {currentPlayer.Mana}/{currentPlayer.MaxMana} Mana\n\n" +
                           "¿Qué haces?";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCombatKeyboard(),
                    cancellationToken: ct);
                return;
            }
            
            // Combat - Tactics menu
            if (data == "rpg_combat_tactics")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "📋 **TÁCTICAS DE COMBATE**\n\n" +
                    "Elige tu estrategia:\n\n" +
                    "**ATAQUES ESPECIALES:**\n" +
                    "💥 Carga - Ataque poderoso (35 stamina)\n" +
                    "⚡ Rápido - Ataque veloz (20 stamina)\n" +
                    "🎯 Preciso - Mayor precisión (25 stamina)\n" +
                    "🔨 Pesado - Máximo daño (40 stamina)\n\n" +
                    "**ACCIONES DEFENSIVAS:**\n" +
                    "🛡️ Bloquear - Reduce daño 50%\n" +
                    "💨 Esquivar - Evita próximo ataque\n" +
                    "🔄 Contragolpe - Devuelve daño\n\n" +
                    "**TÁCTICAS AVANZADAS:**\n" +
                    "👁️ Observar - Analiza al enemigo\n" +
                    "🧘 Meditar - Recupera mana (30)\n" +
                    "⏸️ Esperar - Pasa el turno",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💥 Carga", "rpg_combat_charge"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚡ Rápido", "rpg_combat_physical"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎯 Preciso", "rpg_combat_precise")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔨 Pesado", "rpg_combat_heavy"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Contra", "rpg_combat_counter")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧪 Meditar", "rpg_combat_meditate"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⏸️ Esperar", "rpg_combat_wait"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("👁️ Observar", "rpg_combat_observe")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combat_menu")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Combat - Pets menu
            if (data == "rpg_combat_pets")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                // Check if player has pets (future implementation)
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🐾 **MASCOTAS EN COMBATE**\n\n" +
                    "💡 Sistema de mascotas activas en combate.\n\n" +
                    "**Acciones disponibles:**\n" +
                    "🐾 Domar - Intenta domar bestia enemiga\n" +
                    "🐾 Acariciar - Aumenta bond con bestia\n" +
                    "🎶 Calmar - Calma bestia agresiva\n\n" +
                    "_Las mascotas activas atacarán automáticamente._",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Domar", "rpg_combat_tame"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Acariciar", "rpg_combat_pet")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎶 Calmar", "rpg_combat_calm")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combat_menu")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Pets menu (exploration)
            if (data == "rpg_pets_menu")
            {
                // Mostrar menú con información del jugador
                var totalPets = currentPlayer?.PetInventory?.Count ?? 0;
                var activePets = currentPlayer?.ActivePets?.Count ?? 0;
                var maxActive = currentPlayer?.MaxActivePets ?? 2;
                
                var text = "🐾 **MASCOTAS**\n\n";
                text += $"📊 **Domadas:** {totalPets}\n";
                text += $"⚔️ **Activas:** {activePets}/{maxActive}\n\n";
                text += "**Sistema de Mascotas:**\n";
                text += "• Doma bestias en combate\n";
                text += "• Aumenta vínculo (bond) acariciándolas\n";
                text += "• Evoluciónalas con niveles y bond\n";
                text += "• Úsalas en combate para ayudarte\n\n";
                
                if (totalPets == 0)
                {
                    text += "💡 **¿Cómo domar?**\n";
                    text += "1. Explora y encuentra bestias\n";
                    text += "2. Reduce su HP <50%\n";
                    text += "3. Usa el botón 🐾 Domar\n\n";
                }
                
                text += "🏞️ Busca bestias explorando el mundo.";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📋 Ver Mascotas", "pets_list_all"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Gestionar Activas", "pets_manage_active")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🍖 Alimentar", "pets_feed_menu"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⭐ Evolucionar", "pets_evolve_menu")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📖 Guía", "pets_guide"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Options menu
            if (data == "rpg_options")
            {
                await bot.DeleteMessage(chatId, messageId, ct);
                await rpgCommand.ShowOptionsMenu(bot, chatId, currentPlayer, ct);
                return;
            }
            
            // Combat - Skills menu
            if (data == "rpg_combat_skills")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var allSkills = BotTelegram.RPG.Services.SkillDatabase.GetAllSkills();
                var unlocked = allSkills.Where(s => currentPlayer.UnlockedSkills.Contains(s.Id)).ToList();
                var skillsText = "✨ **SKILLS DE COMBATE**\n\n";
                
                if (!unlocked.Any())
                {
                    skillsText += "❌ No tienes skills desbloqueadas.\n";
                }
                else
                {
                    foreach (var skill in unlocked.OrderBy(s => s.RequiredLevel))
                    {
                        var cd = currentPlayer.SkillCooldowns.ContainsKey(skill.Id) && currentPlayer.SkillCooldowns[skill.Id] > 0
                            ? $" (CD: {currentPlayer.SkillCooldowns[skill.Id]})"
                            : "";
                        skillsText += $"{skill.CategoryEmoji} **{skill.Name}**{cd}\n";
                        skillsText += $"   {skill.Description}\n";
                        
                        var costs = new List<string>();
                        if (skill.ManaCost > 0) costs.Add($"{skill.ManaCost} Mana");
                        if (skill.StaminaCost > 0) costs.Add($"{skill.StaminaCost} Stamina");
                        if (costs.Any())
                        {
                            skillsText += $"   💰 {string.Join(", ", costs)}\n";
                        }
                        
                        skillsText += "\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                foreach (var skill in unlocked.Take(8))
                {
                    rows.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"✨ {skill.Name}", $"rpg_combat_skill_{skill.Id}")
                    });
                }
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_combat_menu")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    skillsText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            if (data.StartsWith("rpg_combat_skill_"))
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "✨ Usando skill...", cancellationToken: ct);
                
                var skillId = data.Replace("rpg_combat_skill_", "");
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.UseSkill(currentPlayer, enemy, skillId);
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                rpgService.SavePlayer(currentPlayer);
                
                if (result.EnemyDefeated)
                {
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        narrative,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                    
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                }
                else
                {
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        narrative + "\n\n*¿Qué haces?*",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Defend
            if (data == "rpg_combat_defend")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🛡️ Defendiendo...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PlayerDefend(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else
                {
                    // Fase 5.2: Actualizar vista unificada
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                }
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // Combat - Use Item (Fase 12.5)
            // ═══════════════════════════════════════════════════════════════
            if (data == "combat_use_item")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                // Obtener pociones/consumibles disponibles
                var consumables = currentPlayer.Inventory
                    .Where(i => i.Type == ItemType.Consumable)
                    .GroupBy(i => i.Id)
                    .ToList();
                
                if (consumables.Count == 0)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes pociones", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                // Crear menú de pociones
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                foreach (var group in consumables.Take(6)) // Máximo 6 pociones por espacio
                {
                    var item = group.First();
                    var count = group.Count();
                    var displayText = count > 1 ? $"{item.Emoji} {item.Name} (x{count})" : $"{item.Emoji} {item.Name}";
                    rows.Add(new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(displayText, $"combat_use_item:{item.Id}")
                    });
                }
                rows.Add(new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver a Combate", "combat_back") });
                
                var text = "🧪 **Pociones Disponibles**\n━━━━━━━━━━━━━━━━\nElige una poción para usar:";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows.ToArray()),
                    cancellationToken: ct);
                return;
            }
            
            // Combat - Use Item (execute) (Fase 12.5)
            if (data.StartsWith("combat_use_item:"))
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var itemId = data["combat_use_item:".Length..];
                var item = currentPlayer.Inventory.FirstOrDefault(i => i.Id == itemId);
                
                if (item == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Item no encontrado", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, $"💊 Usando {item.Name}...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.UseItemInCombat(currentPlayer, enemy, itemId);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        narrative,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else
                {
                    // El combate continúa
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: GetCombatKeyboard(),
                        cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Back from items menu
            if (data == "combat_back")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var enemy = currentPlayer.CurrentEnemy;
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                await bot.EditMessageText(
                    chatId,
                    combatMessageId,
                    combatService.GenerateCombatMessage(null, currentPlayer, enemy),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCombatKeyboard(),
                    cancellationToken: ct);
                return;
            }
            
            // Combat - Flee
            if (data == "rpg_combat_flee")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🏃 Intentando huir...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var success = combatService.TryToFlee(currentPlayer, enemy);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (success)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        combatMessageId,
                        $"🏃 **¡Huiste exitosamente!**\n\n" +
                        $"Escapaste del combate.\n" +
                        $"❤️ HP restante: {currentPlayer.HP}/{currentPlayer.MaxHP}",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else
                {
                    var narrative = $"❌ **¡Fallo al huir!**\n\n" +
                                  $"El enemigo te alcanza y ataca.\n" +
                                  $"❤️ HP: {currentPlayer.HP}/{currentPlayer.MaxHP}";
                    
                    if (currentPlayer.HP <= 0)
                    {
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                                }
                            }),
                            cancellationToken: ct);
                            
                        currentPlayer.HP = currentPlayer.MaxHP / 2;
                        rpgService.SavePlayer(currentPlayer);
                    }
                    else
                    {
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            narrative + "\n\n*¡Debes continuar luchando!*",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Atacar", "rpg_combat_attack"),
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛡️ Defender", "rpg_combat_defend"),
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🧪 Item", "combat_use_item")
                                }
                            }),
                            cancellationToken: ct);
                    }
                }
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // FASE 2: HANDLERS DE ACCIÓN CON BESTIAS
            // ═══════════════════════════════════════════════════════════════
            
            // Combat - Tame Beast
            if (data == "rpg_combat_tame")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var enemy = currentPlayer.CurrentEnemy;
                var petTamingService = new BotTelegram.RPG.Services.PetTamingService(rpgService);
                var (success, message, pet) = petTamingService.AttemptTame(currentPlayer, enemy);
                
                if (success && pet != null)
                {
                    // Agregar mascota al inventario
                    if (currentPlayer.PetInventory == null)
                    {
                        currentPlayer.PetInventory = new List<BotTelegram.RPG.Models.RpgPet>();
                    }
                    currentPlayer.PetInventory.Add(pet);
                    
                    // Salir del combate
                    currentPlayer.IsInCombat = false;
                    currentPlayer.CurrentEnemy = null;
                    StateManager.ForceState(currentPlayer, GameState.Idle, "pet_tamed"); // FSM
                    rpgService.SavePlayer(currentPlayer);
                    
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        $"🐾 **¡DOMADO EXITOSO!**\n\n" +
                        $"{message}\n\n" +
                        $"Nueva mascota: {pet.Name} {pet.RarityEmoji}\n" +
                        $"Lv.{pet.Level} | Bond: {pet.Bond}/1000\n\n" +
                        $"💡 Usa /pets para gestionar tus mascotas",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Ver Mascotas", "pets_main"),
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                            }
                        }),
                        cancellationToken: ct);
                }
                else
                {
                    // Fallo al domar, el enemigo ataca
                    await bot.AnswerCallbackQuery(callbackQuery.Id, message, showAlert: false, cancellationToken: ct);
                    
                    // El enemigo contraataca
                    var combatResult = new BotTelegram.RPG.Services.RpgCombatService().PlayerDefend(currentPlayer, enemy);
                    rpgService.SavePlayer(currentPlayer);
                    
                    var narrative = $"❌ **Fallo al domar**\n\n{message}\n\n";
                    narrative += $"El enemigo contraataca...\n";
                    narrative += $"❤️ HP: {currentPlayer.HP}/{currentPlayer.MaxHP}\n";
                    narrative += $"🔥 Enemigo: {enemy.HP}/{enemy.MaxHP}";
                    
                    if (currentPlayer.HP <= 0)
                    {
                        narrative += "\n\n💀 **Game Over**";
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            narrative,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                                }
                            }),
                            cancellationToken: ct);
                        currentPlayer.HP = currentPlayer.MaxHP / 2;
                        rpgService.SavePlayer(currentPlayer);
                    }
                    else
                    {
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            narrative + "\n\n¿Qué haces?",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: GetCombatKeyboard(),
                            cancellationToken: ct);
                    }
                }
                return;
            }
            
            // Combat - Pet Beast (Acariciar)
            if (data == "rpg_combat_pet")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                var enemy = currentPlayer.CurrentEnemy;
                var petTamingService = new BotTelegram.RPG.Services.PetTamingService(rpgService);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var (success, message) = petTamingService.PetBeast(currentPlayer, enemy, actionTracker);
                
                if (success)
                {
                    // Acción completada (puede o no haber domado instantáneo)
                    rpgService.SavePlayer(currentPlayer);
                    
                    // Si el mensaje contiene "Evento especial", significa que hubo domado instantáneo
                    bool wasInstantTame = message.Contains("Evento especial");
                    
                    if (wasInstantTame)
                    {
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            message,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🐾 Ver Mascotas", "pets_main"),
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main")
                                }
                            }),
                            cancellationToken: ct);
                    }
                    else
                    {
                        // Bond aumentado pero no domado aún
                        await bot.AnswerCallbackQuery(callbackQuery.Id, message, showAlert: false, cancellationToken: ct);
                        
                        var narrative = $"💚 **Acariciaste a la bestia**\n\n";
                        narrative += $"{message}\n\n";
                        narrative += $"❤️ HP: {currentPlayer.HP}/{currentPlayer.MaxHP}\n";
                        narrative += $"🔥 Enemigo: {enemy.HP}/{enemy.MaxHP}";
                        
                        await bot.EditMessageText(
                            chatId,
                            messageId,
                            narrative + "\n\n¿Qué haces?",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: GetCombatKeyboard(),
                            cancellationToken: ct);
                    }
                }
                return;
            }
            
            // Combat - Calm Beast
            if (data == "rpg_combat_calm")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                if (currentPlayer.Mana < 20)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente mana (necesitas 20)", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                var enemy = currentPlayer.CurrentEnemy;
                var petTamingService = new BotTelegram.RPG.Services.PetTamingService(rpgService);
                var actionTracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                var (success, message2) = petTamingService.CalmBeast(currentPlayer, enemy, actionTracker);
                rpgService.SavePlayer(currentPlayer);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, message2, showAlert: false, cancellationToken: ct);
                
                var narrative = $"🎶 **Calmaste a la bestia**\n\n";
                narrative += $"{message2}\n\n";
                narrative += $"❤️ HP: {currentPlayer.HP}/{currentPlayer.MaxHP}\n";
                narrative += $"💧 Mana: {currentPlayer.Mana}/{currentPlayer.MaxMana}\n";
                narrative += $"🔥 Enemigo: {enemy.HP}/{enemy.MaxHP} (Pasivo 2 turnos)";
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    narrative + "\n\n¿Qué haces?",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCombatKeyboard(),
                    cancellationToken: ct);
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // NUEVOS HANDLERS DE COMBATE - ATAQUES
            // ═══════════════════════════════════════════════════════════════
            
            // Combat - Physical Attack
            if (data == "rpg_combat_physical")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ Ataque físico!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PlayerAttack(currentPlayer, enemy, useMagic: false);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**\n\nRegresaste a la taberna...",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Magic Attack
            if (data == "rpg_combat_magic")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                if (currentPlayer.Mana < 10)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Mana insuficiente (necesitas 10)", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🔮 Ataque mágico!", showAlert: false, cancellationToken: ct);
                
                currentPlayer.Mana -= 10;
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PlayerAttack(currentPlayer, enemy, useMagic: true);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Charge Attack
            if (data == "rpg_combat_charge")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💨 Envestida!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.ChargeAttack(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Precise Attack
            if (data == "rpg_combat_precise")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🎯 Ataque preciso!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PreciseAttack(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Heavy Attack
            if (data == "rpg_combat_heavy")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💥 Ataque pesado!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.HeavyAttack(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // HANDLERS DE COMBATE - DEFENSAS
            // ═══════════════════════════════════════════════════════════════
            
            // Combat - Block (mantener compatibilidad con rpg_combat_defend)
            if (data == "rpg_combat_block")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🛡️ Bloqueando!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.PlayerDefend(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Dodge
            if (data == "rpg_combat_dodge")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🌀 Esquivando!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.DodgeAction(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Counter
            if (data == "rpg_combat_counter")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💫 Contraataque!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.CounterAction(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.EnemyDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Continuar", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // HANDLERS DE COMBATE - MOVIMIENTOS
            // ═══════════════════════════════════════════════════════════════
            
            // Combat - Jump
            if (data == "rpg_combat_jump")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🦘 Saltando!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.JumpAction(currentPlayer, enemy);
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                rpgService.SavePlayer(currentPlayer);
                
                if (result.PlayerDefeated)
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                }
                else
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n*Próximo turno...*",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Retreat
            if (data == "rpg_combat_retreat")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🏃 Retrocediendo!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.RetreatAction(currentPlayer, enemy);
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                rpgService.SavePlayer(currentPlayer);
                
                if (result.PlayerDefeated)
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                }
                else
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n*Próximo turno...*",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Advance
            if (data == "rpg_combat_advance")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚡ Avanzando!", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.AdvanceAction(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // HANDLERS DE COMBATE - ACCIONES ESPECIALES
            // ═══════════════════════════════════════════════════════════════
            
            // Combat - Meditate
            if (data == "rpg_combat_meditate")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🧘 Meditando...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.MeditateAction(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Observe
            if (data == "rpg_combat_observe")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "👁️ Observando...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.ObserveAction(currentPlayer, enemy);
                
                // Si hay info revelada, mostrarla primero
                if (!string.IsNullOrEmpty(result.RevealedInfo))
                {
                    await bot.SendMessage(chatId, result.RevealedInfo, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, cancellationToken: ct);
                }
                
                var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                
                rpgService.SavePlayer(currentPlayer);
                
                if (result.PlayerDefeated)
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                }
                else
                {
                    await bot.EditMessageText(chatId, messageId, narrative + "\n\n*Próximo turno...*",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - Wait
            if (data == "rpg_combat_wait")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⏸️ Esperando...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                var result = combatService.WaitAction(currentPlayer, enemy);
                rpgService.SavePlayer(currentPlayer);
                
                var combatMessageId = currentPlayer.ActiveCombatMessageId ?? messageId;
                
                if (result.PlayerDefeated)
                {
                    currentPlayer.ActiveCombatMessageId = null;
                    currentPlayer.HP = currentPlayer.MaxHP / 2;
                    rpgService.SavePlayer(currentPlayer);
                    var narrative = combatService.GetCombatNarrative(result, currentPlayer, enemy);
                    await bot.EditMessageText(chatId, combatMessageId, narrative + "\n\n💀 **Game Over**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Volver", "rpg_main") }
                        }), cancellationToken: ct);
                }
                else
                {
                    await bot.EditMessageText(chatId, combatMessageId, combatService.GenerateCombatMessage(result, currentPlayer, enemy),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: GetCombatKeyboard(), cancellationToken: ct);
                }
                return;
            }
            
            // Combat - AI Consultation
            if (data == "rpg_combat_ai")
            {
                if (!currentPlayer.IsInCombat || currentPlayer.CurrentEnemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en combate", cancellationToken: ct);
                    return;
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🤖 Consultando IA...", showAlert: false, cancellationToken: ct);
                
                var enemy = currentPlayer.CurrentEnemy;
                
                // Generar consulta estratégica con IA
                var aiService = new BotTelegram.Services.AIService();
                var prompt = $@"Eres un consejero táctico de RPG. El jugador está en combate:

**JUGADOR:**
- HP: {currentPlayer.HP}/{currentPlayer.MaxHP}
- Mana: {currentPlayer.Mana}/{currentPlayer.MaxMana}
- ATK: {currentPlayer.PhysicalAttack}
- DEF: {currentPlayer.PhysicalDefense}

**ENEMIGO:**
- Nombre: {enemy.Name}
- HP: {enemy.HP}/{enemy.MaxHP}
- ATK: ~{enemy.Attack}
- Nivel: {enemy.Level}

Responde en español en máximo 2-3 líneas con una estrategia concreta (¿atacar, defender, usar skill, huir?).";

                try
                {
                    var response = await aiService.GenerateRpgNarrative(
                        currentPlayer.Name,
                        currentPlayer.Class.ToString(),
                        currentPlayer.Level,
                        "Consejo táctico en combate",
                        currentPlayer.CurrentLocation,
                        enemy.Name,
                        $"HP Jugador: {currentPlayer.HP}/{currentPlayer.MaxHP}, HP Enemigo: {enemy.HP}/{enemy.MaxHP}"
                    );
                    
                    var text = $"🤖 **CONSEJO TÁCTICO**\n\n{response}\n\n";
                    text += $"💚 Tu HP: {currentPlayer.HP}/{currentPlayer.MaxHP}\n";
                    text += $"💙 Tu Mana: {currentPlayer.Mana}/{currentPlayer.MaxMana}\n";
                    text += $"👹 {enemy.Emoji} {enemy.Name}: {enemy.HP}/{enemy.MaxHP} HP";
                    
                    await bot.SendMessage(chatId, text, 
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, 
                        cancellationToken: ct);
                    
                    // No consume turno, solo muestra consejo
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Consejo recibido", cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en AI consultation: {ex.Message}");
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Error al consultar IA", showAlert: true, cancellationToken: ct);
                }
                
                return;
            }
            
            // Rest
            if (data == "rpg_rest")
            {
                // VALIDAR ESTADO: Solo permitido en Idle
                if (!StateManager.IsActionAllowed(currentPlayer, data))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes descansar durante el combate o viaje", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "😴 Descansando...", cancellationToken: ct);
                
                var tracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                // Restaurar recursos
                rpgService.RestoreEnergy(currentPlayer, currentPlayer.MaxEnergy);
                var hpRestored = currentPlayer.MaxHP / 4;
                rpgService.RestoreHP(currentPlayer, hpRestored);
                
                // Regenerar mana (50% del maná máximo)
                var manaRestored = (int)(currentPlayer.MaxMana * 0.5);
                currentPlayer.Mana = Math.Min(currentPlayer.MaxMana, currentPlayer.Mana + manaRestored);
                tracker.TrackAction(currentPlayer, "mana_regen", manaRestored);
                tracker.TrackAction(currentPlayer, "rest");
                
                rpgService.SavePlayer(currentPlayer);
                    
                await bot.DeleteMessage(chatId, messageId, ct);
                await rpgCommand.ShowMainMenu(bot, chatId, currentPlayer, ct);
                return;
            }
            
            // Train
            if (data == "rpg_train")
            {
                if (!rpgService.CanPerformAction(currentPlayer, 20))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 20)", cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚡ Entrenando...", cancellationToken: ct);
                
                var tracker = new BotTelegram.RPG.Services.ActionTrackerService(rpgService);
                
                // Consumir energía y ganar XP
                var xpBefore = currentPlayer.XP;
                rpgService.ConsumeEnergy(currentPlayer, 20);
                rpgService.AddXP(currentPlayer, 15);
                
                // Trackear acción de entrenamiento
                tracker.TrackAction(currentPlayer, "training");
                
                rpgService.SavePlayer(currentPlayer);
                
                // Calcular progreso XP
                var xpNeeded = currentPlayer.Level * 100;
                var xpProgress = (double)currentPlayer.XP / xpNeeded;
                var xpBar = GetProgressBar(xpProgress, 15);
                
                var text = "⚡ **ENTRENAMIENTO**\n\n";
                text += "_Practicas con los muñecos de entrenamiento, perfeccionando tus técnicas de combate..._\n\n";
                
                text += "🎯 **EXPERIENCIA GANADA**\n";
                text += $"{xpBar}\n";
                text += $"💠 XP: {currentPlayer.XP}/{xpNeeded} (Nivel {currentPlayer.Level})\n";
                text += $"✨ +**15 XP** ganado\n\n";
                
                text += "⚡ **COSTO:**\n";
                text += $"🔋 -20 Energía (Restante: {currentPlayer.Energy}/{currentPlayer.MaxEnergy})\n\n";
                
                // Mostrar progreso hacia la próxima habilidad
                var trainingCount = tracker.GetActionCount(currentPlayer, "training");
                text += $"📊 **Sesiones de entrenamiento:** {trainingCount}\n";
                
                // Verificar clases que requieren entrenamiento
                var classesNeedingTraining = BotTelegram.RPG.Services.HiddenClassDatabase.GetAll()
                    .Where(c => !currentPlayer.UnlockedHiddenClasses.Contains(c.Id) && 
                                c.RequiredActions.ContainsKey("training"))
                    .Take(2);
                
                if (classesNeedingTraining.Any())
                {
                    text += "\n🌟 **Progreso hacia clases:**\n";
                    foreach (var hClass in classesNeedingTraining)
                    {
                        var required = hClass.RequiredActions["training"];
                        var classProgress = Math.Min((double)trainingCount / required, 1.0);
                        var classBar = GetProgressBar(classProgress, 10);
                        text += $"{hClass.Emoji} {hClass.Name}: {classBar} {trainingCount}/{required}\n";
                    }
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚡ Entrenar de nuevo", "rpg_train"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_actions")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Work
            if (data == "rpg_work")
            {
                // VALIDAR ESTADO: Solo permitido en Idle
                if (!StateManager.IsActionAllowed(currentPlayer, data))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes trabajar durante el combate o viaje", showAlert: true, cancellationToken: ct);
                    return;
                }
                
                if (!rpgService.CanPerformAction(currentPlayer, 10))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No tienes suficiente energía (necesitas 10)", cancellationToken: ct);
                    return;
                }
                
                // Responder al callback INMEDIATAMENTE
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💼 Trabajando...", cancellationToken: ct);
                
                rpgService.ConsumeEnergy(currentPlayer, 10);
                currentPlayer.Gold += 30;
                rpgService.SavePlayer(currentPlayer);
                
                TelegramLogger.LogRpgEvent(
                    chatId,
                    currentPlayer.Name,
                    "work_action",
                    $"Worked for +30 gold. Energy: {currentPlayer.Energy}");
                    
                await bot.DeleteMessage(chatId, messageId, ct);
                await rpgCommand.ShowMainMenu(bot, chatId, currentPlayer, ct);
                return;
            }
            
            // Lore (Historia del juego)
            if (data == "rpg_lore")
            {
                var loreText = @"📖 **LEYENDA DEL VOID**

*La Historia de Valentia*

Hace milenios, el reino de Valentia era un paraíso de paz y prosperidad. Magos y guerreros vivían en armonía, protegidos por las antiguas defensas de los Primigenios.

Pero todo cambió cuando *el Void* se abrió...

🌑 **El Void**
Una grieta entre dimensiones que apareció sin previo aviso. De ella emergieron criaturas de pesadilla: sombras vivientes, bestias corrompidas, y horrores ancestrales.

⚔️ **Tu Destino**
Los héroes de antaño cayeron uno por uno. Ahora, eres la última esperanza de Valentia. Debes ganar fuerza, explorar tierras olvidadas, y enfrentar las fuerzas del Void antes de que consuman todo.

🔮 **La Profecía**
*'Cuando la oscuridad amenace con devorar la luz, un héroe surgirá de entre las sombras. Solo aquel que domine las cuatro artes podrá sellar el Void y restaurar el equilibrio.'*

📍 **Tu Aventura Comienza**
En Puerto Esperanza, la última ciudad libre. Desde aquí, tu leyenda comenzará...";

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    loreText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Comenzar Aventura", "rpg_main"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❓ Cómo Jugar", "rpg_tutorial")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Tutorial
            if (data == "rpg_tutorial")
            {
                var tutorialText = @"❓ **CÓMO JUGAR - GUÍA RÁPIDA**

**🎮 CONCEPTOS BÁSICOS**

*Stats Principales:*
• ❤️ HP - Puntos de vida
• ⚡ Energía - Para acciones
• ⭐ XP - Experiencia (sube de nivel)
• 💰 Oro - Moneda del juego

*Acciones Principales:*
⚔️ **Explorar** - Busca enemigos (15 energía)
🛡️ **Entrenar** - Gana XP (20 energía)
😴 **Descansar** - Recupera HP y energía
💼 **Trabajar** - Gana oro (10 energía)

**⚔️ COMBATE**

🎲 Sistema d20 (como D&D):
• Ataque: d20 + tu ataque vs defensa enemiga
• Crítico: 20 en el dado (×2 daño)
• Crítico fallido: 1 en el dado

*Opciones en combate:*
⚔️ Atacar - Daño normal
🛡️ Defender - Reduce daño recibido
🏃 Huir - 75% probabilidad de éxito

**📈 PROGRESIÓN**

*Sistema de Clases (14 clases):*
• Tier 1 (Lv.1): Warrior, Mage, Rogue, Cleric
• Tier 2 (Lv.10+): Evoluciones intermedias
• Tier 3 (Lv.20+): Clases avanzadas
• Tier 4 (Lv.30+): Clases maestras

💡 *Tip: Descansa regularmente para mantener HP/energía altos*";

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    tutorialText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Jugar Ahora", "rpg_main"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📖 Lore", "rpg_lore")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Options (Configuración)
            if (data == "rpg_options")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Primero crea un personaje", cancellationToken: ct);
                    return;
                }
                
                var optionsText = $@"⚙️ **OPCIONES DE PERSONAJE**

👤 **{player.Name}** - {player.Class} Nv.{player.Level}
📊 Stats totales: {player.Strength + player.Intelligence + player.Dexterity + player.Constitution + player.Wisdom + player.Charisma}

**Información del Personaje:**
• Creado: {player.CreatedAt:dd/MM/yyyy HH:mm}
• Ubicación: {player.CurrentLocation}
• Enemigos derrotados: {player.Level * 2}
• Tiempo jugado: {(DateTime.UtcNow - player.CreatedAt).TotalHours:F1}h

**📦 BACKUP Y RESTAURACIÓN:**
💾 *Exportar* - Guarda tu personaje en archivo JSON
📥 *Importar* - Restaura personaje desde backup

**⚠️ ACCIONES:**
🗑️ *Borrar Personaje* - Empieza de nuevo
📊 *Ver Stats Completos* - Detalles de atributos

💡 *Nota: El borrado es permanente*";

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    optionsText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💾 Exportar", "rpg_export_character"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📥 Importar", "rpg_import_character")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗑️ Borrar Personaje", "rpg_confirm_delete")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver al Juego", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Confirm delete character
            if (data == "rpg_confirm_delete")
            {
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "⚠️ **¿BORRAR PERSONAJE?**\n\n" +
                    "Esta acción es **PERMANENTE**.\n" +
                    "Perderás todo tu progreso, items, y oro.\n\n" +
                    "¿Estás completamente seguro?",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ SÍ, BORRAR TODO", "rpg_delete_confirmed")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Cancelar", "rpg_options")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Delete confirmed
            if (data == "rpg_delete_confirmed")
            {
                rpgService.DeletePlayer(chatId);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🗑️ **Personaje Borrado**\n\n" +
                    "Tu aventura ha terminado.\n" +
                    "Puedes crear un nuevo personaje cuando quieras.\n\n" +
                    "✨ *Adiós, héroe...*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Nueva Aventura", "rpg_main")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // EXPORT / IMPORT CHARACTER (FASE 5 - PERSISTENCIA)
            // ═══════════════════════════════════════════════════════════════
            
            // Export character to JSON
            if (data == "rpg_export_character")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No hay personaje para exportar", cancellationToken: ct);
                    return;
                }
                
                var json = rpgService.ExportPlayerData(player);
                
                if (string.IsNullOrEmpty(json))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Error al exportar personaje", cancellationToken: ct);
                    return;
                }
                
                // Convertir JSON a bytes
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                var fileName = $"{player.Name}_Lv{player.Level}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                
                using (var stream = new System.IO.MemoryStream(bytes))
                {
                    await bot.SendDocument(
                        chatId,
                        new Telegram.Bot.Types.InputFileStream(stream, fileName),
                        caption: $"💾 **BACKUP DE PERSONAJE**\n\n" +
                                $"👤 **{player.Name}** - {player.Class} Nv.{player.Level}\n" +
                                $"📅 Exportado: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                                $"💡 *Guarda este archivo en un lugar seguro.*\n" +
                                $"Puedes importarlo con **⚙️ Opciones → 📥 Importar**",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: ct);
                }
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Personaje exportado", cancellationToken: ct);
                return;
            }
            
            // Import character from JSON
            if (data == "rpg_import_character")
            {
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "📥 **IMPORTAR PERSONAJE**\n\n" +
                    "**Paso 1:** Envía el archivo JSON de tu personaje\n" +
                    "**Paso 2:** El sistema validará los datos\n" +
                    "**Paso 3:** Tu personaje será restaurado\n\n" +
                    "⚠️ *Aviso:* Si ya tienes un personaje, será **reemplazado**.\n\n" +
                    "📎 **Envía el archivo .json ahora** o cancela.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Cancelar", "rpg_options")
                        }
                    }),
                    cancellationToken: ct);
                
                // Marcar que estamos esperando un archivo
                BotTelegram.RPG.Services.RpgService.SetAwaitingImport(chatId, true);
                
                return;
            }
            
            // Download logs
            if (data == "rpg_download_logs")
            {
                try
                {
                    var userLogs = BotTelegram.Services.TelegramLogger.GetUserLogFiles(chatId);
                    
                    if (userLogs.Count == 0)
                    {
                        await bot.AnswerCallbackQuery(callbackQuery.Id, "📭 No hay logs disponibles aún", showAlert: true, cancellationToken: ct);
                        return;
                    }
                    
                    await bot.AnswerCallbackQuery(callbackQuery.Id, $"📊 Descargando {userLogs.Count} archivo(s) de log...", cancellationToken: ct);
                    
                    // Enviar cada archivo de log
                    foreach (var logFile in userLogs)
                    {
                        using (var stream = System.IO.File.OpenRead(logFile))
                        {
                            var fileName = System.IO.Path.GetFileName(logFile);
                            await bot.SendDocument(
                                chatId,
                                new Telegram.Bot.Types.InputFileStream(stream, fileName),
                                caption: $"📋 *Log: {fileName}*\n\nÚsalo para auditoría y análisis de pruebas.",
                                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                cancellationToken: ct);
                        }
                    }
                    
                    await bot.SendMessage(
                        chatId,
                        "✅ **Logs descargados exitosamente**\n\n" +
                        $"📦 Total de archivos: {userLogs.Count}\n" +
                        "💾 Están listos para análisis y auditoría",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver a Opciones", "rpg_options")
                            }
                        }),
                        cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error descargando logs: {ex.Message}");
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Error al descargar logs", showAlert: true, cancellationToken: ct);
                }
                return;
            }
            
            // AI Chat integration with RPG
            if (data == "rpg_ai_chat")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Primero crea un personaje", cancellationToken: ct);
                    return;
                }
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    $"💬 **CHAT CON IA - MODO RPG**\n\n" +
                    $"Chatea con la IA sobre tu aventura:\n\n" +
                    $"👤 {player.Name} ({player.Class} Nv.{player.Level})\n" +
                    $"📍 {player.CurrentLocation}\n\n" +
                    $"✨ *La IA conoce tu personaje y puede:*\n" +
                    $"• Narrar tus aventuras\n" +
                    $"• Darte consejos de estrategia\n" +
                    $"• Crear historias personalizadas\n" +
                    $"• Describir el mundo de Valentia\n\n" +
                    $"💡 Usa `/chat <mensaje>` para hablar con la IA\n" +
                    $"📝 La IA recordará toda la conversación\n\n" +
                    $"⚠️ *Importante:* Este chat es INDEPENDIENTE del chat normal de IA",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💬 Iniciar Chat RPG", "rpg_start_chat")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver al Juego", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                return;
            }
            
            // Iniciar chat con modo RPG
            if (data == "rpg_start_chat")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Primero crea un personaje", cancellationToken: ct);
                    return;
                }
                
                // Activar modo RPG chat
                BotTelegram.Services.AIService.SetRpgChatMode(chatId, true);
                
                await bot.AnswerCallbackQuery(
                    callbackQuery.Id,
                    "✅ Modo RPG activado. Escribe tu mensaje.",
                    showAlert: false,
                    cancellationToken: ct);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    $"🎮 **MODO CHAT RPG ACTIVADO**\n\n" +
                    $"✅ La IA ahora conoce tu contexto RPG:\n\n" +
                    $"👤 **{player.Name}**\n" +
                    $"🏹 Clase: {player.Class} Nv.{player.Level}\n" +
                    $"📍 Ubicación: {player.CurrentLocation}\n" +
                    $"❤️ Vida: {player.HP}/{player.MaxHP}\n" +
                    $"🔮 Mana: {player.Mana}/{player.MaxMana}\n\n" +
                    $"💬 **¡Empieza a conversar!**\n" +
                    $"Escribe cualquier mensaje y la IA responderá con narrativa contextualizada.\n\n" +
                    $"💡 **Ejemplos:**\n" +
                    $"• _\"Describe el ambiente de la taberna\"_\n" +
                    $"• _\"Dame un consejo para mi próxima batalla\"_\n" +
                    $"• _\"Cuéntame sobre el Void\"_\n" +
                    $"• _\"Qué debo hacer ahora\"_\n\n" +
                    $"🛑 **Para salir:** Usa el botón de abajo o vuelve al menú RPG",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🚫 Desactivar Modo RPG", "rpg_stop_chat")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Ver Menú RPG", "rpg_main")
                        }
                    }),
                    cancellationToken: ct);
                
                TelegramLogger.LogRpgEvent(
                    chatId,
                    player.Name,
                    "rpg_chat_activated",
                    "User activated RPG AI chat mode");
                
                return;
            }
            
            // Desactivar chat con modo RPG
            if (data == "rpg_stop_chat")
            {
                var player = rpgService.GetPlayer(chatId);
                if (player == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Primero crea un personaje", cancellationToken: ct);
                    return;
                }
                
                // Desactivar modo RPG chat
                BotTelegram.Services.AIService.SetRpgChatMode(chatId, false);
                
                await bot.AnswerCallbackQuery(
                    callbackQuery.Id,
                    "🚫 Modo RPG desactivado",
                    showAlert: false,
                    cancellationToken: ct);
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    $"🚪 **MODO CHAT RPG DESACTIVADO**\n\n" +
                    $"✅ Has salido del modo chat RPG exitosamente.\n\n" +
                    $"💬 **Conversaciones guardadas:**\n" +
                    $"Tu historial de chat RPG se mantiene guardado y puedes reanudarlo cuando quieras.\n\n" +
                    $"🔄 **Para reactivar:**\n" +
                    $"Vuelve a este menú y presiona 'Activar Modo RPG'.\n\n" +
                    $"🎮 **¿Qué hacer ahora?**\n" +
                    $"Puedes continuar tu aventura o explorar el mundo de Valentia.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💬 Volver a Activar", "rpg_start_chat")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🎮 Menú RPG", "rpg_main"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🗺️ Explorar", "rpg_explore")
                        }
                    }),
                    cancellationToken: ct);
                
                TelegramLogger.LogRpgEvent(
                    chatId,
                    player.Name,
                    "rpg_chat_deactivated",
                    "User deactivated RPG AI chat mode");
                
                return;
            }

            // rpg_back_welcome — botón Volver en pantalla de creación de nombre
            if (data == "rpg_back_welcome")
            {
                BotTelegram.RPG.Services.RpgService.SetAwaitingName(chatId, false);
                await rpgCommand.Execute(bot, callbackQuery.Message!, ct);
                return;
            }

            // rpg_guide — Guía del juego
            if (data == "rpg_guide")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                var guideText = "📖 **GUÍA DEL JUEGO — Leyenda del Void**\n\n" +
                    "**🎮 Empezar:**\n" +
                    "Usa `/rpg` para abrir el menú principal.\n" +
                    "Crea tu personaje y elige una clase base.\n\n" +
                    "**⚔️ Clases base:**\n" +
                    "• Guerrero — Físico, alto HP\n" +
                    "• Mago — Mágico, alto daño\n" +
                    "• Pícaro — Velocidad y críticos\n" +
                    "• Clérigo — Soporte y curación\n\n" +
                    "**🗺️ Explorar:**\nBusca enemigos, recursos y tesoros en tu zona actual.\n\n" +
                    "**🏰 Mazmorras:**\nCompleta pisos para obtener llaves y equipamiento raro.\n\n" +
                    "**🛡️ Gremio:**\nÚnete o crea un gremio para cooperar con otros.\n\n" +
                    "**⚔️ Arena PvP:**\nDesafía otros jugadores y sube tu rating ELO.\n\n" +
                    "**💡 Consejo:** Usa la Ciudad para craftear, comprar y aceptar misiones.";
                await bot.EditMessageText(chatId, messageId, guideText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_help") }
                    }), cancellationToken: ct);
                return;
            }

            // rpg_commands — Lista de comandos
            if (data == "rpg_commands")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                var cmdText = "📊 **COMANDOS DISPONIBLES**\n\n" +
                    "`/rpg` — Menú principal RPG\n" +
                    "`/map` — Mapa de zonas\n" +
                    "`/stats` — Estadísticas del personaje\n" +
                    "`/pets` — Gestionar mascotas\n" +
                    "`/leaderboard` — Rankings globales\n" +
                    "`/gremio` — Sistema de Gremio\n" +
                    "`/arena` — Arena PvP\n" +
                    "`/chat` — Chat con IA\n" +
                    "`/start` — Menú principal del bot\n" +
                    "`/help` — Ayuda rápida";
                await bot.EditMessageText(chatId, messageId, cmdText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_help") }
                    }), cancellationToken: ct);
                return;
            }

            // rpg_bug_report — Reportar bug
            if (data == "rpg_bug_report")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await bot.EditMessageText(chatId, messageId,
                    "🐛 **REPORTAR BUG**\n\n" +
                    "Para reportar un error, envía un mensaje describiendo:\n\n" +
                    "1️⃣ ¿Qué acción realizaste?\n" +
                    "2️⃣ ¿Qué esperabas que ocurriera?\n" +
                    "3️⃣ ¿Qué ocurrió en realidad?\n\n" +
                    "Puedes usar el chat `/chat` para describírselo directamente al asistente.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💬 Abrir Chat IA", "rpg_ai_chat") },
                        new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_menu_help") }
                    }), cancellationToken: ct);
                return;
            }

            // ─── ENTRENAMIENTO (train_*) ─────────────────────
            if (data.StartsWith("train_"))
            {
                await BotTelegram.RPG.Commands.TrainingCommand.HandleCallback(bot, callbackQuery, data, ct);
                return;
            }

            // Default
            await bot.AnswerCallbackQuery(callbackQuery.Id, "🚧 Función en desarrollo", cancellationToken: ct);
        }
        
        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Convierte action IDs a nombres legibles en español
        /// </summary>
        private static string GetActionName(string actionId)
        {
            return actionId switch
            {
                // ═══════════════════════════════════════
                // COMBATE BÁSICO
                // ═══════════════════════════════════════
                "physical_attack" => "Ataques físicos",
                "magic_attack" => "Ataques mágicos",
                "critical_hit" => "Golpes críticos",
                "dodge_success" => "Esquivas exitosas",
                "dodge" => "Evasiones",
                "block" => "Bloqueos",
                "defend" => "Defensas",
                "counter" => "Contraataques",
                "counter_attack" => "Contraataques",
                "perfect_parry" => "Parrys perfectos",
                "berserk" => "Usos de Berserk",
                "kill_enemy" => "Enemigos eliminados",
                "enemy_kill" => "Enemigos eliminados",
                "take_damage" => "Daño recibido",
                "survive_low_hp" => "Supervivencias HP crítico",
                
                // ═══════════════════════════════════════
                // COMBATE AVANZADO (FASE 5C)
                // ═══════════════════════════════════════
                "approach_enemy" => "Acercarse al enemigo",
                "retreat" => "Retirarse/huir",
                "charge_attack" => "Envestidas",
                "heavy_attack" => "Ataques pesados",
                "light_attack" => "Ataques rápidos",
                "precise_attack" => "Ataques precisos",
                "reckless_attack" => "Ataques temerarios",
                "defensive_attack" => "Ataques defensivos",
                "consecutive_attacks" => "Ataques consecutivos",
                "combo_3x" => "Combos 3x",
                "combo_5x" => "Combos 5x",
                "combo_10x" => "Combos 10x",
                "combo_20x" => "Combos 20x",
                "overkill_damage" => "Overkills",
                "no_damage_combat" => "Combates sin daño",
                "no_critical_combat" => "Combates sin crítico",
                "speed_advantage" => "Ventajas de velocidad",
                "double_turn" => "Turnos dobles",
                
                // ═══════════════════════════════════════
                // DEFENSA Y SUPERVIVENCIA
                // ═══════════════════════════════════════
                "block_damage" => "Daño bloqueado (total)",
                "perfect_block" => "Bloqueos perfectos",
                "parry" => "Contragolpes",
                "tank_hit" => "Golpes tanqueados",
                "survive_lethal" => "Supervivencias letales",
                "survive_critical" => "Supervivencias a críticos",
                "hp_below_10_survive" => "Supervivencias <10% HP",
                "hp_below_30_kill" => "Kills con <30% HP",
                "low_hp_combat" => "Combates HP baja",
                "no_dodge_combat" => "Combates sin esquivar",
                "damage_taken" => "Daño recibido (total)",
                "shield_bash" => "Golpes de escudo",
                "taunt_enemy" => "Provocaciones",
                
                // ═══════════════════════════════════════
                // MAGIA Y MANA
                // ═══════════════════════════════════════
                "spell_cast" => "Hechizos lanzados",
                "fire_spell_cast" => "Hechizos de fuego",
                "water_spell_cast" => "Hechizos de agua",
                "earth_spell_cast" => "Hechizos de tierra",
                "air_spell_cast" => "Hechizos de aire",
                "ice_spell_cast" => "Hechizos de hielo",
                "lightning_spell_cast" => "Hechizos de rayo",
                "dark_magic_cast" => "Magia oscura",
                "holy_magic_cast" => "Magia sagrada",
                "void_magic_cast" => "Magia del vacío",
                "combo_spell" => "Combos elementales",
                "spell_critical" => "Críticos mágicos",
                "mana_spent" => "Mana gastado (total)",
                "mana_regen" => "Mana regenerado (total)",
                "low_mana_cast" => "Casteos con mana bajo",
                "mana_drain" => "Drenar mana",
                "overcharge_spell" => "Spells sobrecargados",
                
                // ═══════════════════════════════════════
                // INVOCACIÓN Y MASCOTAS
                // ═══════════════════════════════════════
                "summon_undead" => "Invocar no-muertos",
                "summon_elemental" => "Invocar elementales",
                "summon_skeleton" => "Invocar esqueletos",
                "summon_zombie" => "Invocar zombies",
                "summon_ghost" => "Invocar fantasmas",
                "summon_lich" => "Invocar lichs",
                "summon_beast" => "Invocar bestias",
                "summon_aberration" => "Invocar aberraciones",
                "sacrifice_minion" => "Sacrificar minion",
                "pet_bond_max" => "Bonds máximos",
                "pet_evolution" => "Evoluciones de mascotas",
                "pet_combo_kill" => "Kills combo con mascota",
                "pet_death" => "Mascotas caídas",
                "pet_action" => "Acciones con mascotas",
                "tame_boss" => "Domar bosses",
                "tame_creature" => "Criaturas domadas",
                
                // ═══════════════════════════════════════
                // STEALTH Y ENGAÑO
                // ═══════════════════════════════════════
                "stealth_approach" => "Acercamientos sigilosos",
                "stealth_kill" => "Asesinatos sigilosos",
                "backstab" => "Ataques por la espalda",
                "vanish" => "Desvanecimientos",
                "shadow_travel" => "Viajes por sombras",
                "assassination" => "Asesinatos",
                
                // ═══════════════════════════════════════
                // CRAFTING Y RECURSOS
                // ═══════════════════════════════════════
                "craft_item" => "Ítems crafteados",
                "upgrade_equipment" => "Equipos mejorados",
                "enchant_equipment" => "Equipos encantados",
                "forge_weapon" => "Armas forjadas",
                "gather_herbs" => "Hierbas recolectadas",
                "mine_ore" => "Minerales minados",
                "fish" => "Peces pescados",
                "cook_food" => "Comidas cocinadas",
                
                // ═══════════════════════════════════════
                // SOCIAL Y EXPLORACIÓN
                // ═══════════════════════════════════════
                "trade_npc" => "Comercios con NPCs",
                "negotiate" => "Negociaciones",
                "quest_complete" => "Misiones completadas",
                "discover_zone" => "Zonas descubiertas",
                "boss_encounter" => "Encuentros con bosses",
                
                // ═══════════════════════════════════════
                // PROGRESO
                // ═══════════════════════════════════════
                "level_up" => "Subir de nivel",
                "boss_kill" => "Jefes derrotados",
                "beast_kills" => "Bestias derrotadas",
                "undead_kills" => "No-muertos derrotados",
                
                // ═══════════════════════════════════════
                // EXPLORACIÓN
                // ═══════════════════════════════════════
                "meditation" => "Meditaciones",
                "rest" => "Descansos",
                "explore" => "Exploraciones",
                "treasure_found" => "Tesoros encontrados",
                "loot_found" => "Loot recolectado",
                
                // ═══════════════════════════════════════
                // INTERACCIÓN CON BESTIAS
                // ═══════════════════════════════════════
                "pet_beast" => "Acariciar bestias",
                "calm_beast" => "Calmar bestias",
                "tame_beast" => "Domar bestias",
                
                // ═══════════════════════════════════════
                // COMBOS
                // ═══════════════════════════════════════
                "combo_5plus" => "Combos 5+ hits",
                "combo_10plus" => "Combos 10+ hits",
                "combo_20plus" => "Combos 20+ hits",
                
                // ═══════════════════════════════════════
                // NIGROMANCIA
                // ═══════════════════════════════════════
                "life_drain" => "Drenar vida",
                "desecrate" => "Profanaciones",
                "sacrifice" => "Sacrificios",
                
                // ═══════════════════════════════════════
                // OTROS
                // ═══════════════════════════════════════
                "low_hp_victory" => "Victorias con HP baja",
                "heal" => "Curaciones",
                "divine_bless" => "Bendiciones",
                "revive_ally" => "Resurrecciones",
                "skill_used" => "Habilidades usadas",
                "gold_earned" => "Oro acumulado",
                
                _ => actionId.Replace("_", " ").Replace("skill ", "")
            };
        }
        
        // ═══════════════════════════════════════════════════════════════
        // FASE 2: HANDLERS DE CALLBACKS DEL SISTEMA DE MASCOTAS
        // ═══════════════════════════════════════════════════════════════
        
        private static async Task HandlePetsCallback(
            ITelegramBotClient bot,
            Telegram.Bot.Types.CallbackQuery callbackQuery,
            string data,
            CancellationToken ct)
        {
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;
            var rpgService = new BotTelegram.RPG.Services.RpgService();
            var petTamingService = new BotTelegram.RPG.Services.PetTamingService(rpgService);
            
            var player = rpgService.GetPlayer(chatId);
            if (player == null)
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Necesitas crear un personaje primero", cancellationToken: ct);
                return;
            }
            
            // pets_list_all - Listar todas las mascotas
            if (data == "pets_list_all")
            {
                var text = "🐾 **LISTA COMPLETA DE MASCOTAS**\n\n";
                
                if (player.PetInventory == null || player.PetInventory.Count == 0)
                {
                    text += "❌ No tienes ninguna mascota domada.\n\n";
                    text += "💡 Encuentra bestias en exploración y dómalas en combate.";
                }
                else
                {
                    foreach (var pet in player.PetInventory.OrderByDescending(p => p.Level).ThenBy(p => p.Name))
                    {
                        var emoji = GetPetEmoji(pet.Species);
                        var isActive = player.ActivePets?.Contains(pet) ?? false;
                        var activeTag = isActive ? "⚔️ " : "💤 ";
                        
                        text += $"{activeTag}{emoji} **{pet.Name}** {pet.RarityEmoji}\n";
                        text += $"   Lv.{pet.Level} | HP: {pet.HP}/{pet.MaxHP}\n";
                        text += $"   {pet.LoyaltyEmoji} {pet.Loyalty} | Bond: {pet.Bond}/1000\n";
                        text += $"   ATK: {pet.EffectiveAttack} | DEF: {pet.EffectiveDefense} | SPD: {pet.Speed}\n";
                        text += $"   Kills: {pet.TotalKills} | Boss: {pet.BossKills}\n\n";
                    }
                }
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⚔️ Gestionar Activas", "pets_manage_active"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🍖 Alimentar", "pets_feed_menu")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⭐ Evolucionar", "pets_evolve_menu"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }
            
            // pets_manage_active - Gestionar mascotas activas
            if (data == "pets_manage_active")
            {
                var text = "⚔️ **GESTIONAR MASCOTAS ACTIVAS**\n\n";
                text += $"Límite: {player.ActivePets?.Count ?? 0}/{player.MaxActivePets}\n\n";
                
                if (player.PetInventory == null || player.PetInventory.Count == 0)
                {
                    text += "❌ No tienes mascotas para activar.";
                }
                else
                {
                    text += "Selecciona una mascota para activar/desactivar:\n\n";
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(pet.Species);
                        var isActive = player.ActivePets?.Contains(pet) ?? false;
                        var status = isActive ? "✅ ACTIVA" : "💤 Inactiva";
                        text += $"{emoji} {pet.Name} (Lv.{pet.Level}) - {status}\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null)
                {
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var isActive = player.ActivePets?.Contains(pet) ?? false;
                        var buttonText = isActive ? $"❌ Desactivar {pet.Name}" : $"✅ Activar {pet.Name}";
                        rows.Add(new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(buttonText, $"pets_toggle_{pet.Id}")
                        });
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_toggle_{id} - Activar/Desactivar mascota
            if (data.StartsWith("pets_toggle_"))
            {
                var petId = data.Replace("pets_toggle_", "");
                var message = petTamingService.ToggleActivePet(player, petId);
                
                rpgService.SavePlayer(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, message, cancellationToken: ct);
                
                // Reconstruir menú de gestión con datos actualizados
                var text = "⚔️ **GESTIONAR MASCOTAS ACTIVAS**\n\n";
                text += $"Límite: {player.ActivePets?.Count ?? 0}/{player.MaxActivePets}\n\n";
                
                if (player.PetInventory == null || player.PetInventory.Count == 0)
                {
                    text += "❌ No tienes mascotas para activar.";
                }
                else
                {
                    text += "Selecciona una mascota para activar/desactivar:\n\n";
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(pet.Species);
                        var isActive = player.ActivePets?.Contains(pet) ?? false;
                        var status = isActive ? "✅ ACTIVA" : "💤 Inactiva";
                        text += $"{emoji} {pet.Name} (Lv.{pet.Level}) - {status}\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null)
                {
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var isActive = player.ActivePets?.Contains(pet) ?? false;
                        var buttonText = isActive ? $"❌ Desactivar {pet.Name}" : $"✅ Activar {pet.Name}";
                        rows.Add(new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(buttonText, $"pets_toggle_{pet.Id}")
                        });
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_feed_menu - Menú de alimentación
            if (data == "pets_feed_menu")
            {
                var text = "🍖 **ALIMENTAR MASCOTAS**\n\n";
                text += $"💰 Oro disponible: **{player.Gold}**\n";
                text += "Costo: **5 oro** por mascota\n\n";
                text += "💚 Beneficios:\n";
                text += "• +20 puntos de Bond\n";
                text += "• +30% HP restaurado\n";
                text += "• Mejora la lealtad\n\n";
                
                if (player.PetInventory == null || player.PetInventory.Count == 0)
                {
                    text += "❌ No tienes mascotas para alimentar.";
                }
                else
                {
                    text += "Selecciona una mascota:\n\n";
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(pet.Species);
                        var hpPercent = (double)pet.HP / pet.MaxHP * 100;
                        text += $"{emoji} {pet.Name} - Bond: {pet.Bond}/1000 (HP: {hpPercent:F0}%)\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null && player.Gold >= 5)
                {
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        rows.Add(new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"🍖 Alimentar {pet.Name}", $"pets_feed_{pet.Id}")
                        });
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_feed_{id} - Alimentar mascota específica
            if (data.StartsWith("pets_feed_"))
            {
                var petId = data.Replace("pets_feed_", "");
                var pet = player.PetInventory?.FirstOrDefault(p => p.Id == petId);
                
                if (pet == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Mascota no encontrada", cancellationToken: ct);
                    return;
                }
                
                var message = petTamingService.FeedPet(player, pet);
                rpgService.SavePlayer(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, message, showAlert: true, cancellationToken: ct);
                
                // Reconstruir menú de alimentación con datos actualizados
                var text = "🍖 **ALIMENTAR MASCOTAS**\n\n";
                text += $"💰 Oro disponible: **{player.Gold}**\n";
                text += "Costo: **5 oro** por mascota\n\n";
                text += "💚 Beneficios:\n";
                text += "• +20 puntos de Bond\n";
                text += "• +30% HP restaurado\n";
                text += "• Mejora la lealtad\n\n";
                
                if (player.PetInventory == null || player.PetInventory.Count == 0)
                {
                    text += "❌ No tienes mascotas para alimentar.";
                }
                else
                {
                    text += "Selecciona una mascota:\n\n";
                    foreach (var petItem in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(petItem.Species);
                        var hpPercent = (double)petItem.HP / petItem.MaxHP * 100;
                        text += $"{emoji} {petItem.Name} - Bond: {petItem.Bond}/1000 (HP: {hpPercent:F0}%)\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null && player.Gold >= 5)
                {
                    foreach (var petItem in player.PetInventory.Take(8))
                    {
                        rows.Add(new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"🍖 Alimentar {petItem.Name}", $"pets_feed_{petItem.Id}")
                        });
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_evolve_menu - Menú de evolución
            if (data == "pets_evolve_menu")
            {
                var text = "⭐ **EVOLUCIONAR MASCOTAS**\n\n";
                text += "Las mascotas pueden evolucionar 3 veces:\n";
                text += "🌱 Básica → 🌿 Avanzada → 🌳 Definitiva\n\n";
                
                var canEvolveCount = 0;
                if (player.PetInventory != null)
                {
                    foreach (var pet in player.PetInventory)
                    {
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(pet.Species);
                        if (speciesData?.EvolutionRequirements != null)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            if (pet.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills))
                            {
                                canEvolveCount++;
                            }
                        }
                    }
                }
                
                if (canEvolveCount > 0)
                {
                    text += $"✨ Tienes **{canEvolveCount}** mascota(s) lista(s) para evolucionar!\n\n";
                }
                else
                {
                    text += "❌ Ninguna mascota lista para evolucionar aún.\n\n";
                }
                
                if (player.PetInventory != null && player.PetInventory.Count > 0)
                {
                    text += "**TUS MASCOTAS:**\n\n";
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(pet.Species);
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(pet.Species);
                        
                        text += $"{emoji} **{pet.Name}** - Etapa {pet.EvolutionStage}/3\n";
                        
                        if (speciesData?.EvolutionRequirements != null && pet.EvolutionStage < 3)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            var canEvolve = pet.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills);
                            
                            if (canEvolve)
                            {
                                text += $"   ✅ LISTA PARA EVOLUCIONAR!\n";
                            }
                            else
                            {
                                text += $"   Necesita: Lv.{pet.GetRequiredLevelForEvolution()} ";
                                text += $"| Bond: {pet.Bond}/{reqs.Bond} ";
                                text += $"| Kills: {pet.TotalKills}/{reqs.Kills}\n";
                            }
                        }
                        else if (pet.EvolutionStage >= 3)
                        {
                            text += $"   🌟 FORMA FINAL\n";
                        }
                        
                        text += "\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null)
                {
                    foreach (var pet in player.PetInventory.Take(8))
                    {
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(pet.Species);
                        if (speciesData?.EvolutionRequirements != null && pet.EvolutionStage < 3)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            if (pet.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills))
                            {
                                rows.Add(new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"⭐ Evolucionar {pet.Name}", $"pets_evolve_{pet.Id}")
                                });
                            }
                        }
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_evolve_{id} - Evolucionar mascota
            if (data.StartsWith("pets_evolve_"))
            {
                var petId = data.Replace("pets_evolve_", "");
                var pet = player.PetInventory?.FirstOrDefault(p => p.Id == petId);
                
                if (pet == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Mascota no encontrada", cancellationToken: ct);
                    return;
                }
                
                var evolved = BotTelegram.RPG.Services.PetDatabase.EvolvePet(pet);
                
                if (evolved)
                {
                    rpgService.SavePlayer(player);
                    var emoji = GetPetEmoji(pet.Species);
                    var message = $"✨ ¡{pet.Name} ha evolucionado a etapa {pet.EvolutionStage}!\n{emoji} Stats mejorados significativamente!";
                    await bot.AnswerCallbackQuery(callbackQuery.Id, message, showAlert: true, cancellationToken: ct);
                }
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No se pudo evolucionar la mascota", cancellationToken: ct);
                }
                
                // Reconstruir menú de evolución con datos actualizados
                var text = "⭐ **EVOLUCIONAR MASCOTAS**\n\n";
                text += "Las mascotas pueden evolucionar 3 veces:\n";
                text += "🌱 Básica → 🌿 Avanzada → 🌳 Definitiva\n\n";
                
                var canEvolveCount = 0;
                if (player.PetInventory != null)
                {
                    foreach (var petCheck in player.PetInventory)
                    {
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(petCheck.Species);
                        if (speciesData?.EvolutionRequirements != null)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            if (petCheck.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills))
                            {
                                canEvolveCount++;
                            }
                        }
                    }
                }
                
                if (canEvolveCount > 0)
                {
                    text += $"✨ Tienes **{canEvolveCount}** mascota(s) lista(s) para evolucionar!\n\n";
                }
                else
                {
                    text += "❌ Ninguna mascota lista para evolucionar aún.\n\n";
                }
                
                if (player.PetInventory != null && player.PetInventory.Count > 0)
                {
                    text += "**TUS MASCOTAS:**\n\n";
                    foreach (var petItem in player.PetInventory.Take(8))
                    {
                        var emoji = GetPetEmoji(petItem.Species);
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(petItem.Species);
                        
                        text += $"{emoji} **{petItem.Name}** - Etapa {petItem.EvolutionStage}/3\n";
                        
                        if (speciesData?.EvolutionRequirements != null && petItem.EvolutionStage < 3)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            var canEvolve = petItem.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills);
                            
                            if (canEvolve)
                            {
                                text += $"   ✅ LISTA PARA EVOLUCIONAR!\n";
                            }
                            else
                            {
                                text += $"   Necesita: Lv.{petItem.GetRequiredLevelForEvolution()} ";
                                text += $"| Bond: {petItem.Bond}/{reqs.Bond} ";
                                text += $"| Kills: {petItem.TotalKills}/{reqs.Kills}\n";
                            }
                        }
                        else if (petItem.EvolutionStage >= 3)
                        {
                            text += $"   🌟 FORMA FINAL\n";
                        }
                        
                        text += "\n";
                    }
                }
                
                var rows = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                
                if (player.PetInventory != null)
                {
                    foreach (var petItem in player.PetInventory.Take(8))
                    {
                        var speciesData = BotTelegram.RPG.Services.PetDatabase.GetSpeciesData(petItem.Species);
                        if (speciesData?.EvolutionRequirements != null && petItem.EvolutionStage < 3)
                        {
                            var reqs = speciesData.EvolutionRequirements;
                            if (petItem.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills))
                            {
                                rows.Add(new[]
                                {
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData($"⭐ Evolucionar {petItem.Name}", $"pets_evolve_{petItem.Id}")
                                });
                            }
                        }
                    }
                }
                
                rows.Add(new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(rows),
                    cancellationToken: ct);
                return;
            }
            
            // pets_guide - Guía del sistema de mascotas
            if (data == "pets_guide")
            {
                var text = "📖 **GUÍA DEL SISTEMA DE MASCOTAS**\n\n";
                text += "**🐾 ¿Cómo domar mascotas?**\n";
                text += "1. Encuentra una **Bestia** en exploración\n";
                text += "2. Reduce su HP por debajo del **50%**\n";
                text += "3. Usa el botón **🐾 Domar** en combate\n";
                text += "4. Chance: 40% + Charisma% + Debilidad%\n\n";
                
                text += "**💖 Sistema de Bond (Lealtad)**\n";
                text += "• 0-199: 💢 Hostile (-30% stats)\n";
                text += "• 200-399: 😐 Neutral (0% bonus)\n";
                text += "• 400-599: 😊 Friendly (+20% stats)\n";
                text += "• 600-799: 💙 Loyal (+50% stats)\n";
                text += "• 800-1000: 💖 Devoted (+100% stats!)\n\n";
                
                text += "**⭐ Evolución (3 etapas)**\n";
                text += "• Etapa 1 → 2: Nivel 15, Bond 400, 50 kills\n";
                text += "• Etapa 2 → 3: Nivel 35, Bond 700, 200 kills\n\n";
                
                text += "**⚔️ En Combate**\n";
                text += "• Tus mascotas atacan después de ti\n";
                text += "• Límite: 1-5 pets según tu clase oculta\n";
                text += "• Tipos: Physical o Magical\n\n";
                
                text += "**🍖 Cuidados**\n";
                text += "• Alimentar: 5 oro (+20 bond, +30% HP)\n";
                text += "• Aumenta bond ganando combates juntos\n";
                text += "• Pierde bond si la mascota muere\n\n";
                
                text += "**🐉 Familias de Mascotas**\n";
                text += "🐺 Caninos (Physical) - Veloces y feroces\n";
                text += "🐻 Osos (Physical) - Tanques resistentes\n";
                text += "🐉 Dragones (Magical) - Poder elemental\n";
                text += "🐱 Felinos (Physical) - Críticos mortales\n";
                text += "🦅 Aves (Physical) - Evasión suprema\n";
                text += "🐍 Reptiles (Magical) - Veneno letal\n";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "pets_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }
            
            // pets_main - Volver al menú principal de pets (ejecutar comando)
            if (data == "pets_main")
            {
                await bot.DeleteMessage(chatId, messageId, ct);
                var petsCommand = new BotTelegram.RPG.Commands.PetsCommand();
                await petsCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
        }
        
        private static string GetPetEmoji(string species)
        {
            if (species.StartsWith("wolf_")) return "🐺";
            if (species.StartsWith("bear_")) return "🐻";
            if (species.StartsWith("dragon_")) return "🐉";
            if (species.StartsWith("cat_") || species.StartsWith("wildcat_")) return "🐱";
            if (species.StartsWith("eagle_")) return "🦅";
            if (species.StartsWith("snake_")) return "🐍";
            return "🐾";
        }
        
        private static string GetProgressBar(double progress, int length)
        {
            var filled = (int)(progress * length);
            var empty = length - filled;
            return new string('█', filled) + new string('░', empty);
        }

        // ==========================================
        // LEADERBOARD HANDLERS
        // ==========================================
        
        private static async Task HandleLeaderboardCallback(
            ITelegramBotClient bot,
            CallbackQuery callbackQuery,
            string data,
            CancellationToken ct)
        {
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;
            var userId = callbackQuery.From.Id;

            // Get required services
            var rpgService = new BotTelegram.RPG.Services.RpgService();
            var leaderboardService = new BotTelegram.RPG.Services.LeaderboardService(rpgService);
            var leaderboardCommand = new BotTelegram.RPG.Commands.LeaderboardCommand();

            // leaderboard_main - Show main dashboard
            if (data == "leaderboard_main")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "📊 Cargando rankings...", cancellationToken: ct);
                
                await leaderboardCommand.ShowMainLeaderboard(bot, chatId, ct);
                await bot.DeleteMessage(chatId, messageId, ct);
                return;
            }

            // leaderboard_level - Top by Level
            if (data == "leaderboard_level")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⭐ Cargando ranking por nivel...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByLevel(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - NIVEL MÁS ALTO", "Nivel");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Level);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_gold - Top by Gold
            if (data == "leaderboard_gold")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💰 Cargando ranking por oro...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByGold(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS ORO", "Oro");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Gold);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_kills - Top by Total Kills
            if (data == "leaderboard_kills")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ Cargando ranking por kills...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByKills(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS KILLS", "Kills");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Kills);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_boss_kills - Top by Boss Kills
            if (data == "leaderboard_boss_kills")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "👹 Cargando ranking por jefes...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByBossKills(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS JEFES DERROTADOS", "Jefes");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.BossKills);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_damage - Top by Total Damage
            if (data == "leaderboard_damage")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "💥 Cargando ranking por daño...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByDamage(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS DAÑO TOTAL", "Daño");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.TotalDamage);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_pets - Top by Pet Count
            if (data == "leaderboard_pets")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🐾 Cargando ranking por mascotas...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopByPets(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS MASCOTAS", "Mascotas");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.PetCount);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_skills - Top by Skills Unlocked
            if (data == "leaderboard_skills")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "✨ Cargando ranking por habilidades...", cancellationToken: ct);
                
                var entries = leaderboardService.GetTopBySkills(10);
                var text = BotTelegram.RPG.Commands.LeaderboardCommand.FormatLeaderboard(entries, "TOP 10 - MÁS HABILIDADES", "Habilidades");
                
                var (rank, total) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.SkillsUnlocked);
                text += $"\n━━━━━━━━━━━━━━━\n📊 **Tu posición:** #{rank} de {total}";
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // leaderboard_my_profile - Show personal profile with all rankings
            if (data == "leaderboard_my_profile")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "👤 Cargando tu perfil...", cancellationToken: ct);
                
                var player = rpgService.GetPlayer(userId);
                if (player == null)
                {
                    await bot.EditMessageText(
                        chatId,
                        messageId,
                        "❌ No tienes un personaje RPG creado.\nUsa `/rpg` para empezar tu aventura.",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: ct);
                    return;
                }

                var classEmoji = player.Class switch
                {
                    CharacterClass.Warrior => "⚔️",
                    CharacterClass.Mage => "🔮",
                    CharacterClass.Ranger => "🏹",
                    CharacterClass.Assassin => "🗡️",
                    _ => "🎭"
                };

                var username = string.IsNullOrEmpty(player.Username) ? player.Name : $"@{player.Username}";

                string text = $"👤 **PERFIL DE {username.ToUpper()}**\n";
                text += $"{classEmoji} {player.Class} - Nivel {player.Level}\n";
                text += $"━━━━━━━━━━━━━━━\n\n";
                text += $"📊 **TUS POSICIONES EN LOS RANKINGS:**\n\n";

                // Get rankings for all categories
                var (rankLevel, totalLevel) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Level);
                var (rankGold, totalGold) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Gold);
                var (rankKills, totalKills) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.Kills);
                var (rankBossKills, totalBossKills) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.BossKills);
                var (rankDamage, totalDamage) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.TotalDamage);
                var (rankPets, totalPets) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.PetCount);
                var (rankSkills, totalSkills) = leaderboardService.GetPlayerRank(userId, BotTelegram.RPG.Models.LeaderboardType.SkillsUnlocked);

                text += $"⭐ **Nivel:** #{rankLevel} de {totalLevel}\n";
                text += $"💰 **Oro:** #{rankGold} de {totalGold}\n";
                text += $"⚔️ **Kills:** #{rankKills} de {totalKills}\n";
                text += $"👹 **Jefes:** #{rankBossKills} de {totalBossKills}\n";
                text += $"💥 **Daño:** #{rankDamage} de {totalDamage}\n";
                text += $"🐾 **Mascotas:** #{rankPets} de {totalPets}\n";
                text += $"✨ **Habilidades:** #{rankSkills} de {totalSkills}\n";

                text += $"\n━━━━━━━━━━━━━━━\n";
                text += $"💎 **Estadísticas Actuales:**\n";
                text += $"❤️ HP: {player.HP}/{player.MaxHP}\n";
                text += $"⚡ Mana: {player.Mana}/{player.MaxMana}\n";
                text += $"💰 Oro: {player.Gold}\n";
                text += $"⚔️ Kills: {player.TotalKills}\n";
                text += $"👹 Jefes: {player.BossKills}\n";
                text += $"💥 Daño Total: {player.TotalDamageDealt}\n";
                text += $"🐾 Mascotas: {player.PetInventory.Count}\n";
                text += $"✨ Habilidades: {player.UnlockedSkills.Count}\n";

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 Volver", "leaderboard_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }
        }
        
        // ═══════════════════════════════════════════════════════════════
        // FASE 3: DUNGEON SYSTEM CALLBACKS
        // ═══════════════════════════════════════════════════════════════
        
        private static async Task HandleDungeonCallback(ITelegramBotClient bot, CallbackQuery callbackQuery, string data, CancellationToken ct)
        {
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;
            var rpgService = new RpgService();
            var player = rpgService.GetPlayer(chatId);
            
            if (player == null)
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Necesitas crear un personaje primero.", cancellationToken: ct);
                return;
            }
            
            var dungeonService = new DungeonService(rpgService);
            var combatService = new RpgCombatService();
            
            // dungeon_enter_{dungeonId}
            if (data.StartsWith("dungeon_enter_"))
            {
                var dungeonId = data.Replace("dungeon_enter_", "");
                
                if (player.IsInCombat || player.CurrentDungeon != null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Ya estás en combate o en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                bool success = dungeonService.StartDungeon(player, dungeonId, consumeKey: true);
                
                if (success)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "🏰 ¡Entrando a la mazmorra!", cancellationToken: ct);
                    
                    // Mostrar progreso de la mazmorra
                    var dungeonCommand = new DungeonCommand();
                    await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                }
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes entrar a esta mazmorra (nivel insuficiente o sin llave).", cancellationToken: ct);
                }
                return;
            }
            
            // dungeon_fight_{floor}
            if (data.StartsWith("dungeon_fight_"))
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                var currentFloor = player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1];
                
                if (currentFloor.Enemy == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No hay enemigos en este piso.", cancellationToken: ct);
                    return;
                }
                
                // Iniciar combate con el enemigo del piso
                player.IsInCombat = true;
                player.CurrentEnemy = currentFloor.Enemy;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.ActiveCombatMessageId = messageId;
                StateManager.TransitionTo(player, GameState.InDungeonCombat, currentFloor.Enemy.Name); // FSM
                
                rpgService.SavePlayer(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "⚔️ ¡Combate iniciado!", cancellationToken: ct);
                
                // Mostrar vista de combate - redirigir al comando rpg que maneja el combate
                var rpgCommand = new RpgCommand();
                await rpgCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
            
            // dungeon_rest_{floor}
            if (data.StartsWith("dungeon_rest_"))
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                var currentFloor = player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1];
                
                if (currentFloor.Type != FloorType.Rest)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Este no es un piso de descanso.", cancellationToken: ct);
                    return;
                }
                
                // Restaurar HP/Mana/Stamina
                var restorePercent = currentFloor.RestorePercentage / 100.0;
                var hpRestored = (int)(player.MaxHP * restorePercent) - player.HP;
                var manaRestored = (int)(player.MaxMana * restorePercent) - player.Mana;
                var staminaRestored = (int)(player.MaxStamina * restorePercent) - player.Stamina;
                
                player.HP = Math.Min(player.MaxHP, (int)(player.MaxHP * (1 - restorePercent / 100.0)) + player.HP);
                player.Mana = Math.Min(player.MaxMana, (int)(player.MaxMana * (1 - restorePercent / 100.0)) + player.Mana);
                player.Stamina = Math.Min(player.MaxStamina, (int)(player.MaxStamina * (1 - restorePercent / 100.0)) + player.Stamina);
                
                // Marcar piso como completado
                currentFloor.IsCleared = true;
                
                rpgService.SavePlayer(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, $"😴 Descansaste. +{hpRestored} HP, +{manaRestored} Mana, +{staminaRestored} Stamina", cancellationToken: ct);
                
                // Mostrar progreso actualizado
                var dungeonCommand = new DungeonCommand();
                await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
            
            // dungeon_avoid_trap_{floor} o dungeon_face_trap_{floor}
            if (data.StartsWith("dungeon_avoid_trap_") || data.StartsWith("dungeon_face_trap_"))
            {
                bool tryAvoid = data.StartsWith("dungeon_avoid_trap_");
                
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                var currentFloor = player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1];
                
                if (currentFloor.Type != FloorType.Trap || currentFloor.Trap == null)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No hay trampa en este piso.", cancellationToken: ct);
                    return;
                }
                
                var trap = currentFloor.Trap;
                bool avoided = false;
                var random = new Random();
                
                if (tryAvoid && trap.CanAvoid)
                {
                    // DEX check
                    var dexBonus = player.Dexterity / 2;
                    var roll = random.Next(1, 21) + dexBonus;
                    avoided = roll >= trap.AvoidDC;
                }
                
                string resultText;
                
                if (avoided)
                {
                    resultText = $"✅ ¡Esquivaste la trampa! ({trap.Name})";
                    
                    // Posible recompensa
                    if (trap.SuccessReward != null)
                    {
                        player.Gold += trap.SuccessReward.Gold;
                        resultText += $"\n💰 +{trap.SuccessReward.Gold} oro";
                    }
                }
                else
                {
                    resultText = $"💥 {trap.Name}!";
                    
                    if (trap.Damage > 0)
                    {
                        player.HP = Math.Max(1, player.HP - trap.Damage);
                        resultText += $"\n💔 -{trap.Damage} HP";
                    }
                    
                    if (trap.ManaDrain > 0)
                    {
                        player.Mana = Math.Max(0, player.Mana - trap.ManaDrain);
                        resultText += $"\n💙 -{trap.ManaDrain} Mana";
                    }
                    
                    if (trap.StaminaDrain > 0)
                    {
                        player.Stamina = Math.Max(0, player.Stamina - trap.StaminaDrain);
                        resultText += $"\n💛 -{trap.StaminaDrain} Stamina";
                    }
                    
                    if (trap.GoldLoss > 0)
                    {
                        player.Gold = Math.Max(0, player.Gold - trap.GoldLoss);
                        resultText += $"\n💰 -{trap.GoldLoss} oro";
                    }
                    
                    // Marcar perfect run como false si recibe daño
                    if (trap.Damage > 0)
                    {
                        player.CurrentDungeon.IsPerfectRun = false;
                    }
                }
                
                // Marcar piso como completado
                currentFloor.IsCleared = true;
                
                rpgService.SavePlayer(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, resultText, cancellationToken: ct);
                
                // Mostrar progreso actualizado
                var dungeonCommand = new DungeonCommand();
                await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
            
            // dungeon_next_floor
            if (data == "dungeon_next_floor")
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                bool advanced = dungeonService.AdvanceToNextFloor(player);
                
                if (advanced)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, $"➡️ Avanzaste al piso {player.CurrentDungeon.CurrentFloor}", cancellationToken: ct);
                    
                    // Mostrar nuevo piso
                    var dungeonCommand = new DungeonCommand();
                    await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                }
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes avanzar (piso no completado o es el último).", cancellationToken: ct);
                }
                return;
            }
            
            // dungeon_complete
            if (data == "dungeon_complete")
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                var dungeon = player.CurrentDungeon;
                
                // Verificar que todos los pisos están completados
                if (dungeon.CurrentFloor < dungeon.TotalFloors || !dungeon.Floors.All(f => f.IsCleared))
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Aún quedan pisos por completar.", cancellationToken: ct);
                    return;
                }
                
                dungeonService.CompleteDungeon(player);
                
                var text = new System.Text.StringBuilder();
                text.AppendLine("🏆 **¡MAZMORRA COMPLETADA!**");
                text.AppendLine($"{dungeon.Emoji} **{dungeon.Name}**");
                text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
                text.AppendLine();
                text.AppendLine("📊 **ESTADÍSTICAS:**");
                text.AppendLine($"⚔️ Enemigos derrotados: {dungeon.TotalEnemiesDefeated}");
                text.AppendLine($"💀 Jefes vencidos: {dungeon.TotalBossesDefeated}");
                text.AppendLine($"📍 Pisos completados: {dungeon.TotalFloors}/{dungeon.TotalFloors}");
                text.AppendLine($"⭐ Puntuación: {(dungeon.IsPerfectRun ? "S (Perfect Run!)" : "A")}");
                text.AppendLine();
                text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
                text.AppendLine("🎁 **RECOMPENSAS:**");
                text.AppendLine();
                text.AppendLine($"💰 Oro: {dungeon.FinalRewards.Gold}");
                text.AppendLine($"✨ XP: {dungeon.FinalRewards.XP}");
                
                if (dungeon.IsPerfectRun)
                {
                    var bonusXP = (int)(dungeon.FinalRewards.XP * dungeon.FinalRewards.PerfectionBonus);
                    text.AppendLine($"⭐ **Perfect Run Bonus:** +{bonusXP} XP ({(int)(dungeon.FinalRewards.PerfectionBonus * 100)}%)");
                }
                
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏰 Mazmorras", "dungeon_main"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔙 RPG", "rpg_main")
                    }
                });
                
                await bot.EditMessageText(
                    chatId,
                    messageId,
                    text.ToString(),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                    
                await bot.AnswerCallbackQuery(callbackQuery.Id, "🏆 ¡Mazmorra completada!", cancellationToken: ct);
                return;
            }
            
            // dungeon_abandon
            if (data == "dungeon_abandon")
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }
                
                dungeonService.AbandonDungeon(player);
                
                await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Abandonaste la mazmorra. Perdiste todo el progreso.", cancellationToken: ct);
                
                // Volver al menú de mazmorras
                var dungeonCommand = new DungeonCommand();
                await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                return;
            }
            
            // dungeon_advance_{floor} — alias del botón "⏭️ Continuar" entre pisos
            if (data.StartsWith("dungeon_advance_"))
            {
                if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No estás en una mazmorra.", cancellationToken: ct);
                    return;
                }

                bool advanced = dungeonService.AdvanceToNextFloor(player);

                if (advanced)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, $"➡️ Avanzaste al piso {player.CurrentDungeon.CurrentFloor}", cancellationToken: ct);
                    var dungeonCmd = new DungeonCommand();
                    await dungeonCmd.Execute(bot, callbackQuery.Message, ct);
                }
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes avanzar (piso no completado o es el último).", cancellationToken: ct);
                }
                return;
            }

            // dungeon_main o callbacks de navegación
            if (data == "dungeon_main" || data == "dungeon_keys" || data == "dungeon_rankings")
            {
                var dungeonCommand = new DungeonCommand();
                await dungeonCommand.Execute(bot, callbackQuery.Message, ct);
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                return;
            }
        }
        
        // ═══════════════════════════════════════════════════════════════
        // FASE 4: CLASS SYSTEM CALLBACKS
        // ═══════════════════════════════════════════════════════════════
        
        private static async Task HandleClassCallback(ITelegramBotClient bot, CallbackQuery callbackQuery, string data, CancellationToken ct)
        {
            var chatId = callbackQuery.Message!.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;
            var rpgService = new RpgService();
            var player = rpgService.GetPlayer(chatId);
            
            if (player == null)
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ Necesitas crear un personaje primero.", cancellationToken: ct);
                return;
            }
            
            var tracker = new ActionTrackerService(rpgService);
            
            // classes_menu: Mostrar menú principal de clases
            if (data == "classes_menu")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await ClassesCommand.ShowClassMenu(bot, chatId, player, ct, messageId);
                return;
            }
            
            // class_progress: Ver progreso detallado de desbloqueo
            if (data == "class_progress")
            {
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                await ClassesCommand.ShowClassProgress(bot, chatId, player, ct, messageId);
                return;
            }
            
            // class_equip_{classId}: Equipar una clase desbloqueada
            if (data.StartsWith("class_equip_"))
            {
                var classId = data.Replace("class_equip_", "");
                
                if (player.ActiveClassId == classId)
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "✅ Ya tienes esta clase activa.", cancellationToken: ct);
                    return;
                }
                
                bool success = tracker.EquipClass(player, classId);
                
                if (success)
                {
                    var def = ClassUnlockDatabase.GetAllClassDefinitions()
                        .FirstOrDefault(d => d.ClassId == classId);
                    
                    var className = def?.Name ?? classId;
                    var classEmoji = def?.Emoji ?? "🎭";
                    
                    await bot.AnswerCallbackQuery(callbackQuery.Id, $"{classEmoji} ¡Clase {className} equipada!", cancellationToken: ct);
                    await ClassesCommand.ShowClassMenu(bot, chatId, player, ct, messageId);
                }
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "❌ No puedes equipar esta clase (no desbloqueada).", cancellationToken: ct);
                }
                return;
            }
            
            // ═══ FASE 11: PVP CALLBACKS ══════════════════════════════════════
            if (data.StartsWith("pvp_"))
            {
                await BotTelegram.RPG.Commands.PvpCommand.HandleCallback(bot, callbackQuery, data, ct);
                return;
            }

            // ═══ FASE 10: GUILD CALLBACKS ════════════════════════════════════
            if (data.StartsWith("guild_"))
            {
                await BotTelegram.RPG.Commands.GuildCommand.HandleCallback(bot, callbackQuery, data, ct);
                return;
            }

            await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
        }
    }
}
