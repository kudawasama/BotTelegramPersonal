using BotTelegram.Models;
using System.Text.Json;

namespace BotTelegram.Services
{
    public class RpgService
    {
        private readonly string _filePath;
        private static readonly object _fileLock = new();
        private static readonly Random _random = new();
        
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
                Strength = 10,
                Intelligence = 10,
                Dexterity = 10,
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
            
            // Stats iniciales según clase
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    player.Strength = 14;
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
                    player.MaxEnergy = 70;
                    player.Energy = 70;
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
                    player.Intelligence = 12;
                    player.Strength = 12;
                    player.MaxHP = 110;
                    player.HP = 110;
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
            player.Level++;
            player.XP -= player.XPNeeded;
            
            // Stat increases
            player.MaxHP += 10;
            player.HP = player.MaxHP; // Full heal on level up
            player.MaxEnergy += 5;
            player.Energy = player.MaxEnergy;
            
            player.Strength += 2;
            player.Intelligence += 2;
            player.Dexterity += 2;
            
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
            var enemies = GetEnemyPool(difficulty);
            var template = enemies[_random.Next(enemies.Count)];
            
            // Scale to player level
            var levelDiff = difficulty switch
            {
                EnemyDifficulty.Easy => -1,
                EnemyDifficulty.Medium => 0,
                EnemyDifficulty.Hard => 1,
                EnemyDifficulty.Boss => 2,
                _ => 0
            };
            
            var enemyLevel = Math.Max(1, playerLevel + levelDiff);
            
            return new RpgEnemy
            {
                Name = template.Name,
                Emoji = template.Emoji,
                Level = enemyLevel,
                HP = template.HP + (enemyLevel - 1) * 10,
                MaxHP = template.MaxHP + (enemyLevel - 1) * 10,
                Attack = template.Attack + (enemyLevel - 1) * 2,
                Defense = template.Defense + (enemyLevel - 1) * 1,
                XPReward = template.XPReward + (enemyLevel - 1) * 10,
                GoldReward = template.GoldReward + (enemyLevel - 1) * 5,
                Difficulty = difficulty
            };
        }
        
        private List<RpgEnemy> GetEnemyPool(EnemyDifficulty difficulty)
        {
            return difficulty switch
            {
                EnemyDifficulty.Easy => new List<RpgEnemy>
                {
                    new() { Name = "Lobo Salvaje", Emoji = "🐺", HP = 30, MaxHP = 30, Attack = 8, Defense = 2, XPReward = 20, GoldReward = 15 },
                    new() { Name = "Goblin", Emoji = "👺", HP = 25, MaxHP = 25, Attack = 6, Defense = 3, XPReward = 15, GoldReward = 20 },
                    new() { Name = "Esqueleto", Emoji = "💀", HP = 20, MaxHP = 20, Attack = 10, Defense = 1, XPReward = 18, GoldReward = 12 }
                },
                EnemyDifficulty.Medium => new List<RpgEnemy>
                {
                    new() { Name = "Orco Guerrero", Emoji = "👹", HP = 50, MaxHP = 50, Attack = 12, Defense = 4, XPReward = 40, GoldReward = 30 },
                    new() { Name = "Araña Gigante", Emoji = "🕷️", HP = 40, MaxHP = 40, Attack = 10, Defense = 3, XPReward = 35, GoldReward = 25 },
                    new() { Name = "Bandido", Emoji = "🏴‍☠️", HP = 45, MaxHP = 45, Attack = 14, Defense = 2, XPReward = 38, GoldReward = 40 }
                },
                EnemyDifficulty.Hard => new List<RpgEnemy>
                {
                    new() { Name = "Troll de Hielo", Emoji = "🧊", HP = 80, MaxHP = 80, Attack = 18, Defense = 6, XPReward = 70, GoldReward = 60 },
                    new() { Name = "Demonio Menor", Emoji = "😈", HP = 70, MaxHP = 70, Attack = 20, Defense = 4, XPReward = 75, GoldReward = 55 },
                    new() { Name = "Caballero Oscuro", Emoji = "⚔️", HP = 75, MaxHP = 75, Attack = 16, Defense = 8, XPReward = 65, GoldReward = 70 }
                },
                _ => new List<RpgEnemy>()
            };
        }
        
        // Helper: Roll dice
        public static int RollDice(int sides = 20)
        {
            return _random.Next(1, sides + 1);
        }
    }
}
