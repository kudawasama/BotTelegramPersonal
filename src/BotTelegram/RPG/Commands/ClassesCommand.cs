using System.Text;
using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /clases - Sistema de clases desbloqueables (FASE 4)
    /// Muestra progreso de desbloqueo, clases activas y permite cambiar de clase.
    /// </summary>
    public class ClassesCommand
    {
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var rpgService = new RpgService();
            var player = rpgService.GetPlayer(chatId);

            if (player == null)
            {
                await bot.SendMessage(chatId,
                    "❌ No tienes un personaje. Usa /rpg para crear uno.",
                    cancellationToken: ct);
                return;
            }

            await ShowClassMenu(bot, chatId, player, ct);
        }

        public static async Task ShowClassMenu(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? editMessageId = null)
        {
            var allDefs = ClassUnlockDatabase.GetAllClassDefinitions();
            var unlockedIds = player.UnlockedClasses;

            var text = new StringBuilder();
            text.AppendLine("🎭 **SISTEMA DE CLASES**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine();

            // Clase activa con sus bonos aplicados
            var activeEmoji = player.ClassEmoji;
            var activeName = GetClassName(player.Class);
            var activeBonus = ClassBonusService.GetBonusDescription(player.ActiveClassId);
            var activeMastery = ClassMasteryService.GetMasteryLevel(player, player.ActiveClassId);
            var masteryTag = activeMastery > 0 ? $" {ClassMasteryService.GetMasteryStars(activeMastery)} M{activeMastery}" : "";
            text.AppendLine($"**CLASE ACTIVA:** {activeEmoji} {activeName}{masteryTag}");
            text.AppendLine($"🎁 Bonos activos: {activeBonus}");
            text.AppendLine($"👤 Nivel {player.Level} | 🔓 {unlockedIds.Count + 1} clases desbloqueadas");
            text.AppendLine();

            // Agrupadas por tier
            foreach (ClassTier tier in Enum.GetValues<ClassTier>().Where(t => t != ClassTier.Hidden && t != ClassTier.Legendary))
            {
                var tierDefs = allDefs.Where(d => d.Tier == tier).ToList();
                if (!tierDefs.Any()) continue;

                text.AppendLine($"{ClassUnlockDatabase.GetTierEmoji(tier)} **TIER {ClassUnlockDatabase.GetTierName(tier).ToUpper()}**");

                foreach (var def in tierDefs)
                {
                    bool isUnlocked = unlockedIds.Contains(def.ClassId);
                    bool isActive = player.ActiveClassId == def.ClassId;
                    double progress = ClassUnlockDatabase.GetUnlockProgress(player, def);
                    bool canUnlock = ClassUnlockDatabase.CanUnlock(player, def);

                    if (isUnlocked)
                    {
                        var activeTag = isActive ? " ◄ ACTIVA" : "";
                        var bonus = ClassBonusService.GetBonusDescription(def.ClassId);
                        var mLevel = ClassMasteryService.GetMasteryLevel(player, def.ClassId);
                        var mTag = mLevel > 0 ? $" {ClassMasteryService.GetMasteryStars(mLevel)}M{mLevel}" : "";
                        text.AppendLine($"  ✅ {def.Emoji} **{def.Name}**{mTag}{activeTag}");
                        text.AppendLine($"      🎁 {bonus}");
                    }
                    else if (progress > 0 || canUnlock)
                    {
                        int pct = (int)(progress * 100);
                        var bar = GetMiniBar(progress);
                        text.AppendLine($"  🔓 {def.Emoji} {def.Name} {bar} {pct}%");
                    }
                    else
                    {
                        text.AppendLine($"  🔒 {def.Emoji} {def.Name} *(requisitos no alcanzados)*");
                    }
                }

                text.AppendLine();
            }

            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine("💡 Selecciona una clase para ver detalles y cambiar.");

            // Teclado
            var keyboard = new List<InlineKeyboardButton[]>();

            // Botones de clases desbloqueadas (para equipar/ver)
            var unlocked = allDefs.Where(d => unlockedIds.Contains(d.ClassId)).ToList();
            var rows = unlocked
                .Select(d => InlineKeyboardButton.WithCallbackData(
                    $"{d.Emoji} {d.Name}{(player.ActiveClassId == d.ClassId ? " ✓" : "")}",
                    $"class_equip_{d.ClassId}"))
                .Chunk(2)
                .Select(chunk => chunk.ToArray())
                .ToList();
            keyboard.AddRange(rows);

            // Botón para ver progreso detallado y maestría
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("📊 Progreso Desbloqueo", "class_progress"),
                InlineKeyboardButton.WithCallbackData("🏅 Maestría", "class_mastery")
            });
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main")
            });

            var markup = new InlineKeyboardMarkup(keyboard);

            if (editMessageId.HasValue)
            {
                await bot.EditMessageText(chatId, editMessageId.Value, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
            else
            {
                await bot.SendMessage(chatId, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
        }

        public static async Task ShowClassProgress(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? editMessageId = null)
        {
            var allDefs = ClassUnlockDatabase.GetAllClassDefinitions();
            var unlockedIds = player.UnlockedClasses;

            var text = new StringBuilder();
            text.AppendLine("📊 **PROGRESO DE DESBLOQUEO**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine();

            // Clases en progreso
            var inProgress = allDefs
                .Where(d => !unlockedIds.Contains(d.ClassId))
                .OrderByDescending(d => ClassUnlockDatabase.GetUnlockProgress(player, d))
                .Take(8)
                .ToList();

            foreach (var def in inProgress)
            {
                double progress = ClassUnlockDatabase.GetUnlockProgress(player, def);
                bool missingLevel = player.Level < def.RequiredLevel;
                bool missingClass = def.RequiredClasses.Any(rc =>
                {
                    var rcDef = ClassUnlockDatabase.GetDefinition(rc);
                    return rcDef != null && !unlockedIds.Contains(rcDef.ClassId);
                });

                text.AppendLine($"{def.Emoji} **{def.Name}** {ClassUnlockDatabase.GetTierEmoji(def.Tier)}");

                if (missingLevel)
                    text.AppendLine($"   ⚠️ Requiere nivel {def.RequiredLevel} (tienes {player.Level})");

                if (missingClass)
                {
                    var missingNames = def.RequiredClasses
                        .Where(rc => { var d = ClassUnlockDatabase.GetDefinition(rc); return d != null && !unlockedIds.Contains(d.ClassId); })
                        .Select(rc => ClassUnlockDatabase.GetDefinition(rc)?.Name ?? "?");
                    text.AppendLine($"   ⚠️ Requiere: {string.Join(", ", missingNames)}");
                }

                foreach (var (actionId, required) in def.RequiredActions)
                {
                    var current = player.ActionCounters.TryGetValue(actionId, out var val) ? val : 0;
                    double ap = Math.Min(1.0, (double)current / required);
                    var bar = GetMiniBar(ap);
                    var actionName = ClassUnlockDatabase.GetActionName(actionId);
                    text.AppendLine($"   {bar} {current}/{required} {actionName}");
                }

                text.AppendLine();
            }

            if (!inProgress.Any())
            {
                text.AppendLine("🏆 ¡Has desbloqueado todas las clases disponibles!");
            }

            var markup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("🎭 Ver Clases", "classes_menu") },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main") }
            });

            if (editMessageId.HasValue)
            {
                await bot.EditMessageText(chatId, editMessageId.Value, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
            else
            {
                await bot.SendMessage(chatId, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
        }

        public static async Task ShowMasteryPanel(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? editMessageId = null)
        {
            var allDefs = ClassUnlockDatabase.GetAllClassDefinitions();
            var unlockedIds = player.UnlockedClasses;

            var text = new StringBuilder();
            text.AppendLine("🏅 **MAESTRÍA DE CLASE**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine("Usa una clase en combate para ganar Maestría.");
            text.AppendLine("Los bonos son *PERMANENTES* al cambiar de clase.");
            text.AppendLine();

            // Bonos totales acumulados
            var totalBonus = new StringBuilder();
            if (player.MasteryBonusStr > 0) totalBonus.Append($"+{player.MasteryBonusStr}💪 ");
            if (player.MasteryBonusInt > 0) totalBonus.Append($"+{player.MasteryBonusInt}🔮 ");
            if (player.MasteryBonusDex > 0) totalBonus.Append($"+{player.MasteryBonusDex}🏃 ");
            if (player.MasteryBonusCon > 0) totalBonus.Append($"+{player.MasteryBonusCon}🛡️ ");
            if (player.MasteryBonusWis > 0) totalBonus.Append($"+{player.MasteryBonusWis}🌟 ");
            if (player.MasteryBonusCha > 0) totalBonus.Append($"+{player.MasteryBonusCha}🎭 ");
            
            var totalStr = totalBonus.Length > 0 ? totalBonus.ToString().Trim() : "Ninguno aún";
            text.AppendLine($"🌟 **Bonos permanentes totales:** {totalStr}");
            text.AppendLine();

            // Mostrar cada clase desbloqueada con su maestría
            var masteredClasses = allDefs
                .Where(d => unlockedIds.Contains(d.ClassId))
                .OrderByDescending(d => ClassMasteryService.GetMasteryLevel(player, d.ClassId))
                .ToList();

            foreach (var def in masteredClasses)
            {
                var mLevel = ClassMasteryService.GetMasteryLevel(player, def.ClassId);
                var stars = ClassMasteryService.GetMasteryStars(mLevel);
                var bar = ClassMasteryService.GetProgressBar(player, def.ClassId);
                var bonusPerLvl = ClassMasteryService.GetBonusDescription(def.ClassId);
                var totalClassBonus = ClassMasteryService.GetTotalBonusForClass(player, def.ClassId);
                var isActive = player.ActiveClassId == def.ClassId;
                var activeTag = isActive ? " ◄" : "";

                text.AppendLine($"{def.Emoji} **{def.Name}** {stars} Lv.{mLevel}/{ClassMasteryService.MaxMasteryLevel}{activeTag}");
                text.AppendLine($"   {bar}");
                text.AppendLine($"   Por nivel: {bonusPerLvl}");
                if (totalClassBonus.HasAnyBonus)
                    text.AppendLine($"   Total acumulado: {totalClassBonus.ToDisplayString()}");
                text.AppendLine();
            }

            if (!masteredClasses.Any())
            {
                text.AppendLine("⚠️ Desbloquea clases para comenzar a ganar maestría.");
            }

            var markup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("🎭 Ver Clases", "classes_menu") },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main") }
            });

            if (editMessageId.HasValue)
            {
                await bot.EditMessageText(chatId, editMessageId.Value, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
            else
            {
                await bot.SendMessage(chatId, text.ToString(),
                    parseMode: ParseMode.Markdown, replyMarkup: markup, cancellationToken: ct);
            }
        }

        private static string GetMiniBar(double progress)
        {
            int filled = (int)(progress * 5);
            return "[" + new string('█', filled) + new string('░', 5 - filled) + "]";
        }

        private static string GetClassName(CharacterClass c) => c switch
        {
            CharacterClass.Adventurer  => "Aventurero",
            CharacterClass.Warrior     => "Guerrero",
            CharacterClass.Mage        => "Mago",
            CharacterClass.Rogue       => "Ladrón",
            CharacterClass.Cleric      => "Clérigo",
            CharacterClass.Paladin     => "Paladín",
            CharacterClass.Ranger      => "Explorador",
            CharacterClass.Warlock     => "Brujo",
            CharacterClass.Monk        => "Monje/Sumo Sacerdote",
            CharacterClass.Berserker   => "Berserker",
            CharacterClass.Assassin    => "Asesino",
            CharacterClass.Sorcerer    => "Hechicero",
            CharacterClass.Druid       => "Druida",
            CharacterClass.Necromancer => "Nigromante",
            CharacterClass.Bard        => "Bardo",
            _ => c.ToString()
        };
    }
}
