using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Lógica de la Arena PvP (Fase 11)</summary>
    public class PvpService
    {
        private static readonly Random _rng        = new();
        private readonly PvpDatabase   _db          = new();
        private readonly RpgService    _rpgSvc      = new();

        // ── ELO config ────────────────────────────────────────────────
        public const int BaseRating     = 1200;
        public const int KFactor        = 32;
        public const int MaxTurns       = 20;           // max turnos por combate PvP
        public const int CooldownMinutes = 5;           // cooldown entre peleas

        // ── Tiers ──────────────────────────────────────────────────────
        public static string GetTier(int rating) => rating switch
        {
            >= 2000 => "💎 Legendario",
            >= 1700 => "🏆 Maestro",
            >= 1500 => "⭐ Experto",
            >= 1350 => "🥇 Avanzado",
            >= 1200 => "🥈 Veterano",
            >= 1000 => "🥉 Novato",
            _       => "🪨 Sin clasificar"
        };

        // ── Cooldown ───────────────────────────────────────────────────
        public bool IsOnCooldown(RpgPlayer player, out TimeSpan remaining)
        {
            var diff = DateTime.UtcNow - player.LastPvpBattle;
            if (diff.TotalMinutes < CooldownMinutes)
            {
                remaining = TimeSpan.FromMinutes(CooldownMinutes) - diff;
                return true;
            }
            remaining = TimeSpan.Zero;
            return false;
        }

        // ── Retos ──────────────────────────────────────────────────────
        public (bool Ok, string Msg, PvpChallenge? Challenge) SendChallenge(
            RpgPlayer challenger, long targetChatId, int betAmount = 0)
        {
            if (IsOnCooldown(challenger, out var rem))
                return (false, $"⏳ Cooldown: {rem.Minutes}m {rem.Seconds}s restantes.", null);

            if (challenger.ChatId == targetChatId)
                return (false, "❌ No puedes retarte a ti mismo.", null);

            var target = _rpgSvc.GetPlayer(targetChatId);
            if (target is null) return (false, "❌ Ese jugador no existe.", null);

            if (betAmount > 0 && challenger.Gold < betAmount)
                return (false, $"❌ No tienes suficiente oro (tienes {challenger.Gold} 💰).", null);

            var c = new PvpChallenge
            {
                ChallengerId   = challenger.ChatId,
                ChallengerName = challenger.Name,
                ChallengedId   = targetChatId,
                ChallengedName = target.Name,
                BetAmount      = betAmount
            };
            _db.SaveChallenge(c);
            return (true, $"⚔️ Reto enviado a **{target.Name}**!", c);
        }

        public PvpChallenge? GetPendingChallenge(long challenged)
            => _db.GetChallengeForPlayer(challenged);

        public PvpChallenge? GetSentChallenge(long challenger)
            => _db.GetChallengeSentBy(challenger);

        public void CancelChallenge(long challenger)
        {
            var c = _db.GetChallengeSentBy(challenger);
            if (c is not null) _db.DeleteChallenge(c.Id);
        }

        // ── Simulación de combate PvP ──────────────────────────────────
        public (PvpMatch Match, string Narrative) SimulatePvp(RpgPlayer a, RpgPlayer b)
        {
            // Snapshots de HP (no modificamos el jugador real hasta aplicar resultado)
            int hpA = a.MaxHP;
            int hpB = b.MaxHP;

            var log = new List<PvpTurnLog>();

            for (int turn = 1; turn <= MaxTurns && hpA > 0 && hpB > 0; turn++)
            {
                // A ataca a B
                var (dmgA, critA, missA, skillA) = CalcAttack(a, b);
                if (!missA) hpB = Math.Max(0, hpB - dmgA);

                log.Add(new PvpTurnLog
                {
                    Turn      = turn,
                    Attacker  = a.Name,
                    Defender  = b.Name,
                    Damage    = missA ? 0 : dmgA,
                    WasCritical = critA,
                    WasMiss   = missA,
                    HpAfterA  = hpA,
                    HpAfterB  = hpB,
                    SkillUsed = skillA
                });

                if (hpB <= 0) break;

                // B ataca a A
                var (dmgB, critB, missB, skillB) = CalcAttack(b, a);
                if (!missB) hpA = Math.Max(0, hpA - dmgB);

                log.Add(new PvpTurnLog
                {
                    Turn      = turn,
                    Attacker  = b.Name,
                    Defender  = a.Name,
                    Damage    = missB ? 0 : dmgB,
                    WasCritical = critB,
                    WasMiss   = missB,
                    HpAfterA  = hpA,
                    HpAfterB  = hpB,
                    SkillUsed = skillB
                });
            }

            // Determinar resultado
            PvpMatchResult result;
            long winnerId; string winnerName;

            if (hpA > 0 && hpB <= 0)         { result = PvpMatchResult.WinnerA; winnerId = a.ChatId; winnerName = a.Name; }
            else if (hpB > 0 && hpA <= 0)    { result = PvpMatchResult.WinnerB; winnerId = b.ChatId; winnerName = b.Name; }
            else if (hpA > hpB)               { result = PvpMatchResult.WinnerA; winnerId = a.ChatId; winnerName = a.Name; } // más HP gana por puntos
            else if (hpB > hpA)               { result = PvpMatchResult.WinnerB; winnerId = b.ChatId; winnerName = b.Name; }
            else                              { result = PvpMatchResult.Draw;    winnerId = 0;         winnerName = "Empate"; }

            // ELO
            var (eloA, eloB) = CalcElo(a.PvpRating, b.PvpRating, result);

            var match = new PvpMatch
            {
                PlayerA       = a.ChatId,  NameA  = a.Name,  LevelA = a.Level,
                PlayerB       = b.ChatId,  NameB  = b.Name,  LevelB = b.Level,
                Result        = result,
                WinnerId      = winnerId,
                WinnerName    = winnerName,
                TurnsPlayed   = log.Count / 2,
                RatingChangeA = eloA,
                RatingChangeB = eloB,
                TurnLog       = log
            };

            // Narrativa resumida (últimas 4 acciones + resultado)
            var narr = BuildNarrative(match, hpA, hpB);
            return (match, narr);
        }

        // ── Aplicar resultado a los jugadores ─────────────────────────
        public (int GoldA, int GoldB) ApplyMatchResult(
            RpgPlayer a, RpgPlayer b, PvpMatch match, int betAmount)
        {
            // Stats
            if (match.Result == PvpMatchResult.WinnerA)     { a.PvpWins++;  b.PvpLosses++; }
            else if (match.Result == PvpMatchResult.WinnerB) { b.PvpWins++;  a.PvpLosses++; }
            else                                              { a.PvpDraws++; b.PvpDraws++; }

            // ELO
            a.PvpRating = Math.Max(0, a.PvpRating + match.RatingChangeA);
            b.PvpRating = Math.Max(0, b.PvpRating + match.RatingChangeB);

            // Cooldown
            a.LastPvpBattle = DateTime.UtcNow;
            b.LastPvpBattle = DateTime.UtcNow;

            // Apuesta
            int deltaA = 0, deltaB = 0;
            if (betAmount > 0)
            {
                a.Gold = Math.Max(0, a.Gold - betAmount);
                b.Gold = Math.Max(0, b.Gold - betAmount);
                if (match.Result == PvpMatchResult.WinnerA)      { a.Gold += betAmount * 2; deltaA =  betAmount; deltaB = -betAmount; }
                else if (match.Result == PvpMatchResult.WinnerB)  { b.Gold += betAmount * 2; deltaB =  betAmount; deltaA = -betAmount; }
                else                                               { a.Gold += betAmount;     b.Gold += betAmount; } // devolver en empate
            }

            // Guardar
            _db.SaveMatch(match);
            _rpgSvc.SavePlayer(a);
            _rpgSvc.SavePlayer(b);

            return (deltaA, deltaB);
        }

        // ── Ranking PvP ───────────────────────────────────────────────
        public List<PvpRankEntry> GetRanking(int top = 10)
        {
            return _rpgSvc.GetAllPlayers()
                .Where(p => p.PvpWins + p.PvpLosses + p.PvpDraws > 0 || p.PvpRating != BaseRating)
                .OrderByDescending(p => p.PvpRating)
                .Take(top)
                .Select(p => new PvpRankEntry
                {
                    ChatId = p.ChatId,
                    Name   = p.Name,
                    Rating = p.PvpRating,
                    Wins   = p.PvpWins,
                    Losses = p.PvpLosses,
                    Draws  = p.PvpDraws,
                    Tier   = GetTier(p.PvpRating)
                })
                .ToList();
        }

        public List<PvpMatch> GetHistory(long chatId, int count = 5)
            => _db.GetMatchHistory(chatId, count);

        // ══════════════════════════════════════════════════════════════
        // PRIVADOS
        // ══════════════════════════════════════════════════════════════

        /// <summary>Calcula un ataque de 'attacker' contra 'defender'. Devuelve (daño, crit, miss, nombreSkill?)</summary>
        private static (int Dmg, bool Crit, bool Miss, string? Skill) CalcAttack(RpgPlayer attacker, RpgPlayer defender)
        {
            // Tasa de fallo: 10% base, reducida por Dex
            double missChance = Math.Max(0.05, 0.15 - (attacker.Dexterity * 0.003));
            if (_rng.NextDouble() < missChance) return (0, false, true, null);

            // Daño base: PhysicalAttack vs defense
            int baseAtk = Math.Max(1, attacker.PhysicalAttack);
            int defense  = Math.Max(0, defender.Constitution / 4);
            int raw      = Math.Max(1, baseAtk - defense + _rng.Next(-3, 6));

            // Crítico: basado en Dex
            double critChance = 0.05 + attacker.Dexterity * 0.005;
            bool crit = _rng.NextDouble() < critChance;
            if (crit) raw = (int)(raw * 1.75);

            // Skill aleatoria (20% probabilidad, solo cosmética en PvP simulado)
            string? skill = null;
            if (_rng.Next(5) == 0 && attacker.Skills.Count > 0)
                skill = attacker.Skills[_rng.Next(attacker.Skills.Count)].Name;

            return (raw, crit, false, skill);
        }

        /// <summary>Calcula cambios ELO. Devuelve (deltaA, deltaB).</summary>
        private static (int DeltaA, int DeltaB) CalcElo(int ratingA, int ratingB, PvpMatchResult result)
        {
            double expectedA = 1.0 / (1.0 + Math.Pow(10.0, (ratingB - ratingA) / 400.0));
            double scoreA    = result == PvpMatchResult.WinnerA ? 1.0
                             : result == PvpMatchResult.WinnerB ? 0.0
                             : 0.5;

            int deltaA = (int)Math.Round(KFactor * (scoreA - expectedA));
            int deltaB = -deltaA;  // ELO zero-sum
            return (deltaA, deltaB);
        }

        private static string BuildNarrative(PvpMatch match, int finalHpA, int finalHpB)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("⚔️ **COMBATE PvP — RESUMEN**\n");

            // Últimas 6 entradas del log (3 turnos)
            var last = match.TurnLog.TakeLast(6).ToList();
            foreach (var t in last)
            {
                if (t.WasMiss)
                    sb.AppendLine($"• **{t.Attacker}** falla el ataque contra {t.Defender}.");
                else
                {
                    var crLabel = t.WasCritical ? " 💥 *¡Crítico!*" : "";
                    var sklLabel = t.SkillUsed != null ? $" [🌟 {t.SkillUsed}]" : "";
                    sb.AppendLine($"• **{t.Attacker}** → **{t.Damage}** daño a {t.Defender}{crLabel}{sklLabel}");
                }
            }

            sb.AppendLine();

            // Resultado
            if (match.Result == PvpMatchResult.Draw)
            {
                sb.AppendLine("🤝 **¡EMPATE!** Ambos guerreros se miran exhaustos.");
            }
            else
            {
                sb.AppendLine($"🏆 **¡{match.WinnerName} gana el combate!**");
                sb.AppendLine($"HP final → {match.NameA}: **{Math.Max(0,finalHpA)}** | {match.NameB}: **{Math.Max(0,finalHpB)}**");
            }

            // ELO
            var signA = match.RatingChangeA >= 0 ? "+" : "";
            var signB = match.RatingChangeB >= 0 ? "+" : "";
            sb.AppendLine($"\n📊 Rating: **{match.NameA}** {signA}{match.RatingChangeA} | **{match.NameB}** {signB}{match.RatingChangeB}");

            return sb.ToString();
        }
    }
}
