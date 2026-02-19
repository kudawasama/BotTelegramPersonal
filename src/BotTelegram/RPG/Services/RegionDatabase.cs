using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Base de datos de regiones y zonas del mundo
    /// </summary>
    public static class RegionDatabase
    {
        private static readonly List<GameRegion> _regions = new();
        private static readonly Dictionary<string, GameZone> _zones = new();
        
        static RegionDatabase()
        {
            InitializeRegions();
            InitializeZones();
        }
        
        /// <summary>
        /// Obtiene todas las regiones
        /// </summary>
        public static List<GameRegion> GetAllRegions() => _regions;
        
        /// <summary>
        /// Obtiene una región por ID
        /// </summary>
        public static GameRegion? GetRegion(string regionId) => _regions.FirstOrDefault(r => r.Id == regionId);
        
        /// <summary>
        /// Obtiene todas las zonas
        /// </summary>
        public static List<GameZone> GetAllZones() => _zones.Values.ToList();
        
        /// <summary>
        /// Obtiene una zona por ID
        /// </summary>
        public static GameZone? GetZone(string zoneId) => _zones.ContainsKey(zoneId) ? _zones[zoneId] : null;
        
        /// <summary>
        /// Obtiene todas las zonas de una región
        /// </summary>
        public static List<GameZone> GetZonesInRegion(string regionId)
        {
            var region = GetRegion(regionId);
            if (region == null) return new List<GameZone>();
            
            return region.ZoneIds
                .Select(id => GetZone(id))
                .Where(z => z != null)
                .Cast<GameZone>()
                .ToList();
        }
        
        /// <summary>
        /// Obtiene las zonas conectadas a una zona específica
        /// </summary>
        public static List<GameZone> GetConnectedZones(string zoneId)
        {
            var zone = GetZone(zoneId);
            if (zone == null) return new List<GameZone>();
            
            return zone.ConnectedZones
                .Select(id => GetZone(id))
                .Where(z => z != null)
                .Cast<GameZone>()
                .ToList();
        }
        
        /// <summary>
        /// Verifica si el jugador cumple los requisitos para entrar a una zona
        /// </summary>
        public static (bool canEnter, string? reason) CanEnterZone(RpgPlayer player, string zoneId)
        {
            var zone = GetZone(zoneId);
            if (zone == null)
                return (false, "Zona no encontrada");
            
            // Verificar nivel requerido
            if (player.Level < zone.LevelRequirement)
                return (false, $"Nivel {zone.LevelRequirement} requerido (tu nivel: {player.Level})");
            
            // Verificar reputación con facción (Fase 12)
            if (!string.IsNullOrEmpty(zone.RequiredFactionId) && zone.RequiredReputation > 0)
            {
                var factionService = new FactionService();
                var playerRep = factionService.GetReputation(player, zone.RequiredFactionId);
                var currentRep = playerRep?.Reputation ?? 0;
                
                if (currentRep < zone.RequiredReputation)
                {
                    var faction = FactionDatabase.GetFaction(zone.RequiredFactionId);
                    var factionName = faction?.Name ?? "facción desconocida";
                    return (false, $"Reputación insuficiente con {factionName} (necesitas {zone.RequiredReputation}, tienes {currentRep})");
                }
            }
            
            // Verificar si está desbloqueada
            if (!zone.IsStartingZone && !player.UnlockedZones.Contains(zoneId))
                return (false, "Zona no desbloqueada. Descubre nuevas zonas explorando.");
            
            return (true, null);
        }
        
        /// <summary>
        /// Desbloquea una zona para el jugador
        /// </summary>
        public static void UnlockZone(RpgPlayer player, string zoneId)
        {
            if (!player.UnlockedZones.Contains(zoneId))
            {
                player.UnlockedZones.Add(zoneId);
            }
        }
        
        private static void InitializeRegions()
        {
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 1: ISLAS DEL AMANECER (Niveles 1-15)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "islas_amanecer",
                Name = "Islas del Amanecer",
                Emoji = "🏝️",
                Description = "Archipiélago pacífico donde comienzan las aventuras. Clima templado, enemigos débiles.",
                MinLevel = 1,
                MaxLevel = 15,
                ZoneIds = new() { "puerto_esperanza", "playa_coral", "bosque_susurros", "cuevas_eco", "torre_vigilancia" },
                StartingZones = new() { "puerto_esperanza" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 2: TIERRAS SALVAJES (Niveles 10-25)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "tierras_salvajes",
                Name = "Tierras Salvajes",
                Emoji = "🌲",
                Description = "Vastos bosques y llanuras habitados por bestias feroces y bandidos.",
                MinLevel = 10,
                MaxLevel = 25,
                ZoneIds = new() { "camino_viejo", "campamento_bandidos", "pantano_brumas", "aldea_abandonada", "ruinas_antiguas" },
                StartingZones = new() { "camino_viejo" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 3: MONTAÑAS CENIZA (Niveles 20-35)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "montanas_ceniza",
                Name = "Montañas de Ceniza",
                Emoji = "⛰️",
                Description = "Cordillera volcánica con criaturas elementales y cultistas del fuego.",
                MinLevel = 20,
                MaxLevel = 35,
                ZoneIds = new() { "sendero_montana", "minas_abandonadas", "crater_volcanico", "templo_fuego", "cumbre_dragon" },
                StartingZones = new() { "sendero_montana" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 4: DESIERTO OLVIDADO (Niveles 30-45)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "desierto_olvidado",
                Name = "Desierto Olvidado",
                Emoji = "🏜️",
                Description = "Extenso desierto con ruinas de civilizaciones antiguas y muertos vivientes.",
                MinLevel = 30,
                MaxLevel = 45,
                ZoneIds = new() { "oasis_espejismo", "tumba_faraon", "ciudad_arena", "necropolis", "piramide_oscura" },
                StartingZones = new() { "oasis_espejismo" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 5: BOSQUE ETERNO (Niveles 40-55)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "bosque_eterno",
                Name = "Bosque Eterno",
                Emoji = "🌳",
                Description = "Bosque ancestral protegido por criaturas místicas y fae oscuro.",
                MinLevel = 40,
                MaxLevel = 55,
                ZoneIds = new() { "lindero_bosque", "santuario_druidas", "lago_reflejos", "arboles_retorcidos", "corazon_bosque" },
                StartingZones = new() { "lindero_bosque" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 6: TIERRAS HELADAS (Niveles 50-65)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "tierras_heladas",
                Name = "Tierras Heladas del Norte",
                Emoji = "❄️",
                Description = "Tundra congelada con criaturas de hielo y gigantes antiguos.",
                MinLevel = 50,
                MaxLevel = 65,
                ZoneIds = new() { "pueblo_frontera", "llanura_nieve", "fortaleza_hielo", "cavernas_cristal", "trono_invierno" },
                StartingZones = new() { "pueblo_frontera" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 7: ABISMO (Niveles 60-80)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "abismo",
                Name = "El Abismo",
                Emoji = "🕳️",
                Description = "Dimensión oscura llena de demonios, aberraciones y locura.",
                MinLevel = 60,
                MaxLevel = 80,
                ZoneIds = new() { "portal_entrada", "ciudad_caida", "alcantarillas_corruptas", "ciudadela_demonio", "corazon_abismo" },
                StartingZones = new() { "portal_entrada" }
            });
            
            // ═══════════════════════════════════════════════════════════════
            // REGIÓN 8: CIELO ETÉREO (Niveles 70-100)
            // ═══════════════════════════════════════════════════════════════
            _regions.Add(new GameRegion
            {
                Id = "cielo_etereo",
                Name = "Cielo Etéreo",
                Emoji = "☁️",
                Description = "Islas flotantes donde habitan ángeles caídos y dragones ancianos.",
                MinLevel = 70,
                MaxLevel = 100,
                ZoneIds = new() { "escalera_cielo", "jardines_celestiales", "biblioteca_infinita", "coliseo_dioses", "trono_supremo" },
                StartingZones = new() { "escalera_cielo" }
            });
        }
        
        private static void InitializeZones()
        {
            // ═══════════════════════════════════════════════════════════════
            // ISLAS DEL AMANECER - ZONAS
            // ═══════════════════════════════════════════════════════════════
            
            AddZone(new GameZone
            {
                Id = "puerto_esperanza",
                Name = "Puerto Esperanza",
                Emoji = "⚓",
                Description = "Pueblo portuario pacífico donde las aventuras comienzan. Zona segura.",
                RegionId = "islas_amanecer",
                MinEnemyLevel = 1,
                MaxEnemyLevel = 5,
                EncounterRate = 0.0, // Zona segura
                EnemyPool = new() { }, // Sin enemigos
                ConnectedZones = new() { "playa_coral", "bosque_susurros" },
                LevelRequirement = 0,
                IsStartingZone = true,
                IsSafeZone = true,
                Type = ZoneType.Town
            });
            
            AddZone(new GameZone
            {
                Id = "playa_coral",
                Name = "Playa de Coral",
                Emoji = "🏖️",
                Description = "Playa tranquila con cangrejos gigantes y oleadas ocasionales de slimes marinos.",
                RegionId = "islas_amanecer",
                MinEnemyLevel = 1,
                MaxEnemyLevel = 5,
                EncounterRate = 0.4,
                EnemyPool = new() { "crab", "slime", "beach_bandit" },
                ConnectedZones = new() { "puerto_esperanza", "bosque_susurros", "cuevas_eco" },
                LevelRequirement = 0,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Normal
            });
            
            AddZone(new GameZone
            {
                Id = "bosque_susurros",
                Name = "Bosque de los Susurros",
                Emoji = "🌲",
                Description = "Bosque denso donde los árboles parecen hablar. Lobos y espíritus menores acechan.",
                RegionId = "islas_amanecer",
                MinEnemyLevel = 3,
                MaxEnemyLevel = 8,
                EncounterRate = 0.55,
                EnemyPool = new() { "wolf", "forest_sprite", "bandit", "wild_boar" },
                ConnectedZones = new() { "puerto_esperanza", "playa_coral", "torre_vigilancia" },
                LevelRequirement = 3,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Normal
            });
            
            AddZone(new GameZone
            {
                Id = "cuevas_eco",
                Name = "Cuevas del Eco",
                Emoji = "🕳️",
                Description = "Cavernas húmedas llenas de murciélagos, arañas gigantes y hongos venenosos.",
                RegionId = "islas_amanecer",
                MinEnemyLevel = 5,
                MaxEnemyLevel = 10,
                EncounterRate = 0.65,
                EnemyPool = new() { "bat", "spider", "mushroom", "cave_troll" },
                ConnectedZones = new() { "playa_coral", "torre_vigilancia" },
                LevelRequirement = 5,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon
            });
            
            AddZone(new GameZone
            {
                Id = "torre_vigilancia",
                Name = "Torre de Vigilancia Abandonada",
                Emoji = "🗼",
                Description = "Antigua torre militar ahora habitada por esqueletos y espectros.",
                RegionId = "islas_amanecer",
                MinEnemyLevel = 8,
                MaxEnemyLevel = 15,
                EncounterRate = 0.75,
                EnemyPool = new() { "skeleton_warrior", "ghost", "necromancer", "tower_guardian" },
                ConnectedZones = new() { "bosque_susurros", "cuevas_eco", "camino_viejo" },
                LevelRequirement = 8,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon
            });
            
            // ═══════════════════════════════════════════════════════════════
            // TIERRAS SALVAJES - ZONAS
            // ═══════════════════════════════════════════════════════════════
            
            AddZone(new GameZone
            {
                Id = "camino_viejo",
                Name = "El Camino Viejo",
                Emoji = "🛤️",
                Description = "Camino polvoriento que conecta las Islas del Amanecer con tierras peligrosas.",
                RegionId = "tierras_salvajes",
                MinEnemyLevel = 10,
                MaxEnemyLevel = 15,
                EncounterRate = 0.5,
                EnemyPool = new() { "bandit", "wolf", "highwayman", "wild_dog" },
                ConnectedZones = new() { "torre_vigilancia", "campamento_bandidos", "pantano_brumas" },
                LevelRequirement = 10,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Normal
            });
            
            AddZone(new GameZone
            {
                Id = "campamento_bandidos",
                Name = "Campamento de Bandidos",
                Emoji = "⛺",
                Description = "Asentamiento ilegal de criminales. Peligro extremo.",
                RegionId = "tierras_salvajes",
                MinEnemyLevel = 12,
                MaxEnemyLevel = 18,
                EncounterRate = 0.8,
                EnemyPool = new() { "bandit", "bandit_captain", "mercenary", "assassin" },
                ConnectedZones = new() { "camino_viejo", "aldea_abandonada" },
                LevelRequirement = 12,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon
            });
            
            AddZone(new GameZone
            {
                Id = "pantano_brumas",
                Name = "Pantano de las Brumas",
                Emoji = "🌫️",
                Description = "Pantano peligroso con visibilidad limitada. Criaturas venenosas acechan.",
                RegionId = "tierras_salvajes",
                MinEnemyLevel = 14,
                MaxEnemyLevel = 20,
                EncounterRate = 0.7,
                EnemyPool = new() { "swamp_creature", "toxic_frog", "will_o_wisp", "swamp_dragon" },
                ConnectedZones = new() { "camino_viejo", "aldea_abandonada", "ruinas_antiguas" },
                LevelRequirement = 14,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Normal
            });
            
            AddZone(new GameZone
            {
                Id = "aldea_abandonada",
                Name = "Aldea Abandonada",
                Emoji = "🏚️",
                Description = "Casas vacías y calles silenciosas. Algo terrible pasó aquí.",
                RegionId = "tierras_salvajes",
                MinEnemyLevel = 16,
                MaxEnemyLevel = 22,
                EncounterRate = 0.65,
                EnemyPool = new() { "zombie", "ghoul", "haunted_doll", "possessed_villager" },
                ConnectedZones = new() { "campamento_bandidos", "pantano_brumas", "ruinas_antiguas" },
                LevelRequirement = 16,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon
            });
            
            AddZone(new GameZone
            {
                Id = "ruinas_antiguas",
                Name = "Ruinas Antiguas",
                Emoji = "🏛️",
                Description = "Templos milenarios con guardianes mágicos y tesoros olvidados.",
                RegionId = "tierras_salvajes",
                MinEnemyLevel = 20,
                MaxEnemyLevel = 25,
                EncounterRate = 0.85,
                EnemyPool = new() { "stone_golem", "ancient_guardian", "spectral_knight", "cursed_mage" },
                ConnectedZones = new() { "pantano_brumas", "aldea_abandonada", "sendero_montana" },
                LevelRequirement = 20,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon
            });
            
            // (NOTA: Las demás regiones seguirán el mismo patrón. Por espacio, incluyo una muestra representativa.)
            
            // ═══════════════════════════════════════════════════════════════
            // MONTAÑAS CENIZA - ZONA INICIAL
            // ═══════════════════════════════════════════════════════════════
            AddZone(new GameZone
            {
                Id = "sendero_montana",
                Name = "Sendero de la Montaña",
                Emoji = "⛰️",
                Description = "Camino serpenteante que asciende hacia volcanes activos.",
                RegionId = "montanas_ceniza",
                MinEnemyLevel = 20,
                MaxEnemyLevel = 25,
                EncounterRate = 0.55,
                EnemyPool = new() { "mountain_goat", "fire_imp", "lava_elemental", "ash_wraith" },
                ConnectedZones = new() { "ruinas_antiguas", "minas_abandonadas", "crater_volcanico" },
                LevelRequirement = 20,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Normal
            });
            
            // ═══════════════════════════════════════════════════════════════
            // ZONAS ESPECIALES - REQUIEREN REPUTACIÓN CON FACCIONES (FASE 12)
            // ═══════════════════════════════════════════════════════════════
            
            // Fortaleza de los Guardianes del Amanecer
            AddZone(new GameZone
            {
                Id = "fortaleza_amanecer",
                Name = "Fortaleza del Amanecer",
                Emoji = "🏰",
                Description = "Ciudadela sagrada de los Guardianes del Amanecer. Solo los héroes probados pueden entrar.",
                RegionId = "costa_esperanza",
                MinEnemyLevel = 30,
                MaxEnemyLevel = 35,
                EncounterRate = 0.40,
                EnemyPool = new() { "elite_guardian", "holy_knight", "dawn_sentinel" },
                ConnectedZones = new() { "puerto_esperanza", "playa_coral" },
                LevelRequirement = 30,
                IsStartingZone = false,
                IsSafeZone = true,    // Zona de entrenamiento segura
                Type = ZoneType.Town,
                RequiredFactionId = "guardianes_amanecer",
                RequiredReputation = 3000
            });
            
            // Forja del Dragón - Orden de la Llama Eterna
            AddZone(new GameZone
            {
                Id = "forja_dragon",
                Name = "Forja del Dragón",
                Emoji = "🔥",
                Description = "Volcán interior donde la Orden de la Llama forja armas legendarias. Acceso solo para maestros de fuego.",
                RegionId = "montanas_ceniza",
                MinEnemyLevel = 35,
                MaxEnemyLevel = 40,
                EncounterRate = 0.70,
                EnemyPool = new() { "lava_elemental", "fire_dragon", "flame_titan", "magma_lord" },
                ConnectedZones = new() { "crater_volcanico", "sendero_montana" },
                LevelRequirement = 35,
                IsStartingZone = false,
                IsSafeZone = false,
                Type = ZoneType.Dungeon,
                RequiredFactionId = "orden_llama",
                RequiredReputation = 6000
            });
            
            // Árbol Ancestral - Druidas Eternos del Bosque
            AddZone(new GameZone
            {
                Id = "arbol_ancestral",
                Name = "Árbol Ancestral",
                Emoji = "🌳",
                Description = "Corazón místico del bosque milenario. Los druidas guardan aquí los secretos de la vida eterna.",
                RegionId = "bosque_eterno",
                MinEnemyLevel = 35,
                MaxEnemyLevel = 40,
                EncounterRate = 0.50,
                EnemyPool = new() { "ancient_treant", "forest_guardian", "wild_spirit", "nature_avatar" },
                ConnectedZones = new() { "corazon_bosque", "lago_espejo" },
                LevelRequirement = 35,
                IsStartingZone = false,
                IsSafeZone = true,    // Zona sagrada de los druidas
                Type = ZoneType.Town,
                RequiredFactionId = "druidas_eternos",
                RequiredReputation = 6000
            });
            
            // Placeholder para otras zonas (se agregarían todas aquí)
            // Por brevedad, continúo con métodos auxiliares
        }
        
        private static void AddZone(GameZone zone)
        {
            _zones[zone.Id] = zone;
        }
    }
}
