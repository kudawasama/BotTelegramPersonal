using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de habilidades desbloqueables mediante combinaciones de acciones
    /// </summary>
    public static class SkillUnlockDatabase
    {
        public class ComboSkillRequirement
        {
            public string SkillId { get; set; } = "";
            public Dictionary<string, int> RequiredActions { get; set; } = new();
        }

        private static readonly List<ComboSkillRequirement> _requirements = new()
        {
            // ═══════════════════════════════════════════════════════════════
            // COMBATE FÍSICO - Skills de ataque cuerpo a cuerpo
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "charge_strike",
                RequiredActions = new Dictionary<string, int>
                {
                    { "physical_attack", 100 },
                    { "critical_hit", 50 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "rampage",
                RequiredActions = new Dictionary<string, int>
                {
                    { "physical_attack", 300 },
                    { "kill_count", 100 },
                    { "critical_hit", 100 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "execute",
                RequiredActions = new Dictionary<string, int>
                {
                    { "kill_count", 200 },
                    { "critical_hit", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "whirlwind",
                RequiredActions = new Dictionary<string, int>
                {
                    { "physical_attack", 250 },
                    { "dodge_success", 100 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "blood_strike",
                RequiredActions = new Dictionary<string, int>
                {
                    { "physical_attack", 200 },
                    { "damage_taken", 2000 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // COMBATE MÁGICO - Skills de hechizos elementales
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "meteor_storm",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 300 },
                    { "mana_spent", 5000 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "arcane_burst",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 150 },
                    { "mana_spent", 3000 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "mana_void",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 200 },
                    { "mana_regen", 3000 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "elemental_fury",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 400 },
                    { "kill_count", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "chaos_bolt",
                RequiredActions = new Dictionary<string, int>
                {
                    { "magic_attack", 250 },
                    { "critical_hit", 80 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // DEFENSA Y TANQUE - Skills de supervivencia
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "iron_fortress",
                RequiredActions = new Dictionary<string, int>
                {
                    { "block_success", 100 },
                    { "damage_taken", 3000 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "shield_wall",
                RequiredActions = new Dictionary<string, int>
                {
                    { "block_success", 150 },
                    { "combat_count", 50 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "last_stand",
                RequiredActions = new Dictionary<string, int>
                {
                    { "damage_taken", 5000 },
                    { "survived_battles", 100 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "counter_strike",
                RequiredActions = new Dictionary<string, int>
                {
                    { "block_success", 80 },
                    { "critical_hit", 60 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "guardian_aura",
                RequiredActions = new Dictionary<string, int>
                {
                    { "damage_taken", 4000 },
                    { "pet_beast", 100 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // SIGILO Y CRÍTICOS - Skills de asesino
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "assassinate",
                RequiredActions = new Dictionary<string, int>
                {
                    { "critical_hit", 200 },
                    { "dodge_success", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "shadow_dance",
                RequiredActions = new Dictionary<string, int>
                {
                    { "dodge_success", 200 },
                    { "physical_attack", 200 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "backstab_mastery",
                RequiredActions = new Dictionary<string, int>
                {
                    { "critical_hit", 300 },
                    { "kill_count", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "vanishing_strike",
                RequiredActions = new Dictionary<string, int>
                {
                    { "dodge_success", 250 },
                    { "critical_hit", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "lethal_precision",
                RequiredActions = new Dictionary<string, int>
                {
                    { "critical_hit", 500 },
                    { "kill_count", 200 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // SOPORTE Y CURACIÓN - Skills de sanador
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "divine_intervention",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 300 },
                    { "meditation", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "mass_resurrection",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 500 },
                    { "survived_battles", 150 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "holy_nova",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 200 },
                    { "magic_attack", 100 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "life_transfer",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 250 },
                    { "damage_taken", 2000 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "sanctuary",
                RequiredActions = new Dictionary<string, int>
                {
                    { "heal_cast", 400 },
                    { "meditation", 200 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // INVOCACIÓN Y MASCOTAS - Skills de domador/necromante
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "beast_stampede",
                RequiredActions = new Dictionary<string, int>
                {
                    { "pet_beast", 200 },
                    { "tame_beast", 50 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "army_of_dead",
                RequiredActions = new Dictionary<string, int>
                {
                    { "summon_undead", 300 },
                    { "kill_count", 200 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "pet_fusion",
                RequiredActions = new Dictionary<string, int>
                {
                    { "pet_beast", 300 },
                    { "tame_beast", 100 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "dark_pact",
                RequiredActions = new Dictionary<string, int>
                {
                    { "summon_undead", 200 },
                    { "meditation", 150 }
                }
            },
            
            // ═══════════════════════════════════════════════════════════════
            // HÍBRIDAS - Skills que combinan múltiples estilos
            // ═══════════════════════════════════════════════════════════════
            new ComboSkillRequirement
            {
                SkillId = "battle_meditation",
                RequiredActions = new Dictionary<string, int>
                {
                    { "meditation", 200 },
                    { "physical_attack", 200 },
                    { "magic_attack", 200 }
                }
            },
            new ComboSkillRequirement
            {
                SkillId = "time_warp",
                RequiredActions = new Dictionary<string, int>
                {
                    { "dodge_success", 300 },
                    { "magic_attack", 250 }
                }
            }
        };

        /// <summary>
        /// Obtiene todos los requisitos de skills combinadas
        /// </summary>
        public static List<ComboSkillRequirement> GetAll()
        {
            return _requirements;
        }

        /// <summary>
        /// Obtiene los requisitos de una skill específica
        /// </summary>
        public static ComboSkillRequirement? GetRequirement(string skillId)
        {
            return _requirements.FirstOrDefault(r => r.SkillId == skillId);
        }

        /// <summary>
        /// Verifica qué skills puede desbloquear el jugador
        /// </summary>
        public static List<string> GetUnlockableSkills(RpgPlayer player)
        {
            var unlockable = new List<string>();
            
            foreach (var requirement in _requirements)
            {
                // Skip si ya tiene la skill
                if (player.UnlockedSkills.Contains(requirement.SkillId))
                    continue;
                
                // Verificar si cumple todos los requisitos
                bool meetsAll = true;
                foreach (var (actionId, requiredCount) in requirement.RequiredActions)
                {
                    int currentCount = player.ActionCounters.TryGetValue(actionId, out var count) ? count : 0;
                    if (currentCount < requiredCount)
                    {
                        meetsAll = false;
                        break;
                    }
                }
                
                if (meetsAll)
                {
                    unlockable.Add(requirement.SkillId);
                }
            }
            
            return unlockable;
        }

        /// <summary>
        /// Obtiene el progreso del jugador hacia una skill específica
        /// </summary>
        public static Dictionary<string, (int current, int required)> GetProgressTowards(RpgPlayer player, string skillId)
        {
            var requirement = GetRequirement(skillId);
            if (requirement == null)
                return new Dictionary<string, (int, int)>();
            
            var progress = new Dictionary<string, (int current, int required)>();
            
            foreach (var (actionId, requiredCount) in requirement.RequiredActions)
            {
                int currentCount = player.ActionCounters.TryGetValue(actionId, out var count) ? count : 0;
                progress[actionId] = (currentCount, requiredCount);
            }
            
            return progress;
        }
    }
}
