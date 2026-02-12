namespace BotTelegram.RPG.Services
{
    using BotTelegram.RPG.Models;
    
    /// <summary>
    /// Base de datos de enemigos con mitología completa
    /// </summary>
    public static class EnemyDatabase
    {
        public static List<RpgEnemy> GetEasyEnemies()
        {
            return new List<RpgEnemy>
            {
                // ═══ LOBO SALVAJE ═══
                new()
                {
                    Name = "Lobo Salvaje",
                    Emoji = "🐺",
                    Description = "Un lobo hambriento que defiende su territorio",
                    Type = EnemyType.Beast,
                    Behavior = EnemyBehavior.Aggressive,
                    HP = 55, MaxHP = 55,            // +120% (era 25) - REBALANCEADO
                    Attack = 28, MagicPower = 0,    // +133% (era 12) - REBALANCEADO
                    PhysicalDefense = 20, MagicResistance = 8,  // +150%/167% - REBALANCEADO
                    Accuracy = 22, Evasion = 18, Speed = 8,     // +47%/50%/60% - REBALANCEADO
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 1.3 } },
                    Resistances = new() { { DamageType.Ice, 0.3 } },
                    XPReward = 15, GoldReward = 12  // -40% rewards para progresión más lenta
                },
                
                // ═══ GOBLIN ═══
                new()
                {
                    Name = "Goblin",
                    Emoji = "👺",
                    Description = "Criatura pequeña pero astuta con armas improvisadas",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Coward,
                    HP = 48, MaxHP = 48,            // +140% (era 20) - REBALANCEADO
                    Attack = 24, MagicPower = 0,    // +140% (era 10) - REBALANCEADO
                    PhysicalDefense = 13, MagicResistance = 12,  // +160%/140% - REBALANCEADO
                    Accuracy = 19, Evasion = 16, Speed = 8,    // +58%/60%/60% - REBALANCEADO
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Fire, 1.5 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    XPReward = 12, GoldReward = 15  // -40% rewards
                },
                
                // ═══ ESQUELETO ═══
                new()
                {
                    Name = "Esqueleto",
                    Emoji = "💀",
                    Description = "Muerto viviente reanimado por magia oscura",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Passive,
                    HP = 44, MaxHP = 44,            // +144% (era 18) - REBALANCEADO
                    Attack = 32, MagicPower = 0,    // +129% (era 14) - REBALANCEADO
                    PhysicalDefense = 8, MagicResistance = 22,  // +167%/120% - REBALANCEADO
                    Accuracy = 17, Evasion = 10, Speed = 6,      // +70%/100%/20% - REBALANCEADO
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() 
                    { 
                        { DamageType.Bludgeoning, 1.5 },
                        { DamageType.Holy, 2.0 }
                    },
                    Resistances = new() 
                    { 
                        { DamageType.Slashing, 0.5 },
                        { DamageType.Piercing, 0.5 }
                    },
                    Immunities = new() { DamageType.Poison },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Poisoned, 
                        StatusEffectType.Bleeding 
                    },
                    XPReward = 14, GoldReward = 10  // -40% rewards
                },
                
                // ═══ SLIME ═══
                new()
                {
                    Name = "Slime",
                    Emoji = "🟢",
                    Description = "Criatura gelatinosa que se mueve lentamente",
                    Type = EnemyType.Aberration,
                    Behavior = EnemyBehavior.Passive,
                    HP = 72, MaxHP = 72,            // +140% (era 30) - REBALANCEADO
                    Attack = 14, MagicPower = 0,     // +133% (era 6) - REBALANCEADO
                    PhysicalDefense = 6, MagicResistance = 6,  // +200%/200% - REBALANCEADO
                    Accuracy = 13, Evasion = 7, Speed = 4,     // +63%/133%/100% - REBALANCEADO
                    PrimaryDamageType = DamageType.Acid,
                    Weaknesses = new() 
                    { 
                        { DamageType.Fire, 2.0 },
                        { DamageType.Ice, 1.5 }
                    },
                    Resistances = new() 
                    { 
                        { DamageType.Physical, 0.4 },
                        { DamageType.Piercing, 0.7 }
                    },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    CanPoison = true,
                    XPReward = 10, GoldReward = 8  // -40% rewards
                }
            };
        }
        
        public static List<RpgEnemy> GetMediumEnemies()
        {
            return new List<RpgEnemy>
            {
                // ═══ ORCO GUERRERO ═══
                new()
                {
                    Name = "Orco Guerrero",
                    Emoji = "👹",
                    Description = "Guerrero brutal con armadura pesada",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Aggressive,
                    HP = 110, MaxHP = 110,            // +100% (era 55) - REBALANCEADO
                    Attack = 36, MagicPower = 0,    // +100% (era 18) - REBALANCEADO
                    PhysicalDefense = 30, MagicResistance = 10,  // +100% - REBALANCEADO
                    Accuracy = 18, Evasion = 12, Speed = 7,      // +50%/100% - REBALANCEADO
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Magical, 1.3 } },
                    Resistances = new() { { DamageType.Physical, 0.2 } },
                    XPReward = 33, GoldReward = 24  // -40% rewards
                },
                
                // ═══ GOLEM DE PIEDRA ═══
                new()
                {
                    Name = "Golem de Piedra",
                    Emoji = "🗿",
                    Description = "Constructo animado de roca sólida, lento pero resistente",
                    Type = EnemyType.Construct,
                    Behavior = EnemyBehavior.Defensive,
                    HP = 160, MaxHP = 160,          // +100% (era 80) - REBALANCEADO
                    Attack = 44, MagicPower = 0,    // +100% (era 22) - REBALANCEADO
                    PhysicalDefense = 70, MagicResistance = 10,  // +100% - REBALANCEADO
                    Accuracy = 11, Evasion = 4, Speed = 4,      // +37%/100%/33% - REBALANCEADO
                    PrimaryDamageType = DamageType.Bludgeoning,
                    Weaknesses = new() 
                    { 
                        { DamageType.Magical, 1.8 },
                        { DamageType.Lightning, 1.5 }
                    },
                    Resistances = new() 
                    { 
                        { DamageType.Physical, 0.5 },
                        { DamageType.Piercing, 0.6 }
                    },
                    Immunities = new() { DamageType.Poison },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Poisoned,
                        StatusEffectType.Bleeding,
                        StatusEffectType.Stunned
                    },
                    XPReward = 42, GoldReward = 21  // -40% rewards
                },
                
                // ═══ ARAÑA GIGANTE ═══
                new()
                {
                    Name = "Araña Gigante",
                    Emoji = "🕷️",
                    Description = "Arácnido colosal con veneno mortal",
                    Type = EnemyType.Beast,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 80, MaxHP = 80,            // +100% (era 40) - REBALANCEADO
                    Attack = 30, MagicPower = 0,    // +100% (era 15) - REBALANCEADO
                    PhysicalDefense = 16, MagicResistance = 16,  // +100% - REBALANCEADO
                    Accuracy = 27, Evasion = 23, Speed = 12,     // +50%/53%/50% - REBALANCEADO
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Resistances = new() { { DamageType.Poison, 0.8 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    CanPoison = true,
                    XPReward = 30, GoldReward = 21  // -40% rewards
                },
                
                // ═══ ELEMENTAL DE FUEGO ═══
                new()
                {
                    Name = "Elemental de Fuego",
                    Emoji = "🔥",
                    Description = "Ser de llamas puras que arde furiosamente",
                    Type = EnemyType.Elemental,
                    Behavior = EnemyBehavior.Berserker,
                    HP = 52, MaxHP = 52,            // +48% (era 35)
                    Attack = 12, MagicPower = 36,   // +50/44% (era 8/25)
                    PhysicalDefense = 8, MagicResistance = 28,  // +60/40% (era 5/20)
                    Accuracy = 20, Evasion = 24, Speed = 11,    // +33/33/22% (era 15/18/9)
                    PrimaryDamageType = DamageType.Fire,
                    Weaknesses = new() 
                    { 
                        { DamageType.Water, 2.5 },
                        { DamageType.Ice, 2.0 }
                    },
                    Immunities = new() { DamageType.Fire },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Burning,
                        StatusEffectType.Poisoned
                    },
                    SpecialAbilities = new() { "Aura de fuego (daño AOE)" },
                    XPReward = 62, GoldReward = 48  // +38/37% recompensas
                },
                
                // ═══ BANDIDO ═══
                new()
                {
                    Name = "Bandido",
                    Emoji = "🏴‍☠️",
                    Description = "Ladrón experimentado con dagas envenenadas",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 55, MaxHP = 55,            // +45% (era 38)
                    Attack = 28, MagicPower = 0,    // +40% (era 20)
                    PhysicalDefense = 15, MagicResistance = 12,  // +50/50% (era 10/8)
                    Accuracy = 28, Evasion = 24, Speed = 10,     // +27/33/25% (era 22/18/8)
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Magical, 1.3 } },
                    CanPoison = true,
                    XPReward = 53, GoldReward = 68  // +39/36% recompensas
                }
            };
        }
        
        public static List<RpgEnemy> GetHardEnemies()
        {
            return new List<RpgEnemy>
            {
                // ═══ TROLL DE HIELO ═══
                new()
                {
                    Name = "Troll de Hielo",
                    Emoji = "🧊",
                    Description = "Gigante de hielo con regeneración constante",
                    Type = EnemyType.Beast,
                    Behavior = EnemyBehavior.Aggressive,
                    HP = 250, MaxHP = 250,          // +150% (era 100) - REBALANCEADO
                    Attack = 65, MagicPower = 38,   // +150% (era 26/15) - REBALANCEADO
                    PhysicalDefense = 50, MagicResistance = 38,  // +150% - REBALANCEADO
                    Accuracy = 18, Evasion = 12, Speed = 7,       // +50%/100%/40% - REBALANCEADO
                    PrimaryDamageType = DamageType.Bludgeoning,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Immunities = new() { DamageType.Ice },
                    Resistances = new() { { DamageType.Physical, 0.3 } },
                    CanRegenerate = true,
                    XPReward = 57, GoldReward = 48  // -40% rewards
                },
                
                // ═══ DEMONIO MENOR ═══
                new()
                {
                    Name = "Demonio Menor",
                    Emoji = "😈",
                    Description = "Ser infernal que domina la magia oscura",
                    Type = EnemyType.Demon,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 188, MaxHP = 188,          // +150% (era 75) - REBALANCEADO
                    Attack = 38, MagicPower = 75,   // +153% (era 15/30) - REBALANCEADO
                    PhysicalDefense = 30, MagicResistance = 63,  // +150%/152% - REBALANCEADO
                    Accuracy = 27, Evasion = 25, Speed = 12,     // +50%/56%/50% - REBALANCEADO
                    PrimaryDamageType = DamageType.Fire,
                    Weaknesses = new() 
                    { 
                        { DamageType.Holy, 2.5 },
                        { DamageType.Water, 1.3 }
                    },
                    Immunities = new() { DamageType.Fire, DamageType.Dark },
                    Resistances = new() { { DamageType.Magical, 0.5 } },
                    CanTeleport = true,
                    XPReward = 63, GoldReward = 45  // -40% rewards
                },
                
                // ═══ CABALLERO OSCURO ═══
                new()
                {
                    Name = "Caballero Oscuro",
                    Emoji = "⚔️",
                    Description = "Guerrero corrupto con armadura maldita",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Balanced,
                    HP = 213, MaxHP = 213,          // +150% (era 85) - REBALANCEADO
                    Attack = 70, MagicPower = 30,   // +150% (era 28/12) - REBALANCEADO
                    PhysicalDefense = 70, MagicResistance = 45,  // +150% - REBALANCEADO
                    Accuracy = 30, Evasion = 18, Speed = 10,      // +50%/50%/43% - REBALANCEADO
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Holy, 1.8 } },
                    Resistances = new() 
                    { 
                        { DamageType.Physical, 0.3 },
                        { DamageType.Dark, 0.7 }
                    },
                    Immunities = new() { DamageType.Poison },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Poisoned,
                        StatusEffectType.Bleeding
                    },
                    XPReward = 54, GoldReward = 57  // -40% rewards
                },
                
                // ═══ ESPECTRO ═══
                new()
                {
                    Name = "Espectro",
                    Emoji = "👻",
                    Description = "Fantasma intangible que drena vida",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 150, MaxHP = 150,            // +150% (era 60) - REBALANCEADO
                    Attack = 25, MagicPower = 70,   // +150% (era 10/28) - REBALANCEADO
                    PhysicalDefense = 13, MagicResistance = 75,  // +160%/150% - REBALANCEADO
                    Accuracy = 30, Evasion = 38, Speed = 13,    // +50%/52%/44% - REBALANCEADO
                    PrimaryDamageType = DamageType.Dark,
                    Weaknesses = new() 
                    { 
                        { DamageType.Holy, 3.0 },
                        { DamageType.Magical, 1.2 }
                    },
                    Resistances = new() { { DamageType.Physical, 0.8 } },
                    Immunities = new() { DamageType.Poison },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Poisoned,
                        StatusEffectType.Bleeding,
                        StatusEffectType.Stunned
                    },
                    CanFly = true,
                    XPReward = 110, GoldReward = 62  // +37/38% recompensas
                }
            };
        }
        
        public static List<RpgEnemy> GetBossEnemies()
        {
            return new List<RpgEnemy>
            {
                // ═══ DRAGÓN JOVEN ═══
                new()
                {
                    Name = "Dragón Joven",
                    Emoji = "🐉",
                    Description = "Bestia legendaria con aliento de fuego devastador",
                    Type = EnemyType.Dragon,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 320, MaxHP = 320,          // +60% (era 200)
                    Attack = 52, MagicPower = 60,   // +48/50% (era 35/40)
                    PhysicalDefense = 60, MagicResistance = 52,  // +50/48% (era 40/35)
                    Accuracy = 30, Evasion = 21, Speed = 9,      // +36/40/29% (era 22/15/7)
                    PrimaryDamageType = DamageType.Fire,
                    Weaknesses = new() { { DamageType.Ice, 1.5 } },
                    Resistances = new() 
                    { 
                        { DamageType.Physical, 0.3 },
                        { DamageType.Magical, 0.3 }
                    },
                    Immunities = new() { DamageType.Fire },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Stunned,
                        StatusEffectType.Poisoned,
                        StatusEffectType.Burning
                    },
                    CanFly = true,
                    SpecialAbilities = new() 
                    { 
                        "Aliento de fuego (AOE masivo)",
                        "Vuelo (evita ataques terrestres)",
                        "Intimidación (reduce stats)"
                    },
                    XPReward = 380, GoldReward = 300  // +52/50% recompensas
                },
                
                // ═══ LICH ═══
                new()
                {
                    Name = "Lich",
                    Emoji = "☠️",
                    Description = "Nigromante ancestral con poder necrótico absoluto",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 230, MaxHP = 230,          // +53% (era 150)
                    Attack = 30, MagicPower = 75,   // +50/50% (era 20/50)
                    PhysicalDefense = 38, MagicResistance = 66,  // +52/47% (era 25/45)
                    Accuracy = 35, Evasion = 28, Speed = 8,      // +40/40/33% (era 25/20/6)
                    PrimaryDamageType = DamageType.Dark,
                    Weaknesses = new() { { DamageType.Holy, 2.0 } },
                    Resistances = new() 
                    { 
                        { DamageType.Physical, 0.5 },
                        { DamageType.Magical, 0.3 }
                    },
                    Immunities = new() 
                    { 
                        DamageType.Poison,
                        DamageType.Dark
                    },
                    StatusImmunities = new() 
                    { 
                        StatusEffectType.Poisoned,
                        StatusEffectType.Bleeding,
                        StatusEffectType.Stunned
                    },
                    CanHeal = true,
                    CanTeleport = true,
                    SpecialAbilities = new() 
                    { 
                        "Reanimar muertos",
                        "Drenar vida",
                        "Teletransporte"
                    },
                    XPReward = 300, GoldReward = 250
                }
            };
        }
        
        /// <summary>
        /// Escala un enemigo al nivel del jugador
        /// </summary>
        public static RpgEnemy ScaleEnemy(RpgEnemy template, int playerLevel, int levelDiff)
        {
            var enemy = new RpgEnemy
            {
                Name = template.Name,
                Emoji = template.Emoji,
                Description = template.Description,
                Type = template.Type,
                Behavior = template.Behavior,
                Level = Math.Max(1, playerLevel + levelDiff),
                
                // Copiar stats base
                HP = template.HP,
                MaxHP = template.MaxHP,
                Attack = template.Attack,
                MagicPower = template.MagicPower,
                PhysicalDefense = template.PhysicalDefense,
                MagicResistance = template.MagicResistance,
                Accuracy = template.Accuracy,
                Evasion = template.Evasion,
                Speed = template.Speed,
                
                // Copiar mitología
                PrimaryDamageType = template.PrimaryDamageType,
                Resistances = new(template.Resistances),
                Weaknesses = new(template.Weaknesses),
                Immunities = new(template.Immunities),
                StatusImmunities = new(template.StatusImmunities),
                
                // Copiar habilidades
                CanPoison = template.CanPoison,
                CanStun = template.CanStun,
                CanHeal = template.CanHeal,
                CanFly = template.CanFly,
                CanTeleport = template.CanTeleport,
                CanRegenerate = template.CanRegenerate,
                SpecialAbilities = new(template.SpecialAbilities),
                
                XPReward = template.XPReward,
                GoldReward = template.GoldReward,
                Difficulty = template.Difficulty
            };
            
            // Escalar stats según nivel (DIFICULTAD AUMENTADA)
            // Enemigos ahora son mucho más peligrosos
            var scaledLevel = enemy.Level - 1;
            enemy.HP += scaledLevel * 18;          // +50% (era 12)
            enemy.MaxHP += scaledLevel * 18;       // +50% (era 12)
            enemy.Attack += scaledLevel * 3;       // +50% (era 2)
            enemy.MagicPower += scaledLevel * 3;   // +50% (era 2)
            enemy.PhysicalDefense += scaledLevel * 2;  // +100% (era 1)
            enemy.MagicResistance += scaledLevel * 2;  // +100% (era 1)
            enemy.Accuracy += scaledLevel / 2;
            enemy.Evasion += scaledLevel / 2;
            
            // Las recompensas también escalan más
            enemy.XPReward += scaledLevel * 15;    // +50% (era 10)
            enemy.GoldReward += scaledLevel * 8;   // +60% (era 5)
            
            return enemy;
        }
    }
}
