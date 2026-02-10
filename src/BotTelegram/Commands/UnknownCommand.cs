using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;

namespace BotTelegram.Commands
{
    public class UnknownCommand
    {
        public async Task Execute(
            ITelegramBotClient client,
            Message message,
            CancellationToken ct)
        {
            await client.SendMessage(
                chatId: message.Chat.Id,
                text: "❓ Comando no reconocido. Usa /help.",
                cancellationToken: ct
            );
        }
    }
}
