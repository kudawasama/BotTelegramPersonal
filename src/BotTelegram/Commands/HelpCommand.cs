using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;

namespace BotTelegram.Commands
{
    public class HelpCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            await client.SendMessage(
                chatId: message.Chat.Id,
                text:
@"📌 Comandos disponibles:
/start - Iniciar el bot
/help - Ver comandos
/remember <texto> - Crear recordatorio en lenguaje natural (ej: /remember Tomar agua en 10 min)
/list - Listar recordatorios pendientes",
                cancellationToken: ct
            );
        }
    }
}

