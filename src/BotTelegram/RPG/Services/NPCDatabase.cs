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
            
            _npcs.Add(new NPC
            {
                Id = "elder_thorn",
                Name = "Anciano Thorn",
                Emoji = "🌳",
                Description = "Archidruida del Bosque Eterno.",
                ZoneId = "corazon_bosque",
                FactionId = "druidas_eternos",
                Type = NPCType.FactionLeader,
                RequiredReputation = 3000
            });
            
            _npcs.Add(new NPC
            {
                Id = "serina_merchant",
                Name = "Serina",
                Emoji = "🏪",
                Description = "Comerciante de Puerto Esperanza.",
                ZoneId = "puerto_esperanza",
                FactionId = "guardianes_amanecer",
                Type = NPCType.Merchant,
                RequiredReputation = 0
            });
            
            _npcs.Add(new NPC
            {
                Id = "garrus_blacksmith",
                Name = "Garrus el Herrero",
                Emoji = "⚒️",
                Description = "Maestro herrero especializado en armas.",
                ZoneId = "puerto_esperanza",
                FactionId = null,
                Type = NPCType.Blacksmith,
                RequiredReputation = -10000
            });
            
            _npcs.Add(new NPC
            {
                Id = "lysandra_innkeeper",
                Name = "Lysandra",
                Emoji = "🏨",
                Description = "Dueña de la Posada del Amanecer.",
                ZoneId = "puerto_esperanza",
                FactionId = null,
                Type = NPCType.Innkeeper,
                RequiredReputation = -10000
            });
            
            _npcs.Add(new NPC
            {
                Id = "kael_shadow",
                Name = "Kael",
                Emoji = "🗡️",
                Description = "Agente de la Hermandad de Sombras. Habla en susurros.",
                ZoneId = "campamento_bandidos",
                FactionId = "hermandad_sombras",
                Type = NPCType.QuestGiver,
                RequiredReputation = 1000
            });
            
            _npcs.Add(new NPC
            {
                Id = "azura_desert",
                Name = "Azura",
                Emoji = "🏜️",
                Description = "Guardiana del oasis y líder de los Custodios.",
                ZoneId = "oasis_espejismo",
                FactionId = "custodios_desierto",
                Type = NPCType.FactionLeader,
                RequiredReputation = 1000
            });
            
            _npcs.Add(new NPC
            {
                Id = "frostblade_commander",
                Name = "Comandante Frostblade",
                Emoji = "❄️",
                Description = "Alto comandante de la Legión del Hielo.",
                ZoneId = "fortaleza_hielo",
                FactionId = "legion_hielo",
                Type = NPCType.FactionLeader,
                RequiredReputation = 3000
            });
            
            _npcs.Add(new NPC
            {
                Id = "morgath_cultist",
                Name = "Morgath",
                Emoji = "🕳️",
                Description = "Sumo sacerdote del Culto del Abismo.",
                ZoneId = "ciudadela_demonio",
                FactionId = "culto_abismo",
                Type = NPCType.FactionLeader,
                RequiredReputation = 3000
            });
            
            _npcs.Add(new NPC
            {
                Id = "seraphine_celestial",
                Name = "Seraphine",
                Emoji = "☁️",
                Description = "Ángel caída, líder de los Centinelas.",
                ZoneId = "jardines_celestiales",
                FactionId = "centinelas_cielo",
                Type = NPCType.FactionLeader,
                RequiredReputation = 6000
            });
            
            _npcs.Add(new NPC
            {
                Id = "rolf_trainer",
                Name = "Rolf el Maestro",
                Emoji = "⚔️",
                Description = "Entrenador de combate en Puerto Esperanza.",
                ZoneId = "puerto_esperanza",
                FactionId = null,
                Type = NPCType.Trainer,
                RequiredReputation = -10000
            });
        }
        
        public static List<NPC> GetAllNPCs() => _npcs;
        public static NPC? GetNPC(string npcId) => _npcs.FirstOrDefault(n => n.Id == npcId);
        public static List<NPC> GetNPCsInZone(string zoneId) => _npcs.Where(n => n.ZoneId == zoneId).ToList();
    }
}
