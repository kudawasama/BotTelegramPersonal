namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Clase helper con información detallada de todas las clases del juego
    /// </summary>
    public static class ClassInfo
    {
        public static ClassDetails GetClassDetails(CharacterClass characterClass)
        {
            return characterClass switch
            {
                // TIER 1: BÁSICAS (Nivel 1)
                CharacterClass.Warrior => new ClassDetails
                {
                    Name = "Guerrero",
                    Emoji = "⚔️",
                    Tier = 1,
                    Description = "Maestro del combate cuerpo a cuerpo. Alta vida y defensa.",
                    StartingStats = new Stats { Str = 14, Int = 8, Dex = 10, Con = 14, Wis = 8, Cha = 8 },
                    StartingHP = 120,
                    StartingMana = 0,
                    StartingEnergy = 50,
                    PrimaryStat = "Fuerza",
                    PlayStyle = "Tanque / DPS cuerpo a cuerpo",
                    Strengths = "Alta supervivencia, buen daño físico constante",
                    Weaknesses = "Sin magia, alcance limitado"
                },
                
                CharacterClass.Mage => new ClassDetails
                {
                    Name = "Mago",
                    Emoji = "🔮",
                    Tier = 1,
                    Description = "Maestro de la magia elemental. Alto daño a distancia.",
                    StartingStats = new Stats { Str = 6, Int = 16, Dex = 8, Con = 8, Wis = 12, Cha = 10 },
                    StartingHP = 80,
                    StartingMana = 100,
                    StartingEnergy = 50,
                    PrimaryStat = "Inteligencia",
                    PlayStyle = "Caster / Daño a distancia",
                    Strengths = "Altísimo daño mágico, control de masas",
                    Weaknesses = "Muy frágil, dependiente de maná"
                },
                
                CharacterClass.Rogue => new ClassDetails
                {
                    Name = "Ladrón",
                    Emoji = "🗡️",
                    Tier = 1,
                    Description = "Experto en sigilo y críticos devastadores.",
                    StartingStats = new Stats { Str = 10, Int = 10, Dex = 16, Con = 10, Wis = 10, Cha = 10 },
                    StartingHP = 100,
                    StartingMana = 30,
                    StartingEnergy = 60,
                    PrimaryStat = "Destreza",
                    PlayStyle = "DPS burst / Críticos",
                    Strengths = "Críticos frecuentes, alta evasión",
                    Weaknesses = "Baja vida, riesgo alto"
                },
                
                CharacterClass.Cleric => new ClassDetails
                {
                    Name = "Clérigo",
                    Emoji = "✨",
                    Tier = 1,
                    Description = "Sanador divino. Balance entre combate y soporte.",
                    StartingStats = new Stats { Str = 10, Int = 12, Dex = 8, Con = 12, Wis = 14, Cha = 12 },
                    StartingHP = 110,
                    StartingMana = 80,
                    StartingEnergy = 50,
                    PrimaryStat = "Sabiduría",
                    PlayStyle = "Soporte / Curación",
                    Strengths = "Auto-curación, versátil, resistente",
                    Weaknesses = "Daño moderado"
                },
                
                // TIER 2: AVANZADAS (Nivel 10+)
                CharacterClass.Paladin => new ClassDetails
                {
                    Name = "Paladín",
                    Emoji = "🛡️",
                    Tier = 2,
                    EvolvesFrom = CharacterClass.Warrior,
                    RequiredLevel = 10,
                    Description = "Guerrero sagrado. Combina fuerza física con magia divina.",
                    StartingStats = new Stats { Str = 14, Int = 10, Dex = 8, Con = 14, Wis = 12, Cha = 12 },
                    StartingHP = 130,
                    StartingMana = 60,
                    StartingEnergy = 50,
                    PrimaryStat = "Fuerza + Sabiduría",
                    PlayStyle = "Tank mágico / Soporte ofensivo",
                    Strengths = "Tanque inmortal, auto-curación, auras",
                    Weaknesses = "Daño mágico moderado"
                },
                
                CharacterClass.Ranger => new ClassDetails
                {
                    Name = "Guardabosques",
                    Emoji = "🏹",
                    Tier = 2,
                    EvolvesFrom = CharacterClass.Rogue,
                    RequiredLevel = 10,
                    Description = "Arquero experto. Ataque a distancia con mascotas.",
                    StartingStats = new Stats { Str = 10, Int = 10, Dex = 16, Con = 12, Wis = 14, Cha = 8 },
                    StartingHP = 105,
                    StartingMana = 50,
                    StartingEnergy = 60,
                    PrimaryStat = "Destreza + Sabiduría",
                    PlayStyle = "DPS a distancia / Pet class",
                    Strengths = "Seguro (distancia), compañero animal, trampas",
                    Weaknesses = "Débil en cuerpo a cuerpo"
                },
                
                CharacterClass.Warlock => new ClassDetails
                {
                    Name = "Brujo",
                    Emoji = "🔥",
                    Tier = 2,
                    EvolvesFrom = CharacterClass.Mage,
                    RequiredLevel = 10,
                    Description = "Mago oscuro. Pactos demoníacos y magia prohibida.",
                    StartingStats = new Stats { Str = 6, Int = 16, Dex = 10, Con = 10, Wis = 10, Cha = 14 },
                    StartingHP = 85,
                    StartingMana = 120,
                    StartingEnergy = 50,
                    PrimaryStat = "Inteligencia + Carisma",
                    PlayStyle = "DoT (daño en tiempo) / Invocaciones",
                    Strengths = "Daño sostenido brutal, life steal, pets",
                    Weaknesses = "Muy frágil, necesita setup"
                },
                
                CharacterClass.Monk => new ClassDetails
                {
                    Name = "Monje",
                    Emoji = "🥋",
                    Tier = 2,
                    EvolvesFrom = CharacterClass.Cleric,
                    RequiredLevel = 10,
                    Description = "Maestro de artes marciales. Ki y puños sagrados.",
                    StartingStats = new Stats { Str = 12, Int = 10, Dex = 14, Con = 12, Wis = 16, Cha = 10 },
                    StartingHP = 115,
                    StartingMana = 70,
                    StartingEnergy = 70,
                    PrimaryStat = "Sabiduría + Destreza",
                    PlayStyle = "Combate rápido / Movilidad",
                    Strengths = "Muchos ataques, alta evasión, stuns",
                    Weaknesses = "Requiere posicionamiento perfecto"
                },
                
                // TIER 3: ÉPICAS (Nivel 20+)
                CharacterClass.Berserker => new ClassDetails
                {
                    Name = "Berserker",
                    Emoji = "⚡",
                    Tier = 3,
                    EvolvesFrom = CharacterClass.Warrior,
                    RequiredLevel = 20,
                    Description = "Furia pura. Sacrifica defensa por daño apocalíptico.",
                    StartingStats = new Stats { Str = 20, Int = 6, Dex = 12, Con = 16, Wis = 6, Cha = 8 },
                    StartingHP = 140,
                    StartingMana = 0,
                    StartingEnergy = 50,
                    PrimaryStat = "Fuerza + Constitución",
                    PlayStyle = "Glass cannon / Berserker",
                    Strengths = "DPS físico más alto del juego, ejecuciones",
                    Weaknesses = "Pierde defensa al atacar, todo o nada"
                },
                
                CharacterClass.Assassin => new ClassDetails
                {
                    Name = "Asesino",
                    Emoji = "🗡️",
                    Tier = 3,
                    EvolvesFrom = CharacterClass.Rogue,
                    RequiredLevel = 20,
                    Description = "Muerte silenciosa. Críticos garantizados desde sigilo.",
                    StartingStats = new Stats { Str = 12, Int = 12, Dex = 20, Con = 10, Wis = 12, Cha = 10 },
                    StartingHP = 105,
                    StartingMana = 40,
                    StartingEnergy = 70,
                    PrimaryStat = "Destreza",
                    PlayStyle = "One-shot / Burst extremo",
                    Strengths = "Críticos letales, venenos, invisibilidad",
                    Weaknesses = "Si falla el burst, muere rápido"
                },
                
                CharacterClass.Sorcerer => new ClassDetails
                {
                    Name = "Hechicero",
                    Emoji = "🌟",
                    Tier = 3,
                    EvolvesFrom = CharacterClass.Mage,
                    RequiredLevel = 20,
                    Description = "Magia caótica innata. Elementos combinados.",
                    StartingStats = new Stats { Str = 6, Int = 20, Dex = 10, Con = 10, Wis = 10, Cha = 16 },
                    StartingHP = 90,
                    StartingMana = 150,
                    StartingEnergy = 50,
                    PrimaryStat = "Inteligencia + Carisma",
                    PlayStyle = "Multi-elemento / AoE masivo",
                    Strengths = "Versatilidad mágica total, metamagia",
                    Weaknesses = "Frágil, RNG dependiente"
                },
                
                CharacterClass.Druid => new ClassDetails
                {
                    Name = "Druida",
                    Emoji = "🌿",
                    Tier = 3,
                    EvolvesFrom = CharacterClass.Cleric,
                    RequiredLevel = 20,
                    Description = "Guardián de la naturaleza. Cambiaformas y curación.",
                    StartingStats = new Stats { Str = 12, Int = 12, Dex = 12, Con = 14, Wis = 18, Cha = 12 },
                    StartingHP = 120,
                    StartingMana = 100,
                    StartingEnergy = 50,
                    PrimaryStat = "Sabiduría",
                    PlayStyle = "Híbrido / Transformación",
                    Strengths = "Versatilidad extrema, regeneración, formas animales",
                    Weaknesses = "Maestro de nada"
                },
                
                // TIER 4: LEGENDARIAS (Nivel 30+)
                CharacterClass.Necromancer => new ClassDetails
                {
                    Name = "Nigromante",
                    Emoji = "💀",
                    Tier = 4,
                    EvolvesFrom = CharacterClass.Warlock,
                    RequiredLevel = 30,
                    Description = "Maestro de la muerte. Ejército de no-muertos.",
                    StartingStats = new Stats { Str = 8, Int = 22, Dex = 10, Con = 12, Wis = 14, Cha = 16 },
                    StartingHP = 95,
                    StartingMana = 180,
                    StartingEnergy = 50,
                    PrimaryStat = "Inteligencia",
                    PlayStyle = "Pet master / AoE",
                    Strengths = "Ejército infinito, life drain masivo",
                    Weaknesses = "Setup lento, countered por clérigos"
                },
                
                CharacterClass.Bard => new ClassDetails
                {
                    Name = "Bardo",
                    Emoji = "🎵",
                    Tier = 4,
                    EvolvesFrom = CharacterClass.Cleric, // O cualquiera con Charisma
                    RequiredLevel = 30,
                    Description = "Músico mágico. Buffs de equipo y versatilidad.",
                    StartingStats = new Stats { Str = 10, Int = 14, Dex = 14, Con = 12, Wis = 14, Cha = 20 },
                    StartingHP = 110,
                    StartingMana = 120,
                    StartingEnergy = 60,
                    PrimaryStat = "Carisma",
                    PlayStyle = "Soporte / Buffer / Jack-of-all-trades",
                    Strengths = "Buffs masivos, puede hacer de todo aceptablemente",
                    Weaknesses = "No es el mejor en nada específico"
                },
                
                _ => new ClassDetails { Name = "Desconocido", Emoji = "❓" }
            };
        }
        
        public static List<CharacterClass> GetAvailableEvolutions(CharacterClass currentClass, int playerLevel)
        {
            var evolutions = new List<CharacterClass>();
            
            if (playerLevel >= 10)
            {
                switch (currentClass)
                {
                    case CharacterClass.Warrior:
                        evolutions.Add(CharacterClass.Paladin);
                        break;
                    case CharacterClass.Rogue:
                        evolutions.Add(CharacterClass.Ranger);
                        break;
                    case CharacterClass.Mage:
                        evolutions.Add(CharacterClass.Warlock);
                        break;
                    case CharacterClass.Cleric:
                        evolutions.Add(CharacterClass.Monk);
                        break;
                }
            }
            
            if (playerLevel >= 20)
            {
                switch (currentClass)
                {
                    case CharacterClass.Warrior:
                    case CharacterClass.Paladin:
                        evolutions.Add(CharacterClass.Berserker);
                        break;
                    case CharacterClass.Rogue:
                    case CharacterClass.Ranger:
                        evolutions.Add(CharacterClass.Assassin);
                        break;
                    case CharacterClass.Mage:
                    case CharacterClass.Warlock:
                        evolutions.Add(CharacterClass.Sorcerer);
                        break;
                    case CharacterClass.Cleric:
                    case CharacterClass.Monk:
                        evolutions.Add(CharacterClass.Druid);
                        break;
                }
            }
            
            if (playerLevel >= 30)
            {
                switch (currentClass)
                {
                    case CharacterClass.Warlock:
                    case CharacterClass.Sorcerer:
                        evolutions.Add(CharacterClass.Necromancer);
                        break;
                    case CharacterClass.Monk:
                    case CharacterClass.Druid:
                    case CharacterClass.Paladin:
                        if (playerLevel >= 30) evolutions.Add(CharacterClass.Bard);
                        break;
                }
            }
            
            return evolutions;
        }
    }
    
    public class ClassDetails
    {
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "";
        public int Tier { get; set; } = 1;
        public CharacterClass? EvolvesFrom { get; set; }
        public int RequiredLevel { get; set; } = 1;
        public string Description { get; set; } = "";
        public Stats StartingStats { get; set; } = new();
        public int StartingHP { get; set; } = 100;
        public int StartingMana { get; set; } = 0;
        public int StartingEnergy { get; set; } = 50;
        public string PrimaryStat { get; set; } = "";
        public string PlayStyle { get; set; } = "";
        public string Strengths { get; set; } = "";
        public string Weaknesses { get; set; } = "";
    }
    
    public class Stats
    {
        public int Str { get; set; } = 10;
        public int Int { get; set; } = 10;
        public int Dex { get; set; } = 10;
        public int Con { get; set; } = 10;
        public int Wis { get; set; } = 10;
        public int Cha { get; set; } = 10;
    }
}
