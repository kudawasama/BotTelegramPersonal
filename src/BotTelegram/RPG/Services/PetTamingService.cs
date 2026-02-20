using BotTelegram.RPG.Models;

namespace BotTelegram.RPG.Services
{
    /// <summary>
    /// Servicio para domar y manejar mascotas
    /// </summary>
    public class PetTamingService
    {
        private static readonly Random _random = new();
        private readonly RpgService _rpgService;
        
        public PetTamingService(RpgService rpgService)
        {
            _rpgService = rpgService;
        }
        
        /// <summary>
        /// Intenta domar una bestia enemiga
        /// </summary>
        public (bool success, string message, RpgPet? pet) AttemptTame(RpgPlayer player, RpgEnemy enemy)
        {
            // Solo bestias pueden ser domadas
            if (enemy.Type != EnemyType.Beast)
            {
                return (false, "❌ Solo las bestias pueden ser domadas.", null);
            }
            
            // El enemigo debe estar debilitado (<50% HP)
            double hpPercent = (double)enemy.HP / enemy.MaxHP;
            if (hpPercent > 0.5)
            {
                return (false, $"⚠️ {enemy.Name} aún está muy fuerte. Debe tener menos del 50% HP para domarlo.", null);
            }
            
            // Calcular chance de éxito basado en Charisma y HP restante
            double baseChance = 0.40; // 40% base
            double charismaBonus = player.Charisma * 0.01; // +1% por punto de Charisma
            double weaknessBonus = (1 - hpPercent) * 0.30; // +30% adicional si está en 0% HP
            
            double successChance = Math.Min(0.95, baseChance + charismaBonus + weaknessBonus);
            
            bool success = _random.NextDouble() <= successChance;
            
            if (!success)
            {
                return (false, $"⚠️ El intento de domar a {enemy.Name} falló. (Chance: {successChance:P0})", null);
            }
            
            // Crear mascota basada en el enemigo
            string speciesId = GetSpeciesIdFromEnemy(enemy);
            var pet = PetDatabase.CreatePet(speciesId, enemy.Name);
            
            // Ajustar bond inicial basado en cómo fue la pelea
            int initialBond = 200; // Neutral por defecto
            if (hpPercent < 0.10)
            {
                initialBond += 100; // Bonus si lo domaste casi muerto (más impresionante)
            }
            
            pet.Bond = initialBond;
            pet.UpdateLoyalty();
            
            // Agregar al inventario de mascotas
            player.PetInventory.Add(pet);
            
            // Si hay espacio en active pets, agregarlo
            if (player.ActivePets.Count < player.MaxActivePets)
            {
                player.ActivePets.Add(pet);
            }
            
            _rpgService.SavePlayer(player);
            
            return (true, $"✅ **¡Has domado a {pet.Name}!**\n🐾 Rarity: {pet.RarityEmoji} {pet.Rarity}\n💙 Bond: {pet.Bond}/1000 ({pet.Loyalty})\n⚔️ Stats: {pet.MaxHP} HP | {pet.Attack} ATK | {pet.Defense} DEF", pet);
        }
        
        /// <summary>
        /// Acaricia a una bestia para aumentar bond
        /// </summary>
        public (bool canPet, string message) PetBeast(RpgPlayer player, RpgEnemy enemy, ActionTrackerService tracker)
        {
            // Solo bestias pueden ser acariciadas
            if (enemy.Type != EnemyType.Beast)
            {
                return (false, "❌ Solo puedes acariciar bestias salvajes.");
            }
            
            // El enemigo debe estar vivo pero derrotado/debilitado (<30% HP)
            double hpPercent = (double)enemy.HP / enemy.MaxHP;
            if (enemy.HP <= 0)
            {
                return (false, "❌ No puedes acariciar un enemigo muerto.");
            }
            
            if (hpPercent > 0.30)
            {
                return (false, "⚠️ La bestia está muy agresiva. Debe estar más debilitada (<30% HP).");
            }
            
            // Aumentar bond con bestias (para futuro taming)
            // También trackea la acción para Beast Tamer class
            tracker.TrackAction(player, "pet_beast");
            
            // 15% chance de domar instantáneamente
            if (_random.NextDouble() <= 0.15)
            {
                var (success, message, pet) = AttemptTame(player, enemy);
                if (success)
                {
                    return (true, $"🌟 **¡Evento especial!**\nMientras acariciabas la bestia, establecieron un vínculo instantáneo!\n\n{message}");
                }
            }
            
            return (true, $"🐾 Acariciaste a {enemy.Name}.\n✅ Se siente más tranquilo.\n📊 Acción registrada: Acariciar Bestia ({tracker.GetActionCount(player, "pet_beast")})");
        }
        
        /// <summary>
        /// Calma a una bestia durante combate
        /// </summary>
        public (bool success, string message) CalmBeast(RpgPlayer player, RpgEnemy enemy, ActionTrackerService tracker)
        {
            if (enemy.Type != EnemyType.Beast)
            {
                return (false, "❌ Solo las bestias pueden ser calmadas.");
            }
            
            if (player.Mana < 20)
            {
                return (false, "❌ No tienes suficiente mana. (Requiere 20 Mana)");
            }
            
            player.Mana -= 20;
            
            // Cambiar comportamiento del enemigo a pasivo por 2 turnos
            enemy.Behavior = EnemyBehavior.Passive;
            
            tracker.TrackAction(player, "calm_beast");
            
            return (true, $"🎶 Has calmado a {enemy.Name}.\n✨ No atacará los próximos 2 turnos.\n📊 Acción registrada: Calmar Bestia ({tracker.GetActionCount(player, "calm_beast")})");
        }
        
