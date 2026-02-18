using System;
using System.Collections.Generic;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Representa una mazmorra completa con múltiples pisos
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public class Dungeon
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Emoji { get; set; } = "🏰";
        
        // Requisitos
        public int MinLevel { get; set; } = 1;
        public DungeonDifficulty Difficulty { get; set; }
        public bool RequiresKey { get; set; } = false;
        public string? RequiredKeyId { get; set; }
        
        // Estructura
        public int TotalFloors { get; set; }
        public int CurrentFloor { get; set; } = 0;
        public List<DungeonFloor> Floors { get; set; } = new();
        
        // Estado
        public bool IsActive { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        
        // Recompensas
        public DungeonRewards FinalRewards { get; set; } = new();
        
        // Estadísticas
        public int TotalEnemiesDefeated { get; set; } = 0;
        public int TotalBossesDefeated { get; set; } = 0;
        public bool IsPerfectRun { get; set; } = true; // No pisos perdidos
    }
    
    /// <summary>
    /// Dificultad de la mazmorra (determina pisos, nivel mínimo y recompensas)
    /// </summary>
    public enum DungeonDifficulty
    {
        Common,      // 5 pisos, Lv 5+
        Uncommon,    // 8 pisos, Lv 10+
        Rare,        // 12 pisos, Lv 15+
        Epic,        // 18 pisos, Lv 20+
        Legendary    // 25 pisos, Lv 25+
    }
    
    /// <summary>
    /// Recompensas finales al completar una mazmorra
    /// </summary>
    public class DungeonRewards
    {
        public int Gold { get; set; }
        public int XP { get; set; }
        public int SkillPoints { get; set; } = 0;
        public List<RpgEquipment> Equipment { get; set; } = new();
        public List<RpgItem> Items { get; set; } = new();
        public DungeonKey? KeyReward { get; set; }
        public double PerfectionBonus { get; set; } = 0; // % bonus si es perfect run
    }
}
