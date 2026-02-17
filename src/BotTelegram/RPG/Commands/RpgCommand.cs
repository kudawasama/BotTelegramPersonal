using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;
using BotTelegram.Services;

namespace BotTelegram.RPG.Commands
{
    public class RpgCommand
    {
        private readonly RpgService _rpgService;
        
        public RpgCommand()
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
            
            // 🎯 LOG: Registrar comando /rpg
            TelegramLogger.LogUserAction(
                chatId: chatId,
                username: message.From?.Username ?? "unknown",
                action: "/rpg",
                details: player == null ? "New player (welcome screen)" : $"Existing player: {player.Name} (Lv.{player.Level})"
            );
            
            if (player == null)
            {
                // Show welcome screen
                await ShowWelcomeScreen(bot, chatId, ct);
            }
            else
            {
                // Show main game menu
                await ShowMainMenu(bot, chatId, player, ct);
            }
        }
        
        private async Task ShowWelcomeScreen(ITelegramBotClient bot, long chatId, CancellationToken ct)
        {
            var text = @"🎭 **LEYENDA DEL VOID**

Bienvenido, aventurero. El reino de Valentia está en peligro. Criaturas oscuras emergen del Void y amenazan con destruir todo lo que conocemos.

Solo los más valientes pueden enfrentar este destino...

*¿Estás listo para tu aventura?*";
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Nueva Partida", "rpg_new_game")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Historia", "rpg_lore"),
                    InlineKeyboardButton.WithCallbackData("❓ Cómo Jugar", "rpg_tutorial")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "start")
                }
            });
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        public async Task ShowMainMenu(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var classEmoji = player.Class switch
            {
                CharacterClass.Warrior => "⚔️",
                CharacterClass.Mage => "🔮",
                CharacterClass.Rogue => "🗡️",
                CharacterClass.Cleric => "✨",
                _ => "👤"
            };
            
            var statusBar = $"❤️ {player.HP}/{player.MaxHP} | 🔮 {player.Mana}/{player.MaxMana} | ⚡ {player.Energy}/{player.MaxEnergy}";
            var xpBar = GetXPBar(player);
            
            var text = $@"🎮 **MENÚ RPG**

{classEmoji} **{player.Name}** - {player.Class} Nv.{player.Level}
{statusBar}
{xpBar}
💰 {player.Gold} oro

📍 *{player.CurrentLocation}*

";
            
            // Mostrar mascotas activas si las hay
            if (player.ActivePets != null && player.ActivePets.Any(p => p.HP > 0))
            {
                text += "🐾 **Compañeras activas:**\n";
                foreach (var pet in player.ActivePets.Where(p => p.HP > 0).Take(3))
                {
                    var hpPercent = (double)pet.HP / pet.MaxHP * 100;
                    var hpEmoji = hpPercent > 70 ? "💚" : hpPercent > 30 ? "💛" : "❤️";
                    text += $"  • {pet.Name} (Lv.{pet.Level}) {hpEmoji} {hpPercent:F0}% | {pet.LoyaltyEmoji}\n";
                }
                text += "\n";
            }
            
            if (player.IsInCombat && player.CurrentEnemy != null)
            {
                text += $"⚔️ **¡COMBATE!**\n{player.CurrentEnemy.Emoji} {player.CurrentEnemy.Name} (Lv.{player.CurrentEnemy.Level})\n❤️ {player.CurrentEnemy.HP}/{player.CurrentEnemy.MaxHP} HP\n\n";
            }
            
            var keyboard = player.IsInCombat 
                ? GetCombatKeyboard()
                : GetExplorationKeyboard();
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        private InlineKeyboardMarkup GetExplorationKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                // ACCIONES PRINCIPALES
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Explorar", "rpg_explore_menu"),
                    InlineKeyboardButton.WithCallbackData("🗺️ Aventura", "rpg_adventure"),
                    InlineKeyboardButton.WithCallbackData("🐾 Mascotas", "rpg_pets_menu")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("😴 Descansar", "rpg_rest"),
                    InlineKeyboardButton.WithCallbackData("💼 Trabajar", "rpg_work"),
                    InlineKeyboardButton.WithCallbackData("🧘 Meditar", "rpg_action_meditate")
                },
                // INFORMACIÓN Y PROGRESO
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📊 Stats", "rpg_stats"),
                    InlineKeyboardButton.WithCallbackData("🎒 Inventario", "rpg_inventory"),
                    InlineKeyboardButton.WithCallbackData("🏪 Tienda", "rpg_shop")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🌟 Progreso", "rpg_progress"),
                    InlineKeyboardButton.WithCallbackData("💎 Pasivas", "rpg_passives"),
                    InlineKeyboardButton.WithCallbackData("📈 Counters", "rpg_counters")
                },
                // HABILIDADES Y COMBATE
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✨ Skills", "rpg_skills"),
                    InlineKeyboardButton.WithCallbackData("🎯 Combos", "rpg_combo_skills"),
                    InlineKeyboardButton.WithCallbackData("🛡️ Entrenar", "rpg_train")
                },
                // UTILIDADES
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("💬 Chat IA", "rpg_ai_chat"),
                    InlineKeyboardButton.WithCallbackData("⚙️ Opciones", "rpg_options"),
                    InlineKeyboardButton.WithCallbackData("🏠 Salir", "start")
                }
            });
        }
        
        public InlineKeyboardMarkup GetCombatKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Atacar", "rpg_combat_attack"),
                    InlineKeyboardButton.WithCallbackData("�️ Defender", "rpg_combat_defend"),
                    InlineKeyboardButton.WithCallbackData("🔮 Magia", "rpg_combat_magic")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✨ Skills", "rpg_combat_skills"),
                    InlineKeyboardButton.WithCallbackData("🐾 Mascotas", "rpg_combat_pets"),
                    InlineKeyboardButton.WithCallbackData("🧪 Ítems", "rpg_combat_item")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("👁️ Observar", "rpg_combat_observe"),
                    InlineKeyboardButton.WithCallbackData("💬 Consulta", "rpg_combat_ai"),
                    InlineKeyboardButton.WithCallbackData("🏃 Huir", "rpg_combat_flee")
                }
            });
        }
        
        public async Task ShowCharacterCreation(ITelegramBotClient bot, long chatId, CancellationToken ct)
        {
            var text = @"✨ **CREACIÓN DE PERSONAJE**

¿Cuál es tu nombre, héroe?

Escribe tu nombre y te guiaré en la elección de tu clase.

_(Escribe cualquier nombre entre 3-20 caracteres)_";
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_back_welcome")
                }
            });
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        public async Task ShowClassSelection(ITelegramBotClient bot, long chatId, string playerName, CancellationToken ct)
        {
            var text = $@"⚔️ **ELIGE TU CLASE**

Bienvenido, **{playerName}**. Elige tu camino:

⚔️ **Guerrero**
   • Alta vida y defensa
   • Especialista en combate cuerpo a cuerpo
   • Ideal para principiantes
   
🔮 **Mago**
   • Alta energía e inteligencia
   • Poderes mágicos devastadores
   • Requiere estrategia
   
🗡️ **Ladrón**
   • Alta destreza y críticos
   • Ataques rápidos y evasión
   • Alto riesgo, alta recompensa
   
✨ **Clérigo**
   • Balance entre combate y curación
   • Soporte y supervivencia
   • Versátil y resistente

¿Qué clase eliges?";
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Guerrero", $"rpg_class_warrior:{playerName}"),
                    InlineKeyboardButton.WithCallbackData("🔮 Mago", $"rpg_class_mage:{playerName}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗡️ Ladrón", $"rpg_class_rogue:{playerName}"),
                    InlineKeyboardButton.WithCallbackData("✨ Clérigo", $"rpg_class_cleric:{playerName}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔙 Cambiar Nombre", "rpg_new_game")
                }
            });
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        public async Task ShowStats(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var classEmoji = player.Class switch
            {
                CharacterClass.Warrior => "⚔️",
                CharacterClass.Mage => "🔮",
                CharacterClass.Rogue => "🗡️",
                CharacterClass.Cleric => "✨",
                _ => "👤"
            };
            
            var weaponInfo = player.EquippedWeapon != null 
                ? $"{player.EquippedWeapon.Emoji} {player.EquippedWeapon.Name} (+{player.EquippedWeapon.AttackBonus})"
                : "❌ Sin arma";
                
            var armorInfo = player.EquippedArmor != null
                ? $"{player.EquippedArmor.Emoji} {player.EquippedArmor.Name} (+{player.EquippedArmor.DefenseBonus})"
                : "❌ Sin armadura";
            
            var text = $@"📊 **ESTADÍSTICAS**

{classEmoji} **{player.Name}**
🎖️ Nivel: {player.Level}
⭐ XP: {player.XP}/{player.XPNeeded}

**Atributos:**
💪 Fuerza: {player.Strength}
🧠 Inteligencia: {player.Intelligence}
🎯 Destreza: {player.Dexterity}

**Combate:**
⚔️ Ataque Total: {player.TotalAttack}
🛡️ Defensa Total: {player.TotalDefense}
❤️ Vida: {player.HP}/{player.MaxHP}
⚡ Energía: {player.Energy}/{player.MaxEnergy}

**Equipamiento:**
🗡️ Arma: {weaponInfo}
🛡️ Armadura: {armorInfo}

**Recursos:**
💰 Oro: {player.Gold}
🎒 Inventario: {player.Inventory.Count}/20 items

📍 Ubicación: {player.CurrentLocation}
🕐 Última acción: {GetTimeAgo(player.LastActionTime)}";
            
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔙 Volver", "rpg_main")
                }
            });
            
            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        
        private string GetXPBar(RpgPlayer player)
        {
            // Asegurar que XP no sea negativo para la barra
            var currentXP = Math.Max(0, player.XP);
            var percentage = (double)currentXP / player.XPNeeded;
            percentage = Math.Clamp(percentage, 0.0, 1.0);
            
            var barLength = 10;
            var filled = (int)(percentage * barLength);
            var empty = Math.Max(0, barLength - filled);
            
            var bar = "⭐ " + new string('█', filled) + new string('░', empty);
            return $"{bar} {currentXP}/{player.XPNeeded} XP";
        }
        
        private string GetTimeAgo(DateTime time)
        {
            var diff = DateTime.UtcNow - time;
            
            if (diff.TotalMinutes < 1) return "hace un momento";
            if (diff.TotalMinutes < 60) return $"hace {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"hace {(int)diff.TotalHours}h";
            return $"hace {(int)diff.TotalDays} días";
        }

        /// <summary>
        /// Muestra el menú de opciones del juego (backup, importar, etc)
        /// </summary>
        public async Task ShowOptionsMenu(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct)
        {
            var text = $@"⚙️ **OPCIONES DE PERSONAJE**

👤 **{player.Name}** - {player.Class} Nv.{player.Level}
💰 Oro: {player.Gold}
❤️ HP: {player.HP}/{player.MaxHP}

**Gestión de Datos:**

💾 **Exportar Personaje**
   Descarga tu personaje en formato JSON
   Úsalo para hacer backup o compartir

📥 **Importar Personaje**
   Restaura un personaje desde un backup
   ⚠️ Reemplaza tu personaje actual

🗂️ **Descargar Logs**
   Descarga todos los logs de prueba
   Perfecto para auditoría y análisis

**Cuenta:**

🗑️ **Borrar Personaje**
   ⚠️ Esta acción es PERMANENTE

🏠 **Volver**
   Regresa al menú principal";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("💾 Exportar", "rpg_export_character"),
                    InlineKeyboardButton.WithCallbackData("📥 Importar", "rpg_import_character")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗂️ Logs", "rpg_download_logs")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗑️ Borrar", "rpg_confirm_delete")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Volver", "rpg_main")
                }
            });

            await bot.SendMessage(
                chatId,
                text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
    }
}
