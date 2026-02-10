using BotTelegram.Models;
using System.Text.Json;

namespace BotTelegram.Services
{
    public class ReminderService
    {
        private static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        private static readonly string FilePath = Path.Combine(DataDir, "reminders.json");

        public ReminderService()
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);
            if (!File.Exists(FilePath))
                File.WriteAllText(FilePath, "[]");
        }

        public List<Reminder> GetAll()
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<Reminder>>(json) ?? new List<Reminder>();
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
                var json = JsonSerializer.Serialize(reminders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReminderService] Error guardando archivo: {ex.Message}");
            }
        }
    }
}

