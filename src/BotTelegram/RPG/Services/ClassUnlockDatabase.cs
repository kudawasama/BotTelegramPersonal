using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de requisitos y datos para desbloquear clases del juego.
    /// FASE 4: Sistema de Clases Desbloqueables
    /// </summary>
    public static class ClassUnlockDatabase
    {
        /// <summary>
        /// Definición completa de los requisitos para desbloquear cada clase
        /// </summary>
        public static List<ClassUnlockDefinition> GetAllClassDefinitions()
        {
            return new List<ClassUnlockDefinition>
            {
                // ══════════════════════════════════════════════
                // TIER 1: CLASES BÁSICAS
                // ══════════════════════════════════════════════
                
                new ClassUnlockDefinition
                {
                    ClassId = "warrior",
                    TargetClass = CharacterClass.Warrior,
                    Name = "Guerrero",
                    Emoji = "⚔️",
                    Tier = ClassTier.Basic,
                    Description = "Maestro del combate cuerpo a cuerpo. Alta vida y defensa física.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "physical_attack", 100 } // 100 ataques físicos
                    },
                    RequiredLevel = 1,
                    RequiredClasses = new List<CharacterClass>(), // Ninguna
                    HintText = "Realiza *100 ataques físicos* en combate"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "mage",
                    TargetClass = CharacterClass.Mage,
                    Name = "Mago",
                    Emoji = "🔮",
                    Tier = ClassTier.Basic,
                    Description = "Maestro de la magia elemental. Alto daño a distancia.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "magic_cast", 100 } // 100 hechizos lanzados
                    },
                    RequiredLevel = 1,
                    RequiredClasses = new List<CharacterClass>(),
                    HintText = "Lanza *100 hechizos mágicos* en combate"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "rogue",
                    TargetClass = CharacterClass.Rogue,
                    Name = "Ladrón",
                    Emoji = "🗡️",
                    Tier = ClassTier.Basic,
                    Description = "Experto en sigilo y críticos devastadores.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "evasion_success", 80 } // 80 esquivas exitosas
                    },
                    RequiredLevel = 1,
                    RequiredClasses = new List<CharacterClass>(),
                    HintText = "Esquiva *80 ataques* en combate"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "cleric",
                    TargetClass = CharacterClass.Cleric,
                    Name = "Clérigo",
                    Emoji = "✨",
                    Tier = ClassTier.Basic,
                    Description = "Sanador divino. Curación y soporte del grupo.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "hp_healed", 1000 } // 1000 HP curados total
                    },
                    RequiredLevel = 1,
                    RequiredClasses = new List<CharacterClass>(),
                    HintText = "Cura un total de *1000 HP* (con ítems o habilidades)"
                },
                
                // ══════════════════════════════════════════════
                // TIER 2: CLASES AVANZADAS
                // ══════════════════════════════════════════════
                
                new ClassUnlockDefinition
                {
                    ClassId = "paladin",
                    TargetClass = CharacterClass.Paladin,
                    Name = "Paladín",
                    Emoji = "🛡️",
                    Tier = ClassTier.Advanced,
                    Description = "Caballero sagrado. Combina la fuerza del guerrero con magia divina.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "physical_attack", 500 },
                        { "damage_blocked", 300 },
                        { "hp_healed", 2000 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Warrior },
                    HintText = "Sé Guerrero Lv.10+, 500 ataques, 300 bloqueos, y cura 2000 HP"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "berserker",
                    TargetClass = CharacterClass.Berserker,
                    Name = "Berserker",
                    Emoji = "💢",
                    Tier = ClassTier.Advanced,
                    Description = "Guerrero enloquecido. Sacrifica defensa por daño devastador.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "physical_attack", 800 },
                        { "critical_hit", 200 },
                        { "damage_taken", 5000 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Warrior },
                    HintText = "Sé Guerrero Lv.10+, 800 ataques, 200 críticos, y recibe 5000 daño"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "ranger",
                    TargetClass = CharacterClass.Ranger,
                    Name = "Explorador",
                    Emoji = "🏹",
                    Tier = ClassTier.Advanced,
                    Description = "Arquero y rastreador. Maestro del campo de batalla.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "evasion_success", 400 },
                        { "physical_attack", 500 },
                        { "critical_hit", 150 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Rogue },
                    HintText = "Sé Ladrón Lv.10+, 400 esquivas, 500 ataques, 150 críticos"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "assassin",
                    TargetClass = CharacterClass.Assassin,
                    Name = "Asesino",
                    Emoji = "🔪",
                    Tier = ClassTier.Advanced,
                    Description = "Especialista en muerte silenciosa y venenos.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "evasion_success", 400 },
                        { "critical_hit", 300 },
                        { "enemy_kill", 150 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Rogue },
                    HintText = "Sé Ladrón Lv.10+, 400 esquivas, 300 críticos, 150 kills"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "warlock",
                    TargetClass = CharacterClass.Warlock,
                    Name = "Brujo",
                    Emoji = "🔥",
                    Tier = ClassTier.Advanced,
                    Description = "Mago que pacta con fuerzas oscuras para obtener poder ilimitado.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "magic_cast", 500 },
                        { "mana_spent", 5000 },
                        { "enemy_kill", 200 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Mage },
                    HintText = "Sé Mago Lv.10+, 500 hechizos, 5000 de maná gastado, 200 kills"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "high_priest",
                    TargetClass = CharacterClass.Monk, // Usando Monk como High Priest temporalmente
                    Name = "Sumo Sacerdote",
                    Emoji = "💫",
                    Tier = ClassTier.Advanced,
                    Description = "Curandero supremo con poderes divinos de resurrección.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "hp_healed", 10000 },
                        { "buff_applied", 200 },
                        { "combat_survived", 50 }
                    },
                    RequiredLevel = 10,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Cleric },
                    HintText = "Sé Clérigo Lv.10+, cura 10000 HP, aplica 200 buffs, sobrevive 50 combates"
                },
                
                // ══════════════════════════════════════════════
                // TIER 3: CLASES ÉPICAS
                // ══════════════════════════════════════════════
                
                new ClassUnlockDefinition
                {
                    ClassId = "necromancer",
                    TargetClass = CharacterClass.Necromancer,
                    Name = "Nigromante",
                    Emoji = "💀",
                    Tier = ClassTier.Epic,
                    Description = "Maestro de la muerte. Convoca ejércitos de no-muertos.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "magic_cast", 1000 },
                        { "minion_summoned", 500 },
                        { "enemy_kill", 500 },
                        { "boss_kill", 10 }
                    },
                    RequiredLevel = 20,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Warlock },
                    HintText = "Sé Brujo Lv.20+, 1000 hechizos, 500 invocaciones, 500 kills, 10 bosses"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "sorcerer",
                    TargetClass = CharacterClass.Sorcerer,
                    Name = "Hechicero",
                    Emoji = "🌟",
                    Tier = ClassTier.Epic,
                    Description = "Maestro de todos los elementos. Daño mágico sin precedentes.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "magic_cast", 1500 },
                        { "mana_spent", 20000 },
                        { "boss_kill", 15 }
                    },
                    RequiredLevel = 20,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Mage, CharacterClass.Warlock },
                    HintText = "Sé Mago+Brujo Lv.20+, 1500 hechizos, 20000 maná, 15 bosses"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "druid",
                    TargetClass = CharacterClass.Druid,
                    Name = "Druida",
                    Emoji = "🌿",
                    Tier = ClassTier.Epic,
                    Description = "Guardián de la naturaleza. Transforma entre formas animales.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "hp_healed", 30000 },
                        { "pet_tamed", 10 },
                        { "exploration_count", 200 }
                    },
                    RequiredLevel = 20,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Cleric },
                    HintText = "Sé Clérigo Lv.20+, cura 30000 HP, domestica 10 mascotas, explora 200 veces"
                },
                
                new ClassUnlockDefinition
                {
                    ClassId = "bard",
                    TargetClass = CharacterClass.Bard,
                    Name = "Bardo",
                    Emoji = "🎵",
                    Tier = ClassTier.Epic,
                    Description = "Maestro de inspiración. Eleva a sus aliados a nuevas alturas.",
                    RequiredActions = new Dictionary<string, int>
                    {
                        { "buff_applied", 500 },
                        { "combat_survived", 200 },
                        { "boss_kill", 20 }
                    },
                    RequiredLevel = 20,
                    RequiredClasses = new List<CharacterClass> { CharacterClass.Cleric, CharacterClass.Rogue },
                    HintText = "Sé Clérigo+Ladrón Lv.20+, 500 buffs, 200 combates sobrevividos, 20 bosses"
                }
            };
        }
        
        /// <summary>
        /// Obtiene la definición de una clase por su CharacterClass
        /// </summary>
        public static ClassUnlockDefinition? GetDefinition(CharacterClass characterClass)
        {
            return GetAllClassDefinitions().FirstOrDefault(d => d.TargetClass == characterClass);
        }
        
        /// <summary>
        /// Obtiene clases disponibles para desbloquear (requisitos de clase previa cumplidos)
        /// </summary>
        public static List<ClassUnlockDefinition> GetAvailableToUnlock(RpgPlayer player)
        {
            var unlockedClassIds = player.UnlockedClasses;
            
            return GetAllClassDefinitions().Where(def =>
            {
                // Ya está desbloqueada
                if (unlockedClassIds.Contains(def.ClassId)) return false;
                
                // Verificar que tiene todas las clases previas
                foreach (var requiredClass in def.RequiredClasses)
                {
                    var requiredDef = GetDefinition(requiredClass);
                    if (requiredDef != null && !unlockedClassIds.Contains(requiredDef.ClassId))
                        return false;
                }
                
                return true;
            }).ToList();
        }
        
        /// <summary>
        /// Calcula el progreso (0.0 a 1.0) para desbloquear una clase
        /// </summary>
        public static double GetUnlockProgress(RpgPlayer player, ClassUnlockDefinition def)
        {
            if (def.RequiredActions.Count == 0) return 1.0;
            
            double total = 0;
            foreach (var (actionId, required) in def.RequiredActions)
            {
                var current = player.ActionCounters.TryGetValue(actionId, out var val) ? val : 0;
                total += Math.Min(1.0, (double)current / required);
            }
            
            return total / def.RequiredActions.Count;
        }
        
        /// <summary>
        /// Verifica si el jugador puede desbloquear una clase
        /// </summary>
        public static bool CanUnlock(RpgPlayer player, ClassUnlockDefinition def)
        {
            if (player.Level < def.RequiredLevel) return false;
            
            foreach (var requiredClass in def.RequiredClasses)
            {
                var requiredDef = GetDefinition(requiredClass);
                if (requiredDef != null && !player.UnlockedClasses.Contains(requiredDef.ClassId))
                    return false;
            }
            
            foreach (var (actionId, required) in def.RequiredActions)
            {
                var current = player.ActionCounters.TryGetValue(actionId, out var val) ? val : 0;
                if (current < required) return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Nombre legible de un action ID
        /// </summary>
        public static string GetActionName(string actionId) => actionId switch
        {
            "physical_attack"   => "Ataques físicos",
            "magic_cast"        => "Hechizos lanzados",
            "evasion_success"   => "Esquivas exitosas",
            "hp_healed"         => "HP curado",
            "damage_blocked"    => "Daño bloqueado",
            "damage_taken"      => "Daño recibido",
            "critical_hit"      => "Golpes críticos",
            "enemy_kill"        => "Enemigos derrotados",
            "boss_kill"         => "Jefes derrotados",
            "mana_spent"        => "Maná gastado",
            "minion_summoned"   => "Invocaciones",
            "buff_applied"      => "Buffs aplicados",
            "combat_survived"   => "Combates sobrevividos",
            "pet_tamed"         => "Mascotas domadas",
            "exploration_count" => "Exploraciones",
            _ => actionId
        };
        
        /// <summary>
        /// Nombre del tier
        /// </summary>
        public static string GetTierName(ClassTier tier) => tier switch
        {
            ClassTier.Basic    => "Básica",
            ClassTier.Advanced => "Avanzada",
            ClassTier.Epic     => "Épica",
            ClassTier.Legendary => "Legendaria",
            _ => "Oculta"
        };
        
        public static string GetTierEmoji(ClassTier tier) => tier switch
        {
            ClassTier.Basic    => "⭐",
            ClassTier.Advanced => "⭐⭐",
            ClassTier.Epic     => "⭐⭐⭐",
            ClassTier.Legendary => "👑",
            _ => "❓"
        };
    }
    
    /// <summary>
    /// Definición completa de los requisitos para desbloquear una clase
    /// </summary>
    public class ClassUnlockDefinition
    {
        public string ClassId { get; set; } = "";
        public CharacterClass TargetClass { get; set; }
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "";
        public ClassTier Tier { get; set; }
        public string Description { get; set; } = "";
        public Dictionary<string, int> RequiredActions { get; set; } = new();
        public int RequiredLevel { get; set; } = 1;
        public List<CharacterClass> RequiredClasses { get; set; } = new();
        public string HintText { get; set; } = "";
    }
    
    /// <summary>
    /// Tier/rango de una clase
    /// </summary>
    public enum ClassTier
    {
        Basic,
        Advanced,
        Epic,
        Legendary,
        Hidden
    }
}
