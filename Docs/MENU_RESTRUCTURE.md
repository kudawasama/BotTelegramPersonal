# 🎮 REESTRUCTURACIÓN DE MENÚS RPG

> **Documento de Diseño:** Optimización y Expansión de Menús del Sistema RPG
> 
> **Fecha:** 13 de Febrero de 2026
> 
> **Estado:** 🚧 EN PROGRESO

---

## 📋 RESUMEN DE CAMBIOS

### ✅ **COMPLETADO**
- ✅ Menú principal reorganizado (6 filas)
- ✅ Menú de combate mejorado (3x3)
- ✅ Menú de exploración expandido (6 opciones)
- ✅ Aventura aleatoria implementada
- ✅ Búsqueda de recursos implementada
- ✅ Búsqueda de tesoros implementada
- ✅ Búsqueda de mascotas implementada
- ✅ Eventos aleatorios implementados
- ✅ Menú de tácticas en combate
- ✅ Menú de opciones implementado
- ✅ Compilación exitosa

### 🔄 **EN PROGRESO**
- Ninguno

### ⏳ **PENDIENTE**
- Testing en producción
- Deploy a Fly.io

---

## 📊 ANÁLISIS DE MENÚS ACTUALES

### 🏠 **MENÚ PRINCIPAL (Exploración)**

**Botones Actuales:**
```
Fila 1: ⚔️ Explorar | 🛡️ Entrenar
Fila 2: 😴 Descansar | 💼 Trabajar
Fila 3: 📊 Stats | 🎒 Equipment
Fila 4: 🏪 Tienda
Fila 5: ✨ Skills | 📈 Counters
Fila 6: 🌟 Skills Combinadas
Fila 7: 🌟 Progreso | 💎 Pasivas
Fila 8: 🧘 Acciones | 💬 Chat IA
Fila 9: ⚙️ Opciones | 🏠 Menú Bot
```

**Problemas Identificados:**
- ❌ Menú muy largo (9 filas)
- ❌ Opciones poco organizadas
- ❌ Falta categorización clara
- ❌ "Explorar" solo genera enemigos (poco variedad)
- ❌ Dos iconos de estrella (🌟) - confuso
- ❌ "Chat IA" y "Preguntar a IA" redundantes

---

### ⚔️ **MENÚ DE COMBATE**

**Botones Actuales:**
```
Fila 1: ⚔️ Atacar | 🛡️ Defender
Fila 2: 🧪 Usar Ítem | 🏃 Huir
Fila 3: ✨ Skills
Fila 4: 💬 Preguntar a IA
```

**Problemas Identificados:**
- ❌ Falta acceso a tácticas avanzadas
- ❌ No hay botón para mascotas (si las tienes activas)
- ❌ Skills sin categorización en combate
- ❌ Falta "Observar enemigo" en menú principal

---

## 🎯 PROPUESTA DE MEJORA

### 📝 **NUEVO MENÚ PRINCIPAL (Exploración)**

#### **Categorías Reorganizadas:**

**🎮 ACCIONES PRINCIPALES** (Fila 1-2)
```
Fila 1: ⚔️ Explorar | 🗺️ Aventura | 🐾 Mascotas
Fila 2: 😴 Descansar | 💼 Trabajar | 🧘 Meditar
```

**📊 INFORMACIÓN Y PROGRESO** (Fila 3-4)
```
Fila 3: 📊 Stats | 🎒 Inventario | 🏪 Tienda
Fila 4: 🌟 Progreso | 💎 Pasivas | 📈 Counters
```

**⚔️ HABILIDADES Y COMBATE** (Fila 5)
```
Fila 5: ✨ Skills | 🌟 Combos | 🛡️ Entrenar
```

**⚙️ UTILIDADES** (Fila 6)
```
Fila 6: 💬 Chat IA | ⚙️ Opciones | 🏠 Salir
```

**Reducido de 9 filas a 6 filas (-33%)**

---

### ⚔️ **NUEVO MENÚ DE COMBATE**

