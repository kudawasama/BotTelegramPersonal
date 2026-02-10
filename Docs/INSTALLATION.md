# 📦 Instalación de BotTelegram

> Guía completa para instalar y configurar BotTelegram en diferentes entornos.

---

## 🔧 Requisitos previos

### Opción 1: Replit (Recomendado - Sin instalación)
- Navegador web
- Cuenta en [Replit.com](https://replit.com) (gratis)
- Token de Telegram Bot

### Opción 2: Local - Windows/macOS/Linux
- **.NET 9.0** o superior
- **Git** para clonar el repositorio
- **Token de Telegram Bot**

### Opción 3: Docker
- **Docker** instalado
- Token de Telegram Bot

---

## 🚀 Instalación Replit (Más fácil)

### Paso 1: Crea cuenta en Replit
1. Abre https://replit.com
2. Click en **"Sign up"**
3. Usa tu cuenta **Google**, **GitHub** o **email**

### Paso 2: Importa el repositorio
1. Click en **"Create"** en la página principal
2. Click en **"Import from GitHub"**
3. Pega la URL:
   ```
   https://github.com/kudawasama/BotTelegramPersonal
   ```
4. Click en **"Import"**

### Paso 3: Obtén tu token de Telegram
1. Abre Telegram y busca **@BotFather**
2. Envía el comando `/mybots`
3. Selecciona tu bot
4. Copia el token (formato: `1234567890:ABCDefGhIjklmnop...`)

### Paso 4: Añade el token como Secret
1. En Replit, click en **🔑 Secrets** (icono en la izquierda)
2. Click en **"Add new secret"**
3. Llena:
   - **Key:** `TELEGRAM_BOT_TOKEN`
   - **Value:** Tu token de Telegram
4. Click en **"Add Secret"**

### Paso 5: Ejecuta el bot
En la terminal de Replit, escribe:

```bash
cd src/BotTelegram
dotnet run
```

Verás algo como:
```
✅ Token cargado correctamente
🤖 Bot iniciado correctamente
🌐 API REST iniciada en http://localhost:5000
✅ StartReceiving() iniciado
📱 Telegram Bot: Presiona ENTER para salir
```

✅ **¡Listo! Tu bot está en línea 24/7**

---

## 💻 Instalación Local - Windows

### Paso 1: Instala .NET 9.0
1. Descarga: https://dotnet.microsoft.com/download
2. Descarga **.NET 9.0 SDK** (no Runtime)
3. Ejecuta el instalador
4. Verifica la instalación:
   ```powershell
   dotnet --version
   ```
   Debería mostrar `9.0.x`

### Paso 2: Clona el repositorio
```powershell
cd Desktop
git clone https://github.com/kudawasama/BotTelegramPersonal.git
cd BotTelegram
```

### Paso 3: Configura el token
Abre `src/BotTelegram/appsettings.json`:

```json
{
  "Telegram": {
    "Token": "tu_token_aqui"
  }
}
```

⚠️ **Nunca** commitees el token a Git

### Paso 4: Ejecuta el bot
```powershell
cd src/BotTelegram
dotnet run
```

---

## 🍎 Instalación Local - macOS

### Paso 1: Instala .NET (con Brew)
```bash
brew install dotnet
```

Verifica:
```bash
dotnet --version
```

### Paso 2-4: Igual que Windows

---

## 🐧 Instalación Local - Linux

### Paso 1: Instala .NET
```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest

# Fedora
sudo dnf install dotnet-sdk-9.0
```

### Paso 2-4: Igual que Windows

---

## 🐳 Instalación Docker

### Paso 1: Crea Dockerfile
En la raíz del proyecto, crea `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
WORKDIR /src
COPY . .
RUN dotnet build "src/BotTelegram/BotTelegram.csproj" -c Release

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /src/src/BotTelegram/bin/Release/net9.0 .
ENV TELEGRAM_BOT_TOKEN=""
ENTRYPOINT ["dotnet", "BotTelegram.dll"]
```

### Paso 2: Build la imagen
```bash
docker build -t bottelegram:latest .
```

### Paso 3: Run el contenedor
```bash
docker run -e TELEGRAM_BOT_TOKEN=tu_token_aqui bottelegram:latest
```

---

## ☁️ Deploy en Railway

### Paso 1: Crea cuenta
https://railway.app

### Paso 2: Crea proyecto
1. Click en **"New Project"**
2. **"Deploy from GitHub repo"**
3. Selecciona `BotTelegramPersonal`

### Paso 3: Configura variables
En el panel de Railway:
- Click en **"Variables"**
- Añade: `TELEGRAM_BOT_TOKEN=tu_token`

### Paso 4: Deploy
Railway detecta automáticamente que es .NET y lo despliega.

---

## ☁️ Deploy en Fly.io

### Paso 1: Instala Fly CLI
```powershell
choco install flyctl
```

### Paso 2: Autentica
```bash
flyctl auth signup
```

### Paso 3: Crea app
```bash
cd BotTelegram
flyctl launch
```

### Paso 4: Configura secrets
```bash
flyctl secrets set TELEGRAM_BOT_TOKEN=tu_token
```

### Paso 5: Deploy
```bash
flyctl deploy
```

---

## 🚀 Verificar instalación

### Opción 1: Prueba con Telegram
1. Abre Telegram
2. Busca tu bot
3. Envía `/help`
4. Debería responder con el listado de comandos

### Opción 2: Prueba API
```bash
curl http://localhost:5000/api/reminders
```

Debería devolver: `[]` (lista vacía)

---

## 🛠️ Troubleshooting

### "Token no encontrado"
- Verifica que `appsettings.json` existe
- Verifica que el token está correcto
- En Replit, verifica que el Secret está añadido

### "Puerto 5000 ya está en uso"
```powershell
# Encuentra el proceso
netstat -ano | findstr :5000

# Mata el proceso (Windows)
taskkill /PID <PID> /F

# O cambia el puerto en Program.cs
```

### "dotnet no se reconoce"
- Reinicia PowerShell/Terminal
- Instala .NET SDK (no Runtime)
- Verifica: `dotnet --version`

### "No se puede conectar a Telegram"
- Verifica tu conexión a internet
- Verifica que el token es correcto
- Intenta con `/newtoken` en @BotFather

---

## ✅ Próximos pasos

1. Lee [USAGE.md](./USAGE.md) para aprender los comandos
2. Lee [API.md](./API.md) para integrar con otras apps
3. Abre [ROADMAP.md](./ROADMAP.md) para ver planes futuros

---

**¿Problemas?** Abre un [Issue en GitHub](https://github.com/kudawasama/BotTelegramPersonal/issues)
