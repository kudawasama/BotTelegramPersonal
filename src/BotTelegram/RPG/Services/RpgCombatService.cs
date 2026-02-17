using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public partial class RpgCombatService
    {
        private readonly RpgService _rpgService;
        private static readonly Random _random = new();
        
        public RpgCombatService()
        {
            _rpgService = new RpgService();
        }
        
        public CombatResult PlayerAttack(RpgPlayer player, RpgEnemy enemy, bool useMagic = false)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            // Verificar si está aturdido
            if (player.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                result.PlayerStunned = true;
                AddCombatLog(player, $"Atacar", "⚠️ ATURDIDO - No puedes actuar");
                ProcessStatusEffects(player, enemy, result);
                return result;
            }
            
            // ═══════════════════════════════════════
            // SISTEMA DE PROBABILIDADES FIJAS (REBALANCEADO)
            // ═══════════════════════════════════════
            
            // 1. CALCULAR HIT CHANCE (probabilidad de golpear)
            double baseHitChance = 65.0; // Base 65% (reducido de 85% para mayor dificultad)
            double accuracyBonus = (player.Accuracy - enemy.Evasion) * 0.5; // Cada punto de diferencia = 0.5%
            double hitChance = Math.Clamp(baseHitChance + accuracyBonus, 10.0, 95.0); // Min 10%, Max 95%
            
            // Roll de probabilidad (0-100)
            double hitRoll = _random.Next(0, 10000) / 100.0;
            result.HitChancePercent = hitChance;
            result.HitRoll = hitRoll;
            result.PlayerHit = hitRoll <= hitChance;
            
            if (result.PlayerHit)
            {
                // Incrementar combo
                player.ComboCount++;
                
                // Track tipo de ataque para skill unlocks
                TrackAction(player, useMagic ? "magic_attack" : "physical_attack");
                
                // 2. CALCULAR DAÑO BASE
                int baseDamage;
                int defenseValue;
                
                if (useMagic)
                {
                    baseDamage = player.MagicalAttack;
                    defenseValue = enemy.MagicResistance;
                    result.AttackType = AttackType.Magical;
                }
                else
                {
                    baseDamage = player.PhysicalAttack;
                    defenseValue = enemy.PhysicalDefense;
                    result.AttackType = AttackType.Physical;
                }
                
                // 3. APLICAR VARIACIÓN DE DAÑO (90-110% del base)
                var damageVariation = _random.Next(90, 111) / 100.0;
                baseDamage = (int)(baseDamage * damageVariation);
                
                // 4. BONUS DE COMBO (5% por ataque, máx 25%)
                if (player.ComboCount > 1)
                {
                    var comboBonus = Math.Min(player.ComboCount - 1, 5) * 0.05;
                    var comboDamage = (int)(baseDamage * comboBonus);
                    baseDamage += comboDamage;
                    result.ComboBonus = comboDamage;
                }
                
                // 5. BONUS DE EMPODERAMIENTO
                var empowerEffect = player.StatusEffects.FirstOrDefault(e => e.Type == StatusEffectType.Empowered);
                if (empowerEffect != null)
                {
                    baseDamage += empowerEffect.Intensity;
                }
                
                // 6. CALCULAR CRITICAL HIT
                double critRoll = _random.Next(0, 10000) / 100.0;
                result.CriticalChancePercent = player.CriticalChance;
                result.CriticalRoll = critRoll;
                result.PlayerCritical = critRoll <= player.CriticalChance;
                
                if (result.PlayerCritical)
                {
                    baseDamage = (int)(baseDamage * 1.75); // Crítico = 175% daño
                    TrackCriticalHit(player); // Track críticos para skill unlocks
                    
                    // Combo x3+ con crítico = Sangrado
                    if (player.ComboCount >= 3)
                    {
                        enemy.StatusEffects.Add(new StatusEffect(StatusEffectType.Bleeding, 3, 5));
                        result.InflictedEffect = StatusEffectType.Bleeding;
                    }
                }
                
                // 7. REDUCCIÓN POR DEFENSA (Defensa reduce % del daño)
                // Fórmula: DamageReduction = Defense / (Defense + 100)
                // Ejemplo: 50 def = 33% reducción, 100 def = 50% reducción
                double damageReduction = defenseValue / (defenseValue + 100.0);
                int finalDamage = (int)(baseDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage); // Mínimo 1 daño
                result.DamageReduction = (int)(baseDamage * damageReduction);
                enemy.HP -= result.PlayerDamage;
                TrackDamageDealt(player, result.PlayerDamage); // Track daño total para skills
                
                var attackTypeEmoji = useMagic ? "🔮" : "⚔️";
                var criticalText = result.PlayerCritical ? " [CRÍTICO!]" : "";
                AddCombatLog(player, $"Atacar (Combo x{player.ComboCount})", 
                    $"{attackTypeEmoji} {result.PlayerDamage} daño{criticalText}");
                
                Console.WriteLine($"[Combat] {attackTypeEmoji} {player.Name} ataca: chance={hitChance:F1}%, roll={hitRoll:F1}%, daño={result.PlayerDamage}, combo={player.ComboCount}");
            }
            else
            {
                // Romper combo al fallar
                if (player.ComboCount > 0)
                {
                    result.ComboBroken = true;
                    AddCombatLog(player, $"Atacar", "❌ FALLO - Combo roto");
                }
                player.ComboCount = 0;
                
                Console.WriteLine($"[Combat] ❌ {player.Name} falla: chance={hitChance:F1}%, roll={hitRoll:F1}%");
            }
            
            // Check if enemy is dead
            if (enemy.HP <= 0)
            {
                ApplyVictoryRewards(player, enemy, result);
                return result;
            }
            
            // ═══ FASE 2: TURNOS DE MASCOTAS ═══
            // Las mascotas atacan después del jugador pero antes del contraataque enemigo
            if (player.ActivePets?.Any(p => p.HP > 0) == true)
            {
                ProcessPetTurns(player, enemy, result);
                
                // Verificar si el enemigo fue derrotado por las mascotas
                if (enemy.HP <= 0)
                {
                    ApplyVictoryRewards(player, enemy, result);
                    return result;
                }
            }
            
            // Procesar efectos de estado antes del contraataque
            ProcessStatusEffects(player, enemy, result);
            
            // FASE 5A: Turno de minions (después del jugador y mascotas, antes del enemigo)
            ExecuteMinionsTurn(player, enemy, result);
            
            // Si el enemigo fue derrotado por los minions
            if (enemy.HP <= 0)
            {
                ApplyVictoryRewards(player, enemy, result);
                return result;
            }
            
            // Si el jugador murió por efectos o minions traicioneros
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                return result;
            }
            
            // Enemy counterattack (si no está aturdido)
            if (!enemy.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                PerformEnemyAttack(player, enemy, result);
            }
            else
            {
                AddCombatLog(player, $"{enemy.Name}", "💫 Aturdido - No ataca");
            }
            
            // Check if player died
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                player.HP = 0;
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.TotalDeaths++;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Derrota", "💀 Has sido derrotado");
                
                Console.WriteLine($"[Combat] 💀 {player.Name} fue derrotado...");
            }
            
            return result;
        }
        
        // ═══════════════════════════════════════
        // FASE 2: SISTEMA DE TURNOS DE MASCOTAS
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Procesa los turnos de todas las mascotas activas del jugador
        /// </summary>
        private void ProcessPetTurns(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            if (player.ActivePets == null || player.ActivePets.Count == 0 || enemy.HP <= 0)
            {
                return; // No hay mascotas o enemigo ya derrotado
            }
            
            foreach (var pet in player.ActivePets.Where(p => p.HP > 0).ToList())
            {
                // Verificar si la mascota está aturdida
                if (pet.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
                {
                    AddCombatLog(player, $"🐾 {pet.Name}", "💫 Aturdido - No ataca");
                    continue;
                }
                
                var petResult = PetAttack(player, pet, enemy);
                result.PetTurns.Add(petResult);
                result.TotalPetDamage += petResult.Damage;
                
                // Si el enemigo fue derrotado por la mascota
                if (enemy.HP <= 0)
                {
                    pet.TotalKills++;
                    if (enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss)
                    {
                        pet.BossKills++;
                    }
                    break;
                }
            }
        }
        
        /// <summary>
        /// Ejecuta el ataque de una mascota individual
        /// </summary>
        private PetTurnResult PetAttack(RpgPlayer player, RpgPet pet, RpgEnemy enemy)
        {
            var petResult = new PetTurnResult
            {
                PetName = pet.Name,
                Emoji = GetPetEmoji(pet.Species)
            };
            
            // ═══ CALCULAR HIT CHANCE ═══
            double baseHitChance = 70.0; // Base 70% para mascotas
            
            // Bonus de loyalty (mascotas leales son más precisas)
            double loyaltyBonus = pet.Loyalty switch
            {
                PetLoyalty.Hostile => -20.0,
                PetLoyalty.Neutral => 0.0,
                PetLoyalty.Friendly => 5.0,
                PetLoyalty.Loyal => 10.0,
                PetLoyalty.Devoted => 15.0,
                _ => 0.0
            };
            
            // Bonus por nivel y agilidad
            double levelBonus = (pet.Level - enemy.Level) * 2.0;
            double speedBonus = (pet.Speed - enemy.Evasion) * 0.5;
            
            double hitChance = Math.Clamp(baseHitChance + loyaltyBonus + levelBonus + speedBonus, 20.0, 95.0);
            
            double hitRoll = _random.Next(0, 10000) / 100.0;
            petResult.HitChance = hitChance;
            petResult.HitRoll = hitRoll;
            petResult.Hit = hitRoll <= hitChance;
            
            if (!petResult.Hit)
            {
                AddCombatLog(player, $"{petResult.Emoji} {pet.Name}", "💨 ¡Falla el ataque!");
                return petResult;
            }
            
            // ═══ CALCULAR DAÑO ═══
            int baseDamage = pet.EffectiveAttack; // Ya incluye loyalty bonus
            
            // Variación de daño (85-115% para mascotas)
            baseDamage = (int)(baseDamage * (_random.Next(85, 116) / 100.0));
            
            // Aplicar comportamiento de la mascota
            baseDamage = ApplyPetBehavior(pet, enemy, baseDamage, petResult);
            
            // Reducción por defensa enemiga
            int defense = pet.AttackType == AttackType.Magical ? enemy.MagicResistance : enemy.PhysicalDefense;
            int damageReduction = defense / 2;
            int finalDamage = Math.Max(1, baseDamage - damageReduction);
            
            // ═══ CRÍTICO ═══
            double critChance = 5.0 + (pet.Level * 0.5); // Base 5% + nivel
            if (pet.Loyalty == PetLoyalty.Devoted)
            {
                critChance += 10.0; // +10% crit si está devoto
            }
            
            double critRoll = _random.Next(0, 10000) / 100.0;
            petResult.Critical = critRoll <= critChance;
            
            if (petResult.Critical)
            {
                finalDamage = (int)(finalDamage * 1.75);
            }
            
            // Aplicar daño
            enemy.HP -= finalDamage;
            pet.TotalDamageDealt += finalDamage;
            petResult.Damage = finalDamage;
            petResult.AttackType = pet.AttackType;
            
            // ═══ LOG ═══
            string critText = petResult.Critical ? " ⚡ CRÍTICO" : "";
            string behaviorText = GetBehaviorText(pet.Behavior);
            AddCombatLog(player, $"{petResult.Emoji} {pet.Name}", 
                $"{behaviorText} {finalDamage} daño{critText}");
            
            // ═══ EFECTOS ESPECIALES ═══
            CheckPetSpecialEffects(pet, enemy, petResult);
            
            // XP para la mascota
            if (enemy.HP <= 0)
            {
                bool isBoss = enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss;
                pet.EvolutionXP += (int)(enemy.Level * 50 * (isBoss ? 3 : 1));
            }
            
            return petResult;
        }
        
        /// <summary>
        /// Aplica el comportamiento de la mascota al daño
        /// </summary>
        private int ApplyPetBehavior(RpgPet pet, RpgEnemy enemy, int baseDamage, PetTurnResult result)
        {
            double hpPercent = (double)enemy.HP / enemy.MaxHP;
            
            return pet.Behavior switch
            {
                PetBehavior.Aggressive => (int)(baseDamage * 1.2), // +20% daño siempre
                PetBehavior.Defensive => hpPercent < 0.3 ? (int)(baseDamage * 1.4) : (int)(baseDamage * 0.8), // +40% si enemigo bajo HP
                PetBehavior.Balanced => baseDamage, // Sin modificación
                PetBehavior.Supportive => (int)(baseDamage * 0.7), // -30% daño (se enfoca en buffs)
                PetBehavior.Smart => hpPercent > 0.7 ? (int)(baseDamage * 1.3) : baseDamage, // +30% si enemigo con mucho HP
                _ => baseDamage
            };
        }
        
        /// <summary>
        /// Verifica efectos especiales de mascotas (veneno, burn, etc.)
        /// </summary>
        private void CheckPetSpecialEffects(RpgPet pet, RpgEnemy enemy, PetTurnResult result)
        {
            // Dragones tienen chance de quemar
            if (pet.Species.StartsWith("dragon_") && _random.Next(100) < 20)
            {
                var burnEffect = new StatusEffect(StatusEffectType.Burning, 3, pet.MagicPower / 2);
                enemy.StatusEffects.Add(burnEffect);
                result.InflictedEffect = StatusEffectType.Burning;
            }
            
            // Serpientes tienen chance de envenenar
            if (pet.Species.StartsWith("snake_") && _random.Next(100) < 25)
            {
                var poisonEffect = new StatusEffect(StatusEffectType.Poisoned, 4, pet.Attack / 3);
                enemy.StatusEffects.Add(poisonEffect);
                result.InflictedEffect = StatusEffectType.Poisoned;
            }
            
            // Osos tienen chance de aturdir
            if (pet.Species.StartsWith("bear_") && _random.Next(100) < 15)
            {
                var stunEffect = new StatusEffect(StatusEffectType.Stunned, 1, 0);
                enemy.StatusEffects.Add(stunEffect);
                result.InflictedEffect = StatusEffectType.Stunned;
            }
        }
        
        /// <summary>
        /// Obtiene el emoji según la especie de la mascota
        /// </summary>
        private string GetPetEmoji(string species)
        {
            if (species.StartsWith("wolf_")) return "🐺";
            if (species.StartsWith("bear_")) return "🐻";
            if (species.StartsWith("dragon_")) return "🐉";
            if (species.StartsWith("wildcat_")) return "🐱";
            if (species.StartsWith("eagle_")) return "🦅";
            if (species.StartsWith("snake_")) return "🐍";
            return "🐾";
        }
        
        /// <summary>
        /// Obtiene el texto descriptivo del comportamiento
        /// </summary>
        private string GetBehaviorText(PetBehavior behavior)
        {
            return behavior switch
            {
                PetBehavior.Aggressive => "🔥 Embiste:",
                PetBehavior.Defensive => "🛡️ Protege:",
                PetBehavior.Balanced => "⚔️ Ataca:",
                PetBehavior.Supportive => "💚 Ayuda:",
                PetBehavior.Smart => "🧠 Calcula:",
                _ => "⚔️ Ataca:"
            };
        }
        
        public CombatResult PlayerDefend(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            result.PlayerDefended = true;
            StartPlayerTurn(player);
            
            // Romper combo al defender
            if (player.ComboCount > 0)
            {
                player.ComboCount = 0;
                result.ComboBroken = true;
            }
            
            // Verificar si está aturdido
            if (player.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                result.PlayerStunned = true;
                AddCombatLog(player, "Estado", "⚠️ ATURDIDO - No puedes defenderte bien");
                AddCombatLog(player, "Defender", "🛡️ Postura defensiva (DEBILITADA)");
                ProcessStatusEffects(player, enemy, result);
                
                // Ataque enemigo con penalización menor
                if (!enemy.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
                {
                    PerformEnemyAttack(player, enemy, result, defenseBonus: player.Dexterity / 4);
                }
                return result;
            }
            
            // Boost defense temporarily
            var defenseBoost = player.Dexterity / 2;
            
            // Bonus de escudo
            var shieldEffect = player.StatusEffects.FirstOrDefault(e => e.Type == StatusEffectType.Shielded);
            var shieldBonus = 0;
            if (shieldEffect != null)
            {
                shieldBonus = shieldEffect.Intensity;
                defenseBoost += shieldBonus;
            }
            
            // Mensaje mejorado de defensa
            var defenseMessage = $"🛡️ Postura defensiva\n" +
                                $"📊 Defensa base: {player.PhysicalDefense}\n" +
                                $"   + Bonus DEX: {player.Dexterity / 2}\n";
            
            if (shieldBonus > 0)
            {
                defenseMessage += $"   + Bonus escudo: {shieldBonus}\n";
            }
            
            defenseMessage += $"   = Defensa total: {player.PhysicalDefense + defenseBoost}\n";
            defenseMessage += $"⚡ Costo: 0 Stamina\n";
            defenseMessage += $"🔄 Reducción: ~{(defenseBoost * 0.5):F1} daño menos";
            
            AddCombatLog(player, "Defender", defenseMessage);
            
            // Procesar efectos de estado
            ProcessStatusEffects(player, enemy, result);
            
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                return result;
            }
            
            // Enemy attack with boosted defense
            if (!enemy.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                PerformEnemyAttack(player, enemy, result, defenseBoost);
            }
            else
            {
                AddCombatLog(player, $"{enemy.Name}", "💫 Aturdido - No ataca");
            }
            
            // Check if player died
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                player.HP = 0;
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.TotalDeaths++;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Derrota", "💀 Has sido derrotado");
            }
            
            return result;
        }
        
        public bool TryToFlee(RpgPlayer player, RpgEnemy enemy)
        {
            StartPlayerTurn(player);
            
            // Romper combo
            player.ComboCount = 0;
            
            // ═══ SISTEMA DE PROBABILIDADES PARA HUIR ═══
            
            // 1. CALCULAR PROBABILIDAD DE HUIDA
            double baseFleeChance = 60.0; // Chance base
            
            // Bonus por DEX y Evasión (la agilidad ayuda a huir)
            double agilityBonus = (player.Dexterity - 10) * 1.0; // +1% por cada punto de DEX sobre 10
            double evasionBonus = player.Evasion * 0.5; // +0.5% por cada punto de Evasión
            
            // Penalización por diferencia de nivel
            double levelPenalty = (enemy.Level - player.Level) * 5.0; // -5% por cada nivel del enemigo sobre el jugador
            
            double fleeChance = baseFleeChance + agilityBonus + evasionBonus - levelPenalty;
            fleeChance = Math.Clamp(fleeChance, 10.0, 95.0); // Min 10%, Max 95%
            
            // 2. ROLL DE PROBABILIDAD (0-100)
            double fleeRoll = _random.Next(0, 10000) / 100.0;
            bool success = fleeRoll <= fleeChance;
            
            if (success)
            {
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Huir", $"✅ Escapaste (chance: {fleeChance:F1}%, roll: {fleeRoll:F1}%)");
                Console.WriteLine($"[Combat] 🏃 {player.Name} huyó exitosamente (chance={fleeChance:F1}%, roll={fleeRoll:F1}%)");
            }
            else
            {
                // Enemy gets a free attack (usa su ataque físico)
                double damageReduction = player.PhysicalDefense / (player.PhysicalDefense + 100.0);
                int damage = (int)(enemy.Attack * (1.0 - damageReduction));
                damage = Math.Max(1, damage);
                
                player.HP -= damage;
                
                AddCombatLog(player, "Huir", $"❌ Fallo (chance: {fleeChance:F1}%, roll: {fleeRoll:F1}%) - Recibiste {damage} daño");
                Console.WriteLine($"[Combat] ❌ Fallo al huir (chance={fleeChance:F1}%, roll={fleeRoll:F1}%). Recibe {damage} daño");
                
                if (player.HP <= 0)
                {
                    player.HP = 0;
                    player.IsInCombat = false;
                    player.CurrentEnemy = null;
                    player.ComboCount = 0;
                    player.CombatTurnCount = 0;
                    player.TotalDeaths++;
                    player.StatusEffects.Clear();
                }
            }
            
            return success;
        }
        
        // Método auxiliar: procesar efectos de estado
        private void ProcessStatusEffects(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            // Efectos del jugador
            var toRemove = new List<StatusEffect>();
            foreach (var effect in player.StatusEffects.ToList())
            {
                switch (effect.Type)
                {
                    case StatusEffectType.Bleeding:
                        player.HP -= effect.Intensity;
                        AddCombatLog(player, "Sangrado", $"🩸 -{effect.Intensity} HP");
                        result.StatusDamage += effect.Intensity;
                        break;
                        
                    case StatusEffectType.Poisoned:
                        var poisonDmg = effect.Intensity * (4 - effect.Duration); // Crece con el tiempo
                        player.HP -= poisonDmg;
                        AddCombatLog(player, "Veneno", $"🧪 -{poisonDmg} HP");
                        result.StatusDamage += poisonDmg;
                        break;
                        
                    case StatusEffectType.Burning:
                        player.HP -= effect.Intensity;
                        AddCombatLog(player, "Quemadura", $"🔥 -{effect.Intensity} HP");
                        result.StatusDamage += effect.Intensity;
                        break;
                        
                    case StatusEffectType.Regenerating:
                        var heal = effect.Intensity;
                        player.HP = Math.Min(player.HP + heal, player.MaxHP);
                        AddCombatLog(player, "Regeneración", $"💚 +{heal} HP");
                        break;
                }
                
                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    toRemove.Add(effect);
                }
            }
            
            foreach (var effect in toRemove)
            {
                player.StatusEffects.Remove(effect);
                AddCombatLog(player, "Efecto", $"✨ {GetEffectName(effect.Type)} terminó");
            }
            
            // Efectos del enemigo
            toRemove.Clear();
            foreach (var effect in enemy.StatusEffects.ToList())
            {
                switch (effect.Type)
                {
                    case StatusEffectType.Bleeding:
                        enemy.HP -= effect.Intensity;
                        AddCombatLog(player, $"{enemy.Name}", $"🩸 Sangrado -{effect.Intensity} HP");
                        break;
                    
                    case StatusEffectType.Poisoned:
                        var poisonDmg = effect.Intensity * (4 - effect.Duration);
                        enemy.HP -= poisonDmg;
                        AddCombatLog(player, $"{enemy.Name}", $"🧪 Veneno -{poisonDmg} HP");
                        break;
                }
                
                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    toRemove.Add(effect);
                }
            }
            
            foreach (var effect in toRemove)
            {
                enemy.StatusEffects.Remove(effect);
            }
        }
        
        // Método auxiliar: ataque enemigo
        private void PerformEnemyAttack(RpgPlayer player, RpgEnemy enemy, CombatResult result, int defenseBonus = 0)
        {
            // SISTEMA DE PROBABILIDADES FIJAS para enemigo
            double baseHitChance = 80.0; // Base 80% (enemigos un poco menos precisos)
            double accuracyBonus = (enemy.Accuracy - player.Evasion) * 0.5;
            double hitChance = Math.Clamp(baseHitChance + accuracyBonus, 10.0, 90.0);
            
            double hitRoll = _random.Next(0, 10000) / 100.0;
            result.EnemyHitChancePercent = hitChance;
            result.EnemyHitRoll = hitRoll;
            result.EnemyHit = hitRoll <= hitChance;
            
            if (result.EnemyHit)
            {
                // Daño base del enemigo
                int baseDamage = enemy.Attack;
                
                // Variación (90-110%)
                var damageVariation = _random.Next(90, 111) / 100.0;
                baseDamage = (int)(baseDamage * damageVariation);
                
                // Crítico (probabilidad fija 10% para enemigos)
                double critRoll = _random.Next(0, 10000) / 100.0;
                result.EnemyCritical = critRoll <= 10.0;
                
                if (result.EnemyCritical)
                {
                    baseDamage = (int)(baseDamage * 1.5);
                }
                
                // Defensa física del jugador
                double damageReduction = (player.PhysicalDefense + defenseBonus) / (player.PhysicalDefense + defenseBonus + 100.0);
                int finalDamage = (int)(baseDamage * (1.0 - damageReduction));
                
                // Reducir daño adicional si está defendiendo
                if (defenseBonus > 0)
                {
                    finalDamage = finalDamage / 2;
                }
                
                result.EnemyDamage = Math.Max(1, finalDamage);
                player.HP -= result.EnemyDamage;
                TrackDamageTaken(player, result.EnemyDamage); // Track daño recibido para skills
                
                // Romper combo si recibe daño y no está defendiendo
                if (player.ComboCount > 0 && defenseBonus == 0)
                {
                    player.ComboCount = 0;
                    result.ComboBroken = true;
                }
                
                var critText = result.EnemyCritical ? " [CRÍTICO!]" : "";
                AddCombatLog(player, $"{enemy.Name} ataca", 
                    $"⚔️ {result.EnemyDamage} daño{critText}");
                
                Console.WriteLine($"[Combat] ⚔️ {enemy.Name} contraataca: chance={hitChance:F1}%, roll={hitRoll:F1}%, daño={result.EnemyDamage}");
            }
            else
            {
                AddCombatLog(player, $"{enemy.Name} ataca", "🛡️ Esquivado");
                Console.WriteLine($"[Combat] 🛡️ {player.Name} esquiva: chance={hitChance:F1}%, roll={hitRoll:F1}%");
            }
        }
        
        // Método auxiliar: agregar al log de combate
        private void AddCombatLog(RpgPlayer player, string action, string result)
        {
            player.CombatLog.Add(new CombatLogEntry
            {
                Turn = player.CombatTurnCount,
                Action = action,
                Result = result
            });
        }
        
        // Método auxiliar: nombre de efecto
        private string GetEffectName(StatusEffectType type)
        {
            return type switch
            {
                StatusEffectType.Bleeding => "Sangrado",
                StatusEffectType.Poisoned => "Veneno",
                StatusEffectType.Stunned => "Aturdimiento",
                StatusEffectType.Burning => "Quemadura",
                StatusEffectType.Frozen => "Congelamiento",
                StatusEffectType.Regenerating => "Regeneración",
                StatusEffectType.Shielded => "Escudo",
                StatusEffectType.Empowered => "Potenciamiento",
                _ => "Desconocido"
            };
        }
        
        public string GetCombatNarrative(CombatResult result, RpgPlayer player, RpgEnemy enemy)
        {
            var narrative = $"⚔️ **COMBATE - Turno {player.CombatTurnCount}**\n\n";
            
            // Player action
            if (result.PlayerStunned)
            {
                narrative += "💫 *Estás ATURDIDO y no puedes actuar*\n\n";
            }
            else if (!string.IsNullOrEmpty(result.SkillFailureReason))
            {
                narrative += $"❌ *{result.SkillFailureReason}*\n\n";
            }
            else if (result.SkillUsed)
            {
                narrative += $"✨ **{result.SkillName}**\n";
                
                // Mostrar detalles adicionales de la skill (ej: invocaciones)
                if (!string.IsNullOrEmpty(result.SkillDetails))
                {
                    narrative += $"{result.SkillDetails}\n";
                }
                
                if (result.SkillDamage > 0)
                {
                    var hits = result.SkillHits > 1 ? $" ({result.SkillHits} golpes)" : "";
                    narrative += $"💥 Daño: {result.SkillDamage}{hits}\n";
                }
                if (result.SkillHealed > 0)
                {
                    narrative += $"💚 Cura: +{result.SkillHealed} HP\n";
                }
                if (result.InflictedEffect != null)
                {
                    narrative += $"🩸 *¡Infligiste {GetEffectName(result.InflictedEffect.Value)}!*\n";
                }
                narrative += "\n";
            }
            else if (result.PlayerDefended)
            {
                narrative += "🛡️ *Adoptas postura defensiva*\n\n";
            }
            else if (result.PlayerHit)
            {
                // Tipo de ataque
                var attackEmoji = result.AttackType == AttackType.Magical ? "🔮" : "⚔️";
                var attackType = result.AttackType == AttackType.Magical ? "Mágico" : "Físico";
                
                if (result.PlayerCritical)
                {
                    narrative += $"✨ **¡GOLPE CRÍTICO {attackType.ToUpper()}!**\n";
                }
                else
                {
                    narrative += $"{attackEmoji} *Ataque {attackType} preciso*\n";
                }
                
                // Mostrar sistema de probabilidades
                narrative += $"🎯 Probabilidad: {result.HitChancePercent:F1}% | Roll: {result.HitRoll:F1}%\n";
                if (result.PlayerCritical)
                {
                    narrative += $"💫 Crítico: {result.CriticalChancePercent:F1}% | Roll: {result.CriticalRoll:F1}%\n";
                }
                
                narrative += $"💥 Daño: {result.PlayerDamage}";
                
                if (result.DamageReduction > 0)
                {
                    narrative += $" (bloqueado: {result.DamageReduction})";
                }
                
                if (result.ComboBonus > 0)
                {
                    narrative += $" ⚡ (+{result.ComboBonus} combo)";
                }
                narrative += "\n";
                
                if (result.InflictedEffect != null)
                {
                    narrative += $"🩸 *¡Infligiste {GetEffectName(result.InflictedEffect.Value)}!*\n";
                }
                
                narrative += "\n";
            }
            else
            {
                narrative += $"❌ *Tu ataque falla*\n";
                narrative += $"🎯 Probabilidad: {result.HitChancePercent:F1}% | Roll: {result.HitRoll:F1}%\n";
                if (result.ComboBroken)
                {
                    narrative += "💔 *Combo roto*\n";
                }
                narrative += "\n";
            }
            
            // ═══ TURNOS DE MASCOTAS ═══
            if (result.PetTurns != null && result.PetTurns.Any())
            {
                narrative += "━━━━━━━━━━━━━━━\n";
                narrative += "🐾 **MASCOTAS COMPAÑERAS**\n\n";
                
                foreach (var petTurn in result.PetTurns)
                {
                    if (petTurn.Hit)
                    {
                        var attackEmoji = petTurn.AttackType == AttackType.Magical ? "🔮" : "⚔️";
                        var critText = petTurn.Critical ? " ⚡ CRÍTICO" : "";
                        narrative += $"{petTurn.Emoji} **{petTurn.PetName}**: {attackEmoji} {petTurn.Damage} daño{critText}\n";
                        
                        if (petTurn.InflictedEffect != null)
                        {
                            narrative += $"   🩸 *Infligió {GetEffectName(petTurn.InflictedEffect.Value)}*\n";
                        }
                    }
                    else
                    {
                        narrative += $"{petTurn.Emoji} **{petTurn.PetName}**: 💨 Falla\n";
                    }
                }
                
                if (result.TotalPetDamage > 0)
                {
                    narrative += $"\n💥 **Total mascotas**: {result.TotalPetDamage} daño\n";
                }
                narrative += "\n";
            }
            
            // Status damage
            if (result.StatusDamage > 0)
            {
                narrative += $"🩸 *Efectos de estado: -{result.StatusDamage} HP*\n\n";
            }
            
            // Enemy status
            if (result.EnemyDefeated)
            {
                narrative += $"✅ **¡{enemy.Emoji} {enemy.Name} derrotado!**\n\n";
                narrative += $"🎖️ +{result.XPGained} XP\n";
                narrative += $"💰 +{result.GoldGained} oro\n\n";
                
                if (result.LootDrop != null)
                {
                    narrative += $"💎 Loot: **{result.LootDrop.Name}** {result.LootDrop.RarityEmoji}\n\n";
                }
                
                // Mostrar resumen de combate
                if (player.CombatLog.Count > 0)
                {
                    narrative += "📊 **Resumen del Combate:**\n";
                    narrative += $"• Duración: {player.CombatTurnCount} turnos\n";
                    var maxCombo = player.CombatLog
                        .Where(l => l.Action.Contains("Combo"))
                        .Select(l => {
                            var match = System.Text.RegularExpressions.Regex.Match(l.Action, @"x(\d+)");
                            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();
                    if (maxCombo > 1)
                    {
                        narrative += $"• Combo máximo: x{maxCombo}\n";
                    }
                    narrative += "\n";
                }
                
                narrative += "🎉 ¡Victoria!";
                
                // Limpiar log después de mostrar
                player.CombatLog.Clear();
                return narrative;
            }
            
            // Enemy counterattack
            if (!result.PlayerDefended && result.EnemyHit)
            {
                if (result.EnemyCritical)
                {
                    narrative += $"💀 **¡{enemy.Name} crítico!**\n";
                }
                else
                {
                    narrative += $"⚔️ *{enemy.Name} contraataca*\n";
                }
                narrative += $"🎯 Probabilidad: {result.EnemyHitChancePercent:F1}% | Roll: {result.EnemyHitRoll:F1}%\n";
                narrative += $"🩸 Daño: {result.EnemyDamage}\n";
                if (result.ComboBroken && !result.PlayerDefended)
                {
                    narrative += "💔 *Combo roto*\n";
                }
                narrative += "\n";
            }
            else if (result.PlayerDefended && result.EnemyHit)
            {
                narrative += $"🛡️ *Bloqueas parcialmente el ataque*\n";
                narrative += $"🎯 Probabilidad: {result.EnemyHitChancePercent:F1}% | Roll: {result.EnemyHitRoll:F1}%\n";
                narrative += $"🩸 Daño reducido: {result.EnemyDamage}\n\n";
            }
            else if (!result.EnemyHit && result.EnemyHitChancePercent > 0)
            {
                narrative += $"🛡️ *¡Esquivas el ataque!*\n";
                narrative += $"🎯 Probabilidad enemiga: {result.EnemyHitChancePercent:F1}% | Roll: {result.EnemyHitRoll:F1}%\n\n";
            }
            
            // Combat status
            narrative += "━━━━━━━━━━━━━━━\n";
            narrative += $"👤 **{player.Name}**: ";
            
            // HP bar
            var hpPercent = (double)player.HP / player.MaxHP;
            var hpEmoji = hpPercent > 0.7 ? "💚" : hpPercent > 0.3 ? "💛" : "❤️";
            narrative += $"{hpEmoji} {player.HP}/{player.MaxHP} HP";
            
            // Combo counter
            if (player.ComboCount > 0)
            {
                narrative += $" ⚡x{player.ComboCount}";
            }
            
            narrative += "\n";
            
            // Player status effects
            if (player.StatusEffects.Any())
            {
                narrative += "   🔮 " + string.Join(", ", player.StatusEffects.Select(e => 
                    GetEffectEmoji(e.Type) + $" {GetEffectName(e.Type)} ({e.Duration})"
                )) + "\n";
            }
            
            narrative += $"\n{enemy.Emoji} **{enemy.Name}**: ";
            
            // Enemy HP
            var enemyHpPercent = (double)enemy.HP / enemy.MaxHP;
            var enemyHpEmoji = enemyHpPercent > 0.7 ? "💚" : enemyHpPercent > 0.3 ? "💛" : "❤️";
            narrative += $"{enemyHpEmoji} {enemy.HP}/{enemy.MaxHP} HP\n";
            
            // Enemy status effects
            if (enemy.StatusEffects.Any())
            {
                narrative += "   🔮 " + string.Join(", ", enemy.StatusEffects.Select(e => 
                    GetEffectEmoji(e.Type) + $" {GetEffectName(e.Type)} ({e.Duration})"
                )) + "\n";
            }
            
            if (result.PlayerDefeated)
            {
                narrative += "\n💀 **Has sido derrotado...**";
                
                // Mostrar resumen de combate
                if (player.CombatLog.Count > 0)
                {
                    narrative += "\n\n📊 **Resumen:**\n";
                    narrative += $"• Duraste {player.CombatTurnCount} turnos\n";
                }
                
                player.CombatLog.Clear();
            }
            
            return narrative;
        }
        
        // Método auxiliar: emoji de efecto
        private string GetEffectEmoji(StatusEffectType type)
        {
            return type switch
            {
                StatusEffectType.Bleeding => "🩸",
                StatusEffectType.Poisoned => "🧪",
                StatusEffectType.Stunned => "💫",
                StatusEffectType.Burning => "🔥",
                StatusEffectType.Frozen => "❄️",
                StatusEffectType.Regenerating => "💚",
                StatusEffectType.Shielded => "🛡️",
                StatusEffectType.Empowered => "⚡",
                _ => "✨"
            };
        }
        
        // ═══════════════════════════════════════
        // SISTEMA DE MINIONS (FASE 5A)
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Ejecuta el turno de todos los minions activos
        /// </summary>
        public void ExecuteMinionsTurn(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            if (player.ActiveMinions.Count == 0)
                return;
            
            var log = new System.Text.StringBuilder();
            log.AppendLine("\n⚔️ **TURNO DE ESBIRROS**");
            
            // Crear lista de minions que sobrevivieron
            var survivingMinions = new List<Minion>();
            
            var totalDamageToEnemy = 0;
            var totalDamageToPlayer = 0;
            var betrayals = 0;
            
            foreach (var minion in player.ActiveMinions)
            {
                // Decrementar turnos
                minion.TickTurn();
                
                // Si expiró, eliminar
                if (minion.TurnsRemaining <= 0)
                {
                    log.AppendLine($"💀 {minion.Emoji} **{minion.Name}** desaparece (duración terminada)...");
                    TrackAction(player, $"minion_expired_{minion.Type.ToString().ToLower()}");
                    continue;
                }
                
                // Calcular probabilidad de golpe (85% base - defensa enemiga)
                var hitChance = 85.0 - (enemy.PhysicalDefense * 0.3);
                hitChance = Math.Max(30.0, Math.Min(95.0, hitChance));
                var hitRoll = Random.Shared.NextDouble() * 100.0;
                
                // Si está controlado, ataca al enemigo
                if (minion.IsControlled)
                {
                    if (hitRoll <= hitChance)
                    {
                        var damage = CalculateMinionDamage(minion, enemy);
                        enemy.HP -= damage;
                        totalDamageToEnemy += damage;
                        
                        log.AppendLine($"{minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t)");
                        log.AppendLine($"  🎯 Hit: {hitRoll:F1}% ≤ {hitChance:F1}% → **{damage}** daño");
                        TrackAction(player, $"minion_attack_{minion.Type.ToString().ToLower()}");
                    }
                    else
                    {
                        log.AppendLine($"{minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t)");
                        log.AppendLine($"  ❌ Fallo: {hitRoll:F1}% > {hitChance:F1}%");
                    }
                }
                else
                {
                    // NO controlado: 30% ataca al jugador, 70% ataca al enemigo
                    var loyaltyRoll = _random.Next(0, 100);
                    var loyaltyThreshold = 30;
                    
                    if (loyaltyRoll < loyaltyThreshold)
                    {
                        // Traición: ataca al jugador
                        betrayals++;
                        
                        if (hitRoll <= hitChance)
                        {
                            var damage = CalculateMinionDamage(minion, null);
                            player.HP -= damage;
                            totalDamageToPlayer += damage;
                            
                            log.AppendLine($"😱 {minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t) ⚠️ NO CONTROLADO");
                            log.AppendLine($"  🎲 Fidelidad: {loyaltyRoll}% < {loyaltyThreshold}% → ¡TE ATACA!");
                            log.AppendLine($"  🎯 Hit: {hitRoll:F1}% ≤ {hitChance:F1}% → **{damage}** daño");
                            TrackAction(player, "minion_betrayal");
                        }
                        else
                        {
                            log.AppendLine($"😱 {minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t) ⚠️ NO CONTROLADO");
                            log.AppendLine($"  🎲 Fidelidad: {loyaltyRoll}% < {loyaltyThreshold}% → ¡TE ATACA!");
                            log.AppendLine($"  ❌ Fallo: {hitRoll:F1}% > {hitChance:F1}%");
                        }
                    }
                    else
                    {
                        // Ataca al enemigo (aunque no esté controlado)
                        if (hitRoll <= hitChance)
                        {
                            var damage = CalculateMinionDamage(minion, enemy);
                            enemy.HP -= damage;
                            totalDamageToEnemy += damage;
                            
                            log.AppendLine($"{minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t) ⚠️ NO CONTROLADO");
                            log.AppendLine($"  🎲 Fidelidad: {loyaltyRoll}% ≥ {loyaltyThreshold}% → Ataca enemigo");
                            log.AppendLine($"  🎯 Hit: {hitRoll:F1}% ≤ {hitChance:F1}% → **{damage}** daño");
                            TrackAction(player, $"minion_attack_{minion.Type.ToString().ToLower()}");
                        }
                        else
                        {
                            log.AppendLine($"{minion.Emoji} **{minion.Name}** ({minion.HP}/{minion.MaxHP} HP, {minion.TurnsRemaining}t) ⚠️ NO CONTROLADO");
                            log.AppendLine($"  🎲 Fidelidad: {loyaltyRoll}% ≥ {loyaltyThreshold}% → Ataca enemigo");
                            log.AppendLine($"  ❌ Fallo: {hitRoll:F1}% > {hitChance:F1}%");
                        }
                    }
                }
                
                survivingMinions.Add(minion);
            }
            
            // Resumen de daño
            log.AppendLine($"\n📊 **Resumen:**");
            log.AppendLine($"  ⚔️ Daño al enemigo: **{totalDamageToEnemy}**");
            if (totalDamageToPlayer > 0)
            {
                log.AppendLine($"  💔 Daño recibido de esbirros: **{totalDamageToPlayer}** ({betrayals} traiciones)");
            }
            
            // Actualizar lista de minions
            player.ActiveMinions = survivingMinions;
            
            if (log.Length > 0)
            {
                AddCombatLog(player, "Minions", log.ToString());
            }
            
            // Verificar si el jugador murió por sus propios minions
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                AddCombatLog(player, "Derrota", "💀 Fuiste asesinado por tus propios esbirros...");
            }
        }
        
        /// <summary>
        /// Calcula el daño de un minion
        /// </summary>
        private int CalculateMinionDamage(Minion minion, RpgEnemy? enemy)
        {
            var baseDamage = minion.Attack;
            
            // Variación 90-110%
            var variation = _random.Next(90, 111) / 100.0;
            baseDamage = (int)(baseDamage * variation);
            
            // Aplicar defensa del enemigo (si hay enemigo)
            if (enemy != null)
            {
                var damageReduction = enemy.PhysicalDefense * 0.5;
                baseDamage = Math.Max(1, (int)(baseDamage - damageReduction));
            }
            
            return baseDamage;
        }
        
        /// <summary>
        /// Invoca un minion al combate
        /// </summary>
        public string SummonMinion(RpgPlayer player, MinionType type)
        {
            // Verificar límite de minions
            if (player.ActiveMinions.Count >= player.MaxActiveMinions)
            {
                return $"❌ Ya tienes el máximo de esbirros activos ({player.MaxActiveMinions})";
            }
            
            // Crear minion escalado al nivel del jugador
            var minion = MinionDatabase.CreateMinion(type, player.Level);
            
            if (minion == null)
            {
                return "❌ Error al crear el esbirro";
            }
            
            // Agregar a la lista
            player.ActiveMinions.Add(minion);
            
            // Track invocación
            TrackAction(player, $"summon_{type.ToString().ToLower()}");
            
            var controlText = minion.IsControlled ? "✅ CONTROLADO" : "⚠️ NO CONTROLADO";
            var info = MinionDatabase.GetMinionInfo(type);
            
            return $"{minion.Emoji} **{minion.Name}** invocado ({minion.HP} HP, {minion.TurnsRemaining} turnos) {controlText}\n" +
                   $"📋 {info.Description}\n" +
                   $"✨ {info.SpecialAbility}";
        }
        
        /// <summary>
        /// Sacrifica un minion para curar al jugador
        /// </summary>
        public string SacrificeMinion(RpgPlayer player, int minionIndex)
        {
            if (minionIndex < 0 || minionIndex >= player.ActiveMinions.Count)
            {
                return "❌ Esbirro no válido";
            }
            
            var minion = player.ActiveMinions[minionIndex];
            
            // Calcular curación: HP restante del minion + 50% del ATK del jugador
            var healAmount = minion.HP + (int)(player.PhysicalAttack * 0.5);
            player.HP = Math.Min(player.MaxHP, player.HP + healAmount);
            
            // Remover minion
            player.ActiveMinions.RemoveAt(minionIndex);
            
            // Track sacrificio
            TrackAction(player, $"sacrifice_{minion.Type.ToString().ToLower()}");
            TrackAction(player, "sacrifice_minion");
            
            AddCombatLog(player, "Sacrificio", $"💀 Sacrificaste {minion.Emoji} **{minion.Name}** → +{healAmount} HP");
            
            return $"💀 Sacrificaste {minion.Emoji} **{minion.Name}**\n" +
                   $"💚 Restauraste **{healAmount} HP** (HP restante: {minion.HP} + bono: {(int)(player.PhysicalAttack * 0.5)})";
        }
        
        /// <summary>
        /// Obtiene el estado de todos los minions activos
        /// </summary>
        public string GetMinionsStatus(RpgPlayer player)
        {
            if (player.ActiveMinions.Count == 0)
            {
                return "Sin esbirros activos";
            }
            
            var status = new System.Text.StringBuilder();
            status.AppendLine($"⚔️ **ESBIRROS ACTIVOS** ({player.ActiveMinions.Count}/{player.MaxActiveMinions}):\n");
            
            for (int i = 0; i < player.ActiveMinions.Count; i++)
            {
                var minion = player.ActiveMinions[i];
                var controlEmoji = minion.IsControlled ? "✅" : "⚠️";
                status.AppendLine($"{i + 1}. {minion.Emoji} **{minion.Name}** {controlEmoji}");
                status.AppendLine($"   ❤️ HP: {minion.HP}/{minion.MaxHP} | ⚔️ ATK: {minion.Attack} | 🛡️ DEF: {minion.Defense}");
                status.AppendLine($"   ⏰ Turnos restantes: {minion.TurnsRemaining}");
            }
            
            return status.ToString();
        }
    }
    
    public class CombatResult
    {
        // Información del jugador
        public bool PlayerHit { get; set; }
        public bool PlayerCritical { get; set; }
        public int PlayerDamage { get; set; }
        public bool PlayerDefended { get; set; }
        public bool PlayerStunned { get; set; }
        public bool SkillUsed { get; set; }
        public string? SkillName { get; set; }
        public string? SkillFailureReason { get; set; }
        public string? SkillDetails { get; set; } // Detalles adicionales (invocaciones, efectos especiales)
        public int SkillDamage { get; set; }
        public int SkillHits { get; set; }
        public int SkillHealed { get; set; }
        
        // Sistema de probabilidades (jugador)
        public double HitChancePercent { get; set; }
        public double HitRoll { get; set; }
        public double CriticalChancePercent { get; set; }
        public double CriticalRoll { get; set; }
        public AttackType AttackType { get; set; } = AttackType.Physical;
        public int DamageReduction { get; set; } // Daño absorbido por defensa
        
        // Nuevas propiedades de acciones avanzadas
        public bool Dodged { get; set; }
        public bool Blocked { get; set; }
        public bool Countered { get; set; }
        public string? RevealedInfo { get; set; }
        
        // Información del enemigo
        public bool EnemyHit { get; set; }
        public bool EnemyCritical { get; set; }
        public int EnemyDamage { get; set; }
        
        // Sistema de probabilidades (enemigo)
        public double EnemyHitChancePercent { get; set; }
        public double EnemyHitRoll { get; set; }
        
        // Resultado del combate
        public bool EnemyDefeated { get; set; }
        public bool PlayerDefeated { get; set; }
        
        public int XPGained { get; set; }
        public int GoldGained { get; set; }
        public RpgEquipment? LootDrop { get; set; }
        
        // Sistema de combos
        public int ComboBonus { get; set; }
        public bool ComboBroken { get; set; }
        
        // Efectos de estado
        public StatusEffectType? InflictedEffect { get; set; }
        public int StatusDamage { get; set; }
        
        // Skills desbloqueadas al terminar combate
        public List<RpgSkill> UnlockedSkills { get; set; } = new();
        
        // Sistema de mascotas (FASE 2)
        public List<PetTurnResult> PetTurns { get; set; } = new();
        public int TotalPetDamage { get; set; }
        
        // Legacy (compatibilidad)
        [Obsolete("Use HitChancePercent y HitRoll instead")]
        public int PlayerRoll { get; set; }
        
        [Obsolete("Use EnemyHitChancePercent y EnemyHitRoll instead")]
        public int EnemyRoll { get; set; }
    }
    
    public class PetTurnResult
    {
        public string PetName { get; set; } = "";
        public bool Hit { get; set; }
        public bool Critical { get; set; }
        public int Damage { get; set; }
        public double HitChance { get; set; }
        public double HitRoll { get; set; }
        public AttackType AttackType { get; set; }
        public StatusEffectType? InflictedEffect { get; set; }
        public string Emoji { get; set; } = "🐾";
    }
    
}