using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio que gestiona el tracking de acciones del jugador
    /// y verifica/desbloquea pasivas y clases ocultas
    /// </summary>
    public class ActionTrackerService
    {
        private readonly RpgService _rpgService;
        
        public ActionTrackerService(RpgService rpgService)
        {
            _rpgService = rpgService;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // TRACKING DE ACCIONES
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Registra que el jugador realizó una acción
        /// </summary>
        public void TrackAction(RpgPlayer player, string actionId, int count = 1)
        {
            if (!player.ActionCounters.ContainsKey(actionId))
            {
                player.ActionCounters[actionId] = 0;
            }
            
            player.ActionCounters[actionId] += count;
            
            Console.WriteLine($"[ActionTracker] {player.Name} realizó '{actionId}' (total: {player.ActionCounters[actionId]})");
            
            // Verificar si se desbloqueó algo
            CheckForUnlocks(player);
        }
        
        /// <summary>
        /// Obtiene el contador actual de una acción
        /// </summary>
        public int GetActionCount(RpgPlayer player, string actionId)
        {
            return player.ActionCounters.TryGetValue(actionId, out var count) ? count : 0;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // VERIFICACIÓN DE DESBLOQUEOS
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Verifica si el jugador ha cumplido requisitos para desbloquear algo
        /// </summary>
        private void CheckForUnlocks(RpgPlayer player)
        {
            // Verificar clases ocultas
            CheckHiddenClassUnlocks(player);
            
            // ═══ FASE 4: Verificar clases desbloqueables ═══
            CheckClassUnlocks(player);
            
            // Verificar pasivas individuales (TODO: agregar sistema de requisitos por pasiva)
            CheckPassiveUnlocks(player);
            
            // Verificar skills combinadas
            CheckComboSkillUnlocks(player);
        }
        
        /// <summary>
        /// FASE 4: Verifica y desbloquea clases si se cumplen todos los requisitos
        /// </summary>
        private void CheckClassUnlocks(RpgPlayer player)
        {
            var available = ClassUnlockDatabase.GetAvailableToUnlock(player);
            
            foreach (var def in available)
            {
                if (ClassUnlockDatabase.CanUnlock(player, def))
                {
                    if (!player.UnlockedClasses.Contains(def.ClassId))
                    {
                        player.UnlockedClasses.Add(def.ClassId);
                        Console.WriteLine($"[ClassUnlock] 🎉 {player.Name} desbloqueó clase: {def.Emoji} {def.Name}!");
                        _rpgService.SavePlayer(player);
                    }
                }
            }
        }
        
        /// <summary>
        /// FASE 4: Verifica si el jugador puede cambiar a una clase dada
        /// </summary>
        public bool CanEquipClass(RpgPlayer player, string classId)
        {
            if (classId == "adventurer") return true;
            return player.UnlockedClasses.Contains(classId);
        }
        
        /// <summary>
        /// FASE 4: Cambia la clase activa del jugador y aplica los bonos de stats correspondientes
        /// </summary>
        public bool EquipClass(RpgPlayer player, string classId)
        {
            if (!CanEquipClass(player, classId)) return false;
            
            var def = ClassUnlockDatabase.GetAllClassDefinitions()
                .FirstOrDefault(d => d.ClassId == classId);

            // Aplicar bonos de stats (quita anteriores, aplica nuevos)
            ClassBonusService.ApplyClass(player, classId);

            player.ActiveClassId = classId;
            
            if (def != null)
                player.Class = def.TargetClass;
            else if (classId == "adventurer")
                player.Class = CharacterClass.Adventurer;
            
            _rpgService.SavePlayer(player);
            return true;
        }
        
        /// <summary>
        /// Verifica y desbloquea clases ocultas si se cumplen requisitos
        /// </summary>
        private void CheckHiddenClassUnlocks(RpgPlayer player)
        {
            var availableClasses = HiddenClassDatabase.GetAvailableForPlayer(player);
            
            foreach (var hiddenClass in availableClasses)
            {
                if (MeetsRequirements(player, hiddenClass.RequiredActions))
                {
                    UnlockHiddenClass(player, hiddenClass);
                }
            }
        }
        
        /// <summary>
        /// Verifica si el jugador cumple todos los requisitos
        /// </summary>
        public bool MeetsRequirements(RpgPlayer player, Dictionary<string, int> requirements)
        {
            foreach (var (actionId, requiredCount) in requirements)
            {
                var currentCount = GetActionCount(player, actionId);
                if (currentCount < requiredCount)
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Desbloquea una clase oculta para el jugador
        /// </summary>
        private void UnlockHiddenClass(RpgPlayer player, HiddenClass hiddenClass)
        {
            // Agregar clase a lista de desbloqueadas
            player.UnlockedHiddenClasses.Add(hiddenClass.Id);
            
            // Otorgar todas las pasivas de la clase
            foreach (var passiveId in hiddenClass.GrantedPassives)
            {
                if (!player.UnlockedPassives.Contains(passiveId))
                {
                    player.UnlockedPassives.Add(passiveId);
                    Console.WriteLine($"[ActionTracker] 🎉 {player.Name} desbloqueó pasiva: {passiveId}");
                }
            }
            
            // Otorgar todas las habilidades de la clase
            foreach (var skillId in hiddenClass.UnlockedSkills)
            {
                if (!player.UnlockedSkills.Contains(skillId))
                {
                    player.UnlockedSkills.Add(skillId);
                    Console.WriteLine($"[ActionTracker] 🎉 {player.Name} desbloqueó skill: {skillId}");
                }
            }
            
            Console.WriteLine($"[ActionTracker] 🌟 {player.Name} desbloqueó clase oculta: {hiddenClass.Name}!");
            
            _rpgService.SavePlayer(player);
        }
        
        /// <summary>
        /// Verifica y desbloquea pasivas individuales basadas en acciones
        /// </summary>
        private void CheckPassiveUnlocks(RpgPlayer player)
        {
            // Pasivas básicas desbloqueadas por acciones simples
            // Usando List para permitir múltiples pasivas por acción
            var passiveUnlocks = new List<(string actionId, string passiveId, int requiredCount)>
            {
                // Beast Whisperer - Desbloquea después de acariciar 50 bestias
                ("pet_beast", "beast_whisperer", 50),
                
                // Critical Mastery - Desbloquea después de 100 críticos
                ("critical_hit", "critical_mastery", 100),
                
                // Lethal Strikes - Desbloquea después de 500 críticos
                ("critical_hit", "lethal_strikes", 500),
                
                // Iron Skin - Desbloquea después de recibir 1000 de daño
                ("damage_taken", "iron_skin", 1000),
                
                // Regeneration - Desbloquea después de meditar 50 veces
                ("meditation", "regeneration", 50),
                
                // Meditation Master - Desbloquea después de meditar 200 veces
                ("meditation", "meditation_master", 200),
                
                // Counter Master - Desbloquea después de contraatacar 100 veces
                ("counter_attack", "counter_master", 100),
                
                // Life Steal - Desbloquea después de matar 200 enemigos
                ("enemy_kill", "life_steal", 200),
                
                // Treasure Hunter - Desbloquea después de encontrar 50 loots
                ("loot_found", "treasure_hunter", 50),
                
                // Gold Magnate - Desbloquea después de acumular 10000 oro
                ("gold_earned", "gold_magnate", 10000),
                
                // Fast Learner - Desbloquea después de subir 10 niveles
                ("level_up", "fast_learner", 10),
                
                // Bloodlust - Desbloquea después de ganar 20 combates con <30% HP
                ("low_hp_victory", "bloodlust", 20),

                // Shadow Step - progreso por aventuras furtivas (nuevo sistema)
                ("adventure_stealth", "shadow_step", 120),

                // Mana Font - progreso por meditación profunda
                ("deep_meditation", "mana_font", 120),

                // Arcane Power - progreso por entrenamiento mental
                ("train_mind", "arcane_power", 150),

                // Berserker Rage - progreso por entrenamiento físico
                ("train_body", "berserker_rage", 150),

                // Fast Learner - progreso por estudio/investigación
                ("study", "fast_learner", 150),
                ("investigate", "fast_learner", 120)
            };
            
            foreach (var (actionId, passiveId, requiredCount) in passiveUnlocks)
            {
                // Verificar si la pasiva ya está desbloqueada
                if (!player.UnlockedPassives.Contains(passiveId))
                {
                    var currentCount = GetActionCount(player, actionId);
                    if (currentCount >= requiredCount)
                    {
                        player.UnlockedPassives.Add(passiveId);
                        var passive = PassiveDatabase.GetById(passiveId);
                        Console.WriteLine($"[ActionTracker] ✨ {player.Name} desbloqueó pasiva: {passive?.Name ?? passiveId}");
                        _rpgService.SavePlayer(player);
                    }
                }
            }
        }
        
        // ═══════════════════════════════════════════════════════════════
        // UTILIDADES DE PROGRESO
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Obtiene el progreso del jugador hacia una clase oculta
        /// </summary>
        public ClassUnlockProgress GetClassProgress(RpgPlayer player, string classId)
        {
            var hiddenClass = HiddenClassDatabase.GetById(classId);
            if (hiddenClass == null)
            {
                return new ClassUnlockProgress { ClassId = classId };
            }
            
            var progress = new ClassUnlockProgress
            {
                ClassId = classId,
                IsUnlocked = player.UnlockedHiddenClasses.Contains(classId),
                CurrentProgress = new Dictionary<string, int>(),
                RequirementsMet = new Dictionary<string, bool>()
            };
            
            foreach (var (actionId, requiredCount) in hiddenClass.RequiredActions)
            {
                var currentCount = GetActionCount(player, actionId);
                progress.CurrentProgress[actionId] = currentCount;
                progress.RequirementsMet[actionId] = currentCount >= requiredCount;
            }
            
            return progress;
        }
        
        /// <summary>
        /// Genera un reporte de progreso para todas las clases ocultas
        /// </summary>
        public Dictionary<string, ClassUnlockProgress> GetAllClassProgress(RpgPlayer player)
        {
            var report = new Dictionary<string, ClassUnlockProgress>();
            var allClasses = HiddenClassDatabase.GetAll();
            
            foreach (var hiddenClass in allClasses)
            {
                report[hiddenClass.Id] = GetClassProgress(player, hiddenClass.Id);
            }
            
            return report;
        }
        
        /// <summary>
        /// Obtiene el porcentaje de completitud hacia una clase oculta (0-100)
        /// </summary>
        public double GetClassProgressPercentage(RpgPlayer player, string classId)
        {
            var progress = GetClassProgress(player, classId);
            if (progress.IsUnlocked) return 100.0;
            
            if (progress.RequirementsMet.Count == 0) return 0.0;
            
            var metCount = progress.RequirementsMet.Count(kvp => kvp.Value);
            return (metCount / (double)progress.RequirementsMet.Count) * 100.0;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // ACTIVACIÓN DE CLASE OCULTA
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Activa una clase oculta (aplica bonuses de stats)
        /// </summary>
        public bool ActivateHiddenClass(RpgPlayer player, string classId)
        {
            // Verificar que el jugador tiene la clase desbloqueada
            if (!player.UnlockedHiddenClasses.Contains(classId))
            {
                return false;
            }
            
            var hiddenClass = HiddenClassDatabase.GetById(classId);
            if (hiddenClass == null)
            {
                return false;
            }
            
            // Remover bonuses de clase anterior si existía
            if (player.ActiveHiddenClass != null)
            {
                RemoveHiddenClassBonuses(player, player.ActiveHiddenClass);
            }
            
            // Aplicar bonuses de nueva clase
            player.Strength += hiddenClass.StrengthBonus;
            player.Intelligence += hiddenClass.IntelligenceBonus;
            player.Dexterity += hiddenClass.DexterityBonus;
            player.Constitution += hiddenClass.ConstitutionBonus;
            player.Wisdom += hiddenClass.WisdomBonus;
            player.Charisma += hiddenClass.CharismaBonus;
            
            // Ajustar MaxHP/Mana/Stamina basado en nuevos stats
            RecalculateMaxResources(player);
            
            player.ActiveHiddenClass = classId;
            _rpgService.SavePlayer(player);
            
            Console.WriteLine($"[ActionTracker] 🌟 {player.Name} activó clase oculta: {hiddenClass.Name}");
            return true;
        }
        
        /// <summary>
        /// Desactiva la clase oculta actual
        /// </summary>
        public void DeactivateHiddenClass(RpgPlayer player)
        {
            if (player.ActiveHiddenClass != null)
            {
                RemoveHiddenClassBonuses(player, player.ActiveHiddenClass);
                player.ActiveHiddenClass = null;
                _rpgService.SavePlayer(player);
            }
        }
        
        private void RemoveHiddenClassBonuses(RpgPlayer player, string classId)
        {
            var hiddenClass = HiddenClassDatabase.GetById(classId);
            if (hiddenClass == null) return;
            
            player.Strength -= hiddenClass.StrengthBonus;
            player.Intelligence -= hiddenClass.IntelligenceBonus;
            player.Dexterity -= hiddenClass.DexterityBonus;
            player.Constitution -= hiddenClass.ConstitutionBonus;
            player.Wisdom -= hiddenClass.WisdomBonus;
            player.Charisma -= hiddenClass.CharismaBonus;
            
            RecalculateMaxResources(player);
        }
        
        private void RecalculateMaxResources(RpgPlayer player)
        {
            // Recalcular basado en stats (simplificado)
            var oldMaxHP = player.MaxHP;
            var oldMaxMana = player.MaxMana;
            var oldMaxStamina = player.MaxStamina;
            
            player.MaxHP = 100 + (player.Constitution * 5) + (player.Level * 10);
            player.MaxMana = player.Intelligence * 3 + (player.Level * 5);
            player.MaxStamina = 50 + (player.Strength * 2) + (player.Level * 3);
            
            // Mantener proporciones de recursos actuales
            if (oldMaxHP > 0)
            {
                var hpRatio = player.HP / (double)oldMaxHP;
                player.HP = (int)(player.MaxHP * hpRatio);
            }
            if (oldMaxMana > 0)
            {
                var manaRatio = player.Mana / (double)oldMaxMana;
                player.Mana = (int)(player.MaxMana * manaRatio);
            }
            if (oldMaxStamina > 0)
            {
                var staminaRatio = player.Stamina / (double)oldMaxStamina;
                player.Stamina = (int)(player.MaxStamina * staminaRatio);
            }
        }
        
        // ═══════════════════════════════════════════════════════════════
        // DESBLOQUEO DE SKILLS COMBINADAS
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Verifica y desbloquea skills combinadas si se cumplen requisitos
        /// </summary>
        private void CheckComboSkillUnlocks(RpgPlayer player)
        {
            var unlockableSkills = SkillUnlockDatabase.GetUnlockableSkills(player);
            
            foreach (var skillId in unlockableSkills)
            {
                // Agregar a skills desbloqueadas
                player.UnlockedSkills.Add(skillId);
                
                Console.WriteLine($"[ActionTracker] 🌟 {player.Name} desbloqueó skill combinada: {skillId}!");
                
                // Guardar progreso
                _rpgService.SavePlayer(player);
            }
        }
    }
}
