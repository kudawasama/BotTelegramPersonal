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
                    HP = 25, MaxHP = 25,
                    Attack = 12, MagicPower = 0,
                    PhysicalDefense = 8, MagicResistance = 3,
                    Accuracy = 15, Evasion = 12, Speed = 7,
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 1.3 } },
                    Resistances = new() { { DamageType.Ice, 0.3 } },
                    XPReward = 20, GoldReward = 15
                },
                
                // ═══ GOBLIN ═══
                new()
                {
                    Name = "Goblin",
                    Emoji = "👺",
                    Description = "Criatura pequeña pero astuta con armas improvisadas",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Coward,
                    HP = 20, MaxHP = 20,
                    Attack = 10, MagicPower = 0,
                    PhysicalDefense = 5, MagicResistance = 5,
                    Accuracy = 12, Evasion = 10, Speed = 6,
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Fire, 1.5 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    XPReward = 15, GoldReward = 20
                },
                
                // ═══ ESQUELETO ═══
                new()
                {
                    Name = "Esqueleto",
                    Emoji = "💀",
                    Description = "Muerto viviente reanimado por magia oscura",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Passive,
                    HP = 18, MaxHP = 18,
                    Attack = 14, MagicPower = 0,
                    PhysicalDefense = 3, MagicResistance = 10,
                    Accuracy = 10, Evasion = 5, Speed = 4,
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
                    XPReward = 18, GoldReward = 12
                },
                
                // ═══ SLIME ═══
                new()
                {
                    Name = "Slime",
                    Emoji = "🟢",
                    Description = "Criatura gelatinosa que se mueve lentamente",
                    Type = EnemyType.Aberration,
                    Behavior = EnemyBehavior.Passive,
                    HP = 30, MaxHP = 30,
                    Attack = 6, MagicPower = 0,
                    PhysicalDefense = 2, MagicResistance = 2,
                    Accuracy = 8, Evasion = 3, Speed = 2,
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
                    XPReward = 12, GoldReward = 10
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
                    HP = 55, MaxHP = 55,
                    Attack = 18, MagicPower = 0,
                    PhysicalDefense = 15, MagicResistance = 5,
                    Accuracy = 12, Evasion = 6, Speed = 5,
                    PrimaryDamageType = DamageType.Slashing,
                    Weaknesses = new() { { DamageType.Magical, 1.3 } },
                    Resistances = new() { { DamageType.Physical, 0.2 } },
                    XPReward = 40, GoldReward = 30
                },
                
                // ═══ GOLEM DE PIEDRA ═══
                new()
                {
                    Name = "Golem de Piedra",
                    Emoji = "🗿",
                    Description = "Constructo animado de roca sólida, lento pero resistente",
                    Type = EnemyType.Construct,
                    Behavior = EnemyBehavior.Defensive,
                    HP = 80, MaxHP = 80,
                    Attack = 22, MagicPower = 0,
                    PhysicalDefense = 35, MagicResistance = 5,
                    Accuracy = 8, Evasion = 2, Speed = 3,
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
                    XPReward = 50, GoldReward = 25
                },
                
                // ═══ ARAÑA GIGANTE ═══
                new()
                {
                    Name = "Araña Gigante",
                    Emoji = "🕷️",
                    Description = "Arácnido colosal con veneno mortal",
                    Type = EnemyType.Beast,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 40, MaxHP = 40,
                    Attack = 15, MagicPower = 0,
                    PhysicalDefense = 8, MagicResistance = 8,
                    Accuracy = 18, Evasion = 15, Speed = 8,
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Resistances = new() { { DamageType.Poison, 0.8 } },
                    StatusImmunities = new() { StatusEffectType.Poisoned },
                    CanPoison = true,
                    XPReward = 35, GoldReward = 25
                },
                
                // ═══ ELEMENTAL DE FUEGO ═══
                new()
                {
                    Name = "Elemental de Fuego",
                    Emoji = "🔥",
                    Description = "Ser de llamas puras que arde furiosamente",
                    Type = EnemyType.Elemental,
                    Behavior = EnemyBehavior.Berserker,
                    HP = 35, MaxHP = 35,
                    Attack = 8, MagicPower = 25,
                    PhysicalDefense = 5, MagicResistance = 20,
                    Accuracy = 15, Evasion = 18, Speed = 9,
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
                    XPReward = 45, GoldReward = 35
                },
                
                // ═══ BANDIDO ═══
                new()
                {
                    Name = "Bandido",
                    Emoji = "🏴‍☠️",
                    Description = "Ladrón experimentado con dagas envenenadas",
                    Type = EnemyType.Humanoid,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 38, MaxHP = 38,
                    Attack = 20, MagicPower = 0,
                    PhysicalDefense = 10, MagicResistance = 8,
                    Accuracy = 22, Evasion = 18, Speed = 8,
                    PrimaryDamageType = DamageType.Piercing,
                    Weaknesses = new() { { DamageType.Magical, 1.3 } },
                    CanPoison = true,
                    XPReward = 38, GoldReward = 50
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
                    HP = 100, MaxHP = 100,
                    Attack = 26, MagicPower = 15,
                    PhysicalDefense = 20, MagicResistance = 15,
                    Accuracy = 12, Evasion = 6, Speed = 5,
                    PrimaryDamageType = DamageType.Bludgeoning,
                    Weaknesses = new() { { DamageType.Fire, 2.0 } },
                    Immunities = new() { DamageType.Ice },
                    Resistances = new() { { DamageType.Physical, 0.3 } },
                    CanRegenerate = true,
                    XPReward = 70, GoldReward = 60
                },
                
                // ═══ DEMONIO MENOR ═══
                new()
                {
                    Name = "Demonio Menor",
                    Emoji = "😈",
                    Description = "Ser infernal que domina la magia oscura",
                    Type = EnemyType.Demon,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 75, MaxHP = 75,
                    Attack = 15, MagicPower = 30,
                    PhysicalDefense = 12, MagicResistance = 25,
                    Accuracy = 18, Evasion = 16, Speed = 8,
                    PrimaryDamageType = DamageType.Fire,
                    Weaknesses = new() 
                    { 
                        { DamageType.Holy, 2.5 },
                        { DamageType.Water, 1.3 }
                    },
                    Immunities = new() { DamageType.Fire, DamageType.Dark },
                    Resistances = new() { { DamageType.Magical, 0.5 } },
                    CanTeleport = true,
                    XPReward = 75, GoldReward = 55
                },
                
                // ═══ CABALLERO OSCURO ═══
                new()
                {
                    Name = "Caballero Oscuro",
                    Emoji = "⚔️",
                    Description = "Guerrero corrupto con armadura maldita",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Balanced,
                    HP = 85, MaxHP = 85,
                    Attack = 28, MagicPower = 12,
                    PhysicalDefense = 28, MagicResistance = 18,
                    Accuracy = 20, Evasion = 12, Speed = 7,
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
                    XPReward = 65, GoldReward = 70
                },
                
                // ═══ ESPECTRO ═══
                new()
                {
                    Name = "Espectro",
                    Emoji = "👻",
                    Description = "Fantasma intangible que drena vida",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 60, MaxHP = 60,
                    Attack = 10, MagicPower = 28,
                    PhysicalDefense = 5, MagicResistance = 30,
                    Accuracy = 20, Evasion = 25, Speed = 9,
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
                    XPReward = 80, GoldReward = 45
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
                    HP = 200, MaxHP = 200,
                    Attack = 35, MagicPower = 40,
                    PhysicalDefense = 40, MagicResistance = 35,
                    Accuracy = 22, Evasion = 15, Speed = 7,
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
                    XPReward = 250, GoldReward = 200
                },
                
                // ═══ LICH ═══
                new()
                {
                    Name = "Lich",
                    Emoji = "☠️",
                    Description = "Nigromante ancestral con poder necrótico absoluto",
                    Type = EnemyType.Undead,
                    Behavior = EnemyBehavior.Intelligent,
                    HP = 150, MaxHP = 150,
                    Attack = 20, MagicPower = 50,
                    PhysicalDefense = 25, MagicResistance = 45,
                    Accuracy = 25, Evasion = 20, Speed = 6,
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
            
            // Escalar stats según nivel
            var scaledLevel = enemy.Level - 1;
            enemy.HP += scaledLevel * 12;
            enemy.MaxHP += scaledLevel * 12;
            enemy.Attack += scaledLevel * 2;
            enemy.MagicPower += scaledLevel * 2;
            enemy.PhysicalDefense += scaledLevel * 1;
            enemy.MagicResistance += scaledLevel * 1;
            enemy.Accuracy += scaledLevel / 2;
            enemy.Evasion += scaledLevel / 2;
            
            enemy.XPReward += scaledLevel * 10;
            enemy.GoldReward += scaledLevel * 5;
            
            return enemy;
        }
    }
}
