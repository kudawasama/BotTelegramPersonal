using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    public class RpgStatsCommand
    {
        private readonly RpgService _rpgService;
        
        public RpgStatsCommand()
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
            
            await ShowDetailedStats(bot, chatId, player, ct);
        }
        
        private async Task ShowDetailedStats(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var classEmoji = player.Class switch
            {
                CharacterClass.Warrior => "⚔️",
                CharacterClass.Mage => "🔮",
                CharacterClass.Rogue => "🗡️",
                CharacterClass.Cleric => "✨",
                _ => "👤"
            };
            
            // Stats principales con active values
            var stats = $@"📊 **ESTADÍSTICAS DETALLADAS**

{classEmoji} **{player.Name}** - {player.Class} Nivel {player.Level}
━━━━━━━━━━━━━━━━━━━━━━

💪 **Stats Primarias** (Base → Activo)
  • Fuerza: {player.Strength} → **{player.ActiveStrength}**
  • Inteligencia: {player.Intelligence} → **{player.ActiveIntelligence}**
  • Destreza: {player.Dexterity} → **{player.ActiveDexterity}**
  • Constitución: {player.Constitution} → **{player.ActiveConstitution}**
  • Sabiduría: {player.Wisdom} → **{player.ActiveWisdom}**
  • Carisma: {player.Charisma} → **{player.ActiveCharisma}**

⚔️ **Stats de Combate**
  • Ataque Físico: **{player.PhysicalAttack}**
  • Ataque Mágico: **{player.MagicalAttack}**
  • Defensa Física: **{player.PhysicalDefense}**
  • Defensa Mágica: **{player.MagicResistance}**
  • Precisión: **{player.Accuracy}**
  • Evasión: **{player.Evasion}**
  • Crit Chance: **{player.CriticalChance:F1}%**

❤️ **Recursos**
  • HP: {player.HP}/{player.MaxHP}
  • Mana: {player.Mana}/{player.MaxMana}
  • Stamina: {player.Stamina}/{player.MaxStamina}
  • Energía: {player.Energy}/{player.MaxEnergy}

💰 **Progreso**
  • Oro: {player.Gold}
  • XP: {player.XP}/{player.XPNeeded}
  • Kills: {player.TotalKills}
  • Muertes: {player.TotalDeaths}
  • Oro Total: {player.TotalGoldEarned}

";
            
            // Equipment equipado
            var equipment = "🎒 **Equipment Equipado**\n";
            
            if (player.EquippedWeaponNew != null)
            {
                var wpn = player.EquippedWeaponNew;
                equipment += $"  {wpn.TypeEmoji} **{wpn.Name}** {wpn.RarityEmoji}\n";
                equipment += $"     Lv.{wpn.RequiredLevel} | ";
                if (wpn.BonusAttack > 0) equipment += $"+{wpn.BonusAttack} Atk ";
                if (wpn.BonusMagicPower > 0) equipment += $"+{wpn.BonusMagicPower} MP ";
                equipment += "\n";
            }
            else if (player.EquippedWeapon != null)
            {
                equipment += $"  🗡️ **{player.EquippedWeapon.Name}** (Legacy)\n";
            }
            else
            {
                equipment += "  🗡️ *Sin arma*\n";
            }
            
            if (player.EquippedArmorNew != null)
            {
                var arm = player.EquippedArmorNew;
                equipment += $"  {arm.TypeEmoji} **{arm.Name}** {arm.RarityEmoji}\n";
                equipment += $"     Lv.{arm.RequiredLevel} | ";
                if (arm.BonusDefense > 0) equipment += $"+{arm.BonusDefense} Def ";
                if (arm.BonusMagicResistance > 0) equipment += $"+{arm.BonusMagicResistance} MR ";
                equipment += "\n";
            }
            else if (player.EquippedArmor != null)
            {
                equipment += $"  🛡️ **{player.EquippedArmor.Name}** (Legacy)\n";
            }
            else
            {
                equipment += "  🛡️ *Sin armadura*\n";
            }
            
            if (player.EquippedAccessoryNew != null)
            {
                var acc = player.EquippedAccessoryNew;
                equipment += $"  {acc.TypeEmoji} **{acc.Name}** {acc.RarityEmoji}\n";
                equipment += $"     Lv.{acc.RequiredLevel} | ";
                var bonuses = new List<string>();
                if (acc.BonusStrength > 0) bonuses.Add($"+{acc.BonusStrength} STR");
                if (acc.BonusIntelligence > 0) bonuses.Add($"+{acc.BonusIntelligence} INT");
                if (acc.BonusDexterity > 0) bonuses.Add($"+{acc.BonusDexterity} DEX");
                if (bonuses.Any()) equipment += string.Join(", ", bonuses);
                equipment += "\n";
            }
            else if (player.EquippedAccessory != null)
            {
                equipment += $"  💍 **{player.EquippedAccessory.Name}** (Legacy)\n";
            }
            else
            {
                equipment += "  💍 *Sin accesorio*\n";
            }
            
            stats += equipment;
            
            // Skills desbloqueadas
            stats += $"\n✨ **Skills Desbloqueadas**: {player.UnlockedSkills.Count}/16\n";
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🎒 Equipment", "rpg_equipment"),
                    InlineKeyboardButton.WithCallbackData("✨ Skills", "rpg_skills")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📊 Counters", "rpg_counters"),
                    InlineKeyboardButton.WithCallbackData("🔄 Actualizar", "rpg_stats")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú RPG", "rpg_main")
                }
            });
            
            await bot.SendMessage(
                chatId,
                stats,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
    }
}
