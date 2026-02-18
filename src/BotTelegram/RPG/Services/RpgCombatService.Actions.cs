using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio extendido para manejar todas las acciones de combate avanzadas
    /// </summary>
    public partial class RpgCombatService
    {
        // ═══════════════════════════════════════════════════════════════
        // ACCIONES DE ATAQUE AVANZADAS
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Envestida: Corre hacia el enemigo e impacta (+30% daño, -10% precisión, cuesta Stamina)
        /// </summary>
        public CombatResult ChargeAttack(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            // Costo de stamina
            const int staminaCost = 15;
            if (player.Stamina < staminaCost)
            {
                result.PlayerHit = false;
                AddCombatLog(player, "Envestida", "❌ Stamina insuficiente");
                return result;
            }
            
            player.Stamina -= staminaCost;
            
            // Hit chance reducido (-10%)
            double baseHitChance = 75.0;
            double accuracyBonus = (player.Accuracy - enemy.Evasion) * 0.5;
            double hitChance = Math.Clamp(baseHitChance + accuracyBonus, 10.0, 95.0);
            
            double hitRoll = _random.Next(0, 10000) / 100.0;
            result.HitChancePercent = hitChance;
            result.HitRoll = hitRoll;
            result.PlayerHit = hitRoll <= hitChance;
            
            if (result.PlayerHit)
            {
                player.ComboCount++;
                TrackAction(player, "charge_attack"); // Track para skill unlocks
                
                // Daño aumentado (+30%)
                int baseDamage = (int)(player.PhysicalAttack * 1.3);
                var damageVariation = _random.Next(90, 111) / 100.0;
                baseDamage = (int)(baseDamage * damageVariation);
                
                // Aplicar defensa
                var damageType = DamageType.Bludgeoning; // Envestida es daño contundente
                var multiplier = enemy.GetDamageMultiplier(damageType);
                baseDamage = (int)(baseDamage * multiplier);
                
                double damageReduction = enemy.PhysicalDefense / (enemy.PhysicalDefense + 100.0);
                int finalDamage = (int)(baseDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage);
                result.AttackType = AttackType.Physical;
                enemy.HP -= result.PlayerDamage;
                TrackDamageDealt(player, result.PlayerDamage); // Track daño total
                
                // Chance de aturdir (20%)
                if (_random.Next(100) < 20 && !enemy.IsImmuneToEffect(StatusEffectType.Stunned))
                {
                    enemy.StatusEffects.Add(new StatusEffect(StatusEffectType.Stunned, 1, 0));
                    result.InflictedEffect = StatusEffectType.Stunned;
                }
                
                AddCombatLog(player, "💨 Envestida", $"💥 {result.PlayerDamage} daño");
            }
            else
            {
                player.ComboCount = 0;
                AddCombatLog(player, "💨 Envestida", "❌ Fallo");
            }
            
            ProcessEnemyTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Ataque Preciso: +20% precisión, -20% daño
        /// </summary>
        public CombatResult PreciseAttack(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            // Hit chance aumentado (+20%)
            double baseHitChance = 95.0; // Casi siempre acierta
            double accuracyBonus = (player.Accuracy - enemy.Evasion) * 0.5;
            double hitChance = Math.Clamp(baseHitChance + accuracyBonus, 50.0, 99.0);
            
            double hitRoll = _random.Next(0, 10000) / 100.0;
            result.HitChancePercent = hitChance;
            result.HitRoll = hitRoll;
            result.PlayerHit = hitRoll <= hitChance;
            
            if (result.PlayerHit)
            {
                player.ComboCount++;
                TrackAction(player, "precise_attack"); // Track para skill unlocks
                
                // Daño reducido (-20%)
                int baseDamage = (int)(player.PhysicalAttack * 0.8);
                var damageVariation = _random.Next(95, 106) / 100.0; // Menos variación
                baseDamage = (int)(baseDamage * damageVariation);
                
                var damageType = DamageType.Piercing;
                var multiplier = enemy.GetDamageMultiplier(damageType);
                baseDamage = (int)(baseDamage * multiplier);
                
                double damageReduction = enemy.PhysicalDefense / (enemy.PhysicalDefense + 100.0);
                int finalDamage = (int)(baseDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage);
                result.AttackType = AttackType.Physical;
                enemy.HP -= result.PlayerDamage;
                TrackDamageDealt(player, result.PlayerDamage); // Track daño total
                
                AddCombatLog(player, "🎯 Ataque Preciso", $"💥 {result.PlayerDamage} daño");
            }
            else
            {
                player.ComboCount = 0;
                AddCombatLog(player, "🎯 Ataque Preciso", "❌ Fallo");
            }
            
            ProcessEnemyTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Ataque Pesado: +50% daño, -20% precisión, alto costo stamina
        /// </summary>
        public CombatResult HeavyAttack(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            const int staminaCost = 20;
            if (player.Stamina < staminaCost)
            {
                result.PlayerHit = false;
                AddCombatLog(player, "Ataque Pesado", "❌ Stamina insuficiente");
                return result;
            }
            
            player.Stamina -= staminaCost;
            
            double baseHitChance = 70.0;
            double accuracyBonus = (player.Accuracy - enemy.Evasion) * 0.5;
            double hitChance = Math.Clamp(baseHitChance + accuracyBonus, 10.0, 90.0);
            
            double hitRoll = _random.Next(0, 10000) / 100.0;
            result.HitChancePercent = hitChance;
            result.HitRoll = hitRoll;
            result.PlayerHit = hitRoll <= hitChance;
            
            if (result.PlayerHit)
            {
                player.ComboCount++;
                TrackAction(player, "heavy_attack"); // Track para skill unlocks
                
                int baseDamage = (int)(player.PhysicalAttack * 1.5);
                var damageVariation = _random.Next(90, 111) / 100.0;
                baseDamage = (int)(baseDamage * damageVariation);
                
                var damageType = DamageType.Bludgeoning;
                var multiplier = enemy.GetDamageMultiplier(damageType);
                baseDamage = (int)(baseDamage * multiplier);
                
                double damageReduction = enemy.PhysicalDefense / (enemy.PhysicalDefense + 100.0);
                int finalDamage = (int)(baseDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage);
                result.AttackType = AttackType.Physical;
                enemy.HP -= result.PlayerDamage;
                TrackDamageDealt(player, result.PlayerDamage); // Track daño total
                
                AddCombatLog(player, "💥 Ataque Pesado", $"💥 {result.PlayerDamage} daño");
            }
            else
            {
                player.ComboCount = 0;
                AddCombatLog(player, "💥 Ataque Pesado", "❌ Fallo");
            }
            
            ProcessEnemyTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // ACCIONES DEFENSIVAS
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Esquivar: Intenta evitar completamente el ataque (basado en DEX)
        /// </summary>
        public CombatResult DodgeAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            // Chance de esquivar basado en Evasion
            double dodgeChance = Math.Clamp(player.Evasion * 1.5, 30.0, 80.0);
            double dodgeRoll = _random.Next(0, 10000) / 100.0;
            
            bool dodged = dodgeRoll <= dodgeChance;
            
            if (dodged)
            {
                result.Dodged = true;
                TrackAction(player, "dodge"); // Track para skill unlocks
                TrackPerfectDodge(player); // Track esquives perfectos
                
                // Mensaje mejorado:
                var bonusEvasion = player.Evasion * 0.5; // El bonus agregado (1.5x - 1.0x = 0.5x)
                AddCombatLog(player, "🌀 Esquivar", 
                    $"✅ Exitoso\n" +
                    $"📊 Base: {player.Evasion:F1} + Bonus: {bonusEvasion:F1} = {dodgeChance:F1}%\n" +
                    $"🎲 Roll: {dodgeRoll:F1}% ≤ {dodgeChance:F1}%\n" +
                    $"💨 +20% Evasión en próximo turno\n" +
                    $"⚡ +10 Iniciativa ganada");
                
                // Enemigo ataca y falla
                result.EnemyHit = false;
                AddCombatLog(player, $"{enemy.Name}", "💨 Ataque esquivado");
            }
            else
            {
                result.Dodged = false;
                AddCombatLog(player, "🌀 Esquivar", 
                    $"❌ Fallo\n" +
                    $"📊 Probabilidad: {dodgeChance:F1}%\n" +
                    $"🎲 Roll: {dodgeRoll:F1}% > {dodgeChance:F1}%");
                
                // Recibe ataque normal
                PerformEnemyAttack(player, enemy, result, 0);
            }
            
            player.ComboCount = 0; // Rompe combo
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Contraataque: Si esquiva, contraataca automáticamente
        /// </summary>
        public CombatResult CounterAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            const int staminaCost = 20;
            if (player.Stamina < staminaCost)
            {
                result.PlayerHit = false;
                AddCombatLog(player, "Contraataque", "❌ Stamina insuficiente");
                return result;
            }
            
            player.Stamina -= staminaCost;
            
            double counterChance = Math.Clamp(player.Evasion * 1.2, 25.0, 70.0);
            double counterRoll = _random.Next(0, 10000) / 100.0;
            
            if (counterRoll <= counterChance)
            {
                result.Countered = true;
                result.EnemyHit = false;
                TrackAction(player, "counter"); // Track para skill unlocks
                
                // Contraataque exitoso - daña al enemigo
                int counterDamage = (int)(player.PhysicalAttack * 1.2);
                var damageType = DamageType.Slashing;
                var multiplier = enemy.GetDamageMultiplier(damageType);
                counterDamage = (int)(counterDamage * multiplier);
                
                double damageReduction = enemy.PhysicalDefense / (enemy.PhysicalDefense + 100.0);
                int finalDamage = (int)(counterDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage);
                enemy.HP -= result.PlayerDamage;
                TrackDamageDealt(player, result.PlayerDamage); // Track daño total
                
                AddCombatLog(player, "💫 Contraataque", $"✅ ¡Éxito! {result.PlayerDamage} daño");
            }
            else
            {
                result.Countered = false;
                AddCombatLog(player, "💫 Contraataque", "❌ Fallo");
                PerformEnemyAttack(player, enemy, result, 0);
            }
            
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // ACCIONES DE MOVIMIENTO
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Saltar: Evita ataques de zona, posicionamiento temporal
        /// </summary>
        public CombatResult JumpAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            const int staminaCost = 10;
            if (player.Stamina < staminaCost)
            {
                result.PlayerHit = false;
                AddCombatLog(player, "Saltar", "❌ Stamina insuficiente");
                return result;
            }
            
            player.Stamina -= staminaCost;
            
            TrackAction(player, "jump"); // Track para skill unlocks
            
            // Gana evasión temporal para próximo turno
            double jumpBonus = 25.0;
            double enemyHitChance = Math.Max(10.0, 80.0 - jumpBonus - (player.Evasion * 0.5));
            double enemyHitRoll = _random.Next(0, 10000) / 100.0;
            
            result.EnemyHit = enemyHitRoll <= enemyHitChance;
            
            if (!result.EnemyHit)
            {
                AddCombatLog(player, "🦘 Saltar", "✅ Evitaste el ataque");
            }
            else
            {
                PerformEnemyAttack(player, enemy, result, 0);
            }
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Retroceder: +20% evasión, -30% ataque siguiente turno
        /// </summary>
        public CombatResult RetreatAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            TrackAction(player, "retreat"); // Track para skill unlocks
            
            // Mejora evasión pero no ataca
            double retreatBonus = 30.0;
            double enemyHitChance = Math.Max(10.0, 80.0 - retreatBonus - (player.Evasion * 0.5));
            double enemyHitRoll = _random.Next(0, 10000) / 100.0;
            
            result.EnemyHit = enemyHitRoll <= enemyHitChance;
            
            if (!result.EnemyHit)
            {
                AddCombatLog(player, "🏃 Retroceder", "✅ Mantienes distancia");
            }
            else
            {
                // Daño reducido por la distancia
                PerformEnemyAttack(player, enemy, result, 10);
            }
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Avanzar: +10% daño siguiente turno, -10% defensa
        /// </summary>
        public CombatResult AdvanceAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            TrackAction(player, "advance"); // Track para skill unlocks
            
            AddCombatLog(player, "⚡ Avanzar", "Te acercas al enemigo");
            
            // Recibe ataque con defensas reducidas
            PerformEnemyAttack(player, enemy, result, -5);
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // ACCIONES ESPECIALES
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Meditar: Recupera 25% Mana, vulnerable durante turno
        /// </summary>
        public CombatResult MeditateAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            TrackAction(player, "meditate"); // Track para skill unlocks
            
            int manaRestore = (int)(player.MaxMana * 0.25);
            player.Mana = Math.Min(player.MaxMana, player.Mana + manaRestore);
            TrackAction(player, "mana_regen", manaRestore); // Track mana regenerado
            
            AddCombatLog(player, "🧘 Meditar", $"💙 +{manaRestore} Mana\n⚠️ Vulnerable (-15% defensa)\n💥 Enemigo ataca con ventaja");
            
            // Vulnerable a ataque enemigo (sin defensas)
            var tempDefense = player.PhysicalDefense;
            // Enemy ataca con bonus
            PerformEnemyAttack(player, enemy, result, -15);
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Observar: Revela info del enemigo (resistencias, debilidades)
        /// </summary>
        public CombatResult ObserveAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            TrackAction(player, "observe"); // Track para skill unlocks
            
            string info = $"📊 **{enemy.Name}** (Lv.{enemy.Level})\n\n";
            info += $"❤️ HP: {enemy.HP}/{enemy.MaxHP}\n";
            info += $"⚔️ Ataque: {enemy.Attack} | 🔮 Magia: {enemy.MagicPower}\n";
            info += $"🛡️ Def Física: {enemy.PhysicalDefense} | 🌀 Def Mágica: {enemy.MagicResistance}\n";
            info += $"🎯 Precisión: {enemy.Accuracy} | 💨 Evasión: {enemy.Evasion}\n";
            info += $"⚡ Velocidad: {enemy.Speed}/10\n\n";
            
            if (enemy.Weaknesses.Any())
            {
                info += "💥 **Debilidades:**\n";
                foreach (var w in enemy.Weaknesses)
                {
                    info += $"   • {w.Key}: x{w.Value:F1}\n";
                }
                info += "\n";
            }
            
            if (enemy.Resistances.Any())
            {
                info += "🛡️ **Resistencias:**\n";
                foreach (var r in enemy.Resistances)
                {
                    info += $"   • {r.Key}: -{(r.Value * 100):F0}%\n";
                }
                info += "\n";
            }
            
            if (enemy.Immunities.Any())
            {
                info += "❌ **Inmunidades:**\n";
                foreach (var i in enemy.Immunities)
                {
                    info += $"   • {i}\n";
                }
            }
            
            result.RevealedInfo = info;
            AddCombatLog(player, "👁️ Observar", "Analizas al enemigo");
            
            // Enemigo aprovecha para atacar
            PerformEnemyAttack(player, enemy, result, 0);
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Esperar/Pasar turno: Regenera 10% Stamina, enemigo ataca
        /// </summary>
        public CombatResult WaitAction(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            TrackAction(player, "wait"); // Track para skill unlocks
            
            int staminaRestore = (int)(player.MaxStamina * 0.10);
            player.Stamina = Math.Min(player.MaxStamina, player.Stamina + staminaRestore);
            
            AddCombatLog(player, "⏸️ Esperar", $"⚡ +{staminaRestore} Stamina");
            
            PerformEnemyAttack(player, enemy, result, 0);
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }

        /// <summary>
        /// Usar una skill desbloqueada durante el combate
        /// </summary>
        public CombatResult UseSkill(RpgPlayer player, RpgEnemy enemy, string skillId)
        {
            var result = new CombatResult();
            StartPlayerTurn(player);
            
            var skill = SkillDatabase.GetAllSkills().FirstOrDefault(s => s.Id == skillId);
            if (skill == null || !player.UnlockedSkills.Contains(skillId))
            {
                result.SkillFailureReason = "Skill no desbloqueada";
                AddCombatLog(player, "✨ Skill", "❌ No desbloqueada");
                ProcessEnemyTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            if (player.SkillCooldowns.ContainsKey(skillId) && player.SkillCooldowns[skillId] > 0)
            {
                result.SkillFailureReason = $"Skill en cooldown ({player.SkillCooldowns[skillId]})";
                AddCombatLog(player, skill.Name, "⏱️ En cooldown");
                ProcessEnemyTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            // FASE 5A: Verificar si es una skill de invocación
            if (skillId.StartsWith("summon_"))
            {
                return HandleSummoningSkill(player, enemy, skill, result);
            }
            if (skillId == "sacrifice_minion")
            {
                return HandleSacrificeSkill(player, enemy, skill, result);
            }
            
            if (player.Mana < skill.ManaCost || player.Stamina < skill.StaminaCost)
            {
                result.SkillFailureReason = "Recursos insuficientes";
                AddCombatLog(player, skill.Name, "❌ Recursos insuficientes");
                ProcessEnemyTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            player.Mana -= skill.ManaCost;
            player.Stamina -= skill.StaminaCost;
            if (skill.ManaCost > 0)
            {
                TrackAction(player, "mana_spent", skill.ManaCost);
            }
            if (skill.Cooldown > 0)
            {
                player.SkillCooldowns[skillId] = skill.Cooldown;
            }
            
            result.SkillUsed = true;
            result.SkillName = skill.Name;
            TrackSkillUsed(player, skillId);
            
            // ═══ FASE 4: Tracking para desbloqueo de clases ═══
            if (skill.Category == SkillCategory.Magic || skill.ManaCost > 0)
            {
                if (!player.ActionCounters.ContainsKey("magic_cast"))
                    player.ActionCounters["magic_cast"] = 0;
                player.ActionCounters["magic_cast"]++;
            }
            
            if (skill.HealAmount > 0)
            {
                var heal = Math.Min(skill.HealAmount, player.MaxHP - player.HP);
                player.HP += heal;
                result.SkillHealed = heal;
                // Fase 4: trackear HP curado
                if (heal > 0)
                {
                    if (!player.ActionCounters.ContainsKey("hp_healed"))
                        player.ActionCounters["hp_healed"] = 0;
                    player.ActionCounters["hp_healed"] += heal;
                }
            }
            
            var baseDamage = skill.Category == SkillCategory.Magic ? player.MagicalAttack : player.PhysicalAttack;
            if (skill.DamageMultiplier > 0)
            {
                baseDamage = (int)(baseDamage * (skill.DamageMultiplier / 100.0));
            }
            
            if (baseDamage > 0)
            {
                var hits = Math.Max(1, skill.MultiHit ? skill.HitCount : 1);
                var totalDamage = 0;
                
                for (var i = 0; i < hits; i++)
                {
                    var damage = baseDamage;
                    var multiplier = enemy.GetDamageMultiplier(skill.DamageType);
                    damage = (int)(damage * multiplier);
                    
                    if (!skill.IgnoresDefense)
                    {
                        var defenseValue = skill.Category == SkillCategory.Magic
                            ? enemy.MagicResistance
                            : enemy.PhysicalDefense;
                        var reduction = defenseValue / (defenseValue + 100.0);
                        damage = (int)(damage * (1.0 - reduction));
                    }
                    
                    totalDamage += Math.Max(1, damage);
                }
                
                enemy.HP -= totalDamage;
                result.SkillDamage = totalDamage;
                result.SkillHits = hits;
                
                TrackDamageDealt(player, totalDamage);
                if (skill.CanStun && _random.Next(100) < skill.StunChance && !enemy.IsImmuneToEffect(StatusEffectType.Stunned))
                {
                    enemy.StatusEffects.Add(new StatusEffect(StatusEffectType.Stunned, 1, 0));
                    result.InflictedEffect = StatusEffectType.Stunned;
                }
            }
            
            if (skill.StatBuffs.Count > 0)
            {
                var buffDuration = Math.Max(1, skill.BuffDuration);
                var defenseBuff = 0;
                var attackBuff = 0;
                
                if (skill.StatBuffs.TryGetValue("Defense", out var def))
                {
                    defenseBuff += def;
                }
                if (skill.StatBuffs.TryGetValue("MagicResistance", out var mr))
                {
                    defenseBuff += mr;
                }
                if (skill.StatBuffs.TryGetValue("Attack", out var atk))
                {
                    attackBuff += atk;
                }
                if (skill.StatBuffs.TryGetValue("MagicPower", out var mp))
                {
                    attackBuff += mp;
                }
                
                if (defenseBuff > 0)
                {
                    player.StatusEffects.Add(new StatusEffect(StatusEffectType.Shielded, buffDuration, defenseBuff));
                }
                if (attackBuff > 0)
                {
                    player.StatusEffects.Add(new StatusEffect(StatusEffectType.Empowered, buffDuration, attackBuff));
                }
            }
            
            AddCombatLog(player, $"✨ {skill.Name}", result.SkillDamage > 0 ? $"💥 {result.SkillDamage} daño" : "✅ Ejecutada");
            
            ProcessEnemyTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // FASE 5A: SKILLS DE INVOCACIÓN
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Maneja las skills de invocación de minions
        /// </summary>
        private CombatResult HandleSummoningSkill(RpgPlayer player, RpgEnemy enemy, RpgSkill skill, CombatResult result)
        {
            // Verificar recursos
            if (player.Mana < skill.ManaCost || player.Stamina < skill.StaminaCost)
            {
                result.SkillFailureReason = "Recursos insuficientes";
                AddCombatLog(player, skill.Name, "❌ Recursos insuficientes");
                ProcessEnemyTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            // Detectar tipo de minion desde el skillId
            var minionType = skill.Id switch
            {
                "summon_skeleton" => MinionType.Skeleton,
                "summon_zombie" => MinionType.Zombie,
                "summon_ghost" => MinionType.Ghost,
                "summon_lich" => MinionType.Lich,
                "summon_elemental" => DetermineElementalType(player),
                "summon_horror" => MinionType.VoidHorror,
                "army_of_dead" => MinionType.Skeleton, // Invoca múltiples
                _ => MinionType.Skeleton
            };
            
            // Army of Dead invoca 5 skeletons
            if (skill.Id == "army_of_dead")
            {
                player.Mana -= skill.ManaCost;
                player.Stamina -= skill.StaminaCost;
                if (skill.ManaCost > 0)
                {
                    TrackAction(player, "mana_spent", skill.ManaCost);
                }
                
                if (skill.Cooldown > 0)
                {
                    player.SkillCooldowns[skill.Id] = skill.Cooldown;
                }
                
                // Calcular probabilidad base una vez
                var (armyProb, armyProbDetails) = CalculateSummonProbability(player, MinionType.Skeleton, enemy);
                
                var summoned = 0;
                var failed = 0;
                var summonDetails = $"🎖️ **Ejército de los Muertos**\n{armyProbDetails}\n\n";
                
                for (int i = 0; i < 5; i++)
                {
                    if (player.ActiveMinions.Count >= player.MaxActiveMinions)
                    {
                        summonDetails += $"⚠️ Límite de esbirros alcanzado ({player.MaxActiveMinions})\n";
                        break;
                    }
                    
                    var armyRoll = Random.Shared.NextDouble() * 100.0;
                    
                    if (armyRoll <= armyProb)
                    {
                        var message = SummonMinion(player, MinionType.Skeleton);
                        if (!message.Contains("❌"))
                        {
                            summoned++;
                            summonDetails += $"✅ Esqueleto #{summoned}: Roll {armyRoll:F1}% ≤ {armyProb:F1}%\n";
                        }
                    }
                    else
                    {
                        failed++;
                        summonDetails += $"❌ Invocación #{i+1} falló: Roll {armyRoll:F1}% > {armyProb:F1}%\n";
                    }
                }
                
                summonDetails += $"\n📊 **Resultado:** {summoned} exitosos, {failed} fallidos";
                
                result.SkillUsed = true;
                result.SkillName = skill.Name;
                result.SkillDetails = summonDetails;
                TrackSkillUsed(player, skill.Id);
                AddCombatLog(player, $"✨ {skill.Name}", $"💀 ¡Invocaste {summoned} esqueletos!");
                
                ProcessEnemyTurn(player, enemy, result);
                ExecuteMinionsTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            // Summon Horror cuesta HP adicional (40%)
            if (skill.Id == "summon_horror")
            {
                var hpCost = (int)(player.MaxHP * 0.4);
                if (player.HP <= hpCost)
                {
                    result.SkillFailureReason = "HP insuficiente (necesitas >40% HP)";
                    AddCombatLog(player, skill.Name, "❌ HP insuficiente");
                    ProcessEnemyTurn(player, enemy, result);
                    CheckCombatEnd(player, enemy, result);
                    return result;
                }
                
                player.HP -= hpCost;
                AddCombatLog(player, skill.Name, $"💔 Sacrificaste {hpCost} HP");
            }
            
            // Invocar el minion
            player.Mana -= skill.ManaCost;
            player.Stamina -= skill.StaminaCost;
            if (skill.ManaCost > 0)
            {
                TrackAction(player, "mana_spent", skill.ManaCost);
            }
            
            if (skill.Cooldown > 0)
            {
                player.SkillCooldowns[skill.Id] = skill.Cooldown;
            }
            
            // Calcular probabilidad de invocación
            var (probability, probDetails) = CalculateSummonProbability(player, minionType, enemy);
            var roll = Random.Shared.NextDouble() * 100.0;
            
            // Si falla la invocación
            if (roll > probability)
            {
                var failMessage = $"❌ **Invocación Fallida**\n" +
                                 $"{probDetails}\n" +
                                 $"🎲 Roll: {roll:F1}% (necesitabas ≤{probability:F1}%)\n" +
                                 $"💫 La energía se disipa sin éxito\n" +
                                 $"⚠️ Recursos consumidos: -{skill.ManaCost} Mana, -{skill.StaminaCost} Stamina";
                
                result.SkillUsed = true;
                result.SkillName = skill.Name;
                result.SkillDetails = failMessage;
                result.SkillFailureReason = "Invocación fallida (roll)";
                
                AddCombatLog(player, $"✨ {skill.Name}", failMessage);
                
                // El jugador pierde el turno, pero el enemigo NO ataca (fallo de invocación)
                ExecuteMinionsTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                
                return result;
            }
            
            // Invocación exitosa
            var summonMessage = SummonMinion(player, minionType);
            
            // Agregar información de probabilidad al mensaje de éxito
            summonMessage = $"✅ **Invocación Exitosa**\n" +
                           $"{probDetails}\n" +
                           $"🎲 Roll: {roll:F1}% ≤ {probability:F1}%\n" +
                           $"\n{summonMessage}";
            
            result.SkillUsed = true;
            result.SkillName = skill.Name;
            result.SkillDetails = summonMessage; // Mostrar detalles de la invocación
            TrackSkillUsed(player, skill.Id);
            AddCombatLog(player, $"✨ {skill.Name}", summonMessage);
            
            ProcessEnemyTurn(player, enemy, result);
            ExecuteMinionsTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        /// <summary>
        /// Determina qué tipo de elemental invocar según la clase del jugador
        /// </summary>
        private MinionType DetermineElementalType(RpgPlayer player)
        {
            // Usar clase activa o clase base (convertir enum a string)
            var className = player.ActiveHiddenClass ?? player.Class.ToString();
            
            return className switch
            {
                "Mago" => MinionType.FireElemental,
                "Guerrero" => MinionType.EarthElemental,
                "Explorador" => MinionType.AirElemental,
                "Clérigo" => MinionType.WaterElemental,
                _ => MinionType.FireElemental // Default
            };
        }
        
        /// <summary>
        /// Calcula la probabilidad de invocar exitosamente un minion
        /// </summary>
        private (double probability, string details) CalculateSummonProbability(RpgPlayer player, MinionType type, RpgEnemy enemy)
        {
            // Tasas base por tipo de minion
            double baseProbability = type switch
            {
                MinionType.Skeleton => 85.0,
                MinionType.Zombie => 75.0,
                MinionType.Ghost => 65.0,
                MinionType.Lich => 50.0,
                MinionType.VoidHorror => 40.0,
                MinionType.FireElemental => 70.0,
                MinionType.WaterElemental => 70.0,
                MinionType.EarthElemental => 70.0,
                MinionType.AirElemental => 70.0,
                MinionType.Aberration => 35.0,
                _ => 60.0
            };
            
            var details = $"📊 Base: {baseProbability:F1}%";
            
            // Modificador de inteligencia (+1% cada 10 INT)
            double intBonus = player.Intelligence / 10.0;
            if (intBonus > 0)
            {
                details += $" | INT: +{intBonus:F1}%";
                baseProbability += intBonus;
            }
            
            // Modificador de sabiduría (+1.5% cada 10 WIS)
            double wisBonus = player.Wisdom / 10.0 * 1.5;
            if (wisBonus > 0)
            {
                details += $" | WIS: +{wisBonus:F1}%";
                baseProbability += wisBonus;
            }
            
            // Bonus de clase oculta Necromancer
            if (player.ActiveHiddenClass == "Nigromante" || player.ActiveHiddenClass == "Señor Nigromante")
            {
                double necroBonus = player.ActiveHiddenClass == "Señor Nigromante" ? 15.0 : 10.0;
                details += $" | {player.ActiveHiddenClass}: +{necroBonus:F1}%";
                baseProbability += necroBonus;
            }
            
            // Penalización por diferencia de nivel (si el enemigo es mucho más fuerte)
            var levelDiff = enemy.Level - player.Level;
            if (levelDiff > 0)
            {
                double levelPenalty = levelDiff * 2.0;
                details += $" | Nivel: -{levelPenalty:F1}%";
                baseProbability -= levelPenalty;
            }
            
            // Límites: mínimo 10%, máximo 95%
            baseProbability = Math.Max(10.0, Math.Min(95.0, baseProbability));
            
            return (baseProbability, details);
        }
        
        /// <summary>
        /// Maneja la skill de sacrificio de minion
        /// </summary>
        private CombatResult HandleSacrificeSkill(RpgPlayer player, RpgEnemy enemy, RpgSkill skill, CombatResult result)
        {
            if (player.ActiveMinions.Count == 0)
            {
                result.SkillFailureReason = "No tienes esbirros activos";
                AddCombatLog(player, skill.Name, "❌ No hay esbirros");
                ProcessEnemyTurn(player, enemy, result);
                CheckCombatEnd(player, enemy, result);
                return result;
            }
            
            // Sacrificar el primer minion (el más antiguo)
            var message = SacrificeMinion(player, 0);
            
            if (skill.Cooldown > 0)
            {
                player.SkillCooldowns[skill.Id] = skill.Cooldown;
            }
            
            result.SkillUsed = true;
            result.SkillName = skill.Name;
            TrackSkillUsed(player, skill.Id);
            
            ProcessEnemyTurn(player, enemy, result);
            ExecuteMinionsTurn(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════════════════════════════

        private void StartPlayerTurn(RpgPlayer player)
        {
            player.CombatTurnCount++;
            ReduceSkillCooldowns(player);
        }
        
        private void ReduceSkillCooldowns(RpgPlayer player)
        {
            if (player.SkillCooldowns == null || player.SkillCooldowns.Count == 0)
            {
                return;
            }
            
            var keys = player.SkillCooldowns.Keys.ToList();
            foreach (var key in keys)
            {
                if (player.SkillCooldowns[key] > 0)
                {
                    player.SkillCooldowns[key]--;
                }
                
                if (player.SkillCooldowns[key] <= 0)
                {
                    player.SkillCooldowns.Remove(key);
                }
            }
        }
        
        private void ProcessEnemyTurn(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            // Procesar efectos de estado
            ProcessStatusEffects(player, enemy, result);
            
            // Enemigo ataca si no está aturdido
            var stunnedEffect = enemy.StatusEffects.FirstOrDefault(e => e.Type == StatusEffectType.Stunned);
            if (stunnedEffect == null && !result.EnemyDefeated)
            {
                PerformEnemyAttack(player, enemy, result, 0);
            }
            else if (stunnedEffect != null)
            {
                AddCombatLog(player, $"{enemy.Name}", "💫 Aturdido - No ataca");
            }
        }
        
        private void CheckCombatEnd(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            if (enemy.HP <= 0)
            {
                ApplyVictoryRewards(player, enemy, result);
            }
            
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
                StateManager.ForceState(player, GameState.Idle, "defeated"); // FSM
                
                AddCombatLog(player, "Derrota", "💀 Has sido derrotado");
            }
        }

        private void ApplyVictoryRewards(RpgPlayer player, RpgEnemy enemy, CombatResult result)
        {
            result.EnemyDefeated = true;
            result.XPGained = enemy.XPReward;
            result.GoldGained = enemy.GoldReward;
            
            // Track para skill unlocks
            TrackEnemyDefeated(player);
            TrackCombatSurvived(player); // También trackea low_hp_combat si HP < 30%
            if (player.ComboCount >= 5)
            {
                TrackCombo(player, player.ComboCount);
            }
            
            // Auto-desbloquear skills al terminar combate
            var newSkills = SkillDatabase.CheckAndUnlockSkills(player);
            if (newSkills.Any())
            {
                result.UnlockedSkills = newSkills;
            }
            
            // Loot de equipo (armas/armaduras)
            var loot = EquipmentDatabase.GenerateLoot(player.Level);
            if (loot != null)
            {
                player.EquipmentInventory.Add(loot);
                result.LootDrop = loot;
                AddCombatLog(player, "Loot", $"💎 {loot.Name} {loot.RarityEmoji}");
            }
            
            // Loot de consumibles/materiales (25% chance)
            var consumableDrop = InventoryService.GenerateConsumableDrop(player.Level);
            if (consumableDrop != null && player.Inventory.Count < 40)
            {
                player.Inventory.Add(consumableDrop);
                result.ConsumableDrop = consumableDrop;
                AddCombatLog(player, "ConsumableDrop", $"{consumableDrop.Emoji} {consumableDrop.Name}");
            }
            
            player.Gold += result.GoldGained;
            player.TotalKills++;
            player.TotalGoldEarned += result.GoldGained;
            _rpgService.AddXP(player, result.XPGained);
            
            // ═══ FASE 4: Tracking de kills para desbloqueo de clases ═══
            TrackEnemyDefeated(player);
            if (enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss)
            {
                if (!player.ActionCounters.ContainsKey("boss_kill"))
                    player.ActionCounters["boss_kill"] = 0;
                player.ActionCounters["boss_kill"]++;
            }
            
            // ═══ FASE 3.5: XP para Mascotas ═══
            if (player.ActivePets != null && player.ActivePets.Any())
            {
                foreach (var pet in player.ActivePets)
                {
                    pet.CombatsParticipated++;
                    
                    // 50 XP base por participar en el combate
                    int petXP = 50;
                    
                    // Bonus de 100 XP si la pet dio el golpe final (verificar si su último ataque fue crítico o daño alto)
                    if (player.ComboCount >= 3 || (result.PetTurns != null && result.PetTurns.Any(pt => pt.Critical)))
                    {
                        petXP += 100; // Kill bonus
                        pet.TotalKills++;
                    }
                    
                    // Boss bonus
                    if (enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss)
                    {
                        petXP += 500;
                        pet.BossKills++;
                    }
                    
                    // Dar XP y verificar levelup
                    bool leveledUp = pet.GainExperience(petXP);
                    
                    if (leveledUp)
                    {
                        var petEmoji = PetDatabase.GetSpeciesData(pet.Species)?.Emoji ?? "🐾";
                        result.PetLevelUps.Add($"{petEmoji} {pet.Name} alcanzó **nivel {pet.Level}**!");
                    }
                }
            }
            
            // ═══ FASE 3.5: XP para Minions ═══
            if (player.ActiveMinions != null && player.ActiveMinions.Any())
            {
                foreach (var minion in player.ActiveMinions)
                {
                    minion.CombatsSurvived++;
                    
                    // 30 XP base por sobrevivir al combate
                    int minionXP = 30;
                    
                    // Bonus de 150 XP si el minion participó activamente
                    if (result.TotalPetDamage > 0) // Si hubo daño de compañeros, asumimos que minions ayudaron
                    {
                        minionXP += 150;
                        minion.Kills++;
                    }
                    
                    // Boss bonus
                    if (enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss)
                    {
                        minionXP += 300;
                    }
                    
                    // Dar XP y verificar levelup
                    bool leveledUp = minion.GainExperience(minionXP);
                    
                    if (leveledUp)
                    {
                        result.MinionLevelUps.Add($"{minion.Emoji} {minion.Name} alcanzó **nivel {minion.Level}**!");
                    }
                }
            }
            
            // ═══ FASE 3: Sistema de Mazmorras ═══
            // Si está en mazmorra, marcar piso actual como completado
            if (player.CurrentDungeon != null && player.CurrentDungeon.IsActive)
            {
                var currentFloor = player.CurrentDungeon.Floors[player.CurrentDungeon.CurrentFloor - 1];
                
                if (!currentFloor.IsCleared)
                {
                    currentFloor.IsCleared = true;
                    player.CurrentDungeon.TotalEnemiesDefeated++;
                    
                    if (currentFloor.Type == FloorType.Boss)
                    {
                        player.CurrentDungeon.TotalBossesDefeated++;
                    }
                    
                    player.TotalDungeonFloorsCleaned++;
                    
                    // Aplicar recompensas del piso
                    if (currentFloor.Reward != null)
                    {
                        player.Gold += currentFloor.Reward.Gold;
                        _rpgService.AddXP(player, currentFloor.Reward.XP);
                        
                        result.GoldGained += currentFloor.Reward.Gold;
                        result.XPGained += currentFloor.Reward.XP;
                    }
                }
            }
            
            // ═══ FASE 3: Key Drops de Bosses ═══
            // Los bosses tienen probabilidad de dropear llaves de mazmorra
            if (enemy.Difficulty == EnemyDifficulty.Boss || enemy.Difficulty == EnemyDifficulty.WorldBoss)
            {
                // Determinar qué llave puede dropear según nivel del jugador
                DungeonDifficulty? keyDifficulty = null;
                
                if (player.Level >= 25) keyDifficulty = DungeonDifficulty.Legendary;
                else if (player.Level >= 20) keyDifficulty = DungeonDifficulty.Epic;
                else if (player.Level >= 15) keyDifficulty = DungeonDifficulty.Rare;
                else if (player.Level >= 10) keyDifficulty = DungeonDifficulty.Uncommon;
                else if (player.Level >= 5) keyDifficulty = DungeonDifficulty.Common;
                
                if (keyDifficulty.HasValue)
                {
                    var dropChance = DungeonKey.GetDropChance(keyDifficulty.Value);
                    var roll = new Random().NextDouble();
                    
                    if (roll < dropChance)
                    {
                        var key = new DungeonKey
                        {
                            Id = $"key_{keyDifficulty.Value.ToString().ToLower()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                            Name = $"Llave de Mazmorra {DungeonDatabase.GetDifficultyName(keyDifficulty.Value)}",
                            Emoji = DungeonKey.GetEmojiForDifficulty(keyDifficulty.Value),
                            UnlocksDifficulty = keyDifficulty.Value,
                            IsConsumed = false,
                            ObtainedAt = DateTime.UtcNow
                        };
                        
                        player.DungeonKeys.Add(key);
                        AddCombatLog(player, "KeyDrop", $"{key.Emoji} ¡Llave de Mazmorra obtenida!");
                        Console.WriteLine($"[Combat] {key.Emoji} Key drop: {key.Name}");
                    }
                }
            }
            
            player.IsInCombat = false;
            player.CurrentEnemy = null;
            player.ComboCount = 0;
            player.CombatTurnCount = 0;
            player.StatusEffects.Clear();
            StateManager.ForceState(player, GameState.Idle, "victory"); // FSM

            // ═══ FASE 9: Tracking de objetivos de misión ═══
            // Kill objectives
            var questKillNotifs = QuestService.UpdateKillObjective(player, enemy.Name, enemy.Level);
            result.QuestNotifications.AddRange(questKillNotifs);

            // Collect objectives (items que acaban de dropear)
            var questCollectNotifs = QuestService.UpdateCollectObjective(player);
            result.QuestNotifications.AddRange(questCollectNotifs);

            AddCombatLog(player, "Victoria", $"✅ ¡{enemy.Name} derrotado!");
            Console.WriteLine($"[Combat] ✅ ¡{enemy.Name} derrotado! +{result.XPGained} XP, +{result.GoldGained} oro");
        }
        
        // ═══════════════════════════════════════════════════════════════
        // SISTEMA DE TRACKING DE ACCIONES
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Trackea una acción del jugador para desbloqueo de skills
        /// </summary>
        private void TrackAction(RpgPlayer player, string actionType, int count = 1)
        {
            if (!player.ActionCounters.ContainsKey(actionType))
            {
                player.ActionCounters[actionType] = 0;
            }
            
            player.ActionCounters[actionType] += count;
            
            // ═══ FASE 4: Mapeo a contadores de clase ═══
            // Ataques físicos: charge, precise, heavy, counter, advance
            if (actionType is "charge_attack" or "precise_attack" or "heavy_attack" or "counter_attack" or "counter")
            {
                if (!player.ActionCounters.ContainsKey("physical_attack"))
                    player.ActionCounters["physical_attack"] = 0;
                player.ActionCounters["physical_attack"] += count;
            }
        }
        
        /// <summary>
        /// Trackea daño infligido
        /// </summary>
        private void TrackDamageDealt(RpgPlayer player, int damage)
        {
            TrackAction(player, "damage_dealt", damage);
        }
        
        /// <summary>
        /// Trackea daño recibido
        /// </summary>
        private void TrackDamageTaken(RpgPlayer player, int damage)
        {
            TrackAction(player, "damage_taken", damage);
        }
        
        /// <summary>
        /// Trackea golpe crítico
        /// </summary>
        private void TrackCriticalHit(RpgPlayer player)
        {
            TrackAction(player, "critical_hit");
        }
        
        /// <summary>
        /// Trackea esquiva perfecta (no recibir daño)
        /// </summary>
        private void TrackPerfectDodge(RpgPlayer player)
        {
            TrackAction(player, "perfect_dodge");
            // Fase 4: también cuenta para evasion_success
            if (!player.ActionCounters.ContainsKey("evasion_success"))
                player.ActionCounters["evasion_success"] = 0;
            player.ActionCounters["evasion_success"]++;
        }
        
        /// <summary>
        /// Trackea combate sobrevivido
        /// </summary>
        private void TrackCombatSurvived(RpgPlayer player)
        {
            TrackAction(player, "combat_survived");
            
            // Trackear si sobrevivió con HP baja
            if (player.HP < player.MaxHP * 0.3)
            {
                TrackAction(player, "low_hp_combat");
            }
        }
        
        /// <summary>
        /// Trackea enemigo derrotado
        /// </summary>
        private void TrackEnemyDefeated(RpgPlayer player)
        {
            TrackAction(player, "enemy_defeated");
            // Fase 4: trackear enemy_kill y boss_kill
            if (!player.ActionCounters.ContainsKey("enemy_kill"))
                player.ActionCounters["enemy_kill"] = 0;
            player.ActionCounters["enemy_kill"]++;
        }
        
        /// <summary>
        /// Trackea combo largo
        /// </summary>
        private void TrackCombo(RpgPlayer player, int comboCount)
        {
            if (comboCount >= 5)
            {
                TrackAction(player, "combo_5plus");
            }
            if (comboCount >= 10)
            {
                TrackAction(player, "combo_10plus");
            }
        }
        
        /// <summary>
        /// Trackea uso de habilidad
        /// </summary>
        private void TrackSkillUsed(RpgPlayer player, string skillId)
        {
            TrackAction(player, "skill_used");
            TrackAction(player, $"skill_{skillId}");
            
            // Tracking especial para invocaciones de no-muertos (req. Necromancer Lord)
            if (skillId == "summon_skeleton" || skillId == "summon_zombie" || 
                skillId == "summon_ghost" || skillId == "summon_lich" || 
                skillId == "army_of_dead" || skillId == "raise_undead")
            {
                TrackAction(player, "summon_undead");
            }
        }
    }
}
