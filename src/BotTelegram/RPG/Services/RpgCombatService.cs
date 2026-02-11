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
            player.CombatTurnCount++;
            
            // Verificar si está aturdido
            if (player.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                result.PlayerStunned = true;
                AddCombatLog(player, $"Atacar", "⚠️ ATURDIDO - No puedes actuar");
                ProcessStatusEffects(player, enemy, result);
                return result;
            }
            
            // ═══════════════════════════════════════
            // SISTEMA DE PROBABILIDADES FIJAS
            // ═══════════════════════════════════════
            
            // 1. CALCULAR HIT CHANCE (probabilidad de golpear)
            double baseHitChance = 85.0; // Base 85%
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
                
                player.Gold += result.GoldGained;
                player.TotalKills++;
                player.TotalGoldEarned += result.GoldGained;
                _rpgService.AddXP(player, result.XPGained);
                
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Victoria", $"✅ {enemy.Name} derrotado");
                
                Console.WriteLine($"[Combat] ✅ ¡{enemy.Name} derrotado! +{result.XPGained} XP, +{result.GoldGained} oro");
                return result;
            }
            
            // Procesar efectos de estado antes del contraataque
            ProcessStatusEffects(player, enemy, result);
            
            // Si el jugador murió por efectos, no hay contraataque
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
        
        public CombatResult PlayerDefend(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            result.PlayerDefended = true;
            player.CombatTurnCount++;
            
            // Romper combo al defender
            if (player.ComboCount > 0)
            {
                player.ComboCount = 0;
                result.ComboBroken = true;
            }
            
            AddCombatLog(player, "Defender", "🛡️ Postura defensiva");
            
            // Verificar si está aturdido
            if (player.StatusEffects.Any(e => e.Type == StatusEffectType.Stunned))
            {
                result.PlayerStunned = true;
                AddCombatLog(player, "Estado", "⚠️ ATURDIDO - No puedes defenderte bien");
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
            if (shieldEffect != null)
            {
                defenseBoost += shieldEffect.Intensity;
            }
            
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
            player.CombatTurnCount++;
            
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
    }
    
    public class CombatResult
    {
        // Información del jugador
        public bool PlayerHit { get; set; }
        public bool PlayerCritical { get; set; }
        public int PlayerDamage { get; set; }
        public bool PlayerDefended { get; set; }
        public bool PlayerStunned { get; set; }
        
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
        
        // Sistema de combos
        public int ComboBonus { get; set; }
        public bool ComboBroken { get; set; }
        
        // Efectos de estado
        public StatusEffectType? InflictedEffect { get; set; }
        public int StatusDamage { get; set; }
        
        // Legacy (compatibilidad)
        [Obsolete("Use HitChancePercent y HitRoll instead")]
        public int PlayerRoll { get; set; }
        
        [Obsolete("Use EnemyHitChancePercent y EnemyHitRoll instead")]
        public int EnemyRoll { get; set; }
    }
    
    public enum AttackType
    {
        Physical,  // Usa defensa física
        Magical    // Usa resistencia mágica
    }
}