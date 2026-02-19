using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;
using BotTelegram.Services;
using BotTelegram.RPG.Services;

namespace BotTelegram.Commands
{
    public class StartCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            Console.WriteLine($"   [StartCommand] Enviando mensaje de bienvenida");
            
            // 🔄 MIGRACIÓN: Actualizar personaje existente con nuevas propiedades
            var migrationService = new PlayerMigrationService();
            bool playerUpdated = migrationService.MigratePlayer(message.Chat.Id);
            
            if (playerUpdated)
            {
                Console.WriteLine($"   [StartCommand] ⬆️ Personaje actualizado a la última versión");
            }
            
            // 🎯 LOG: Registrar comando /start
            TelegramLogger.LogUserAction(
                chatId: message.Chat.Id,
                username: message.From?.Username ?? "unknown",
                action: "/start",
                details: playerUpdated ? "Menu principal + personaje actualizado" : "Menu principal desplegado"
            );
            
            // Menú principal reorganizado por categorías
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🎮 JUEGO RPG", "menu_ai")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("ℹ️ AYUDA E INFORMACIÓN", "menu_info")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 *¡Bienvenido al Bot RPG con IA!*\n\n" +
                      "✨ Tu aventura épica comienza aquí:\n" +
                      "• Juego RPG inmersivo con combate por turnos\n" +
                      "• Chat con IA avanzada\n" +
                      "• Sistema de mascotas y habilidades\n" +
                      "• Rankings globales y competencia\n\n" +
                      (playerUpdated ? "🔄 *Tu personaje ha sido actualizado a la última versión*\n\n" : "") +
                      "🎯 *Selecciona una categoría:*\n\n" +
                      $"━━━━━━━━━━━━━━━\n{BuildInfo.GetVersionBlock()}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );
            Console.WriteLine($"   [StartCommand] ✅ Mensaje enviado");
        }
    }
}

