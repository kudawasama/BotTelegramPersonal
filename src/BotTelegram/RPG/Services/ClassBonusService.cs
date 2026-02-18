using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio que calcula y aplica los bonos de stats que cada clase otorga al equiparse.
    /// Los bonos son delta respecto al Aventurero base (STR 10, INT 10, DEX 10, CON 10, WIS 10, CHA 10, HP 100, Mana 0).
    /// </summary>
    public static class ClassBonusService
    {
        // ═══════════════════════════════════════
        // CONSTANTES BASE (Aventurero)
        // ═══════════════════════════════════════
        private const int BaseStr = 10;
        private const int BaseInt = 10;
        private const int BaseDex = 10;
        private const int BaseCon = 10;
        private const int BaseWis = 10;
        private const int BaseCha = 10;
        private const int BaseHP  = 100;
        private const int BaseMana = 0;

        // ═══════════════════════════════════════
        // RECORD de bonos por clase
        // ═══════════════════════════════════════
        public record ClassBonus(
            int Str = 0, int Int = 0, int Dex = 0, int Con = 0, int Wis = 0, int Cha = 0,
            int MaxHP = 0, int MaxMana = 0
        )
        {
            /// <summary>Descripción compacta para la UI del bot (solo stats con delta != 0)</summary>
            public string ToDisplayString()
            {
                var parts = new List<string>();
                if (Str  != 0) parts.Add($"{(Str  > 0 ? "+" : "")}{Str}💪");
                if (Int  != 0) parts.Add($"{(Int  > 0 ? "+" : "")}{Int}🔮");
                if (Dex  != 0) parts.Add($"{(Dex  > 0 ? "+" : "")}{Dex}🏃");
                if (Con  != 0) parts.Add($"{(Con  > 0 ? "+" : "")}{Con}🛡️");
                if (Wis  != 0) parts.Add($"{(Wis  > 0 ? "+" : "")}{Wis}🌟");
                if (Cha  != 0) parts.Add($"{(Cha  > 0 ? "+" : "")}{Cha}🎭");
                if (MaxHP   != 0) parts.Add($"{(MaxHP   > 0 ? "+" : "")}{MaxHP}❤️");
                if (MaxMana != 0) parts.Add($"{(MaxMana > 0 ? "+" : "")}{MaxMana}💧");
                return parts.Count > 0 ? string.Join(" ", parts) : "Sin bonos especiales";
            }
        }

        // ═══════════════════════════════════════
        // OBTENER BONOS POR classId
        // ═══════════════════════════════════════
        public static ClassBonus GetBonuses(string classId)
        {
            // Calculamos delta respecto a la base del Aventurero
            return classId switch
            {
                // ── Tier 1 Básicas ──────────────────────────────────────
                "warrior"  => CalcDelta(14, 8,  10, 14, 8,  8,  120, 0),
                "mage"     => CalcDelta(6,  16, 8,  8,  12, 10, 80,  100),
                "rogue"    => CalcDelta(10, 10, 16, 10, 10, 10, 100, 30),
                "cleric"   => CalcDelta(10, 12, 8,  12, 14, 12, 110, 80),

                // ── Tier 2 Avanzadas ─────────────────────────────────────
                "paladin"  => CalcDelta(14, 10, 8,  14, 12, 12, 130, 60),
                "ranger"   => CalcDelta(10, 10, 16, 12, 14, 8,  105, 50),
                "warlock"  => CalcDelta(6,  16, 10, 10, 10, 14, 85,  120),
                "monk"     => CalcDelta(12, 10, 14, 12, 16, 10, 115, 70),

                // ── Tier 3 Épicas ─────────────────────────────────────────
                "berserker"=> CalcDelta(20, 6,  12, 16, 6,  8,  140, 0),
                "assassin" => CalcDelta(12, 12, 20, 10, 12, 10, 105, 40),
                "sorcerer" => CalcDelta(6,  20, 10, 10, 10, 16, 90,  150),
                "druid"    => CalcDelta(12, 12, 12, 14, 18, 12, 120, 100),

                // ── Tier 4 Legendarias ────────────────────────────────────
                "necromancer" => CalcDelta(8,  22, 10, 12, 14, 16, 95,  180),
                "bard"        => CalcDelta(10, 14, 14, 12, 14, 20, 110, 120),

                // Aventurero / desconocido → sin bonos
                _ => new ClassBonus()
            };
        }

        private static ClassBonus CalcDelta(int str, int @int, int dex, int con, int wis, int cha, int hp, int mana)
            => new(str - BaseStr, @int - BaseInt, dex - BaseDex, con - BaseCon, wis - BaseWis, cha - BaseCha, hp - BaseHP, mana - BaseMana);

        // ═══════════════════════════════════════
        // APLICAR CLASE AL JUGADOR
        // ═══════════════════════════════════════
        /// <summary>
        /// Quita los bonos de la clase anterior y aplica los de la nueva.
        /// Ajusta HP/Mana actuales proporcionalmente para no matar al jugador.
        /// </summary>
        public static void ApplyClass(RpgPlayer player, string newClassId)
        {
            // 1. Quitar bonos del clase anterior
            var oldBonus = player.ActiveClassId != null ? GetBonuses(player.ActiveClassId) : new ClassBonus();
            player.ClassBonusStr  -= oldBonus.Str;
            player.ClassBonusInt  -= oldBonus.Int;
            player.ClassBonusDex  -= oldBonus.Dex;
            player.ClassBonusCon  -= oldBonus.Con;
            player.ClassBonusWis  -= oldBonus.Wis;
            player.ClassBonusCha  -= oldBonus.Cha;

            int oldHPBonus   = oldBonus.MaxHP;
            int oldManaBonus = oldBonus.MaxMana;

            // 2. Aplicar nuevos bonos
            var newBonus = GetBonuses(newClassId);
            player.ClassBonusStr  += newBonus.Str;
            player.ClassBonusInt  += newBonus.Int;
            player.ClassBonusDex  += newBonus.Dex;
            player.ClassBonusCon  += newBonus.Con;
            player.ClassBonusWis  += newBonus.Wis;
            player.ClassBonusCha  += newBonus.Cha;

            // 3. Ajustar MaxHP
            int hpDelta = newBonus.MaxHP - oldHPBonus;
            player.MaxHP = Math.Max(1, player.MaxHP + hpDelta);
            // HP actual: sube con la clase, nunca baja por debajo de lo que ya tiene
            if (hpDelta > 0)
                player.HP = Math.Min(player.HP + hpDelta, player.MaxHP);
            else
                player.HP = Math.Min(player.HP, player.MaxHP);

            // 4. Ajustar MaxMana
            int manaDelta = newBonus.MaxMana - oldManaBonus;
            player.MaxMana = Math.Max(0, player.MaxMana + manaDelta);
            if (manaDelta > 0)
                player.Mana = Math.Min(player.Mana + manaDelta, player.MaxMana);
            else
                player.Mana = Math.Min(player.Mana, player.MaxMana);
        }

        // ═══════════════════════════════════════
        // CADENA DE DESCRIPCIÓN PARA LA UI
        // ═══════════════════════════════════════
        public static string GetBonusDescription(string classId)
            => GetBonuses(classId).ToDisplayString();
    }
}
