namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Estados posibles del jugador en el juego.
    /// FASE 6: Máquina de Estados Finita (FSM)
    /// </summary>
    public enum GameState
    {
        Idle,            // Menú principal / libre
        Exploring,       // Explorando zona activamente
        InCombat,        // En combate activo
        InDungeon,       // Dentro de una mazmorra (entre pisos)
        InDungeonCombat, // Combate dentro de mazmorra
        Shopping,        // En tienda/mercado
        Resting,         // Descansando en posada
        TravelMenu,      // Viendo mapa / seleccionando destino
        PetManagement,   // Gestionando mascotas
        SkillsMenu,      // Viendo habilidades
        ClassMenu,       // Gestionando clases
        Crafting         // Creando/ mejorando items (futuro)
    }
    
    /// <summary>
    /// Datos de estado del jugador para la FSM
    /// </summary>
    public class PlayerStateData
    {
        public GameState CurrentState { get; set; } = GameState.Idle;
        public DateTime StateChangedAt { get; set; } = DateTime.UtcNow;
        public string? StateContext { get; set; } // Datos adicionales del estado (ej: ID de mazmorra)
        
        /// <summary>
        /// Devuelve un string legible del estado actual
        /// </summary>
        public string GetDisplayName() => CurrentState switch
        {
            GameState.Idle            => "🏠 En el Menú",
            GameState.Exploring       => "🗺️ Explorando",
            GameState.InCombat        => "⚔️ En Combate",
            GameState.InDungeon       => "🏰 En Mazmorra",
            GameState.InDungeonCombat => "💀 Combate en Mazmorra",
            GameState.Shopping        => "🛒 En Tienda",
            GameState.Resting         => "😴 Descansando",
            GameState.TravelMenu      => "🗺️ Viajando",
            GameState.PetManagement   => "🐾 Mascotas",
            GameState.SkillsMenu      => "✨ Skills",
            GameState.ClassMenu       => "🎭 Clases",
            GameState.Crafting        => "⚒️ Elaborando",
            _ => "❓ Desconocido"
        };
    }
}
