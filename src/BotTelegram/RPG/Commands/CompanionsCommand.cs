using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /companions - Muestra todos los compañeros (mascotas y minions) con su información de nivel y XP
    /// FASE 3.5: Sistema de leveling para compañeros
    /// </summary>
    public class CompanionsCommand
    {
        private readonly RpgService _rpgService;
        
        public CompanionsCommand()
        {
            _rpgService = new RpgService();
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
            
            var text = BuildCompanionsText(player);
            var keyboard = BuildCompanionsKeyboard(player);
            
            await bot.SendMessage(
                message.Chat.Id,
                text,
                parseMode: ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        /// <summary>
        /// Construye el texto mostrando todos los compañeros con información de nivel
        /// </summary>
        private string BuildCompanionsText(RpgPlayer player)
        {
            var text = "⚔️ **MIS COMPAÑEROS**\n";
            text += "━━━━━━━━━━━━━━━━━━\n\n";
            
            var hasCompanions = false;
            
            // ═══ MASCOTAS ACTIVAS ═══
            if (player.ActivePets != null && player.ActivePets.Any())
            {
                hasCompanions = true;
                text += "🐾 **MASCOTAS ACTIVAS**\n\n";
                
                foreach (var pet in player.ActivePets)
                {
                    var petEmoji = PetDatabase.GetSpeciesData(pet.Species)?.Emoji ?? "🐾";
                    var hpBar = RpgCombatService.GenerateProgressBar(pet.HP, pet.MaxHP);
                    var xpBar = RpgCombatService.GenerateProgressBar(pet.XP, pet.XPNeeded);
                    
                    text += $"{petEmoji} **{pet.Name}** {pet.RarityEmoji}\n";
                    text += $"   📊 Nivel: **{pet.Level}** | Etapa: {pet.EvolutionStage}/3\n";
                    text += $"   ❤️ HP: {hpBar} {pet.HP}/{pet.MaxHP}\n";
                    text += $"   ⭐ XP: {xpBar} {pet.XP}/{pet.XPNeeded}\n";
                    text += $"   ⚔️ Combates: {pet.CombatsParticipated} | 💀 Kills: {pet.TotalKills}\n";
                    text += $"   👑 Boss Kills: {pet.BossKills} | {pet.LoyaltyEmoji} {pet.Loyalty}\n";
                    text += $"   🗡️ Atk: {pet.Attack} | 🛡️ Def: {pet.Defense}\n\n";
                }
            }
            
            // ═══ MINIONS ACTIVOS ═══
            if (player.ActiveMinions != null && player.ActiveMinions.Any())
            {
                hasCompanions = true;
                text += "👥 **MINIONS ACTIVOS**\n\n";
                
                foreach (var minion in player.ActiveMinions)
                {
                    var xpBar = RpgCombatService.GenerateProgressBar(minion.Experience, minion.ExperienceNeeded);
                    var permanentBadge = minion.IsPermanent ? "🌟" : "⏳";
                    
                    text += $"{minion.Emoji} **{minion.Name}** {permanentBadge}\n";
                    text += $"   📊 Nivel: **{minion.Level}** | Tipo: {minion.Type}\n";
                    text += $"   ⭐ XP: {xpBar} {minion.Experience}/{minion.ExperienceNeeded}\n";
                    text += $"   ❤️ HP: {minion.HP}/{minion.MaxHP} | 🗡️ Atk: {minion.Attack}\n";
                    text += $"   ⚔️ Combates: {minion.CombatsSurvived} | 💀 Kills: {minion.Kills}\n";
                    text += $"   🔥 Daño Total: {minion.TotalDamageDealt}\n";
                    
                    if (!minion.IsPermanent && minion.TurnsRemaining > 0)
                    {
                        text += $"   ⏱️ Turnos restantes: {minion.TurnsRemaining}\n";
                    }
                    
                    text += "\n";
                }
            }
            
            // ═══ MASCOTAS EN INVENTARIO ═══
            var inactivePets = player.PetInventory?.Where(p => !player.ActivePets!.Contains(p)).ToList();
            if (inactivePets != null && inactivePets.Count > 0)
            {
                hasCompanions = true;
                text += $"💤 **MASCOTAS EN DESCANSO** ({inactivePets.Count})\n";
                
                foreach (var pet in inactivePets.Take(5))
                {
                    var petEmoji = PetDatabase.GetSpeciesData(pet.Species)?.Emoji ?? "🐾";
                    var xpBar = RpgCombatService.GenerateProgressBar(pet.XP, pet.XPNeeded);
                    
                    text += $"{petEmoji} **{pet.Name}** - Lv.{pet.Level} {pet.RarityEmoji}\n";
                    text += $"   ⭐ XP: {xpBar} {pet.XP}/{pet.XPNeeded}\n";
                    text += $"   ⚔️ Combates: {pet.CombatsParticipated} | {pet.LoyaltyEmoji}\n\n";
                }
                
                if (inactivePets.Count > 5)
                {
                    text += $"   ... y {inactivePets.Count - 5} más\n\n";
                }
            }
            
            if (!hasCompanions)
            {
                text += "❌ No tienes ningún compañero.\n\n";
                text += "💡 **¿Cómo conseguir compañeros?**\n\n";
                text += "🐾 **Mascotas:**\n";
                text += "• Explora y encuentra bestias salvajes\n";
                text += "• Reduce su HP por debajo del 50%\n";
                text += "• Usa el botón **🐾 Domar** en combate\n";
                text += "• Ganan XP en cada combate\n\n";
                text += "👥 **Minions:**\n";
                text += "• Usa habilidades de clase (Nigromante, etc.)\n";
                text += "• Ganan XP sobreviviendo combates\n";
                text += "• Pueden volverse permanentes\n\n";
            }
            else
            {
                text += "━━━━━━━━━━━━━━━━━━\n";
                text += "💡 **Tips:**\n";
                text += "• Compañeros ganan XP en combate\n";
                text += "• 🐾 Pets: 50 base + 100 kill + 500 boss\n";
                text += "• 👥 Minions: 30 survival + 150 activo + 300 boss\n";
                text += "• Entrenar mascotas: (+200 XP por 100 oro)\n";
            }
            
            return text;
        }
        
        /// <summary>
        /// Construye el teclado inline del menú de compañeros
        /// </summary>
        private InlineKeyboardMarkup BuildCompanionsKeyboard(RpgPlayer player)
        {
            var rows = new List<InlineKeyboardButton[]>();
            
            var hasPets = player.ActivePets != null && player.ActivePets.Any();
            var hasMinions = player.ActiveMinions != null && player.ActiveMinions.Any();
            
            // Botones de gestión de mascotas
            if (hasPets)
            {
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("🍖 Alimentar Pets", "pets_feed_menu"),
                    InlineKeyboardButton.WithCallbackData("⚒️ Entrenar (+XP)", "companions_train_menu")
                });
                
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⭐ Evolucionar", "pets_evolve_menu"),
                    InlineKeyboardButton.WithCallbackData("⚔️ Gestionar Activas", "pets_manage_active")
                });
            }
            
            // Botones de minions
            if (hasMinions)
            {
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("👥 Gestionar Minions", "minions_manage"),
                    InlineKeyboardButton.WithCallbackData("📊 Stats Minions", "minions_stats")
                });
            }
            
            // Botones de navegación
            if (hasPets || hasMinions)
            {
                rows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "companions_refresh"),
                    InlineKeyboardButton.WithCallbackData("📖 Guía", "companions_guide")
                });
            }
            
            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main")
            });
            
            return new InlineKeyboardMarkup(rows);
        }
    }
}
