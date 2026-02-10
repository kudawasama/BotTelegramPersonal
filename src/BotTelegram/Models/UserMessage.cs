namespace BotTelegram.Models
{
    // Representa un mensaje del usuario
    public class UserMessage
    {
        // Texto del mensaje
        public string Text { get; set; } = string.Empty;

        // Nombre del usuario
        public string? Username { get; set; }

        // ID del usuario (Telegram lo usa)
        public long UserId { get; set; }
    }
}
