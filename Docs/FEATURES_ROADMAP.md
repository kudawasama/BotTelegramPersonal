# 🗺️ Roadmap de Características - Bot de Recordatorios

Este documento detalla las características futuras planificadas para el bot de Telegram.

---

## 🎤 TRANSCRIPCIÓN DE AUDIO (Planificado)

### 📋 Descripción
Permitir a los usuarios crear recordatorios enviando notas de voz en lugar de escribir texto.

### 🎯 Flujo de Usuario
1. Usuario graba nota de voz: "Recordarme comprar leche en 2 horas"
2. Bot descarga el audio de Telegram
3. API de transcripción convierte voz → texto
4. Bot procesa el texto como `/remember Comprar leche en 2 horas`
5. Usuario recibe confirmación del recordatorio creado

### 🛠️ Implementación Técnica

#### **Opción Recomendada: OpenAI Whisper API**

**Pros:**
- ✅ Precisión excelente (95%+ en español)
- ✅ Soporte multi-idioma nativo
- ✅ API RESTful simple y documentada
- ✅ Reconoce lenguaje natural y contextos
- ✅ Muy rápido (~2-3 segundos por audio)

**Contras:**
- ❌ Servicio de pago: $0.006 USD por minuto de audio
- ❌ Requiere cuenta OpenAI con método de pago
- ❌ Dependencia de servicio externo (requiere internet)

**Costos Estimados:**
```
• 100 audios de 10 segundos = $0.10 USD
• 500 audios de 15 segundos = $0.75 USD
• 1000 recordatorios/mes ≈ $1-2 USD/mes
• Para uso personal: ~$1-5 USD/mes
```

#### **Alternativas:**

**1. AssemblyAI**
- ✅ Tier gratuito: 5 horas/mes
- ✅ Buena precisión (85-90%)
- ✅ Español soportado
- ❌ Límite mensual estricto
- ❌ Más lento que Whisper (5-8 segundos)
- **Costo adicional:** $0.00025 por segundo después del tier gratuito

**2. Vosk (Offline - Open Source)**
- ✅ Completamente GRATIS
- ✅ Sin límites de uso
- ✅ Privacidad total (procesa offline)
- ✅ No requiere API keys
- ❌ Precisión menor (~75-80% en español)
- ❌ Requiere servidor con más RAM (~2GB para modelo español)
- ❌ Modelos pesados (~500MB por idioma)
- ❌ Configuración más compleja

**3. Google Cloud Speech-to-Text**
- ✅ 60 minutos gratis/mes
- ✅ Alta precisión
- ❌ Configuración compleja con GCP
- **Costo adicional:** $0.006 por 15 segundos

### 📦 Dependencias Necesarias

```xml
<!-- Para OpenAI Whisper -->
<PackageReference Include="OpenAI" Version="1.11.0" />

<!-- O alternativa con HttpClient manual -->
<PackageReference Include="System.Net.Http.Json" Version="9.0.0" />
```

### 💻 Pseudocódigo de Implementación

```csharp
// 1. Detectar mensaje de voz en MessageHandler.cs
if (update.Message.Voice != null)
{
    await HandleVoiceMessage(bot, update.Message, ct);
}

// 2. Método de procesamiento de voz
private static async Task HandleVoiceMessage(
    ITelegramBotClient bot,
    Message message,
    CancellationToken ct)
{
    // Notificar al usuario que estamos procesando
    await bot.SendMessage(message.Chat.Id, 
        "🎤 Procesando tu audio...", ct);
    
    // Descargar audio de Telegram
    var file = await bot.GetFile(message.Voice.FileId, ct);
    var audioStream = new MemoryStream();
    await bot.DownloadFile(file.FilePath, audioStream, ct);
    
    // Transcribir con Whisper
    var transcription = await TranscribeWithWhisper(audioStream);
    
    // Procesar como comando /remember
    var reminderText = $"/remember {transcription}";
    var fakeMessage = new Message 
    { 
        Text = reminderText,
        Chat = message.Chat,
        From = message.From
    };
    
    await CommandRouter.Route(bot, fakeMessage, ct);
}

// 3. Llamada a OpenAI Whisper API
private static async Task<string> TranscribeWithWhisper(Stream audioStream)
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    
    var content = new MultipartFormDataContent();
    content.Add(new StreamContent(audioStream), "file", "audio.ogg");
    content.Add(new StringContent("whisper-1"), "model");
    content.Add(new StringContent("es"), "language");
    content.Add(new StringContent("transcribe"), "task");
    
    var response = await client.PostAsync(
        "https://api.openai.com/v1/audio/transcriptions", 
        content);
    
    var result = await response.Content.ReadFromJsonAsync<WhisperResponse>();
    return result.Text;
}

// 4. Modelo de respuesta
public class WhisperResponse
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
```

