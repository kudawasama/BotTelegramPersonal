using System;
using System.Collections.Generic;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos con información de todos los tipos de minions invocables
    /// </summary>
    public static class MinionDatabase
    {
        public static Dictionary<MinionType, MinionInfo> Minions { get; } = new()
        {
            // ═══════════════════════════════════════
            // NO-MUERTOS (NECROMANCER/LICH KING)
            // ═══════════════════════════════════════
            {
                MinionType.Skeleton, new MinionInfo
                {
                    Name = "Esqueleto",
                    Emoji = "💀",
                    Type = MinionType.Skeleton,
                    Description = "Guerrero no-muerto básico. Rápido pero frágil.",
                    ManaCost = 30,
                    HPCost = 0,
                    Duration = 10, // turnos
                    IsControlled = true,
                    SpecialAbility = "Ataque Rápido: Actúa primero en su turno"
                }
            },
            {
                MinionType.Zombie, new MinionInfo
                {
                    Name = "Zombie",
                    Emoji = "🧟",
                    Type = MinionType.Zombie,
                    Description = "Cadáver reanimado. Resistente y tanque.",
                    ManaCost = 45,
                    HPCost = 0,
                    Duration = 12,
                    IsControlled = true,
                    SpecialAbility = "Resistencia: Reduce 50% del daño recibido"
                }
            },
            {
                MinionType.Ghost, new MinionInfo
                {
                    Name = "Fantasma",
                    Emoji = "👻",
                    Type = MinionType.Ghost,
                    Description = "Espíritu vengativo. Puede atravesar defensas.",
                    ManaCost = 50,
                    HPCost = 0,
                    Duration = 8,
                    IsControlled = true,
                    SpecialAbility = "Intangible: Ignora 50% de la defensa enemiga"
                }
            },
            {
                MinionType.Lich, new MinionInfo
                {
                    Name = "Lich Menor",
                    Emoji = "☠️",
                    Type = MinionType.Lich,
                    Description = "Hechicero no-muerto poderoso. Alto daño mágico.",
                    ManaCost = 100,
                    HPCost = 0,
                    Duration = 15,
                    IsControlled = true,
                    SpecialAbility = "Magia Oscura: Ataques drenan 20% de vida"
                }
            },
            
            // ═══════════════════════════════════════
            // ELEMENTALES (ELEMENTAL OVERLORD)
            // ═══════════════════════════════════════
            {
                MinionType.FireElemental, new MinionInfo
                {
                    Name = "Elemental de Fuego",
                    Emoji = "🔥",
                    Type = MinionType.FireElemental,
                    Description = "Ser de fuego puro. Ataques causan quemaduras.",
                    ManaCost = 60,
                    HPCost = 0,
                    Duration = 10,
                    IsControlled = true,
                    SpecialAbility = "Ignición: 30% chance de quemar (DoT 5/turno x3)"
                }
            },
            {
                MinionType.WaterElemental, new MinionInfo
                {
                    Name = "Elemental de Agua",
                    Emoji = "💧",
                    Type = MinionType.WaterElemental,
                    Description = "Espíritu acuático. Cura al invocador.",
                    ManaCost = 55,
                    HPCost = 0,
                    Duration = 12,
                    IsControlled = true,
                    SpecialAbility = "Regeneración: Cura al invocador 5% MaxHP/turno"
                }
            },
            {
                MinionType.EarthElemental, new MinionInfo
                {
                    Name = "Elemental de Tierra",
                    Emoji = "🪨",
                    Type = MinionType.EarthElemental,
                    Description = "Coloso de piedra. Máxima defensa.",
                    ManaCost = 70,
                    HPCost = 0,
                    Duration = 15,
                    IsControlled = true,
                    SpecialAbility = "Terremoto: AoE daña enemigos, aturde 1 turno"
                }
            },
            {
                MinionType.AirElemental, new MinionInfo
                {
                    Name = "Elemental de Aire",
                    Emoji = "💨",
                    Type = MinionType.AirElemental,
                    Description = "Torbellino viviente. Velocidad extrema.",
                    ManaCost = 50,
                    HPCost = 0,
                    Duration = 8,
                    IsControlled = true,
                    SpecialAbility = "Velocidad del Viento: Actúa 2 veces por turno"
                }
            },
            
            // ═══════════════════════════════════════
            // VOID/ABERRACIONES (VOID SUMMONER)
            // ═══════════════════════════════════════
            {
                MinionType.VoidHorror, new MinionInfo
                {
                    Name = "Horror del Vacío",
                    Emoji = "👁️",
                    Type = MinionType.VoidHorror,
                    Description = "Aberración cósmica. Extremadamente peligroso.",
                    ManaCost = 80,
                    HPCost = 40, // 40% del HP máximo
                    Duration = 6,
                    IsControlled = false, // NO controlable
                    SpecialAbility = "Furia Ciega: 30% chance de atacar al invocador"
                }
            },
            {
                MinionType.Aberration, new MinionInfo
                {
                    Name = "Aberración",
                    Emoji = "🐙",
                    Type = MinionType.Aberration,
                    Description = "Entidad inimaginable. Poder devastador sin control.",
                    ManaCost = 120,
                    HPCost = 50, // 50% del HP máximo
                    Duration = 5,
                    IsControlled = false, // NO controlable
                    SpecialAbility = "Locura: Ataca objetivo aleatorio, ignora órdenes"
                }
            }
        };
        
        /// <summary>
        /// Crea una nueva instancia de minion con stats escaladas
        /// </summary>
        public static Minion CreateMinion(MinionType type, int summonerLevel, double statsMultiplier = 1.0)
        {
            if (!Minions.ContainsKey(type))
                throw new ArgumentException($"Tipo de minion desconocido: {type}");
            
            var info = Minions[type];
            var minion = new Minion
            {
                Name = info.Name,
                Emoji = info.Emoji,
                Type = type,
                IsTemporary = info.Duration > 0,
                TurnsRemaining = info.Duration,
                IsControlled = info.IsControlled,
                SpecialAbility = info.SpecialAbility,
                SummonerLevel = summonerLevel,
                StatsMultiplier = statsMultiplier
            };
            
            minion.ScaleToSummonerLevel(summonerLevel);
            
            return minion;
        }
        
        /// <summary>
        /// Obtiene información de un tipo de minion
        /// </summary>
        public static MinionInfo? GetMinionInfo(MinionType type)
        {
            return Minions.ContainsKey(type) ? Minions[type] : null;
        }
        
        /// <summary>
        /// Obtiene todos los minions disponibles para una clase
        /// </summary>
        public static List<MinionType> GetAvailableMinions(CharacterClass playerClass)
        {
            var available = new List<MinionType>();
            
            // Necromancer/Lich King pueden invocar no-muertos
            if (playerClass == CharacterClass.Warrior) // TODO: Cambiar a Necromancer cuando exista
            {
                available.AddRange(new[]
                {
                    MinionType.Skeleton,
                    MinionType.Zombie,
                    MinionType.Ghost,
                    MinionType.Lich
                });
            }
            
            // Elemental Overlord puede invocar elementales
            if (playerClass == CharacterClass.Mage) // TODO: Cambiar a ElementalOverlord cuando exista
            {
                available.AddRange(new[]
                {
                    MinionType.FireElemental,
                    MinionType.WaterElemental,
                    MinionType.EarthElemental,
                    MinionType.AirElemental
                });
            }
            
            // Void Summoner puede invocar aberraciones
            // TODO: Implementar cuando exista la clase
            
            // Por ahora, todos pueden invocar básicos para testing
            if (available.Count == 0)
            {
                available.Add(MinionType.Skeleton);
                available.Add(MinionType.Zombie);
            }
            
            return available;
        }
    }
    
    /// <summary>
    /// Información estática de un tipo de minion
    /// </summary>
    public class MinionInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "👻";
        public MinionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public int ManaCost { get; set; }
        public int HPCost { get; set; } // Como % del MaxHP
        public int Duration { get; set; } // Turnos, -1 = permanente
        public bool IsControlled { get; set; } = true;
        public string? SpecialAbility { get; set; }
    }
}
