using System;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Llave para desbloquear mazmorras de ciertas dificultades
    /// FASE 3: Sistema de Mazmorras
    /// </summary>
    public class DungeonKey
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "🔑";
        public DungeonDifficulty UnlocksDifficulty { get; set; }
        public bool IsConsumed { get; set; } = false; // Se consume al entrar a la mazmorra
        public DateTime? ObtainedAt { get; set; }
        
        public string GetDescription()
        {
            return UnlocksDifficulty switch
            {
                DungeonDifficulty.Common => "🔑 Abre mazmorras comunes (5 pisos, Lv 5+)",
                DungeonDifficulty.Uncommon => "🗝️ Abre mazmorras poco comunes (8 pisos, Lv 10+)",
                DungeonDifficulty.Rare => "🔐 Abre mazmorras raras (12 pisos, Lv 15+)",
                DungeonDifficulty.Epic => "🎖️ Abre mazmorras épicas (18 pisos, Lv 20+)",
                DungeonDifficulty.Legendary => "👑 Abre mazmorras legendarias (25 pisos, Lv 25+)",
                _ => "🔑 Llave misteriosa"
            };
        }
        
        public static string GetEmojiForDifficulty(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => "🔑",
                DungeonDifficulty.Uncommon => "🗝️",
                DungeonDifficulty.Rare => "🔐",
                DungeonDifficulty.Epic => "🎖️",
                DungeonDifficulty.Legendary => "👑",
                _ => "🔑"
            };
        }
        
        public static double GetDropChance(DungeonDifficulty difficulty)
        {
            return difficulty switch
            {
                DungeonDifficulty.Common => 0.15,      // 15%
                DungeonDifficulty.Uncommon => 0.10,    // 10%
                DungeonDifficulty.Rare => 0.07,        // 7%
                DungeonDifficulty.Epic => 0.05,        // 5%
                DungeonDifficulty.Legendary => 0.03,   // 3%
                _ => 0.0
            };
        }
    }
}
