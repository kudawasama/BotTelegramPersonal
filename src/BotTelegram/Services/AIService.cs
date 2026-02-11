using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BotTelegram.Services
{
    public class AIService
    {
        private static readonly HttpClient _client = new();
        private readonly string _apiKey;
        private static readonly Dictionary<long, List<ChatMessage>> _conversations = new();
        private static readonly HashSet<long> _chatMode = new();
        private static readonly object _chatModeLock = new();
        
        // Memory cleanup: tracking de última actividad por usuario
        private static readonly Dictionary<long, DateTime> _lastActivity = new();
        private static readonly Timer? _cleanupTimer;
        
        // Rate limiting: 10 requests por minuto por usuario
        private static readonly Dictionary<long, Queue<DateTime>> _userRequests = new();
        private const int MAX_REQUESTS_PER_MINUTE = 10;
        
        static AIService()
        {
            // Cleanup cada 5 minutos: elimina conversaciones inactivas >1h
            _cleanupTimer = new Timer(CleanupInactiveUsers, null, 
                TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public AIService()
        {
            _apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") 
                     ?? throw new Exception("❌ GROQ_API_KEY no encontrada en variables de entorno");
            
            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                // IMPORTANTE: usar API key real en header, sanitizar solo en logs
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
            
            Console.WriteLine($"[AIService] ✅ Servicio IA inicializado (key: {SanitizeApiKey(_apiKey)})");
        }
        
        // Sanitiza API keys para logs (muestra solo primeros/últimos 4 chars)
        private static string SanitizeApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey) || apiKey.Length <= 8)
                return "***";
            return $"{apiKey.Substring(0, 4)}...{apiKey.Substring(apiKey.Length - 4)}";
        }
        
        // Rate limiting: valida que usuario no exceda 10 req/min
        private static bool CheckRateLimit(long chatId)
        {
            var now = DateTime.UtcNow;
            
            if (!_userRequests.ContainsKey(chatId))
            {
                _userRequests[chatId] = new Queue<DateTime>();
            }
            
            var queue = _userRequests[chatId];
            
            // Eliminar requests más antiguos de 1 minuto
            while (queue.Count > 0 && (now - queue.Peek()).TotalMinutes > 1)
            {
                queue.Dequeue();
            }
            
            if (queue.Count >= MAX_REQUESTS_PER_MINUTE)
            {
                Console.WriteLine($"[AIService] ⚠️ Rate limit excedido para ChatId {chatId}");
                return false;
            }
            
            queue.Enqueue(now);
            return true;
        }
        
        // Cleanup: elimina conversaciones inactivas >1h
        private static void CleanupInactiveUsers(object? state)
        {
            var now = DateTime.UtcNow;
            var inactiveThreshold = TimeSpan.FromHours(1);
            var toRemove = new List<long>();
            
            lock (_chatModeLock)
            {
                foreach (var kvp in _lastActivity)
                {
                    if (now - kvp.Value > inactiveThreshold)
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
                
                foreach (var chatId in toRemove)
                {
                    _conversations.Remove(chatId);
                    _lastActivity.Remove(chatId);
                    _userRequests.Remove(chatId);
                    Console.WriteLine($"[AIService] 🧹 Cleanup: removido ChatId {chatId} (inactivo >1h)");
                }
                
                if (toRemove.Count > 0)
                {
                    Console.WriteLine($"[AIService] 🧹 Cleanup completado: {toRemove.Count} usuarios removidos");
                }
            }
        }

        public async Task<string> Chat(long chatId, string userMessage)
        {
            try
            {
                Console.WriteLine($"[AIService] 🤖 Iniciando chat para ChatId {chatId}");
                
                // Rate limiting: validar 10 req/min
                if (!CheckRateLimit(chatId))
                {
                    return "⚠️ *Demasiadas solicitudes*\n\nPor favor espera un momento antes de enviar otro mensaje. (Máximo 10 mensajes por minuto)";
                }
                
                // Actualizar última actividad
                lock (_chatModeLock)
                {
                    _lastActivity[chatId] = DateTime.UtcNow;
                }
                
                // Verificar si es comando de reinicio
                if (userMessage.ToLower().Contains("reiniciar") || 
                    userMessage.ToLower().Contains("limpiar") ||
                    userMessage.ToLower().Contains("borrar conversacion"))
                {
                    _conversations.Remove(chatId);
                    Console.WriteLine($"[AIService] 🔄 Conversación reiniciada para ChatId {chatId}");
                    return "🔄 *Conversación reiniciada*\n\n¡Perfecto! Hemos limpiado el historial. ¿En qué puedo ayudarte ahora?";
                }

                // Obtener o crear historial de conversación
                if (!_conversations.ContainsKey(chatId))
                {
                    _conversations[chatId] = new List<ChatMessage>();
                }

                var history = _conversations[chatId];
                Console.WriteLine($"[AIService] 📊 Memoria: {history.Count} mensajes previos");

                // Construir mensajes para la API
                var messages = new List<object>();

                // System prompt (personalidad del bot)
                messages.Add(new
                {
                    role = "system",
                    content = @"Eres un asistente personal amigable llamado 'Bot Recordatorios' integrado en Telegram.

Tu función principal es ayudar al usuario a:
- Gestionar y organizar sus recordatorios
- Responder preguntas sobre sus tareas pendientes
- Sugerir formas de ser más productivo
- Responder consultas generales de forma útil

Instrucciones de personalidad:
• Sé conciso: máximo 3-4 párrafos por respuesta
• Usa emojis apropiados para darle vida (pero sin exceso)
• Habla en español natural y amigable
• Si el usuario quiere crear un recordatorio, explícale cómo usar el comando /remember
• Sé proactivo sugiriendo recordatorios cuando sea relevante
• Si no sabes algo, admítelo honestamente

Formato de respuestas:
• Usa negritas con *texto* cuando sea apropiado
• Usa listas con • cuando enumeres cosas
• Sé directo y útil"
                });

                // Agregar historial (últimos 10 mensajes)
                foreach (var msg in history.TakeLast(10))
                {
                    messages.Add(new
                    {
                        role = msg.Role,
                        content = msg.Content
                    });
                }

                // Agregar mensaje actual del usuario
                messages.Add(new
                {
                    role = "user",
                    content = userMessage
                });

                // Preparar request
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant", // Modelo rápido y gratuito
                    messages = messages,
                    temperature = 0.7,
                    max_tokens = 600,
                    top_p = 0.95
                };

                Console.WriteLine("[AIService] 📤 Enviando request a Groq API...");
                
                var response = await _client.PostAsJsonAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AIService] ❌ Error API: {response.StatusCode} - {error}");
                    return "❌ Lo siento, hubo un error al procesar tu mensaje. Intenta de nuevo en un momento.";
                }

                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var aiResponse = result?.Choices?[0]?.Message?.Content?.Trim() 
                                ?? "Lo siento, no pude procesar tu mensaje.";

                Console.WriteLine($"[AIService] ✅ Respuesta recibida ({aiResponse.Length} chars)");

                // Guardar en historial (últimos 10 mensajes)
                history.Add(new ChatMessage { Role = "user", Content = userMessage });
                history.Add(new ChatMessage { Role = "assistant", Content = aiResponse });
                
                if (history.Count > 20) // 10 intercambios (10 user + 10 assistant)
                {
                    history.RemoveRange(0, history.Count - 20);
                }
                
                _conversations[chatId] = history;

                return aiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIService] ❌ Excepción: {ex.Message}");
                Console.WriteLine($"[AIService] Stack: {ex.StackTrace}");
                return "❌ Ocurrió un error inesperado. Por favor intenta de nuevo.";
            }
        }

        public static void SetChatMode(long chatId, bool enabled)
        {
            lock (_chatModeLock)
            {
                if (enabled)
                {
                    _chatMode.Add(chatId);
                }
                else
                {
                    _chatMode.Remove(chatId);
                }
            }
        }

        public static bool IsChatMode(long chatId)
        {
            lock (_chatModeLock)
            {
                return _chatMode.Contains(chatId);
            }
        }

        public static void ClearChatMode(long chatId)
        {
            lock (_chatModeLock)
            {
                _chatMode.Remove(chatId);
            }
        }

        public void ClearConversation(long chatId)
        {
            _conversations.Remove(chatId);
            Console.WriteLine($"[AIService] 🗑️ Conversación eliminada para ChatId {chatId}");
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = ""; // "user" o "assistant"
        public string Content { get; set; } = "";
    }

    public class GroqResponse
    {
        [JsonPropertyName("choices")]
        public List<GroqChoice>? Choices { get; set; }
    }

    public class GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqMessage? Message { get; set; }
    }

    public class GroqMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
