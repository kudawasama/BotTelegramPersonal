using BotTelegram.RPG.Models;
using BotTelegram.RPG.Services;

namespace BotTelegram.Services
{
    /// <summary>
    /// Servicio de migración y actualización de personajes para nuevas versiones del bot.
    /// Actualiza personajes existentes con nuevas propiedades sin perder progreso.
    /// </summary>
    public class PlayerMigrationService
    {
        private readonly RpgService _rpgService;
        private const string CURRENT_SCHEMA_VERSION = "3.2.0"; // Actualizar con cada migración mayor
        
        public PlayerMigrationService()
        {
            _rpgService = new RpgService();
        }
        
        /// <summary>
        /// Actualiza un jugador existente con las nuevas propiedades introducidas en actualizaciones.
        /// Se ejecuta automáticamente al iniciar el bot con /start.
        /// </summary>
        public bool MigratePlayer(long chatId)
        {
            var player = _rpgService.GetPlayer(chatId);
            if (player == null)
                return false; // No hay jugador, no hay nada que migrar
            
            bool needsSave = false;
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIÓN v3.1.9: Sistema de Clases Fase 4
            // ═══════════════════════════════════════════════════════════════
            
            // Inicializar ActiveClassId si no existe
            if (string.IsNullOrEmpty(player.ActiveClassId))
            {
                player.ActiveClassId = GetClassIdFromEnum(player.Class);
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: ActiveClassId inicializado a '{player.ActiveClassId}'");
            }
            
            // Inicializar UnlockedClasses si está vacío
            if (player.UnlockedClasses == null || player.UnlockedClasses.Count == 0)
            {
                player.UnlockedClasses = new List<string> { player.ActiveClassId };
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: UnlockedClasses inicializado con clase actual");
            }
            
            // Inicializar ActionCounters si no existe
            if (player.ActionCounters == null)
            {
                player.ActionCounters = new Dictionary<string, int>();
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: ActionCounters inicializado");
            }
            
            // Inicializar UnlockedSkills si no existe
            if (player.UnlockedSkills == null)
            {
                player.UnlockedSkills = new List<string>();
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: UnlockedSkills inicializado");
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIÓN v3.2.0: Sistema de Facciones Fase 12
            // ═══════════════════════════════════════════════════════════════
            
            // Inicializar FactionReputations si no existe
            if (player.FactionReputations == null)
            {
                player.FactionReputations = new List<PlayerFactionReputation>();
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: FactionReputations inicializado");
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIÓN v3.0.0-3.1.0: Sistema de Guild + PvP
            // ═══════════════════════════════════════════════════════════════
            
            // Inicializar PvP rating si es 0 (default incorrecto, debería ser 1200)
            if (player.PvpRating == 0)
            {
                player.PvpRating = 1200;
                needsSave = true;
                Console.WriteLine($"[Migration] {player.Name}: PvpRating inicializado a 1200");
            }
            
            // GuildRole y GuildContribution ya tienen defaults correctos (GuildRole.Member, 0)
            // No necesitan migración
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIÓN v2.1.0: Sistema de Quests
            // ═══════════════════════════════════════════════════════════════
            
            if (player.ActiveQuests == null)
            {
                player.ActiveQuests = new List<PlayerQuest>();
                needsSave = true;
            }
            
            if (player.CompletedQuestIds == null)
            {
                player.CompletedQuestIds = new List<string>();
                needsSave = true;
            }
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIÓN v2.0.0: Sistema de Crafteo
            // ═══════════════════════════════════════════════════════════════
            
            // El sistema de crafteo no requiere migración de datos.
            // Las recetas se desbloquean automáticamente por nivel del jugador.
            
            // ═══════════════════════════════════════════════════════════════
            // MIGRACIONES FUTURAS SE AÑADEN AQUÍ
            // ═══════════════════════════════════════════════════════════════
            
            // TODO v3.2.0: Fase 12 - Mundo Abierto
            // TODO v4.0.0: Sistema de Imágenes
            // TODO v4.1.0: IA Narrativa
            
            // Guardar si hubo cambios
            if (needsSave)
            {
                _rpgService.SavePlayer(player);
                Console.WriteLine($"[Migration] ✅ {player.Name} actualizado a schema v{CURRENT_SCHEMA_VERSION}");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Obtiene el ClassId string desde el enum CharacterClass.
        /// Necesario para migrar jugadores antiguos que solo tenían el enum.
        /// </summary>
        private string GetClassIdFromEnum(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Adventurer => "adventurer",
                CharacterClass.Warrior => "warrior",
                CharacterClass.Mage => "mage",
                CharacterClass.Rogue => "rogue",
                CharacterClass.Cleric => "cleric",
                CharacterClass.Paladin => "paladin",
                CharacterClass.Ranger => "ranger",
                CharacterClass.Warlock => "warlock",
                CharacterClass.Monk => "monk",
                CharacterClass.Berserker => "berserker",
                CharacterClass.Assassin => "assassin",
                CharacterClass.Sorcerer => "sorcerer",
                CharacterClass.Druid => "druid",
                CharacterClass.Necromancer => "necromancer",
                CharacterClass.Bard => "bard",
                _ => "adventurer"
            };
        }
        
        /// <summary>
        /// Migra todos los jugadores existentes en batch.
        /// Útil para ejecutar manualmente después de una actualización mayor.
        /// </summary>
        public int MigrateAllPlayers()
        {
            var allPlayers = _rpgService.GetAllPlayers();
            int migratedCount = 0;
            
            Console.WriteLine($"[Migration] Iniciando migración de {allPlayers.Count} jugadores...");
            
            foreach (var player in allPlayers)
            {
                if (MigratePlayer(player.ChatId))
                    migratedCount++;
            }
            
            Console.WriteLine($"[Migration] ✅ {migratedCount}/{allPlayers.Count} jugadores actualizados");
            return migratedCount;
        }
    }
}
