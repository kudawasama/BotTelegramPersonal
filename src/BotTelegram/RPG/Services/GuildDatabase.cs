using BotTelegram.RPG.Models;
using System.Text.Json;

namespace BotTelegram.RPG.Services
{
    /// <summary>Persistencia JSON de gremios (Fase 10)</summary>
    public class GuildDatabase
    {
        private readonly string _filePath;
        private static readonly object _lock = new();

        public GuildDatabase()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var root = currentDir;
            while (!File.Exists(Path.Combine(root, "BotTelegram.csproj")))
            {
                var parent = Directory.GetParent(root);
                if (parent == null) { root = currentDir; break; }
                root = parent.FullName;
            }
            var dataDir = Path.Combine(root, "data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "rpg_guilds.json");
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        // ── CRUD ───────────────────────────────────────────────────────────
        public List<Guild> GetAll()
        {
            lock (_lock)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<List<Guild>>(json, _opts) ?? new();
                }
                catch { return new(); }
            }
        }

        public Guild? GetById(string id)
            => GetAll().FirstOrDefault(g => g.Id == id);

        public Guild? GetByMember(long chatId)
            => GetAll().FirstOrDefault(g => g.IsMember(chatId));

        public void Save(Guild guild)
        {
            lock (_lock)
            {
                var all = GetAll();
                var idx = all.FindIndex(g => g.Id == guild.Id);
                if (idx >= 0) all[idx] = guild;
                else          all.Add(guild);
                File.WriteAllText(_filePath, JsonSerializer.Serialize(all, _opts));
            }
        }

        public void Delete(string id)
        {
            lock (_lock)
            {
                var all = GetAll();
                all.RemoveAll(g => g.Id == id);
                File.WriteAllText(_filePath, JsonSerializer.Serialize(all, _opts));
            }
        }

        public List<Guild> GetRanking(int top = 10)
            => GetAll()
                .OrderByDescending(g => g.Level)
                .ThenByDescending(g => g.TotalContribution)
                .Take(top)
                .ToList();
    }
}
