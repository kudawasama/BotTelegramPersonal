namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Habilidad que puede usar un jugador o enemigo en combate
    /// </summary>
    public class RpgSkill
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "⚔️";
        
        public SkillType Type { get; set; } = SkillType.Physical;
        public SkillTarget Target { get; set; } = SkillTarget.SingleEnemy;
        
        // Costos
        public int ManaCost { get; set; } = 0;
        public int StaminaCost { get; set; } = 0;
        public int EnergyCost { get; set; } = 0;
        
        // Requisitos
        public int LevelRequired { get; set; } = 1;
        public CharacterClass? ClassRequired { get; set; } = null;
        
        // Efectividad
        public int BaseDamage { get; set; } = 0;
        public double BaseCritChance { get; set; } = 0.05; // 5% base
        public double HitChance { get; set; } = 0.85; // 85% base
        
        // Scaling (qué stat escala el daño)
        public StatType PrimaryScalingStat { get; set; } = StatType.Strength;
        public double ScalingMultiplier { get; set; } = 1.0; // Multiplicador del stat
        
        // Efectos especiales
        public StatusEffectType? InflictEffect { get; set; } = null;
        public double EffectChance { get; set; } = 0; // Probabilidad de aplicar efecto
        public int EffectDuration { get; set; } = 0;
        public int EffectIntensity { get; set; } = 0;
        
        // Cooldown
        public int Cooldown { get; set; } = 0; // Turnos de recarga
        public int CurrentCooldown { get; set; } = 0; // Turnos restantes
        
        // Flags
        public bool IgnoresDefense { get; set; } = false;
        public bool CannotMiss { get; set; } = false;
        public bool MultiHit { get; set; } = false;
        public int HitCount { get; set; } = 1;
    }
    
    public enum SkillType
    {
        Physical,      // Usa defensa física del enemigo
        Magical,       // Usa resistencia mágica del enemigo
        True,          // Ignora defensas
        Healing,       // Cura HP
        Buff,          // Mejora stats
        Debuff         // Reduce stats enemigo
    }
    
    public enum SkillTarget
    {
        Self,          // Solo el usuario
        SingleEnemy,   // Un enemigo
        AllEnemies,    // Todos los enemigos
        SingleAlly,    // Un aliado
        AllAllies      // Todos los aliados
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
