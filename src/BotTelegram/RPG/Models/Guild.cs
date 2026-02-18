namespace BotTelegram.RPG.Models
{
    public enum GuildRole
    {
        Member,
        Officer,
        Owner
    }

    public class GuildMember
    {
        public long ChatId   { get; set; }
        public string Name   { get; set; } = "";
        public GuildRole Role { get; set; } = GuildRole.Member;
        /// <summary>Oro total aportado al banco del gremio</summary>
        public int Contribution { get; set; } = 0;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

    public class Guild
    {
        public string Id          { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public string Name        { get; set; } = "";
        /// <summary>Tag de 2-5 letras, ej: [RPG]</summary>
        public string Tag         { get; set; } = "";
        public string Emoji       { get; set; } = "⚔️";
        public string Description { get; set; } = "";
        public long   OwnerId     { get; set; }
        public List<GuildMember> Members { get; set; } = new();

        public int Level      { get; set; } = 1;
        public int Experience { get; set; } = 0;
        /// <summary>XP total necesaria al siguiente nivel (Level * 500)</summary>
        public int ExperienceToNextLevel => Level * 500;

        /// <summary>Oro en el banco del gremio</summary>
        public int GuildBank  { get; set; } = 0;

        public int MaxMembers => 10 + (Level * 5);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Helpers ───────────────────────────────────────────────────────
        public GuildMember? GetMember(long chatId) =>
            Members.FirstOrDefault(m => m.ChatId == chatId);

        public bool IsMember(long chatId)   => Members.Any(m => m.ChatId == chatId);
        public bool IsOfficer(long chatId)  => Members.Any(m => m.ChatId == chatId && m.Role >= GuildRole.Officer);
        public bool IsOwner(long chatId)    => OwnerId == chatId;
        public bool CanManage(long chatId)  => IsOwner(chatId) || IsOfficer(chatId);

        /// <summary>Total de contribuciones de todos los miembros.</summary>
        public int TotalContribution => Members.Sum(m => m.Contribution);

        /// <summary>Emoji de rango por nivel del gremio.</summary>
        public string LevelEmoji => Level switch
        {
            >= 10 => "💎",
            >= 7  => "🏆",
            >= 5  => "⭐",
            >= 3  => "🥈",
            _     => "🥉"
        };
    }
}
