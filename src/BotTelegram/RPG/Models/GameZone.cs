namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Zona específica dentro de una región (bosque, cueva, ciudad, etc)
    /// </summary>
    public class GameZone
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "📍";
        
        /// <summary>
        /// ID de la región a la que pertenece
        /// </summary>
        public string RegionId { get; set; } = "";
        
        /// <summary>
        /// Nivel mínimo de enemigos en esta zona
        /// </summary>
        public int MinEnemyLevel { get; set; }
        
        /// <summary>
        /// Nivel máximo de enemigos en esta zona
        /// </summary>
        public int MaxEnemyLevel { get; set; }
        
        /// <summary>
        /// Tasa de encuentro al explorar (0.0 a 1.0)
        /// 0.5 = 50% de encontrar enemigo al explorar
        /// </summary>
        public double EncounterRate { get; set; } = 0.5;
        
        /// <summary>
        /// IDs de enemigos que pueden aparecer en esta zona
        /// </summary>
        public List<string> EnemyPool { get; set; } = new();
        
        /// <summary>
        /// IDs de zonas conectadas (puedes viajar a estas zonas desde aquí)
        /// </summary>
        public List<string> ConnectedZones { get; set; } = new();
        
        /// <summary>
        /// Requisito de nivel para entrar (0 = sin requisito)
        /// </summary>
        public int LevelRequirement { get; set; } = 0;
        
        /// <summary>
        /// ¿Es una zona de inicio? (jugadores nuevos empiezan aquí)
        /// </summary>
        public bool IsStartingZone { get; set; } = false;
        
        /// <summary>
        /// ¿Es una zona segura? (sin encuentros aleatorios)
        /// </summary>
        public bool IsSafeZone { get; set; } = false;
        
        /// <summary>
        /// Tipo de zona (para mecánicas especiales)
        /// </summary>
        public ZoneType Type { get; set; } = ZoneType.Normal;
    }
    
    public enum ZoneType
    {
        Normal,          // Zona estándar con encuentros normales
        Town,            // Ciudad/Pueblo (zona segura)
        Dungeon,         // Mazmorra (encuentros más frecuentes)
        Boss,            // Zona de jefe (un enemigo especial)
        Resource,        // Zona de recursos (minería, herbolaría)
        PvP              // Zona PvP (jugadores pueden atacarse)
    }
}
