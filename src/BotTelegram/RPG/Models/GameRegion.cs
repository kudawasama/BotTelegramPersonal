namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Región del mundo (continente o gran área geográfica)
    /// </summary>
    public class GameRegion
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "🗺️";
        
        /// <summary>
        /// Nivel mínimo recomendado para explorar esta región
        /// </summary>
        public int MinLevel { get; set; }
        
        /// <summary>
        /// Nivel máximo útil (después de esto, los enemigos dan poco XP)
        /// </summary>
        public int MaxLevel { get; set; }
        
        /// <summary>
        /// IDs de las zonas que pertenecen a esta región
        /// </summary>
        public List<string> ZoneIds { get; set; } = new();
        
        /// <summary>
        /// Zonas iniciales (donde el jugador puede empezar)
        /// </summary>
        public List<string> StartingZones { get; set; } = new();
    }
}
