#!/bin/bash
# Script de instalación automática para Oracle Cloud Ubuntu VM

set -e

echo "🚀 Instalando BotTelegram en Oracle Cloud..."

# Actualizar sistema
echo "📦 Actualizando sistema..."
sudo apt update && sudo apt upgrade -y

# Instalar .NET 8 SDK
echo "📦 Instalando .NET 8 SDK..."
sudo apt install -y wget apt-transport-https
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# Instalar Git (si no está)
echo "📦 Instalando Git..."
sudo apt install -y git

# Clonar repositorio
echo "📥 Clonando repositorio..."
cd ~
if [ -d "BotTelegram" ]; then
    echo "⚠️  Directorio BotTelegram ya existe, actualizando..."
    cd BotTelegram
    git pull
else
    git clone https://github.com/kudawasama/BotTelegramPersonal.git BotTelegram
    cd BotTelegram
fi

# Crear directorio de datos
echo "📁 Creando directorios de datos..."
mkdir -p ~/BotTelegram/src/BotTelegram/bin/Release/net8.0/data

# Build del proyecto
echo "🔨 Compilando proyecto..."
cd ~/BotTelegram/src/BotTelegram
dotnet publish -c Release -o ~/BotTelegram/publish

# Crear archivo de variables de entorno
echo "🔐 Configurando variables de entorno..."
if [ ! -f ~/.bottelegram.env ]; then
    echo "TELEGRAM_BOT_TOKEN=TU_TOKEN_AQUI" > ~/.bottelegram.env
    echo "GROQ_API_KEY=TU_GROQ_KEY_AQUI" >> ~/.bottelegram.env
    echo ""
    echo "⚠️  IMPORTANTE: Edita el archivo ~/.bottelegram.env con tus tokens reales"
    echo "   nano ~/.bottelegram.env"
fi

# Crear servicio systemd
echo "⚙️  Creando servicio systemd..."
sudo tee /etc/systemd/system/bottelegram.service > /dev/null <<EOF
[Unit]
Description=Bot Telegram Personal
After=network.target

[Service]
Type=notify
User=$USER
WorkingDirectory=$HOME/BotTelegram/publish
ExecStart=/usr/bin/dotenv -f $HOME/.bottelegram.env /usr/bin/dotnet $HOME/BotTelegram/publish/BotTelegram.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

# Instalar dotenv (para cargar variables de entorno)
echo "📦 Instalando dotenv..."
sudo apt install -y ruby
sudo gem install dotenv-cli

echo ""
echo "✅ Instalación completada!"
echo ""
echo "📝 PRÓXIMOS PASOS:"
echo "1. Edita tus tokens: nano ~/.bottelegram.env"
echo "2. Recarga systemd: sudo systemctl daemon-reload"
echo "3. Inicia el bot: sudo systemctl start bottelegram"
echo "4. Verifica estado: sudo systemctl status bottelegram"
echo "5. Ver logs: sudo journalctl -u bottelegram -f"
echo "6. Habilitar auto-inicio: sudo systemctl enable bottelegram"
