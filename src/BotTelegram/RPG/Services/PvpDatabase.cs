using BotTelegram.RPG.Models;
using System.Text.Json;

namespace BotTelegram.RPG.Services
{
    /// <summary>Persistencia JSON para historial PvP y retos pendientes (Fase 11)</summary>
    public class PvpDatabase
    {
        private readonly string _matchesFile;
        private readonly string _challengesFile;
        private static readonly object _lockM = new();
        private static readonly object _lockC = new();

        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public PvpDatabase()
        {
            var root = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(root, "BotTelegram.csproj")))
            {
                var parent = Directory.GetParent(root);
                if (parent == null) break;
                root = parent.FullName;
            }
            var dataDir = Path.Combine(root, "data");
            Directory.CreateDirectory(dataDir);
            _matchesFile    = Path.Combine(dataDir, "pvp_matches.json");
            _challengesFile = Path.Combine(dataDir, "pvp_challenges.json");
            if (!File.Exists(_matchesFile))    File.WriteAllText(_matchesFile,    "[]");
            if (!File.Exists(_challengesFile)) File.WriteAllText(_challengesFile, "[]");
        }

        // ── Partidas ────────────────────────────────────────────────────
        public List<PvpMatch> GetAllMatches()
        {
            lock (_lockM)
            {
                try { return JsonSerializer.Deserialize<List<PvpMatch>>(File.ReadAllText(_matchesFile), _opts) ?? new(); }
                catch { return new(); }
            }
        }

        public void SaveMatch(PvpMatch match)
        {
            lock (_lockM)
            {
                var all = GetAllMatches();
                all.Insert(0, match); // más reciente primero
                if (all.Count > 500) all = all.Take(500).ToList(); // max 500 registros
                File.WriteAllText(_matchesFile, JsonSerializer.Serialize(all, _opts));
            }
        }

        public List<PvpMatch> GetMatchHistory(long chatId, int count = 5)
            => GetAllMatches()
                .Where(m => m.PlayerA == chatId || m.PlayerB == chatId)
                .Take(count)
                .ToList();

        // ── Retos pendientes ────────────────────────────────────────────
        public List<PvpChallenge> GetChallenges()
        {
            lock (_lockC)
            {
                try { return JsonSerializer.Deserialize<List<PvpChallenge>>(File.ReadAllText(_challengesFile), _opts) ?? new(); }
                catch { return new(); }
            }
        }

        private void WriteChallenges(List<PvpChallenge> list)
            => File.WriteAllText(_challengesFile, JsonSerializer.Serialize(list, _opts));

        public void SaveChallenge(PvpChallenge c)
        {
            lock (_lockC)
            {
                var all = GetChallenges();
                all.RemoveAll(x => x.Expired);
                all.RemoveAll(x => x.ChallengerId == c.ChallengerId); // solo un reto activo
                all.Add(c);
                WriteChallenges(all);
            }
        }

        public PvpChallenge? GetChallengeForPlayer(long challenged)
            => GetChallenges().FirstOrDefault(c => c.ChallengedId == challenged && !c.Expired);

        public PvpChallenge? GetChallengeSentBy(long challenger)
            => GetChallenges().FirstOrDefault(c => c.ChallengerId == challenger && !c.Expired);

        public void DeleteChallenge(string id)
        {
            lock (_lockC)
            {
                var all = GetChallenges();
                all.RemoveAll(c => c.Id == id || c.Expired);
                WriteChallenges(all);
            }
        }
    }
}
