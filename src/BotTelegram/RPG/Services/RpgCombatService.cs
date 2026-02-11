using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public class RpgCombatService
    {
        private readonly RpgService _rpgService;
        private static readonly Random _random = new();
        
        public RpgCombatService()
        {
            _rpgService = new RpgService();
        }
        
        public CombatResult PlayerAttack(RpgPlayer player, RpgEnemy enemy)
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
            
            // Roll player attack
            var attackRoll = RpgService.RollDice(20);
            var hitChance = 10 + enemy.Defense;
            
            result.PlayerRoll = attackRoll;
            result.PlayerHit = attackRoll >= hitChance;
            
            if (result.PlayerHit)
            {
                // Incrementar combo
                player.ComboCount++;
                
                // Calculate damage
                var baseDamage = player.TotalAttack;
                
                // Bonus de combo (5% por ataque consecutivo, máx 25%)
                if (player.ComboCount > 1)
                {
                    var comboBonus = Math.Min(player.ComboCount - 1, 5) * 0.05;
                    baseDamage = (int)(baseDamage * (1 + comboBonus));
                    result.ComboBonus = (int)(baseDamage * comboBonus);
                }
                
                // Bonus de empoderamiento
                var empowerEffect = player.StatusEffects.FirstOrDefault(e => e.Type == StatusEffectType.Empowered);
                if (empowerEffect != null)
                {
                    baseDamage += empowerEffect.Intensity;
                }
                
                var damageRoll = _random.Next((int)(baseDamage * 0.8), (int)(baseDamage * 1.2));
                
                // Critical hit (natural 20)
                if (attackRoll == 20)
                {
                    damageRoll = (int)(damageRoll * 1.5);
                    result.PlayerCritical = true;
                    
                    // Combo de 3+ con crítico = Sangrado
                    if (player.ComboCount >= 3)
                    {
                        enemy.StatusEffects.Add(new StatusEffect(StatusEffectType.Bleeding, 3, 5));
                        result.InflictedEffect = StatusEffectType.Bleeding;
                    }
                }
                
                result.PlayerDamage = Math.Max(1, damageRoll - enemy.Defense / 2);
                enemy.HP -= result.PlayerDamage;
                
                AddCombatLog(player, $"Atacar (Combo x{player.ComboCount})", 
                    $"⚔️ {result.PlayerDamage} daño" + (result.PlayerCritical ? " [CRÍTICO]" : ""));
                
                Console.WriteLine($"[Combat] 🗡️ {player.Name} ataca: dado={attackRoll}, daño={result.PlayerDamage}, combo={player.ComboCount}");
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
                
                Console.WriteLine($"[Combat] ❌ {player.Name} falla: dado={attackRoll} < {hitChance}");
            }
            
            // Check if enemy is dead
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
            
            var fleeRoll = RpgService.RollDice(20);
            var fleeDifficulty = 10 + (enemy.Level - player.Level) * 2;
            
            var success = fleeRoll + player.Dexterity >= fleeDifficulty;
            
            if (success)
            {
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                player.ComboCount = 0;
                player.CombatTurnCount = 0;
                player.StatusEffects.Clear();
                
                AddCombatLog(player, "Huir", $"✅ Escapaste (dado: {fleeRoll})");
                Console.WriteLine($"[Combat] 🏃 {player.Name} huyó exitosamente (dado: {fleeRoll})");
            }
            else
            {
                // Enemy gets a free attack
                var damage = Math.Max(1, enemy.Attack - player.TotalDefense);
                player.HP -= damage;
                
                AddCombatLog(player, "Huir", $"❌ Fallo (dado: {fleeRoll}) - Recibiste {damage} daño");
                Console.WriteLine($"[Combat] ❌ Fallo al huir (dado: {fleeRoll}). Recibe {damage} daño");
                
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
            var enemyAttackRoll = RpgService.RollDice(20);
            var enemyHitChance = 10 + player.TotalDefense + defenseBonus;
            
            result.EnemyRoll = enemyAttackRoll;
            result.EnemyHit = enemyAttackRoll >= enemyHitChance;
            
            if (result.EnemyHit)
            {
                var enemyBaseDamage = enemy.Attack;
                var enemyDamageRoll = _random.Next((int)(enemyBaseDamage * 0.8), (int)(enemyBaseDamage * 1.2));
                
                // Enemy critical
                if (enemyAttackRoll == 20)
                {
                    enemyDamageRoll = (int)(enemyDamageRoll * 1.5);
                    result.EnemyCritical = true;
                }
                
                // Reducir daño si está defendiendo
                var damageReduction = defenseBonus > 0 ? 2 : 1;
                result.EnemyDamage = Math.Max(1, (enemyDamageRoll - player.TotalDefense - defenseBonus) / damageReduction);
                player.HP -= result.EnemyDamage;
                
                // Romper combo del jugador si recibe daño
                if (player.ComboCount > 0 && defenseBonus == 0)
                {
                    player.ComboCount = 0;
                    result.ComboBroken = true;
                }
                
                AddCombatLog(player, $"{enemy.Name} ataca", 
                    $"⚔️ {result.EnemyDamage} daño" + (result.EnemyCritical ? " [CRÍTICO]" : ""));
                
                Console.WriteLine($"[Combat] ⚔️ {enemy.Name} contraataca: dado={enemyAttackRoll}, daño={result.EnemyDamage}");
            }
            else
            {
                AddCombatLog(player, $"{enemy.Name} ataca", "🛡️ Esquivado");
                Console.WriteLine($"[Combat] 🛡️ {player.Name} esquiva: dado enemigo={enemyAttackRoll} < {enemyHitChance}");
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
                if (result.PlayerCritical)
                {
                    narrative += $"⚔️ **¡GOLPE CRÍTICO!**\n";
                }
                else
                {
                    narrative += $"⚔️ *Atacas con precisión*\n";
                }
                
                narrative += $"🎲 Dado: {result.PlayerRoll}/20\n";
                narrative += $"💥 Daño: {result.PlayerDamage}";
                
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
                narrative += $"🎲 Dado: {result.PlayerRoll}/20 (necesitabas ≥{10 + enemy.Defense})\n";
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
                narrative += $"🎲 Dado: {result.EnemyRoll}/20\n";
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
                narrative += $"🩸 Daño reducido: {result.EnemyDamage}\n\n";
            }
            else if (!result.EnemyHit && result.EnemyRoll > 0)
            {
                narrative += $"🛡️ *¡Esquivas el ataque!*\n";
                narrative += $"🎲 Dado enemigo: {result.EnemyRoll}/20\n\n";
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
        public bool PlayerHit { get; set; }
        public bool PlayerCritical { get; set; }
        public int PlayerDamage { get; set; }
        public int PlayerRoll { get; set; }
        public bool PlayerDefended { get; set; }
        public bool PlayerStunned { get; set; }
        
        public bool EnemyHit { get; set; }
        public bool EnemyCritical { get; set; }
        public int EnemyDamage { get; set; }
        public int EnemyRoll { get; set; }
        
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
    }
}
