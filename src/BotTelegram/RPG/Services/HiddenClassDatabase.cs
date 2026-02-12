using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de todas las clases ocultas y sus requisitos de desbloqueo
    /// </summary>
    public static class HiddenClassDatabase
    {
        private static readonly List<HiddenClass> _classes = new()
        {
            // ═══════════════════════════════════════════════════════════════
            // BEAST TAMER - Domador de Bestias
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "beast_tamer",
                Name = "Domador de Bestias",
                Emoji = "🐺",
                Description = "Maestro en el arte de comunicarse con criaturas salvajes. Puede domar bestias y luchar junto a ellas.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "pet_beast", 50 },          // Acariciar bestias 50 veces
                    { "calm_beast", 30 },         // Calmar bestias agresivas 30 veces
                    { "tame_beast", 100 },        // Usar habilidad Domar 100 veces
                    { "meditation", 100 },        // Meditar 100 veces
                    { "beast_kills", 200 }        // Matar 200 bestias/animales
                },
                GrantedPassives = new List<string>
                {
                    "beast_whisperer",            // Puede comunicarse con bestias
                    "beast_companion",            // Bestia acompañante en combate (+20% daño)
                    "beast_empathy"               // Bestias no te atacan al explorar
                },
                UnlockedSkills = new List<string>
                {
                    "tame_beast",                 // Domar bestia salvaje
                    "beast_fury",                 // Tu bestia ataca ferozmente
                    "beast_heal"                  // Curar a tu bestia
                },
                StrengthBonus = 5,
                DexterityBonus = 10,
                WisdomBonus = 15,
                CharismaBonus = 10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // SHADOW WALKER - Caminante de las Sombras
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "shadow_walker",
                Name = "Caminante de las Sombras",
                Emoji = "👤",
                Description = "Maestro del sigilo y la oscuridad. Se mueve entre las sombras sin ser detectado.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "stealth_kill", 100 },      // Matar desde el sigilo 100 veces
                    { "critical_hit", 500 },      // 500 golpes críticos
                    { "dodge_success", 300 },     // Esquivar 300 ataques
                    { "backstab", 150 },          // Atacar por la espalda 150 veces
                    { "vanish", 50 }              // Usar habilidad Desvanecerse 50 veces
                },
                GrantedPassives = new List<string>
                {
                    "shadow_step",                // Puede atacar desde las sombras (+50% crítico)
                    "night_vision",               // Ve en la oscuridad
                    "silent_movement"             // No hace ruido al moverse
                },
                UnlockedSkills = new List<string>
                {
                    "shadow_strike",              // Ataque desde las sombras (200% daño)
                    "vanish",                     // Desaparecer en las sombras (evade próximo ataque)
                    "shadow_clone"                // Crear clon de sombra (confunde enemigos)
                },
                DexterityBonus = 20,
                IntelligenceBonus = 5,
                CharismaBonus = 5
            },
            
            // ═══════════════════════════════════════════════════════════════
            // DIVINE PROPHET - Profeta Divino
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "divine_prophet",
                Name = "Profeta Divino",
                Emoji = "⛪",
                Description = "Bendecido por los dioses. Sus curaciones son milagrosas y puede resucitar a los caídos.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 500 },         // Curar 500 veces
                    { "revive_ally", 20 },        // Revivir aliados 20 veces (futuro: party system)
                    { "divine_bless", 100 },      // Bendecir 100 veces
                    { "meditation", 300 },        // Meditar 300 veces
                    { "undead_kills", 200 }       // Matar 200 no-muertos
                },
                GrantedPassives = new List<string>
                {
                    "divine_blessing",            // Heals +50% más efectivos
                    "holy_aura",                  // Regenera 5% HP por turno
                    "resurrection"                // Auto-revive 1 vez por combate
                },
                UnlockedSkills = new List<string>
                {
                    "divine_intervention",        // Evita muerte (1 vez por combate)
                    "mass_heal",                  // Curación masiva
                    "holy_smite"                  // Ataque sagrado vs no-muertos (300% daño)
                },
                IntelligenceBonus = 10,
                WisdomBonus = 20,
                CharismaBonus = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // NECROMANCER LORD - Señor Nigromante
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "necromancer_lord",
                Name = "Señor Nigromante",
                Emoji = "💀",
                Description = "Maestro de la magia oscura y la nigromancia. Puede invocar a los muertos para luchar.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "dark_magic_cast", 400 },   // Lanzar magia oscura 400 veces
                    { "summon_undead", 200 },     // Invocar no-muertos 200 veces
                    { "life_drain", 300 },        // Drenar vida 300 veces
                    { "desecrate", 100 },         // Profanar 100 veces
                    { "sacrifice", 50 }           // Sacrificar HP por poder 50 veces
                },
                GrantedPassives = new List<string>
                {
                    "necrotic_touch",             // Ataqués causan daño oscuro (+20 daño)
                    "lichdom",                    // 50% less daño de fuentes físicas
                    "soul_harvest"                // Obtiene +20% XP de enemigos
                },
                UnlockedSkills = new List<string>
                {
                    "raise_undead",               // Invocar esqueleto guerrero
                    "death_coil",                 // Proyectil oscuro (daño + heal)
                    "dark_pact"                   // Sacrifica HP por mana y daño
                },
                IntelligenceBonus = 25,
                ConstitutionBonus = -5,
                WisdomBonus = 10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // ELEMENTAL SAGE - Sabio Elemental
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "elemental_sage",
                Name = "Sabio Elemental",
                Emoji = "🌊",
                Description = "Maestro de todos los elementos. Puede combinar fuego, agua, tierra y aire en ataques devastadores.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "fire_spell_cast", 200 },   // Lanzar hechizos de fuego 200 veces
                    { "water_spell_cast", 200 },  // Lanzar hechizos de agua 200 veces
                    { "earth_spell_cast", 200 },  // Lanzar hechizos de tierra 200 veces
                    { "air_spell_cast", 200 },    // Lanzar hechizos de aire 200 veces
                    { "combo_spell", 100 }        // Combinar elementos 100 veces
                },
                GrantedPassives = new List<string>
                {
                    "elemental_affinity",         // +30% resistencia elemental
                    "elemental_mastery",          // Hechizos cuestan -20% mana
                    "primal_force"                // +15% daño mágico
                },
                UnlockedSkills = new List<string>
                {
                    "elemental_blast",            // Ráfaga de todos los elementos
                    "elemental_shield",           // Escudo elemental rotatorio
                    "meteor_storm"                // Tormenta de meteoros (AOE)
                },
                IntelligenceBonus = 30,
                WisdomBonus = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // BLADE DANCER - Danzante de Espadas
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "blade_dancer",
                Name = "Danzante de Espadas",
                Emoji = "⚔️",
                Description = "Artista marcial que convierte el combate en danza. Sus combos son imparables.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "combo_10x", 100 },         // Hacer combo de 10+ ataques 100 veces
                    { "combo_20x", 50 },          // Hacer combo de 20+ ataques 50 veces
                    { "perfect_parry", 200 },     // Parrys perfectos 200 veces
                    { "dodge_success", 500 },     // Esquivar 500 ataques
                    { "no_damage_combat", 100 }   // Ganar 100 combates sin recibir daño
                },
                GrantedPassives = new List<string>
                {
                    "blade_dancer",               // Combo no se resetea al fallar
                    "flow_state",                 // +5% daño por cada hit del combo
                    "graceful_fighter"            // +20% evasión durante combos
                },
                UnlockedSkills = new List<string>
                {
                    "blade_storm",                // Ataque múltiple (5 hits)
                    "perfect_counter",            // Contraataque perfecto
                    "dance_of_death"              // Secuencia de ataques coreografiados
                },
                StrengthBonus = 15,
                DexterityBonus = 25,
                ConstitutionBonus = 5
            }
        };
        
        public static List<HiddenClass> GetAll() => _classes;
        
        public static HiddenClass? GetById(string id) => _classes.FirstOrDefault(c => c.Id == id);
        
        public static List<HiddenClass> GetAvailableForPlayer(RpgPlayer player)
        {
            // Retorna clases que el jugador aún no ha desbloqueado
            return _classes.Where(c => !player.UnlockedHiddenClasses.Contains(c.Id)).ToList();
        }
        
        public static List<HiddenClass> GetUnlockedByPlayer(RpgPlayer player)
        {
            return _classes.Where(c => player.UnlockedHiddenClasses.Contains(c.Id)).ToList();
        }
    }
}
