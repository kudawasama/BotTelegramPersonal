using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public static class DialogueDatabase
    {
        private static readonly List<Dialogue> _dialogues = new();
        
        static DialogueDatabase()
        {
            InitializeDialogues();
        }
        
        public static List<Dialogue> GetAllDialogues() => _dialogues;
        public static Dialogue? GetDialogue(string dialogueId) => _dialogues.FirstOrDefault(d => d.Id == dialogueId);
        public static List<Dialogue> GetDialoguesForNPC(string npcId) => _dialogues.Where(d => d.NPCId == npcId).ToList();
        
        private static void InitializeDialogues()
        {
            // Marina la Guardiana - Greeting
            _dialogues.Add(new Dialogue
            {
                Id = "marina_greeting",
                NPCId = "marina_guardian",
                Text = "¡Bienvenido, aventurero! Soy Marina, líder de los Guardianes del Amanecer. Protegemos estas islas de las fuerzas oscuras.",
                Type = DialogueType.Greeting,
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "¿Qué son los Guardianes?", NextDialogueId = "marina_about" },
                    new DialogueOption { Id = "2", Text = "¿Cómo puedo ayudar?", NextDialogueId = "marina_help" },
                    new DialogueOption { Id = "3", Text = "Adiós." }
                }
            });
            
            _dialogues.Add(new Dialogue
            {
                Id = "marina_about",
                NPCId = "marina_guardian",
                Text = "Somos una orden dedicada a mantener la paz en las Islas del Amanecer. Combatimos criaturas oscuras y ayudamos a nuevos aventureros.",
                Type = DialogueType.Story,
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "Entiendo.", NextDialogueId = "marina_greeting" }
                }
            });
            
            _dialogues.Add(new Dialogue
            {
                Id = "marina_help",
                NPCId = "marina_guardian",
                Text = "Puedes ganar nuestra confianza derrotando criaturas malignas y completando misiones. A mayor reputación, mayores recompensas.",
                Type = DialogueType.Story,
                Reward = new DialogueReward { Reputation = 10, FactionId = "guardianes_amanecer" },
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "Gracias por la información." }
                }
            });
            
            // Garrus el Herrero
            _dialogues.Add(new Dialogue
            {
                Id = "garrus_greeting",
                NPCId = "garrus_blacksmith",
                Text = "¿Necesitas reparar tu equipo o forjar algo nuevo? Mi forja está a tu disposición.",
                Type = DialogueType.Greeting,
                Options = new()
                {
                    new DialogueOption 
                    { 
                        Id = "1", 
                        Text = "Ver servicios de herrería", 
                        Action = new DialogueAction { Type = DialogueActionType.OpenShop }
                    },
                    new DialogueOption { Id = "2", Text = "Tal vez después." }
                }
            });
            
            // Lysandra Posada
            _dialogues.Add(new Dialogue
            {
                Id = "lysandra_greeting",
                NPCId = "lysandra_innkeeper",
                Text = "¡Bienvenido a la Posada del Amanecer! ¿Necesitas descansar? Por 25 de oro restauro tu HP y Mana completamente.",
                Type = DialogueType.Greeting,
                Options = new()
                {
                    new DialogueOption 
                    { 
                        Id = "1", 
                        Text = "Descansar (25 oro)", 
                        Action = new DialogueAction { Type = DialogueActionType.TakeGold, Value = 25 }
                    },
                    new DialogueOption { Id = "2", Text = "No, gracias." }
                }
            });
            
            // Kael Shadow
            _dialogues.Add(new Dialogue
            {
                Id = "kael_greeting",
                NPCId = "kael_shadow",
                Text = "*Susurra desde las sombras* Si buscas trabajos... discretos... puedo ofrecerte algo. Pero solo si has demostrado tu valía con la Hermandad.",
                Type = DialogueType.Greeting,
                Requirement = new DialogueRequirement { MinReputation = 1000 },
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "Cuéntame más.", NextDialogueId = "kael_quest" },
                    new DialogueOption { Id = "2", Text = "No me interesa." }
                }
            });
            
            _dialogues.Add(new Dialogue
            {
                Id = "kael_quest",
                NPCId = "kael_shadow",
                Text = "Tenemos... competencia en el campamento. Elimina 5 bandidos enemigos y recibirás una generosa recompensa.",
                Type = DialogueType.QuestOffer,
                Options = new()
                {
                    new DialogueOption 
                    { 
                        Id = "1", 
                        Text = "Acepto.", 
                        Action = new DialogueAction { Type = DialogueActionType.StartQuest, TargetId = "shadow_elimination" }
                    },
                    new DialogueOption { Id = "2", Text = "Necesito pensarlo." }
                }
            });
            
            // Ignatius (Orden Llama)
            _dialogues.Add(new Dialogue
            {
                Id = "ignatius_greeting",
                NPCId = "ignatius_flame",
                Text = "Las llamas revelan la verdad, aventurero. Si buscas el poder del fuego, debes primero demostrar tu valor ante nuestra orden.",
                Type = DialogueType.Greeting,
                Requirement = new DialogueRequirement { MinReputation = 1000 },
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "¿Cómo puedo ganar su favor?", NextDialogueId = "ignatius_trial" },
                    new DialogueOption { Id = "2", Text = "Volveré cuando esté listo." }
                }
            });
            
            _dialogues.Add(new Dialogue
            {
                Id = "ignatius_trial",
                NPCId = "ignatius_flame",
                Text = "Derrota al Dragón de Ceniza en la Cumbre del Dragón. Solo entonces serás digno de nuestras enseñanzas más profundas.",
                Type = DialogueType.Story,
                Reward = new DialogueReward { Reputation = 50, FactionId = "orden_llama" },
                Options = new()
                {
                    new DialogueOption { Id = "1", Text = "Enfrentaré al dragón." }
                }
            });
        }
    }
}
