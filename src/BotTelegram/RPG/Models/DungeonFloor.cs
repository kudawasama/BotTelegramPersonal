using System;
using System.Collections.Generic;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Representa un piso individual dentro de una mazmorra
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public class DungeonFloor
    {
        public int FloorNumber { get; set; }
        public FloorType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        
        // Estado
        public bool IsCleared { get; set; } = false;
        public bool IsCurrentFloor { get; set; } = false;
        
        // Combate (si es Combat, Elite o Boss floor)
        public RpgEnemy? Enemy { get; set; }
        public int EnemyCount { get; set; } = 1; // Para Combat puede ser 1-3
        
        // Rest Floor
        public int RestorePercentage { get; set; } = 50; // % de HP/Mana/Stamina restaurado
        
        // Trap Floor
        public TrapEvent? Trap { get; set; }
        
        // Recompensas
        public FloorReward? Reward { get; set; }
    }
    
    /// <summary>
    /// Tipos de pisos en una mazmorra
    /// </summary>
    public enum FloorType
    {
        Combat,    // 60% - 1-3 enemigos normales
        Elite,     // 20% - 1 enemigo élite (HP +50%, Stats +30%)
        Boss,      // Cada 5 pisos - 1 jefe poderoso
        Rest,      // 10% - Restaura 50% HP/Mana/Stamina
        Trap       // 10% - Evento de trampa (daño o pérdida de recursos)
    }
    
    /// <summary>
    /// Recompensas de un piso individual
    /// </summary>
    public class FloorReward
    {
        public int Gold { get; set; }
        public int XP { get; set; }
        public RpgEquipment? Equipment { get; set; }
        public List<RpgItem> Items { get; set; } = new();
        public bool HasSkillPoint { get; set; } = false; // Solo en Boss floors
    }
    
    /// <summary>
    /// Evento de trampa en un piso
    /// </summary>
    public class TrapEvent
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrapType Type { get; set; }
        public int Damage { get; set; } = 0; // Daño directo a HP
        public int ManaDrain { get; set; } = 0; // Pérdida de mana
        public int StaminaDrain { get; set; } = 0; // Pérdida de stamina
        public int GoldLoss { get; set; } = 0; // Pérdida de oro
        public bool CanAvoid { get; set; } = true; // Si se puede esquivar con DEX check
        public int AvoidDC { get; set; } = 15; // Difficulty Check para esquivar
        public FloorReward? SuccessReward { get; set; } // Recompensa si se supera la trampa
    }
    
    /// <summary>
    /// Tipos de trampas
    /// </summary>
    public enum TrapType
    {
        SpikeTrap,      // Daño físico
        PoisonGas,      // Daño + stamina drain
        ManaDrain,      // Drena mana
        CursedChest,    // Oro loss o item maldito
        Ambush,         // Combate sorpresa con enemigos débiles
        Illusion,       // Confusión (puede dar recompensa si se supera)
        FallingFloor    // Mucho daño pero esquivable
    }
}