### 🔐 Configuración de API Key

**En local (appsettings.json):**
```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-..."
  }
}
```

**Variables de entorno (Azure/App Service o shell local):**
```bash
OPENAI_API_KEY=sk-proj-...
```

### ✅ Validaciones Necesarias
- Verificar que el audio no esté vacío
- Limitar duración máxima (ej: 1 minuto para evitar costos altos)
- Validar que la transcripción contiene información de tiempo
- Manejo de errores si la transcripción falla

### 📊 Métricas de Éxito
- Precisión de transcripción > 90%
- Tiempo de respuesta < 5 segundos
- Tasa de error < 5%

---

## 🌐 BÚSQUEDA WEB INTELIGENTE (Planificado)

### 📋 Descripción
Permitir al bot buscar información en internet y responder preguntas del usuario.

### 🎯 Flujo de Usuario

**Modo 1: Búsqueda Simple**
```
Usuario: /search ¿Cuál es la capital de Francia?
Bot: 🔍 Resultados para: ¿Cuál es la capital de Francia?

• París - Wikipedia
  París es la capital de Francia y su ciudad más poblada...
  
• Todo sobre París | Francia.fr
  Descubre la historia, cultura y atracciones de París...
  
📚 [Ver más resultados]
```

**Modo 2: Búsqueda con IA (Avanzado)**
```
Usuario: /ask ¿Cómo preparar café espresso?
Bot: ☕ Respuesta:

Para preparar un café espresso necesitas:
1. Moler granos frescos (18-20g)
2. Calentar la máquina a 90-95°C
3. Compactar el café uniformemente
4. Extraer por 25-30 segundos
5. Servir inmediatamente

📚 Fuentes:
• CoffeeGeek.com
• BaristaTutorials.com
```

### 🛠️ Implementación Técnica

#### **Opción 1: Bing Search API** ⭐ RECOMENDADA

**Pros:**
- ✅ **Tier gratuito: 1000 búsquedas/mes**
- ✅ Resultados de calidad (motor de Microsoft)
- ✅ API RESTful simple
- ✅ Respuesta rápida (~1 segundo)
- ✅ Sin configuración compleja

**Contras:**
- ❌ Requiere cuenta Azure (gratis)
- ❌ Solo búsquedas web básicas en tier gratuito
- ❌ Límite mensual estricto

**Costos:**
```
• Gratis: 1000 búsquedas/mes (S1 tier)
• Pago: $7 USD por 1000 búsquedas adicionales
• Para uso personal: GRATIS
```

