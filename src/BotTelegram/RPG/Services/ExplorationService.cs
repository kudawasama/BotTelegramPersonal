using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio de exploración y encuentros en el mundo
    /// </summary>
    public class ExplorationService
    {
        private readonly RpgService _rpgService;
        private readonly Random _random;
        
        public ExplorationService(RpgService rpgService)
        {
            _rpgService = rpgService;
            _random = new Random();
        }
        
        /// <summary>
        /// Explora la zona actual y genera un encuentro
        /// </summary>
        public ExplorationResult Explore(RpgPlayer player)
        {
            var zone = RegionDatabase.GetZone(player.CurrentZone);
            if (zone == null)
            {
                return new ExplorationResult
                {
                    Type = ExplorationResultType.Error,
                    Message = "❌ Zona no encontrada. Contacta a un administrador."
                };
            }
            
            // Zonas seguras no tienen encuentros
            if (zone.IsSafeZone)
            {
                return new ExplorationResult
                {
                    Type = ExplorationResultType.Nothing,
                    Message = $"🏘️ Estás en {zone.Name}, una zona segura.\n\n" +
                             "No hay enemigos aquí. Viaja a otra zona para explorar."
                };
            }
            
            // Determinar tipo de encuentro basado en tasa
            var roll = _random.NextDouble();
            
            // Encuentro de combate (según encounter rate de zona)
            if (roll <= zone.EncounterRate)
            {
                return GenerateCombatEncounter(player, zone);
            }
            
            // 10% de encontrar zona conectada nueva
            if (roll <= zone.EncounterRate + 0.10)
            {
                return GenerateZoneDiscovery(player, zone);
            }
            
            // 5% de encontrar recursos
            if (roll <= zone.EncounterRate + 0.15)
            {
                return GenerateResourceFind(player, zone);
            }
            
            // Nada encontrado
            return new ExplorationResult
            {
                Type = ExplorationResultType.Nothing,
                Message = $"🌄 Exploraste {zone.Name}...\n\n" +
                         "No encontraste nada interesante esta vez.\n" +
                         "💡 *Tip: Diferentes zonas tienen diferentes tasas de encuentro*"
            };
        }
        
        /// <summary>
        /// Genera un encuentro de combate
        /// </summary>
        private ExplorationResult GenerateCombatEncounter(RpgPlayer player, GameZone zone)
        {
            if (zone.EnemyPool.Count == 0)
            {
                return new ExplorationResult
                {
                    Type = ExplorationResultType.Nothing,
                    Message = "⚠️ Esta zona no tiene enemigos configurados."
                };
            }
            
            // Determinar dificultad basada en nivel de zona
            EnemyDifficulty difficulty;
            if (zone.MinEnemyLevel <= 5)
                difficulty = EnemyDifficulty.Easy;
            else if (zone.MinEnemyLevel <= 15)
                difficulty = EnemyDifficulty.Medium;
            else if (zone.MinEnemyLevel <= 30)
                difficulty = EnemyDifficulty.Hard;
            else
                difficulty = EnemyDifficulty.Elite;
            
            // Generar nivel de enemigo dentro del rango de la zona
            var targetLevel = _random.Next(zone.MinEnemyLevel, zone.MaxEnemyLevel + 1);
            
            // Crear enemigo usando RpgService
            var enemy = _rpgService.GenerateEnemy(targetLevel, difficulty);
            
            if (enemy == null)
            {
                return new ExplorationResult
                {
                    Type = ExplorationResultType.Nothing,
                    Message = "⚠️ Error al generar enemigo."
                };
            }
            
            return new ExplorationResult
            {
                Type = ExplorationResultType.Combat,
                Enemy = enemy,
                Message = $"⚔️ **¡ENEMIGO ENCONTRADO!**\n\n" +
                         $"{enemy.Emoji} **{enemy.Name}** (Nivel {enemy.Level})\n" +
                         $"❤️ {enemy.HP}/{enemy.MaxHP} HP\n" +
                         $"⚔️ Ataque: {enemy.Attack}\n" +
                         $"🛡️ Defensa: {enemy.PhysicalDefense}\n\n" +
                         $"📍 Ubicación: {zone.Name}"
            };
        }
        
        /// <summary>
        /// Genera descubrimiento de nueva zona
        /// </summary>
        private ExplorationResult GenerateZoneDiscovery(RpgPlayer player, GameZone zone)
        {
            // Buscar zonas conectadas no desbloqueadas
            var lockedZones = zone.ConnectedZones
                .Where(zoneId => !player.UnlockedZones.Contains(zoneId))
                .ToList();
            
            if (lockedZones.Count == 0)
            {
                // Ya tiene todas las zonas conectadas
                return GenerateResourceFind(player, zone);
            }
            
            // Desbloquear zona random
            var newZoneId = lockedZones[_random.Next(lockedZones.Count)];
            var newZone = RegionDatabase.GetZone(newZoneId);
            
            if (newZone == null)
            {
                return GenerateResourceFind(player, zone);
            }
            
            // Desbloquear zona
            RegionDatabase.UnlockZone(player, newZoneId);
            _rpgService.SavePlayer(player);
            
            return new ExplorationResult
            {
                Type = ExplorationResultType.ZoneDiscovered,
                Message = $"🗺️ **¡NUEVA ZONA DESCUBIERTA!**\n\n" +
                         $"{newZone.Emoji} **{newZone.Name}**\n" +
                         $"📖 {newZone.Description}\n\n" +
                         $"📊 Nivel de enemigos: {newZone.MinEnemyLevel}-{newZone.MaxEnemyLevel}\n" +
                         $"⚔️ Tasa de encuentro: {newZone.EncounterRate * 100:F0}%\n\n" +
                         $"💡 Usa `/travel {newZone.Name}` para viajar allí"
            };
        }
        
        /// <summary>
        /// Genera hallazgo de recursos
        /// </summary>
        private ExplorationResult GenerateResourceFind(RpgPlayer player, GameZone zone)
        {
            var roll = _random.Next(1, 4);
            
            switch (roll)
            {
                case 1: // Oro
                    var goldAmount = _random.Next(10, 50) * player.Level;
                    player.Gold += goldAmount;
                    player.TotalGoldEarned += goldAmount;
                    _rpgService.SavePlayer(player);
                    
                    return new ExplorationResult
                    {
                        Type = ExplorationResultType.Treasure,
                        Message = $"💰 **¡TESORO ENCONTRADO!**\n\n" +
                                 $"Encontraste un cofre oculto en {zone.Name}.\n\n" +
                                 $"💰 +{goldAmount:N0} oro\n" +
                                 $"💼 Oro total: {player.Gold:N0}"
                    };
                
                case 2: // Poción
                    var healthRestored = (int)(player.MaxHP * 0.3);
                    player.HP = Math.Min(player.MaxHP, player.HP + healthRestored);
                    _rpgService.SavePlayer(player);
                    
                    return new ExplorationResult
                    {
                        Type = ExplorationResultType.Treasure,
                        Message = $"⚗️ **¡POCIÓN ENCONTRADA!**\n\n" +
                                 $"Encontraste una poción de salud.\n\n" +
                                 $"💚 +{healthRestored} HP\n" +
                                 $"❤️ HP actual: {player.HP}/{player.MaxHP}"
                    };
                
                case 3: // XP
                    var xpAmount = _random.Next(20, 80) * player.Level;
                    player.XP += xpAmount;
                    _rpgService.SavePlayer(player);
                    
                    return new ExplorationResult
                    {
                        Type = ExplorationResultType.Treasure,
                        Message = $"📚 **¡CONOCIMIENTO ANCESTRAL!**\n\n" +
                                 $"Encontraste un pergamino antiguo en {zone.Name}.\n\n" +
                                 $"⭐ +{xpAmount:N0} XP\n" +
                                 $"📊 XP total: {player.XP:N0}"
                    };
                
                default:
                    return new ExplorationResult
                    {
                        Type = ExplorationResultType.Nothing,
                        Message = "🌿 Exploraste la zona pero no encontraste nada de valor esta vez."
                    };
            }
        }
        
        /// <summary>
        /// Viaja a otra zona
        /// </summary>
        public TravelResult Travel(RpgPlayer player, string targetZoneId)
        {
            var currentZone = RegionDatabase.GetZone(player.CurrentZone);
            var targetZone = RegionDatabase.GetZone(targetZoneId);
            
            if (targetZone == null)
            {
                return new TravelResult
                {
                    Success = false,
                    Message = "❌ Zona no encontrada."
                };
            }
            
            // Verificar si la zona está desbloqueada
            if (!player.UnlockedZones.Contains(targetZoneId) && !targetZone.IsStartingZone)
            {
                return new TravelResult
                {
                    Success = false,
                    Message = $"🔒 **{targetZone.Name}** no está desbloqueada.\n\n" +
                             "Explora zonas cercanas para descubrir nuevas ubicaciones."
                };
            }
            
            // Verificar requisito de nivel
            if (player.Level < targetZone.LevelRequirement)
            {
                return new TravelResult
                {
                    Success = false,
                    Message = $"⚠️ **{targetZone.Name}** requiere nivel {targetZone.LevelRequirement}.\n\n" +
                             $"Tu nivel actual: {player.Level}\n" +
                             $"Necesitas {targetZone.LevelRequirement - player.Level} niveles más."
                };
            }
            
            // Verificar si está conectada (si no es zona inicial)
            if (currentZone != null && !currentZone.ConnectedZones.Contains(targetZoneId) && !targetZone.IsStartingZone)
            {
                return new TravelResult
                {
                    Success = false,
                    Message = $"🚫 No puedes viajar directamente a **{targetZone.Name}** desde **{currentZone.Name}**.\n\n" +
                             "Usa `/map` para ver zonas conectadas."
                };
            }
            
            // Viaje exitoso
            player.CurrentZone = targetZoneId;
            player.CurrentLocation = targetZone.Name;
            _rpgService.SavePlayer(player);
            
            var region = RegionDatabase.GetAllRegions()
                .FirstOrDefault(r => r.ZoneIds.Contains(targetZoneId));
            
            return new TravelResult
            {
                Success = true,
                Zone = targetZone,
                Region = region,
                Message = $"🗺️ **Viajaste a {targetZone.Name}**\n\n" +
                         $"{targetZone.Emoji} {targetZone.Description}\n\n" +
                         $"🌍 Región: {region?.Name ?? "Desconocida"}\n" +
                         $"📊 Nivel de enemigos: {targetZone.MinEnemyLevel}-{targetZone.MaxEnemyLevel}\n" +
                         $"⚔️ Tasa de encuentro: {targetZone.EncounterRate * 100:F0}%\n" +
                         $"{(targetZone.IsSafeZone ? "🏘️ Zona segura" : "⚠️ Zona peligrosa")}"
            };
        }
    }
    
    /// <summary>
    /// Resultado de exploración
    /// </summary>
    public class ExplorationResult
    {
        public ExplorationResultType Type { get; set; }
        public string Message { get; set; } = "";
        public RpgEnemy? Enemy { get; set; }
    }
    
    public enum ExplorationResultType
    {
        Combat,           // Encontró un enemigo
        Treasure,         // Encontró recursos/oro/items
        ZoneDiscovered,   // Descubrió nueva zona
        Nothing,          // No encontró nada
        Error             // Error
    }
    
    /// <summary>
    /// Resultado de viaje
    /// </summary>
    public class TravelResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public GameZone? Zone { get; set; }
        public GameRegion? Region { get; set; }
    }
}
