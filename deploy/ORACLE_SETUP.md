# 🔥 Guía de Deployment en Oracle Cloud Always Free

## ✅ Ventajas
- **GRATIS PARA SIEMPRE** (no expira)
- **Siempre activo** (sin sleep)
- 1 GB RAM, 1 vCPU, 10GB disco
- Control total del servidor

---

## 📋 PASO 1: Crear Cuenta Oracle Cloud

1. Ve a: https://www.oracle.com/cloud/free/
2. Click **"Start for free"**
3. Completa registro:
   - Email
   - Contraseña
   - País
   - **Tarjeta de crédito** (solo verificación, NO se cobra)
4. Verifica email
5. Espera aprobación (puede tardar 5-30 min)

---

## 🖥️ PASO 2: Crear VM (Compute Instance)

1. Login en: https://cloud.oracle.com
2. Dashboard → **Compute** → **Instances**
3. Click **"Create Instance"**

### Configuración:
- **Name:** `bottelegram`
- **Image:** Ubuntu 22.04 (cambiar si viene otra)
- **Shape:** 
  - Click "Change Shape"
  - Selecciona **"Ampere"** → **VM.Standard.A1.Flex**
  - OCPU: 1
  - Memory: 6 GB (puedes usar hasta 24GB gratis!)
- **Networking:**
  - Deja la VCN por defecto
  - ✅ **Assign a public IPv4 address**
- **SSH Keys:**
  - Selecciona **"Generate SSH key pair"**
  - Click **"Save Private Key"** → Guarda el archivo `.key`
  - Click **"Save Public Key"** → Guarda el archivo `.pub`
4. Click **"Create"**
5. Espera 2-3 min (icono naranja → verde)
6. **Copia la IP pública** que aparece

---

## 🔓 PASO 3: Abrir Puertos (Firewall)

### En Oracle Cloud Console:
1. En tu instancia → **Virtual cloud network** → Click en el VCN
2. **Security Lists** → Click en "Default Security List"
3. **Add Ingress Rules:**
   - Source CIDR: `0.0.0.0/0`
   - Destination Port: `10000`
   - Description: `Bot Web API`
4. Click **"Add Ingress Rule"**

### En la VM (después de conectarte):
```bash
sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 10000 -j ACCEPT
sudo netfilter-persistent save
```

---

## 🔌 PASO 4: Conectarte a la VM por SSH

### En Windows PowerShell:
```powershell
# Cambia la ruta a donde guardaste la clave privada
ssh -i "C:\Users\jose.cespedes\Downloads\ssh-key.key" ubuntu@TU_IP_PUBLICA
```

**Si da error de permisos:**
```powershell
icacls "C:\Users\jose.cespedes\Downloads\ssh-key.key" /inheritance:r
icacls "C:\Users\jose.cespedes\Downloads\ssh-key.key" /grant:r "%username%:R"
```

---

## ⚙️ PASO 5: Instalar Bot (Automático)

Una vez conectado por SSH:

```bash
# Descargar script de instalación
wget https://raw.githubusercontent.com/kudawasama/BotTelegramPersonal/master/deploy/oracle-setup.sh

# Darle permisos de ejecución
chmod +x oracle-setup.sh

# Ejecutar instalación
./oracle-setup.sh
```

El script instalará:
- ✅ .NET 8 SDK
- ✅ Git
- ✅ Tu bot (clone + build)
- ✅ Servicio systemd para auto-restart

---

## 🔐 PASO 6: Configurar Tokens

```bash
# Editar variables de entorno
nano ~/.bottelegram.env
```

Cambia:
```bash
TELEGRAM_BOT_TOKEN=7898706508:AAG5vJ7zXXXXXXXXXXXXXXXXXXXX
GROQ_API_KEY=gsk_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

**Guardar:** `Ctrl+O` → `Enter` → `Ctrl+X`

---

## 🚀 PASO 7: Iniciar el Bot

```bash
# Recargar systemd
sudo systemctl daemon-reload

# Iniciar bot
sudo systemctl start bottelegram

# Ver estado
sudo systemctl status bottelegram

# Ver logs en tiempo real
sudo journalctl -u bottelegram -f

# Habilitar auto-inicio al reiniciar VM
sudo systemctl enable bottelegram
```

---

## 📊 Comandos Útiles

```bash
# Ver logs del bot
sudo journalctl -u bottelegram -f

# Reiniciar bot
sudo systemctl restart bottelegram

# Detener bot
sudo systemctl stop bottelegram

# Ver estado
sudo systemctl status bottelegram

# Actualizar bot con últimos cambios de GitHub
cd ~/BotTelegram
git pull
cd ~/BotTelegram/src/BotTelegram
dotnet publish -c Release -o ~/BotTelegram/publish
sudo systemctl restart bottelegram
```

---

## 🔄 Actualizar Bot Desde GitHub

Cuando hagas cambios en tu código:

```bash
# Conectarte por SSH
ssh -i "ruta/a/tu/clave.key" ubuntu@TU_IP

# Actualizar código
cd ~/BotTelegram
git pull

# Re-compilar
cd ~/BotTelegram/src/BotTelegram
dotnet publish -c Release -o ~/BotTelegram/publish

# Reiniciar servicio
sudo systemctl restart bottelegram

# Ver logs para verificar
sudo journalctl -u bottelegram -f
```

---

## ⚠️ Troubleshooting

### Bot no inicia:
```bash
# Ver logs detallados
sudo journalctl -u bottelegram -n 50 --no-pager

# Verificar que .NET está instalado
dotnet --version

# Verificar permisos
ls -la ~/BotTelegram/publish/
```

### Puerto 10000 no responde:
```bash
# Verificar firewall local
sudo iptables -L -n | grep 10000

# Abrir puerto
sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 10000 -j ACCEPT
sudo netfilter-persistent save
```

### Reinstalar todo:
```bash
sudo systemctl stop bottelegram
sudo systemctl disable bottelegram
rm -rf ~/BotTelegram
./oracle-setup.sh
```

---

## 💰 Costos

**$0.00 USD** - El tier Always Free incluye:
- 2 VMs Ampere A1 (hasta 4 OCPU + 24GB RAM total)
- 200 GB almacenamiento
- Tráfico ilimitado

**NO CADUCA NUNCA** mientras uses los recursos al menos una vez cada 90 días.

---

## 🎉 ¡Listo!

Tu bot ahora está:
- ✅ Activo 24/7 sin sleep
- ✅ Auto-restart si crashea
- ✅ Auto-inicio al reiniciar VM
- ✅ GRATIS para siempre
