using System;
using System.Collections.Generic;
using System.Linq;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio principal para la gestión de mazmorras
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public class DungeonService
    {
        private readonly RpgService _rpgService;
        private readonly Random _random = new();
        
        public DungeonService(RpgService rpgService)
        {
            _rpgService = rpgService;
        }
        
        /// <summary>
        /// Genera una nueva mazmorra basada en una plantilla
        /// </summary>
        public Dungeon GenerateDungeon(string templateId)
        {
            var template = DungeonDatabase.GetDungeonTemplate(templateId);
            if (template == null)
            {
                throw new ArgumentException($"Plantilla de mazmorra no encontrada: {templateId}");
            }
            
            var totalFloors = DungeonDatabase.GetTotalFloorsForDifficulty(template.Difficulty);
            var dungeon = new Dungeon
            {
                Id = $"{templateId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Name = template.Name,
                Description = template.Description,
                Emoji = template.Emoji,
                MinLevel = template.MinLevel,
                Difficulty = template.Difficulty,
                RequiresKey = template.RequiresKey,
                RequiredKeyId = template.Difficulty.ToString().ToLower(),
                TotalFloors = totalFloors,
                CurrentFloor = 0,
                Floors = new List<DungeonFloor>(),
                IsActive = false,
                StartTime = null,
                FinalRewards = GenerateFinalRewards(template.Difficulty)
            };
            
            // Generar pisos
            for (int i = 1; i <= totalFloors; i++)
            {
                dungeon.Floors.Add(GenerateFloor(i, totalFloors, template));
            }
            
            return dungeon;
        }
        
        /// <summary>
        /// Genera un piso individual
        /// </summary>
        private DungeonFloor GenerateFloor(int floorNumber, int totalFloors, DungeonDatabase.DungeonTemplate template)
        {
            var floor = new DungeonFloor
            {
                FloorNumber = floorNumber,
                IsCleared = false,
                IsCurrentFloor = floorNumber == 1
            };
            
            // Boss floor cada 5 pisos (5, 10, 15, 20, 25)
            if (floorNumber % 5 == 0)
            {
                floor.Type = FloorType.Boss;
                floor.Enemy = GenerateBossEnemy(template, floorNumber);
                floor.Description = $"💀 Piso {floorNumber}: ¡Jefe Final!";
                floor.Reward = GenerateBossReward(template.Difficulty);
            }
            else
            {
                // Distribución: Combat 60%, Elite 20%, Rest 10%, Trap 10%
                double roll = _random.NextDouble();
                
                if (roll < 0.60) // 60% Combat
                {
                    floor.Type = FloorType.Combat;
                    floor.EnemyCount = _random.Next(1, 4); // 1-3 enemigos
                    floor.Enemy = GenerateNormalEnemy(template, floorNumber);
                    floor.Description = $"🗡️ Piso {floorNumber}: Enemigos comunes ({floor.EnemyCount})";
                    floor.Reward = GenerateNormalReward(template.Difficulty);
                }
                else if (roll < 0.80) // 20% Elite
                {
                    floor.Type = FloorType.Elite;
                    floor.Enemy = GenerateEliteEnemy(template, floorNumber);
                    floor.Description = $"⚔️ Piso {floorNumber}: Enemigo Élite";
                    floor.Reward = GenerateEliteReward(template.Difficulty);
                }
                else if (roll < 0.90) // 10% Rest
                {
                    floor.Type = FloorType.Rest;
                    floor.RestorePercentage = 50;
                    floor.Description = $"😴 Piso {floorNumber}: Sala de Descanso";
                    floor.Reward = null;
                }
                else // 10% Trap
                {
                    floor.Type = FloorType.Trap;
                    floor.Trap = GenerateTrap(template.Difficulty);
                    floor.Description = $"🪤 Piso {floorNumber}: Trampa";
                    floor.Reward = null;
                }
            }
            
            return floor;
        }
        
        /// <summary>
        /// Genera un enemigo normal para combat floor
        /// </summary>
        private RpgEnemy GenerateNormalEnemy(DungeonDatabase.DungeonTemplate template, int floorNumber)
        {
            // TODO: Integrar con EnemyDatabase para obtener enemigos reales
            // Por ahora, generar enemigo genérico
            var baseLevel = template.MinLevel + (floorNumber / 3);
            
            return new RpgEnemy
            {
                Name = $"Enemigo Piso {floorNumber}",
                Level = baseLevel,
                HP = 50 + (baseLevel * 10),
                MaxHP = 50 + (baseLevel * 10),
                Attack = 10 + (baseLevel * 2),
                Defense = 5 + baseLevel,
                XPReward = 30 + (baseLevel * 5),
                GoldReward = 20 + (baseLevel * 3),
                Difficulty = EnemyDifficulty.Medium
            };
        }
        
        /// <summary>
        /// Genera un enemigo élite (HP +50%, Stats +30%)
        /// </summary>
        private RpgEnemy GenerateEliteEnemy(DungeonDatabase.DungeonTemplate template, int floorNumber)
        {
            var normal = GenerateNormalEnemy(template, floorNumber);
            
            normal.Name = $"Élite - {normal.Name}";
            normal.HP = (int)(normal.HP * 1.5);
            normal.MaxHP = (int)(normal.MaxHP * 1.5);
            normal.Attack = (int)(normal.Attack * 1.3);
            normal.Defense = (int)(normal.Defense * 1.3);
            normal.XPReward = (int)(normal.XPReward * 1.5);
            normal.GoldReward = (int)(normal.GoldReward * 2);
            normal.Difficulty = EnemyDifficulty.Elite;
            
            return normal;
        }
        
        /// <summary>
        /// Genera un jefe para boss floor
        /// </summary>
        private RpgEnemy GenerateBossEnemy(DungeonDatabase.DungeonTemplate template, int floorNumber)
        {
            var baseLevel = template.MinLevel + (floorNumber / 2);
            
            return new RpgEnemy
            {
                Name = $"Jefe del Piso {floorNumber}",
                Level = baseLevel,
                HP = 150 + (baseLevel * 25),
                MaxHP = 150 + (baseLevel * 25),
                Attack = 20 + (baseLevel * 4),
                Defense = 15 + (baseLevel * 2),
                XPReward = 100 + (baseLevel * 20),
                GoldReward = 100 + (baseLevel * 15),
                Difficulty = EnemyDifficulty.Boss
            };
        }
        
        /// <summary>
        /// Genera recompensa para piso normal
        /// </summary>
        private FloorReward GenerateNormalReward(DungeonDifficulty difficulty)
        {
            var goldMultiplier = GetDifficultyMultiplier(difficulty);
            
            return new FloorReward
            {
                Gold = _random.Next(10, 30) * goldMultiplier,
                XP = _random.Next(20, 40) * goldMultiplier,
                HasSkillPoint = false
            };
        }
        
        /// <summary>
        /// Genera recompensa para piso élite
        /// </summary>
        private FloorReward GenerateEliteReward(DungeonDifficulty difficulty)
        {
            var goldMultiplier = GetDifficultyMultiplier(difficulty);
            
            return new FloorReward
            {
                Gold = _random.Next(30, 60) * goldMultiplier,
                XP = _random.Next(40, 80) * goldMultiplier,
                HasSkillPoint = false
            };
        }
        
        /// <summary>
        /// Genera recompensa para boss floor
        /// </summary>
        private FloorReward GenerateBossReward(DungeonDifficulty difficulty)
        {
            var goldMultiplier = GetDifficultyMultiplier(difficulty);
            
            return new FloorReward
            {
                Gold = _random.Next(100, 200) * goldMultiplier,
                XP = _random.Next(150, 300) * goldMultiplier,
                HasSkillPoint = true
            };
        }
        
        /// <summary>
        /// Genera recompensas finales de la mazmorra
        /// </summary>
        private DungeonRewards GenerateFinalRewards(DungeonDifficulty difficulty)
        {
            var multiplier = GetDifficultyMultiplier(difficulty);
            
            return new DungeonRewards
            {
                Gold = _random.Next(500, 1000) * multiplier,
                XP = _random.Next(1000, 2000) * multiplier,
                SkillPoints = difficulty switch
                {
                    DungeonDifficulty.Common => 1,
                    DungeonDifficulty.Uncommon => 2,
                    DungeonDifficulty.Rare => 3,
                    DungeonDifficulty.Epic => 4,
                    DungeonDifficulty.Legendary => 5,
                    _ => 1
                },
                PerfectionBonus = 0.20 // 20% bonus si perfect run
            };
        }
        
        /// <summary>
        /// Genera una trampa
        /// </summary>
        private TrapEvent GenerateTrap(DungeonDifficulty difficulty)
        {
            var trapTypes = Enum.GetValues<TrapType>();
            var randomTrap = trapTypes[_random.Next(trapTypes.Length)];
            
            var damageMultiplier = GetDifficultyMultiplier(difficulty);
            
            return randomTrap switch
            {
                TrapType.SpikeTrap => new TrapEvent
                {
                    Name = "Trampa de Pinchos",
                    Description = "El suelo se abre revelando pinchos afilados!",
                    Type = TrapType.SpikeTrap,
                    Damage = _random.Next(20, 40) * damageMultiplier,
                    CanAvoid = true,
                    AvoidDC = 12 + (damageMultiplier * 2)
                },
                TrapType.PoisonGas => new TrapEvent
                {
                    Name = "Gas Venenoso",
                    Description = "Una nube de gas tóxico llena la habitación!",
                    Type = TrapType.PoisonGas,
                    Damage = _random.Next(15, 30) * damageMultiplier,
                    StaminaDrain = _random.Next(10, 20) * damageMultiplier,
                    CanAvoid = true,
                    AvoidDC = 15 + (damageMultiplier * 2)
                },
                TrapType.ManaDrain => new TrapEvent
                {
                    Name = "Sifón de Mana",
                    Description = "Runas antiguas absorben tu energía mágica!",
                    Type = TrapType.ManaDrain,
                    ManaDrain = _random.Next(30, 60) * damageMultiplier,
                    CanAvoid = true,
                    AvoidDC = 14 + (damageMultiplier * 2)
                },
                _ => new TrapEvent
                {
                    Name = "Trampa Desconocida",
                    Description = "Algo malo está por suceder...",
                    Type = randomTrap,
                    Damage = _random.Next(10, 30) * damageMultiplier,
                    CanAvoid = true,
                    AvoidDC = 13 + (damageMultiplier * 2)
                }
            };
        }
        
        /// <summary>
        /// Obtiene multiplicador según dificultad
        /// </summary>
        private int GetDifficultyMultiplier(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => 1,
                DungeonDifficulty.Uncommon => 2,
                DungeonDifficulty.Rare => 3,
                DungeonDifficulty.Epic => 5,
                DungeonDifficulty.Legendary => 8,
                _ => 1
            };
        }
        
        /// <summary>
        /// Inicia una mazmorra para un jugador
        /// </summary>
        public bool StartDungeon(RpgPlayer player, string dungeonTemplateId, bool consumeKey = true)
        {
            // Validar que el jugador no esté en combate o en otra mazmorra
            if (player.IsInCombat || player.CurrentDungeon != null)
            {
                return false;
            }
            
            var template = DungeonDatabase.GetDungeonTemplate(dungeonTemplateId);
            if (template == null) return false;
            
            // Validar nivel mínimo
            if (player.Level < template.MinLevel)
            {
                return false;
            }
            
            // Validar y consumir llave si es necesaria
            if (template.RequiresKey && consumeKey)
            {
                var key = player.DungeonKeys.FirstOrDefault(k => 
                    k.UnlocksDifficulty == template.Difficulty && !k.IsConsumed);
                    
                if (key == null) return false;
                
                key.IsConsumed = true;
            }
            
            // Generar y activar mazmorra
            var dungeon = GenerateDungeon(dungeonTemplateId);
            dungeon.IsActive = true;
            dungeon.StartTime = DateTime.UtcNow;
            dungeon.CurrentFloor = 1;
            dungeon.Floors[0].IsCurrentFloor = true;
            
            player.CurrentDungeon = dungeon;
            StateManager.ForceState(player, GameState.InDungeon, dungeonTemplateId); // FSM
            _rpgService.SavePlayer(player);
            
            return true;
        }
        
        /// <summary>
        /// Avanza al siguiente piso
        /// </summary>
        public bool AdvanceToNextFloor(RpgPlayer player)
        {
            if (player.CurrentDungeon == null || !player.CurrentDungeon.IsActive)
            {
                return false;
            }
            
            var currentFloor = player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1];
            
            // Verificar que el piso actual esté completado
            if (!currentFloor.IsCleared)
            {
                return false;
            }
            
            // Verificar si es el último piso
            if (player.CurrentDungeon.CurrentFloor >= player.CurrentDungeon.TotalFloors)
            {
                CompleteDungeon(player);
                return false;
            }
            
            // Avanzar al siguiente piso
            currentFloor.IsCurrentFloor = false;
            player.CurrentDungeon.CurrentFloor++;
            player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1].IsCurrentFloor = true;
            
            _rpgService.SavePlayer(player);
            return true;
        }
        
        /// <summary>
        /// Completa la mazmorra y otorga recompensas finales
        /// </summary>
        public void CompleteDungeon(RpgPlayer player)
        {
            if (player.CurrentDungeon == null) return;
            
            var dungeon = player.CurrentDungeon;
            dungeon.IsCompleted = true;
            dungeon.IsActive = false;
            dungeon.CompletionTime = DateTime.UtcNow;
            
            // Aplicar recompensas finales
            player.Gold += dungeon.FinalRewards.Gold;
            _rpgService.AddXP(player, dungeon.FinalRewards.XP);
            // TODO: Agregar SkillPoints a RpgPlayer cuando se implemente sistema de skills
            // player.SkillPoints += dungeon.FinalRewards.SkillPoints;
            
            // Bonus de perfección si no perdió ningún piso
            if (dungeon.IsPerfectRun)
            {
                var bonusXP = (int)(dungeon.FinalRewards.XP * dungeon.FinalRewards.PerfectionBonus);
                _rpgService.AddXP(player, bonusXP);
            }
            
            // Actualizar estadísticas
            player.TotalDungeonsCompleted++;
            player.TotalDungeonFloorsCleaned += dungeon.TotalFloors;

            // ═══ FASE 9: Tracking de objetivos de misión (explorar mazmorra) ═══
            var questNotifs = QuestService.UpdateExploreObjective(player, dungeon.Id);
            // Las notificaciones se guardan pero no se muestran aquí (el UI las recupera del próximo mensaje)
            
            if (!player.DungeonsCompleted.ContainsKey(dungeon.Id))
            {
                player.DungeonsCompleted[dungeon.Id] = 0;
            }
            player.DungeonsCompleted[dungeon.Id]++;
            
            // Limpiar mazmorra actual
            player.CurrentDungeon = null;
            StateManager.ForceState(player, GameState.Idle, "dungeon_completed"); // FSM
            
            _rpgService.SavePlayer(player);
        }
        
        /// <summary>
        /// Abandona la mazmorra (pierde todo)
        /// </summary>
        public void AbandonDungeon(RpgPlayer player)
        {
            if (player.CurrentDungeon == null) return;
            
            player.CurrentDungeon = null;
            player.IsInCombat = false;
            player.CurrentEnemy = null;
            StateManager.ForceState(player, GameState.Idle, "dungeon_abandoned"); // FSM
            
            _rpgService.SavePlayer(player);
        }
    }
}
