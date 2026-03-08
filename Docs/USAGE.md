# 📖 Guía de uso - BotTelegram

> Cómo usar todos los comandos del bot de recordatorios en Telegram

---

## 📋 Tabla de contenidos
1. [Inicio rápido](#inicio-rápido)
2. [Comandos disponibles](#comandos-disponibles)
3. [Formatos de tiempo](#formatos-de-tiempo)
4. [Ejemplos prácticos](#ejemplos-prácticos)
5. [Recurrencias](#recurrencias)
6. [Preguntas frecuentes](#preguntas-frecuentes)

---

## 🚀 Inicio rápido

### Paso 1: Busca el bot en Telegram
1. Abre Telegram
2. En la búsqueda, escribe el nombre de tu bot
3. Click en el bot

### Paso 2: Inicia la conversación
Envía cualquier mensaje, o usa `/start`

Verás:
```
¡Hola! 👋 Bienvenido a BotTelegram

Soy tu asistente de recordatorios personal.
Estoy aquí para ayudarte a no olvidar nada importante.

⚡ Usa /help para ver todos mis comandos
```

### Paso 3: Lee los comandos disponibles
Envía `/help`

---

## 🎯 Comandos disponibles

### `/start` - Iniciar conversación
**Uso:** `/start`

**Respuesta:**
```
¡Hola! 👋 Bienvenido a BotTelegram
```

---

### `/help` - Ver comandos
**Uso:** `/help`

**Respuesta:**
```
📋 Comandos disponibles:

/start - Iniciar conversación
/help - Ver este mensaje
/remember - Crear recordatorio
/list - Ver mis recordatorios
/delete - Eliminar recordatorio
/edit - Editar recordatorio
/recur - Crear recordatorio recurrente
```

---

### `/remember` - Crear recordatorio
**Uso:** `/remember "texto" [en X seg/min/hora/día]`

Crea un nuevo recordatorio que te notificará en el momento indicado.

**Formato:**
```
/remember "descripción del recordatorio" en 10 minutos
/remember "descripción del recordatorio" 5 horas
/remember "descripción del recordatorio" mañana a las 14:30
/remember "descripción del recordatorio" en 3 días
```

**Variantes de tiempo:**
- **Segundos:** `seg`, `segundo`, `segundos`
- **Minutos:** `min`, `minuto`, `minutos`
- **Horas:** `hora`, `horas`, `h`
- **Días:** `día`, `días`, `d`

**Ejemplos:**
```
/remember "Llamar a mamá" en 2 horas
/remember "Comprar leche" en 30 minutos
/remember "Estudiar matemáticas" en 2 horas 30 minutos
/remember "Cumpleaños de Juan" en 45 días
/remember "Coffee break" en 5 minutos
```

**Response:**
```
✅ Recordatorio creado
ID: a1b2c3d4
Texto: Llamar a mamá
Cuando: 2025-02-10 16:30:00
```

---

### `/list` - Ver recordatorios
**Uso:** `/list`

Muestra todos tus recordatorios pendientes ordenados por fecha.

**Response ejemplo:**
```
📋 Tus recordatorios (3 pendientes):

[a1b2c3d4] 📅 2025-02-15 14:30 - Estudiar 🔄 Daily
[e5f6g7h8] 📅 2025-02-20 18:00 - Llamar director
[i9j0k1l2] 📅 2025-03-01 10:00 - Reunión importante
```

**Leyenda:**
- `[ID]` - Identificador único del recordatorio
- `📅 FECHA HORA` - Cuándo te recordará
- `🔄 TIPO` - Tipo de recurrencia (None, Daily, Weekly, etc.)

---

### `/delete` - Eliminar recordatorio
**Uso:** `/delete [ID]`

Elimina un recordatorio por su ID.

**Ejemplo:**
```
/delete a1b2c3d4
```

**Response:**
```
✅ Recordatorio a1b2c3d4 eliminado correctamente
```

---

### `/edit` - Editar recordatorio
**Uso:** `/edit [ID] [nuevo texto]`

Modifica el texto de un recordatorio existente.

**Ejemplo:**
```
/edit a1b2c3d4 Llamar a papá en lugar de mamá
```

**Response:**
```
✅ Recordatorio a1b2c3d4 actualizado
Nuevo texto: Llamar a papá en lugar de mamá
```

**Nota:** Al editar, se resetea el estado "notificado", así que volverá a notificarte.

---

### `/recur` - Configurar recurrencia
**Uso:** `/recur [ID] [tipo]`

Hace que un recordatorio se repita automáticamente.

**Tipos disponibles:**
- `none` - No recurrente (eliminará recurrencia)
- `daily` - Todos los días
- `weekly` - Una vez por semana
- `monthly` - Una vez al mes
- `yearly` - Una vez al año

**Ejemplos:**
```
/recur a1b2c3d4 daily
/recur e5f6g7h8 weekly
/recur i9j0k1l2 none
```

**Response:**
```
✅ Recordatorio a1b2c3d4 configurado como Daily
Se repetirá automáticamente cada día
```

---

## ⏰ Formatos de tiempo

### Formatos cortos (recomendado)
```
en 5 seg         → En 5 segundos
en 10 minutos    → En 10 minutos
en 2 horas       → En 2 horas
en 3 días        → En 3 días
```

### Formatos alternativos
```
en 1 segundo     → En 1 segundo
en 1 minuto      → En 1 minuto
en 1 hora        → En 1 hora
en 1 día         → En 1 día
```

### Formatos sin "en"
```
5 seg            → En 5 segundos
10 minutos       → En 10 minutos
2 horas          → En 2 horas
3 días           → En 3 días
```

### Combinaciones
```
en 2 horas 30 minutos        → En 2.5 horas
mañana a las 14:30           → Mañana a las 2:30 PM
21 de febrero a las 10:00    → 21/02 a las 10:00 AM
```

---

## 📚 Ejemplos prácticos

### Ejemplo 1: Recordatorio simple
```
Usuario: /remember "Beber agua" en 30 minutos
Bot: ✅ Recordatorio creado
     ID: a1b2c3d4
     Estado: En 30 minutos

[30 minutos después...]
Bot: 💬 RECORDATORIO ⏰
     a1b2c3d4 - Beber agua
```

### Ejemplo 2: Recordatorio recurrente
```
Usuario: /remember "Meditación" en 1 hora
Bot: ✅ ID: b2c3d4e5

Usuario: /recur b2c3d4e5 daily
Bot: ✅ Recordatorio b2c3d4e5 configurado como Daily

[Mañana a la misma hora...]
Bot: 💬 RECORDATORIO ⏰
     b2c3d4e5 - Meditación (Daily)
```

### Ejemplo 3: Gestión de múltiples recordatorios
```
Usuario: /remember "Estudiar" en 2 horas
Bot: ✅ ID: c3d4e5f6

Usuario: /remember "Hacer ejercicio" en 4 horas
Bot: ✅ ID: d4e5f6g7

Usuario: /list
Bot: 📋 Tus recordatorios (2 pendientes):
     [c3d4e5f6] 📅 14:30 - Estudiar
     [d4e5f6g7] 📅 16:30 - Hacer ejercicio

Usuario: /delete c3d4e5f6
Bot: ✅ Recordatorio c3d4e5f6 eliminado

Usuario: /list
Bot: 📋 Tus recordatorios (1 pendiente):
     [d4e5f6g7] 📅 16:30 - Hacer ejercicio
```

### Ejemplo 4: Editar recordatorio
```
Usuario: /remember "Llamar a Juan" en 1 hora
Bot: ✅ ID: e5f6g7h8

Usuario: /edit e5f6g7h8 Llamar a Carlos
Bot: ✅ Recordatorio e5f6g7h8 actualizado
     Nuevo texto: Llamar a Carlos
```

---

## 🔄 Recurrencias

### ¿Cómo funcionan?

1. Creas un recordatorio normal
2. Configuras su recurrencia con `/recur`
3. El bot te notifica en la fecha/hora indicada
4. Automáticamente genera la próxima recurrencia
5. Se repite indefinidamente

### Ejemplos por tipo

#### Daily (Diariamente)
```
Usuario: /remember "Tomar vitamina" en 08:00
Usuario: /recur ID daily

Día 1 → Bot te notifica a las 08:00
Día 2 → Bot te notifica a las 08:00
Día 3 → Bot te notifica a las 08:00
...
```

#### Weekly (Semanalmente)
```
Usuario: /remember "Día de yoga" viernes a las 18:00
Usuario: /recur ID weekly

Semana 1 → Viernes 18:00
Semana 2 → Viernes 18:00
Semana 3 → Viernes 18:00
...
```

#### Monthly (Mensualmente)
```
Usuario: /remember "Pagar renta" el 1 de mes a las 09:00
Usuario: /recur ID monthly

Mes 1 → 1 a las 09:00
Mes 2 → 1 a las 09:00
Mes 3 → 1 a las 09:00
...
```

#### Yearly (Anualmente)
```
Usuario: /remember "Cumpleaños de ana" 25 febrero a las 10:00
Usuario: /recur ID yearly

2025 → 25 feb 10:00
2026 → 25 feb 10:00
2027 → 25 feb 10:00
...
```

### Cancelar recurrencia
```
Usuario: /recur ID none
Bot: ✅ Recurrencia desactivada
     El recordatorio solo se ejecutará una vez
```

---

## ❓ Preguntas frecuentes

### P: ¿Puedo tener recordatorios para varias personas/temas?
**R:** Sí, tienes recordatorios ilimitados. Cada uno tiene su ID único.

### P: ¿Qué pasa si reinicia el bot?
**R:** No hay problema, los recordatorios se guardan en archivo y continuará notificándote.

### P: ¿Puedo editar solo la hora, no el texto?
**R:** Actualmente no, pero está en el roadmap. Ahora debes usar `/edit` con texto + hora.

### P: ¿Puedo crear recordatorios para el pasado?
**R:** No, el bot rechaza fechas en el pasado.

### P: ¿Cómo veo recordatorios ya notificados?
**R:** No aparecen en `/list`, pero si era recurrente, el bot generó automáticamente uno nuevo.

### P: ¿Puedo cambiar la hora de un recordatorio?
**R:** Usa `/edit` con la nueva información:
```
/edit a1b2c3d4 Nuevo texto a las 15:00
```

### P: ¿Qué pasa si elimino accidentalmente un recordatorio?
**R:** Se elimina permanentemente. Debes crear uno nuevo.

### P: ¿Funciona con múltiples chats?
**R:** Sí, cada chat tiene sus propios recordatorios independientes.

### P: ¿Hay límite de recordatorios?
**R:** No, puedes crear los que necesites.

### P: ¿El bot consume datos?
**R:** Muy poco, solo cuando sincroniza con Telegram (algunos KB por día).

### P: ¿Funciona 24/7?
**R:** Sí, en Azure App Service (plan B1 con Always On) el bot puede mantenerse siempre online.

---

## 💡 Consejos y trucos

### 1. ID de recordatorios
Siempre guarda los IDs de tus recordatorios importantes (copia al portapapeles).

### 2. Usar `/list` regularmente
Checa tu lista de recordatorios cada mañana para mantenerla actualizada.

### 3. Recurrencias para tareas habituales
```
/remember "Workout" en 18:00
/recur daily
```
Así nunca olvidarás tus rutinas.

### 4. Recordatorios anidados
Puedes tener múltiples recordatorios para la misma tarea en diferentes horas.

### 5. Nombres descriptivos
```
❌ /remember "hacer" en 2 horas
✅ /remember "Hacer la compra de frutas y verduras" en 2 horas
```

---

## 🔗 Recursos adicionales

- [INSTALLATION.md](./INSTALLATION.md) - Cómo instalar el bot
- [API.md](./API.md) - Integración API para desarrolladores
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Cómo funciona internamente
- [ROADMAP.md](./ROADMAP.md) - Próximas funcionalidades

---

**¿Necesitas ayuda? Abre un** [**Issue en GitHub**](https://github.com/kudawasama/BotTelegramPersonal/issues)

**Última actualización:** Febrero 2025
