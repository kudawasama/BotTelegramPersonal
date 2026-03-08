# 🤖 BotTelegram - Telegram Reminder Bot

> Un bot de Telegram avanzado para gestionar recordatorios con inteligencia artificial, soporte para recurrencia y API REST integrada.

[![GitHub](https://img.shields.io/badge/GitHub-kudawasama%2FBotTelegramPersonal-blue?logo=github)](https://github.com/kudawasama/BotTelegramPersonal)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](#license)

---

## ✨ Características principales

### 📱 Bot de Telegram
- **Recordatorios inteligentes** con parsing de lenguaje natural
- **Comandos completos**: `/start`, `/help`, `/remember`, `/list`, `/delete`, `/edit`, `/recur`
- **Gestión avanzada**: Editar, eliminar, hacer recurrentes tus recordatorios
- **Recurrencia automática**: Diario, semanal, mensual, anual
- **Notificaciones push** automáticas a la hora exacta

### 🌐 API REST
- **Interfaz web** en puerto 5000
- **CRUD completo** de recordatorios
- **Integración fácil** con otras aplicaciones
- **Documentación automática** con Swagger (próximo)

### 🔒 Seguridad
- **Token protegido** en variables de entorno
- **Base de datos persistente** en JSON
- **Validación de entrada** en todos los comandos
- **Manejo de errores** robusto

### ☁️ Deploy
- **Listo para producción** en Azure App Service (Basic B1)
- **24/7 en línea** sin intervención manual
- **Escalable** y fácil de mantener

---

## 🚀 Quick Start

Antes de retomar trabajo técnico, revisa el checkpoint:

- `Docs/ESTADO_ACTUAL.md`

### Opción 1: Local

```bash
# Clonar repositorio
git clone https://github.com/kudawasama/BotTelegramPersonal.git
cd BotTelegram/src/BotTelegram

# Ejecutar
dotnet run
```

### Opción 2: Azure App Service (Plan Básico B1)

1. Crea una Web App Linux en un plan `B1`.
2. Configura `TELEGRAM_BOT_TOKEN` en variables de entorno.
3. Crea los secrets de GitHub: `AZURE_CREDENTIALS` y `AZURE_WEBAPP_NAME`.
4. Usa el workflow `.github/workflows/azure-deploy.yml`.
5. Revisa la guía completa en `deploy/AZURE_SETUP.md`.

Requiere:
- .NET 9.0 o superior
- Token de Telegram Bot (obtén uno en [@BotFather](https://t.me/botfather))

---

## 📚 Comandos disponibles

| Comando | Descripción | Ejemplo |
|---------|------------|---------|
| `/start` | Iniciar el bot | `/start` |
| `/help` | Ver todos los comandos | `/help` |
| `/remember` | Crear recordatorio | `/remember Tomar agua en 10 min` |
| `/list` | Listar todos los recordatorios | `/list` |
| `/delete` | Eliminar un recordatorio | `/delete abc123` |
| `/edit` | Modificar un recordatorio | `/edit abc123 Nuevo texto en 5 min` |
| `/recur` | Establecer recurrencia | `/recur abc123 daily` |

### Formatos soportados para `/remember`

```
en 10 segundos          → En 10 segundos
en 5 minutos            → En 5 minutos
en 2 horas              → En 2 horas
en 3 días               → En 3 días
hoy a las 18:00         → Hoy a las 6 PM
mañana a las 09:00      → Mañana a las 9 AM
```

### Tipos de recurrencia para `/recur`

```
/recur <id> daily       → Cada día
/recur <id> weekly      → Cada semana
/recur <id> monthly     → Cada mes
/recur <id> yearly      → Cada año
/recur <id> none        → Una sola vez
```

---

## 🌐 API REST

### Endpoints disponibles

```bash
# Listar todos los recordatorios
GET http://localhost:5000/api/reminders

# Obtener un recordatorio específico
GET http://localhost:5000/api/reminders/{id}

# Crear nuevo recordatorio
POST http://localhost:5000/api/reminders
Content-Type: application/json
{
  "chatId": 1234567890,
  "text": "Tomar agua",
  "dueAt": "2026-02-10T20:00:00-03:00",
  "recurrence": 0
}

# Actualizar un recordatorio
PUT http://localhost:5000/api/reminders/{id}

# Eliminar un recordatorio
DELETE http://localhost:5000/api/reminders/{id}
```

### Ejemplos con cURL

```bash
# Listar todos
curl http://localhost:5000/api/reminders

# Obtener uno
curl http://localhost:5000/api/reminders/abc123

# Crear
curl -X POST http://localhost:5000/api/reminders \
  -H "Content-Type: application/json" \
  -d '{"chatId":1392641621,"text":"Test","dueAt":"2026-02-10T20:00:00-03:00"}'

# Eliminar
curl -X DELETE http://localhost:5000/api/reminders/abc123
```

---

## 📁 Estructura del proyecto

```
BotTelegram/
├── src/BotTelegram/
│   ├── Commands/              # Comando handlers
│   │   ├── StartCommand.cs
│   │   ├── HelpCommand.cs
│   │   ├── RememberCommand.cs
│   │   ├── ListCommand.cs
│   │   ├── DeleteCommand.cs
│   │   ├── EditCommand.cs
│   │   ├── RecurCommand.cs
│   │   └── UnknownCommand.cs
│   ├── Core/                  # Lógica central
│   │   ├── Bot.cs
│   │   ├── CommandRouter.cs
│   │   └── BotService.cs
│   ├── Models/                # Modelos de datos
│   │   └── Reminder.cs
│   ├── Services/              # Servicios
│   │   ├── ReminderService.cs
│   │   ├── ReminderScheduler.cs
│   │   └── MessageHandler.cs
│   ├── API/                   # Controladores REST
│   │   └── RemindersController.cs
│   ├── Handlers/              # Event handlers
│   ├── Program.cs             # Entry point
│   ├── appsettings.json       # Configuración
│   └── BotTelegram.csproj    # Proyecto
├── Docs/                      # Documentación
│   ├── README.md
│   ├── INSTALLATION.md
│   ├── USAGE.md
│   ├── API.md
│   ├── ARCHITECTURE.md
│   └── ROADMAP.md
└── README.md                  # Este archivo
```

---

## 🔧 Configuración

### Variables de entorno (Local/Azure)

```
TELEGRAM_BOT_TOKEN=tu_token_aqui
```

### Archivo de configuración (Local)

```json
{
  "Telegram": {
    "Token": "tu_token_aqui"
  }
}
```

⚠️ **Nunca commitees el token a Git. Usa `.gitignore`**

---

## 📊 Datos persistidos

Los recordatorios se guardan en `bin/Debug/net9.0/data/reminders.json`:

```json
[
  {
    "Id": "abc123",
    "ChatId": 1392641621,
    "Text": "Tomar agua",
    "DueAt": "2026-02-10T20:00:00-03:00",
    "Notified": false,
    "Recurrence": "Daily",
    "CreatedAt": "2026-02-10T15:30:00-03:00"
  }
]
```

---

## 🛠️ Desarrolladores

### Stack tecnológico

- **Lenguaje**: C# (.NET 9.0)
- **Bot**: Telegram.Bot v22.9.0
- **Web**: ASP.NET Core
- **Persistencia**: JSON
- **Async**: Task-based async/await

### Estructura de código

- **Architecture**: Command Pattern + Middleware
- **Logging**: Console output con detalles por módulo
- **Error Handling**: Try-catch con mensajes específicos
- **Testing**: Pronto (en Roadmap)

---

## 🌍 Deployment

### Azure App Service (Básico B1) ☁️
```bash
# 1. Configurar secrets en GitHub: AZURE_CREDENTIALS y AZURE_WEBAPP_NAME
# 2. Configurar variable TELEGRAM_BOT_TOKEN en Azure Web App
# 3. Hacer push a master para disparar el workflow de deploy
```

### Docker 🐳
```bash
docker build -t bottelegram .
docker run -e TELEGRAM_BOT_TOKEN=tu_token bottelegram
```

---

## 📝 Licencia

MIT License - Siéntete libre de usar, modificar y distribuir.

---

## 🤝 Contribuir

¡Las contribuciones son bienvenidas! Por favor:

1. Fork el repositorio
2. Crea una rama (`git checkout -b feature/mejora`)
3. Commit tus cambios (`git commit -am 'Añade mejora'`)
4. Push a la rama (`git push origin feature/mejora`)
5. Abre un Pull Request

---

## 📞 Soporte

- **Issues**: [GitHub Issues](https://github.com/kudawasama/BotTelegramPersonal/issues)
- **Documentación**: Ver carpeta [Docs/](./Docs/)
- **Roadmap**: Ver [Docs/ROADMAP.md](./Docs/ROADMAP.md)

---

## 🎯 Próximas características

Ver [Roadmap completo](./Docs/ROADMAP.md) para planes futuros.

---

**Hecho con ❤️ por [@kudawasama](https://github.com/kudawasama)**
