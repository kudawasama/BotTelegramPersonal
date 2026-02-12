# 🏠 Plan de Setup - Fly.io desde Notebook Personal

Este documento es tu guía para configurar el bot en Fly.io desde tu notebook personal, y luego trabajar desde el PC de la empresa sin necesitar instalar nada adicional.

---

## 📋 RESUMEN DEL PLAN

### **Fase 1: Setup inicial (desde notebook en casa - 15 min)**
Instalas Fly CLI, creas la app, haces el primer deploy.

### **Fase 2: Configurar auto-deploy (5 min)**
Configuras GitHub Actions para que cada `git push` haga deploy automático.

### **Fase 3: Trabajo diario (desde PC empresa)**
Solo necesitas Git y navegador. Cada push despliega automáticamente.

---

## 🏠 FASE 1: Setup desde Notebook Personal

### **1.1 Instalar Fly CLI**

Abre PowerShell normal (no admin necesario):

```powershell
# Instalar Fly CLI
powershell -Command "iwr https://fly.io/install.ps1 -useb | iex"
```

Cierra y abre nueva terminal.

Verifica:
```powershell
fly version
```

---

### **1.2 Login en Fly.io**

```powershell
fly auth login
```

Se abre navegador:
1. **Sign up** con GitHub o email
2. Completa perfil
3. **Agregar tarjeta** (solo verificación, gratis si cabe en 256MB)
4. Vuelve a terminal → "✅ successfully logged in"

---

### **1.3 Clonar repositorio (si no lo tienes)**

```powershell
cd C:\Users\TU_USUARIO\Documents
git clone https://github.com/kudawasama/BotTelegramPersonal.git
cd BotTelegramPersonal
```

O si ya lo tienes:
```powershell
cd ruta\a\BotTelegramPersonal
git pull origin master
```

---

### **1.4 Crear app en Fly.io**

```powershell
# Crear app (usa fly.toml ya configurado)
fly launch --no-deploy

# Confirma:
# - App name: bottelegram (o el que prefieras)
# - Region: scl (Santiago, Chile)
# - PostgreSQL? → NO
# - Redis? → NO
```

**Anota el nombre de la app que elegiste** (lo necesitas después).

---

### **1.5 Configurar secrets (tokens)**

**Reemplaza con tus tokens reales:**

```powershell
fly secrets set TELEGRAM_BOT_TOKEN="7898706508:AAG5vJ7zXXXXXXXXXXXXXXXXXXXX"

fly secrets set GROQ_API_KEY="gsk_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
```

---

### **1.6 Primer deploy**

```powershell
fly deploy
```

⏳ Espera 3-5 minutos...

Cuando termine:
```powershell
# Ver logs
fly logs

# Debe mostrar:
# ✅ Token cargado correctamente
# 🤖 Bot iniciado correctamente
```

**Prueba en Telegram:** `/rpg` → debe responder

---

## 🔄 FASE 2: Configurar Auto-Deploy con GitHub Actions

### **2.1 Obtener token de Fly.io**

```powershell
fly tokens create deploy
```

**Copia el token que aparece** (empieza con `FlyV1_...`)

---

### **2.2 Agregar token a GitHub**

1. Ve a: https://github.com/kudawasama/BotTelegramPersonal/settings/secrets/actions

2. Click **"New repository secret"**

3. Name: `FLY_API_TOKEN`

4. Value: **Pega el token** de `fly tokens create deploy`

5. Click **"Add secret"**

---

### **2.3 Verificar que GitHub Actions está activo**

El archivo `.github/workflows/fly-deploy.yml` ya está en el repo.

Para verificar:
```powershell
# Desde tu notebook
git pull origin master

# Verifica que existe
ls .github\workflows\fly-deploy.yml
```

Si existe → ✅ Todo listo

---

### **2.4 Probar auto-deploy**

```powershell
# Hacer un cambio de prueba
echo "# Auto-deploy configurado" >> README.md

# Commit y push
git add README.md
git commit -m "test: verificar auto-deploy"
git push origin master
```

**Verifica en GitHub:**
1. Ve a: https://github.com/kudawasama/BotTelegramPersonal/actions
2. Deberías ver un workflow corriendo
3. Espera a que termine (círculo verde)
4. Bot se actualiza automáticamente en Fly.io

---

## 💼 FASE 3: Trabajo Diario desde PC de Empresa

### **3.1 Lo que SÍ necesitas en PC empresa:**

✅ Git (ya lo tienes)  
✅ VS Code (ya lo tienes)  
✅ Navegador web  

❌ NO necesitas Fly CLI  
❌ NO necesitas Docker  
❌ NO necesitas permisos admin  

---

### **3.2 Flujo de trabajo normal:**

```powershell
# En PC de empresa
cd C:\Users\jose.cespedes\Documents\GitHub\BotTelegram

# 1. Pull últimos cambios
git pull origin master

# 2. Editar código (mejoras, fixes, features)
# ... trabajas en VS Code ...

# 3. Commit y push
git add .
git commit -m "feat: nueva funcionalidad"
git push origin master

# 4. GitHub Actions hace deploy automático (3-5 min)
# ✅ Listo, bot actualizado
```

---

### **3.3 Ver logs y status (desde navegador):**

**Dashboard de Fly.io:**
https://fly.io/dashboard

- **Logs en tiempo real**
- **Métricas** (CPU, RAM, red)
- **Status** (running, stopped)
- **Billing** (cuánto llevas gastado)

