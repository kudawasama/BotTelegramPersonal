using BotTelegram.RPG.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio unificado para gestión de inventario del jugador.
    /// Maneja tanto RpgItem (consumibles/materiales) como RpgEquipment (armas/armaduras).
    /// </summary>
    public class InventoryService
    {
        private readonly RpgService _rpgService;

        public InventoryService(RpgService rpgService)
        {
            _rpgService = rpgService;
        }

        // ══════════════════════════════════════════════════════════
        // CONSUMIBLES / ITEMS (RpgItem)
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Agrega un consumible/material al inventario (máx 40 slots).
        /// </summary>
        public bool AddItem(RpgPlayer player, RpgItem item)
        {
            if (player.Inventory.Count >= 40) return false;
            player.Inventory.Add(item);
            _rpgService.SavePlayer(player);
            return true;
        }

        /// <summary>
        /// Usa un ítem consumible (por ID). Retorna mensaje de resultado.
        /// </summary>
        public (bool success, string message) UseItem(RpgPlayer player, string itemId)
        {
            var item = player.Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return (false, "❌ Ítem no encontrado en tu inventario.");

            if (item.Type == ItemType.Consumable)
            {
                var msgs = new List<string>();

                if (item.HPRestore > 0)
                {
                    var healed = Math.Min(item.HPRestore, player.MaxHP - player.HP);
                    player.HP += healed;
                    msgs.Add($"❤️ +{healed} HP");
                }
                if (item.ManaRestore > 0)
                {
                    var restored = Math.Min(item.ManaRestore, player.MaxMana - player.Mana);
                    player.Mana += restored;
                    msgs.Add($"💙 +{restored} Maná");
                }

                if (!msgs.Any()) return (false, "❌ Este ítem consumible no tiene efecto.");

                player.Inventory.Remove(item);
                _rpgService.SavePlayer(player);
                return (true, $"{item.Emoji} **{item.Name}** usado.\n{string.Join(" | ", msgs)}");
            }

            if (item.Type == ItemType.Material)
                return (false, "🔩 Los materiales se usan en la herrería para craftear.");

            if (item.Type == ItemType.Quest)
                return (false, "📜 Los ítems de misión no se pueden usar directamente.");

            return (false, "❌ No puedes usar este tipo de ítem.");
        }

        /// <summary>
        /// Vende un ítem del inventario. Retorna oro ganado o -1 si falla.
        /// </summary>
        public (bool success, string message) SellItem(RpgPlayer player, string itemId)
        {
            var item = player.Inventory.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return (false, "❌ Ítem no encontrado.");

            if (item.Type == ItemType.Quest)
                return (false, "📜 No puedes vender ítems de misión.");

            var sellValue = Math.Max(1, item.Value / 2);
            player.Gold += sellValue;
            player.Inventory.Remove(item);
            _rpgService.SavePlayer(player);
            return (true, $"💰 Vendiste **{item.Name}** por **{sellValue} oro**.");
        }

        // ══════════════════════════════════════════════════════════
        // EQUIPAMIENTO (RpgEquipment)
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Agrega equipo al inventario (máx 30 piezas).
        /// </summary>
        public bool AddEquipment(RpgPlayer player, RpgEquipment item)
        {
            if (player.EquipmentInventory.Count >= 30) return false;
            player.EquipmentInventory.Add(item);
            _rpgService.SavePlayer(player);
            return true;
        }

        /// <summary>
        /// Equipa un ítem de equipo. Retorna el ítem desplazado (si había uno equipado).
        /// </summary>
        public (bool success, string message, RpgEquipment? displaced) EquipItem(RpgPlayer player, string equipId)
        {
            var item = player.EquipmentInventory.FirstOrDefault(e => e.Id == equipId);
            if (item == null) return (false, "❌ Equipo no encontrado.", null);

            if (item.RequiredLevel > player.Level)
                return (false, $"❌ Necesitas nivel **{item.RequiredLevel}** para equipar esto.", null);

            RpgEquipment? displaced = null;

            switch (item.Type)
            {
                case EquipmentType.Weapon:
                    displaced = player.EquippedWeaponNew;
                    player.EquippedWeaponNew = item;
                    break;
                case EquipmentType.Armor:
                    displaced = player.EquippedArmorNew;
                    player.EquippedArmorNew = item;
                    break;
                case EquipmentType.Accessory:
                    displaced = player.EquippedAccessoryNew;
                    player.EquippedAccessoryNew = item;
                    break;
                default:
                    return (false, "❌ Tipo de equipo desconocido.", null);
            }

            // El ítem equipado sale del inventario; el desplazado vuelve
            player.EquipmentInventory.Remove(item);
            if (displaced != null)
                player.EquipmentInventory.Add(displaced);

            _rpgService.SavePlayer(player);
            return (true, $"✅ **{item.Name}** equipado. Bonos aplicados.", displaced);
        }

        /// <summary>
        /// Desequipa una pieza de equipo (devuelve al inventario).
        /// </summary>
        public (bool success, string message) UnequipItem(RpgPlayer player, EquipmentType slot)
        {
            RpgEquipment? item = slot switch
            {
                EquipmentType.Weapon => player.EquippedWeaponNew,
                EquipmentType.Armor => player.EquippedArmorNew,
                EquipmentType.Accessory => player.EquippedAccessoryNew,
                _ => null
            };

            if (item == null) return (false, "❌ No tienes nada equipado en ese slot.");
            if (player.EquipmentInventory.Count >= 30)
                return (false, "❌ Inventario lleno. Vende algo primero.");

            switch (slot)
            {
                case EquipmentType.Weapon: player.EquippedWeaponNew = null; break;
                case EquipmentType.Armor: player.EquippedArmorNew = null; break;
                case EquipmentType.Accessory: player.EquippedAccessoryNew = null; break;
            }

            player.EquipmentInventory.Add(item);
            _rpgService.SavePlayer(player);
            return (true, $"🔓 **{item.Name}** desequipado.");
        }

        /// <summary>
        /// Vende un equipo del inventario.
        /// </summary>
        public (bool success, string message) SellEquipment(RpgPlayer player, string equipId)
        {
            var item = player.EquipmentInventory.FirstOrDefault(e => e.Id == equipId);
            if (item == null) return (false, "❌ Equipo no encontrado.");

            var sellValue = Math.Max(1, item.Price / 2);
            player.Gold += sellValue;
            player.EquipmentInventory.Remove(item);
            _rpgService.SavePlayer(player);
            return (true, $"💰 Vendiste **{item.Name}** {item.RarityEmoji} por **{sellValue} oro**.");
        }

        // ══════════════════════════════════════════════════════════
        // DROPS EN COMBATE
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Pool de consumibles que pueden dropearse tras un combate.
        /// </summary>
        public static List<RpgItem> GetConsumableDropPool(int playerLevel)
        {
            return new List<RpgItem>
            {
                new() { Name = "Poción Menor", Emoji = "🧪", Description = "Restaura 50 HP",       Type = ItemType.Consumable, HPRestore = 50,  Value = 20, Rarity = ItemRarity.Common },
                new() { Name = "Poción de Vida", Emoji = "❤️", Description = "Restaura 150 HP",    Type = ItemType.Consumable, HPRestore = 150, Value = 60, Rarity = ItemRarity.Common },
                new() { Name = "Poción Mayor",   Emoji = "🍶", Description = "Restaura 300 HP",    Type = ItemType.Consumable, HPRestore = 300, Value = 120,Rarity = ItemRarity.Uncommon },
                new() { Name = "Poción de Maná", Emoji = "💧", Description = "Restaura 50 maná",   Type = ItemType.Consumable, ManaRestore= 50, Value = 25, Rarity = ItemRarity.Common },
                new() { Name = "Elixir de Maná", Emoji = "💎", Description = "Restaura 150 maná",  Type = ItemType.Consumable, ManaRestore= 150,Value = 80, Rarity = ItemRarity.Uncommon },
                new() { Name = "Tónico de Fuerza",Emoji ="⚡",  Description = "+30 ataque (1 batalla)",Type = ItemType.Consumable,AttackBonus=30,Value=50, Rarity= ItemRarity.Uncommon },
                new() { Name = "Fragmento de Cristal", Emoji = "🔷", Description = "Material de crafteo", Type = ItemType.Material, Value = 30, Rarity = ItemRarity.Common },
                new() { Name = "Esencia Mágica", Emoji = "✨",  Description = "Material raro de crafteo",  Type = ItemType.Material, Value = 80, Rarity = ItemRarity.Rare },
                new() { Name = "Gema Oscura",    Emoji = "🖤",  Description = "Material épico de crafteo",  Type = ItemType.Material, Value = 200,Rarity = ItemRarity.Epic },
                new() { Name = "Runa Antigua",   Emoji = "🔶",  Description = "Scroll de habilidad",        Type = ItemType.Material, Value = 150,Rarity = ItemRarity.Rare },
            };
        }

        /// <summary>
        /// Genera un drop consumible aleatorio según nivel.
        /// 25% base de chance de dropear algo.
        /// </summary>
        public static RpgItem? GenerateConsumableDrop(int playerLevel)
        {
            var rng = new Random();
            if (rng.NextDouble() > 0.25) return null; // 25% chance

            var pool = GetConsumableDropPool(playerLevel);

            // Ponderar por rareza (más comunes = más probables)
            var weights = pool.Select(i => i.Rarity switch
            {
                ItemRarity.Common   => 50,
                ItemRarity.Uncommon => 25,
                ItemRarity.Rare     => 15,
                ItemRarity.Epic     => 8,
                ItemRarity.Legendary=> 2,
                _ => 10
            }).ToList();

            int total = weights.Sum();
            int roll  = rng.Next(total);
            int cumul = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                cumul += weights[i];
                if (roll < cumul) return pool[i];
            }

            return pool[0];
        }

        // ══════════════════════════════════════════════════════════
        // ESTADÍSTICAS DE EQUIPO ACTIVO
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Genera texto con el resumen de equipo equipado actualmente.
        /// </summary>
        public static string GetEquippedSummary(RpgPlayer player)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("⚔️ **EQUIPO ACTUAL**\n");

            if (player.EquippedWeaponNew != null)
                sb.AppendLine($"⚔️ Arma: **{player.EquippedWeaponNew.Name}** {player.EquippedWeaponNew.RarityEmoji}");
            else
                sb.AppendLine("⚔️ Arma: _Sin equipar_");

            if (player.EquippedArmorNew != null)
                sb.AppendLine($"🛡️ Armadura: **{player.EquippedArmorNew.Name}** {player.EquippedArmorNew.RarityEmoji}");
            else
                sb.AppendLine("🛡️ Armadura: _Sin equipar_");

            if (player.EquippedAccessoryNew != null)
                sb.AppendLine($"💍 Accesorio: **{player.EquippedAccessoryNew.Name}** {player.EquippedAccessoryNew.RarityEmoji}");
            else
                sb.AppendLine("💍 Accesorio: _Sin equipar_");

            var totalAttack = (player.EquippedWeaponNew?.BonusAttack ?? 0);
            var totalDef    = (player.EquippedArmorNew?.BonusDefense ?? 0);
            var totalHP     = (player.EquippedWeaponNew?.BonusHP ?? 0) + (player.EquippedArmorNew?.BonusHP ?? 0) + (player.EquippedAccessoryNew?.BonusHP ?? 0);

            sb.AppendLine($"\n📊 Bonos totales: ⚔️+{totalAttack} | 🛡️+{totalDef} | ❤️+{totalHP}");
            return sb.ToString();
        }
    }
}
