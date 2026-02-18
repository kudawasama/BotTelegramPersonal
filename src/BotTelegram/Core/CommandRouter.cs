using Telegram.Bot;
using Telegram.Bot.Types;
using BotTelegram.Commands;
using BotTelegram.RPG.Commands;
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

            if (message.Text.StartsWith("/faq"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando FaqCommand");
                await FaqCommand.Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/chat"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando ChatCommand (IA)");
                await new ChatCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/rpg"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando RpgCommand (Juego RPG)");
                await new RpgCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/pets"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando PetsCommand (Gestión de Mascotas)");
                await new PetsCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/companions") || message.Text.StartsWith("/compañeros"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando CompanionsCommand (Todos los Compañeros)");
                await new CompanionsCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/dungeon") || message.Text.StartsWith("/mazmorra"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando DungeonCommand (Sistema de Mazmorras)");
                await new DungeonCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/clases") || message.Text.StartsWith("/classes") || message.Text.StartsWith("/clase"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando ClassesCommand (Sistema de Clases)");
                await new ClassesCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/rpgstats") || message.Text.StartsWith("/stats"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando RpgStatsCommand (Estadísticas Detalladas)");
                await new RpgStatsCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/leaderboard") || 
                message.Text.StartsWith("/rankings") || 
                message.Text.StartsWith("/top"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando LeaderboardCommand (Rankings Globales)");
                await new LeaderboardCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/map") || message.Text.StartsWith("/mapa"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando MapCommand (Mapa del Mundo)");
                await new MapCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/travel") || message.Text.StartsWith("/viajar"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando TravelCommand (Viajar entre Zonas)");
                await new TravelCommand().Execute(bot, message, ct);
                return;
            }

            Console.WriteLine("   [CommandRouter] → Ejecutando UnknownCommand");
            await new UnknownCommand().Execute(bot, message, ct);
        }
    }
}


