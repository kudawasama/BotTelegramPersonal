namespace BotTelegram.Models
{
    public class Reminder
    {
        public long ChatId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset DueAt { get; set; }
        public bool Notified { get; set; } = false;
    }
}
