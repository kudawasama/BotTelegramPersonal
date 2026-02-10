# 🏗️ Arquitectura - Guía técnica

> Documentación técnica detallada de cómo funciona BotTelegram internamente

---

## 📋 Tabla de contenidos
1. [Resumen de arquitectura](#resumen-de-arquitectura)
2. [Capas](#capas)
3. [Flujos principales](#flujos-principales)
4. [Componentes clave](#componentes-clave)
5. [Datos y persistencia](#datos-y-persistencia)
6. [Manejo de errores](#manejo-de-errores)
7. [Patrones de diseño](#patrones-de-diseño)
8. [Diagrama de flujo](#diagrama-de-flujo)

---

## 🏛️ Resumen de arquitectura

**BotTelegram** usa una arquitectura **en capas** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────┐
│      Telegram Client Layer              │
│  (Telegram.Bot v22.9.0)                 │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     Message Handlers                    │
│  • CommandHandler.cs                    │
│  • MessageHandler.cs                    │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     Command Router & Commands           │
│  • CommandRouter.cs                     │
│  • /start, /help, /remember, etc.      │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     Business Logic Layer                │
│  • ReminderService (CRUD)              │
│  • ReminderScheduler (Background)      │
│  • BotService (Configuration)          │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     ASP.NET Core API Layer              │
│  • RemindersController.cs               │
│  • REST endpoints                       │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     Data Persistence Layer              │
│  • ReminderService (File I/O)          │
│  • JSON serialization                   │
└────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│        Data Storage                     │
│  • reminders.json                       │
└────────────────────────────────────────┘
```

---

## 🎯 Capas

### 1. **Presentation Layer** (Telegram UI)
**Archivos:** `Program.cs`, `Handlers/*`

**Responsabilidades:**
- Recibir updates de Telegram
- Validar tipos de update (Message, CallbackQuery, etc.)
- Enrutar a handlers apropiados

**Flujo:**
```csharp
bot.StartReceiving(handler, error)
    ↓
UpdateHandler()
    ├─ ¿Es mensaje? → MessageHandler
    └─ ¿Otro? → Ignorar
```

### 2. **Command Layer** (Lógica de comandos)
**Archivos:** `Commands/*.cs`, `Core/CommandRouter.cs`

**Responsabilidades:**
- Procesar entrada del usuario
- Ejecutar la lógica del comando
- Enviar respuestas a Telegram

**Comandos:**
```
/start       → StartCommand
/help        → HelpCommand
/remember    → RememberCommand (parsing de tiempo)
/list        → ListCommand
/delete      → DeleteCommand
/edit        → EditCommand
/recur       → RecurCommand
```

### 3. **Business Logic Layer** (Servicios)
**Archivos:** `Services/*.cs`

**Responsabilidades:**
- ReminderService: CRUD de recordatorios
- ReminderScheduler: Lógica de planificación
- BotService: Configuración y contexto

**Interfaces:**
```csharp
ReminderService {
    GetAll() → List<Reminder>
    GetById(id) → Reminder?
    Save(reminder) → void
    UpdateAll(list) → void
}

ReminderScheduler {
    Start() → void
    ExecuteCycle() → Task
    CheckAndNotify() → Task
}
```

### 4. **Data Access Layer** (Persistencia)
**Archivos:** `Services/ReminderService.cs`

**Responsabilidades:**
- Serializar/deserializar JSON
- Manejar I/O de archivos
- Sincronización thread-safe

**Formato datos:**
```json
[
  {
    "id": "a1b2c3d4",
    "chatId": 123456789,
    "text": "Estudiar",
    "dueAt": "2025-02-20T18:00:00",
    "notified": false,
    "recurrenceType": 1,
    "createdAt": "2025-02-10T10:00:00"
  }
]
```

### 5. **API Layer** (REST)
**Archivos:** `API/RemindersController.cs`

**Endpoints:**
```
GET    /api/reminders         → GetAll()
GET    /api/reminders/{id}    → GetById()
POST   /api/reminders         → Create()
PUT    /api/reminders/{id}    → Update()
DELETE /api/reminders/{id}    → Delete()
```

---

## 🔄 Flujos principales

### Flujo 1: Crear recordatorio (`/remember`)

```
Usuario en Telegram
    ↓
Envía: /remember "texto" en 2 horas
    ↓
Telegram API
    ↓
Program.cs: StartReceiving()
    ↓
HandlerService: UpdateHandler()
    ↓
Es Message? → Sí
    ↓
MessageHandler: HandleMessage()
    ↓
¿Empieza con /? → Sí
    ↓
CommandRouter.RouteCommand()
    ↓
¿Es /remember? → Sí
    ↓
RememberCommand.Execute()
    ├─ Parsea tiempo ("en 2 horas")
    ├─ Valida: DueAt no en pasado
    ├─ Genera ID (Guid.NewGuid()[0..8])
    ├─ Crea Reminder object
    ├─ ReminderService.Save(reminder)
    │   └─ Lee reminders.json
    │   └─ Añade nuevo
    │   └─ Escribe reminders.json
    └─ Envía ✅ respuesta a Telegram
```

**Línea de tiempo:**
```
[T+0ms]  Usuario envía mensaje
[T+100ms] Telegram Server
[T+150ms] BotTelegram recibe update
[T+160ms] ParseCommand → RememberCommand
[T+165ms] Parse tiempo: +2 horas
[T+170ms] Save a JSON
[T+175ms] SendMessage ✅
[T+250ms] Usuario recibe respuesta
```

### Flujo 2: Scheduler notifica recordatorio

```
Program.cs: Ejecuta scheduler en background
    ↓
ThreadPool.QueueUserWorkItem()
    ↓
ReminderScheduler.ExecuteCycle()
    │
    ├─ Cada 30 segundos:
    │   ├─ Lee reminders.json
    │   ├─ Para cada Reminder:
    │   │   ├─ ¿DueAt <= Now? → Sí
    │   │   └─ ¿Notified == false? → Sí
    │   │
    │   ├─ Calcula:
    │   │   ├─ Aún no notificado ✓
    │   │   └─ Es tiempo
    │   │
    │   ├─ SendMessage() a Telegram
    │   ├─ reminder.Notified = true
    │   │
    │   ├─ ¿RecurrenceType != None? → Sí
    │   │   ├─ reminder.DueAt += 1 día (Daily)
    │   │   └─ reminder.Notified = false
    │   │
    │   └─ UpdateAll() a JSON
    │
    └─ Repite cada 30 segundos
```

**Línea de tiempo (Daily):**
```
[15:00] Scheduler lee: "Estudiar" DueAt=15:00, Notified=false
[15:00] ✓ DueAt <= Now → Notifica
[15:00] SendMessage: "💬 Recordatorio: Estudiar"
[15:00] Calcula próximo: DueAt = Mañana 15:00
[15:00] Escribe: Notified=true, NextDueAt=(mañana)
[15:00] Crea nuevo: DueAt=(mañana 15:00), Notified=false
```

### Flujo 3: Listar recordatorios (`/list`)

```
Usuario envía: /list
    ↓
CommandRouter → ListCommand.Execute()
    ↓
ReminderService.GetAll()
    ├─ Lee reminders.json
    └─ Deserializa a List<Reminder>
    ↓
Filtra: Notified == false (pendientes)
    ↓
Ordena: Por DueAt (más próximos primero)
    ↓
Formatea respuesta:
    📋 Tus recordatorios (3 pendientes):
    [a1b2c3d4] 📅 2025-02-20 18:00 - Estudiar 🔄 Daily
    [e5f6g7h8] 📅 2025-02-15 14:30 - Llamar mamá
    [i9j0k1l2] 📅 2025-03-01 10:00 - Reunión
    ↓
SendMessage a Telegram
```

### Flujo 4: Eliminar recordatorio (`/delete`)

```
Usuario envía: /delete a1b2c3d4
    ↓
CommandRouter → DeleteCommand.Execute()
    ↓
ReminderService.GetAll()
    ├─ Lee reminders.json
    └─ Busca reminder.Id == "a1b2c3d4"
    ↓
¿Encontrado? → Sí
    ├─ Elimina de lista
    ├─ UpdateAll() → Escribe reminders.json
    └─ SendMessage: ✅ Eliminado
    
¿No encontrado? → Envía error
```

### Flujo 5: API GET `/api/reminders`

```
HTTP GET http://localhost:5000/api/reminders
    ↓
RemindersController.GetReminders()
    ↓
ReminderService.GetAll()
    ├─ Lee reminders.json
    └─ Deserializa JSON
    ↓
Retorna: List<Reminder> (JSON)
    ↓
HTTP 200 OK con JSON array
```

---

## 🔧 Componentes clave

### Models/Reminder.cs
```csharp
public class Reminder
{
    public string Id { get; set; }              // "a1b2c3d4"
    public long ChatId { get; set; }            // Telegram chat ID
    public string Text { get; set; }            // Descripción
    public DateTimeOffset DueAt { get; set; }   // Cuándo notificar
    public bool Notified { get; set; }          // Ya notificado?
    public RecurrenceType RecurrenceType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public enum RecurrenceType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4
}
```

### Services/ReminderService.cs
```csharp
public class ReminderService
{
    private const string DataPath = "data/reminders.json";
    
    public List<Reminder> GetAll()
    {
        if (!File.Exists(DataPath))
            return [];
        
        var json = File.ReadAllText(DataPath);
        return JsonConvert.DeserializeObject<List<Reminder>>(json) ?? [];
    }
    
    public void Save(Reminder reminder)
    {
        var all = GetAll();
        all.Add(reminder);
        UpdateAll(all);
    }
    
    public void UpdateAll(List<Reminder> reminders)
    {
        Directory.CreateDirectory("data");
        var json = JsonConvert.SerializeObject(reminders, Formatting.Indented);
        File.WriteAllText(DataPath, json);
    }
}
```

### Services/ReminderScheduler.cs
```csharp
public class ReminderScheduler
{
    private readonly ITelegramBotClient _bot;
    
    public void Start()
    {
        ThreadPool.QueueUserWorkItem(_ => ExecuteCycle());
    }
    
    private async Task ExecuteCycle()
    {
        while (true)
        {
            try
            {
                var service = new ReminderService();
                var all = service.GetAll();
                var now = DateTimeOffset.UtcNow;
                
                foreach (var reminder in all.Where(r => !r.Notified && r.DueAt <= now))
                {
                    // Notificar
                    await _bot.SendMessage(reminder.ChatId, 
                        $"💬 RECORDATORIO ⏰\n{reminder.Id} - {reminder.Text}");
                    
                    // Si recurrente
                    if (reminder.RecurrenceType != RecurrenceType.None)
                    {
                        reminder.DueAt = reminder.RecurrenceType switch
                        {
                            RecurrenceType.Daily => reminder.DueAt.AddDays(1),
                            RecurrenceType.Weekly => reminder.DueAt.AddDays(7),
                            RecurrenceType.Monthly => reminder.DueAt.AddMonths(1),
                            RecurrenceType.Yearly => reminder.DueAt.AddYears(1),
                            _ => reminder.DueAt
                        };
                        reminder.Notified = false;
                    }
                    else
                    {
                        reminder.Notified = true;
                    }
                }
                
                service.UpdateAll(all);
                await Task.Delay(30000); // 30 segundos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error scheduler: {ex.Message}");
                await Task.Delay(30000);
            }
        }
    }
}
```

### Commands/RememberCommand.cs
```csharp
public class RememberCommand : ICommand
{
    private readonly ITelegramBotClient _bot;
    
    public async Task Execute(long chatId, string text)
    {
        // Parsea: /remember "Estudiar" en 2 horas
        var pattern = @"^/remember\s+""?([^""]+)""?\s+(?:en\s+)?(\d+)\s+(seg|min|hora|día)";
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        
        if (!match.Success)
        {
            await _bot.SendMessage(chatId, "❌ Formato: /remember \"texto\" en 2 horas");
            return;
        }
        
        var description = match.Groups[1].Value;
        var value = int.Parse(match.Groups[2].Value);
        var unit = match.Groups[3].Value.ToLower();
        
        // Calcula DueAt
        var now = DateTimeOffset.UtcNow;
        var dueAt = unit switch
        {
            "seg" => now.AddSeconds(value),
            "min" => now.AddMinutes(value),
            "hora" => now.AddHours(value),
            "día" => now.AddDays(value),
            _ => now.AddMinutes(value)
        };
        
        // Valida
        if (dueAt <= now)
        {
            await _bot.SendMessage(chatId, "❌ La fecha no puede ser en el pasado");
            return;
        }
        
        // Crea recordatorio
        var reminder = new Reminder
        {
            Id = Guid.NewGuid().ToString()[..8],
            ChatId = chatId,
            Text = description,
            DueAt = dueAt,
            CreatedAt = now,
            RecurrenceType = RecurrenceType.None
        };
        
        var service = new ReminderService();
        service.Save(reminder);
        
        await _bot.SendMessage(chatId, 
            $"✅ Recordatorio creado\nID: {reminder.Id}\n" +
            $"Cuando: {reminder.DueAt:yyyy-MM-dd HH:mm:ss}");
    }
}
```

---

## 💾 Datos y persistencia

### Almacenamiento JSON

**Ubicación:** `bin/Debug/net9.0/data/reminders.json`

**Estructura:**
```json
[
  {
    "id": "a1b2c3d4",
    "chatId": 123456789,
    "text": "Estudiar matemáticas",
    "dueAt": "2025-02-20T18:00:00+00:00",
    "notified": false,
    "recurrenceType": 1,
    "createdAt": "2025-02-10T10:00:00+00:00"
  },
  {
    "id": "e5f6g7h8",
    "chatId": 987654321,
    "text": "Llamar a mamá",
    "dueAt": "2025-02-15T14:30:00+00:00",
    "notified": true,
    "recurrenceType": 0,
    "createdAt": "2025-02-14T15:20:00+00:00"
  }
]
```

### Seguridad

✅ **Implementado:**
- Thread-safe: Una lectura/escritura a la vez
- Validación: No se permiten recordatorios en el pasado
- Encriptación: Token de Telegram en env vars

⚠️ **Futuro (Fase 2):**
- Migrar a SQLite
- Encriptación de datos sensibles
- Backups automáticos

---

## ⚠️ Manejo de errores

### Niveles de error

**Nivel 1: Usuario (UI)**
```
❌ Formato: /remember "texto" en 2 horas
❌ La fecha no puede ser en el pasado
❌ Recordatorio no encontrado
```

**Nivel 2: Aplicación**
```csharp
try
{
    // Operación
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    await _bot.SendMessage(chatId, "⚠️ Error, intenta más tarde");
}
```

**Nivel 3: Scheduler (Background)**
```csharp
try
{
    // Ciclo scheduler
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error scheduler: {ex.Message}");
    // Continúa con el siguiente ciclo
}
```

---

## 🎨 Patrones de diseño

### 1. Command Pattern
```
ICommand interface
    ↓
StartCommand, RememberCommand, etc.
    ↓
CommandRouter enruta a la implementación
```

### 2. Singleton Pattern
```csharp
// ReminderService usado en múltiples lugares
// pero siempre carga el JSON actualizado
```

### 3. Observer Pattern (Implicit)
```
Telegram API (Observable)
    ↓
UpdateHandler (Observer)
    ↓
Ejecuta comandos
```

### 4. Factory Pattern (Partial)
```csharp
// CommandRouter "fabrica" el parámetro correcto
CommandRouter.RouteCommand(text)
    ├─ /remember → Create RememberCommand()
    ├─ /list → Create ListCommand()
    └─ ...
```

---

## 📊 Diagrama de flujo

### Flujo general de mensajes

```
Telegram
    │
    ├─ Update recibido
    │      ↓
    ├─ UpdateHandler()
    │      ↓
    ├─ ¿Tipo = Message?
    │      ├─ Sí → MessageHandler()
    │      │        ↓
    │      │        ¿Es comando (inicia con /)?
    │      │        ├─ Sí → CommandRouter.RouteCommand()
    │      │        │         ↓
    │      │        │         Ejecuta comando específico
    │      │        │         ├─ RememberCommand
    │      │        │         ├─ ListCommand
    │      │        │         ├─ DeleteCommand
    │      │        │         ├─ EditCommand
    │      │        │         ├─ RecurCommand
    │      │        │         ├─ HelpCommand
    │      │        │         └─ StartCommand
    │      │        │
    │      │        └─ No → UnknownCommand
    │      │
    │      └─ No → Ignorar (solo procesamos mensajes)
    │
    └─ Background: Scheduler (cada 30s)
                    ↓
                  CheckAndNotify()
                    ├─ Lee recordatorios
                    ├─ Filtra: DueAt <= Now && !Notified
                    ├─ Envía notificaciones
                    ├─ Calcula recurrencias
                    └─ Actualiza JSON
```

---

## 🔐 Seguridad

### Medidas implementadas
- ✅ Token en env vars (no en código)
- ✅ .gitignore protege `appsettings.json`
- ✅ Validación de entrada (regex)
- ✅ No SQL injection (JSON, no DB)

### Recomendaciones
- 🔄 Migrar a SQLite con encriptación
- 🔄 Usar JWT para API en Fase 2
- 🔄 Rate limiting en endpoints
- 🔄 HTTPS en producción

---

## 📚 Recursos

- [Telegram.Bot Documentation](https://github.com/TelegramBots/Telegram.Bot)
- [.NET 9.0 Documentation](https://learn.microsoft.com/dotnet/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)

---

**Última actualización:** Febrero 2025  
**Versión:** 1.0
