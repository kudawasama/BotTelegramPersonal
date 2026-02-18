using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>Servicio de crafteo (Fase 8)</summary>
    public static class CraftingService
    {
        /// <summary>Verifica si el jugador tiene todos los ingredientes.</summary>
        public static (bool Ok, List<string> Missing) CheckIngredients(RpgPlayer player, CraftRecipe recipe)
        {
            var missing = new List<string>();
            foreach (var ingredient in recipe.Ingredients)
            {
                int have = player.Inventory.Count(i =>
                    i.Name.Equals(ingredient.ItemName, StringComparison.OrdinalIgnoreCase));
                if (have < ingredient.Quantity)
                {
                    int need = ingredient.Quantity - have;
                    missing.Add($"{ingredient.ItemName} ×{need}");
                }
            }
            return (missing.Count == 0, missing);
        }

        /// <summary>Consume los ingredientes del inventario del jugador.</summary>
        private static void ConsumeIngredients(RpgPlayer player, CraftRecipe recipe)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                int toRemove = ingredient.Quantity;
                for (int i = player.Inventory.Count - 1; i >= 0 && toRemove > 0; i--)
                {
                    if (player.Inventory[i].Name.Equals(ingredient.ItemName, StringComparison.OrdinalIgnoreCase))
                    {
                        player.Inventory.RemoveAt(i);
                        toRemove--;
                    }
                }
            }
        }

        /// <summary>Ejecuta el crafteo. Devuelve (true, mensaje, item?, equip?).</summary>
        public static (bool Success, string Message, RpgItem? Item, RpgEquipment? Equipment)
            Craft(RpgPlayer player, string recipeId)
        {
            var recipe = CraftingDatabase.GetById(recipeId);
            if (recipe is null)
                return (false, "❌ Receta no encontrada.", null, null);

            if (player.Level < recipe.RequiredLevel)
                return (false, $"❌ Necesitas nivel {recipe.RequiredLevel} para esta receta.", null, null);

            var (ok, missing) = CheckIngredients(player, recipe);
            if (!ok)
                return (false, $"❌ Faltan ingredientes:\n• {string.Join("\n• ", missing)}", null, null);

            // Consumir materiales
            ConsumeIngredients(player, recipe);

            // Producir resultado
            if (recipe.ResultType == CraftResultType.Item)
            {
                var item = new RpgItem
                {
                    Name        = recipe.ResultName,
                    Emoji       = recipe.ResultEmoji,
                    Description = recipe.ResultDesc,
                    Type        = ItemType.Consumable,
                    Rarity      = recipe.ResultRarity,
                    HPRestore   = recipe.ResultHPRestore,
                    ManaRestore = recipe.ResultManaRestore,
                    Value       = recipe.ResultValue > 0 ? recipe.ResultValue : 50
                };
                player.Inventory.Add(item);
                return (true, $"✅ ¡Crafteaste **{recipe.ResultEmoji} {recipe.ResultName}**!", item, null);
            }
            else // Equipment
            {
                // Intentar obtener del EquipmentDatabase; si no existe, crear uno genérico
                RpgEquipment? equip = null;
                if (!string.IsNullOrEmpty(recipe.ResultEquipmentId))
                    equip = EquipmentDatabase.GetById(recipe.ResultEquipmentId);

                if (equip is null)
                {
                    equip = new RpgEquipment
                    {
                        Id          = recipe.ResultEquipmentId ?? recipe.Id,
                        Name        = recipe.ResultName,
                        Description = recipe.ResultDesc,
                        Rarity      = (EquipmentRarity)(int)recipe.ResultRarity,
                        BonusAttack = recipe.ResultRarity switch
                        {
                            ItemRarity.Uncommon => 15,
                            ItemRarity.Rare     => 30,
                            ItemRarity.Epic     => 55,
                            _                   => 10
                        },
                        BonusDefense = recipe.ResultRarity switch
                        {
                            ItemRarity.Uncommon => 10,
                            ItemRarity.Rare     => 20,
                            ItemRarity.Epic     => 40,
                            _                   => 5
                        },
                        Price = recipe.ResultRarity switch
                        {
                            ItemRarity.Uncommon => 600,
                            ItemRarity.Rare     => 1200,
                            ItemRarity.Epic     => 2500,
                            _                   => 300
                        }
                    };
                }

                player.EquipmentInventory.Add(equip);
                return (true, $"✅ ¡Forjaste **{recipe.ResultEmoji} {recipe.ResultName}**! ({recipe.ResultRarity})", null, equip);
            }
        }

        /// <summary>Texto con la lista de ingredientes de una receta, marcando cuáles faltan.</summary>
        public static string IngredientStatusText(RpgPlayer player, CraftRecipe recipe)
        {
            var lines = new List<string>();
            foreach (var ing in recipe.Ingredients)
            {
                int have = player.Inventory.Count(i =>
                    i.Name.Equals(ing.ItemName, StringComparison.OrdinalIgnoreCase));
                string check = have >= ing.Quantity ? "✅" : "❌";
                lines.Add($"{check} {ing.ItemName} {have}/{ing.Quantity}");
            }
            return string.Join("\n", lines);
        }
    }
}