**GitHub Actions (historial de deploys):**
https://github.com/kudawasama/BotTelegramPersonal/actions

- Ver si deploy pasó o falló
- Logs del proceso de deploy
- Tiempo de cada deploy

---

### **3.4 Comandos útiles (solo Git):**

```powershell
# Ver status local
git status

# Ver últimos commits
git log --oneline -10

# Crear branch para feature nueva
git checkout -b feature/nueva-funcionalidad

# Trabajar en la branch
git add .
git commit -m "feat: trabajo en progreso"
git push origin feature/nueva-funcionalidad

# Merge a master (trigger deploy)
git checkout master
git merge feature/nueva-funcionalidad
git push origin master
# → Auto-deploy se activa
```

---

## 📊 MONITOREO Y MANTENIMIENTO

### **Desde navegador (PC empresa o casa):**

#### **Ver logs en tiempo real:**
https://fly.io/dashboard → Tu app → Logs

#### **Ver costos:**
https://fly.io/dashboard → Billing

**Tu bot probablemente usa:**
- $0 USD/mes (si cabe en free tier)
- $1-2 USD/mes (si usa un poco más)

#### **Ver deploys:**
https://github.com/kudawasama/BotTelegramPersonal/actions

---

## 🔧 TROUBLESHOOTING

### **Deploy falló en GitHub Actions:**

1. Ve a https://github.com/kudawasama/BotTelegramPersonal/actions
2. Click el workflow fallido
3. Revisa logs
4. Causas comunes:
   - Token `FLY_API_TOKEN` expiró o es incorrecto
   - Error de compilación en el código
   - Problema de red GitHub ↔ Fly.io

### **Bot no responde en Telegram:**

1. Ve a https://fly.io/dashboard → Tu app → Logs
2. Busca errores
3. Verifica que variables estén configuradas:
   ```powershell
   # Desde notebook (con Fly CLI)
   fly secrets list
   ```

### **Necesitas rehacer algo desde notebook:**

```powershell
# Desde notebook personal
cd ruta\a\BotTelegramPersonal

# Re-deploy manual
fly deploy

# Ver logs
fly logs

# Reiniciar app
fly apps restart bottelegram

# Ver status
fly status
```

---

## 📝 CHECKLIST FINAL

### **Antes de irte de casa (notebook):**

- [ ] Fly CLI instalado y funcionando
- [ ] `fly auth login` exitoso
- [ ] `fly launch` completado
- [ ] Secrets configurados (TELEGRAM_BOT_TOKEN, GROQ_API_KEY)
- [ ] Primer `fly deploy` exitoso
- [ ] Bot responde en Telegram
- [ ] Token `FLY_API_TOKEN` agregado a GitHub Secrets
- [ ] Test de auto-deploy funcionando (workflow verde)

### **En PC de empresa (mañana):**

- [ ] `git pull` funciona
- [ ] Puedes editar código
- [ ] `git push` dispara auto-deploy
- [ ] Dashboard de Fly.io accesible desde navegador
- [ ] Logs visibles en dashboard

---

## 🎯 RESUMEN VISUAL

```
┌─────────────────────────────────────────────────────────────┐
│  NOTEBOOK (Casa) - Setup 1 vez                              │
│  ┌────────────┐   ┌──────────┐   ┌───────────┐             │
│  │  Fly CLI   │ → │ fly auth │ → │fly deploy │             │
│  └────────────┘   └──────────┘   └───────────┘             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  GITHUB (intermediario automático)                          │
│  ┌────────────┐   ┌──────────────┐   ┌──────────────┐      │
│  │ git push   │ → │GitHub Actions│ → │  fly deploy  │      │
│  └────────────┘   └──────────────┘   └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  PC EMPRESA (trabajo diario)                                │
│  ┌────────────┐   ┌──────────┐   ┌───────────┐             │
│  │  VS Code   │ → │ git push │ → │ Navegador │             │
│  │  (editar)  │   │ (deploy) │   │  (logs)   │             │
│  └────────────┘   └──────────┘   └───────────┘             │
└─────────────────────────────────────────────────────────────┘
```

---

## 💡 CONSEJOS

1. **Guarda tu token FLY_API_TOKEN** en un lugar seguro (por si necesitas regenerarlo)

2. **Configura límite de gasto** (primera vez en casa):
   ```powershell
   fly billing limit 5  # Máximo $5 USD/mes
   ```

3. **Branches para features grandes:**
   - Trabaja en branch separada
   - Solo merge a `master` cuando esté listo
   - `master` = producción (auto-deploy)

4. **Logs son tu amigo:**
   - Siempre revisa logs después de deploy
   - Dashboard de Fly.io → Logs en tiempo real

5. **Testea localmente antes de push:**
   ```powershell
   # En PC empresa
   cd src/BotTelegram
   dotnet run
   # Prueba que compila y funciona
   # Luego git push
   ```

---

## 🎉 ¡Éxito!

Con este setup tendrás:
- ✅ Bot 24/7 sin sleep
- ✅ Auto-deploy desde PC de empresa
- ✅ Sin necesidad de CLI en trabajo
- ✅ Probablemente gratis ($0-2 USD/mes)
- ✅ Logs en tiempo real desde navegador

**Cuando llegues a casa, sigue la FASE 1 paso a paso. ¡Avísame si tienes dudas!**
