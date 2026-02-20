using System.Text.Json.Serialization;

namespace BotTelegram.RPG.Models
{
    public class RpgEnemy
    {
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "👹";
        public string Description { get; set; } = "";
        public int Level { get; set; } = 1;
        public int HP { get; set; } = 30;
        public int MaxHP { get; set; } = 30;
        public int Attack { get; set; } = 8;
        public int MagicPower { get; set; } = 5;
        public int PhysicalDefense { get; set; } = 2;
        public int MagicResistance { get; set; } = 1;
        public int Accuracy { get; set; } = 10;
        public int Evasion { get; set; } = 5;
        public int Speed { get; set; } = 5;  // Velocidad de ataque (1-10)
        public int XPReward { get; set; } = 20;
        public int GoldReward { get; set; } = 15;
        public EnemyDifficulty Difficulty { get; set; } = EnemyDifficulty.Easy;
        public EnemyType Type { get; set; } = EnemyType.Beast;
        
        // ═══════════════════════════════════════
        // SISTEMA DE RESISTENCIAS Y DEBILIDADES
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Tipo de daño principal que inflige el enemigo
        /// </summary>
        public DamageType PrimaryDamageType { get; set; } = DamageType.Physical;
        
        /// <summary>
        /// Resistencias: reduce daño recibido (0.0 = normal, 0.5 = mitad, 1.0 = inmune)
        /// </summary>
        public Dictionary<DamageType, double> Resistances { get; set; } = new();
        
        /// <summary>
        /// Debilidades: aumenta daño recibido (1.5 = +50%, 2.0 = doble)
        /// </summary>
        public Dictionary<DamageType, double> Weaknesses { get; set; } = new();
        
        /// <summary>
        /// Inmunidades completas a tipos de daño
        /// </summary>
        public List<DamageType> Immunities { get; set; } = new();
        
        /// <summary>
        /// Inmunidades a efectos de estado
        /// </summary>
        public List<StatusEffectType> StatusImmunities { get; set; } = new();
        
        // ═══════════════════════════════════════
        // COMPORTAMIENTO Y HABILIDADES
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Comportamiento de IA (Agresivo, Defensivo, Inteligente, etc)
        /// </summary>
        public EnemyBehavior Behavior { get; set; } = EnemyBehavior.Balanced;
        
        // Special abilities
        public bool CanPoison { get; set; } = false;
        public bool CanStun { get; set; } = false;
        public bool CanHeal { get; set; } = false;
        public bool CanFly { get; set; } = false;
        public bool CanTeleport { get; set; } = false;
        public bool CanRegenerate { get; set; } = false;
        
        /// <summary>
        /// Habilidades especiales del enemigo
        /// </summary>
        public List<string> SpecialAbilities { get; set; } = new();
        
        // Combat avanzado
        public List<StatusEffect> StatusEffects { get; set; } = new();
        
        // Estado de combate
        public bool IsDefending { get; set; } = false;
        public int DefenseBonus { get; set; } = 0;
        
        // Loot drops
        public List<RpgItem>? PossibleLoot { get; set; }
        
        // ═══════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Obtiene el multiplicador de daño según tipo
        /// </summary>
        public double GetDamageMultiplier(DamageType damageType)
        {
            // Inmunidad total
            if (Immunities.Contains(damageType))
                return 0.0;
            
            // Debilidad
            if (Weaknesses.ContainsKey(damageType))
                return Weaknesses[damageType];
            
            // Resistencia
            if (Resistances.ContainsKey(damageType))
                return 1.0 - Resistances[damageType];
            
            // Normal
            return 1.0;
        }
        
        /// <summary>
        /// Verifica si es inmune a un efecto de estado
        /// </summary>
        public bool IsImmuneToEffect(StatusEffectType effect)
        {
            return StatusImmunities.Contains(effect);
        }
        
        // Legacy property (compatibilidad)
        [JsonIgnore]
        public int Defense
        {
            get => PhysicalDefense;
            set => PhysicalDefense = value;
        }
    }
    
    /// <summary>
    /// Comportamiento de IA del enemigo
    /// </summary>
    public enum EnemyBehavior
    {
        Passive,         // Pasivo - rara vez ataca
        Defensive,       // Defensivo - prioriza bloquear/defender
        Balanced,        // Balanceado - mix de ataque y defensa
        Aggressive,      // Agresivo - siempre ataca
        Berserker,       // Berserker - sacrifica defensa por daño
        Intelligent,     // Inteligente - cambia tácticas según situación
        Coward,          // Cobarde - intenta huir si está herido
        Supportive       // Soporte - buff enemigos/debuff jugador
    }
    
    public enum EnemyDifficulty
    {
        Easy,       // Legacy: facil
        Medium,     // Legacy: medio
        Hard,       // Legacy: dificil
        Common,     // Nuevo: comun
        Uncommon,   // Nuevo: poco comun
        Rare,       // Nuevo: raro
        Elite,
        Boss,
        WorldBoss
    }
    
    public enum EnemyType
    {
        Beast,       // Animales (lobos, osos)
        Humanoid,    // Goblins, orcos, bandidos
        Undead,      // Esqueletos, zombies
        Demon,       // Demonios
        Dragon,      // Dragones
        Elemental,   // Elementales
        Aberration,  // Criaturas místicas
        Construct    // Golems
    }
}
