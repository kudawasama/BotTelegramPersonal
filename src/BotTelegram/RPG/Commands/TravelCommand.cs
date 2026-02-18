using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    public class TravelCommand
    {
        private readonly RpgService _rpgService;
        private readonly ExplorationService _explorationService;
        
        public TravelCommand()
        {
            _rpgService = new RpgService();
            _explorationService = new ExplorationService(_rpgService);
        }
        
        public async Task Execute(
            ITelegramBotClient bot,
            Message message,
            CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var player = _rpgService.GetPlayer(chatId);
            
            if (player == null)
            {
                await bot.SendMessage(
                    chatId,
                    "❌ No tienes un personaje creado. Usa /rpg para comenzar.",
                    cancellationToken: ct);
                return;
            }
            
            // Obtener ID de zona del comando
            var args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (args == null || args.Length < 2)
            {
                await ShowZoneList(bot, chatId, player, ct);
                return;
            }
            
            var targetZoneId = args[1].ToLower().Trim();
            
            // Intentar viajar
            await ExecuteTravel(bot, chatId, player, targetZoneId, ct);
        }
        
        public async Task ExecuteTravelById(
            ITelegramBotClient bot,
            long chatId,
            RpgPlayer player,
            string zoneId,
            CancellationToken ct)
        {
            await ExecuteTravel(bot, chatId, player, zoneId, ct);
        }
        
        private async Task ExecuteTravel(
            ITelegramBotClient bot,
            long chatId,
            RpgPlayer player,
            string targetZoneId,
            CancellationToken ct)
        {
            var result = _explorationService.Travel(player, targetZoneId);
            
            var text = "";
            
            if (result.Success && result.Zone != null)
            {
                // Viaje exitoso
                text += $"🗺️ **VIAJE EXITOSO**\n\n";
                text += $"✨ Has llegado a: **{result.Zone.Name}** {result.Zone.Emoji}\n\n";
                text += $"📖 {result.Zone.Description}\n\n";
                
                if (result.Region != null)
                {
                    text += $"🌍 **Región:** {result.Region.Name} {result.Region.Emoji}\n";
                    text += $"📊 Niveles recomendados: {result.Region.MinLevel}-{result.Region.MaxLevel}\n\n";
                }
                
                text += $"⚔️ **Información de zona:**\n";
                text += $"👾 Nivel de enemigos: {result.Zone.MinEnemyLevel}-{result.Zone.MaxEnemyLevel}\n";
                text += $"📈 Tasa de encuentro: {result.Zone.EncounterRate * 100:F0}%\n";
                text += $"{(result.Zone.IsSafeZone ? "🏘️ Zona segura" : "⚠️ Zona peligrosa")}\n\n";
                
                text += $"━━━━━━━━━━━━━━━━━━━━━━\n";
                text += $"💡 Usa /explore para buscar aventuras\n";
                text += $"🗺️ Usa /map para ver el mapa";
                
                // Botones
                var buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("🔍 Explorar", "rpg_explore"),
                        InlineKeyboardButton.WithCallbackData("🗺️ Ver mapa", "rpg_map")
                    },
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
                    }
                };
                
                var keyboard = new InlineKeyboardMarkup(buttons);
                
                await bot.SendMessage(
                    chatId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            else
            {
                // Viaje fallido
                text += $"❌ **NO PUEDES VIAJAR**\n\n";
                text += result.Message;
                
                // Botones
                var buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("🗺️ Ver mapa", "rpg_map"),
                        InlineKeyboardButton.WithCallbackData("📊 Ver stats", "rpg_stats")
                    },
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
                    }
                };
                
                var keyboard = new InlineKeyboardMarkup(buttons);
                
                await bot.SendMessage(
                    chatId,
                    text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: ct);
            }
        }
        
        private async Task ShowZoneList(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var currentZone = RegionDatabase.GetZone(player.CurrentZone);
            
            var text = $"🗺️ **VIAJE ENTRE ZONAS**\n\n";
            text += $"📍 Ubicación actual: {currentZone?.Name ?? "Desconocida"}\n\n";
            text += $"🧭 **Zonas disponibles:**\n\n";
            
            var connectedZones = RegionDatabase.GetConnectedZones(player.CurrentZone);
            var buttons = new List<List<InlineKeyboardButton>>();
            
            if (connectedZones.Count == 0)
            {
                text += "❌ No hay zonas conectadas disponibles.\n\n";
            }
            else
            {
                foreach (var zone in connectedZones)
                {
                    var isUnlocked = player.UnlockedZones.Contains(zone.Id);
                    var meetsLevel = player.Level >= zone.LevelRequirement;
                    
                    if (isUnlocked)
                    {
                        var status = meetsLevel ? "✅" : "⚠️";
                        text += $"{status} {zone.Emoji} **{zone.Name}** (Lv.{zone.MinEnemyLevel}-{zone.MaxEnemyLevel})\n";
                        text += $"   ID: `{zone.Id}`\n";
                        
                        if (!meetsLevel)
                        {
                            text += $"   ⚠️ Requiere nivel {zone.LevelRequirement}\n";
                        }
                        
                        text += "\n";
                        
                        // Agregar botón solo si puede viajar
                        if (meetsLevel)
                        {
                            buttons.Add(new List<InlineKeyboardButton>
                            {
                                InlineKeyboardButton.WithCallbackData(
                                    $"{zone.Emoji} Viajar a {zone.Name}",
                                    $"rpg_travel_{zone.Id}")
                            });
                        }
                    }
                    else
                    {
                        text += $"🔒 **???** - Zona bloqueada\n\n";
                    }
                }
            }
            
            text += $"━━━━━━━━━━━━━━━━━━━━━━\n";
            text += $"💡 **Uso:**\n";
            text += $"• `/travel <id_zona>` - Viajar a una zona\n";
            text += $"• Ejemplo: `/travel bosque_susurros`\n";
            text += $"• O usa los botones de abajo\n";
            
            // Botones de navegación
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🗺️ Ver mapa", "rpg_map"),
                InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
            });
            
            var keyboard = new InlineKeyboardMarkup(buttons);
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
    }
}
