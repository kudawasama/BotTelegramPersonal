using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Base de datos de recetas de crafteo (Fase 8)</summary>
    public static class CraftingDatabase
    {
        private static readonly List<CraftRecipe> _recipes = new()
        {
            // ── TIER 1: Pociones ────────────────────────────────────────────
            new CraftRecipe
            {
                Id          = "pocion_mayor",
                Name        = "Poción Mayor de Vida",
                Emoji       = "🧪",
                Description = "Restaura 300 HP. Elaborada con cristales purificados.",
                RequiredLevel = 1,
                ResultType  = CraftResultType.Item,
                ResultName  = "Poción Mayor de Vida",
                ResultEmoji = "🧪",
                ResultDesc  = "Restaura 300 HP",
                ResultHPRestore = 300,
                ResultRarity = ItemRarity.Uncommon,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Fragmento de Cristal", Quantity = 2 },
                    new CraftIngredient { ItemName = "Esencia Mágica",       Quantity = 1 }
                }
            },
            new CraftRecipe
            {
                Id          = "elixir_mana",
                Name        = "Elixir de Maná",
                Emoji       = "💧",
                Description = "Restaura 200 Maná. Elaborado con pura esencia mágica.",
                RequiredLevel = 1,
                ResultType  = CraftResultType.Item,
                ResultName  = "Elixir de Maná",
                ResultEmoji = "💧",
                ResultDesc  = "Restaura 200 Maná",
                ResultManaRestore = 200,
                ResultRarity = ItemRarity.Uncommon,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Esencia Mágica", Quantity = 2 }
                }
            },
            new CraftRecipe
            {
                Id          = "pocion_suprema",
                Name        = "Poción Suprema",
                Emoji       = "⚗️",
                Description = "Restaura todo el HP. Solo los alquimistas más hábiles la crean.",
                RequiredLevel = 5,
                ResultType  = CraftResultType.Item,
                ResultName  = "Poción Suprema",
                ResultEmoji = "⚗️",
                ResultDesc  = "Restaura todo el HP",
                ResultHPRestore = 9999,
                ResultRarity = ItemRarity.Rare,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Gema Oscura",    Quantity = 1 },
                    new CraftIngredient { ItemName = "Esencia Mágica", Quantity = 2 },
                    new CraftIngredient { ItemName = "Runa Antigua",   Quantity = 1 }
                }
            },
            new CraftRecipe
            {
                Id          = "tonico_fuerza",
                Name        = "Tónico de Fuerza",
                Emoji       = "💪",
                Description = "Aumenta el ataque en el siguiente combate (+30 ATK temporal).",
                RequiredLevel = 3,
                ResultType  = CraftResultType.Item,
                ResultName  = "Tónico de Fuerza",
                ResultEmoji = "💪",
                ResultDesc  = "+30 ATK temporal",
                ResultValue = 30,
                ResultRarity = ItemRarity.Uncommon,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Runa Antigua",       Quantity = 1 },
                    new CraftIngredient { ItemName = "Fragmento de Cristal", Quantity = 1 }
                }
            },

            // ── TIER 2: Equipos básicos ─────────────────────────────────────
            new CraftRecipe
            {
                Id          = "espada_cristal",
                Name        = "Espada de Cristal",
                Emoji       = "⚔️",
                Description = "Arma forjada con fragmentos de cristal. Brilla con luz mágica.",
                RequiredLevel = 5,
                ResultType  = CraftResultType.Equipment,
                ResultName  = "Espada de Cristal",
                ResultEquipmentId = "weapon_crystal_sword",
                ResultEmoji = "⚔️",
                ResultDesc  = "Espada mágica Poco Común",
                ResultRarity = ItemRarity.Uncommon,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Fragmento de Cristal", Quantity = 5 },
                    new CraftIngredient { ItemName = "Esencia Mágica",       Quantity = 2 }
                }
            },
            new CraftRecipe
            {
                Id          = "vara_magica",
                Name        = "Vara Mágica",
                Emoji       = "🪄",
                Description = "Vara imbuida con esencia arcana. Amplifica los hechizos.",
                RequiredLevel = 5,
                ResultType  = CraftResultType.Equipment,
                ResultName  = "Vara Mágica",
                ResultEquipmentId = "weapon_magic_wand",
                ResultEmoji = "🪄",
                ResultDesc  = "Bastón mágico Poco Común",
                ResultRarity = ItemRarity.Uncommon,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Esencia Mágica", Quantity = 4 },
                    new CraftIngredient { ItemName = "Runa Antigua",   Quantity = 1 }
                }
            },
            new CraftRecipe
            {
                Id          = "armadura_reforzada",
                Name        = "Armadura Reforzada",
                Emoji       = "🛡️",
                Description = "Armadura forjada con cristales y gemas. Alta resistencia.",
                RequiredLevel = 5,
                ResultType  = CraftResultType.Equipment,
                ResultName  = "Armadura Reforzada",
                ResultEquipmentId = "armor_reinforced",
                ResultEmoji = "🛡️",
                ResultDesc  = "Armadura Rara",
                ResultRarity = ItemRarity.Rare,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Fragmento de Cristal", Quantity = 3 },
                    new CraftIngredient { ItemName = "Gema Oscura",          Quantity = 1 }
                }
            },

            // ── TIER 3: Equipos avanzados (Lv 10+) ─────────────────────────
            new CraftRecipe
            {
                Id          = "arma_runica",
                Name        = "Arma Rúnica",
                Emoji       = "🗡️",
                Description = "Arma épica grabada con runas ancestrales. Poder devastador.",
                RequiredLevel = 10,
                ResultType  = CraftResultType.Equipment,
                ResultName  = "Arma Rúnica",
                ResultEquipmentId = "weapon_runic_blade",
                ResultEmoji = "🗡️",
                ResultDesc  = "Arma Épica",
                ResultRarity = ItemRarity.Epic,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Runa Antigua",       Quantity = 3 },
                    new CraftIngredient { ItemName = "Gema Oscura",        Quantity = 2 },
                    new CraftIngredient { ItemName = "Esencia Mágica",     Quantity = 3 }
                }
            },
            new CraftRecipe
            {
                Id          = "manto_sombras",
                Name        = "Manto de Sombras",
                Emoji       = "🧥",
                Description = "Capa élfica imbuida con gemas oscuras. Incrementa la evasión.",
                RequiredLevel = 10,
                ResultType  = CraftResultType.Equipment,
                ResultName  = "Manto de Sombras",
                ResultEquipmentId = "armor_shadow_cloak",
                ResultEmoji = "🧥",
                ResultDesc  = "Capa Épica",
                ResultRarity = ItemRarity.Epic,
                Ingredients = new()
                {
                    new CraftIngredient { ItemName = "Gema Oscura",        Quantity = 3 },
                    new CraftIngredient { ItemName = "Runa Antigua",       Quantity = 2 },
                    new CraftIngredient { ItemName = "Fragmento de Cristal", Quantity = 4 }
                }
            },
        };

        public static IReadOnlyList<CraftRecipe> AllRecipes => _recipes;

        public static CraftRecipe? GetById(string id) =>
            _recipes.FirstOrDefault(r => r.Id == id);

        public static List<CraftRecipe> GetAvailableFor(int level) =>
            _recipes.Where(r => r.RequiredLevel <= level).ToList();
    }
}
