using BotTelegram.Models;

namespace BotTelegram.Services
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
            
            // Roll player attack
            var attackRoll = RpgService.RollDice(20);
            var hitChance = 10 + enemy.Defense;
            
            result.PlayerRoll = attackRoll;
            result.PlayerHit = attackRoll >= hitChance;
            
            if (result.PlayerHit)
            {
                // Calculate damage
                var baseDamage = player.TotalAttack;
                var damageRoll = _random.Next((int)(baseDamage * 0.8), (int)(baseDamage * 1.2));
                
                // Critical hit (natural 20)
                if (attackRoll == 20)
                {
                    damageRoll = (int)(damageRoll * 1.5);
                    result.PlayerCritical = true;
                }
                
                result.PlayerDamage = Math.Max(1, damageRoll - enemy.Defense / 2);
                enemy.HP -= result.PlayerDamage;
                
                Console.WriteLine($"[Combat] 🗡️ {player.Name} ataca: dado={attackRoll}, daño={result.PlayerDamage}");
            }
            else
            {
                Console.WriteLine($"[Combat] ❌ {player.Name} falla: dado={attackRoll} < {hitChance}");
            }
            
            // Check if enemy is dead
            if (enemy.HP <= 0)
            {
                result.EnemyDefeated = true;
                result.XPGained = enemy.XPReward;
                result.GoldGained = enemy.GoldReward;
                
                player.Gold += result.GoldGained;
                _rpgService.AddXP(player, result.XPGained);
                
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                
                Console.WriteLine($"[Combat] ✅ ¡{enemy.Name} derrotado! +{result.XPGained} XP, +{result.GoldGained} oro");
                return result;
            }
            
            // Enemy counterattack
            var enemyAttackRoll = RpgService.RollDice(20);
            var enemyHitChance = 10 + player.TotalDefense;
            
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
                
                result.EnemyDamage = Math.Max(1, enemyDamageRoll - player.TotalDefense);
                player.HP -= result.EnemyDamage;
                
                Console.WriteLine($"[Combat] ⚔️ {enemy.Name} contraataca: dado={enemyAttackRoll}, daño={result.EnemyDamage}");
            }
            else
            {
                Console.WriteLine($"[Combat] 🛡️ {player.Name} esquiva: dado enemigo={enemyAttackRoll} < {enemyHitChance}");
            }
            
            // Check if player died
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                player.HP = 0;
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                
                Console.WriteLine($"[Combat] 💀 {player.Name} fue derrotado...");
            }
            
            return result;
        }
        
        public CombatResult PlayerDefend(RpgPlayer player, RpgEnemy enemy)
        {
            var result = new CombatResult();
            result.PlayerDefended = true;
            
            // Boost defense temporarily
            var oldDefense = player.TotalDefense;
            var defenseBoost = player.Dexterity / 2;
            
            // Enemy attack with boosted defense
            var enemyAttackRoll = RpgService.RollDice(20);
            var enemyHitChance = 10 + oldDefense + defenseBoost;
            
            result.EnemyRoll = enemyAttackRoll;
            result.EnemyHit = enemyAttackRoll >= enemyHitChance;
            
            if (result.EnemyHit)
            {
                var enemyBaseDamage = enemy.Attack;
                var enemyDamageRoll = _random.Next((int)(enemyBaseDamage * 0.8), (int)(enemyBaseDamage * 1.2));
                
                // Reduced damage when defending
                result.EnemyDamage = Math.Max(1, (enemyDamageRoll - oldDefense - defenseBoost) / 2);
                player.HP -= result.EnemyDamage;
                
                Console.WriteLine($"[Combat] 🛡️ Defendiendo: daño reducido a {result.EnemyDamage}");
            }
            else
            {
                Console.WriteLine($"[Combat] 🛡️ ¡Defensa perfecta! Sin daño");
            }
            
            // Check if player died
            if (player.HP <= 0)
            {
                result.PlayerDefeated = true;
                player.HP = 0;
                player.IsInCombat = false;
                player.CurrentEnemy = null;
            }
            
            return result;
        }
        
        public bool TryToFlee(RpgPlayer player, RpgEnemy enemy)
        {
            var fleeRoll = RpgService.RollDice(20);
            var fleeDifficulty = 10 + (enemy.Level - player.Level) * 2;
            
            var success = fleeRoll + player.Dexterity >= fleeDifficulty;
            
            if (success)
            {
                player.IsInCombat = false;
                player.CurrentEnemy = null;
                Console.WriteLine($"[Combat] 🏃 {player.Name} huyó exitosamente (dado: {fleeRoll})");
            }
            else
            {
                // Enemy gets a free attack
                var damage = Math.Max(1, enemy.Attack - player.TotalDefense);
                player.HP -= damage;
                
                Console.WriteLine($"[Combat] ❌ Fallo al huir (dado: {fleeRoll}). Recibe {damage} daño");
                
                if (player.HP <= 0)
                {
                    player.HP = 0;
                    player.IsInCombat = false;
                    player.CurrentEnemy = null;
                }
            }
            
            return success;
        }
        
        public string GetCombatNarrative(CombatResult result, RpgPlayer player, RpgEnemy enemy)
        {
            var narrative = "";
            
            // Player action
            if (result.PlayerDefended)
            {
                narrative += "🛡️ *Adoptas postura defensiva*\n\n";
            }
            else if (result.PlayerHit)
            {
                if (result.PlayerCritical)
                {
                    narrative += $"⚔️ *¡GOLPE CRÍTICO!*\n🎲 Dado: {result.PlayerRoll}/20\n💥 Daño: {result.PlayerDamage}\n\n";
                }
                else
                {
                    narrative += $"⚔️ *Atacas con precisión*\n🎲 Dado: {result.PlayerRoll}/20\n💥 Daño: {result.PlayerDamage}\n\n";
                }
            }
            else
            {
                narrative += $"❌ *Tu ataque falla*\n🎲 Dado: {result.PlayerRoll}/20 (necesitabas ≥{10 + enemy.Defense})\n\n";
            }
            
            // Enemy status
            if (result.EnemyDefeated)
            {
                narrative += $"✅ *¡{enemy.Emoji} {enemy.Name} derrotado!*\n\n";
                narrative += $"🎖️ +{result.XPGained} XP\n💰 +{result.GoldGained} oro\n\n";
                narrative += "🎉 ¡Victoria!";
                return narrative;
            }
            
            // Enemy counterattack
            if (!result.PlayerDefended && result.EnemyHit)
            {
                if (result.EnemyCritical)
                {
                    narrative += $"💀 *¡{enemy.Name} crítico!*\n🎲 Dado: {result.EnemyRoll}/20\n🩸 Daño: {result.EnemyDamage}\n\n";
                }
                else
                {
                    narrative += $"⚔️ *{enemy.Name} contraataca*\n🎲 Dado: {result.EnemyRoll}/20\n🩸 Daño: {result.EnemyDamage}\n\n";
                }
            }
            else if (result.PlayerDefended && result.EnemyHit)
            {
                narrative += $"🛡️ *Bloqueas parcialmente*\n🩸 Daño reducido: {result.EnemyDamage}\n\n";
            }
            else if (!result.EnemyHit)
            {
                narrative += $"🛡️ *¡Esquivas el ataque!*\n🎲 Dado enemigo: {result.EnemyRoll}/20\n\n";
            }
            
            // Combat status
            narrative += "*Estado actual:*\n";
            narrative += $"👤 {player.Name}: ❤️ {player.HP}/{player.MaxHP} HP\n";
            narrative += $"{enemy.Emoji} {enemy.Name}: ❤️ {enemy.HP}/{enemy.MaxHP} HP";
            
            if (result.PlayerDefeated)
            {
                narrative += "\n\n💀 *Has sido derrotado...*";
            }
            
            return narrative;
        }
    }
    
    public class CombatResult
    {
        public bool PlayerHit { get; set; }
        public bool PlayerCritical { get; set; }
        public int PlayerDamage { get; set; }
        public int PlayerRoll { get; set; }
        public bool PlayerDefended { get; set; }
        
        public bool EnemyHit { get; set; }
        public bool EnemyCritical { get; set; }
        public int EnemyDamage { get; set; }
        public int EnemyRoll { get; set; }
        
        public bool EnemyDefeated { get; set; }
        public bool PlayerDefeated { get; set; }
        
        public int XPGained { get; set; }
        public int GoldGained { get; set; }
    }
}
