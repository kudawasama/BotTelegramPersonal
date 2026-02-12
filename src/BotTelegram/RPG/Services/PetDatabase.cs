using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de especies de mascotas con sus stats y evoluciones
    /// </summary>
    public static class PetDatabase
    {
        // ══════════════════════════════════════════════════════════════════
        // Wolf FAMILY - Lobo → Lobo Alfa → Fenrir
        // ══════════════════════════════════════════════════════════════════
        
        public static Dictionary<string, PetSpeciesData> GetAllSpecies()
        {
            return new Dictionary<string, PetSpeciesData>
            {
                // ═══ FAMILY: CANINOS ═══
                ["wolf_1"] = new PetSpeciesData
                {
                    Species = "Lobo Salvaje",
                    Emoji = "🐺",
                    EvolutionStage = 1,
                    NextEvolution = "wolf_2",
                    Rarity = PetRarity.Common,
                    BaseStats = new PetStats { HP = 50, Attack = 25, Defense = 15, Speed = 8, MagicPower = 0 },
                    Abilities = new List<string> { "bite" },
                    EvolutionRequirements = new EvolutionReqs { Level = 15, Bond = 400, Kills = 50, BossKills = 0 }
                },
                ["wolf_2"] = new PetSpeciesData
                {
                    Species = "Lobo Alfa",
                    Emoji = "🐺",
                    EvolutionStage = 2,
                    NextEvolution = "wolf_3",
                    Rarity = PetRarity.Uncommon,
                    BaseStats = new PetStats { HP = 120, Attack = 60, Defense = 35, Speed = 12, MagicPower = 0 },
                    Abilities = new List<string> { "bite", "savage_claw", "howl" },
                    EvolutionRequirements = new EvolutionReqs { Level = 35, Bond = 700, Kills = 200, BossKills = 50 }
                },
                ["wolf_3"] = new PetSpeciesData
                {
                    Species = "Fenrir",
                    Emoji = "🐺",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Legendary,
                    BaseStats = new PetStats { HP = 280, Attack = 140, Defense = 80, Speed = 18, MagicPower = 30 },
                    Abilities = new List<string> { "bite", "savage_claw", "howl", "ancestral_fury", "phantom_wolf" },
                    EvolutionRequirements = null,
                    Passive = "Pack Leader: Otros wolves reciben +50% stats"
                },
                
                // ═══ FAMILY: OSOS ═══
                ["bear_1"] = new PetSpeciesData
                {
                    Species = "Oso Pardo",
                    Emoji = "🐻",
                    EvolutionStage = 1,
                    NextEvolution = "bear_2",
                    Rarity = PetRarity.Common,
                    BaseStats = new PetStats { HP = 80, Attack = 30, Defense = 25, Speed = 4, MagicPower = 0 },
                    Abilities = new List<string> { "claw_swipe" },
                    EvolutionRequirements = new EvolutionReqs { Level = 15, Bond = 400, Kills = 50, BossKills = 0 }
                },
                ["bear_2"] = new PetSpeciesData
                {
                    Species = "Oso Acorazado",
                    Emoji = "🐻",
                    EvolutionStage = 2,
                    NextEvolution = "bear_3",
                    Rarity = PetRarity.Uncommon,
                    BaseStats = new PetStats { HP = 200, Attack = 75, Defense = 70, Speed = 6, MagicPower = 0 },
                    Abilities = new List<string> { "claw_swipe", "bear_roar", "iron_hide" },
                    EvolutionRequirements = new EvolutionReqs { Level = 35, Bond = 700, Kills = 200, BossKills = 50 }
                },
                ["bear_3"] = new PetSpeciesData
                {
                    Species = "Ursakar el Eterno",
                    Emoji = "🐻",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Legendary,
                    BaseStats = new PetStats { HP = 450, Attack = 160, Defense = 180, Speed = 8, MagicPower = 0 },
                    Abilities = new List<string> { "claw_swipe", "bear_roar", "iron_hide", "earthquake_slam", "unstoppable_charge" },
                    EvolutionRequirements = null,
                    Passive = "Inmovable: Inmune a stun y knockback, -50% daño recibido de críticos"
                },
                
                // ═══ FAMILY: DRAGONS ═══
                ["dragon_1"] = new PetSpeciesData
                {
                    Species = "Dragón Bebé",
                    Emoji = "🦎",
                    EvolutionStage = 1,
                    NextEvolution = "dragon_2",
                    Rarity = PetRarity.Rare,
                    BaseStats = new PetStats { HP = 60, Attack = 35, Defense = 20, Speed = 7, MagicPower = 40 },
                    Abilities = new List<string> { "flame_breath" },
                    EvolutionRequirements = new EvolutionReqs { Level = 20, Bond = 500, Kills = 100, BossKills = 10 }
                },
                ["dragon_2"] = new PetSpeciesData
                {
                    Species = "Dragón Joven",
                    Emoji = "🐉",
                    EvolutionStage = 2,
                    NextEvolution = "dragon_3",
                    Rarity = PetRarity.Epic,
                    BaseStats = new PetStats { HP = 180, Attack = 90, Defense = 60, Speed = 12, MagicPower = 110 },
                    Abilities = new List<string> { "flame_breath", "dragon_claw", "wing_gust", "fireball" },
                    EvolutionRequirements = new EvolutionReqs { Level = 40, Bond = 800, Kills = 300, BossKills = 100 }
                },
                ["dragon_3"] = new PetSpeciesData
                {
                    Species = "Dragón Ancestral",
                    Emoji = "🐉",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Mythical,
                    BaseStats = new PetStats { HP = 400, Attack = 200, Defense = 140, Speed = 18, MagicPower = 280 },
                    Abilities = new List<string> { "flame_breath", "dragon_claw", "wing_gust", "fireball", "inferno", "dragon_rage", "immortal_flame" },
                    EvolutionRequirements = null,
                    Passive = "Dragón Completo: Inmune a fuego, +100% daño mágico, puede volar (ignora terrain)"
                },
                
                // ═══ FAMILY: FELINOS ═══
                ["cat_1"] = new PetSpeciesData
                {
                    Species = "Gato Montés",
                    Emoji = "🐱",
                    EvolutionStage = 1,
                    NextEvolution = "cat_2",
                    Rarity = PetRarity.Common,
                    BaseStats = new PetStats { HP = 40, Attack = 30, Defense = 10, Speed = 14, MagicPower = 0 },
                    Abilities = new List<string> { "quick_strike" },
                    EvolutionRequirements = new EvolutionReqs { Level = 15, Bond = 400, Kills = 50, BossKills = 0 }
                },
                ["cat_2"] = new PetSpeciesData
                {
                    Species = "Pantera Sombra",
                    Emoji = "🐈‍⬛",
                    EvolutionStage = 2,
OneEvolution = "cat_3",
                    Rarity = PetRarity.Rare,
                    BaseStats = new PetStats { HP = 100, Attack = 80, Defense = 30, Speed = 22, MagicPower = 0 },
                    Abilities = new List<string> { "quick_strike", "shadow_pounce", "evasion_boost" },
                    EvolutionRequirements = new EvolutionReqs { Level = 35, Bond = 700, Kills = 200, BossKills = 50 }
                },
                ["cat_3"] = new PetSpeciesData
                {
                    Species = "Smilodon Espectral",
                    Emoji = "🐅",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Legendary,
                    BaseStats = new PetStats { HP = 220, Attack = 180, Defense = 65, Speed = 30, MagicPower = 40 },
                    Abilities = new List<string> { "quick_strike", "shadow_pounce", "evasion_boost", "assassinate", "phantom_step" },
                    EvolutionRequirements = null,
                    Passive = "Cazador Perfecto: +50% crit chance, +100% crit damage, puede atravesar enemigos"
                },
                
                // ═══ FAMILY: AVES ═══
                ["eagle_1"] = new PetSpeciesData
                {
                    Species = "Águila",
                    Emoji = "🦅",
                    EvolutionStage = 1,
                    NextEvolution = "eagle_2",
                    Rarity = PetRarity.Uncommon,
                    BaseStats = new PetStats { HP = 35, Attack = 28, Defense = 8, Speed = 16, MagicPower = 0 },
                    Abilities = new List<string> { "dive_attack" },
                    EvolutionRequirements = new EvolutionReqs { Level = 15, Bond = 400, Kills = 50, BossKills = 0 }
                },
                ["eagle_2"] = new PetSpeciesData
                {
                    Species = "Águila Real",
                    Emoji = "🦅",
                    EvolutionStage = 2,
                    NextEvolution = "eagle_3",
                    Rarity = PetRarity.Rare,
                    BaseStats = new PetStats { HP = 85, Attack = 70, Defense = 20, Speed = 26, MagicPower = 0 },
                    Abilities = new List<string> { "dive_attack", "talon_strike", "keen_eye" },
                    EvolutionRequirements = new EvolutionReqs { Level = 35, Bond = 700, Kills = 200, BossKills = 50 }
                },
                ["eagle_3"] = new PetSpeciesData
                {
                    Species = "Fénix",
                    Emoji = "🔥🦅",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Mythical,
                    BaseStats = new PetStats { HP = 200, Attack = 140, Defense = 50, Speed = 35, MagicPower = 120 },
                    Abilities = new List<string> { "dive_attack", "talon_strike", "keen_eye", "fire_dive", "rebirth" },
                    EvolutionRequirements = null,
                    Passive = "Renacimiento: Si muere, revive con 50% HP automáticamente (1 vez por combate)"
                },
                
                // ═══ FAMILY: REPTILES ═══
                ["snake_1"] = new PetSpeciesData
                {
                    Species = "Serpiente Venenosa",
                    Emoji = "🐍",
                    EvolutionStage = 1,
                    NextEvolution = "snake_2",
                    Rarity = PetRarity.Common,
                    BaseStats = new PetStats { HP = 45, Attack = 22, Defense = 12, Speed = 10, MagicPower = 15 },
                    Abilities = new List<string> { "poison_bite" },
                    EvolutionRequirements = new EvolutionReqs { Level = 15, Bond = 400, Kills = 50, BossKills = 0 }
                },
                ["snake_2"] = new PetSpeciesData
                {
                    Species = "Basilisco",
                    Emoji = "🐍",
                    EvolutionStage = 2,
                    NextEvolution = "snake_3",
                    Rarity = PetRarity.Rare,
                    BaseStats = new PetStats { HP = 110, Attack = 60, Defense = 35, Speed = 14, MagicPower = 50 },
                    Abilities = new List<string> { "poison_bite", "petrifying_gaze", "venom_spit" },
                    EvolutionRequirements = new EvolutionReqs { Level = 35, Bond = 700, Kills = 200, BossKills = 50 }
                },
                ["snake_3"] = new PetSpeciesData
                {
                    Species = "Jörmungandr",
                    Emoji = "🐍",
                    EvolutionStage = 3,
                    NextEvolution = null,
                    Rarity = PetRarity.Mythical,
                    BaseStats = new PetStats { HP = 350, Attack = 130, Defense = 90, Speed = 16, MagicPower = 150 },
                    Abilities = new List<string> { "poison_bite", "petrifying_gaze", "venom_spit", "world_serpent_coil", "toxic_deluge" },
                    EvolutionRequirements = null,
                    Passive = "Veneno Eterno: Todos los ataques causan poison, poison hace 3x daño"
                }
            };
        }
        
        /// <summary>
        /// Crea una nueva mascota basada en una especie
        /// </summary>
        public static RpgPet CreatePet(string speciesId, string customName = "")
        {
            var allSpecies = GetAllSpecies();
            if (!allSpecies.ContainsKey(speciesId))
            {
                throw new ArgumentException($"Especie no encontrada: {speciesId}");
            }
            
            var speciesData = allSpecies[speciesId];
            var pet = new RpgPet
            {
                Species = speciesId,
                Name = string.IsNullOrWhiteSpace(customName) ? speciesData.Species : customName,
                Rarity = speciesData.Rarity,
                EvolutionStage = speciesData.EvolutionStage,
                HP = speciesData.BaseStats.HP,
                MaxHP = speciesData.BaseStats.HP,
                Attack = speciesData.BaseStats.Attack,
                Defense = speciesData.BaseStats.Defense,
                Speed = speciesData.BaseStats.Speed,
                MagicPower = speciesData.BaseStats.MagicPower,
                Abilities = new List<string>(speciesData.Abilities),
                Bond = 200, // Empieza en Neutral
                Loyalty = PetLoyalty.Neutral
            };
            
            pet.UpdateLoyalty();
            return pet;
        }
        
        /// <summary>
        /// Obtiene datos de especie por ID
        /// </summary>
        public static PetSpeciesData? GetSpeciesData(string speciesId)
        {
            var allSpecies = GetAllSpecies();
            return allSpecies.TryGetValue(speciesId, out var data) ? data : null;
        }
        
        /// <summary>
        /// Evoluciona una mascota a su siguiente forma
        /// </summary>
        public static bool EvolvePet(RpgPet pet)
        {
            var speciesData = GetSpeciesData(pet.Species);
            if (speciesData == null || speciesData.NextEvolution == null)
            {
                return false; // No puede evolucionar más
            }
            
            // Verificar requisitos
            var reqs = speciesData.EvolutionRequirements;
            if (reqs != null && !pet.CheckEvolution(reqs.Bond, reqs.Kills, reqs.BossKills))
            {
                return false;
            }
            
            // Obtener datos de next evolution
            var nextSpeciesData = GetSpeciesData(speciesData.NextEvolution);
            if (nextSpeciesData == null)
            {
                return false;
            }
            
            // Guardar % de HP actual
            double hpPercent = (double)pet.HP / pet.MaxHP;
            
            // Evolucionarpet.Species = speciesData.NextEvolution;
            pet.EvolutionStage = nextSpeciesData.EvolutionStage;
            pet.Rarity = nextSpeciesData.Rarity;
            
            // Actualizar stats
            pet.MaxHP = nextSpeciesData.BaseStats.HP;
            pet.HP = (int)(pet.MaxHP * hpPercent); // Mantener % de HP
            pet.Attack = nextSpeciesData.BaseStats.Attack;
            pet.Defense = nextSpeciesData.BaseStats.Defense;
            pet.Speed = nextSpeciesData.BaseStats.Speed;
            pet.MagicPower = nextSpeciesData.BaseStats.MagicPower;
            
            // Actualizar habilidades
            pet.Abilities = new List<string>(nextSpeciesData.Abilities);
            
            pet.CanEvolve = false;
            
            return true;
        }
    }
    
    // ══════════════════════════════════════════════════════════════════
    // HELPER CLASSES
    // ══════════════════════════════════════════════════════════════════
    
    public class PetSpeciesData
    {
        public string Species { get; set; } = "";
        public string Emoji { get; set; } = "";
        public int EvolutionStage { get; set; } = 1;
        public string? NextEvolution { get; set; }
        public PetRarity Rarity { get; set; }
        public PetStats BaseStats { get; set; } = new();
        public List<string> Abilities { get; set; } = new();
        public EvolutionReqs? EvolutionRequirements { get; set; }
        public string? Passive { get; set; }
    }
    
    public class PetStats
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public int MagicPower { get; set; }
    }
    
    public class EvolutionReqs
    {
        public int Level { get; set; }
        public int Bond { get; set; }
        public int Kills { get; set; }
        public int BossKills { get; set; }
    }
}
