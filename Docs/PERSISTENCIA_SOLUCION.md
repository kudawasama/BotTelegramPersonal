# 🔧 Solución de Persistencia de Datos

## 📋 PROBLEMA IDENTIFICADO

### Causa Raíz
```
data/rpg_players.json → Guardado en contenedor Docker efímero
Nuevo deploy → Contenedor reemplazado → ❌ DATOS PERDIDOS
```

### Evidencia
```csharp
// RpgService.cs línea 48-55
var dataDir = Path.Combine(projectRoot, "data");
_filePath = Path.Combine(dataDir, "rpg_players.json");
```

---

## ✅ SOLUCIÓN 1: FLY.IO VOLUMES (IMPLEMENTADO)

### Configuración
Volumen persistente que sobrevive a deploys y mantiene los datos.

### Pasos de Configuración

#### 1. **Crear volumen en Fly.io**
```powershell
fly volumes create rpg_data --region gru --size 1
```

#### 2. **Actualizar fly.toml**
```toml
[mounts]
  source = "rpg_data"
  destination = "/app/data"
```

#### 3. **Verificar**
```powershell
fly volumes list
fly deploy
fly ssh console
ls -la /app/data/
```

### Ventajas
- ✅ Datos persisten entre deploys
- ✅ Sin cambios en código
- ✅ Backup automático semanal
- ✅ Free tier: 3GB gratis

---

## ✅ SOLUCIÓN 2: EXPORT/IMPORT PERSONAJES (IMPLEMENTADO)

### Comandos Añadidos

#### **Exportar Personaje**
```
/rpg → ⚙️ Opciones → 💾 Exportar Personaje
```
- Genera archivo JSON con todo el progreso
- Se envía por Telegram
- El jugador lo guarda en su dispositivo

#### **Importar Personaje**
```
/rpg → ⚙️ Opciones → 📥 Importar Personaje
```
- Envía el archivo JSON guardado
- Sistema lo procesa y restaura progreso
- Valida integridad de datos

### Código Implementado
```csharp
// Callbacks añadidos:
rpg_export_character  → Genera y envía JSON
rpg_import_character  → Activa modo de recepción
rpg_import_confirm    → Procesa archivo recibido
```

---

## ✅ SOLUCIÓN 3: ACTUALIZACIÓN INTELIGENTE (IMPLEMENTADO)

### Sistema de Migración Automática

Cuando un jugador con datos antiguos se conecta después de una actualización:

```csharp
public void MigratePlayerData(RpgPlayer player)
{
    // Detecta campos nuevos
    if (player.ActiveMinions == null)
        player.ActiveMinions = new List<Minion>();
    
    if (player.UnlockedZones == null)
        player.UnlockedZones = new List<string> { "puerto_esperanza" };
    
    // ✅ NO BORRA progreso existente
    // ✅ SOLO AGREGA campos nuevos
}
```

### Ventaja
- 🔄 Actualización transparente
- ✅ Sin pérdida de progreso
- ✅ Retrocompatibilidad

---

## 📊 COMPARACIÓN DE SOLUCIONES

| Solución | Automático | Seguro | Setup |
|----------|------------|--------|-------|
| **Fly Volumes** | ✅ | ✅✅✅ | Fácil |
| **Export/Import** | ❌ | ✅✅ | Ninguno |
| **Migración** | ✅ | ✅ | Ya incluido |

---

## 🎯 RECOMENDACIÓN FINAL

### **Usar TODAS las soluciones simultáneamente:**

1. **Fly Volumes** → Persistencia principal
2. **Export/Import** → Backup manual de emergencia
3. **Migración** → Compatibilidad con actualizaciones

### **Workflow del Jugador:**
```
1. Jugar normalmente (Volumes maneja todo)
2. Antes de actualizaciones grandes:
   → Exportar personaje (backup)
3. Después de actualizar:
   → Sistema migra automáticamente
   → Si falla → Importar backup
```

---

## 🚀 COMANDOS ÚTILES

### Verificar volumen
```powershell
fly volumes list
fly volumes show rpg_data
```

### Ver logs del bot
```powershell
fly logs
```

### Acceder al contenedor
```powershell
fly ssh console
cat /app/data/rpg_players.json
```

### Backup manual desde contenedor
```powershell
fly ssh console
cat /app/data/rpg_players.json > backup.json
```

---

## ⚠️ IMPORTANTE

### **Antes de cada actualización GRANDE:**
1. Avisar a jugadores exportar personajes
2. Hacer backup del volumen
3. Desplegar actualización
4. Verificar migración funcionó
5. Si hay problemas → restaurar backup

### **Política de Backups:**
- Fly.io: Backup automático semanal
- Manual: Antes de cambios en RpgPlayer model
- Jugadores: Pueden exportar cuando quieran
