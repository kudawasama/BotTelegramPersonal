namespace BotTelegram.Models
{
    public class Reminder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        public long ChatId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset DueAt { get; set; }
        public bool Notified { get; set; } = false;
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
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
