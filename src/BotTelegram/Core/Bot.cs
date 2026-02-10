using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using BotTelegram.Services;

namespace BotTelegram.Core
{
    public class Bot
    {
        private readonly ITelegramBotClient _client;

        public Bot(string token)
        {
            _client = new TelegramBotClient(token);
        }

        public void Start()
        {
            Console.WriteLine("🤖 Bot de Telegram iniciado...");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<Telegram.Bot.Types.Enums.UpdateType>()
            };

            _client.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions
            );

            var scheduler = new ReminderScheduler(_client);
            scheduler.Start();


            Console.WriteLine("Presiona ENTER para detener el bot");
            Console.ReadLine();
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient bot,
            Update update,
            CancellationToken ct)
        {
            if (update.Message is not Message message)
                return;

            if (string.IsNullOrWhiteSpace(message.Text))
                return;

            // � LOG DEL CHATID
            Console.WriteLine($"📱 [MESSAGE RECIBIDO] ChatId: {message.Chat.Id} | Texto: {message.Text}");

            // �🔁 Enrutamos el mensaje al CommandRouter
            await CommandRouter.Route(bot, message, ct);
        }

        private Task HandleErrorAsync(
            ITelegramBotClient bot,
            Exception exception,
            CancellationToken ct)
        {
            Console.WriteLine($"❌ Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
