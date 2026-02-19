using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public class NPCInteractionService
    {
        private readonly FactionService _factionService;
        
        public NPCInteractionService()
        {
            _factionService = new FactionService();
        }
        
        /// <summary>
        /// Inicia una conversación con un NPC
        /// </summary>
        public NPCInteractionResult StartConversation(RpgPlayer player, string npcId)
        {
            var npc = NPCDatabase.GetNPC(npcId);
            if (npc == null)
                return new NPCInteractionResult { Success = false, Message = "NPC no encontrado." };
            
            // Verificar requisitos de reputación
            if (npc.FactionId != null)
            {
                var rep = _factionService.GetReputation(player, npc.FactionId);
                var repValue = rep?.Reputation ?? 0;
                
                if (repValue < npc.RequiredReputation)
                {
                    var faction = FactionDatabase.GetFaction(npc.FactionId);
                    return new NPCInteractionResult 
                    { 
                        Success = false, 
                        Message = $"❌ {npc.Name} no confía en ti. Necesitas más reputación con {faction?.Name}." 
                    };
                }
            }
            
            // Obtener diálogo inicial
            var dialogues = DialogueDatabase.GetDialoguesForNPC(npcId);
            var greetingDialogue = dialogues.FirstOrDefault(d => 
                d.Type == DialogueType.Greeting && 
                MeetsRequirements(player, d.Requirement));
            
            if (greetingDialogue == null)
            {
                return new NPCInteractionResult
                {
                    Success = true,
                    NPC = npc,
                    Message = $"{npc.Emoji} **{npc.Name}**\n\n{npc.Description}\n\n_No tiene nada que decir por ahora._"
                };
            }
            
            return new NPCInteractionResult
            {
                Success = true,
                NPC = npc,
                CurrentDialogue = greetingDialogue,
                Message = FormatDialogue(npc, greetingDialogue),
                AvailableOptions = greetingDialogue.Options.Select(o => o.Text).ToList()
            };
        }
        
        /// <summary>
        /// Procesa la selección de una opción de diálogo
        /// </summary>
        public NPCInteractionResult ProcessDialogueOption(RpgPlayer player, string npcId, string dialogueId, string optionId)
        {
            var dialogue = DialogueDatabase.GetDialogue(dialogueId);
            if (dialogue == null)
                return new NPCInteractionResult { Success = false, Message = "Diálogo no encontrado." };
            
            var option = dialogue.Options.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return new NPCInteractionResult { Success = false, Message = "Opción inválida." };
            
            // Aplicar recompensa del diálogo si existe
            if (dialogue.Reward != null)
            {
                player.Gold += dialogue.Reward.Gold;
                player.XP += dialogue.Reward.XP;
                if (dialogue.Reward.FactionId != null)
                    _factionService.GainReputation(player, dialogue.Reward.FactionId, dialogue.Reward.Reputation);
            }
            
            // Ejecutar acción de la opción
            var actionResult = ExecuteAction(player, option.Action);
            
            // Si hay siguiente diálogo, mostrarlo
            if (!string.IsNullOrEmpty(option.NextDialogueId))
            {
                var nextDialogue = DialogueDatabase.GetDialogue(option.NextDialogueId);
                if (nextDialogue != null)
                {
                    var npc = NPCDatabase.GetNPC(npcId);
                    return new NPCInteractionResult
                    {
                        Success = true,
                        NPC = npc,
                        CurrentDialogue = nextDialogue,
                        Message = FormatDialogue(npc!, nextDialogue) + actionResult,
                        AvailableOptions = nextDialogue.Options.Select(o => o.Text).ToList()
                    };
                }
            }
            
            return new NPCInteractionResult
            {
                Success = true,
                Message = "Conversación finalizada." + actionResult
            };
        }
        
        private bool MeetsRequirements(RpgPlayer player, DialogueRequirement? req)
        {
            if (req == null) return true;
            
            if (req.MinLevel.HasValue && player.Level < req.MinLevel.Value)
                return false;
            
            if (req.MinReputation.HasValue && req.RequiredItemId != null)
            {
                var rep = _factionService.GetReputation(player, req.RequiredItemId);
                if ((rep?.Reputation ?? 0) < req.MinReputation.Value)
                    return false;
            }
            
            return true;
        }
        
        private string FormatDialogue(NPC npc, Dialogue dialogue)
        {
            var text = $"{npc.Emoji} **{npc.Name}**\n\n\"{dialogue.Text}\"\n\n";
            
            if (dialogue.Options.Any())
            {
                text += "**Opciones:**\n";
                for (int i = 0; i < dialogue.Options.Count; i++)
                {
                    text += $"{i + 1}. {dialogue.Options[i].Text}\n";
                }
            }
            
            return text;
        }
        
        private string ExecuteAction(RpgPlayer player, DialogueAction? action)
        {
            if (action == null) return "";
            
            switch (action.Type)
            {
                case DialogueActionType.GiveGold:
                    player.Gold += action.Value ?? 0;
                    return $"\n\n✅ +{action.Value} oro";
                    
                case DialogueActionType.TakeGold:
                    var cost = action.Value ?? 0;
                    if (player.Gold >= cost)
                    {
                        player.Gold -= cost;
                        player.HP = player.MaxHP;
                        player.Mana = player.MaxMana;
                        player.Stamina = player.MaxStamina;
                        return $"\n\n✅ HP/Mana/Stamina restaurados (-{cost} oro)";
                    }
                    return "\n\n❌ No tienes suficiente oro.";
                    
                case DialogueActionType.GainReputation:
                    if (action.TargetId != null)
                    {
                        _factionService.GainReputation(player, action.TargetId, action.Value ?? 0);
                        return $"\n\n✅ +{action.Value} reputación";
                    }
                    return "";
                    
                default:
                    return "";
            }
        }
    }
}
