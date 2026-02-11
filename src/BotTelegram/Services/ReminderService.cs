using BotTelegram.Models;
using System.Text.Json;

namespace BotTelegram.Services
{
    public class ReminderService
    {
        private readonly string _filePath;

        public ReminderService()
        {
            // Buscar la raíz del proyecto (donde está BotTelegram.csproj)
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

            // Guardar en /data en la raíz del proyecto (no en bin/)
            var dataDir = Path.Combine(projectRoot, "data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
                Console.WriteLine($"[ReminderService] 📁 Creada carpeta: {dataDir}");
            }
            
            _filePath = Path.Combine(dataDir, "reminders.json");
            Console.WriteLine($"[ReminderService] 📁 Usando ruta: {_filePath}");
            
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
                Console.WriteLine($"[ReminderService] ✅ Creado archivo vacío");
            }
        }

        public List<Reminder> GetAll()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Reminder>>(json, options) ?? new List<Reminder>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReminderService] Error leyendo archivo: {ex.Message}");
                return new List<Reminder>();
            }
        }

        public void Save(Reminder reminder)
        {
            var reminders = GetAll();
            reminders.Add(reminder);
            Persist(reminders);
            Console.WriteLine($"[ReminderService] Guardado recordatorio para ChatId={reminder.ChatId} en {reminder.DueAt}");
        }

        public void UpdateAll(List<Reminder> reminders)
        {
            Persist(reminders);
            Console.WriteLine($"[ReminderService] Actualizados {reminders.Count} recordatorios");
        }

        private void Persist(List<Reminder> reminders)
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(reminders, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReminderService] Error guardando archivo: {ex.Message}");
            }
        }
    }
}

