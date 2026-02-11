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
        
        // RPG Context: conversaciones separadas para modo RPG
        private static readonly Dictionary<long, List<ChatMessage>> _rpgConversations = new();
        private static readonly HashSet<long> _rpgChatMode = new();
        
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
                    _rpgConversations.Remove(chatId);
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
                    // Al activar chat normal, desactivar RPG mode
                    _rpgChatMode.Remove(chatId);
                    Console.WriteLine($"[AIService] 💬 Chat mode ACTIVADO para {chatId}");
                }
                else
                {
                    _chatMode.Remove(chatId);
                    Console.WriteLine($"[AIService] 💬 Chat mode DESACTIVADO para {chatId}");
                }
            }
        }
        
        public static void SetRpgChatMode(long chatId, bool enabled)
        {
            lock (_chatModeLock)
            {
                if (enabled)
                {
                    _rpgChatMode.Add(chatId);
                    // Al activar RPG mode, desactivar chat normal
                    _chatMode.Remove(chatId);
                    Console.WriteLine($"[AIService] 🎮 RPG Chat mode ACTIVADO para {chatId}");
                }
                else
                {
                    _rpgChatMode.Remove(chatId);
                    Console.WriteLine($"[AIService] 🎮 RPG Chat mode DESACTIVADO para {chatId}");
                }
            }
        }
        
        public static bool IsRpgChatMode(long chatId)
        {
            lock (_chatModeLock)
            {
                return _rpgChatMode.Contains(chatId);
            }
        }
        
        // Chat con contexto RPG
        public async Task<string> ChatWithRpgContext(long chatId, string userMessage, string playerName, string playerClass, int playerLevel, string location)
        {
            try
            {
                Console.WriteLine($"[AIService] 🎮 Chat RPG para {playerName} (ChatId {chatId})");
                
                // Rate limiting
                if (!CheckRateLimit(chatId))
                {
                    return "⚠️ *Demasiadas solicitudes*\n\nPor favor espera un momento. (Máximo 10 mensajes por minuto)";
                }
                
                // Actualizar última actividad
                lock (_chatModeLock)
                {
                    _lastActivity[chatId] = DateTime.UtcNow;
                }
                
                // Obtener o crear historial RPG separado
                if (!_rpgConversations.ContainsKey(chatId))
                {
                    _rpgConversations[chatId] = new List<ChatMessage>();
                }

                var history = _rpgConversations[chatId];
                Console.WriteLine($"[AIService] 🎮 Memoria RPG: {history.Count} mensajes previos");

                // Construir mensajes
                var messages = new List<object>();

                // System prompt CON CONTEXTO RPG
                messages.Add(new
                {
                    role = "system",
                    content = $@"Eres un Narrador Maestro (Dungeon Master) en un juego de rol de fantasía medieval llamado 'Aventuras de Valentia'.

CONTEXTO DEL PERSONAJE:
• Nombre: {playerName}
• Clase: {playerClass} (Nivel {playerLevel})
• Ubicación actual: {location}
• Mundo: Valentia, un reino amenazado por el Vacío

Tu función es:
✨ Narrar aventuras épicas y personalizadas basadas en el personaje del jugador
🎲 Describir escenarios, NPCs y situaciones de forma inmersiva
💡 Dar consejos estratégicos sobre combate y progresión
📖 Contar historias sobre el lore de Valentia y el Vacío
🗺️ Sugerir misiones y lugares para explorar

Estilo narrativo:
• Usa descripciones ricas y atmosféricas
• Adapta la dificultad narrativa al nivel del personaje
• Incorpora elementos de la clase del personaje en las historias
• Mantén un tono épico pero accesible
• Usa markdown para énfasis (*cursiva* y **negrita**)
• Máximo 4-5 párrafos por respuesta

IMPORTANTE: No inventes mecánicas de juego que no existen. El sistema real usa dados d20, combos, y efectos de estado. Si el jugador pregunta por mecánicas, describe las que realmente existen."
                });

                // Agregar historial (últimos 8 mensajes para dejar espacio al contexto)
                foreach (var msg in history.TakeLast(8))
                {
                    messages.Add(new
                    {
                        role = msg.Role,
                        content = msg.Content
                    });
                }

                // Agregar mensaje del usuario
                messages.Add(new
                {
                    role = "user",
                    content = userMessage
                });

                // Request
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = messages,
                    temperature = 0.85, // Más creativo para narrativa
                    max_tokens = 700, // Más tokens para narrativa rica
                    top_p = 0.95
                };

                Console.WriteLine("[AIService] 📤 Enviando request RPG a Groq API...");
                
                var response = await _client.PostAsJsonAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AIService] ❌ Error API: {response.StatusCode} - {error}");
                    return "❌ El narrador tuvo un problema. Intenta de nuevo.";
                }

                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var aiResponse = result?.Choices?[0]?.Message?.Content?.Trim() 
                                ?? "Lo siento, no pude generar una respuesta.";

                Console.WriteLine($"[AIService] ✅ Narrativa generada ({aiResponse.Length} chars)");

                // Guardar en historial RPG separado
                history.Add(new ChatMessage { Role = "user", Content = userMessage });
                history.Add(new ChatMessage { Role = "assistant", Content = aiResponse });
                
                if (history.Count > 16) // 8 intercambios
                {
                    history.RemoveRange(0, history.Count - 16);
                }
                
                _rpgConversations[chatId] = history;

                return aiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIService] ❌ Excepción RPG: {ex.Message}");
                return "❌ Error inesperado en la narrativa.";
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
            _rpgConversations.Remove(chatId);
            Console.WriteLine($"[AIService] 🗑️ Conversaciones eliminadas para ChatId {chatId} (normal + RPG)");
        }
        
        // ============================================
        // RPG NARRATIVE GENERATION
        // ============================================
        
        public async Task<string> GenerateRpgNarrative(
            string playerName,
            string playerClass,
            int playerLevel,
            string actionType,
            string location,
            string? enemyName = null,
            string? eventResult = null)
        {
            try
            {
                Console.WriteLine($"[AIService] 🎮 Generando narrativa RPG: {actionType}");
                
                var prompt = $@"Eres el narrador épico de 'Leyenda del Void', un RPG incremental oscuro y desafiante.

**Contexto del Jugador:**
- Nombre: {playerName}
- Clase: {playerClass}
- Nivel: {playerLevel}
- Ubicación actual: {location}

**Evento:** {actionType}
{(enemyName != null ? $"- Enemigo: {enemyName}" : "")}
{(eventResult != null ? $"- Resultado: {eventResult}" : "")}

**Tu tarea:**
Narra este evento en 2-3 párrafos cortos con estilo épico pero conciso.
- Usa lenguaje evocativo e inmersivo
- Sé dramático pero no te extiendas demasiado
- Incluye detalles sensoriales (sonidos, olores, sensaciones)
- Si hay combate, describe la acción vívidamente
- Termina con un gancho que impulse la siguiente acción

**Tono:** Oscuro, épico, inmersivo. Como un DM de D&D experimentado.

Responde SOLO con la narrativa, sin introducciones ni explicaciones:";
                
                var messages = new List<object>
                {
                    new { role = "system", content = "Eres un narrador maestro de RPGs, especializado en crear narrativas épicas y concisas." },
                    new { role = "user", content = prompt }
                };
                
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = messages,
                    temperature = 0.8, // Más creatividad para narrativa
                    max_tokens = 400
                };
                
                var response = await _client.PostAsJsonAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    requestBody);
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[AIService] ❌ Error API: {response.StatusCode}");
                    return GetFallbackNarrative(actionType, enemyName);
                }
                
                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var narrative = result?.Choices?[0]?.Message?.Content?.Trim() 
                               ?? GetFallbackNarrative(actionType, enemyName);
                
                Console.WriteLine($"[AIService] ✅ Narrativa generada ({narrative.Length} chars)");
                return narrative;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIService] ❌ Error generando narrativa: {ex.Message}");
                return GetFallbackNarrative(actionType, enemyName);
            }
        }
        
        private string GetFallbackNarrative(string actionType, string? enemyName)
        {
            return actionType.ToLower() switch
            {
                "explore" when enemyName != null => 
                    $"🌲 Avanzas cautelosamente por el sendero oscuro. De pronto, " +
                    $"¡{enemyName} emerge de las sombras con un rugido salvaje!\n\n" +
                    $"El aire se vuelve tenso. Tu mano se mueve hacia tu arma...",
                
                "victory" when enemyName != null => 
                    $"⚔️ Con un último golpe demoledor, derribas a {enemyName}. " +
                    $"La criatura cae al suelo con un gemido final. Victoria es tuya.\n\n" +
                    $"Recuperas el aliento, revisando tus heridas...",
                
                "train" => 
                    "🛡️ Pasas horas perfeccionando tus técnicas. El sudor corre por tu frente " +
                    "mientras repites los movimientos una y otra vez. Sientes que mejoras.",
                
                "rest" => 
                    "😴 Te recuestas junto al fuego de la taberna. El calor reconforta tus " +
                    "músculos cansados mientras recuperas fuerzas para la próxima aventura.",
                
                _ => 
                    "🎮 Continúas tu aventura en el reino de Valentia..."
            };
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
