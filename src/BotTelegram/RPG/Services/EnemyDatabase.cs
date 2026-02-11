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
                    HP = 35, MaxHP = 35,            // +40% (era 25)
                    Attack = 18, MagicPower = 0,    // +50% (era 12)
                    PhysicalDefense = 12, MagicResistance = 5,  // +50% (era 8/3)
                    Accuracy = 18, Evasion = 15, Speed = 7,     // +20% (era 15/12)
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 1.3 } },
                    Resistances = new() { { DamageType.Ice, 0.3 } },
                    XPReward = 25, GoldReward = 20  // +25/33% recompensas
                },
                
                // ═══ GOBLIN ═══
                new()
                {
                    Name = "Goblin",
                    Emoji = "👺",
                    Description = "Criatura pequeña pero astuta con armas improvisadas",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Coward,
                    HP = 30, MaxHP = 30,            // +50% (era 20)
                    Attack = 15, MagicPower = 0,    // +50% (era 10)
                    PhysicalDefense = 8, MagicResistance = 8,  // +60% (era 5/5)
                    Accuracy = 15, Evasion = 13, Speed = 7,    // +25/30% (era 12/10)
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Fire, 1.5 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    XPReward = 20, GoldReward = 25  // +33/25% recompensas
                },
                
                // ═══ ESQUELETO ═══
                new()
                {
                    Name = "Esqueleto",
                    Emoji = "💀",
                    Description = "Muerto viviente reanimado por magia oscura",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Passive,
                    HP = 28, MaxHP = 28,            // +55% (era 18)
                    Attack = 20, MagicPower = 0,    // +43% (era 14)
                    PhysicalDefense = 5, MagicResistance = 15,  // +67/50% (era 3/10)
                    Accuracy = 13, Evasion = 7, Speed = 5,      // +30/40% (era 10/5)
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
                    XPReward = 23, GoldReward = 16  // +28/33% recompensas
                },
                
                // ═══ SLIME ═══
                new()
                {
                    Name = "Slime",
                    Emoji = "🟢",
                    Description = "Criatura gelatinosa que se mueve lentamente",
                    Type = EnemyType.Aberration,
                    Behavior = EnemyBehavior.Passive,
                    HP = 45, MaxHP = 45,            // +50% (era 30)
                    Attack = 9, MagicPower = 0,     // +50% (era 6)
                    PhysicalDefense = 4, MagicResistance = 4,  // +100% (era 2/2)
                    Accuracy = 10, Evasion = 5, Speed = 3,     // +25/67/50% (era 8/3/2)
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
                    XPReward = 16, GoldReward = 14  // +33/40% recompensas
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
                    HP = 80, MaxHP = 80,            // +45% (era 55)
                    Attack = 26, MagicPower = 0,    // +44% (era 18)
                    PhysicalDefense = 22, MagicResistance = 8,  // +47/60% (era 15/5)
                    Accuracy = 16, Evasion = 9, Speed = 6,      // +33/50% (era 12/6)
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Magical, 1.3 } },
                    Resistances = new() { { DamageType.Physical, 0.2 } },
                    XPReward = 55, GoldReward = 40  // +37/33% recompensas
                },
                
                // ═══ GOLEM DE PIEDRA ═══
                new()
                {
                    Name = "Golem de Piedra",
                    Emoji = "🗿",
                    Description = "Constructo animado de roca sólida, lento pero resistente",
                    Type = EnemyType.Construct,
                    Behavior = EnemyBehavior.Defensive,
                    HP = 120, MaxHP = 120,          // +50% (era 80)
                    Attack = 32, MagicPower = 0,    // +45% (era 22)
                    PhysicalDefense = 50, MagicResistance = 8,  // +43/60% (era 35/5)
                    Accuracy = 11, Evasion = 3, Speed = 4,      // +37/50/33% (era 8/2/3)
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
                    XPReward = 70, GoldReward = 35  // +40/40% recompensas
                },
                
                // ═══ ARAÑA GIGANTE ═══
                new()
                {
                    Name = "Araña Gigante",
                    Emoji = "🕷️",
                    Description = "Arácnido colosal con veneno mortal",
                    Type = EnemyType.Beast,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 60, MaxHP = 60,            // +50% (era 40)
                    Attack = 22, MagicPower = 0,    // +47% (era 15)
                    PhysicalDefense = 12, MagicResistance = 12,  // +50/50% (era 8/8)
                    Accuracy = 24, Evasion = 20, Speed = 10,     // +33/33/25% (era 18/15/8)
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Resistances = new() { { DamageType.Poison, 0.8 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    CanPoison = true,
                    XPReward = 50, GoldReward = 35  // +43/40% recompensas
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
                    HP = 150, MaxHP = 150,          // +50% (era 100)
                    Attack = 38, MagicPower = 22,   // +46/47% (era 26/15)
                    PhysicalDefense = 30, MagicResistance = 22,  // +50/47% (era 20/15)
                    Accuracy = 16, Evasion = 9, Speed = 6,       // +33/50/20% (era 12/6/5)
                    PrimaryDamageType = DamageType.Bludgeoning,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Immunities = new() { DamageType.Ice },
                    Resistances = new() { { DamageType.Physical, 0.3 } },
                    CanRegenerate = true,
                    XPReward = 95, GoldReward = 80  // +36/33% recompensas
                },
                
                // ═══ DEMONIO MENOR ═══
                new()
                {
                    Name = "Demonio Menor",
                    Emoji = "😈",
                    Description = "Ser infernal que domina la magia oscura",
                    Type = EnemyType.Demon,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 110, MaxHP = 110,          // +47% (era 75)
                    Attack = 22, MagicPower = 44,   // +47/47% (era 15/30)
                    PhysicalDefense = 18, MagicResistance = 36,  // +50/44% (era 12/25)
                    Accuracy = 24, Evasion = 22, Speed = 10,     // +33/37/25% (era 18/16/8)
                    PrimaryDamageType = DamageType.Fire,
                    Weaknesses = new() 
                    { 
                        { DamageType.Holy, 2.5 },
                        { DamageType.Water, 1.3 }
                    },
                    Immunities = new() { DamageType.Fire, DamageType.Dark },
                    Resistances = new() { { DamageType.Magical, 0.5 } },
                    CanTeleport = true,
                    XPReward = 105, GoldReward = 75  // +40/36% recompensas
                },
                
                // ═══ CABALLERO OSCURO ═══
                new()
                {
                    Name = "Caballero Oscuro",
                    Emoji = "⚔️",
                    Description = "Guerrero corrupto con armadura maldita",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Balanced,
                    HP = 125, MaxHP = 125,          // +47% (era 85)
                    Attack = 40, MagicPower = 18,   // +43/50% (era 28/12)
                    PhysicalDefense = 40, MagicResistance = 26,  // +43/44% (era 28/18)
                    Accuracy = 27, Evasion = 16, Speed = 9,      // +35/33/29% (era 20/12/7)
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
                    XPReward = 90, GoldReward = 95  // +38/36% recompensas
                },
                
                // ═══ ESPECTRO ═══
                new()
                {
                    Name = "Espectro",
                    Emoji = "👻",
                    Description = "Fantasma intangible que drena vida",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 88, MaxHP = 88,            // +47% (era 60)
                    Attack = 15, MagicPower = 40,   // +50/43% (era 10/28)
                    PhysicalDefense = 8, MagicResistance = 42,  // +60/40% (era 5/30)
                    Accuracy = 27, Evasion = 34, Speed = 11,    // +35/36/22% (era 20/25/9)
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
