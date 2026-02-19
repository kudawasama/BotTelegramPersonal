using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    public class FactionService
    {
        public void GainReputation(RpgPlayer player, string factionId, int amount)
        {
            var rep = player.FactionReputations.FirstOrDefault(r => r.FactionId == factionId);
            if (rep == null)
            {
                rep = new PlayerFactionReputation { FactionId = factionId, Reputation = 0 };
                player.FactionReputations.Add(rep);
            }
            
            var previousTier = rep.GetTier();
            rep.Reputation += amount;
            rep.LastUpdated = DateTime.UtcNow;
            
            var newTier = rep.GetTier();
            if (newTier > previousTier)
                ApplyTierReward(player, factionId, newTier);
            
            var faction = FactionDatabase.GetFaction(factionId);
            if (faction?.EnemyFactionId != null)
                GainReputation(player, faction.EnemyFactionId, -amount / 2);
        }
        
        private void ApplyTierReward(RpgPlayer player, string factionId, FactionTier tier)
        {
            var faction = FactionDatabase.GetFaction(factionId);
            if (faction?.Rewards.TryGetValue(tier, out var reward) == true)
            {
                player.Gold += reward.GoldReward;
                player.XP += reward.XPReward;
                if (reward.UnlockedZoneId != null && !player.UnlockedZones.Contains(reward.UnlockedZoneId))
                    player.UnlockedZones.Add(reward.UnlockedZoneId);
            }
        }
        
        public PlayerFactionReputation? GetReputation(RpgPlayer player, string factionId)
            => player.FactionReputations.FirstOrDefault(r => r.FactionId == factionId);
    }
}
