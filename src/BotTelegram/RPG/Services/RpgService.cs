using BotTelegram.RPG.Models;
using System.Text.Json;

namespace BotTelegram.RPG.Services
{
    public class RpgService
    {
        private readonly string _filePath;
        private static readonly object _fileLock = new();
        private static readonly Random _random = new();
        
        // Sistema de estados para creación de personajes
        private static readonly HashSet<long> _awaitingName = new();
        private static readonly HashSet<long> _awaitingImport = new();
        private static readonly object _stateLock = new();
        
        public static void SetAwaitingName(long chatId, bool awaiting)
        {
            lock (_stateLock)
            {
                if (awaiting)
                    _awaitingName.Add(chatId);
                else
                    _awaitingName.Remove(chatId);
            }
        }
        
        public static bool IsAwaitingName(long chatId)
        {
            lock (_stateLock)
            {
                return _awaitingName.Contains(chatId);
            }
        }
        
        public static void SetAwaitingImport(long chatId, bool awaiting)
        {
            lock (_stateLock)
            {
                if (awaiting)
                    _awaitingImport.Add(chatId);
                else
                    _awaitingImport.Remove(chatId);
            }
        }
        
        public static bool IsAwaitingImport(long chatId)
        {
            lock (_stateLock)
            {
                return _awaitingImport.Contains(chatId);
            }
        }
        
        public RpgService()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var projectRoot = currentDir;
            
            while (!File.Exists(Path.Combine(projectRoot, "BotTelegram.csproj")))
            {
                var parent = Directory.GetParent(projectRoot);
                if (parent == null)
                {
                    projectRoot = currentDir;
                    break;
                }
                projectRoot = parent.FullName;
            }
            
            var dataDir = Path.Combine(projectRoot, "data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            
            _filePath = Path.Combine(dataDir, "rpg_players.json");
            Console.WriteLine($"[RpgService] 📁 Usando ruta: {_filePath}");
            
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
                Console.WriteLine($"[RpgService] ✅ Creado archivo de jugadores");
            }
        }
        
