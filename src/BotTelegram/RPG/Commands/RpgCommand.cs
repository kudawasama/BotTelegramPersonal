using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

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
            
            var statusBar = $"❤️ {player.HP}/{player.MaxHP} | ⚡ {player.Energy}/{player.MaxEnergy}";
            var xpBar = GetXPBar(player);
            
            var text = $@"🎮 **MENÚ RPG**

{classEmoji} **{player.Name}** - {player.Class} Nv.{player.Level}
{statusBar}
{xpBar}
💰 {player.Gold} oro

📍 *{player.CurrentLocation}*

";
            
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
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Explorar", "rpg_explore"),
                    InlineKeyboardButton.WithCallbackData("🛡️ Entrenar", "rpg_train")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("😴 Descansar", "rpg_rest"),
                    InlineKeyboardButton.WithCallbackData("💼 Trabajar", "rpg_work")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📊 Stats", "rpg_stats"),
                    InlineKeyboardButton.WithCallbackData("🎒 Equipment", "rpg_equipment")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏪 Tienda", "rpg_shop")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✨ Skills", "rpg_skills"),
                    InlineKeyboardButton.WithCallbackData("📈 Counters", "rpg_counters")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🌟 Progreso", "rpg_progress"),
                    InlineKeyboardButton.WithCallbackData("💎 Pasivas", "rpg_passives")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🧘 Acciones", "rpg_actions"),
                    InlineKeyboardButton.WithCallbackData("💬 Chat IA", "rpg_ai_chat")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚙️ Opciones", "rpg_options"),
                    InlineKeyboardButton.WithCallbackData("🏠 Menú Bot", "start")
                }
            });
        }
        
        private InlineKeyboardMarkup GetCombatKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⚔️ Atacar", "rpg_combat_attack"),
                    InlineKeyboardButton.WithCallbackData("🛡️ Defender", "rpg_combat_defend")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🧪 Usar Ítem", "rpg_combat_item"),
                    InlineKeyboardButton.WithCallbackData("🏃 Huir", "rpg_combat_flee")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✨ Skills", "rpg_combat_skills")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("💬 Preguntar a IA", "rpg_combat_ai")
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
    }
}
