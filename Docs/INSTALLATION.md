# Instalacion de BotTelegram

Guia practica para ejecutar el bot en local, Docker o Azure App Service (Plan Basico B1).

---

## Requisitos previos

### Opcion 1: Local (Windows/macOS/Linux)
- .NET 9.0 SDK o superior
- Git
- Token de Telegram Bot

### Opcion 2: Docker
- Docker instalado
- Token de Telegram Bot

### Opcion 3: Azure App Service (B1)
- Suscripcion de Azure
- Resource Group
- App Service Plan Linux SKU B1
- Web App Linux
- Token de Telegram Bot

---

## Instalacion local - Windows

1. Instala .NET 9 SDK desde https://dotnet.microsoft.com/download.
2. Clona el repositorio:

```powershell
git clone https://github.com/kudawasama/BotTelegramPersonal.git
cd BotTelegramPersonal/src/BotTelegram
```

3. Configura token por variable de entorno o `appsettings.json`.
4. Ejecuta:

```powershell
dotnet run
```

---

## Instalacion local - macOS/Linux

1. Instala .NET 9 SDK.
2. Clona el repositorio y entra al proyecto:

```bash
git clone https://github.com/kudawasama/BotTelegramPersonal.git
cd BotTelegramPersonal/src/BotTelegram
```

3. Exporta token:

```bash
export TELEGRAM_BOT_TOKEN="tu_token"
```

4. Ejecuta:

```bash
dotnet run
```

---

## Instalacion con Docker

1. Desde la raiz del repo, construye imagen:

```bash
docker build -t bottelegram:latest .
```

2. Ejecuta contenedor:

```bash
docker run -e TELEGRAM_BOT_TOKEN=tu_token_aqui bottelegram:latest
```

---

## Deploy en Azure App Service (Plan Basico B1)

### Paso 1: Crear infraestructura en Azure

1. Crea un Resource Group.
2. Crea un App Service Plan Linux con SKU `B1`.
3. Crea una Web App Linux dentro de ese plan.
4. Activa `Always On` para mantener el bot activo.

### Paso 2: Configurar variables en Azure

En la Web App, agrega:

- `TELEGRAM_BOT_TOKEN`
- `ASPNETCORE_ENVIRONMENT=Production`

Opcional:

- `BOT_LOGS_PATH` para ruta custom de logs.

### Paso 3: Configurar CI/CD con GitHub Actions

En GitHub Secrets crea:

- `AZURE_CREDENTIALS`
- `AZURE_WEBAPP_NAME`

Workflow usado:

- `.github/workflows/azure-deploy.yml`

### Paso 4: Desplegar

Haz `push` a `master` y valida que el workflow termine en verde.

---

## Verificar instalacion

1. En Telegram, envia `/start`.
2. Crea un recordatorio de prueba.
3. Si usas RPG, prueba `/rpg`.
4. Revisa logs de aplicacion en consola o Azure.

---

## Troubleshooting

### Token no encontrado

- Verifica `TELEGRAM_BOT_TOKEN`.
- Si usas local, revisa `appsettings.json`.

### Error de compilacion

```powershell
dotnet restore
dotnet build
```

### API no responde

- Revisa puerto configurado por `PORT` o `ASPNETCORE_URLS`.
- En Azure, revisa Application Logs.

---

## Siguientes pasos

1. Revisar `Docs/USAGE.md` para comandos.
2. Revisar `Docs/API.md` para integraciones.
3. Revisar `deploy/AZURE_SETUP.md` para operacion en produccion.
