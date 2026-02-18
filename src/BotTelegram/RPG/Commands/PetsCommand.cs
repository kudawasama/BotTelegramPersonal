using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /pets - Gestión completa de mascotas del jugador
    /// </summary>
    public class PetsCommand
    {
        private readonly RpgService _rpgService;
        private readonly PetTamingService _petTamingService;
        
        public PetsCommand()
        {
            _rpgService = new RpgService();
            _petTamingService = new PetTamingService(_rpgService);
        }
        
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var player = _rpgService.GetPlayer(message.Chat.Id);
            
            if (player == null)
            {
                await bot.SendMessage(
                    message.Chat.Id,
                    "❌ Necesitas crear un personaje primero. Usa /rpg para comenzar.",
                    cancellationToken: ct);
                return;
            }
            
            var text = BuildPetMenuText(player);
            var keyboard = BuildPetMenuKeyboard(player);
            
            await bot.SendMessage(
                message.Chat.Id,
                text,
                parseMode: ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        /// <summary>
        /// Construye el texto del menú principal de mascotas
        /// </summary>
        private string BuildPetMenuText(RpgPlayer player)
        {
            var text = "🐾 **SISTEMA DE MASCOTAS**\n\n";
            
            // Resumen de mascotas
            var totalPets = player.PetInventory?.Count ?? 0;
            var activePets = player.ActivePets?.Count ?? 0;
            
            text += $"📊 **Mascotas Domadas:** {totalPets}\n";
            text += $"⚔️ **Activas en combate:** {activePets}/{player.MaxActivePets}\n\n";
            
            // Mostrar mascotas activas
            if (activePets > 0)
            {
                text += "✨ **MASCOTAS ACTIVAS**\n";
                foreach (var pet in player.ActivePets!)
                {
                    var emoji = GetPetEmoji(pet.Species);
                    var hpBar = BotTelegram.RPG.Services.RpgCombatService.GenerateProgressBar(pet.HP, pet.MaxHP);
                    text += $"{emoji} **{pet.Name}** {pet.RarityEmoji}\n";
                    text += $"   Lv.{pet.Level} | HP: {hpBar} {pet.HP}/{pet.MaxHP}\n";
                    text += $"   {pet.LoyaltyEmoji} {pet.Loyalty} | Bond: {pet.Bond}/1000\n\n";
                }
            }
            
            // Mostrar mascotas en inventario (no activas)
            var inactivePets = player.PetInventory?.Where(p => !player.ActivePets!.Contains(p)).ToList();
            if (inactivePets != null && inactivePets.Count > 0)
            {
                text += $"💤 **MASCOTAS EN DESCANSO** ({inactivePets.Count})\n";
                foreach (var pet in inactivePets.Take(3))
                {
                    var emoji = GetPetEmoji(pet.Species);
                    text += $"{emoji} {pet.Name} (Lv.{pet.Level}) - {pet.LoyaltyEmoji}\n";
                }
                if (inactivePets.Count > 3)
                {
                    text += $"   ... y {inactivePets.Count - 3} más\n";
                }
                text += "\n";
            }
            
            if (totalPets == 0)
            {
                text += "❌ No tienes ninguna mascota domada.\n\n";
                text += "💡 **¿Cómo domar mascotas?**\n";
                text += "1. Explora y encuentra bestias (Lobos, Osos, etc.)\n";
                text += "2. Reduce su HP por debajo del 50%\n";
                text += "3. Usa el botón **🐾 Domar** en combate\n";
                text += "4. Aumenta tu Charisma para mejor chance\n\n";
            }
            
            return text;
        }
        
        /// <summary>
        /// Construye el teclado inline del menú de mascotas
        /// </summary>
        private InlineKeyboardMarkup BuildPetMenuKeyboard(RpgPlayer player)
        {
            var rows = new List<InlineKeyboardButton[]>();
            
            // Botones de gestión
            if (player.PetInventory != null && player.PetInventory.Count > 0)
            {
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("📋 Listar Todas", "pets_list_all"),
                    InlineKeyboardButton.WithCallbackData("⚔️ Gestionar Activas", "pets_manage_active")
                });
                
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("🍖 Alimentar", "pets_feed_menu"),
                    InlineKeyboardButton.WithCallbackData("⭐ Evolucionar", "pets_evolve_menu")
                });
            }
            
            // Guía y volver
            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("📖 Guía", "pets_guide"),
                InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main")
            });
            
            return new InlineKeyboardMarkup(rows);
        }
        
        /// <summary>
        /// Obtiene el emoji según la especie
        /// </summary>
        private string GetPetEmoji(string species)
        {
            if (species.StartsWith("wolf_")) return "🐺";
            if (species.StartsWith("bear_")) return "🐻";
            if (species.StartsWith("dragon_")) return "🐉";
            if (species.StartsWith("cat_") || species.StartsWith("wildcat_")) return "🐱";
            if (species.StartsWith("eagle_")) return "🦅";
            if (species.StartsWith("snake_")) return "🐍";
            return "🐾";
        }
    }
}
