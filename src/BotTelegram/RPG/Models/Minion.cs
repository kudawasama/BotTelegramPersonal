using System;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Representa una invocación/minion del jugador (no-muertos, elementales, etc.)
    /// </summary>
    public class Minion
    {
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = "👻";
        public MinionType Type { get; set; }
        
        // Combat Stats (escalan con nivel del jugador)
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        
        // Duración y control
        public int TurnsRemaining { get; set; } // -1 = permanente
        public bool IsTemporary { get; set; }
        public bool IsControlled { get; set; } = true; // false = incontrolable
        
        // Leveling y Persistencia (Fase 3.5)
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int ExperienceNeeded => Level * 100; // Minions suben cada 100xp por nivel
        public int CombatsSurvived { get; set; } = 0;
        public int TotalDamageDealt { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public bool IsPermanent { get; set; } = false; // Si true, se guarda entre combates
        
        // Scaling
        public int SummonerLevel { get; set; } // Nivel del jugador al invocar
        public double StatsMultiplier { get; set; } = 1.0; // Para scaling personalizado
        
        // Habilidades especiales
        public string? SpecialAbility { get; set; }
        public int SpecialCooldown { get; set; }
        
        public Minion() { }
        
        public Minion(string name, MinionType type, int summonerLevel, bool isTemporary = true, int turns = 10)
        {
            Name = name;
            Type = type;
            SummonerLevel = summonerLevel;
            IsTemporary = isTemporary;
            TurnsRemaining = isTemporary ? turns : -1;
            
            ScaleToSummonerLevel(summonerLevel);
        }
        
        /// <summary>
        /// Escala las stats del minion basado en el nivel del jugador
        /// </summary>
        public void ScaleToSummonerLevel(int playerLevel)
        {
            // Base stats (nivel 1)
            int baseHP = Type switch
            {
                MinionType.Skeleton => 30,
                MinionType.Zombie => 50,
                MinionType.Ghost => 25,
                MinionType.Lich => 80,
                MinionType.FireElemental => 40,
                MinionType.WaterElemental => 45,
                MinionType.EarthElemental => 60,
                MinionType.AirElemental => 35,
                MinionType.VoidHorror => 100,
                MinionType.Aberration => 120,
                _ => 30
            };
            
            int baseAtk = Type switch
            {
                MinionType.Skeleton => 15,
                MinionType.Zombie => 12,
                MinionType.Ghost => 20,
                MinionType.Lich => 35,
                MinionType.FireElemental => 25,
                MinionType.WaterElemental => 18,
                MinionType.EarthElemental => 15,
                MinionType.AirElemental => 22,
                MinionType.VoidHorror => 45,
                MinionType.Aberration => 50,
                _ => 15
            };
            
            int baseDef = Type switch
            {
                MinionType.Skeleton => 8,
                MinionType.Zombie => 15,
                MinionType.Ghost => 5,
                MinionType.Lich => 20,
                MinionType.FireElemental => 10,
                MinionType.WaterElemental => 12,
                MinionType.EarthElemental => 25,
                MinionType.AirElemental => 8,
                MinionType.VoidHorror => 18,
                MinionType.Aberration => 22,
                _ => 8
            };
            
            int baseSpd = Type switch
            {
                MinionType.Skeleton => 12,
                MinionType.Zombie => 8,
                MinionType.Ghost => 18,
                MinionType.Lich => 10,
                MinionType.FireElemental => 14,
                MinionType.WaterElemental => 11,
                MinionType.EarthElemental => 7,
                MinionType.AirElemental => 20,
                MinionType.VoidHorror => 9,
                MinionType.Aberration => 6,
                _ => 10
            };
            
            // Scaling: +10% stats por nivel del jugador
            double scaleFactor = 1.0 + (playerLevel - 1) * 0.10 * StatsMultiplier;
            
            MaxHP = (int)(baseHP * scaleFactor);
            HP = MaxHP;
            Attack = (int)(baseAtk * scaleFactor);
            Defense = (int)(baseDef * scaleFactor);
            Speed = (int)(baseSpd * scaleFactor);
        }
        
        /// <summary>
        /// Reduce turnos restantes. Retorna true si el minion expiró.
        /// </summary>
        public bool TickTurn()
        {
            if (TurnsRemaining > 0)
            {
                TurnsRemaining--;
                return TurnsRemaining <= 0;
            }
            return false;
        }
        
        /// <summary>
        /// Aplica daño al minion. Retorna true si murió.
        /// </summary>
        public bool TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
            return HP <= 0;
        }
        
        /// <summary>
        /// Cura al minion
        /// </summary>
        public void Heal(int amount)
        {
            HP += amount;
            if (HP > MaxHP) HP = MaxHP;
        }
        
        /// <summary>
        /// Gana experiencia y sube de nivel si es necesario (Fase 3.5)
        /// </summary>
        /// <returns>True si subió de nivel</returns>
        public bool GainExperience(int xp)
        {
            Experience += xp;
            
            if (Experience >= ExperienceNeeded)
            {
                return LevelUp();
            }
            
            return false;
        }
        
        /// <summary>
        /// Sube un nivel con bonificaciones de stats (Fase 3.5)
        /// </summary>
        public bool LevelUp()
        {
            if (Experience < ExperienceNeeded) return false;
            
            Experience -= ExperienceNeeded;
            Level++;
            
            // Bonificaciones por nivel
            // Cada nivel: +10% HP, +5% ataque, +3% defensa
            MaxHP = (int)(MaxHP * 1.10);
            HP = MaxHP; // Curación completa al subir de nivel
            Attack = (int)(Attack * 1.05);
            Defense = (int)(Defense * 1.03);
            
            // Bonificaciones especiales
            if (Level % 5 == 0)
            {
                // Cada 5 niveles: Habilidad mejorada (futura implementación)
                MaxHP += 30;
                HP = MaxHP;
                Attack += 8;
                Defense += 5;
                Speed += 1;
            }
            
            if (Level % 10 == 0)
            {
                // Cada 10 niveles: Bonus mayor
                MaxHP += 50;
                HP = MaxHP;
                Attack += 15;
                Defense += 10;
                Speed += 3;
            }
            
            if (Level >= 15 && IsTemporary)
            {
                // Al nivel 15+: +50% duración (más turnos)
                if (TurnsRemaining > 0)
                {
                    TurnsRemaining = (int)(TurnsRemaining * 1.5);
                }
            }
            
            return true;
        }
    }
    
    public enum MinionType
    {
        // No-muertos (Necromancer)
        Skeleton,
        Zombie,
        Ghost,
        Lich,
        
        // Elementales (Elemental Overlord)
        FireElemental,
        WaterElemental,
        EarthElemental,
        AirElemental,
        
        // Void/Aberraciones (Void Summoner)
        VoidHorror,
        Aberration
    }
}
