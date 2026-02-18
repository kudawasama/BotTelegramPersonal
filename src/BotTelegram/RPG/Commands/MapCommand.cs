using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    public class MapCommand
    {
        private readonly RpgService _rpgService;
        
        public MapCommand()
        {
            _rpgService = new RpgService();
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
            
            await ShowMap(bot, chatId, player, ct);
        }
        
        private async Task ShowMap(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var currentZone = RegionDatabase.GetZone(player.CurrentZone);
            
            if (currentZone == null)
            {
                await bot.SendMessage(
                    chatId,
                    "❌ Zona actual no encontrada. Contacta a un administrador.",
                    cancellationToken: ct);
                return;
            }
            
            var region = RegionDatabase.GetAllRegions()
                .FirstOrDefault(r => r.ZoneIds.Contains(player.CurrentZone));
            
            var text = $"🗺️ **MAPA DEL MUNDO**\n\n";
            
            // Ubicación actual
            text += $"📍 **Ubicación Actual:**\n";
            text += $"{currentZone.Emoji} **{currentZone.Name}**\n";
            text += $"📖 {currentZone.Description}\n\n";
            
            if (region != null)
            {
                text += $"🌍 **Región:** {region.Emoji} {region.Name}\n";
                text += $"📊 Niveles: {region.MinLevel}-{region.MaxLevel}\n\n";
            }
            
            // Información de zona actual
            text += $"⚔️ Nivel de enemigos: {currentZone.MinEnemyLevel}-{currentZone.MaxEnemyLevel}\n";
            text += $"📈 Tasa de encuentro: {currentZone.EncounterRate * 100:F0}%\n";
            text += $"{(currentZone.IsSafeZone ? "🏘️ Zona segura" : "⚠️ Zona peligrosa")}\n";
            text += $"━━━━━━━━━━━━━━━━━━━━━━\n\n";
            
            // Zonas conectadas
            var connectedZones = RegionDatabase.GetConnectedZones(player.CurrentZone);
            
            if (connectedZones.Count > 0)
            {
                text += $"🧭 **Zonas Conectadas:**\n";
                
                foreach (var zone in connectedZones)
                {
                    var isUnlocked = player.UnlockedZones.Contains(zone.Id);
                    var meetsLevel = player.Level >= zone.LevelRequirement;
                    
                    if (isUnlocked)
                    {
                        var status = meetsLevel ? "✅" : "⚠️";
                        text += $"{status} {zone.Emoji} **{zone.Name}** (Lv.{zone.MinEnemyLevel}-{zone.MaxEnemyLevel})";
                        
                        if (!meetsLevel)
                        {
                            text += $" - Requiere Lv.{zone.LevelRequirement}";
                        }
                        
                        text += "\n";
                    }
                    else
                    {
                        text += $"🔒 **???** (Zona bloqueada - Explora para descubrir)\n";
                    }
                }
                
                text += "\n";
            }
            
            // Zonas desbloqueadas en región actual
            if (region != null)
            {
                var unlockedInRegion = region.ZoneIds
                    .Where(id => player.UnlockedZones.Contains(id))
                    .ToList();
                
                text += $"📊 **Exploración de {region.Name}:**\n";
                text += $"🗺️ Zonas desbloqueadas: {unlockedInRegion.Count}/{region.ZoneIds.Count}\n\n";
            }
            
            // Estadísticas globales
            text += $"━━━━━━━━━━━━━━━━━━━━━━\n";
            text += $"🌍 **Progreso Total:**\n";
            text += $"🗺️ Zonas desbloqueadas: {player.UnlockedZones.Count}\n";
            
            var allRegions = RegionDatabase.GetAllRegions();
            var unlockedRegions = allRegions
                .Where(r => r.ZoneIds.Any(zId => player.UnlockedZones.Contains(zId)))
                .Count();
            
            text += $"🌍 Regiones visitadas: {unlockedRegions}/{allRegions.Count}\n";
            
            // Botones
            var buttons = new List<List<InlineKeyboardButton>>();
            
            // Botones de viaje rápido (solo zonas conectadas desbloqueadas)
            var travelButtons = new List<InlineKeyboardButton>();
            foreach (var zone in connectedZones.Take(2))
            {
                if (player.UnlockedZones.Contains(zone.Id) && player.Level >= zone.LevelRequirement)
                {
                    travelButtons.Add(InlineKeyboardButton.WithCallbackData(
                        $"{zone.Emoji} {zone.Name}", 
                        $"rpg_travel_{zone.Id}"));
                }
            }
            
            if (travelButtons.Count > 0)
            {
                buttons.Add(travelButtons);
            }
            
            // Botones de exploración
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🔍 Explorar zona", "rpg_explore"),
                InlineKeyboardButton.WithCallbackData("🗺️ Ver todas las zonas", "rpg_zones_list")
            });
            
            // Botones de navegación
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_map"),
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
