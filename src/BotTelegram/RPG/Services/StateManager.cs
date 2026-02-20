using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Máquina de Estados Finita (FSM) para gestionar el estado del jugador.
    /// FASE 6: Controla qué acciones están disponibles según el estado actual
    /// y valida las transiciones entre estados.
    /// </summary>
    public class StateManager
    {
        // ══════════════════════════════════════════════════════════════
        // TRANSICIONES VÁLIDAS ENTRE ESTADOS
        // ══════════════════════════════════════════════════════════════
        
        private static readonly Dictionary<GameState, HashSet<GameState>> ValidTransitions = new()
        {
            [GameState.Idle] = new()
            {
                GameState.Exploring, GameState.InDungeon, GameState.Shopping,
                GameState.Resting, GameState.TravelMenu, GameState.PetManagement,
                GameState.SkillsMenu, GameState.ClassMenu, GameState.InCombat
            },
            [GameState.Exploring] = new()
            {
                GameState.Idle, GameState.InCombat, GameState.TravelMenu
            },
            [GameState.InCombat] = new()
            {
                GameState.Idle, GameState.Exploring, GameState.InDungeonCombat
            },
            [GameState.InDungeon] = new()
            {
                GameState.Idle, GameState.InDungeonCombat
            },
            [GameState.InDungeonCombat] = new()
            {
                GameState.InDungeon, GameState.Idle
            },
            [GameState.Shopping] = new()
            {
                GameState.Idle
            },
            [GameState.Resting] = new()
            {
                GameState.Idle
            },
            [GameState.TravelMenu] = new()
            {
                GameState.Idle, GameState.Exploring
            },
            [GameState.PetManagement] = new()
            {
                GameState.Idle
            },
            [GameState.SkillsMenu] = new()
            {
                GameState.Idle, GameState.InCombat
            },
            [GameState.ClassMenu] = new()
            {
                GameState.Idle
            },
            [GameState.Crafting] = new()
            {
                GameState.Idle
            }
        };
        
        // ══════════════════════════════════════════════════════════════
        // ACCIONES PERMITIDAS POR ESTADO
        // Prefijos/IDs de callbacks válidos en cada estado
        // ══════════════════════════════════════════════════════════════
        
        private static readonly Dictionary<GameState, HashSet<string>> AllowedCallbackPrefixes = new()
        {
            [GameState.Idle] = new()
            {
                // MENÚ PRINCIPAL Y ESTADÍSTICAS
                "rpg_main", "rpg_menu_", "rpg_stats", "rpg_inventory", "rpg_skills",
                // MUNDO: Exploración y viajes
                "rpg_explore", "rpg_map", "rpg_travel_", "rpg_dungeon", 
                // MUNDO: Acciones cotidianas
                "rpg_rest", "rpg_work", 
                // MUNDO: Personaje y sistemas
                "rpg_pets", "rpg_counters", "rpg_passives", "rpg_combo",
                // MUNDO: Lore y tutoriales
                "rpg_adventure", "rpg_lore", "rpg_tutorial", "rpg_hidden",
                // SISTEMAS PRINCIPALES
                "classes_menu", "class_", "dungeon_main", "dungeon_keys",
                "leaderboard_", "pets_", 
                // SISTEMAS DE RECURSOS
                "shop_", "craft_", "quest_", "inv_", "train_",
                // OTROS
                "start"
            },
            [GameState.InCombat] = new()
            {
                "combat_", "rpg_combat_", "rpg_skills", "rpg_inventory",
                "rpg_main" // Para volver al menú (si escapa)
            },
            [GameState.InDungeon] = new()
            {
                "dungeon_", "rpg_inventory", "rpg_main"
            },
            [GameState.InDungeonCombat] = new()
            {
                "combat_", "rpg_combat_", "rpg_skills", "rpg_inventory"
            },
            [GameState.Exploring] = new()
            {
                "rpg_explore", "rpg_map", "rpg_travel_", "rpg_main"
            },
            [GameState.Shopping] = new()
            {
                "shop_", "rpg_main"
            },
            [GameState.Resting] = new()
            {
                "rest_", "rpg_main"
            },
            [GameState.TravelMenu] = new()
            {
                "rpg_travel_", "rpg_map", "rpg_main"
            },
            [GameState.PetManagement] = new()
            {
                "pets_", "rpg_main"
            },
            [GameState.SkillsMenu] = new()
            {
                "rpg_skills", "rpg_main"
            },
            [GameState.ClassMenu] = new()
            {
                "classes_menu", "class_", "rpg_main"
            },
            [GameState.Crafting] = new()
            {
                "craft_", "rpg_main"
            }
        };
        
        // ══════════════════════════════════════════════════════════════
        // API PÚBLICA
        // ══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Determina el estado correcto del jugador basado en sus propiedades actuales.
        /// Útil para sincronizar el estado en jugadores existentes sin PlayerStateData.
        /// </summary>
        public static GameState DeriveStateFromPlayer(RpgPlayer player)
        {
            if (player.IsInCombat)
            {
                return (player.CurrentDungeon != null && player.CurrentDungeon.IsActive)
                    ? GameState.InDungeonCombat
                    : GameState.InCombat;
            }
            
            if (player.CurrentDungeon != null && player.CurrentDungeon.IsActive)
                return GameState.InDungeon;
            
            return GameState.Idle;
        }
        
        /// <summary>
        /// Sincroniza el estado de la FSM con la realidad del jugador.
        /// Llama esto al inicio de cada interacción para garantizar consistencia.
        /// </summary>
        public static void SyncState(RpgPlayer player)
        {
            var derivedState = DeriveStateFromPlayer(player);
            
            if (player.PlayerState == null)
                player.PlayerState = new PlayerStateData();
            
            // Solo actualizar si hay discrepancia grave
            bool stateOutOfSync =
                (derivedState == GameState.InCombat && player.PlayerState.CurrentState != GameState.InCombat && player.PlayerState.CurrentState != GameState.InDungeonCombat) ||
                (derivedState == GameState.InDungeon && player.PlayerState.CurrentState != GameState.InDungeon && player.PlayerState.CurrentState != GameState.InDungeonCombat) ||
                (derivedState == GameState.Idle && (player.PlayerState.CurrentState == GameState.InCombat || player.PlayerState.CurrentState == GameState.InDungeonCombat));
            
            if (stateOutOfSync)
            {
                player.PlayerState.CurrentState = derivedState;
                player.PlayerState.StateChangedAt = DateTime.UtcNow;
                Console.WriteLine($"[FSM] Estado sincronizado para {player.Name}: {derivedState}");
            }
        }
        
        /// <summary>
        /// Verifica si la acción (callback ID o prefijo) está permitida en el estado actual.
        /// </summary>
        public static bool IsActionAllowed(RpgPlayer player, string callbackData)
        {
            if (player.PlayerState == null) return true; // Sin FSM, permitir todo
            
            var state = player.PlayerState.CurrentState;
            
            if (!AllowedCallbackPrefixes.TryGetValue(state, out var allowed))
                return true; // Estado no configurado, permitir todo
            
            // Verificar si algún prefijo válido existe en el callback
            foreach (var prefix in allowed)
            {
                if (callbackData.StartsWith(prefix) || callbackData == prefix)
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Intenta transicionar al jugador a un nuevo estado.
        /// Devuelve true si la transición fue exitosa.
        /// </summary>
        public static bool TransitionTo(RpgPlayer player, GameState newState, string? context = null)
        {
            if (player.PlayerState == null)
                player.PlayerState = new PlayerStateData();
            
            var currentState = player.PlayerState.CurrentState;
            
            if (!ValidTransitions.TryGetValue(currentState, out var validTargets))
            {
                // Estado no configurado, permitir cualquier transición
                SetState(player, newState, context);
                return true;
            }
            
            if (!validTargets.Contains(newState))
            {
                Console.WriteLine($"[FSM] Transición inválida: {currentState} → {newState} para {player.Name}");
                return false;
            }
            
            SetState(player, newState, context);
            return true;
        }
        
        /// <summary>
        /// Fuerza un cambio de estado (sin validar transición).
        /// Usar cuando se necesita restablecer el estado.
        /// </summary>
        public static void ForceState(RpgPlayer player, GameState newState, string? context = null)
        {
            SetState(player, newState, context);
        }
        
        /// <summary>
        /// Retorna el estado actual del jugador (o Idle si no tiene FSM inicializada)
        /// </summary>
        public static GameState GetCurrentState(RpgPlayer player)
        {
            return player.PlayerState?.CurrentState ?? GameState.Idle;
        }
        
        /// <summary>
        /// Obtiene el nombre descriptivo del estado actual
        /// </summary>
        public static string GetStateDisplay(RpgPlayer player)
        {
            return player.PlayerState?.GetDisplayName() ?? "🏠 En el Menú";
        }
        
        // ══════════════════════════════════════════════════════════════
        // HELPERS PRIVADOS
        // ══════════════════════════════════════════════════════════════
        
        private static void SetState(RpgPlayer player, GameState newState, string? context)
        {
            if (player.PlayerState == null)
                player.PlayerState = new PlayerStateData();
            
            var oldState = player.PlayerState.CurrentState;
            player.PlayerState.CurrentState = newState;
            player.PlayerState.StateChangedAt = DateTime.UtcNow;
            player.PlayerState.StateContext = context;
            
            Console.WriteLine($"[FSM] {player.Name}: {oldState} → {newState}" + 
                              (context != null ? $" (ctx: {context})" : ""));
        }
    }
}
