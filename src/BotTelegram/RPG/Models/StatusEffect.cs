namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Efectos de estado que pueden afectar al jugador o enemigos durante el combate
    /// </summary>
    public class StatusEffect
    {
        public StatusEffectType Type { get; set; }
        public int Duration { get; set; } // Número de turnos restantes
        public int Intensity { get; set; } // Daño por turno o magnitud del efecto
        
        public StatusEffect(StatusEffectType type, int duration, int intensity)
        {
            Type = type;
            Duration = duration;
            Intensity = intensity;
        }
    }
    
    public enum StatusEffectType
    {
        Bleeding,      // 🩸 Sangrado: daño por turno
        Poisoned,      // 🧪 Envenenado: daño creciente
        Stunned,       // 💫 Aturdido: no puede atacar
        Burning,       // 🔥 Quemadura: daño de fuego
        Frozen,        // ❄️ Congelado: -50% velocidad
        Regenerating,  // 💚 Regeneración: cura por turno
        Shielded,      // 🛡️ Escudo: +bonus defensa temporal
        Empowered      // ⚡ Potenciado: +bonus ataque temporal
    }
    
    /// <summary>
    /// Entrada en el log de combate para historial
    /// </summary>
    public class CombatLogEntry
    {
        public int Turn { get; set; }
        public string Action { get; set; } = "";
        public string Result { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public override string ToString()
        {
            return $"[T{Turn}] {Action} → {Result}";
        }
    }
}
