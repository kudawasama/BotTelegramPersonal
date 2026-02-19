using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public static class NPCDatabase
    {
        private static readonly List<NPC> _npcs = new();
        
        static NPCDatabase()
        {
            _npcs.Add(new NPC
            {
                Id = "marina_guardian",
                Name = "Marina la Guardiana",
                Emoji = "🛡️",
                Description = "Líder de los Guardianes del Amanecer en Puerto Esperanza.",
                ZoneId = "puerto_esperanza",
                FactionId = "guardianes_amanecer",
                Type = NPCType.FactionLeader,
                RequiredReputation = 0
            });
            
            _npcs.Add(new NPC
            {
                Id = "borek_hunter",
                Name = "Borek el Cazador",
                Emoji = "🏹",
                Description = "Veterano explorador de las Tierras Salvajes.",
                ZoneId = "camino_viejo",
                FactionId = "viajeros_salvajes",
                Type = NPCType.Merchant,
                RequiredReputation = 0
            });
            
            _npcs.Add(new NPC
            {
                Id = "ignatius_flame",
                Name = "Ignatius",
                Emoji = "🔥",
                Description = "Gran Maestre de la Orden de la Llama Eterna.",
                ZoneId = "templo_fuego",
                FactionId = "orden_llama",
                Type = NPCType.FactionLeader,
                RequiredReputation = 1000
            });
        }
        
        public static List<NPC> GetAllNPCs() => _npcs;
        public static NPC? GetNPC(string npcId) => _npcs.FirstOrDefault(n => n.Id == npcId);
        public static List<NPC> GetNPCsInZone(string zoneId) => _npcs.Where(n => n.ZoneId == zoneId).ToList();
    }
}
