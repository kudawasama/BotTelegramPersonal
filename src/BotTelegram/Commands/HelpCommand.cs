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
- mañana a las 09:00",
                cancellationToken: ct
            );
        }
    }
}

