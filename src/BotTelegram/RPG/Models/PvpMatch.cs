namespace BotTelegram.RPG.Models
{
    /// <summary>Resultado de un combate PvP</summary>
    public enum PvpMatchResult { WinnerA, WinnerB, Draw }

    /// <summary>Log de un turno dentro de un combate PvP simulado</summary>
    public class PvpTurnLog
    {
        public int   Turn        { get; set; }
        public string Attacker   { get; set; } = "";  // nombre del atacante
        public string Defender   { get; set; } = "";  // nombre del defensor
        public int   Damage      { get; set; }
        public bool  WasCritical { get; set; }
        public bool  WasMiss     { get; set; }
        public int   HpAfterA    { get; set; }  // HP del jugador A tras el turno
        public int   HpAfterB    { get; set; }  // HP del jugador B tras el turno
        public string? SkillUsed { get; set; }  // null si fue ataque normal
    }

    /// <summary>Registro persistido de un combate PvP</summary>
    public class PvpMatch
    {
        public string   Id          { get; set; } = Guid.NewGuid().ToString()[..8];
        public long     PlayerA     { get; set; }
        public string   NameA       { get; set; } = "";
        public int      LevelA      { get; set; }
        public long     PlayerB     { get; set; }
        public string   NameB       { get; set; } = "";
        public int      LevelB      { get; set; }
        public PvpMatchResult Result { get; set; }
        public long     WinnerId    { get; set; }  // 0 = empate
        public string   WinnerName  { get; set; } = "";
        public int      TurnsPlayed { get; set; }
        public int      RatingChangeA { get; set; }  // +/- ELO para A
        public int      RatingChangeB { get; set; }  // +/- ELO para B
        public List<PvpTurnLog> TurnLog { get; set; } = new();
        public DateTime PlayedAt    { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Entrada del ranking PvP (snapshot)</summary>
    public class PvpRankEntry
    {
        public long   ChatId   { get; set; }
        public string Name     { get; set; } = "";
        public int    Rating   { get; set; }
        public int    Wins     { get; set; }
        public int    Losses   { get; set; }
        public int    Draws    { get; set; }
        public string Tier     { get; set; } = "";

        public int Matches   => Wins + Losses + Draws;
        public double WinRate => Matches == 0 ? 0 : Math.Round(100.0 * Wins / Matches, 1);
    }

    /// <summary>Reto PvP pendiente de aceptar</summary>
    public class PvpChallenge
    {
        public string Id          { get; set; } = Guid.NewGuid().ToString()[..8];
        public long   ChallengerId   { get; set; }
        public string ChallengerName { get; set; } = "";
        public long   ChallengedId   { get; set; }
        public string ChallengedName { get; set; } = "";
        public int    BetAmount      { get; set; } = 0;  // 0 = sin apuesta
        public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
        public bool   Expired        => (DateTime.UtcNow - CreatedAt).TotalMinutes > 10;
    }
}
