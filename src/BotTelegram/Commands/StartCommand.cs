using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;

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
            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 ¡Bienvenido! Usa /help para ver los comandos.",
                cancellationToken: ct
            );
            Console.WriteLine($"   [StartCommand] ✅ Mensaje enviado");
        }
    }
}

