namespace BotTelegram.RPG.Models;

public class GlobalStats
{
    public DateTime LastUpdate { get; set; }
    public int TotalPlayers { get; set; }
    public int ActivePlayers { get; set; } // Jugaron en últimas 24h
    public long TotalGoldCirculating { get; set; }
    public int TotalEnemiesDefeated { get; set; }
    public int TotalBossesDefeated { get; set; }
}

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public long UserId { get; set; }
    public int Rank { get; set; }
    public int Value { get; set; }
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
}

public static class LeaderboardType
{
    public const string Level = "level";
    public const string Gold = "gold";
    public const string Kills = "kills";
    public const string BossKills = "boss_kills";
    public const string TotalDamage = "damage";
    public const string PetCount = "pets";
    public const string SkillsUnlocked = "skills";
}
