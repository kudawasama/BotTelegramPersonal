# 🚀 Guía de Deployment en Fly.io

## ✅ Ventajas
- **GRATIS** si tu bot cabe en 256MB RAM (probablemente sí)
- **Siempre activo** (sin sleep)
- **Región Chile** (Santiago) - baja latencia
- Setup en 5 minutos
- Muy usado por comunidad de Telegram bots

---

## 📋 PASO 1: Instalar Fly CLI

### En PowerShell (Administrador):
```powershell
powershell -Command "iwr https://fly.io/install.ps1 -useb | iex"
```

Cierra y abre nueva terminal después de instalar.

### Verificar instalación:
```powershell
fly version
```

---

## 🔐 PASO 2: Login en Fly.io

```powershell
fly auth login
```

Se abrirá navegador:
1. **Sign up** (crear cuenta) o **Log in**
2. Completa registro
3. **Agrega tarjeta de crédito** (obligatorio pero solo cobran si excedes free tier)
4. Vuelve a terminal → debe decir "successfully logged in"

---

## 🚀 PASO 3: Deploy desde tu proyecto

```powershell
# Ir al directorio del proyecto
cd C:\Users\jose.cespedes\Documents\GitHub\BotTelegram

# Crear app en Fly.io (usa fly.toml existente)
fly launch --no-deploy

# Cuando pregunte:
# - "Would you like to set up a PostgreSQL database?" → NO
# - "Would you like to set up an Upstash Redis database?" → NO
# - Confirma región: Santiago (scl)

# Configurar secrets (variables de entorno)
fly secrets set TELEGRAM_BOT_TOKEN="7898706508:AAG5vJ7zXXXXXXXXXXXXXXXXXXXX"
fly secrets set GROQ_API_KEY="gsk_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"

# Deploy!
fly deploy
```

---

## ⏳ Proceso de Deploy

El deploy toma 3-5 minutos:
1. 📦 Construye la imagen Docker
2. ⬆️ Sube a Fly.io
3. 🚀 Inicia la máquina virtual
4. ✅ Verifica health checks

---

## 📊 PASO 4: Verificar que funciona

### Ver logs en tiempo real:
```powershell
fly logs
```

Deberías ver:
```
✅ Token cargado correctamente
🤖 Bot iniciado correctamente
📊 [CICLO X] ...
```

### Ver status:
```powershell
fly status
```

### Verificar en Telegram:
Envía `/rpg` → debe responder

---

## 🎛️ Comandos Útiles

```powershell
# Ver logs en tiempo real
fly logs

# Ver estado de la app
fly status

# Abrir dashboard web
fly dashboard

# Escalar memoria (si necesitas más)
fly scale memory 512  # Cobra extra

# Ver métricas de uso
fly status --all

# Re-deploy después de cambios en código
git push origin master
fly deploy

# SSH a la máquina (debugging)
fly ssh console

# Ver información de facturación
fly billing

# Detener app (deja de correr, no cobra)
fly scale count 0

# Reiniciar app
fly scale count 1
```

---

## 💰 Monitoreo de Costos

### Verificar uso mensual:
```powershell
fly billing
```

### Dashboard web:
```
https://fly.io/dashboard
```

Ve a **Billing** → verás:
- Uso actual del mes
- Proyección de costo
- Desglose por recurso

### Configurar límite de gasto:
```powershell
fly billing limit 5  # Máximo $5 USD/mes
```

---

## 🔄 Actualizar Bot (después de cambios)

```powershell
# En tu PC
cd C:\Users\jose.cespedes\Documents\GitHub\BotTelegram
git pull  # Si trabajas desde otro lado

# Deploy nueva versión
fly deploy

# Ver logs para verificar
fly logs
```

---

## 📈 Escalamiento (si crece tu bot)

### Aumentar RAM (si necesitas):
```powershell
# 512MB (sale del free tier, ~$2-3/mes)
fly scale memory 512

# Volver a 256MB (free tier)
fly scale memory 256
```

### Múltiples instancias:
```powershell
# 2 instancias (redundancia)
fly scale count 2

# Volver a 1
fly scale count 1
```

---

## ⚠️ Troubleshooting

### Bot no inicia:
```powershell
# Ver logs detallados
fly logs

# Ver eventos de la máquina
fly status --all

# Reiniciar
fly apps restart bottelegram
```

### Puerto no responde:
Verifica `fly.toml`:
```toml
[http_service]
  internal_port = 10000  # Debe coincidir con tu app
```

### Out of Memory:
```powershell
# Aumentar a 512MB (cobra ~$2-3/mes extra)
fly scale memory 512
```

### Secrets no funcionan:
```powershell
# Listar secrets
fly secrets list

# Setear de nuevo
fly secrets set TELEGRAM_BOT_TOKEN="tu_token"
```

---

## 🌍 Regiones Disponibles

Tu bot está en **Santiago (scl)** - Chile.

Otras opciones cercanas:
- `gru` - São Paulo, Brasil
- `iad` - Virginia, USA
- `mia` - Miami, USA

Cambiar región:
```powershell
fly regions set scl gru  # Primary: Santiago, Backup: São Paulo
```

---

## 💸 Costo Estimado

**Free Tier (256MB RAM, 1 instancia):**
- Bot pequeño: **$0 USD** ✅
- Bot mediano: **$0-1 USD**
- Bot grande: **$2-3 USD**

**Con 512MB RAM:**
- **$3-5 USD/mes**

Tu bot probablemente será **$0-1 USD/mes**.

---

## 🔒 Mejores Prácticas

1. **Monitorea tu billing** cada semana al inicio
2. **Pon límite de gasto:** `fly billing limit 5`
3. **No escales hasta que necesites:** 256MB es suficiente
4. **Usa logs para debugging:** `fly logs` en vez de SSH
5. **Mantén secrets actualizados:** nunca en código

---

## 🎉 ¡Listo!

Tu bot ahora está:
- ✅ Activo 24/7 en Chile (baja latencia)
- ✅ Sin sleep
- ✅ Probablemente GRATIS
- ✅ Auto-restart si crashea
- ✅ Logs en tiempo real