        // CRUD Operations
        public RpgPlayer? GetPlayer(long chatId)
        {
            lock (_fileLock)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    var players = JsonSerializer.Deserialize<List<RpgPlayer>>(json) ?? new List<RpgPlayer>();
                    var player = players.FirstOrDefault(p => p.ChatId == chatId);
                    
                    // MIGRACIÓN AUTOMÁTICA: Actualizar campos nuevos sin borrar progreso
                    if (player != null)
                    {
                        MigratePlayerData(player);
                    }
                    
                    return player;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RpgService] ❌ Error leyendo jugador: {ex.Message}");
                    return null;
                }
            }
        }
        
        public void SavePlayer(RpgPlayer player)
        {
            lock (_fileLock)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    var players = JsonSerializer.Deserialize<List<RpgPlayer>>(json) ?? new List<RpgPlayer>();
                    
                    var existing = players.FirstOrDefault(p => p.ChatId == player.ChatId);
                    if (existing != null)
                    {
                        players.Remove(existing);
                    }
                    
                    player.LastPlayedAt = DateTime.UtcNow;
                    players.Add(player);
                    
                    var options = new JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                    };
                    
                    json = JsonSerializer.Serialize(players, options);
                    File.WriteAllText(_filePath, json);
                    
                    Console.WriteLine($"[RpgService] 💾 Jugador guardado: {player.Name} (Lv.{player.Level})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RpgService] ❌ Error guardando jugador: {ex.Message}");
                }
            }
        }
        
        public List<RpgPlayer> GetAllPlayers()
        {
            lock (_fileLock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        Console.WriteLine("[RpgService] Archivo no existe aún, retornando lista vacía");
                        return new List<RpgPlayer>();
                    }

                    var json = File.ReadAllText(_filePath);
                    var players = JsonSerializer.Deserialize<List<RpgPlayer>>(json) ?? new List<RpgPlayer>();
                    
                    Console.WriteLine($"[RpgService] Cargados {players.Count} jugadores desde archivo");
                    return players;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RpgService] ❌ Error al cargar todos los jugadores: {ex.Message}");
                    return new List<RpgPlayer>();
                }
            }
        }
        
        public RpgPlayer CreateNewPlayer(long chatId, string name, CharacterClass characterClass = CharacterClass.Adventurer)
        {
            // FASE 4: Todos los jugadores inician como Adventurer (clase base)
            // Las clases se desbloquean jugando según acciones realizadas
            var player = new RpgPlayer
            {
                ChatId = chatId,
                Name = name,
                Class = CharacterClass.Adventurer,
                Level = 1,
                XP = 0,
                HP = 100,
                MaxHP = 100,
                Energy = 50,
                MaxEnergy = 50,
                Mana = 0,
                MaxMana = 0,
                Strength = 10,
                Intelligence = 10,
                Dexterity = 10,
                Constitution = 10,
                Wisdom = 10,
                Charisma = 10,
                Gold = 50,
                Inventory = new List<RpgItem>
                {
                    new RpgItem
                    {
                        Name = "Poción de Vida",
                        Emoji = "🧪",
                        Description = "Restaura 30 HP",
                        Type = ItemType.Consumable,
                        HPRestore = 30,
                        Value = 15,
                        Rarity = ItemRarity.Common
                    }
                },
                CurrentLocation = "Taberna de Puerto Esperanza",
                ActiveClassId = "adventurer",
                UnlockedClasses = new List<string> { "adventurer" }
            };
            
            // Stats de Adventurer (balanceados, sin especialización)
            // Las clases Tier 1+ se desbloquean con el sistema de ActionTracker
            // No hay equipo inicial, el jugador debe conseguir items explorando
            
            SavePlayer(player);
            Console.WriteLine($"[RpgService] ✨ Nuevo jugador creado: {name} (Adventurer)");
            return player;
        }
        
        public void DeletePlayer(long chatId)
        {
            lock (_fileLock)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    var players = JsonSerializer.Deserialize<List<RpgPlayer>>(json) ?? new List<RpgPlayer>();
                    players.RemoveAll(p => p.ChatId == chatId);
                    
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    json = JsonSerializer.Serialize(players, options);
                    File.WriteAllText(_filePath, json);
                    
                    Console.WriteLine($"[RpgService] 🗑️ Jugador eliminado: {chatId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RpgService] ❌ Error eliminando jugador: {ex.Message}");
                }
            }
        }
        
        // Game Mechanics
        public (bool Success, string Message) EquipItem(RpgPlayer player, string equipmentId)
        {
            var item = player.EquipmentInventory.FirstOrDefault(e => e.Id == equipmentId);
            if (item == null)
            {
                return (false, "❌ No tienes ese equipo en el inventario.");
            }
            
            if (player.Level < item.RequiredLevel)
            {
                return (false, $"❌ Necesitas nivel {item.RequiredLevel} para equipar esto.");
            }
            
            var current = GetEquippedByType(player, item.Type);
            if (current != null)
            {
                player.EquipmentInventory.Add(current);
            }
            
            ApplyEquipmentResources(player, current, item);
            SetEquippedByType(player, item);
            player.EquipmentInventory.Remove(item);
            
            return (true, $"✅ Equipaste {item.Name}.");
        }
        
        public (bool Success, string Message) UnequipItem(RpgPlayer player, EquipmentType type)
        {
            var current = GetEquippedByType(player, type);
            if (current == null)
            {
                return (false, "❌ No tienes nada equipado en ese slot.");
            }
            
            ApplyEquipmentResources(player, current, null);
            player.EquipmentInventory.Add(current);
            ClearEquippedByType(player, type);
            
            return (true, $"✅ Desequipaste {current.Name}.");
        }
        
        public List<RpgEquipment> GetEquipmentInventory(RpgPlayer player, EquipmentType? type = null)
        {
            var items = player.EquipmentInventory ?? new List<RpgEquipment>();
            if (!type.HasValue)
            {
                return items;
            }
            
            return items.Where(e => e.Type == type.Value).ToList();
        }
        
        public void AddXP(RpgPlayer player, int xp)
        {
            player.XP += xp;
            Console.WriteLine($"[RpgService] ⭐ {player.Name} ganó {xp} XP (Total: {player.XP}/{player.XPNeeded})");
            
            // Check level up
            while (player.XP >= player.XPNeeded && player.Level < 50)
            {
                LevelUp(player);
            }
        }
        
        private void LevelUp(RpgPlayer player)
        {
            // Guardar XP necesario ANTES de incrementar nivel
            var xpNeeded = player.XPNeeded;
            
            player.Level++;
            player.XP -= xpNeeded; // Usar el valor guardado
            
            // Stat increases
            player.MaxHP += 10;
            player.HP = player.MaxHP; // Full heal on level up
            player.MaxEnergy += 5;
            player.Energy = player.MaxEnergy;
            
            player.Strength += 2;
            player.Intelligence += 2;
            player.Dexterity += 2;
            player.Constitution += 2;
            player.Wisdom += 1;
            player.Charisma += 1;
            
            Console.WriteLine($"[RpgService] 🎉 ¡{player.Name} subió a nivel {player.Level}!");
        }
        
        public bool CanPerformAction(RpgPlayer player, int energyCost)
        {
            if (player.IsInCombat)
            {
                return false;
            }
            
            return player.Energy >= energyCost;
        }
        
        public void ConsumeEnergy(RpgPlayer player, int amount)
        {
            player.Energy = Math.Max(0, player.Energy - amount);
        }
        
        public void RestoreEnergy(RpgPlayer player, int amount)
        {
            player.Energy = Math.Min(player.MaxEnergy, player.Energy + amount);
        }
        
        public void RestoreHP(RpgPlayer player, int amount)
        {
            player.HP = Math.Min(player.MaxHP, player.HP + amount);
        }

        private static RpgEquipment? GetEquippedByType(RpgPlayer player, EquipmentType type)
        {
            return type switch
            {
                EquipmentType.Weapon => player.EquippedWeaponNew,
                EquipmentType.Armor => player.EquippedArmorNew,
                EquipmentType.Accessory => player.EquippedAccessoryNew,
                _ => null
            };
        }
        
        private static void SetEquippedByType(RpgPlayer player, RpgEquipment item)
        {
            switch (item.Type)
            {
                case EquipmentType.Weapon:
                    player.EquippedWeaponNew = item;
                    break;
                case EquipmentType.Armor:
                    player.EquippedArmorNew = item;
                    break;
                case EquipmentType.Accessory:
                    player.EquippedAccessoryNew = item;
                    break;
            }
        }
        
        private static void ClearEquippedByType(RpgPlayer player, EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    player.EquippedWeaponNew = null;
                    break;
                case EquipmentType.Armor:
                    player.EquippedArmorNew = null;
                    break;
                case EquipmentType.Accessory:
                    player.EquippedAccessoryNew = null;
                    break;
            }
        }
        
        private static void ApplyEquipmentResources(RpgPlayer player, RpgEquipment? oldItem, RpgEquipment? newItem)
        {
            var oldHp = oldItem?.BonusHP ?? 0;
            var oldMana = oldItem?.BonusMana ?? 0;
            var oldStamina = oldItem?.BonusStamina ?? 0;
            
            var newHp = newItem?.BonusHP ?? 0;
            var newMana = newItem?.BonusMana ?? 0;
            var newStamina = newItem?.BonusStamina ?? 0;
            
            player.MaxHP = Math.Max(1, player.MaxHP - oldHp + newHp);
            player.MaxMana = Math.Max(0, player.MaxMana - oldMana + newMana);
            player.MaxStamina = Math.Max(0, player.MaxStamina - oldStamina + newStamina);
            
            player.HP = Math.Min(player.HP, player.MaxHP);
            player.Mana = Math.Min(player.Mana, player.MaxMana);
            player.Stamina = Math.Min(player.Stamina, player.MaxStamina);
        }
        
        // Enemy Generation
        public RpgEnemy GenerateEnemy(int playerLevel, EnemyDifficulty difficulty)
        {
            var enemies = difficulty switch
            {
                EnemyDifficulty.Easy => EnemyDatabase.GetEasyEnemies(),
                EnemyDifficulty.Medium => EnemyDatabase.GetMediumEnemies(),
                EnemyDifficulty.Hard => EnemyDatabase.GetHardEnemies(),
                EnemyDifficulty.Boss => EnemyDatabase.GetBossEnemies(),
                _ => EnemyDatabase.GetEasyEnemies()
            };
            
            var template = enemies[_random.Next(enemies.Count)];
            
            // Scale to player level
            var levelDiff = difficulty switch
            {
                EnemyDifficulty.Easy => -1,
                EnemyDifficulty.Medium => 0,
                EnemyDifficulty.Hard => 1,
                EnemyDifficulty.Elite => 2,
                EnemyDifficulty.Boss => 3,
                _ => 0
            };
            
            return EnemyDatabase.ScaleEnemy(template, playerLevel, levelDiff);
        }
        
        // ═══════════════════════════════════════════════════════════════
        // SISTEMA DE EXPORT/IMPORT Y MIGRACIÓN (FASE 5 - PERSISTENCIA)
        // ═══════════════════════════════════════════════════════════════
        
        /// <summary>
        /// Exporta el personaje como JSON para backup
        /// </summary>
        public string ExportPlayerData(RpgPlayer player)
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                };
                
                var json = JsonSerializer.Serialize(player, options);
                Console.WriteLine($"[RpgService] 📤 Personaje exportado: {player.Name} (Lv.{player.Level})");
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RpgService] ❌ Error exportando personaje: {ex.Message}");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Importa un personaje desde JSON
        /// </summary>
        public RpgPlayer? ImportPlayerData(string json, long chatId)
        {
            try
            {
                var player = JsonSerializer.Deserialize<RpgPlayer>(json);
                
                if (player == null)
                {
                    Console.WriteLine($"[RpgService] ❌ JSON inválido al importar");
                    return null;
                }
                
                // Asegurar que use el chatId correcto
                player.ChatId = chatId;
                
                // Migrar datos si es necesario
                MigratePlayerData(player);
                
                // Guardar
                SavePlayer(player);
                
                Console.WriteLine($"[RpgService] 📥 Personaje importado: {player.Name} (Lv.{player.Level})");
                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RpgService] ❌ Error importando personaje: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Migra datos antiguos a nueva estructura
        /// CRÍTICO: NO borra progreso, solo agrega campos nuevos
        /// </summary>
        public void MigratePlayerData(RpgPlayer player)
        {
            var updated = false;
            
            // FASE 1: Sistema de mascotas
            if (player.PetInventory == null)
            {
                player.PetInventory = new List<RpgPet>();
                updated = true;
            }
            if (player.ActivePets == null)
            {
                player.ActivePets = new List<RpgPet>();
                updated = true;
            }
            
            // FASE 2: Hidden Classes
            if (player.UnlockedHiddenClasses == null)
            {
                player.UnlockedHiddenClasses = new List<string>();
                updated = true;
            }
            
            if (player.UnlockedPassives == null)
            {
                player.UnlockedPassives = new List<string>();
                updated = true;
            }
            
            // FASE 3: Skills System
            if (player.UnlockedSkills == null)
            {
                player.UnlockedSkills = new List<string>();
                updated = true;
            }
            
            if (player.SkillCooldowns == null)
            {
                player.SkillCooldowns = new Dictionary<string, int>();
                updated = true;
            }
            
            // FASE 4: Action Tracking
            if (player.ActionCounters == null)
            {
                player.ActionCounters = new Dictionary<string, int>();
                updated = true;
            }
            
            // CombatLog no existe, usar ActionCounters si es necesario
            
            // FASE 5: Minions & Zones
            if (player.ActiveMinions == null)
            {
                player.ActiveMinions = new List<Minion>();
                updated = true;
            }
            
            if (player.UnlockedZones == null)
            {
                player.UnlockedZones = new List<string> { "puerto_esperanza" };
                updated = true;
            }
            
            if (string.IsNullOrEmpty(player.CurrentZone))
            {
                player.CurrentZone = "puerto_esperanza";
                updated = true;
            }
            
            // Stats adicionales (si no existen)
            if (player.Mana == 0 && player.MaxMana == 0 && player.Class == CharacterClass.Mage)
            {
                player.MaxMana = 100;
                player.Mana = 100;
                updated = true;
            }
            
            if (player.Stamina == 0 && player.MaxStamina == 0)
            {
                player.MaxStamina = 100;
                player.Stamina = 100;
                updated = true;
            }
            
            // Equipamiento
            if (player.EquipmentInventory == null)
            {
                player.EquipmentInventory = new List<RpgEquipment>();
                updated = true;
            }
            
            if (updated)
            {
                Console.WriteLine($"[RpgService] 🔄 Personaje migrado: {player.Name} (añadidos campos nuevos)");
                SavePlayer(player);
            }
        }
        
        /// <summary>
        /// Valida que un JSON sea un personaje válido
        /// </summary>
        public bool ValidatePlayerJson(string json)
        {
            try
            {
                var player = JsonSerializer.Deserialize<RpgPlayer>(json);
                
                if (player == null)
                    return false;
                
                // Campos obligatorios
                if (string.IsNullOrEmpty(player.Name))
                    return false;
                
                if (player.Level < 1 || player.Level > 1000)
                    return false;
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        // Helper: Roll dice
        public static int RollDice(int sides = 20)
        {
            return _random.Next(1, sides + 1);
        }
    }
}