        /// <summary>
        /// Alimenta a una mascota para aumentar bond
        /// </summary>
        public string FeedPet(RpgPlayer player, RpgPet pet)
        {
            // Verificar que el jugador tenga comida (por ahora dummy)
            bool hasFood = player.Gold >= 5; //5 gold por comida por ahora
            if (!hasFood)
            {
                return "❌ No tienes comida. (Requiere 5 Gold)";
            }
            
            player.Gold -= 5;
            
            // Aumentar bond
            pet.IncreaseBond(20);
            
            // Curar 30% HP
            int healAmount = (int)(pet.MaxHP * 0.30);
            pet.HP = Math.Min(pet.MaxHP, pet.HP + healAmount);
            
            pet.LastFed = DateTime.UtcNow;
            
            _rpgService.SavePlayer(player);
            
            return $"🍖 Alimentaste a {pet.Name}.\n" +
                   $"❤️ HP: +{healAmount} ({pet.HP}/{pet.MaxHP})\n" +
                   $"💙 Bond: +20 ({pet.Bond}/1000 - {pet.LoyaltyEmoji} {pet.Loyalty})";
        }
        
        /// <summary>
        /// Mapea enemigos a especies de mascotas
        /// </summary>
        private string GetSpeciesIdFromEnemy(RpgEnemy enemy)
        {
            return enemy.Name.ToLower() switch
            {
                "lobo salvaje" => "wolf_1",
                "oso" => "bear_1",
                "oso pardo" => "bear_1",
                "águila" => "eagle_1",
                "serpiente" => "snake_1",
                "serpiente venenosa" => "snake_1",
                "gato montés" => "cat_1",
                "dragón bebé" => "dragon_1",
                _ => "wolf_1" // Default: wolf
            };
        }
        
        /// <summary>
        /// Activa/desactiva una mascota del equipo activo
        /// </summary>
        public string ToggleActivePet(RpgPlayer player, string petId)
        {
            var pet = player.PetInventory.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return "❌ Mascota no encontrada.";
            }
            
            // Si ya está activa, removerla
            if (player.ActivePets.Any(p => p.Id == petId))
            {
                player.ActivePets.RemoveAll(p => p.Id == petId);
                _rpgService.SavePlayer(player);
                return $"📤 {pet.Name} ha sido desactivado.";
            }
            
            // Si no hay espacio
            if (player.ActivePets.Count >= player.MaxActivePets)
            {
                return $"❌ No hay espacio. Máximo: {player.MaxActivePets} mascotas activas.";
            }
            
            // Activar
            player.ActivePets.Add(pet);
            _rpgService.SavePlayer(player);
            
            return $"✅ {pet.Name} está ahora activo en combate!\n" +
                   $"⚔️ ATK: {pet.EffectiveAttack} | 🛡️ DEF: {pet.EffectiveDefense} | ⚡ SPD: {pet.Speed}";
        }
        
        /// <summary>
        /// Libera una mascota a la naturaleza (desaparece, sin recompensa)
        /// </summary>
        public string ReleasePet(RpgPlayer player, string petId)
        {
            var pet = player.PetInventory.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return "❌ Mascota no encontrada.";
            }
            
            string petName = pet.Name;
            
            // Remover del inventario
            player.PetInventory.RemoveAll(p => p.Id == petId);
            
            // Si estaba activa, removerla del equipo
            if (player.ActivePets.Any(p => p.Id == petId))
            {
                player.ActivePets.RemoveAll(p => p.Id == petId);
            }
            
            _rpgService.SavePlayer(player);
            
            return $"🌳 Has liberado a {petName} a la naturaleza.\n" +
                   $"✨ Se siente libre y salvaje nuevamente.\n" +
                   $"💔 Adiós, amigo...";
        }
        
        /// <summary>
        /// Vende una mascota por oro (basado en rarity y nivel)
        /// </summary>
        public string SellPet(RpgPlayer player, string petId)
        {
            var pet = player.PetInventory.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return "❌ Mascota no encontrada.";
            }
            
            string petName = pet.Name;
            
            // Calcular valor basado en rarity y nivel
            int baseValue = pet.Level * 50;
            int rarityMultiplier = pet.Rarity switch
            {
                BotTelegram.RPG.Models.PetRarity.Common => 1,
                BotTelegram.RPG.Models.PetRarity.Uncommon => 2,
                BotTelegram.RPG.Models.PetRarity.Rare => 4,
                BotTelegram.RPG.Models.PetRarity.Epic => 8,
                BotTelegram.RPG.Models.PetRarity.Legendary => 16,
                BotTelegram.RPG.Models.PetRarity.Mythical => 32,
                _ => 1
            };
            
            int goldEarned = baseValue * rarityMultiplier;
            
            // Remover del inventario
            player.PetInventory.RemoveAll(p => p.Id == petId);
            
            // Si estaba activa, removerla del equipo
            if (player.ActivePets.Any(p => p.Id == petId))
            {
                player.ActivePets.RemoveAll(p => p.Id == petId);
            }
            
            // Agregar oro
            player.Gold += goldEarned;
            
            _rpgService.SavePlayer(player);
            
            return $"💰 Has vendido a {petName} {pet.RarityEmoji}.\n" +
                   $"🪙 Ganaste: {goldEarned} Gold\n" +
                   $"💵 Total: {player.Gold} Gold\n\n" +
                   $"⚠️ El viajero se lleva a tu mascota...";
        }
    }
}
