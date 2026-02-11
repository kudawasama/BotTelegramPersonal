using System.Text.Json.Serialization;

namespace BotTelegram.Models
{
    public class Reminder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        
        [JsonPropertyName("chatId")]
        public long ChatId { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        
        [JsonPropertyName("dueAt")]
        public DateTimeOffset DueAt { get; set; }
        
        [JsonPropertyName("notified")]
        public bool Notified { get; set; } = false;
        
        [JsonPropertyName("recurrence")]
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        
        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }

    public enum RecurrenceType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }
}