**Registro:**
1. Crear cuenta en [Azure Portal](https://portal.azure.com)
2. Crear recurso "Bing Search v7"
3. Copiar API Key desde "Keys and Endpoint"

#### **Alternativas:**

**1. Google Custom Search API**
- ✅ Tier gratuito: 100 búsquedas/día (3000/mes)
- ✅ Resultados de Google (mejor calidad)
- ❌ Configuración compleja (requiere Custom Search Engine)
- ❌ Límite diario bajo
- **Costo adicional:** $5 por 1000 búsquedas

**2. SerpAPI**
- ✅ Scraping automático de Google
- ✅ Sin límites técnicos
- ❌ No tiene tier gratuito
- **Costo:** $50/mes por 5000 búsquedas

**3. DuckDuckGo Instant Answer API**
- ✅ Completamente GRATIS
- ✅ Sin API key necesaria
- ❌ Resultados limitados (solo "instant answers")
- ❌ No funciona bien con preguntas complejas

### 📦 Dependencias Necesarias

```xml
<!-- Para llamadas HTTP -->
<PackageReference Include="System.Net.Http.Json" Version="9.0.0" />

<!-- Para parseo JSON -->
<PackageReference Include="System.Text.Json" Version="9.0.0" />
```

### 💻 Pseudocódigo de Implementación

#### **Búsqueda Simple con Bing:**

```csharp
// 1. Nuevo comando SearchCommand.cs
public class SearchCommand
{
    public static async Task Execute(
        ITelegramBotClient bot,
        Message message,
        CancellationToken ct)
    {
        var query = message.Text.Replace("/search ", "").Trim();
        
        if (string.IsNullOrEmpty(query))
        {
            await bot.SendMessage(message.Chat.Id, 
                "❌ Uso: /search <tu pregunta>", ct);
            return;
        }
        
        // Notificar búsqueda en progreso
        await bot.SendMessage(message.Chat.Id, 
            "🔍 Buscando en la web...", ct);
        
        // Realizar búsqueda
        var results = await BingWebSearch(query);
        
        // Formatear respuesta
        var response = FormatSearchResults(query, results);
        
        await bot.SendMessage(message.Chat.Id, response, 
            parseMode: ParseMode.Markdown, ct);
    }
}

// 2. Método de búsqueda en Bing
private static async Task<List<SearchResult>> BingWebSearch(string query)
{
    var apiKey = Environment.GetEnvironmentVariable("BING_SEARCH_API_KEY");
    var client = new HttpClient();
    
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
    
    var url = $"https://api.bing.microsoft.com/v7.0/search?" +
              $"q={Uri.EscapeDataString(query)}&count=5&mkt=es-ES";
    
    var response = await client.GetAsync(url);
    var json = await response.Content.ReadAsStringAsync();
    
    // Parsear resultados
    var doc = JsonDocument.Parse(json);
    var webPages = doc.RootElement
        .GetProperty("webPages")
        .GetProperty("value");
    
    return webPages.EnumerateArray()
        .Select(item => new SearchResult
        {
            Name = item.GetProperty("name").GetString(),
            Url = item.GetProperty("url").GetString(),
            Snippet = item.GetProperty("snippet").GetString()
        })
        .Take(3)
        .ToList();
}

// 3. Formatear resultados
private static string FormatSearchResults(string query, List<SearchResult> results)
{
    var sb = new StringBuilder();
    sb.AppendLine($"🔍 *Resultados para:* {query}");
    sb.AppendLine();
    
    foreach (var result in results)
    {
        sb.AppendLine($"• [{result.Name}]({result.Url})");
        sb.AppendLine($"  _{result.Snippet}_");
        sb.AppendLine();
    }
    
    return sb.ToString();
}

// 4. Modelo de resultado
public class SearchResult
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Snippet { get; set; }
}
```

#### **Búsqueda Inteligente con IA (Bing + OpenAI):**

```csharp
// Comando avanzado: /ask
public class AskCommand
{
    public static async Task Execute(...)
    {
        var question = message.Text.Replace("/ask ", "").Trim();
        
        // 1. Buscar contexto en web
        var searchResults = await BingWebSearch(question);
        
        // 2. Construir prompt para GPT
        var context = string.Join("\n", 
            searchResults.Select(r => $"• {r.Snippet}"));
        
        var prompt = $@"Usuario pregunta: {question}

Contexto de búsqueda web:
{context}

Responde de forma concisa y útil en español, usando el contexto proporcionado.
Si no hay suficiente información, indícalo.";

        // 3. Consultar OpenAI
        var answer = await OpenAIChat(prompt);
        
        // 4. Formatear respuesta con fuentes
        var response = $"💡 *Respuesta:*\n\n{answer}\n\n" +
                      $"📚 *Fuentes consultadas:*\n";
        
        foreach (var result in searchResults)
        {
            response += $"• [{result.Name}]({result.Url})\n";
        }
        
        await bot.SendMessage(message.Chat.Id, response, 
            parseMode: ParseMode.Markdown, ct);
    }
}

// Método para OpenAI Chat
private static async Task<string> OpenAIChat(string prompt)
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    
    var requestBody = new
    {
        model = "gpt-4o-mini", // Más económico que GPT-4
        messages = new[]
        {
            new { role = "system", content = "Eres un asistente útil y conciso." },
            new { role = "user", content = prompt }
        },
        max_tokens = 300,
        temperature = 0.7
    };
    
    var response = await client.PostAsJsonAsync(
        "https://api.openai.com/v1/chat/completions", 
        requestBody);
    
    var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
    return result.Choices[0].Message.Content;
}
```

### 🔐 Configuración de API Keys

**appsettings.json (Local):**
```json
{
  "BingSearch": {
    "ApiKey": "tu_bing_api_key"
  },
  "OpenAI": {
    "ApiKey": "sk-proj-..." // Solo para modo IA
  }
}
```

**Variables de entorno (Azure/App Service o shell local):**
```bash
BING_SEARCH_API_KEY=tu_bing_api_key
OPENAI_API_KEY=sk-proj-...
```

### ✅ Validaciones Necesarias
- Verificar que la query no esté vacía
- Limitar longitud de query (max 200 caracteres)
- Manejo de errores si la API no responde
- Cache de resultados para evitar búsquedas duplicadas
- Rate limiting para no exceder tier gratuito

### 🎯 Comandos Propuestos

| Comando | Descripción | API Requerida |
|---------|-------------|---------------|
| `/search <pregunta>` | Búsqueda web simple con enlaces | Solo Bing |
| `/ask <pregunta>` | Respuesta inteligente con IA | Bing + OpenAI |
| `/wiki <término>` | Búsqueda específica Wikipedia | Bing o Wikipedia API |

### 📊 Costos de Implementación Completa

| Característica | API | Costo Mensual (uso moderado) |
|----------------|-----|-------------------------------|
| Búsqueda Simple | Bing Search | **GRATIS** (1000/mes) |
| Búsqueda con IA | Bing + GPT-4o-mini | $5-10 USD/mes |
| Transcripción Audio | OpenAI Whisper | $1-5 USD/mes |
| **TOTAL (todas las funciones)** | | **$6-15 USD/mes** |

### 🔮 Casos de Uso

**Búsqueda Simple:**
- "¿Cómo se dice hello en francés?"
- "Clima en Madrid hoy"
- "Receta de paella"

**Búsqueda Inteligente:**
- "Explícame qué es blockchain"
- "Cómo arreglar error 404 en web"
- "Diferencias entre React y Vue"

---

## 🚀 Orden de Implementación Recomendado

1. **✅ FAQ/Manual** - Completado
2. **🎤 Transcripción de Audio** - Prioridad ALTA
   - Impacto: Alto (mejora UX significativamente)
   - Complejidad: Media
   - Costo: Bajo ($1-2/mes)
   
3. **🌐 Búsqueda Web Simple** - Prioridad MEDIA
   - Impacto: Medio
   - Complejidad: Baja
   - Costo: GRATIS (tier Bing)
   
4. **🤖 Búsqueda con IA** - Prioridad BAJA
   - Impacto: Alto pero no esencial
   - Complejidad: Alta
   - Costo: Moderado ($5-10/mes)

---

## 📝 Notas de Desarrollo

### Para Transcripción:
- Agregar handler en `MessageHandler.cs` para `update.Message.Voice`
- Crear servicio `TranscriptionService.cs` reutilizable
- Validar formato de audio (Telegram usa OGG/OPUS)
- Implementar manejo de errores robusto
- Agregar logs de transcripción para debugging

### Para Búsqueda Web:
- Crear `SearchCommand.cs` y opcional `AskCommand.cs`
- Implementar servicio `WebSearchService.cs`
- Considerar cache de resultados (evitar búsquedas duplicadas)
- Implementar rate limiting para no exceder API limits
- Agregar comando `/searchhelp` con ejemplos

### Para Ambos:
- Actualizar `CommandRouter.cs` con nuevos comandos
- Actualizar `/help` con descripción de nuevas funciones
- Agregar sección en FAQ explicando cómo usar
- Proteger API keys en `.gitignore`
- Documentar en `README.md`

---

## 🔐 Seguridad y Privacidad

### Transcripción:
- ⚠️ Los audios se envían a servidores de OpenAI
- Política de privacidad: OpenAI no entrena con datos de API
- Datos del usuario: Solo metadata (duración, idioma)
- Retención: OpenAI no guarda audios después de transcribir

### Búsqueda Web:
- ⚠️ Las queries se envían a Microsoft Bing
- No se comparte información personal del usuario
- Solo se envía el texto de la búsqueda
- No se guarda historial de búsquedas

**Recomendación:** Agregar disclaimer en `/start` informando sobre uso de APIs externas.

---

*Documento actualizado: 11 de febrero de 2026*
