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
                    { "pet_beast", 250 },         // Acariciar bestias 250 veces (x5)
                    { "calm_beast", 150 },        // Calmar bestias agresivas 150 veces (x5)
                    { "tame_beast", 500 },        // Usar habilidad Domar 500 veces (x5)
                    { "meditation", 200 },        // Meditar 200 veces (x2)
                    { "beast_kills", 800 }        // Matar 800 bestias/animales (x4)
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
                    { "stealth_kill", 300 },      // Matar desde el sigilo 300 veces (x3)
                    { "critical_hit", 1500 },     // 1500 golpes críticos (x3)
                    { "dodge_success", 800 },     // Esquivar 800 ataques (x2.6)
                    { "backstab", 500 },          // Atacar por la espalda 500 veces (x3.3)
                    { "vanish", 150 }             // Usar habilidad Desvanecerse 150 veces (x3)
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
                    { "heal_cast", 2000 },        // Curar 2000 veces (x4)
                    { "revive_ally", 50 },        // Revivir aliados 50 veces (futuro: party system) (x2.5)
                    { "divine_bless", 300 },      // Bendecir 300 veces (x3)
                    { "meditation", 500 },        // Meditar 500 veces (x1.6)
                    { "undead_kills", 600 }       // Matar 600 no-muertos (x3)
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
                    { "dark_magic_cast", 1000 },  // Lanzar magia oscura 1000 veces (x2.5)
                    { "summon_undead", 800 },     // Invocar no-muertos 800 veces (x4)
                    { "life_drain", 600 },        // Drenar vida 600 veces (x2)
                    { "desecrate", 300 },         // Profanar 300 veces (x3)
                    { "sacrifice", 200 }          // Sacrificar HP por poder 200 veces (x4)
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
                    { "fire_spell_cast", 500 },   // Lanzar hechizos de fuego 500 veces (x2.5)
                    { "water_spell_cast", 500 },  // Lanzar hechizos de agua 500 veces (x2.5)
                    { "earth_spell_cast", 500 },  // Lanzar hechizos de tierra 500 veces (x2.5)
                    { "air_spell_cast", 500 },    // Lanzar hechizos de aire 500 veces (x2.5)
                    { "combo_spell", 300 }        // Combinar elementos 300 veces (x3)
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
                    { "combo_10x", 300 },         // Hacer combo de 10+ ataques 300 veces (x3)
                    { "combo_20x", 150 },         // Hacer combo de 20+ ataques 150 veces (x3)
                    { "perfect_parry", 600 },     // Parrys perfectos 600 veces (x3)
                    { "dodge_success", 800 },     // Esquivar 800 ataques (x1.6)
                    { "no_damage_combat", 300 }   // Ganar 300 combates sin recibir daño (x3)
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
            },
            
            // ═══════════════════════════════════════════════════════════════
            // FORTRESS KNIGHT - Caballero Fortaleza (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "fortress_knight",
                Name = "Caballero Fortaleza",
                Emoji = "🛡️",
                Description = "Maestro defensivo impenetrable. Puede bloquear daño masivo sin inmutarse.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "block_damage", 5000 },         // Bloquear 5000 de daño total
                    { "perfect_block", 200 },         // Bloqueos perfectos (100% damage negated)
                    { "tank_hit", 1000 },             // Recibir 1000 golpes
                    { "survive_lethal", 50 },         // Sobrevivir a 50 golpes letales (>80% HP)
                    { "taunt_enemy", 300 },           // Provocar enemigos 300 veces
                    { "shield_bash", 150 }            // Golpear con escudo 150 veces
                },
                GrantedPassives = new List<string>
                {
                    "unbreakable_defense",            // +50% block chance, +30 Physical Defense
                    "damage_reflection",              // 25% del daño bloqueado se refleja
                    "shield_mastery"                  // Escudos otorgan +50% stats
                },
                UnlockedSkills = new List<string>
                {
                    "fortress_stance",                // Modo tanque (+100% DEF, -50% daño, 5 turnos)
                    "shield_wall",                    // Inmune a críticos por 3 turnos
                    "guardian_aura"                   // Aliados (pets) reciben -30% daño
                },
                ConstitutionBonus = 30,
                StrengthBonus = 10,
                WisdomBonus = -5
            },
            
            // ═══════════════════════════════════════════════════════════════
            // IMMOVABLE MOUNTAIN - Montaña Inamovible (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "immovable_mountain",
                Name = "Montaña Inamovible",
                Emoji = "⛰️",
                Description = "Nadie puede mover ni derribar esta roca viviente. La última línea de defensa.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "damage_taken", 8000 },         // Recibir 8000 de daño total
                    { "survive_critical", 100 },      // Sobrevivir a 100 críticos
                    { "hp_below_10_survive", 30 },    // Sobrevivir con <10% HP 30 veces
                    { "no_dodge_combat", 200 },       // Completar 200 combates sin esquivar
                    { "heavy_armor_use", 500 }        // Usar armadura pesada 500 combates
                },
                GrantedPassives = new List<string>
                {
                    "stone_skin",                     // Reducción de daño fija 15 (antes de DEF)
                    "last_stand",                     // Al llegar a 1 HP, recupera 40% HP una vez por combate
                    "immovable"                       // Inmune a Stun y Knockback
                },
                UnlockedSkills = new List<string>
                {
                    "earthquake",                     // Daño AoE, aturde enemigos
                    "stone_shell",                    // Invulnerabilidad 1 turno (Cooldown 10)
                    "titan_grip"                      // Armas pesadas no penalizan velocidad
                },
                ConstitutionBonus = 40,
                StrengthBonus = 15,
                DexterityBonus = -10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // BERSERKER BLOOD RAGE - Berserker Furia Sangrienta (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "berserker_blood_rage",
                Name = "Berserker Furia Sangrienta",
                Emoji = "🩸",
                Description = "Sacrifica defensa por daño devastador. Más peligroso cuanto menor HP tiene.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "critical_hit", 1000 },         // 1000 críticos
                    { "hp_below_30_kill", 150 },      // Matar 150 enemigos con <30% HP
                    { "no_armor_combat", 100 },       // Combatir sin armadura 100 veces
                    { "consecutive_attacks", 500 },   // 500 ataques consecutivos sin fallar
                    { "overkill_damage", 200 }        // Hacer overkill (exceso daño) 200 veces
                },
                GrantedPassives = new List<string>
                {
                    "blood_frenzy",                   // +5% daño por cada 10% HP perdido (máx +50%)
                    "reckless_abandon",               // +50% daño, -30% DEF
                    "killing_spree"                   // Cada kill otorga +10% daño por 3 turnos (stackeable x5)
                },
                UnlockedSkills = new List<string>
                {
                    "sacrifice",                      // Consume 40% HP, próximo ataque hace 400% daño
                    "rampage",                        // 6 ataques rápidos (60% daño cada uno)
                    "blood_pact"                      // Convierte HP en daño extra (2 HP = 1 ATK)
                },
                StrengthBonus = 35,
                ConstitutionBonus = -15,
                DexterityBonus = 20
            },
            
            // ═══════════════════════════════════════════════════════════════
            // ARCANE SIPHONER - Sifón Arcano (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "arcane_siphoner",
                Name = "Sifón Arcano",
                Emoji = "🔮",
                Description = "Roba mana de enemigos y convierte todo en daño mágico devastador.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 1200 },         // 1200 ataques mágicos
                    { "mana_spent", 10000 },          // Gastar 10000 de mana total
                    { "low_mana_cast", 200 },         // Castear con <20% mana 200 veces
                    { "mana_drain", 300 },            // Drenar mana de enemigos 300 veces
                    { "spell_critical", 400 }         // 400 críticos mágicos
                },
                GrantedPassives = new List<string>
                {
                    "arcane_overflow",                // Cada spell que excede MaxMana hace +50% daño
                    "mana_burn",                      // Spells consumen HP si no hay mana (2 HP = 1 Mana)
                    "spell_amplification"             // +60% daño mágico, -30% Physical Defense
                },
                UnlockedSkills = new List<string>
                {
                    "mana_void",                      // Drena todo mana enemigo, daño = mana robado x3
                    "arcane_cascade",                 // 8 mini-spells (30% daño cada uno, 5 mana c/u)
                    "spell_leech"                     // Siguiente spell recupera mana = daño hecho / 4
                },
                IntelligenceBonus = 45,
                ConstitutionBonus = -10,
                WisdomBonus = 25
            },
            
            // ═══════════════════════════════════════════════════════════════
            // LIFE WEAVER - Tejedor de Vida (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "life_weaver",
                Name = "Tejedor de Vida",
                Emoji = "🌸",
                Description = "Maestro absoluto de la curación. Puede revivir y regenerar infinitamente.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 2000 },            // Curar 2000 veces
                    { "hp_restored", 50000 },         // Restaurar 50000 HP total
                    { "full_heal", 300 },             // Curar de 0% a 100% en un cast, 300 veces
                    { "meditation", 500 },            // Meditar 500 veces
                    { "survive_poison", 100 },        // Sobrevivir a envenenamiento 100 veces
                    { "no_damage_turn", 800 }         // 800 turnos sin atacar (solo curar)
                },
                GrantedPassives = new List<string>
                {
                    "divine_touch",                   // Heals curan +100%
                    "regeneration_aura",              // Recupera 10% HP cada turno
                    "life_link"                       // Al morir, revive con 60% HP (Cooldown: 1 por combate)
                },
                UnlockedSkills = new List<string>
                {
                    "mass_heal",                      // Cura AoE (jugador + pets) 80% MaxHP
                    "resurrection",                   // Revive pet con 100% HP
                    "sanctuary"                       // Zona inmune a daño por 2 turnos
                },
                WisdomBonus = 40,
                IntelligenceBonus = 20,
                CharismaBonus = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // PUPPET MASTER - Maestro Titiritero (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "puppet_master",
                Name = "Maestro Titiritero",
                Emoji = "🎭",
                Description = "Controla mentes y cuerpos. Convierte enemigos en aliados temporales.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "mind_control", 200 },          // Controlar mentalmente 200 enemigos
                    { "confusion_inflict", 300 },     // Confundir enemigos 300 veces
                    { "charm_beast", 250 },           // Encantar bestias 250 veces
                    { "puppet_kill", 150 },           // Kills hechos por enemigos controlados
                    { "manipulation", 400 }           // Manipular acciones enemigas 400 veces
                },
                GrantedPassives = new List<string>
                {
                    "master_manipulator",             // +30% duración de control mental
                    "puppet_strings",                 // Enemigos controlados hacen +50% daño
                    "mind_immunity"                   // Inmune a control mental y confusión
                },
                UnlockedSkills = new List<string>
                {
                    "dominate",                       // Controla enemigo por 4 turnos
                    "mass_confusion",                 // Confunde todos los enemigos (AoE)
                    "possession"                      // Posees enemigo, controlas sus habilidades
                },
                CharismaBonus = 30,
                IntelligenceBonus = 25,
                WisdomBonus = 10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // TIME BENDER - Manipulador Temporal (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "time_bender",
                Name = "Manipulador Temporal",
                Emoji = "⏰",
                Description = "Controla el flujo del tiempo. Puede acelerar, ralentizar y repetir acciones.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "dodge_success", 800 },         // Esquivar 800 ataques
                    { "speed_advantage", 500 },       // Ganar 500 turnos por velocidad
                    { "double_turn", 200 },           // Actuar 2 veces por turno 200 veces
                    { "time_stop_use", 100 },         // Usar habilidades de tiempo 100 veces
                    { "perfect_timing", 300 }         // Contraataques perfectos 300 veces
                },
                GrantedPassives = new List<string>
                {
                    "temporal_flux",                  // +50% velocidad base
                    "foresight",                      // Ve próximo movimiento enemigo
                    "time_loop"                       // 10% chance de repetir acción gratis
                },
                UnlockedSkills = new List<string>
                {
                    "haste",                          // Actúa 3 veces en 1 turno
                    "time_stop",                      // Enemigo pierde próximo turno
                    "rewind"                          // Revierte último turno (recupera HP/Mana perdido)
                },
                DexterityBonus = 35,
                IntelligenceBonus = 20,
                WisdomBonus = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // ELEMENTAL OVERLORD - Señor Elemental (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "elemental_overlord",
                Name = "Señor Elemental",
                Emoji = "🌊🔥❄️⚡",
                Description = "Controla todos los elementos simultáneamente. Puede crear tormentas elementales.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "fire_spell_cast", 500 },       // 500 hechizos de fuego
                    { "water_spell_cast", 500 },      // 500 hechizos de agua
                    { "earth_spell_cast", 500 },      // 500 hechizos de tierra
                    { "air_spell_cast", 500 },        // 500 hechizos de aire
                    { "combo_spell", 300 },           // 300 combos elementales
                    { "elemental_mastery", 250 }      // 250 kills con ventaja elemental
                },
                GrantedPassives = new List<string>
                {
                    "elemental_fusion",               // Spells combinan 2 elementos
                    "elemental_immunity",             // Inmune a daño elemental
                    "primal_force"                    // Daño elemental +80%
                },
                UnlockedSkills = new List<string>
                {
                    "elemental_storm",                // AoE masivo los 4 elementos
                    "elemental_avatar",               // Transforma en elemental puro (5 turnos)
                    "element_swap"                    // Cambia debilidades/resistencias enemigo
                },
                IntelligenceBonus = 50,
                WisdomBonus = 30,
                ConstitutionBonus = 10
            },
            
            // ═══════════════════════════════════════════════════════════════
            // BEAST LORD - Señor de las Bestias (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "beast_lord",
                Name = "Señor de las Bestias",
                Emoji = "🐲",
                Description = "Evolución de Beast Tamer. Puede tener 3 mascotas simultáneas y fusionarlas.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "pet_beast", 500 },             // Acariciar 500 bestias
                    { "calm_beast", 300 },            // Calmar 300 bestias
                    { "tame_beast", 500 },            // Domar 500 bestias
                    { "beast_kills", 800 },           // Matar 800 bestias
                    { "pet_bond_max", 100 },          // Llevar 100 mascotas a bond máximo
                    { "pet_evolution", 50 }           // Evolucionar 50 mascotas
                },
                GrantedPassives = new List<string>
                {
                    "beast_army",                     // +2 slots de mascota (total 3)
                    "alpha_dominance",                // Mascotas hacen +100% daño
                    "beast_fusion"                    // Puede fusionar 2 mascotas temporalmente
                },
                UnlockedSkills = new List<string>
                {
                    "beast_stampede",                 // Las 3 mascotas atacan juntas
                    "primal_roar",                    // Mascotas entran en furia (+150% ATK, 3 turnos)
                    "beast_sacrifice"                 // Sacrifica mascota para revivir jugador
                },
                CharismaBonus = 30,
                WisdomBonus = 25,
                DexterityBonus = 15
            },
            
            // ═══════════════════════════════════════════════════════════════
            // LICH KING - Rey Lich (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "lich_king",
                Name = "Rey Lich",
                Emoji = "💀👑",
                Description = "Evolución de Necromancer. Ejército de no-muertos imparable.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "summon_undead", 800 },         // Invocar 800 no-muertos
                    { "dark_magic_cast", 1000 },      // 1000 hechizos oscuros
                    { "life_drain", 600 },            // Drenar vida 600 veces
                    { "undead_kills", 500 },          // Matar 500 no-muertos
                    { "sacrifice", 200 },             // Sacrificar minions 200 veces
                    { "desecrate", 300 }              // Profanar 300 veces
                },
                GrantedPassives = new List<string>
                {
                    "undead_mastery",                 // +3 slots de minion (total 5 minions)
                    "death_aura",                     // Enemigos pierden 5% MaxHP cada turno
                    "phylactery"                      // Si mueres con >3 minions, revives con 50% HP
                },
                UnlockedSkills = new List<string>
                {
                    "army_of_dead",                   // Invoca 5 esqueletos
                    "death_and_decay",                // AoE masivo que crea minions de cadáveres
                    "lich_form"                       // Transforma en lich (inmune a físico, +200% magic)
                },
                IntelligenceBonus = 40,
                WisdomBonus = 20,
                ConstitutionBonus = -20
            },
            
            // ═══════════════════════════════════════════════════════════════
            // VOID SUMMONER - Invocador del Vacío (NUEVO - FASE 3)
            // ═══════════════════════════════════════════════════════════════
            new HiddenClass
            {
                Id = "void_summoner",
                Name = "Invocador del Vacío",
                Emoji = "👁️",
                Description = "Invoca criaturas del vacío. Pactos peligrosos con entidades cósmicas.",
                RequiredActions = new Dictionary<string, int>
                {
                    { "void_ritual", 300 },           // Realizar rituales del vacío 300 veces
                    { "summon_aberration", 400 },     // Invocar aberraciones 400 veces
                    { "pact_damage", 5000 },          // Recibir 5000 daño de pactos
                    { "sacrifice_hp", 10000 },        // Sacrificar 10000 HP en rituales
                    { "void_gaze", 200 }              // Mirar al vacío 200 veces (causa daño)
                },
                GrantedPassives = new List<string>
                {
                    "eldritch_pact",                  // Invocaciones cuestan HP en vez de mana
                    "void_touched",                   // +100% daño void, -50% sanity
                    "beyond_death"                    // Revive como aberración si mueres (1 vez por día)
                },
                UnlockedSkills = new List<string>
                {
                    "summon_horror",                  // Invoca entidad cósmica (hace 300% daño, incontrolable)
                    "void_gate",                      // Portal que invoca aberraciones aleatorias
                    "eldritch_blast"                  // Rayo void que ignora defensas
                },
                IntelligenceBonus = 50,
                WisdomBonus = -20,
                CharismaBonus = 30
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
