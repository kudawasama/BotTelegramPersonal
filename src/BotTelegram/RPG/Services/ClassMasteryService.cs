using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Sistema de Maestría de Clase.
    /// Cada clase gana XP de maestría al usarla en combate.
    /// Niveles de maestría (1-10) otorgan bonos PERMANENTES que persisten al cambiar de clase.
    /// </summary>
    public static class ClassMasteryService
    {
        // ═══════════════════════════════════════
        // CONSTANTES
        // ═══════════════════════════════════════
        public const int MaxMasteryLevel = 10;
        
        /// <summary>
        /// XP acumulada necesaria para alcanzar cada nivel de maestría.
        /// Nivel 1 = 0 XP (se obtiene al desbloquear la clase).
        /// </summary>
        private static readonly int[] MasteryXPThresholds = new[]
        {
            0,      // Nivel 1 (base)
            200,    // Nivel 2
            500,    // Nivel 3
            1000,   // Nivel 4
            1800,   // Nivel 5
            3000,   // Nivel 6
            4500,   // Nivel 7
            6500,   // Nivel 8
            9000,   // Nivel 9
            12000   // Nivel 10 (máximo)
        };
        
        // ═══════════════════════════════════════
        // CÁLCULO DE NIVEL
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Obtiene el nivel de maestría actual de una clase (1-10)
        /// </summary>
        public static int GetMasteryLevel(RpgPlayer player, string classId)
        {
            if (!player.ClassMasteryXP.TryGetValue(classId, out var xp))
                return 0; // No tiene maestría en esta clase
            
            for (int i = MasteryXPThresholds.Length - 1; i >= 0; i--)
            {
                if (xp >= MasteryXPThresholds[i])
                    return i + 1;
            }
            
            return 1;
        }
        
        /// <summary>
        /// Obtiene el XP necesario para el siguiente nivel de maestría
        /// </summary>
        public static int GetXPForNextLevel(int currentLevel)
        {
            if (currentLevel >= MaxMasteryLevel) return 0;
            return MasteryXPThresholds[currentLevel]; // [currentLevel] porque arrays son 0-indexed
        }
        
        /// <summary>
        /// Obtiene el XP del nivel actual (umbral)
        /// </summary>
        public static int GetXPForLevel(int level)
        {
            if (level <= 0 || level > MaxMasteryLevel) return 0;
            return MasteryXPThresholds[level - 1];
        }
        
        // ═══════════════════════════════════════
        // OTORGAR XP DE MAESTRÍA
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Otorga XP de maestría a la clase activa del jugador.
        /// Retorna true si el jugador subió de nivel de maestría.
        /// </summary>
        public static MasteryXPResult GrantMasteryXP(RpgPlayer player, int xpAmount)
        {
            var classId = player.ActiveClassId;
            if (classId == "adventurer") 
                return new MasteryXPResult(); // Aventurero no tiene maestría
            
            // Solo clases desbloqueadas ganan maestría
            if (!player.UnlockedClasses.Contains(classId))
                return new MasteryXPResult();
            
            // Inicializar si no existe
            if (!player.ClassMasteryXP.ContainsKey(classId))
                player.ClassMasteryXP[classId] = 0;
            
            int oldLevel = GetMasteryLevel(player, classId);
            
            // Si ya está al máximo, no dar más XP
            if (oldLevel >= MaxMasteryLevel)
                return new MasteryXPResult { ClassId = classId, XPGained = 0, CurrentLevel = MaxMasteryLevel };
            
            player.ClassMasteryXP[classId] += xpAmount;
            
            int newLevel = GetMasteryLevel(player, classId);
            bool leveledUp = newLevel > oldLevel;
            
            // Si subió de nivel, recalcular bonos permanentes
            if (leveledUp)
            {
                RecalculateMasteryBonuses(player);
                Console.WriteLine($"[Mastery] 🏅 {player.Name} alcanzó Maestría {newLevel} en {classId}!");
            }
            
            return new MasteryXPResult
            {
                ClassId = classId,
                XPGained = xpAmount,
                OldLevel = oldLevel,
                CurrentLevel = newLevel,
                LeveledUp = leveledUp,
                TotalXP = player.ClassMasteryXP[classId]
            };
        }
        
        // ═══════════════════════════════════════
        // CÁLCULO DE XP POR COMBATE
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Calcula cuánta XP de maestría otorga una victoria
        /// </summary>
        public static int CalculateCombatMasteryXP(int enemyLevel, EnemyDifficulty difficulty)
        {
            int baseXP = 10 + (enemyLevel * 5);
            
            double difficultyMultiplier = difficulty switch
            {
                EnemyDifficulty.Common => 1.0,
                EnemyDifficulty.Uncommon => 1.3,
                EnemyDifficulty.Rare => 1.6,
                EnemyDifficulty.Elite => 2.0,
                EnemyDifficulty.Boss => 3.0,
                EnemyDifficulty.WorldBoss => 5.0,
                _ => 1.0
            };
            
            return (int)(baseXP * difficultyMultiplier);
        }
        
        // ═══════════════════════════════════════
        // BONOS PERMANENTES POR CLASE
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Define qué bono otorga cada clase por nivel de maestría.
        /// Estos bonos son PERMANENTES y se acumulan de TODAS las clases maestreadas.
        /// </summary>
        public static MasteryBonus GetBonusPerLevel(string classId) => classId switch
        {
            // Tier 1
            "warrior"    => new MasteryBonus(Str: 1, Con: 1),           // +1 STR +1 CON por nivel
            "mage"       => new MasteryBonus(Int: 2),                    // +2 INT por nivel
            "rogue"      => new MasteryBonus(Dex: 2),                    // +2 DEX por nivel
            "cleric"     => new MasteryBonus(Wis: 2),                    // +2 WIS por nivel
            // Tier 2
            "paladin"    => new MasteryBonus(Con: 2, Wis: 1),           // +2 CON +1 WIS
            "berserker"  => new MasteryBonus(Str: 3),                    // +3 STR
            "ranger"     => new MasteryBonus(Dex: 1, Wis: 1),           // +1 DEX +1 WIS
            "assassin"   => new MasteryBonus(Dex: 2, Str: 1),           // +2 DEX +1 STR
            "warlock"    => new MasteryBonus(Int: 2, Cha: 1),           // +2 INT +1 CHA
            "high_priest" => new MasteryBonus(Wis: 2, Int: 1),          // +2 WIS +1 INT (Sumo Sacerdote)
            // Tier 3
            "necromancer" => new MasteryBonus(Int: 3),                   // +3 INT
            "sorcerer"   => new MasteryBonus(Int: 2, Cha: 1),           // +2 INT +1 CHA
            "druid"      => new MasteryBonus(Wis: 2, Con: 1),           // +2 WIS +1 CON
            "bard"       => new MasteryBonus(Cha: 3),                    // +3 CHA
            _ => new MasteryBonus()
        };
        
        /// <summary>
        /// Obtiene la descripción del bono por nivel para mostrar en la UI
        /// </summary>
        public static string GetBonusDescription(string classId)
        {
            var bonus = GetBonusPerLevel(classId);
            return bonus.ToDisplayString();
        }
        
        /// <summary>
        /// Obtiene los bonos TOTALES acumulados de una clase a su nivel actual de maestría
        /// </summary>
        public static MasteryBonus GetTotalBonusForClass(RpgPlayer player, string classId)
        {
            int level = GetMasteryLevel(player, classId);
            if (level <= 0) return new MasteryBonus();
            
            var perLevel = GetBonusPerLevel(classId);
            return new MasteryBonus(
                Str: perLevel.Str * level,
                Int: perLevel.Int * level,
                Dex: perLevel.Dex * level,
                Con: perLevel.Con * level,
                Wis: perLevel.Wis * level,
                Cha: perLevel.Cha * level
            );
        }
        
        // ═══════════════════════════════════════
        // RECÁLCULO DE BONOS ACUMULADOS
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Recalcula los bonos permanentes de maestría sumando TODAS las clases maestreadas.
        /// Llamar cada vez que suba un nivel de maestría.
        /// </summary>
        public static void RecalculateMasteryBonuses(RpgPlayer player)
        {
            int totalStr = 0, totalInt = 0, totalDex = 0;
            int totalCon = 0, totalWis = 0, totalCha = 0;
            
            foreach (var (classId, _) in player.ClassMasteryXP)
            {
                var bonus = GetTotalBonusForClass(player, classId);
                totalStr += bonus.Str;
                totalInt += bonus.Int;
                totalDex += bonus.Dex;
                totalCon += bonus.Con;
                totalWis += bonus.Wis;
                totalCha += bonus.Cha;
            }
            
            player.MasteryBonusStr = totalStr;
            player.MasteryBonusInt = totalInt;
            player.MasteryBonusDex = totalDex;
            player.MasteryBonusCon = totalCon;
            player.MasteryBonusWis = totalWis;
            player.MasteryBonusCha = totalCha;
        }
        
        // ═══════════════════════════════════════
        // HELPERS DE UI
        // ═══════════════════════════════════════
        
        /// <summary>
        /// Obtiene emoji de estrellas según nivel de maestría
        /// </summary>
        public static string GetMasteryStars(int level)
        {
            if (level <= 0) return "";
            if (level >= 10) return "🏅"; // Máximo
            return level switch
            {
                >= 7 => "⭐⭐⭐",
                >= 4 => "⭐⭐",
                >= 1 => "⭐",
                _ => ""
            };
        }
        
        /// <summary>
        /// Barra de progreso mini para maestría
        /// </summary>
        public static string GetProgressBar(RpgPlayer player, string classId)
        {
            int level = GetMasteryLevel(player, classId);
            if (level >= MaxMasteryLevel) return "[██████████] MAX";
            
            int currentXP = player.ClassMasteryXP.TryGetValue(classId, out var xp) ? xp : 0;
            int currentLevelXP = GetXPForLevel(level);
            int nextLevelXP = GetXPForNextLevel(level);
            
            if (nextLevelXP <= currentLevelXP) return "[██████████] MAX";
            
            double progress = (double)(currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP);
            int filled = (int)(progress * 10);
            return "[" + new string('█', filled) + new string('░', 10 - filled) + "]";
        }
    }
    
    // ═══════════════════════════════════════
    // RECORDS Y MODELOS
    // ═══════════════════════════════════════
    
    /// <summary>
    /// Bono de stats por nivel de maestría
    /// </summary>
    public record MasteryBonus(
        int Str = 0, int Int = 0, int Dex = 0,
        int Con = 0, int Wis = 0, int Cha = 0
    )
    {
        public string ToDisplayString()
        {
            var parts = new List<string>();
            if (Str != 0) parts.Add($"+{Str}💪STR");
            if (Int != 0) parts.Add($"+{Int}🔮INT");
            if (Dex != 0) parts.Add($"+{Dex}🏃DEX");
            if (Con != 0) parts.Add($"+{Con}🛡️CON");
            if (Wis != 0) parts.Add($"+{Wis}🌟WIS");
            if (Cha != 0) parts.Add($"+{Cha}🎭CHA");
            return parts.Count > 0 ? string.Join(" ", parts) : "Sin bonos";
        }
        
        public bool HasAnyBonus => Str != 0 || Int != 0 || Dex != 0 || Con != 0 || Wis != 0 || Cha != 0;
    }
    
    /// <summary>
    /// Resultado de otorgar XP de maestría
    /// </summary>
    public class MasteryXPResult
    {
        public string ClassId { get; set; } = "";
        public int XPGained { get; set; }
        public int OldLevel { get; set; }
        public int CurrentLevel { get; set; }
        public bool LeveledUp { get; set; }
        public int TotalXP { get; set; }
    }
}
