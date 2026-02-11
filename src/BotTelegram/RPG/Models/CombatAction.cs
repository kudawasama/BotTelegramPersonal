namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Tipos de acciones disponibles en combate
    /// </summary>
    public enum CombatActionType
    {
        // ATAQUES
        PhysicalAttack,      // ⚔️ Ataque físico normal
        MagicalAttack,       // 🔮 Ataque mágico
        ChargeAttack,        // 💨 Envestida (correr + impacto)
        PreciseAttack,       // 🎯 Ataque preciso
        HeavyAttack,         // 💥 Ataque pesado
        
        // DEFENSAS
        Block,               // 🛡️ Bloquear
        Dodge,               // 🌀 Esquivar
        Counter,             // 💫 Contraataque
        Parry,               // ⚔️ Parada
        
        // MOVIMIENTO
        Jump,                // 🦘 Saltar
        Retreat,             // 🏃 Retroceder
        Advance,             // ⚡ Avanzar
        
        // OTROS
        Meditate,            // 🧘 Meditar (recupera mana)
        Observe,             // 👁️ Observar (revela info)
        UseItem,             // 🎒 Usar item
        Flee,                // 🏃 Huir
        Wait                 // ⏸️ Esperar/Pasar turno
    }
    
    /// <summary>
    /// Tipos de daño en el juego
    /// </summary>
    public enum DamageType
    {
        Physical,            // Físico normal
        Slashing,            // Cortante (espadas, hachas)
        Piercing,            // Perforante (flechas, lanzas)
        Bludgeoning,         // Contundente (mazas, puños)
        
        Magical,             // Mágico puro
        Fire,                // Fuego
        Ice,                 // Hielo
        Lightning,           // Rayo
        Water,               // Agua
        Earth,               // Tierra
        Wind,                // Viento
        
        Holy,                // Sagrado
        Dark,                // Oscuridad
        Poison,              // Veneno
        Acid,                // Ácido
        
        True                 // Daño verdadero (ignora defensas)
    }
    
    /// <summary>
    /// Resultado detallado de una acción de combate
    /// </summary>
    public class CombatActionResult
    {
        public CombatActionType Action { get; set; }
        public bool Success { get; set; }
        public int Damage { get; set; }
        public DamageType DamageType { get; set; }
        public double HitChance { get; set; }
        public double Roll { get; set; }
        public bool Critical { get; set; }
        public double CriticalChance { get; set; }
        public int ManaCost { get; set; }
        public int StaminaCost { get; set; }
        public string Message { get; set; } = "";
        
        // Efectos especiales
        public bool Dodged { get; set; }
        public bool Blocked { get; set; }
        public bool Countered { get; set; }
        public int DamageReduced { get; set; }
        public StatusEffectType? InflictedEffect { get; set; }
        
        // Información revelada (Observar)
        public string? RevealedInfo { get; set; }
    }
}
