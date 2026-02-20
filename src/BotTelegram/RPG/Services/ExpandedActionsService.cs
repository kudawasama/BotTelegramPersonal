using Telegram.Bot;
using Telegram.Bot.Types;
using System;
using System.Collections.Generic;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio para las nuevas acciones expandidas (Aventura Riesgosa, Meditación, etc.)
    /// Conecta con ActionTrackerService para registrar acciones y desbloquear skills/pasivas/clases
    /// </summary>
    public class ExpandedActionsService
    {
        private readonly RpgService _rpgService;
        private readonly ActionTrackerService _trackerService;

        public ExpandedActionsService(RpgService rpgService)
        {
            _rpgService = rpgService;
            _trackerService = new ActionTrackerService(rpgService);
        }

        // ═════════════════════════════════════════════════════════════════════
        // AVENTURA: Nuevas acciones de exploración y aventura
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Aventura Riesgosa: Busca el camino peligroso con posible recompensa mayor
        /// Consume energía, reconocimiento de riesgos de combat
        /// </summary>
        public ActionResult ExecuteAdventureRisky(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 15))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 15)" };

            _rpgService.ConsumeEnergy(player, 15);

            var random = new Random();
            var isSuccessful = random.Next(0, 100) < 75; // 75% chance of success
            
            int xpGained = 0, goldGained = 0, damageReceived = 0;

            if (isSuccessful)
            {
                // Recompensa por aventura exitosa
                xpGained = 30 + random.Next(0, 20);
                goldGained = 50 + random.Next(0, 30);
                _rpgService.AddXP(player, xpGained);
                player.Gold += goldGained;
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🎯 **AVENTURA RIESGOSA EXITOSA**\n\n" +
                              $"Encontraste un camino oculto lleno de tesoros!\n\n" +
                              $"✨ +{xpGained} XP\n" +
                              $"💰 +{goldGained} Oro\n",
                    XpGained = xpGained,
                    GoldGained = goldGained
                };

                _trackerService.TrackAction(player, "adventure_risky");
                return result;
            }
            else
            {
                // Encuentro peligroso
                damageReceived = 20 + random.Next(0, 15);
                player.HP = Math.Max(1, player.HP - damageReceived);
                
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"⚠️ **¡ENCUENTRO PELIGROSO!**\n\n" +
                              $"Encontraste una criatura hostil!\n\n" +
                              $"💔 -{damageReceived} HP\n" +
                              $"Escapaste con dificultad...\n",
                    DamageTaken = damageReceived
                };

                _trackerService.TrackAction(player, "adventure_risky_failed");
                return result;
            }
        }

        /// <summary>
        /// Aventura Furtiva: Movimiento sigiloso para evitar combates, búsqueda de secretos
        /// </summary>
        public ActionResult ExecuteAdventureStealth(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 12))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 12)" };

            _rpgService.ConsumeEnergy(player, 12);

            var random = new Random();
            var steathBonusFromDex = player.Dexterity / 10; // Bonus por destreza
            var isSuccessful = random.Next(0, 100) < (60 + steathBonusFromDex);

            if (isSuccessful)
            {
                var secretGold = 30 + random.Next(0, 20);
                player.Gold += secretGold;
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🥷 **MOVIMIENTO FURTIVO EXITOSO**\n\n" +
                              $"Te moviste silenciosamente por la zona.\n" +
                              $"Descubriste un escondite secreto!\n\n" +
                              $"💰 +{secretGold} Oro (botín)\n",
                    GoldGained = secretGold
                };

                _trackerService.TrackAction(player, "adventure_stealth");
                return result;
            }
            else
            {
                var goldLost = 15;
                player.Gold = Math.Max(0, player.Gold - goldLost);
                
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"🥷 **¡DESCUBIERTO!**\n\n" +
                              $"Un guardia te detectó!\n" +
                              $"Tuviste que sobornarle para escapar.\n\n" +
                              $"💰 -{goldLost} Oro (soborno)\n",
                    GoldLost = goldLost
                };

                _trackerService.TrackAction(player, "adventure_stealth_failed");
                return result;
            }
        }

        /// <summary>
        /// Aventura Social: Interacción con NPCs, obtener información y recompensas
        /// </summary>
        public ActionResult ExecuteAdventureSocial(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 10))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 10)" };

            _rpgService.ConsumeEnergy(player, 10);

            var random = new Random();
            var charismaBonus = player.Charisma / 15; // Bonus por carisma
            var isSuccessful = random.Next(0, 100) < (70 + charismaBonus);

            if (isSuccessful)
            {
                var rewardXp = 20 + random.Next(0, 15);
                var rewardGold = 40 + random.Next(0, 25);
                _rpgService.AddXP(player, rewardXp);
                player.Gold += rewardGold;
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🤝 **INTERACCIÓN SOCIAL EXITOSA**\n\n" +
                              $"Hablaste con un NPC interesante!\n" +
                              $"Te compartió información valiosa.\n\n" +
                              $"✨ +{rewardXp} XP\n" +
                              $"💰 +{rewardGold} Oro\n",
                    XpGained = rewardXp,
                    GoldGained = rewardGold
                };

                _trackerService.TrackAction(player, "adventure_social");
                return result;
            }
            else
            {
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"🤝 **¡CONVERSACIÓN DESAGRADABLE!**\n\n" +
                              $"El NPC se ofendió por tus palabras.\n" +
                              $"Mejor será que te vayas...\n"
                };

                _trackerService.TrackAction(player, "adventure_social_failed");
                return result;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // PERSONAJE: Entrenar mente, cuerpo y estudio
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Entrenar Mente: Aumenta inteligencia y sabiduría
        /// </summary>
        public ActionResult ExecuteTrainMind(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 18))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 18)" };

            _rpgService.ConsumeEnergy(player, 18);

            var random = new Random();
            var statGain = 1 + random.Next(0, 2); // +1 o +2 a Intelligence/Wisdom
            
            player.Intelligence += statGain;
            player.Wisdom += statGain;
            var xpGain = 25 + random.Next(0, 10);
            _rpgService.AddXP(player, xpGain);

            var result = new ActionResult
            {
                Success = true,
                Message = $"🧠 **ENTRENAR MENTE**\n\n" +
                          $"Practicaste ejercicios mentales complejos!\n\n" +
                          $"📈 +{statGain} Inteligencia\n" +
                          $"📈 +{statGain} Sabiduría\n" +
                          $"✨ +{xpGain} XP\n",
                XpGained = xpGain
            };

            _trackerService.TrackAction(player, "train_mind");
            return result;
        }

        /// <summary>
        /// Entrenar Cuerpo: Aumenta fuerza y constitución
        /// </summary>
        public ActionResult ExecuteTrainBody(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 20))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 20)" };

            _rpgService.ConsumeEnergy(player, 20);

            var random = new Random();
            var statGain = 1 + random.Next(0, 2); // +1 o +2 a Strength/Constitution
            
            player.Strength += statGain;
            player.Constitution += statGain;
            var xpGain = 30 + random.Next(0, 15);
            _rpgService.AddXP(player, xpGain);
            
            // Bonus: restaurar un poco de HP después del entrenamiento
            var hpRestored = (int)(player.MaxHP * 0.1);
            player.HP = Math.Min(player.MaxHP, player.HP + hpRestored);

            var result = new ActionResult
            {
                Success = true,
                Message = $"💪 **ENTRENAR CUERPO**\n\n" +
                          $"Realizaste ejercicios físicos intensos!\n\n" +
                          $"📈 +{statGain} Fuerza\n" +
                          $"📈 +{statGain} Constitución\n" +
                          $"❤️ +{hpRestored} HP\n" +
                          $"✨ +{xpGain} XP\n",
                XpGained = xpGain
            };

            _trackerService.TrackAction(player, "train_body");
            return result;
        }

        /// <summary>
        /// Estudiar: Aumenta destreza e inteligencia, desbloquea habilidades
        /// </summary>
        public ActionResult ExecuteStudy(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 16))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 16)" };

            _rpgService.ConsumeEnergy(player, 16);

            var random = new Random();
            var statGain = 1 + random.Next(0, 2);
            
            player.Intelligence += statGain;
            player.Dexterity += statGain;
            var xpGain = 28 + random.Next(0, 12);
            _rpgService.AddXP(player, xpGain);
            
            // Pequeña regeneración de mana por estudio
            var manaRestored = (int)(player.MaxMana * 0.15);
            player.Mana = Math.Min(player.MaxMana, player.Mana + manaRestored);

            var result = new ActionResult
            {
                Success = true,
                Message = $"✍️ **ESTUDIAR**\n\n" +
                          $"Estudiaste antiguos tomos de magia!\n\n" +
                          $"📈 +{statGain} Inteligencia\n" +
                          $"📈 +{statGain} Destreza\n" +
                          $"💙 +{manaRestored} Mana\n" +
                          $"✨ +{xpGain} XP\n",
                XpGained = xpGain
            };

            _trackerService.TrackAction(player, "study");
            return result;
        }

        // ═════════════════════════════════════════════════════════════════════
        // MUNDO: Meditación, Pesca, Investigación
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Meditación: Recuperar mana y recursos mentales
        /// </summary>
        public ActionResult ExecuteMeditate(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 12))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 12)" };

            _rpgService.ConsumeEnergy(player, 12);

            var manaRestored = (int)(player.MaxMana * 0.5);
            player.Mana = Math.Min(player.MaxMana, player.Mana + manaRestored);
            var xpGain = 15;
            _rpgService.AddXP(player, xpGain);

            var result = new ActionResult
            {
                Success = true,
                Message = $"🧘 **MEDITACIÓN**\n\n" +
                          $"Te sumergiste en profunda meditación...\n" +
                          $"Tu mente se llenó de claridad.\n\n" +
                          $"💙 +{manaRestored} Mana\n" +
                          $"✨ +{xpGain} XP\n",
                XpGained = xpGain
            };

            _trackerService.TrackAction(player, "deep_meditation");
            _trackerService.TrackAction(player, "meditation");
            return result;
        }

        /// <summary>
        /// Pesca: Obtener recursos y relajarse
        /// </summary>
        public ActionResult ExecuteFish(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 14))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 14)" };

            _rpgService.ConsumeEnergy(player, 14);

            var random = new Random();
            var isCatch = random.Next(0, 100) < 80; // 80% chance de atrapar algo

            if (isCatch)
            {
                var fishWorth = 30 + random.Next(0, 40);
                player.Gold += fishWorth;
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🎣 **¡ATRAPASTE UN PEZ!**\n\n" +
                              $"Una captura excelente hoy.\n\n" +
                              $"💰 +{fishWorth} Oro\n",
                    GoldGained = fishWorth
                };

                _trackerService.TrackAction(player, "fish_catch");
                return result;
            }
            else
            {
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"🎣 **Nada en el agua hoy...**\n\n" +
                              $"Los peces no pican.\n" +
                              $"Al menos fue relajante.\n"
                };

                _trackerService.TrackAction(player, "fish_action");
                return result;
            }
        }

        /// <summary>
        /// Investigar: Descubrir misterios y secretos
        /// </summary>
        public ActionResult ExecuteInvestigate(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 15))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 15)" };

            _rpgService.ConsumeEnergy(player, 15);

            var random = new Random();
            var wisdomBonus = player.Wisdom / 20;
            var isSuccessful = random.Next(0, 100) < (65 + wisdomBonus);

            if (isSuccessful)
            {
                var xpGain = 35 + random.Next(0, 20);
                var goldGain = 25 + random.Next(0, 15);
                _rpgService.AddXP(player, xpGain);
                player.Gold += goldGain;
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🔍 **¡DESCUBRIMIENTO!**\n\n" +
                              $"Encontraste pistas sobre un antiguo misterio.\n\n" +
                              $"✨ +{xpGain} XP\n" +
                              $"💰 +{goldGain} Oro\n",
                    XpGained = xpGain,
                    GoldGained = goldGain
                };

                _trackerService.TrackAction(player, "investigate");
                return result;
            }
            else
            {
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"🔍 **Investigación Infructuosa**\n\n" +
                              $"No encontraste nada interesante.\n"
                };

                _trackerService.TrackAction(player, "investigate_failed");
                return result;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // CIUDAD: Comercio, Diplomacia, Taberna
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Comercio: Negociar precios o encontrar ofertas
        /// </summary>
        public ActionResult ExecuteTrade(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 10))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 10)" };

            _rpgService.ConsumeEnergy(player, 10);

            var random = new Random();
            var charismaBonus = player.Charisma / 25;
            var isSuccessful = random.Next(0, 100) < (75 + charismaBonus);

            if (isSuccessful)
            {
                var discount = 10 + random.Next(0, 20); // 10-30% discount
                player.Gold += discount; // Simulación: ahorro de dinero
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"💰 **COMERCIO EXITOSO**\n\n" +
                              $"Negociaste un buen precio!\n\n" +
                              $"💰 +{discount} Oro (ahorrado)\n",
                    GoldGained = discount
                };

                _trackerService.TrackAction(player, "trade");
                return result;
            }
            else
            {
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"💰 **Comercio Fallido**\n\n" +
                              $"El vendedor no quiso negociar contigo.\n"
                };

                _trackerService.TrackAction(player, "trade_failed");
                return result;
            }
        }

        /// <summary>
        /// Diplomacia: Ganar influencia y conectar con NPCs importantes
        /// </summary>
        public ActionResult ExecuteDiplomacy(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 12))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 12)" };

            _rpgService.ConsumeEnergy(player, 12);

            var random = new Random();
            var charismaBonus = player.Charisma / 15;
            var isSuccessful = random.Next(0, 100) < (70 + charismaBonus);

            if (isSuccessful)
            {
                var xpGain = 25 + random.Next(0, 15);
                _rpgService.AddXP(player, xpGain);
                
                var result = new ActionResult
                {
                    Success = true,
                    Message = $"🤝 **¡DIPLOMACIA EXITOSA!**\n\n" +
                              $"Ganaste la simpatía de un noble importante.\n\n" +
                              $"✨ +{xpGain} XP\n",
                    XpGained = xpGain
                };

                _trackerService.TrackAction(player, "diplomacy");
                return result;
            }
            else
            {
                var result = new ActionResult
                {
                    Success = false,
                    Message = $"🤝 **¡DIPLOMACIA FALLIDA!**\n\n" +
                              $"Metiste la pata con el noble...\n"
                };

                _trackerService.TrackAction(player, "diplomacy_failed");
                return result;
            }
        }

        /// <summary>
        /// Taberna: Relajarse, conocer gente, obtener información
        /// </summary>
        public ActionResult ExecuteTavern(RpgPlayer player)
        {
            if (player == null)
                return new ActionResult { Success = false, Message = "Jugador no encontrado" };

            if (!_rpgService.CanPerformAction(player, 8))
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente energía (necesitas 8)" };

            if (player.Gold < 10)
                return new ActionResult { Success = false, Message = "❌ No tienes suficiente oro (necesitas 10)" };

            _rpgService.ConsumeEnergy(player, 8);
            player.Gold -= 10; // Costo de bebidas

            var random = new Random();
            var moodType = random.Next(0, 3);
            
            int xpGain = 10;
            int hpRestored = (int)(player.MaxHP * 0.2);
            player.HP = Math.Min(player.MaxHP, player.HP + hpRestored);
            _rpgService.AddXP(player, xpGain);

            var messages = new[]
            {
                "🍻 Te dijeron una broma hilarante mientras bebías.",
                "🎵 La música de la taberna te animó el día.",
                "🍺 Conociste a un aventurero con historias fascinantes."
            };

            var result = new ActionResult
            {
                Success = true,
                Message = $"🎪 **¡NOCHE EN LA TABERNA!**\n\n" +
                          $"{messages[moodType]}\n\n" +
                          $"❤️ +{hpRestored} HP (relajación)\n" +
                          $"✨ +{xpGain} XP\n" +
                          $"💰 -{10} Oro (bebidas)\n",
                XpGained = xpGain
            };

            _trackerService.TrackAction(player, "tavern");
            return result;
        }
    }

    /// <summary>
    /// Resultado de una acción expandida
    /// </summary>
    public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int XpGained { get; set; }
        public int GoldGained { get; set; }
        public int GoldLost { get; set; }
        public int DamageTaken { get; set; }
    }
}
