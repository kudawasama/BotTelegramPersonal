using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    public class RpgCountersCommand
    {
        private readonly RpgService _rpgService;
        
        public RpgCountersCommand()
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
            
            await ShowCounters(bot, chatId, player, ct);
        }
        
        private async Task ShowCounters(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var text = $@"📊 **CONTADORES DE ACCIÓN**

Estas estadísticas rastrean tus acciones en combate y se usan para desbloquear skills.

━━━━━━━━━━━━━━━━━━━━━━
";
            
            if (!player.ActionCounters.Any())
            {
                text += "*No hay estadísticas registradas aún. ¡Comienza a pelear para desbloquear skills!*\n";
            }
            else
            {
                // Agrupar contadores por categoría
                var combatActions = new[] { "physical_attack", "magic_attack", "charge_attack", "precise_attack", "heavy_attack" };
                var defensiveActions = new[] { "block", "dodge", "counter" };
                var movementActions = new[] { "jump", "retreat", "advance" };
                var specialActions = new[] { "meditate", "observe", "wait" };
                var eventCounters = new[] { "critical_hit", "damage_dealt", "damage_taken", "perfect_dodge", "combat_survived", "low_hp_combat", "enemy_defeated", "combo_5plus", "combo_10plus" };
                
                // Acciones de combate
                text += "⚔️ **Acciones de Ataque**\n";
                foreach (var action in combatActions)
                {
                    if (player.ActionCounters.ContainsKey(action))
                    {
                        text += $"  • {FormatActionName(action)}: **{player.ActionCounters[action]:N0}**\n";
                    }
                }
                text += "\n";
                
                // Acciones defensivas
                text += "🛡️ **Acciones Defensivas**\n";
                foreach (var action in defensiveActions)
                {
                    if (player.ActionCounters.ContainsKey(action))
                    {
                        text += $"  • {FormatActionName(action)}: **{player.ActionCounters[action]:N0}**\n";
                    }
                }
                text += "\n";
                
                // Movimiento
                text += "💨 **Movimiento**\n";
                foreach (var action in movementActions)
                {
                    if (player.ActionCounters.ContainsKey(action))
                    {
                        text += $"  • {FormatActionName(action)}: **{player.ActionCounters[action]:N0}**\n";
                    }
                }
                text += "\n";
                
                // Acciones especiales
                text += "✨ **Acciones Especiales**\n";
                foreach (var action in specialActions)
                {
                    if (player.ActionCounters.ContainsKey(action))
                    {
                        text += $"  • {FormatActionName(action)}: **{player.ActionCounters[action]:N0}**\n";
                    }
                }
                text += "\n";
                
                // Eventos de combate
                text += "📈 **Eventos de Combate**\n";
                foreach (var counter in eventCounters)
                {
                    if (player.ActionCounters.ContainsKey(counter))
                    {
                        text += $"  • {FormatActionName(counter)}: **{player.ActionCounters[counter]:N0}**\n";
                    }
                }
                
                // Skills usadas
                var skillCounters = player.ActionCounters
                    .Where(kvp => kvp.Key.StartsWith("skill_") && kvp.Key != "skill_used")
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(5);
                
                if (skillCounters.Any())
                {
                    text += "\n🎯 **Skills Más Usadas**\n";
                    foreach (var skill in skillCounters)
                    {
                        var skillId = skill.Key.Replace("skill_", "");
                        var skillInfo = SkillDatabase.GetById(skillId);
                        var skillName = skillInfo?.Name ?? skillId;
                        text += $"  • {skillName}: **{skill.Value:N0}** veces\n";
                    }
                }
                
                // Total de acciones
                var totalActions = player.ActionCounters
                    .Where(kvp => combatActions.Contains(kvp.Key) || defensiveActions.Contains(kvp.Key) || movementActions.Contains(kvp.Key) || specialActions.Contains(kvp.Key))
                    .Sum(kvp => kvp.Value);
                
                text += $"\n━━━━━━━━━━━━━━━━━━━━━━\n";
                text += $"📊 **Total de Acciones**: {totalActions:N0}\n";
            }
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✨ Ver Skills", "rpg_skills"),
                    InlineKeyboardButton.WithCallbackData("📊 Ver Stats", "rpg_stats")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_counters"),
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
        
        private string FormatActionName(string action)
        {
            return action switch
            {
                "physical_attack" => "Ataques Físicos",
                "magic_attack" => "Ataques Mágicos",
                "charge_attack" => "Envestidas",
                "precise_attack" => "Ataques Precisos",
                "heavy_attack" => "Ataques Pesados",
                "block" => "Bloqueos",
                "dodge" => "Esquivas",
                "counter" => "Contraataques",
                "jump" => "Saltos",
                "retreat" => "Retrocesos",
                "advance" => "Avances",
                "meditate" => "Meditaciones",
                "observe" => "Observaciones",
                "wait" => "Esperas",
                "critical_hit" => "Golpes Críticos",
                "damage_dealt" => "Daño Infligido",
                "damage_taken" => "Daño Recibido",
                "perfect_dodge" => "Esquivas Perfectas",
                "combat_survived" => "Combates Sobrevividos",
                "low_hp_combat" => "Combates con HP Baja",
                "enemy_defeated" => "Enemigos Derrotados",
                "combo_5plus" => "Combos de 5+",
                "combo_10plus" => "Combos de 10+",
                "skill_used" => "Skills Usadas",
                _ => action.Replace("_", " ").ToUpper()
            };
        }
    }
}