```
Fila 1: ⚔️ Atacar | 📋 Tácticas | 🛡️ Defender
Fila 2: ✨ Skills | 🐾 Mascotas | 🧪 Ítems
Fila 3: 👁️ Observar | 💬 Consulta | 🏃 Huir
```

**Mejoras:**
- ✅ Acceso a "Tácticas" (Heavy Attack, Precise, Charge, etc.)
- ✅ Botón "Mascotas" visible si tienes pets activos
- ✅ "Observar" para analizar enemigo
- ✅ Layout 3x3 más balanceado

---

### 📋 **NUEVO SUBMENÚ: TÁCTICAS DE COMBATE**

```
Cuando presionas "📋 Tácticas":

ATAQUES ESPECIALES:
Fila 1: 💥 Carga | ⚡ Rápido | 🎯 Preciso
Fila 2: 🔨 Pesado | 🌀 Mágico | 🔥 Elemental

ACCIONES DEFENSIVAS:
Fila 3: 🛡️ Bloquear | 💨 Esquivar | 🔄 Contragolpe

TÁCTICAS AVANZADAS:
Fila 4: 🦘 Saltar | 🏃 Retroceder | 🚶 Avanzar
Fila 5: 👁️ Observar | 🧘 Meditar | ⏸️ Esperar

MASCOTAS (si tienes):
Fila 6: 🐾 Domar | 🐾 Acariciar | 🎶 Calmar

Fila 7: 🔙 Volver
```

---

### 🗺️ **NUEVO SUBMENÚ: EXPLORAR EXPANDIDO**

```
Cuando presionas "⚔️ Explorar":

EXPLORACIÓN:
Fila 1: ⚔️ Buscar Combate | 🗺️ Aventura Aleatoria
Fila 2: 🏞️ Buscar Recursos | 💎 Buscar Tesoro
Fila 3: 🐾 Buscar Mascotas | 🎲 Evento Aleatorio

ZONAS (futuro):
Fila 4: 🌲 Bosque | ⛰️ Montañas | 🏜️ Desierto

Fila 5: 🔙 Volver
```

**Nuevas Mecánicas:**

1. **⚔️ Buscar Combate** (actual "Explorar")
   - Genera enemigo aleatorio según nivel
   - Cuesta 15 energía

2. **🗺️ Aventura Aleatoria** (NUEVO)
   - 40% Enemigo fácil
   - 25% Enemigo medio
   - 15% Enemigo difícil
   - 10% Cofre de tesoro (+50-200 gold)
   - 5% Comerciante viajero (descuentos)
   - 5% Evento especial (quest, puzzle)
   - Cuesta 20 energía

3. **🏞️ Buscar Recursos** (NUEVO)
   - Gather herbs (30% chance)
   - Mine ore (20% chance)
   - Find materials (25% chance)
   - Nothing (25%)
   - Cuesta 10 energía
   - Trackea: gather_herbs, mine_ore

4. **💎 Buscar Tesoro** (NUEVO)
   - 50% Nothing
   - 30% Small gold (20-50)
   - 15% Medium gold (50-100)
   - 4% Rare item
   - 1% Legendary item
   - Cuesta 25 energía
   - Trackea: treasure_hunt

5. **🐾 Buscar Mascotas** (NUEVO)
   - 60% Nothing
   - 25% Bestia común (Wolf, Cat)
   - 10% Bestia rara (Bear, Eagle)
   - 4% Bestia épica (Dragon baby)
   - 1% Bestia legendaria (????)
   - Cuesta 30 energía
   - Trackea: search_beast

6. **🎲 Evento Aleatorio** (NUEVO)
   - 30% Comerciante
   - 25% NPC con quest
   - 20% Puzzle (gift on solve)
   - 15% Random blessing (+stats temporales)
   - 10% Encuentro con otro jugador (futuro PVP)
   - Cuesta 15 energía
   - Trackea: random_event

---

## 🐾 **NUEVO SUBMENÚ: MASCOTAS**

