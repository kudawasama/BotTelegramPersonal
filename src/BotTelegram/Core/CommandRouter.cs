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

            if (message.Text.StartsWith("/herreria") || message.Text.StartsWith("/craft") || message.Text.StartsWith("/forge"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando CraftingCommand (Sistema de Crafteo)");
                await new CraftingCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/misiones") || message.Text.StartsWith("/quests"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando QuestCommand (Sistema de Misiones)");
                await new QuestCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/arena") || message.Text.StartsWith("/pvp"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando PvpCommand (Arena PvP)");
                await new BotTelegram.RPG.Commands.PvpCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/gremio") || message.Text.StartsWith("/guild"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando GuildCommand (Sistema de Gremio)");
                var guildPlayer = new BotTelegram.RPG.Services.RpgService().GetPlayer(message.Chat.Id);
                if (guildPlayer is null)
                {
                    await bot.SendMessage(message.Chat.Id, "❌ Aún no tienes perfil RPG. Usa /rpg para comenzar.", cancellationToken: System.Threading.CancellationToken.None);
                    return;
                }

                // Subcomando: /gremio crear ...
                if (message.Text.Contains("crear", StringComparison.OrdinalIgnoreCase))
                {
                    await BotTelegram.RPG.Commands.GuildCommand.HandleCreateCommand(bot, message, guildPlayer, ct);
                    return;
                }
                await new BotTelegram.RPG.Commands.GuildCommand().Execute(bot, message, ct);
                return;
            }

            if (message.Text.StartsWith("/facciones") || message.Text.StartsWith("/faction"))
            {
                Console.WriteLine("   [CommandRouter] → Ejecutando FactionCommand (Sistema de Facciones)");
                await new BotTelegram.RPG.Commands.FactionCommand().Execute(bot, message, ct);
                return;
            }

            Console.WriteLine("   [CommandRouter] → Ejecutando UnknownCommand");
            await new UnknownCommand().Execute(bot, message, ct);
        }
    }
}


