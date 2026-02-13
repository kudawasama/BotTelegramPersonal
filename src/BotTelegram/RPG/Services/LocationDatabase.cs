using System;
using System.Collections.Generic;
using System.Linq;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos con todas las zonas/locaciones del juego
    /// </summary>
    public static class LocationDatabase
    {
        public static Dictionary<string, ZoneInfo> Zones { get; } = new()
        {
            {
                "puerto_esperanza", new ZoneInfo
                {
                    Id = "puerto_esperanza",
                    Name = "Puerto Esperanza",
                    Emoji = "🏘️",
                    Description = "Un pequeño pueblo costero. Ideal para aventureros novatos.",
                    MinLevel = 1,
                    RecommendedLevel = "1-5",
                    Enemies = new List<string> { "Lobo Salvaje", "Goblin", "Slime", "Rata Gigante" },
                    BossName = "Goblin Jefe",
                    BossEmoji = "👺",
                    EnemyLevelRange = "1-5",
                    DropQuality = 1.0,
                    XPMultiplier = 1.0,
                    GoldMultiplier = 1.0,
                    DangerLevel = "Fácil",
                    UnlockRequirement = "Ninguno (inicial)"
                }
            },
            {
                "bosque_oscuro", new ZoneInfo
                {
                    Id = "bosque_oscuro",
                    Name = "Bosque Oscuro",
                    Emoji = "🌲",
                    Description = "Un bosque denso y peligroso. Criaturas salvajes acechan entre los árboles.",
                    MinLevel = 6,
                    RecommendedLevel = "6-12",
                    Enemies = new List<string> { "Oso Salvaje", "Araña Gigante", "Orco", "Bandido" },
                    BossName = "Árbol Anciano Corrupto",
                    BossEmoji = "🌳",
                    EnemyLevelRange = "6-12",
                    DropQuality = 1.3,
                    XPMultiplier = 1.5,
                    GoldMultiplier = 1.4,
                    DangerLevel = "Moderado",
                    UnlockRequirement = "Nivel 6 + Derrotar Goblin Jefe"
                }
            },
            {
                "montanas_gelidas", new ZoneInfo
                {
                    Id = "montanas_gelidas",
                    Name = "Montañas Gélidas",
                    Emoji = "⛰️",
                    Description = "Cumbres heladas donde el aire es escaso. Solo los valientes se aventuran aquí.",
                    MinLevel = 13,
                    RecommendedLevel = "13-20",
                    Enemies = new List<string> { "Yeti", "Dragón de Hielo Joven", "Gigante de Hielo", "Hombre Lobo" },
                    BossName = "Dragón de Hielo Anciano",
                    BossEmoji = "🐉",
                    EnemyLevelRange = "13-20",
                    DropQuality = 1.7,
                    XPMultiplier = 2.0,
                    GoldMultiplier = 1.8,
                    DangerLevel = "Difícil",
                    UnlockRequirement = "Nivel 13 + Derrotar Árbol Anciano"
                }
            },
            {
                "ruinas_antiguas", new ZoneInfo
                {
                    Id = "ruinas_antiguas",
                    Name = "Ruinas Antiguas",
                    Emoji = "🏛️",
                    Description = "Restos de una civilización perdida. Guardianes inmortales protegen sus secretos.",
                    MinLevel = 21,
                    RecommendedLevel = "21-30",
                    Enemies = new List<string> { "Guardián de Piedra", "Momia", "Espectro", "Golem Antiguo" },
                    BossName = "Faraón No-Muerto",
                    BossEmoji = "⚱️",
                    EnemyLevelRange = "21-30",
                    DropQuality = 2.2,
                    XPMultiplier = 2.5,
                    GoldMultiplier = 2.3,
                    DangerLevel = "Muy Difícil",
                    UnlockRequirement = "Nivel 21 + Derrotar Dragón de Hielo"
                }
            },
            {
                "abismo_infernal", new ZoneInfo
                {
                    Id = "abismo_infernal",
                    Name = "Abismo Infernal",
                    Emoji = "🔥",
                    Description = "Las profundidades del infierno. Solo los más fuertes sobreviven aquí.",
                    MinLevel = 31,
                    RecommendedLevel = "31-50",
                    Enemies = new List<string> { "Demonio Menor", "Balrog", "Dragón Infernal", "Diablo" },
                    BossName = "Señor Demonio Baal",
                    BossEmoji = "😈",
                    EnemyLevelRange = "31-50",
                    DropQuality = 3.0,
                    XPMultiplier = 3.5,
                    GoldMultiplier = 3.0,
                    DangerLevel = "Extremo",
                    UnlockRequirement = "Nivel 31 + Derrotar Faraón + 3 Clases Ocultas"
                }
            },
            {
                "reino_celestial", new ZoneInfo
                {
                    Id = "reino_celestial",
                    Name = "Reino Celestial",
                    Emoji = "☁️",
                    Description = "El reino de los dioses. Solo los héroes legendarios llegan aquí. (ENDGAME)",
                    MinLevel = 50,
                    RecommendedLevel = "50+",
                    Enemies = new List<string> { "Ángel Caído", "Titán", "Arcángel Corrupto", "Dios Menor" },
                    BossName = "Dios de la Guerra Ares",
                    BossEmoji = "⚡",
                    EnemyLevelRange = "50-100",
                    DropQuality = 5.0,
                    XPMultiplier = 5.0,
                    GoldMultiplier = 4.5,
                    DangerLevel = "IMPOSIBLE",
                    UnlockRequirement = "Nivel 50 + Derrotar Baal + 5 Clases Ocultas + 1 Clase Legendaria"
                }
            }
        };
        
        /// <summary>
        /// Obtiene información de una zona por su ID
        /// </summary>
        public static ZoneInfo? GetZone(string zoneId)
        {
            return Zones.ContainsKey(zoneId) ? Zones[zoneId] : null;
        }
        
        /// <summary>
        /// Verifica si un jugador puede acceder a una zona
        /// </summary>
        public static bool CanAccessZone(Models.RpgPlayer player, string zoneId)
        {
            if (!Zones.ContainsKey(zoneId)) return false;
            
            var zone = Zones[zoneId];
            
            // Verificar nivel mínimo
            if (player.Level < zone.MinLevel) return false;
            
            // Si ya está desbloqueada, permitir acceso
            if (player.UnlockedZones.Contains(zoneId)) return true;
            
            // Puerto Esperanza siempre accesible
            if (zoneId == "puerto_esperanza") return true;
            
            return false;
        }
        
        /// <summary>
        /// Desbloquea una zona para el jugador
        /// </summary>
        public static bool UnlockZone(Models.RpgPlayer player, string zoneId)
        {
            if (!Zones.ContainsKey(zoneId)) return false;
            if (player.UnlockedZones.Contains(zoneId)) return false;
            
            var zone = Zones[zoneId];
            
            // Verificar nivel mínimo
            if (player.Level < zone.MinLevel) return false;
            
            player.UnlockedZones.Add(zoneId);
            return true;
        }
        
        /// <summary>
        /// Obtiene todas las zonas disponibles para un jugador
        /// </summary>
        public static List<ZoneInfo> GetAvailableZones(Models.RpgPlayer player)
        {
            return Zones.Values
                .Where(z => CanAccessZone(player, z.Id))
                .OrderBy(z => z.MinLevel)
                .ToList();
        }
        
        /// <summary>
        /// Obtiene todas las zonas desbloqueables para un jugador
        /// (cumple nivel mínimo pero no desbloqueadas aún)
        /// </summary>
        public static List<ZoneInfo> GetUnlockableZones(Models.RpgPlayer player)
        {
            return Zones.Values
                .Where(z => player.Level >= z.MinLevel && !player.UnlockedZones.Contains(z.Id))
                .OrderBy(z => z.MinLevel)
                .ToList();
        }
        
        /// <summary>
        /// Obtiene el nombre de un enemigo aleatorio de la zona
        /// </summary>
        public static string GetRandomEnemy(string zoneId, Random? random = null)
        {
            if (!Zones.ContainsKey(zoneId)) return "Slime";
            
            var zone = Zones[zoneId];
            var rng = random ?? new Random();
            
            return zone.Enemies[rng.Next(zone.Enemies.Count)];
        }
        
        /// <summary>
        /// Genera un mensaje de bienvenida a una zona
        /// </summary>
        public static string GetZoneWelcomeMessage(string zoneId)
        {
            if (!Zones.ContainsKey(zoneId)) return "Has llegado a una zona desconocida.";
            
            var zone = Zones[zoneId];
            return $"{zone.Emoji} **{zone.Name}**\n\n{zone.Description}\n\n" +
                   $"📊 **Nivel Recomendado:** {zone.RecommendedLevel}\n" +
                   $"⚠️ **Dificultad:** {zone.DangerLevel}\n" +
                   $"👹 **Boss:** {zone.BossEmoji} {zone.BossName}";
        }
    }
    
    /// <summary>
    /// Información sobre una zona/locación
    /// </summary>
    public class ZoneInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "🗺️";
        public string Description { get; set; } = string.Empty;
        public int MinLevel { get; set; }
        public string RecommendedLevel { get; set; } = "1+";
        public List<string> Enemies { get; set; } = new();
        public string BossName { get; set; } = string.Empty;
        public string BossEmoji { get; set; } = "👹";
        public string EnemyLevelRange { get; set; } = "1-10";
        public double DropQuality { get; set; } = 1.0;
        public double XPMultiplier { get; set; } = 1.0;
        public double GoldMultiplier { get; set; } = 1.0;
        public string DangerLevel { get; set; } = "Desconocido";
        public string UnlockRequirement { get; set; } = "Ninguno";
    }
}
