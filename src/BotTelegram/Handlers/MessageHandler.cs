using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.Services;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Commands;

namespace BotTelegram.Handlers
{
    public static class MessageHandler
    {
        public static async Task Handle(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            // Null check crítico: evita crashes con fotos/stickers/videos
            if (message.Text == null)
            {
                // ═════════════════════════════════════════════════════════════
                // MANEJADOR DE ARCHIVOS - IMPORTAR PERSONAJE RPG
                // ═════════════════════════════════════════════════════════════
                
                if (message.Document != null && RpgService.IsAwaitingImport(message.Chat.Id))
                {
                    // El usuario está esperando enviar un archivo de importación
                    try
                    {
                        // Validar que sea un archivo JSON
                        if (!message.Document.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            await bot.SendMessage(
                                message.Chat.Id,
                                "❌ **Error:** Solo se aceptan archivos **.json**\n\n" +
                                "Por favor envía el archivo de backup de tu personaje (debe terminar en .json)",
                                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                cancellationToken: ct);
                            return;
                        }
                        
                        // Descargar el archivo
                        var file = await bot.GetFile(message.Document.FileId, cancellationToken: ct);
                        
                        if (file.FilePath == null)
                        {
                            throw new Exception("No se pudo obtener la ruta del archivo");
                        }
                        
                        var fileStream = new System.IO.MemoryStream();
                        await bot.DownloadFile(file.FilePath, fileStream, cancellationToken: ct);
                        
                        // Leer contenido JSON
                        fileStream.Seek(0, System.IO.SeekOrigin.Begin);
                        using var reader = new System.IO.StreamReader(fileStream);
                        var json = await reader.ReadToEndAsync();
                        
                        // Intentar importar personaje
                        var rpgService = new RpgService();
                        
                        // Validar que sea JSON válido
                        if (!rpgService.ValidatePlayerJson(json))
                        {
                            throw new Exception("El archivo JSON no tiene el formato correcto");
                        }
                        
                        // Importar personaje
                        var importedPlayer = rpgService.ImportPlayerData(json, message.Chat.Id);
                        
                        if (importedPlayer == null)
                        {
                            throw new Exception("No se pudo procesar el personaje");
                        }
                        
                        // Desactivar estado de espera
                        RpgService.SetAwaitingImport(message.Chat.Id, false);
                        
                        // Enviar confirmación
                        await bot.SendMessage(
                            message.Chat.Id,
                            $"✅ **PERSONAJE IMPORTADO EXITOSAMENTE**\n\n" +
                            $"👤 **{importedPlayer.Name}** - {importedPlayer.Class} Lv.{importedPlayer.Level}\n" +
                            $"💰 Oro: {importedPlayer.Gold}\n" +
                            $"⭐ XP: {importedPlayer.XP}\n" +
                            $"📍 Ubicación: {importedPlayer.CurrentLocation}\n\n" +
                            $"✨ *Tu personaje ha sido restaurado exitosamente.*\n" +
                            $"💡 *Usa `/rpg` para continuar tu aventura.*",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            cancellationToken: ct);
                        
                        Console.WriteLine($"[MessageHandler] ✅ Personaje importado para ChatId {message.Chat.Id}: {importedPlayer.Name}");
                        TelegramLogger.LogUserAction(
                            message.Chat.Id,
                            message.Chat.Username ?? "Unknown",
                            "character_import",
                            $"Imported character '{importedPlayer.Name}' (Lv.{importedPlayer.Level} {importedPlayer.Class})");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[MessageHandler] ❌ Error al importar personaje: {ex.Message}");
                        TelegramLogger.LogError(
                            message.Chat.Id,
                            "character_import_failed",
                            $"Failed to import character from JSON file",
                            ex);
                        
                        await bot.SendMessage(
                            message.Chat.Id,
                            $"❌ **Error al importar personaje:**\n\n" +
                            $"{ex.Message}\n\n" +
                            $"💡 *Asegúrate de que:*\n" +
                            $"• El archivo sea un JSON válido\n" +
                            $"• Fue exportado desde este bot\n" +
                            $"• No ha sido modificado\n\n" +
                            $"⚙️ Usa `/rpg` para volver al menú",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            cancellationToken: ct);
                        
                        RpgService.SetAwaitingImport(message.Chat.Id, false);
                        return;
                    }
                }
                
                // Si no es importación de personaje, ignorar otros archivos
                Console.WriteLine($"   [MessageHandler] ⚠️ Mensaje sin texto (media) de ChatId {message.Chat.Id}");
                return;
            }

            Console.WriteLine($"   [MessageHandler] Mensaje de ChatId {message.Chat.Id}: {message.Text}");

            // Captura de nombre de personaje RPG
            if (RpgService.IsAwaitingName(message.Chat.Id) && !message.Text.StartsWith("/"))
            {
                var name = message.Text.Trim();
                
                // Validar nombre
                if (name.Length < 3 || name.Length > 20)
                {
                    await bot.SendMessage(
                        message.Chat.Id,
                        "❌ El nombre debe tener entre 3 y 20 caracteres. Intenta de nuevo:",
                        cancellationToken: ct);
                    return;
                }
                
                // Desactivar estado de espera
                RpgService.SetAwaitingName(message.Chat.Id, false);
                
                TelegramLogger.LogUserAction(
                    message.Chat.Id,
                    message.Chat.Username ?? "Unknown",
                    "character_name_selected",
                    $"Created new character with name '{name}'");
                
                // Mostrar selección de clase
                var rpgCommand = new RpgCommand();
                await rpgCommand.ShowClassSelection(bot, message.Chat.Id, name, ct);
                return;
            }

            // Chat con contexto RPG
            if (AIService.IsRpgChatMode(message.Chat.Id) && !message.Text.StartsWith("/"))
            {
                var rpgService = new RpgService();
                var player = rpgService.GetPlayer(message.Chat.Id);
                
                if (player == null)
                {
                    // Si no tiene personaje, desactivar modo RPG
                    AIService.SetRpgChatMode(message.Chat.Id, false);
                    await bot.SendMessage(
                        message.Chat.Id,
                        "❌ No tienes un personaje creado. Modo RPG desactivado.",
                        cancellationToken: ct);
                    return;
                }
                
                var aiService = new AIService();

                await bot.SendChatAction(
                    message.Chat.Id,
                    Telegram.Bot.Types.Enums.ChatAction.Typing,
                    cancellationToken: ct);

                var response = await aiService.ChatWithRpgContext(
                    message.Chat.Id, 
                    message.Text,
                    player.Name,
                    player.Class.ToString(),
                    player.Level,
                    player.CurrentLocation);

                TelegramLogger.LogRpgEvent(
                    message.Chat.Id,
                    player.Name,
                    "chat_interaction",
                    $"RPG chat: {message.Text.Substring(0, Math.Min(50, message.Text.Length))}...");

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🎮 Menu RPG", "rpg_main"),
                        InlineKeyboardButton.WithCallbackData("🚪 Salir", "exit_chat")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                    }
                });

                // Visual feedback: indicador mágico para modo RPG
                await bot.SendMessage(
                    message.Chat.Id,
                    $"🎲 *Narrador RPG* ({player.Name} - {player.Class})\n\n{response}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // Chat normal (sin contexto RPG)
            if (AIService.IsChatMode(message.Chat.Id) && !message.Text.StartsWith("/"))
            {
                var aiService = new AIService();

                await bot.SendChatAction(
                    message.Chat.Id,
                    Telegram.Bot.Types.Enums.ChatAction.Typing,
                    cancellationToken: ct);

                var response = await aiService.Chat(message.Chat.Id, message.Text);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🔄 Reiniciar chat", "clear_chat"),
                        InlineKeyboardButton.WithCallbackData("🚪 Salir del chat", "exit_chat")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏠 Menú", "start")
                    }
                });

                // Visual feedback: indicador verde para modo chat activo
                await bot.SendMessage(
                    message.Chat.Id,
                    $"🟢 *Modo Chat*\n\n{response}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
                return;
            }

            // Comandos normales
            await CommandHandler.Handle(bot, message, ct);
        }
    }
}

    