using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Commands;
using BotTelegram.Models;

namespace BotTelegram.Core
{
    public static class CommandRouter
    {
        public static async Task Route(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            if (message.Text == null)
                return;

            Console.WriteLine($"   [CommandRouter] Comando recibido: {message.Text}");

            if (message.Text.StartsWith("/start"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando StartCommand");
                await new StartCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/help"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando HelpCommand");
                await new HelpCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/remember"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando RememberCommand");
                await new RememberCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/list"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando ListCommand");
                await new ListCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/delete"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando DeleteCommand");
                await new DeleteCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/edit"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando EditCommand");
                await new EditCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/recur"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando RecurCommand");
                await new RecurCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/faq"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando FaqCommand");
                await FaqCommand.Execute(bot, message, ct);
                return;
            }

            Console.WriteLine("   [CommandRouter] → Ejecutando UnknownCommand");
            await new UnknownCommand().Execute(bot, message, ct);
        }
    }
}


