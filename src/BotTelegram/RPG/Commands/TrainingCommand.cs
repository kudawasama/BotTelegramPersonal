using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BotTelegram.RPG.Services;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Commands
{
    /// <summary>
    /// Comando /entrenar - Sistema de entrenamiento para mejorar atributos
    /// </summary>
    public class TrainingCommand
    {
        public async Task Execute(ITelegramBotClient bot, Message message, CancellationToken ct)
        {
            var chatId = message.Chat.Id;
            var service = new RpgService();
            var player = service.GetPlayer(chatId);

            if (player is null)
            {
                await bot.SendMessage(chatId, "❌ Primero debes crear tu personaje con /rpg", cancellationToken: ct);
                return;
            }

            await ShowTrainingMenu(bot, chatId, player, ct);
        }

        public static async Task ShowTrainingMenu(ITelegramBotClient bot, long chatId, RpgPlayer player, CancellationToken ct, int? editMessageId = null)
        {
            var baseCost = 100 + (player.Level * 50); // Costo base escalado por nivel
            
            var text = $@"🎯 **SALA DE ENTRENAMIENTO**
━━━━━━━━━━━━━━━━━━━━

💰 **Tu oro:** {player.Gold} monedas
📊 **Tu nivel:** {player.Level}

**Atributos Actuales:**
💪 Fuerza: {player.Strength}
🧠 Inteligencia: {player.Intelligence}
🎯 Destreza: {player.Dexterity}
❤️ Constitución: {player.Constitution}
🔮 Sabiduría: {player.Wisdom}
✨ Carisma: {player.Charisma}

**Entrenamiento disponible:**
Costo: **{baseCost} oro** por punto de atributo
_(El costo aumenta con tu nivel)_

Elige un atributo para mejorar:";

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[] 
                { 
                    InlineKeyboardButton.WithCallbackData($"💪 Fuerza (+1) - {baseCost}g", $"train_str:{baseCost}"),
                    InlineKeyboardButton.WithCallbackData($"🧠 Inteligencia (+1) - {baseCost}g", $"train_int:{baseCost}")
                },
                new[] 
                { 
                    InlineKeyboardButton.WithCallbackData($"🎯 Destreza (+1) - {baseCost}g", $"train_dex:{baseCost}"),
                    InlineKeyboardButton.WithCallbackData($"❤️ Constitución (+1) - {baseCost}g", $"train_con:{baseCost}")
                },
                new[] 
                { 
                    InlineKeyboardButton.WithCallbackData($"🔮 Sabiduría (+1) - {baseCost}g", $"train_wis:{baseCost}"),
                    InlineKeyboardButton.WithCallbackData($"✨ Carisma (+1) - {baseCost}g", $"train_cha:{baseCost}")
                },
                new[] { InlineKeyboardButton.WithCallbackData("🔙 Cerrar", "rpg_menu") }
            });

            if (editMessageId.HasValue)
            {
                try
                {
                    await bot.EditMessageText(chatId, editMessageId.Value, text,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: buttons, cancellationToken: ct);
                }
                catch { }
            }
            else
            {
                await bot.SendMessage(chatId, text,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: buttons, cancellationToken: ct);
            }
        }

        public static async Task HandleCallback(ITelegramBotClient bot, CallbackQuery cb, string data, CancellationToken ct)
        {
            var chatId = cb.Message!.Chat.Id;
            var messageId = cb.Message.MessageId;
            var service = new RpgService();
            var player = service.GetPlayer(chatId);

            Console.WriteLine($"[TrainingCommand] 🎯 HandleCallback ejecutado para: {data}");

            if (player is null)
            {
                Console.WriteLine($"[TrainingCommand] ❌ Jugador no encontrado para chatId: {chatId}");
                await bot.AnswerCallbackQuery(cb.Id, "❌ Error: jugador no encontrado", cancellationToken: ct);
                return;
            }

            // Parsear train_ATTR:COST
            var parts = data.Split(':');
            if (parts.Length != 2)
            {
                Console.WriteLine($"[TrainingCommand] ❌ Formato inválido: {data}");
                await bot.AnswerCallbackQuery(cb.Id, "❌ Formato inválido", cancellationToken: ct);
                return;
            }

            var attr = parts[0].Replace("train_", "");
            if (!int.TryParse(parts[1], out var cost))
            {
                Console.WriteLine($"[TrainingCommand] ❌ Costo inválido: {parts[1]}");
                await bot.AnswerCallbackQuery(cb.Id, "❌ Costo inválido", cancellationToken: ct);
                return;
            }

            Console.WriteLine($"[TrainingCommand] 📊 Atributo: {attr}, Costo: {cost}, Oro actual: {player.Gold}");

            // Verificar que tenga suficiente oro
            if (player.Gold < cost)
            {
                Console.WriteLine($"[TrainingCommand] ❌ Oro insuficiente: necesita {cost}, tiene {player.Gold}");
                await bot.AnswerCallbackQuery(cb.Id, $"❌ Necesitas {cost} oro (tienes {player.Gold})", showAlert: true, cancellationToken: ct);
                return;
            }

            // Aplicar entrenamiento
            string attrName = attr switch
            {
                "str" => "Fuerza",
                "int" => "Inteligencia",
                "dex" => "Destreza",
                "con" => "Constitución",
                "wis" => "Sabiduría",
                "cha" => "Carisma",
                _ => "Desconocido"
            };

            Console.WriteLine($"[TrainingCommand] 🎓 Entrenando {attrName}...");

            switch (attr)
            {
                case "str":
                    player.Strength++;
                    Console.WriteLine($"[TrainingCommand] ✅ Fuerza: {player.Strength - 1} → {player.Strength}");
                    break;
                case "int":
                    player.Intelligence++;
                    Console.WriteLine($"[TrainingCommand] ✅ Inteligencia: {player.Intelligence - 1} → {player.Intelligence}");
                    break;
                case "dex":
                    player.Dexterity++;
                    Console.WriteLine($"[TrainingCommand] ✅ Destreza: {player.Dexterity - 1} → {player.Dexterity}");
                    break;
                case "con":
                    var oldCon = player.Constitution;
                    var oldMaxHP = player.MaxHP;
                    player.Constitution++;
                    player.MaxHP += 5; // Bonus de HP por constitución
                    player.HP = Math.Min(player.HP + 5, player.MaxHP);
                    Console.WriteLine($"[TrainingCommand] ✅ Constitución: {oldCon} → {player.Constitution}, MaxHP: {oldMaxHP} → {player.MaxHP}");
                    break;
                case "wis":
                    player.Wisdom++;
                    Console.WriteLine($"[TrainingCommand] ✅ Sabiduría: {player.Wisdom - 1} → {player.Wisdom}");
                    break;
                case "cha":
                    player.Charisma++;
                    Console.WriteLine($"[TrainingCommand] ✅ Carisma: {player.Charisma - 1} → {player.Charisma}");
                    break;
                default:
                    Console.WriteLine($"[TrainingCommand] ❌ Atributo desconocido: {attr}");
                    break;
            }

            player.Gold -= cost;
            Console.WriteLine($"[TrainingCommand] 💰 Oro: {player.Gold + cost} → {player.Gold}");
            
            Console.WriteLine($"[TrainingCommand] 💾 Guardando jugador...");
            service.SavePlayer(player);
            Console.WriteLine($"[TrainingCommand] ✅ Jugador guardado exitosamente");

            await bot.AnswerCallbackQuery(cb.Id, $"✅ {attrName} +1 (-{cost} oro)", cancellationToken: ct);
            await ShowTrainingMenu(bot, chatId, player, ct, messageId);
        }
    }
}
