namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Categoría de habilidad
    /// </summary>
    public enum SkillCategory
    {
        Combat,      // Habilidades de combate
        Magic,       // Habilidades mágicas
        Defense,     // Habilidades defensivas
        Movement,    // Habilidades de movimiento
        Special      // Habilidades especiales
    }
    
    /// <summary>
    /// Requisito para desbloquear una habilidad
    /// </summary>
    public class SkillRequirement
    {
        public string ActionType { get; set; } = ""; // Tipo de acción que se debe realizar
        public int Count { get; set; }               // Cantidad requerida
        public string Description { get; set; } = "";
    }
    
    /// <summary>
    /// Modelo de habilidad desbloqueable
    /// </summary>
    public class RpgSkill
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public SkillCategory Category { get; set; }
        public int RequiredLevel { get; set; }
        
        // Requisitos de desbloqueo
        public List<SkillRequirement> Requirements { get; set; } = new();
        
        // Costo de uso
        public int ManaCost { get; set; }
        public int StaminaCost { get; set; }
        public int Cooldown { get; set; } // Turnos de enfriamiento
        
        // Efectos de la habilidad
        public int DamageMultiplier { get; set; } // Porcentaje de daño (100 = normal, 150 = 1.5x)
        public int HealAmount { get; set; }
        public int BuffDuration { get; set; }      // Duración del buff en turnos
        public Dictionary<string, int> StatBuffs { get; set; } = new(); // Buffs temporales
        
        // Efectos especiales
        public bool IgnoresDefense { get; set; }
        public bool CanStun { get; set; }
        public int StunChance { get; set; }
        public bool MultiHit { get; set; }
        public int HitCount { get; set; }
        public DamageType DamageType { get; set; }
        
        /// <summary>
        /// Emoji según categoría
        /// </summary>
        public string CategoryEmoji => Category switch
        {
            SkillCategory.Combat => "⚔️",
            SkillCategory.Magic => "🔮",
            SkillCategory.Defense => "🛡️",
            SkillCategory.Movement => "💨",
            SkillCategory.Special => "✨",
            _ => "❓"
        };
        
        /// <summary>
        /// Verifica si el jugador cumple los requisitos
        /// </summary>
        public bool MeetsRequirements(RpgPlayer player)
        {
            if (player.Level < RequiredLevel)
                return false;
            
            foreach (var req in Requirements)
            {
                if (!player.ActionCounters.ContainsKey(req.ActionType) || 
                    player.ActionCounters[req.ActionType] < req.Count)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Obtiene el progreso de desbloqueo
        /// </summary>
        public string GetUnlockProgress(RpgPlayer player)
        {
            if (player.Level < RequiredLevel)
                return $"❌ Nivel {RequiredLevel} requerido (actual: {player.Level})";
            
            var progress = "";
            foreach (var req in Requirements)
            {
                var current = player.ActionCounters.ContainsKey(req.ActionType) 
                    ? player.ActionCounters[req.ActionType] 
                    : 0;
                var emoji = current >= req.Count ? "✅" : "⏳";
                progress += $"{emoji} {req.Description}: {current}/{req.Count}\n";
            }
            
            return progress;
        }
        
        /// <summary>
        /// Obtiene descripción completa de la habilidad
        /// </summary>
        public string GetFullDescription(RpgPlayer? player = null)
        {
            var desc = $"{CategoryEmoji} **{Name}**\n{Description}\n\n";
            desc += $"**Categoría:** {Category}\n";
            desc += $"**Nivel requerido:** {RequiredLevel}\n";
            
            if (ManaCost > 0 || StaminaCost > 0)
            {
                desc += "\n**Costo:**\n";
                if (ManaCost > 0) desc += $"• {ManaCost} Mana\n";
                if (StaminaCost > 0) desc += $"• {StaminaCost} Stamina\n";
            }
            
            if (Cooldown > 0)
                desc += $"• Enfriamiento: {Cooldown} turnos\n";
            
            desc += "\n**Efectos:**\n";
            if (DamageMultiplier > 0)
                desc += $"• {DamageMultiplier}% daño base\n";
            if (HealAmount > 0)
                desc += $"• Cura {HealAmount} HP\n";
            if (StatBuffs.Count > 0)
            {
                desc += $"• Buffs ({BuffDuration} turnos):\n";
                foreach (var buff in StatBuffs)
                    desc += $"  - +{buff.Value} {buff.Key}\n";
            }
            if (IgnoresDefense)
                desc += "• Ignora defensa\n";
            if (CanStun)
                desc += $"• {StunChance}% chance de aturdir\n";
            if (MultiHit)
                desc += $"• {HitCount} golpes\n";
            
            if (player != null && Requirements.Count > 0)
            {
                desc += "\n**Requisitos de desbloqueo:**\n";
                desc += GetUnlockProgress(player);
            }
            
            return desc;
        }
    }
    
    public enum StatType
    {
        // Stats primarios
        Strength,      // Fuerza física
        Intelligence,  // Poder mágico
        Dexterity,     // Agilidad, precisión
        Constitution,  // Resistencia, HP
        Wisdom,        // Regeneración mana, resistencia mágica
        Charisma,      // Críticos, negociación
        
        // Stats derivados
        Attack,        // Daño físico total
        MagicPower,    // Daño mágico total
        Defense,       // Defensa física
        MagicResist,   // Resistencia mágica
        CritChance,    // Probabilidad crítico
        Evasion,       // Probabilidad esquivar
        Accuracy       // Probabilidad golpear
    }
}
