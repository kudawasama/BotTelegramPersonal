using System.Collections.Generic;

namespace BotTelegram.RPG.Models
{
    /// <summary>
    /// Define una clase oculta desbloqueable mediante acciones específicas
    /// </summary>
    public class HiddenClass
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "🌟";
        
        // Requisitos para desbloquear (Action ID → Count requerido)
        public Dictionary<string, int> RequiredActions { get; set; } = new();
        
        // Pasivas que otorga al desbloquear
        public List<string> GrantedPassives { get; set; } = new();
        
        // Nuevas acciones/skills que desbloquea
        public List<string> UnlockedSkills { get; set; } = new();
        
        // Bonuses de stats al cambiar a esta clase
        public int StrengthBonus { get; set; } = 0;
        public int IntelligenceBonus { get; set; } = 0;
        public int DexterityBonus { get; set; } = 0;
        public int ConstitutionBonus { get; set; } = 0;
        public int WisdomBonus { get; set; } = 0;
        public int CharismaBonus { get; set; } = 0;
    }

    /// <summary>
    /// Representa el progreso hacia desbloquear una clase oculta
    /// </summary>
    public class ClassUnlockProgress
    {
        public string ClassId { get; set; } = "";
        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }
        public Dictionary<string, int> CurrentProgress { get; set; } = new(); // Action ID → Current count
        public Dictionary<string, bool> RequirementsMet { get; set; } = new(); // Action ID → Met?
    }
