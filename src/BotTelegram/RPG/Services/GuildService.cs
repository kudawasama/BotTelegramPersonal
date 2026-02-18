using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Lógica de negocio del sistema de gremios (Fase 10)</summary>
    public class GuildService
    {
        private readonly GuildDatabase _db;
        private readonly RpgService    _rpgService;

        public const int CreateCost     = 1000;  // Oro para crear gremio
        public const int MaxGuildNameLen = 24;
        public const int MaxTagLen       = 5;

        public GuildService()
        {
            _db         = new GuildDatabase();
            _rpgService = new RpgService();
        }

        // ── Obtener gremio de un jugador ──────────────────────────────────
        public Guild? GetPlayerGuild(RpgPlayer player)
            => string.IsNullOrEmpty(player.GuildId) ? null : _db.GetById(player.GuildId);

        // ── Crear gremio ──────────────────────────────────────────────────
        public (bool Ok, string Message, Guild? Guild) CreateGuild(
            RpgPlayer player, string name, string tag, string description, string emoji)
        {
            if (!string.IsNullOrEmpty(player.GuildId))
                return (false, "❌ Ya perteneces a un gremio. Sal primero.", null);

            if (player.Gold < CreateCost)
                return (false, $"❌ Necesitas **{CreateCost}** 💰 oro para crear un gremio.", null);

            name = name.Trim();
            tag  = tag.Trim().ToUpper();

            if (name.Length < 3 || name.Length > MaxGuildNameLen)
                return (false, $"❌ El nombre debe tener entre 3 y {MaxGuildNameLen} caracteres.", null);

            if (tag.Length < 2 || tag.Length > MaxTagLen)
                return (false, $"❌ El tag debe tener entre 2 y {MaxTagLen} letras.", null);

            var all = _db.GetAll();
            if (all.Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return (false, "❌ Ya existe un gremio con ese nombre.", null);

            if (all.Any(g => g.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                return (false, "❌ Ya existe un gremio con ese tag.", null);

            var guild = new Guild
            {
                Name        = name,
                Tag         = tag,
                Emoji       = emoji,
                Description = description,
                OwnerId     = player.ChatId,
                Members     = new() { new GuildMember
                {
                    ChatId  = player.ChatId,
                    Name    = player.Name,
                    Role    = GuildRole.Owner,
                    JoinedAt = DateTime.UtcNow
                }}
            };

            player.Gold       -= CreateCost;
            player.GuildId     = guild.Id;
            player.GuildRole   = GuildRole.Owner;
            _db.Save(guild);
            _rpgService.SavePlayer(player);

            return (true, $"🏰 ¡Gremio **{emoji} {name}** [{tag}] creado!", guild);
        }

        // ── Unirse a un gremio ────────────────────────────────────────────
        public (bool Ok, string Message) JoinGuild(RpgPlayer player, string guildId)
        {
            if (!string.IsNullOrEmpty(player.GuildId))
                return (false, "❌ Ya perteneces a un gremio.");

            var guild = _db.GetById(guildId);
            if (guild is null) return (false, "❌ Gremio no encontrado.");

            if (guild.Members.Count >= guild.MaxMembers)
                return (false, $"❌ El gremio está lleno ({guild.MaxMembers} miembros máx.).");

            guild.Members.Add(new GuildMember
            {
                ChatId   = player.ChatId,
                Name     = player.Name,
                Role     = GuildRole.Member,
                JoinedAt = DateTime.UtcNow
            });
            player.GuildId   = guild.Id;
            player.GuildRole = GuildRole.Member;

            _db.Save(guild);
            _rpgService.SavePlayer(player);
            return (true, $"✅ Te uniste al gremio **{guild.Emoji} {guild.Name}** [{guild.Tag}]!");
        }

        // ── Salir de un gremio ────────────────────────────────────────────
        public (bool Ok, string Message) LeaveGuild(RpgPlayer player)
        {
            if (string.IsNullOrEmpty(player.GuildId))
                return (false, "❌ No perteneces a ningún gremio.");

            var guild = _db.GetById(player.GuildId);
            if (guild is null) { CleanPlayerGuild(player); return (false, "❌ Gremio no encontrado."); }

            if (guild.IsOwner(player.ChatId))
            {
                // Si el dueño sale y hay otros miembros → transferir a officer/member más antiguo
                var next = guild.Members
                    .Where(m => m.ChatId != player.ChatId)
                    .OrderByDescending(m => m.Role)
                    .ThenBy(m => m.JoinedAt)
                    .FirstOrDefault();

                if (next != null)
                {
                    next.Role    = GuildRole.Owner;
                    guild.OwnerId = next.ChatId;
                    // Actualizar el nuevo dueño en su perfil
                    var newOwnerPlayer = _rpgService.GetPlayer(next.ChatId);
                    if (newOwnerPlayer != null)
                    {
                        newOwnerPlayer.GuildRole = GuildRole.Owner;
                        _rpgService.SavePlayer(newOwnerPlayer);
                    }
                }
                else
                {
                    // Era el único miembro: disolver gremio
                    _db.Delete(guild.Id);
                    CleanPlayerGuild(player);
                    return (true, $"🏚️ Gremio **{guild.Name}** disuelto (eras el único miembro).");
                }
            }

            guild.Members.RemoveAll(m => m.ChatId == player.ChatId);
            CleanPlayerGuild(player);
            _db.Save(guild);
            return (true, $"👋 Saliste del gremio **{guild.Emoji} {guild.Name}**.");
        }

        // ── Expulsar miembro ──────────────────────────────────────────────
        public (bool Ok, string Message) KickMember(RpgPlayer requester, long targetChatId)
        {
            var guild = GetPlayerGuild(requester);
            if (guild is null) return (false, "❌ No perteneces a un gremio.");
            if (!guild.CanManage(requester.ChatId)) return (false, "❌ No tienes permisos.");

            var target = guild.GetMember(targetChatId);
            if (target is null) return (false, "❌ Miembro no encontrado.");
            if (target.Role >= requester.GuildRole)
                return (false, "❌ No puedes expulsar a alguien del mismo rango o superior.");

            guild.Members.Remove(target);
            _db.Save(guild);

            var targetPlayer = _rpgService.GetPlayer(targetChatId);
            if (targetPlayer != null) { CleanPlayerGuild(targetPlayer); }

            return (true, $"✅ {target.Name} fue expulsado del gremio.");
        }

        // ── Promover / degradar ───────────────────────────────────────────
        public (bool Ok, string Message) PromoteMember(RpgPlayer requester, long targetChatId)
        {
            var guild = GetPlayerGuild(requester);
            if (guild is null) return (false, "❌ No perteneces a un gremio.");
            if (!guild.IsOwner(requester.ChatId)) return (false, "❌ Solo el líder puede promover.");

            var target = guild.GetMember(targetChatId);
            if (target is null) return (false, "❌ Miembro no encontrado.");
            if (target.Role == GuildRole.Officer) return (false, "⚠️ Ya es Oficial.");

            target.Role = GuildRole.Officer;
            _db.Save(guild);

            var tp = _rpgService.GetPlayer(targetChatId);
            if (tp != null) { tp.GuildRole = GuildRole.Officer; _rpgService.SavePlayer(tp); }

            return (true, $"⭐ {target.Name} ascendido a **Oficial**.");
        }

        // ── Banco del gremio ──────────────────────────────────────────────
        public (bool Ok, string Message) Deposit(RpgPlayer player, int amount)
        {
            if (amount <= 0) return (false, "❌ Cantidad inválida.");
            if (player.Gold < amount) return (false, "❌ Oro insuficiente.");
            var guild = GetPlayerGuild(player);
            if (guild is null) return (false, "❌ No perteneces a un gremio.");

            player.Gold  -= amount;
            guild.GuildBank += amount;
            var member = guild.GetMember(player.ChatId);
            if (member != null)
            {
                member.Contribution += amount;
                player.GuildContribution += amount;
                // XP al gremio por contribución
                AddGuildXP(guild, amount / 10);
            }
            _db.Save(guild);
            _rpgService.SavePlayer(player);
            return (true, $"💰 Depositaste **{amount}** oro. Banco del gremio: **{guild.GuildBank}** 💰");
        }

        public (bool Ok, string Message) Withdraw(RpgPlayer player, int amount)
        {
            if (amount <= 0) return (false, "❌ Cantidad inválida.");
            var guild = GetPlayerGuild(player);
            if (guild is null) return (false, "❌ No perteneces a un gremio.");
            if (!guild.CanManage(player.ChatId)) return (false, "❌ Solo Oficiales y el Líder pueden retirar.");
            if (guild.GuildBank < amount) return (false, $"❌ El banco solo tiene **{guild.GuildBank}** 💰.");

            guild.GuildBank -= amount;
            player.Gold     += amount;
            _db.Save(guild);
            _rpgService.SavePlayer(player);
            return (true, $"✅ Retiraste **{amount}** 💰 del banco del gremio.");
        }

        // ── XP y nivel del gremio ─────────────────────────────────────────
        public void AddGuildXP(Guild guild, int xp)
        {
            guild.Experience += xp;
            while (guild.Experience >= guild.ExperienceToNextLevel)
            {
                guild.Experience -= guild.ExperienceToNextLevel;
                guild.Level++;
            }
            _db.Save(guild);
        }

        // ── Ranking público ───────────────────────────────────────────────
        public List<Guild> GetRanking(int top = 10) => _db.GetRanking(top);

        public List<Guild> GetJoinableGuilds(int page = 1, int pageSize = 8)
            => _db.GetAll()
                .Where(g => g.Members.Count < g.MaxMembers)
                .OrderByDescending(g => g.Level)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        // ── Utils ─────────────────────────────────────────────────────────
        private void CleanPlayerGuild(RpgPlayer player)
        {
            player.GuildId   = null;
            player.GuildRole = GuildRole.Member;
            _rpgService.SavePlayer(player);
        }
    }
}
