using BotTelegram.RPG.Models;
using BotTelegram.Services;

namespace BotTelegram.RPG.Services;

public class LeaderboardService
{
    private readonly RpgService _rpgService;

    public LeaderboardService(RpgService rpgService)
    {
        _rpgService = rpgService;
    }

    public List<LeaderboardEntry> GetTopByLevel(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.Level)
            .ThenByDescending(p => p.XP)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.Level,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopByGold(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.Gold)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.Gold,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopByKills(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.TotalKills)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.TotalKills,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopByBossKills(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.BossKills)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.BossKills,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopByDamage(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.TotalDamageDealt)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = (int)p.TotalDamageDealt,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopByPets(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.PetInventory?.Count ?? 0)
            .ThenByDescending(p => p.PetInventory?.Sum(pet => pet.Level) ?? 0)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.PetInventory?.Count ?? 0,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public List<LeaderboardEntry> GetTopBySkills(int count = 10)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        return allPlayers
            .OrderByDescending(p => p.UnlockedSkills?.Count ?? 0)
            .Take(count)
            .Select((p, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                PlayerName = p.Name,
                Username = p.Username ?? "",
                UserId = p.ChatId,
                Value = p.UnlockedSkills?.Count ?? 0,
                Class = p.Class.ToString(),
                Level = p.Level
            })
            .ToList();
    }

    public GlobalStats GetGlobalStats()
    {
        var allPlayers = _rpgService.GetAllPlayers();
        var yesterday = DateTime.UtcNow.AddHours(-24);

        return new GlobalStats
        {
            LastUpdate = DateTime.UtcNow,
            TotalPlayers = allPlayers.Count,
            ActivePlayers = allPlayers.Count(p => p.LastPlayedAt > yesterday),
            TotalGoldCirculating = allPlayers.Sum(p => (long)p.Gold),
            TotalEnemiesDefeated = allPlayers.Sum(p => p.TotalKills),
            TotalBossesDefeated = allPlayers.Sum(p => p.BossKills)
        };
    }

    public (int rank, int total) GetPlayerRank(long userId, string type)
    {
        var allPlayers = _rpgService.GetAllPlayers();
        
        List<RpgPlayer> orderedPlayers;
        
        if (type == LeaderboardType.Level)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.Level).ThenByDescending(p => p.XP).ToList();
        }
        else if (type == LeaderboardType.Gold)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.Gold).ToList();
        }
        else if (type == LeaderboardType.Kills)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.TotalKills).ToList();
        }
        else if (type == LeaderboardType.BossKills)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.BossKills).ToList();
        }
        else if (type == LeaderboardType.TotalDamage)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.TotalDamageDealt).ToList();
        }
        else if (type == LeaderboardType.PetCount)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.PetInventory?.Count ?? 0).ToList();
        }
        else if (type == LeaderboardType.SkillsUnlocked)
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.UnlockedSkills?.Count ?? 0).ToList();
        }
        else
        {
            orderedPlayers = allPlayers.OrderByDescending(p => p.Level).ToList();
        }

        var rank = orderedPlayers.FindIndex(p => p.ChatId == userId) + 1;
        return (rank > 0 ? rank : allPlayers.Count, allPlayers.Count);
    }
}
