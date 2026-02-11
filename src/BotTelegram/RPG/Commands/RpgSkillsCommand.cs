using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    public class RpgSkillsCommand
    {
        private readonly RpgService _rpgService;
        
        public RpgSkillsCommand()
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
            
            await ShowSkillsMenu(bot, chatId, player, ct);
        }
        
        private async Task ShowSkillsMenu(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var allSkills = SkillDatabase.GetAllSkills();
            var unlockedSkills = allSkills.Where(s => player.UnlockedSkills.Contains(s.Id)).ToList();
            var lockedSkills = allSkills.Where(s => !player.UnlockedSkills.Contains(s.Id)).ToList();
            
            var text = $@"✨ **SKILLS** ({unlockedSkills.Count}/16 desbloqueadas)

━━━━━━━━━━━━━━━━━━━━━━
";
            
            // Skills desbloqueadas
            if (unlockedSkills.Any())
            {
                text += "🔓 **Skills Activas**\n\n";
                
                foreach (var skill in unlockedSkills.OrderBy(s => s.RequiredLevel))
                {
                    var cooldown = player.SkillCooldowns.ContainsKey(skill.Id) && player.SkillCooldowns[skill.Id] > 0
                        ? $" (CD: {player.SkillCooldowns[skill.Id]})"
                        : "";
                    
                    text += $"{GetCategoryEmoji(skill.Category)} **{skill.Name}** (Lv.{skill.RequiredLevel}){cooldown}\n";
                    
                    // Costos
                    var costs = new List<string>();
                    if (skill.ManaCost > 0) costs.Add($"{skill.ManaCost} Mana");
                    if (skill.StaminaCost > 0) costs.Add($"{skill.StaminaCost} Stamina");
                    if (costs.Any())
                        text += $"   💰 {string.Join(", ", costs)}";
                    
                    if (skill.Cooldown > 0)
                        text += $" | ⏱️ CD: {skill.Cooldown} turnos";
                    
                    text += "\n";
                    text += $"   {skill.Description}\n\n";
                }
            }
            else
            {
                text += "🔒 *No has desbloqueado ninguna skill aún.*\n\n";
            }
            
            // Skills próximas a desbloquear
            var nearlyUnlockable = SkillDatabase.GetNearlyUnlockableSkills(player, threshold: 10);
            
            if (nearlyUnlockable.Any())
            {
                text += "━━━━━━━━━━━━━━━━━━━━━━\n";
                text += "🔜 **Próximas a Desbloquear**\n\n";
                
                foreach (var skill in nearlyUnlockable.Take(3))
                {
                    text += $"{GetCategoryEmoji(skill.Category)} **{skill.Name}** (Lv.{skill.RequiredLevel})\n";
                    
                    var progress = skill.GetUnlockProgress(player);
                    foreach (var req in skill.Requirements)
                    {
                        var current = player.ActionCounters.ContainsKey(req.ActionType) 
                            ? player.ActionCounters[req.ActionType] 
                            : 0;
                        var bar = GetProgressBar(current, req.Count);
                        text += $"   {bar} {req.Description}: {current}/{req.Count}\n";
                    }
                    text += "\n";
                }
            }
            
            // Mostrar 3 skills locked como ejemplo
            var sampleLocked = lockedSkills
                .Where(s => !nearlyUnlockable.Contains(s))
                .OrderBy(s => s.RequiredLevel)
                .Take(3)
                .ToList();
            
            if (sampleLocked.Any())
            {
                text += "━━━━━━━━━━━━━━━━━━━━━━\n";
                text += "🔒 **Skills Bloqueadas** (muestra)\n\n";
                
                foreach (var skill in sampleLocked)
                {
                    text += $"{GetCategoryEmoji(skill.Category)} **{skill.Name}** (Lv.{skill.RequiredLevel})\n";
                    text += $"   {skill.Description}\n";
                    text += $"   Requisitos: ";
                    var reqs = skill.Requirements.Select(r => $"{r.Count} {r.ActionType}");
                    text += string.Join(", ", reqs) + "\n\n";
                }
            }
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats"),
                    InlineKeyboardButton.WithCallbackData("📊 Counters", "rpg_counters")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_skills"),
                    InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
                }
            });
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        private string GetCategoryEmoji(SkillCategory category)
        {
            return category switch
            {
                SkillCategory.Combat => "⚔️",
                SkillCategory.Magic => "🔮",
                SkillCategory.Defense => "🛡️",
                SkillCategory.Movement => "💨",
                SkillCategory.Special => "✨",
                _ => "❓"
            };
        }
        
        private string GetProgressBar(int current, int required)
        {
            var percent = Math.Min(100, (int)((current / (double)required) * 100));
            var filled = percent / 10;
            var empty = 10 - filled;
            return $"[{'█'.ToString().PadLeft(filled, '█')}{'░'.ToString().PadLeft(empty, '░')}]";
        }
    }
}
