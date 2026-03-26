# 🎮 BotTelegram - Telegram RPG Bot

> Un bot de Telegram con un sistema de rol completo: combate táctico, mazmorras, mascotas, clases, gremios, PvP, facciones, NPCs, crafteo y misiones en un mundo abierto expansivo.

[![GitHub](https://img.shields.io/badge/GitHub-kudawasama%2FBotTelegramPersonal-blue?logo=github)](https://github.com/kudawasama/BotTelegramPersonal)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](#license)

---

## ✨ Características principales

### 🎮 Sistema RPG Completo

#### ⚔️ **Combate y Exploración**
- **Combate táctico por turnos** con acciones estratégicas
- **Mapas y zonas explorables** con regiones desbloqueables
- **Mazmorras multi-piso** con enemigos y bosses
- **Sistema de mascotas** - domesticar, liberar o vender
- **12 NPCs interactivos** con diálogos y quests

#### 🎯 **Progresión de Personaje**
- **10+ clases disponibles** (Guerreo, Mago, Arquero, etc.)
- **Atributos personalizables** (STR, INT, DEX, VIT)
- **Inventario y equipamiento** - armas, armaduras, consumibles
- **Habilidades y pasivas** desbloqueables por nivel
- **Crafteo de items** - 9 recetas Tier 1-3

#### 🏛️ **Sistema Social**
- **Gremios** - crear, unir, banco gremial, oficiales
- **Arena PvP** - combate ELO, ranking, apuestas
- **Facciones** - 10 facciones con 7 tiers de reputación
- **Misiones/Quests** - 8 tipos (Kill, Collect, Craft, Explore)

#### 🌍 **Mundo Abierto (Fase 12 - 100%)**
- **Acciones expandidas**: pesca, meditación, investigación, entrenamiento
- **Comercio NPC** - comprar/vender items
- **Entrenamiento de atributos** en tabernas
- **Aventuras riesgosas/sigilosas/sociales**
- **Zonas desbloqueables** por progreso

### 🌐 API REST
- **Health check** en `/health`
- **Dashboard web** (próximamente)
- **Integración fácil** con otras aplicaciones

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

### 🎮 Comandos Principales

| Comando | Descripción | Ejemplo |
|---------|------------|---------|
| `/start` | Iniciar el bot | `/start` |
| `/help` | Ver todos los comandos | `/help` |
| `/rpg` | Abrir menú RPG principal | `/rpg` |
| `/faq` | Ver manual/FAQ del bot | `/faq` |

### ⚔️ Combate y Exploración

| Comando | Descripción |
|---------|------------|
| `/mapa` | Ver mapa de regiones |
| `/mazmorra` | Explorar mazmorras |
| `/viajar` | Viajar entre zonas |
| `/entrenar` | Entrenar atributos |

### 🎯 Personaje

| Comando | Descripción |
|---------|------------|
| `/clases` | Ver clases disponibles |
| `/stats` | Ver estadísticas del personaje |
| `/inventario` | Ver items y equipamiento |
| `/misiones` | Ver quests activas |

### 🏛️ Social

| Comando | Descripción |
|---------|------------|
| `/gremio` | Gestionar gremio |
| `/arena` | Combatir en PvP |
| `/faccion` | Ver reputación de facciones |
| `/ranking` | Ver leaderboard global |

### 🛒 Economía

| Comando | Descripción |
|---------|------------|
| `/tienda` | Comprar/vender items |
| `/herreria` | Craftear equipamiento |
| `/mascotas` | Gestionar mascotas |

---

## 🌐 API REST

### Endpoints disponibles

```bash
# Health check del bot
GET http://localhost:5000/health

# (Próximamente) Dashboard del personaje
GET http://localhost:5000/api/character/{chatId}

# (Próximamente) Inventario
GET http://localhost:5000/api/inventory/{chatId}
```

### Ejemplos con cURL

```bash
# Verificar salud del bot
curl http://localhost:5000/health
```

---

## 📁 Estructura del proyecto

```
BotTelegram/
├── src/BotTelegram/
│   ├── Commands/              # Handlers de comandos
│   │   ├── StartCommand.cs
│   │   ├── HelpCommand.cs
│   │   ├── FaqCommand.cs
│   │   └── ChatCommand.cs
│   ├── Core/                  # Lógica central
│   │   ├── Bot.cs
│   │   ├── CommandRouter.cs
│   │   └── BotService.cs
│   ├── RPG/                   # Sistema RPG completo
│   │   ├── Commands/          # Comandos RPG
│   │   │   ├── RpgCommand.cs
│   │   │   ├── ClassesCommand.cs
│   │   │   ├── DungeonCommand.cs
│   │   │   ├── FactionCommand.cs
│   │   │   ├── GuildCommand.cs
│   │   │   ├── ShopCommand.cs
│   │   │   ├── CraftingCommand.cs
│   │   │   ├── QuestCommand.cs
│   │   │   └── PvpCommand.cs
│   │   ├── Models/            # Modelos de datos RPG
│   │   │   ├── RpgPlayer.cs
│   │   │   ├── RpgItem.cs
│   │   │   ├── RpgEnemy.cs
│   │   │   ├── ClassInfo.cs
│   │   │   ├── Faction.cs
│   │   │   ├── Guild.cs
│   │   │   └── GameState.cs
│   │   └── Services/          # Servicios RPG
│   │       ├── InventoryService.cs
│   │       ├── CombatService.cs
│   │       ├── QuestService.cs
│   │       ├── FactionService.cs
│   │       └── GuildService.cs
│   ├── Handlers/              # Event handlers
│   │   ├── MessageHandler.cs
│   │   └── CallbackQueryHandler.cs
│   ├── Services/              # Servicios generales
│   │   ├── BotService.cs
│   │   └── TelegramLogger.cs
│   ├── API/                   # Controladores REST
│   │   └── HealthController.cs
│   ├── Program.cs             # Entry point
│   ├── appsettings.json       # Configuración
│   └── BotTelegram.csproj     # Proyecto
├── Docs/                      # Documentación
│   ├── README.md
│   ├── INSTALLATION.md
│   ├── USAGE.md
│   ├── API.md
│   ├── ROADMAP.md
│   └── FEATURES_ROADMAP.md
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

Los datos del personaje se guardan en `data/`:

```json
{
  "playerId": "abc123",
  "ChatId": 1392641621,
  "Nombre": "Guerreo",
  "Clase": "Guerreo",
  "Nivel": 5,
  "Stats": { "STR": 10, "INT": 5, "DEX": 8, "VIT": 12 },
  "Inventario": [...],
  "Facciones": {...},
  "Misiones": [...],
  "Gremio": "...",
  "PvpRating": 1200
}
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
