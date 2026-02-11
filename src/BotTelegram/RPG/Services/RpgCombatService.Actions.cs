using BotTelegram.RPG.Models;

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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
            // Chance de esquivar basado en Evasion
            double dodgeChance = Math.Clamp(player.Evasion * 1.5, 30.0, 80.0);
            double dodgeRoll = _random.Next(0, 10000) / 100.0;
            
            bool dodged = dodgeRoll <= dodgeChance;
            
            if (dodged)
            {
                result.Dodged = true;
                AddCombatLog(player, "🌀 Esquivar", "✅ Exitoso");
                
                // Enemigo ataca y falla
                result.EnemyHit = false;
                AddCombatLog(player, $"{enemy.Name}", "💨 Ataque esquivado");
            }
            else
            {
                result.Dodged = false;
                AddCombatLog(player, "🌀 Esquivar", "❌ Fallo");
                
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
            player.CombatTurnCount++;
            
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
                
                // Contraataque exitoso - daña al enemigo
                int counterDamage = (int)(player.PhysicalAttack * 1.2);
                var damageType = DamageType.Slashing;
                var multiplier = enemy.GetDamageMultiplier(damageType);
                counterDamage = (int)(counterDamage * multiplier);
                
                double damageReduction = enemy.PhysicalDefense / (enemy.PhysicalDefense + 100.0);
                int finalDamage = (int)(counterDamage * (1.0 - damageReduction));
                
                result.PlayerDamage = Math.Max(1, finalDamage);
                enemy.HP -= result.PlayerDamage;
                
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
            player.CombatTurnCount++;
            
            const int staminaCost = 10;
            if (player.Stamina < staminaCost)
            {
                result.PlayerHit = false;
                AddCombatLog(player, "Saltar", "❌ Stamina insuficiente");
                return result;
            }
            
            player.Stamina -= staminaCost;
            
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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
            int manaRestore = (int)(player.MaxMana * 0.25);
            player.Mana = Math.Min(player.MaxMana, player.Mana + manaRestore);
            
            AddCombatLog(player, "🧘 Meditar", $"💙 +{manaRestore} Mana");
            
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
            player.CombatTurnCount++;
            
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
            player.CombatTurnCount++;
            
            int staminaRestore = (int)(player.MaxStamina * 0.10);
            player.Stamina = Math.Min(player.MaxStamina, player.Stamina + staminaRestore);
            
            AddCombatLog(player, "⏸️ Esperar", $"⚡ +{staminaRestore} Stamina");
            
            PerformEnemyAttack(player, enemy, result, 0);
            
            player.ComboCount = 0;
            ProcessStatusEffects(player, enemy, result);
            CheckCombatEnd(player, enemy, result);
            
            return result;
        }
        
        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════════════════════════════
        
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
                result.EnemyDefeated = true;
                result.XPGained = enemy.XPReward;
                result.GoldGained = enemy.GoldReward;
                
                player.Gold += result.GoldGained;
                player.TotalKills++;
                player.TotalGoldEarned += result.GoldGained;
                _rpgService.AddXP(player, result.XPGained);
                
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Victoria", $"✅ ¡{enemy.Name} derrotado!");
                Console.WriteLine($"[Combat] ✅ ¡{enemy.Name} derrotado! +{result.XPGained} XP, +{result.GoldGained} oro");
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
                
                AddCombatLog(player, "Derrota", "💀 Has sido derrotado");
            }
        }
    }
}
