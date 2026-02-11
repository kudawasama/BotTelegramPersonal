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
                    return players.FirstOrDefault(p => p.ChatId == chatId);
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
        
        public RpgPlayer CreateNewPlayer(long chatId, string name, CharacterClass characterClass)
        {
            var player = new RpgPlayer
            {
                ChatId = chatId,
                Name = name,
                Class = characterClass,
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
                CurrentLocation = "Taberna de Puerto Esperanza"
            };
            
            // Stats iniciales según clase (Tier 1)
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    player.Strength = 14;
                    player.Constitution = 12;
                    player.MaxHP = 120;
                    player.HP = 120;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Espada de Hierro",
                        Emoji = "🗡️",
                        Type = ItemType.Weapon,
                        AttackBonus = 5,
                        Value = 30,
                        Rarity = ItemRarity.Common
                    };
                    break;
                    
                case CharacterClass.Mage:
                    player.Intelligence = 14;
                    player.Wisdom = 12;
                    player.MaxEnergy = 70;
                    player.Energy = 70;
                    player.MaxMana = 100;
                    player.Mana = 100;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Bastón de Roble",
                        Emoji = "🪄",
                        Type = ItemType.Weapon,
                        AttackBonus = 3,
                        Value = 25,
                        Rarity = ItemRarity.Common
                    };
                    break;
                    
                case CharacterClass.Rogue:
                    player.Dexterity = 14;
                    player.Intelligence = 11;
                    player.MaxEnergy = 60;
                    player.Energy = 60;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Daga de Acero",
                        Emoji = "🔪",
                        Type = ItemType.Weapon,
                        AttackBonus = 4,
                        Value = 28,
                        Rarity = ItemRarity.Common
                    };
                    break;
                    
                case CharacterClass.Cleric:
                    player.Wisdom = 13;
                    player.Strength = 11;
                    player.Constitution = 11;
                    player.MaxHP = 110;
                    player.HP = 110;
                    player.MaxMana = 80;
                    player.Mana = 80;
                    player.Inventory.Add(new RpgItem
                    {
                        Name = "Poción de Vida",
                        Emoji = "🧪",
                        Type = ItemType.Consumable,
                        HPRestore = 30,
                        Value = 15,
                        Rarity = ItemRarity.Common
                    });
                    break;
                    
                // Tier 2 (Lv.10+)
                case CharacterClass.Paladin:
                    player.Strength = 15;
                    player.Charisma = 13;
                    player.Constitution = 13;
                    player.MaxHP = 130;
                    player.HP = 130;
                    player.MaxMana = 60;
                    player.Mana = 60;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Espada Sagrada",
                        Emoji = "⚔️",
                        Type = ItemType.Weapon,
                        AttackBonus = 6,
                        Value = 50,
                        Rarity = ItemRarity.Uncommon
                    };
                    break;
                    
                case CharacterClass.Ranger:
                    player.Dexterity = 15;
                    player.Wisdom = 12;
                    player.Strength = 11;
                    player.MaxEnergy = 70;
                    player.Energy = 70;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Arco Largo",
                        Emoji = "🏹",
                        Type = ItemType.Weapon,
                        AttackBonus = 5,
                        Value = 45,
                        Rarity = ItemRarity.Uncommon
                    };
                    break;
                    
                case CharacterClass.Warlock:
                    player.Charisma = 14;
                    player.Intelligence = 13;
                    player.MaxMana = 120;
                    player.Mana = 120;
                    player.MaxEnergy = 60;
                    player.Energy = 60;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Grimorio Oscuro",
                        Emoji = "📖",
                        Type = ItemType.Weapon,
                        AttackBonus = 4,
                        Value = 55,
                        Rarity = ItemRarity.Uncommon
                    };
                    break;
                    
                case CharacterClass.Monk:
                    player.Dexterity = 14;
                    player.Wisdom = 13;
                    player.Constitution = 12;
                    player.MaxHP = 115;
                    player.HP = 115;
                    player.MaxEnergy = 80;
                    player.Energy = 80;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Guantes de Combate",
                        Emoji = "🥋",
                        Type = ItemType.Weapon,
                        AttackBonus = 4,
                        Value = 40,
                        Rarity = ItemRarity.Uncommon
                    };
                    break;
                    
                // Tier 3 (Lv.20+)
                case CharacterClass.Berserker:
                    player.Strength = 16;
                    player.Constitution = 14;
                    player.Dexterity = 11;
                    player.MaxHP = 150;
                    player.HP = 150;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Hacha de Guerra",
                        Emoji = "🪓",
                        Type = ItemType.Weapon,
                        AttackBonus = 8,
                        Value = 80,
                        Rarity = ItemRarity.Rare
                    };
                    break;
                    
                case CharacterClass.Assassin:
                    player.Dexterity = 16;
                    player.Intelligence = 13;
                    player.Charisma = 11;
                    player.MaxEnergy = 80;
                    player.Energy = 80;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Dagas Gemelas",
                        Emoji = "⚔️",
                        Type = ItemType.Weapon,
                        AttackBonus = 7,
                        Value = 85,
                        Rarity = ItemRarity.Rare
                    };
                    break;
                    
                case CharacterClass.Sorcerer:
                    player.Charisma = 16;
                    player.Intelligence = 13;
                    player.MaxMana = 140;
                    player.Mana = 140;
                    player.MaxEnergy = 70;
                    player.Energy = 70;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Cetro Arcano",
                        Emoji = "🔮",
                        Type = ItemType.Weapon,
                        AttackBonus = 6,
                        Value = 90,
                        Rarity = ItemRarity.Rare
                    };
                    break;
                    
                case CharacterClass.Druid:
                    player.Wisdom = 15;
                    player.Constitution = 13;
                    player.Intelligence = 12;
                    player.MaxHP = 125;
                    player.HP = 125;
                    player.MaxMana = 100;
                    player.Mana = 100;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Bastón Natural",
                        Emoji = "🌿",
                        Type = ItemType.Weapon,
                        AttackBonus = 5,
                        Value = 75,
                        Rarity = ItemRarity.Rare
                    };
                    break;
                    
                // Tier 4 (Lv.30+)
                case CharacterClass.Necromancer:
                    player.Intelligence = 17;
                    player.Charisma = 13;
                    player.Wisdom = 12;
                    player.MaxMana = 160;
                    player.Mana = 160;
                    player.MaxEnergy = 75;
                    player.Energy = 75;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Báculo de Almas",
                        Emoji = "💀",
                        Type = ItemType.Weapon,
                        AttackBonus = 7,
                        Value = 120,
                        Rarity = ItemRarity.Epic
                    };
                    break;
                    
                case CharacterClass.Bard:
                    player.Charisma = 17;
                    player.Dexterity = 13;
                    player.Intelligence = 12;
                    player.MaxHP = 120;
                    player.HP = 120;
                    player.MaxMana = 120;
                    player.Mana = 120;
                    player.MaxEnergy = 90;
                    player.Energy = 90;
                    player.EquippedWeapon = new RpgItem
                    {
                        Name = "Laúd Encantado",
                        Emoji = "🎵",
                        Type = ItemType.Weapon,
                        AttackBonus = 5,
                        Value = 110,
                        Rarity = ItemRarity.Epic
                    };
                    break;
            }
            
            SavePlayer(player);
            Console.WriteLine($"[RpgService] ✨ Nuevo jugador creado: {name} ({characterClass})");
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
        // Helper: Roll dice
        public static int RollDice(int sides = 20)
        {
            return _random.Next(1, sides + 1);
        }
    }
}
