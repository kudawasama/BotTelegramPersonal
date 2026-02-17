using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;
using BotTelegram.Services;

namespace BotTelegram.Commands
{
    public class HelpCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            // 🎯 LOG: Registrar comando /help
            TelegramLogger.LogUserAction(
                chatId: message.Chat.Id,
                username: message.From?.Username ?? "unknown",
                action: "/help",
                details: "Menu de ayuda solicitado"
            );
            
            // Crear botones con acciones principales
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🎮 Juego RPG", "rpg_main"),
                    InlineKeyboardButton.WithCallbackData("💬 Chat IA", "rpg_ai_chat")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏆 Rankings", "leaderboard_main"),
                    InlineKeyboardButton.WithCallbackData("🐾 Mascotas", "pets_main")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });

            await client.SendMessage(
                chatId: message.Chat.Id,
                text:
@"📚 *AYUDA - Bot RPG con IA*

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

*🎯 Click en los botones abajo para acceder rápidamente*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct
            );
        }
    }
}

