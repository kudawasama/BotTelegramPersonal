using System;
using System.Collections.Generic;
using System.Linq;
using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de mazmorras predefinidas
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public static class DungeonDatabase
    {
        private static Dictionary<string, DungeonTemplate> _dungeonTemplates = new();
        
        static DungeonDatabase()
        {
            InitializeDungeonTemplates();
        }
        
        /// <summary>
        /// Plantilla de mazmorra reutilizable
        /// </summary>
        public class DungeonTemplate
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Emoji { get; set; } = "🏰";
            public int MinLevel { get; set; }
            public DungeonDifficulty Difficulty { get; set; }
            public bool RequiresKey { get; set; }
            public List<string> PossibleEnemyTypes { get; set; } = new(); // Tipos de enemigos que aparecen
        }
        
        private static void InitializeDungeonTemplates()
        {
            // ═══ COMMON DUNGEONS (5 pisos, Lv 5+) ═══
            _dungeonTemplates["crypt_common"] = new DungeonTemplate
            {
                Id = "crypt_common",
                Name = "Cripta Abandonada",
                Description = "Una vieja cripta olvidada. Esqueletos y zombies rondan sus pasillos.",
                Emoji = "⚰️",
                MinLevel = 5,
                Difficulty = DungeonDifficulty.Common,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "skeleton", "zombie", "ghoul" }
            };
            
            _dungeonTemplates["cave_common"] = new DungeonTemplate
            {
                Id = "cave_common",
                Name = "Cueva de Murciélagos",
                Description = "Una cueva infestada de murciélagos y criaturas nocturnas.",
                Emoji = "🦇",
                MinLevel = 5,
                Difficulty = DungeonDifficulty.Common,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "bat", "spider", "rat" }
            };
            
            // ═══ UNCOMMON DUNGEONS (8 pisos, Lv 10+) ═══
            _dungeonTemplates["forest_ruins"] = new DungeonTemplate
            {
                Id = "forest_ruins",
                Name = "Ruinas del Bosque Oscuro",
                Description = "Antiguas ruinas tomadas por bestias salvajes y espíritus malignos.",
                Emoji = "🌲",
                MinLevel = 10,
                Difficulty = DungeonDifficulty.Uncommon,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "wolf", "bear", "wraith", "bandit" }
            };
            
            _dungeonTemplates["abandoned_mine"] = new DungeonTemplate
            {
                Id = "abandoned_mine",
                Name = "Mina Abandonada",
                Description = "Una mina profunda donde mineros murieron hace años. Sus espíritus aún merodean.",
                Emoji = "⛏️",
                MinLevel = 10,
                Difficulty = DungeonDifficulty.Uncommon,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "miner_ghost", "rock_golem", "bat", "spider" }
            };
            
            // ═══ RARE DUNGEONS (12 pisos, Lv 15+) ═══
            _dungeonTemplates["cursed_cathedral"] = new DungeonTemplate
            {
                Id = "cursed_cathedral",
                Name = "Catedral Maldita",
                Description = "Una catedral caída en la oscuridad. Cultistas y demonios menores la habitan.",
                Emoji = "⛪",
                MinLevel = 15,
                Difficulty = DungeonDifficulty.Rare,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "cultist", "demon", "gargoyle", "wraith" }
            };
            
            _dungeonTemplates["frozen_fortress"] = new DungeonTemplate
            {
                Id = "frozen_fortress",
                Name = "Fortaleza Congelada",
                Description = "Una fortaleza antigua cubierta de hielo eterno. Criaturas de hielo custodian sus secretos.",
                Emoji = "❄️",
                MinLevel = 15,
                Difficulty = DungeonDifficulty.Rare,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "ice_golem", "frost_wraith", "yeti", "ice_elemental" }
            };
            
            // ═══ EPIC DUNGEONS (18 pisos, Lv 20+) ═══
            _dungeonTemplates["necropolis"] = new DungeonTemplate
            {
                Id = "necropolis",
                Name = "Necrópolis de los Reyes Muertos",
                Description = "Ciudad de los muertos. Antiguos reyes no-muertos gobiernan legiones de esqueletos.",
                Emoji = "👑",
                MinLevel = 20,
                Difficulty = DungeonDifficulty.Epic,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "skeleton_knight", "lich", "death_knight", "bone_dragon" }
            };
            
            _dungeonTemplates["volcanic_depths"] = new DungeonTemplate
            {
                Id = "volcanic_depths",
                Name = "Profundidades Volcánicas",
                Description = "Las entrañas de un volcán activo. Demonios de fuego y elementales dominan este infierno.",
                Emoji = "🌋",
                MinLevel = 20,
                Difficulty = DungeonDifficulty.Epic,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "fire_elemental", "demon", "lava_golem", "hellhound" }
            };
            
            // ═══ LEGENDARY DUNGEONS (25 pisos, Lv 25+) ═══
            _dungeonTemplates["abyss_tower"] = new DungeonTemplate
            {
                Id = "abyss_tower",
                Name = "Torre del Abismo",
                Description = "Una torre que desciende hacia las profundidades del abismo. Los horrores antiguos esperan.",
                Emoji = "🗼",
                MinLevel = 25,
                Difficulty = DungeonDifficulty.Legendary,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "abyssal_horror", "void_spawn", "ancient_demon", "eldritch_beast" }
            };
            
            _dungeonTemplates["celestial_palace"] = new DungeonTemplate
            {
                Id = "celestial_palace",
                Name = "Palacio Celestial Corrupto",
                Description = "Un palacio de los dioses caídos. Ángeles caídos y bestias celestiales corruptas.",
                Emoji = "☁️",
                MinLevel = 25,
                Difficulty = DungeonDifficulty.Legendary,
                RequiresKey = true,
                PossibleEnemyTypes = new() { "fallen_angel", "corrupted_seraphim", "celestial_beast", "archon" }
            };
        }
        
        /// <summary>
        /// Obtiene todas las plantillas de mazmorras
        /// </summary>
        public static Dictionary<string, DungeonTemplate> GetAllDungeonTemplates()
        {
            return _dungeonTemplates;
        }
        
        /// <summary>
        /// Obtiene una plantilla específica por ID
        /// </summary>
        public static DungeonTemplate? GetDungeonTemplate(string id)
        {
            return _dungeonTemplates.GetValueOrDefault(id);
        }
        
        /// <summary>
        /// Obtiene mazmorras disponibles para un nivel y dificultad específica
        /// </summary>
        public static List<DungeonTemplate> GetAvailableDungeons(int playerLevel, DungeonDifficulty? difficulty = null)
        {
            var available = _dungeonTemplates.Values
                .Where(d => d.MinLevel <= playerLevel);
            
            if (difficulty.HasValue)
            {
                available = available.Where(d => d.Difficulty == difficulty.Value);
            }
            
            return available.OrderBy(d => d.MinLevel).ToList();
        }
        
        /// <summary>
        /// Obtiene el número de pisos según la dificultad
        /// </summary>
        public static int GetTotalFloorsForDifficulty(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => 5,
                DungeonDifficulty.Uncommon => 8,
                DungeonDifficulty.Rare => 12,
                DungeonDifficulty.Epic => 18,
                DungeonDifficulty.Legendary => 25,
                _ => 5
            };
        }
        
        /// <summary>
        /// Obtiene el emoji de dificultad
        /// </summary>
        public static string GetDifficultyEmoji(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => "⚪",
                DungeonDifficulty.Uncommon => "🟢",
                DungeonDifficulty.Rare => "🔵",
                DungeonDifficulty.Epic => "🟣",
                DungeonDifficulty.Legendary => "🟠",
                _ => "⚪"
            };
        }
        
        /// <summary>
        /// Obtiene el nombre localizado de la dificultad
        /// </summary>
        public static string GetDifficultyName(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => "Común",
                DungeonDifficulty.Uncommon => "Poco Común",
                DungeonDifficulty.Rare => "Raro",
                DungeonDifficulty.Epic => "Épico",
                DungeonDifficulty.Legendary => "Legendario",
                _ => "Desconocido"
            };
        }
    }
}