```
Cuando presionas "🐾 Mascotas":

MIS MASCOTAS:
Fila 1: Lista de mascotas activas (máx 2)
        Ej: "🐺 Fenrir Lv.25 (HP: 180/180)"

ACCIONES:
Fila 2: 📋 Ver Todas | 🔄 Cambiar Pet | 🍖 Alimentar

COMBATE:
Fila 3: 🐾 Entrenar Pet | ⚔️ Pet Arena (PVP pets)

Fila 4: 🔙 Volver
```

---

## 📝 **VERIFICACIÓN DE CALLBACKS**

### ✅ **Callbacks Funcionales Verificados**
- `rpg_main` - Menú principal
- `rpg_stats` - Ver estadísticas
- `rpg_equipment` - Ver equipo
- `rpg_shop` - Tienda
- `rpg_skills` - Ver skills
- `rpg_combo_skills` - Ver combo skills
- `rpg_counters` - Ver contadores
- `rpg_progress` - Ver progreso clases
- `rpg_passives` - Ver pasivas
- `rpg_actions` - Ver acciones disponibles
- `rpg_explore` - Explorar (combate)
- `rpg_rest` - Descansar
- `rpg_train` - Entrenar
- `rpg_work` - Trabajar

### ❌ **Callbacks Rotos o Faltantes**
- `rpg_options` - ❌ No implementado
- `rpg_ai_chat` - ❌ No implementado en exploración
- `rpg_combat_ai` - ⚠️ Verificar implementación
- `rpg_combat_item` - ⚠️ Verificar si funciona con inventario

### 🔄 **Callbacks Redundantes**
- `rpg_ai_chat` y `rpg_combat_ai` - Consolidar en uno solo
- `rpg_equipment` y `rpg_inventory` - Clarificar diferencia

---

## 🎯 ROADMAP DE IMPLEMENTACIÓN

### **FASE 1: Limpieza y Reorganización** (Est: 2-3 horas)
- [ ] Reorganizar menú principal (6 filas)
- [ ] Consolidar callbacks de IA
- [ ] Implementar `rpg_options` básico
- [ ] Testing de callbacks existentes

### **FASE 2: Menú de Combate Mejorado** (Est: 2-3 horas)
- [ ] Nuevo layout de combate (3x3)
- [ ] Implementar submenú "Tácticas"
- [ ] Agregar botón "Mascotas" (condicional)
- [ ] Testing de combate

### **FASE 3: Exploración Expandida** (Est: 3-4 horas)
- [ ] Implementar "Aventura Aleatoria"
- [ ] Implementar "Buscar Recursos"
- [ ] Implementar "Buscar Tesoro"
- [ ] Implementar "Buscar Mascotas"
- [ ] Implementar "Evento Aleatorio"
- [ ] Testing de exploración

### **FASE 4: Submenú de Mascotas** (Est: 2 horas)
- [ ] Ver lista de pets
- [ ] Cambiar pets activos
- [ ] Alimentar pets
- [ ] Entrenar pets
- [ ] Testing de mascotas

### **FASE 5: Testing y Deploy** (Est: 1-2 horas)
- [ ] Testing completo de todos los menús
- [ ] Verificar que no hay callbacks rotos
- [ ] Deploy a Fly.io
- [ ] Testing en producción

---

## 📊 MÉTRICAS DE ÉXITO

**Antes:**
- Menú principal: 9 filas
- Menú combate: 4 filas
- Opciones de exploración: 1 (solo combate)
- Callbacks rotos: 2-3

**Después:**
- Menú principal: 6 filas (-33%)
- Menú combate: 3 filas (más opciones)
- Opciones de exploración: 6 (+500%)
- Callbacks rotos: 0

---

## 🔄 HISTORIAL DE CAMBIOS

| Fecha | Cambio | Commit |
|-------|--------|--------|
| 2026-02-13 | Documento creado | - |
| 2026-02-13 | Reestructuración completa implementada | 332e4e6 |

---

**Estado actual:** ✅ **COMPLETADO** - Listo para testing y deploy

**Próximos pasos:**  
1. Testing local completo
2. Deploy a Fly.io
3. Testing en producción
