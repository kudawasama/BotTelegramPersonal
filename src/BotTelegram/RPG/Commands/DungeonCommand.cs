using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /dungeon - Sistema de Mazmorras
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public class DungeonCommand
    {
        private readonly RpgService _rpgService;
        private readonly DungeonService _dungeonService;
        
        public DungeonCommand()
        {
            _rpgService = new RpgService();
            _dungeonService = new DungeonService(_rpgService);
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
            
            // Si el jugador está en una mazmorra, mostrar progreso
            if (player.CurrentDungeon != null && player.CurrentDungeon.IsActive)
            {
                await ShowDungeonProgress(bot, message, player, ct);
            }
            else
            {
                await ShowDungeonList(bot, message, player, ct);
            }
        }
        
        /// <summary>
        /// Muestra el progreso actual en la mazmorra
        /// </summary>
        private async Task ShowDungeonProgress(ITelegramBotClient bot, Message message, RpgPlayer player, CancellationToken ct)
        {
            var dungeon = player.CurrentDungeon!;
            var currentFloor = dungeon.Floors[dungeon.CurrentFloor - 1];
            
            var text = new StringBuilder();
            text.AppendLine($"🏰 **{dungeon.Name.ToUpper()}**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"{DungeonDatabase.GetDifficultyEmoji(dungeon.Difficulty)} **Dificultad:** {DungeonDatabase.GetDifficultyName(dungeon.Difficulty)} ({dungeon.TotalFloors} pisos)");
            text.AppendLine($"📍 **Piso actual:** {dungeon.CurrentFloor}/{dungeon.TotalFloors}");
            text.AppendLine($"⚔️ **Nivel recomendado:** {dungeon.MinLevel}+");
            text.AppendLine();
            
            // Estado del jugador
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"👤 **{player.Name}** (Lv.{player.Level})");
            var hpBar = RpgCombatService.GenerateProgressBar(player.HP, player.MaxHP);
            text.AppendLine($"❤️ HP: {hpBar} {player.HP}/{player.MaxHP} ({(player.HP * 100 / player.MaxHP)}%)");
            var manaBar = RpgCombatService.GenerateProgressBar(player.Mana, player.MaxMana);
            text.AppendLine($"💙 Mana: {manaBar} {player.Mana}/{player.MaxMana} ({(player.Mana * 100 / player.MaxMana)}%)");
            var staminaBar = RpgCombatService.GenerateProgressBar(player.Stamina, player.MaxStamina);
            text.AppendLine($"💛 Stamina: {staminaBar} {player.Stamina}/{player.MaxStamina} ({(player.Stamina * 100 / player.MaxStamina)}%)");
            text.AppendLine();
            
            // Compañeros
            if (player.ActivePets != null && player.ActivePets.Any())
            {
                text.AppendLine($"🐾 **Mascotas:** {player.ActivePets.Count}/{player.MaxActivePets}");
                foreach (var pet in player.ActivePets.Take(2))
                {
                    text.AppendLine($"   {PetDatabase.GetSpeciesData(pet.Species)?.Emoji ?? "🐾"} {pet.Name}: {pet.HP}/{pet.MaxHP} HP");
                }
            }
            
            if (player.ActiveMinions != null && player.ActiveMinions.Any())
            {
                text.AppendLine($"💀 **Minions:** {player.ActiveMinions.Count}/3");
                foreach (var minion in player.ActiveMinions.Take(2))
                {
                    text.AppendLine($"   {minion.Emoji} {minion.Name}: {minion.HP}/{minion.MaxHP} HP");
                }
            }
            text.AppendLine();
            
            // Progreso de pisos
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine("📊 **PROGRESO:**");
            
            // Mostrar pisos completados (últimos 3)
            var completedFloors = dungeon.Floors.Where(f => f.IsCleared).TakeLast(3).ToList();
            if (completedFloors.Any())
            {
                foreach (var floor in completedFloors)
                {
                    text.AppendLine($"✅ Piso {floor.FloorNumber}: {GetFloorTypeEmoji(floor.Type)} completado");
                }
            }
            
            // Piso actual
            text.AppendLine($"🔶 **Piso {currentFloor.FloorNumber} (ACTUAL):** {currentFloor.Description}");
            text.AppendLine();
            
            // Próximos 3 pisos (ocultos)
            var upcomingCount = Math.Min(3, dungeon.TotalFloors - dungeon.CurrentFloor);
            if (upcomingCount > 0)
            {
                text.AppendLine($"⬜ Pisos {dungeon.CurrentFloor + 1}-{dungeon.CurrentFloor + upcomingCount}: ???");
                
                // Mostrar si hay un boss floor próximo
                for (int i = dungeon.CurrentFloor + 1; i <= dungeon.CurrentFloor + upcomingCount; i++)
                {
                    if (i % 5 == 0)
                    {
                        text.AppendLine($"💀 **Piso {i}: BOSS FLOOR**");
                        break;
                    }
                }
            }
            text.AppendLine();
            
            // Información del piso actual
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine($"📋 **PISO {currentFloor.FloorNumber}:**");
            
            switch (currentFloor.Type)
            {
                case FloorType.Combat:
                    text.AppendLine($"🗡️ **Combate** - {currentFloor.EnemyCount} enemigo(s)");
                    text.AppendLine($"💰 Recompensa: ~{currentFloor.Reward?.Gold ?? 0} oro, {currentFloor.Reward?.XP ?? 0} XP");
                    break;
                    
                case FloorType.Elite:
                    text.AppendLine($"⚔️ **Enemigo Élite** - Más fuerte que los comunes");
                    text.AppendLine($"💰 Recompensa: ~{currentFloor.Reward?.Gold ?? 0} oro, {currentFloor.Reward?.XP ?? 0} XP");
                    break;
                    
                case FloorType.Boss:
                    text.AppendLine($"💀 **¡JEFE!** - Combate difícil");
                    text.AppendLine($"💰 Recompensa: ~{currentFloor.Reward?.Gold ?? 0} oro, {currentFloor.Reward?.XP ?? 0} XP");
                    if (currentFloor.Reward?.HasSkillPoint ?? false)
                    {
                        text.AppendLine($"⭐ **+1 Skill Point**");
                    }
                    break;
                    
                case FloorType.Rest:
                    text.AppendLine($"😴 **Sala de Descanso**");
                    text.AppendLine($"💚 Restaura {currentFloor.RestorePercentage}% de HP/Mana/Stamina");
                    break;
                    
                case FloorType.Trap:
                    text.AppendLine($"🪤 **Trampa:** {currentFloor.Trap?.Name ?? "Desconocida"}");
                    text.AppendLine($"⚠️ {currentFloor.Trap?.Description ?? "Peligro adelante"}");
                    break;
            }
            
            // Botones de acción
            var keyboard = new List<InlineKeyboardButton[]>();
            
            if (!currentFloor.IsCleared)
            {
                if (currentFloor.Type == FloorType.Rest)
                {
                    keyboard.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("😴 Descansar", $"dungeon_rest_{dungeon.CurrentFloor}"),
                        InlineKeyboardButton.WithCallbackData("⏭️ Continuar", $"dungeon_advance_{dungeon.CurrentFloor}")
                    });
                }
                else if (currentFloor.Type == FloorType.Trap)
                {
                    keyboard.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🎲 Esquivar", $"dungeon_avoid_trap_{dungeon.CurrentFloor}"),
                        InlineKeyboardButton.WithCallbackData("⚔️ Enfrentar", $"dungeon_face_trap_{dungeon.CurrentFloor}")
                    });
                }
                else // Combat, Elite, Boss
                {
                    keyboard.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⚔️ Iniciar Combate", $"dungeon_fight_{dungeon.CurrentFloor}")
                    });
                }
            }
            else
            {
                // Piso completado, permitir avanzar
                if (dungeon.CurrentFloor < dungeon.TotalFloors)
                {
                    keyboard.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("➡️ Avanzar al Siguiente Piso", $"dungeon_next_floor")
                    });
                }
                else
                {
                    keyboard.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏆 Reclamar Recompensas", $"dungeon_complete")
                    });
                }
            }
            
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🎒 Inventario", "rpg_inventory"),
                InlineKeyboardButton.WithCallbackData("📊 Stats", "rpg_stats")
            });
            
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("❌ Rendirse (Pierdes todo)", $"dungeon_abandon")
            });
            
            await bot.SendMessage(
                message.Chat.Id,
                text.ToString(),
                parseMode: ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(keyboard),
                cancellationToken: ct);
        }
        
        /// <summary>
        /// Muestra la lista de mazmorras disponibles
        /// </summary>
        private async Task ShowDungeonList(ITelegramBotClient bot, Message message, RpgPlayer player, CancellationToken ct)
        {
            var text = new StringBuilder();
            text.AppendLine("🏰 **MAZMORRAS DISPONIBLES**");
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine();
            text.AppendLine($"👤 {player.Name} (Lv.{player.Level})");
            text.AppendLine($"🔑 Llaves disponibles: {player.DungeonKeys.Count(k => !k.IsConsumed)}");
            text.AppendLine($"🏆 Mazmorras completadas: {player.TotalDungeonsCompleted}");
            text.AppendLine();
            
            // Obtener mazmorras disponibles
            var templates = DungeonDatabase.GetAllDungeonTemplates();
            
            // Agrupar por dificultad
            var difficulties = new[] 
            { 
                DungeonDifficulty.Common, 
                DungeonDifficulty.Uncommon, 
                DungeonDifficulty.Rare, 
                DungeonDifficulty.Epic, 
                DungeonDifficulty.Legendary 
            };
            
            foreach (var difficulty in difficulties)
            {
                var dungeonsOfDifficulty = templates.Values
                    .Where(d => d.Difficulty == difficulty)
                    .ToList();
                
                if (!dungeonsOfDifficulty.Any()) continue;
                
                text.AppendLine($"{DungeonDatabase.GetDifficultyEmoji(difficulty)} **{DungeonDatabase.GetDifficultyName(difficulty).ToUpper()}**");
                text.AppendLine($"━━━━━━━━━━━━━━━━━━━━");
                
                foreach (var template in dungeonsOfDifficulty)
                {
                    var floors = DungeonDatabase.GetTotalFloorsForDifficulty(template.Difficulty);
                    var keyEmoji = DungeonKey.GetEmojiForDifficulty(template.Difficulty);
                    var hasKey = player.DungeonKeys.Any(k => 
                        k.UnlocksDifficulty == template.Difficulty && !k.IsConsumed);
                    
                    var canEnter = player.Level >= template.MinLevel && hasKey;
                    var statusIcon = canEnter ? "✅" : "🔒";
                    
                    text.AppendLine($"{statusIcon} {template.Emoji} **{template.Name}**");
                    text.AppendLine($"   📊 {floors} pisos | Lv.{template.MinLevel}+ | {keyEmoji} Requiere llave");
                    text.AppendLine($"   📝 {template.Description}");
                    
                    if (!hasKey)
                    {
                        text.AppendLine($"   ⚠️ Necesitas una llave {keyEmoji}");
                    }
                    else if (player.Level < template.MinLevel)
                    {
                        text.AppendLine($"   ⚠️ Nivel insuficiente (necesitas Lv.{template.MinLevel})");
                    }
                    
                    text.AppendLine();
                }
            }
            
            text.AppendLine("━━━━━━━━━━━━━━━━━━━━");
            text.AppendLine("💡 **¿Cómo conseguir llaves?**");
            text.AppendLine("• Derrota jefes (Boss) en combates normales");
            text.AppendLine("• Probabilidad aumenta con dificultad del jefe");
            text.AppendLine("• 🔑 Common: 15% | 🗝️ Uncommon: 10% | 🔐 Rare: 7%");
            text.AppendLine("• 🎖️ Epic: 5% | 👑 Legendary: 3%");
            
            // Botones
            var keyboard = new List<InlineKeyboardButton[]>();
            
            // Botones de mazmorras disponibles (solo las que puede entrar)
            var availableDungeons = templates.Values
                .Where(d => player.Level >= d.MinLevel && 
                           player.DungeonKeys.Any(k => k.UnlocksDifficulty == d.Difficulty && !k.IsConsumed))
                .Take(6) // Máximo 6 botones
                .ToList();
            
            foreach (var dungeon in availableDungeons)
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        $"{dungeon.Emoji} Entrar: {dungeon.Name}", 
                        $"dungeon_enter_{dungeon.Id}")
                });
            }
            
            // Botones de navegación
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔑 Ver Llaves", "dungeon_keys"),
                InlineKeyboardButton.WithCallbackData("🏆 Rankings", "dungeon_rankings")
            });
            
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver RPG", "rpg_main")
            });
            
            await bot.SendMessage(
                message.Chat.Id,
                text.ToString(),
                parseMode: ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(keyboard),
                cancellationToken: ct);
        }
        
        /// <summary>
        /// Obtiene el emoji según el tipo de piso
        /// </summary>
        private string GetFloorTypeEmoji(FloorType type)
        {
            return type switch
            {
                FloorType.Combat => "🗡️",
                FloorType.Elite => "⚔️",
                FloorType.Boss => "💀",
                FloorType.Rest => "😴",
                FloorType.Trap => "🪤",
                _ => "❓"
            };
        }
    }
}
