# 🔌 API REST - Documentación Completa

> Referencia técnica para todos los endpoints REST de BotTelegram

---

## 📋 Tabla de contenidos

---

1. [Base URL](#base-url)
2. [Autenticación](#autenticación)
3. [Formatos](#formatos)
4. [Endpoints](#endpoints)
5. [Códigos de estado](#códigos-de-estado)
6. [Ejemplos cURL](#ejemplos-curl)
7. [Integración](#integración)

---

## 🌐 Base URL

**Local:**

```link
http://localhost:5000
```

**Azure App Service (ejemplo):**

```link
https://bottelegram-rpg.azurewebsites.net
```

---

## 🔐 Autenticación

Actualmente **no hay autenticación requerida** ⚠️

✅ **Recomendación futura (Fase 2):**
- Implementar JWT Bearer token
- Endpoint `/api/auth/login`

---

## 📝 Formatos

### Request
```json
Content-Type: application/json

{
  "chatId": 123456789,
  "text": "Recordatorio importante",
  "dueAt": "2025-02-15T14:30:00"
}
```

### Response exitosa (2xx)
```json
{
  "id": "a1b2c3d4",
  "chatId": 123456789,
  "text": "Recordatorio importante",
  "dueAt": "2025-02-15T14:30:00",
  "notified": false,
  "recurrenceType": "None",
  "createdAt": "2025-02-10T10:00:00"
}
```

### Response error (4xx/5xx)
```json
{
  "error": "Descripción del error",
  "statusCode": 400
}
```

---

## 🔌 Endpoints

### 1️⃣ Obtener todos los recordatorios

**Endpoint:**
```
GET /api/reminders
```

**Descripción:** Obtiene la lista completa de recordatorios

**Parámetros:** Ninguno

**Response ejemplo:**
```json
[
  {
    "id": "a1b2c3d4",
    "chatId": 123456789,
    "text": "Estudiar para examen",
    "dueAt": "2025-02-20T18:00:00",
    "notified": false,
    "recurrenceType": "Daily",
    "createdAt": "2025-02-10T10:00:00"
  },
  {
    "id": "e5f6g7h8",
    "chatId": 123456789,
    "text": "Comprar leche",
    "dueAt": "2025-02-15T09:30:00",
    "notified": true,
    "recurrenceType": "None",
    "createdAt": "2025-02-14T15:22:00"
  }
]
```

**Status codes:**
- `200 OK` - Éxito
- `500 Internal Server Error` - Error del servidor

---

### 2️⃣ Obtener un recordatorio específico

**Endpoint:**
```
GET /api/reminders/{id}
```

**Parámetros:**
- `id` (path): ID del recordatorio (string, 8 caracteres)

**Response ejemplo:**
```json
{
  "id": "a1b2c3d4",
  "chatId": 123456789,
  "text": "Llamar a mamá",
  "dueAt": "2025-02-16T20:00:00",
  "notified": false,
  "recurrenceType": "Weekly",
  "createdAt": "2025-02-10T14:30:00"
}
```

**Status codes:**
- `200 OK` - Encontrado
- `404 Not Found` - ID no existe
- `500 Internal Server Error` - Error del servidor

---

### 3️⃣ Crear nuevo recordatorio

**Endpoint:**
```
POST /api/reminders
```

**Body requerido:**
```json
{
  "chatId": 123456789,
  "text": "Mi nuevo recordatorio",
  "dueAt": "2025-02-20T15:00:00"
}
```

**Campos:**
- `chatId` (number, requerido): ID del chat de Telegram
- `text` (string, requerido): Contenido del recordatorio (max 500 caracteres)
- `dueAt` (string ISO8601, requerido): Fecha y hora (formato: `YYYY-MM-DDTHH:mm:ss`)

**Response:**
```json
{
  "id": "n9o0p1q2",
  "chatId": 123456789,
  "text": "Mi nuevo recordatorio",
  "dueAt": "2025-02-20T15:00:00",
  "notified": false,
  "recurrenceType": "None",
  "createdAt": "2025-02-10T16:45:33"
}
```

**Status codes:**
- `201 Created` - Creado exitosamente
- `400 Bad Request` - Formato inválido
- `500 Internal Server Error` - Error del servidor

**Validaciones:**
- `dueAt` no puede ser en el pasado
- `chatId` debe ser > 0
- `text` no puede estar vacío

---

### 4️⃣ Actualizar recordatorio

**Endpoint:**
```
PUT /api/reminders/{id}
```

**Parámetros:**
- `id` (path): ID del recordatorio a actualizar

**Body requerido:**
```json
{
  "chatId": 123456789,
  "text": "Texto actualizado",
  "dueAt": "2025-02-25T10:00:00"
}
```

**Response:**
```json
{
  "id": "a1b2c3d4",
  "chatId": 123456789,
  "text": "Texto actualizado",
  "dueAt": "2025-02-25T10:00:00",
  "notified": false,
  "recurrenceType": "Weekly",
  "createdAt": "2025-02-10T10:00:00"
}
```

**Status codes:**
- `200 OK` - Actualizado exitosamente
- `404 Not Found` - ID no existe
- `400 Bad Request` - Validation error
- `500 Internal Server Error` - Error del servidor

**Nota:** Al actualizar, se resetea `notified` a `false` para re-enviar el recordatorio

---

### 5️⃣ Eliminar recordatorio

**Endpoint:**
```
DELETE /api/reminders/{id}
```

**Parámetros:**
- `id` (path): ID del recordatorio a eliminar

**Response exitosa:**
```json
{
  "message": "Recordatorio eliminado correctamente"
}
```

**Status codes:**
- `200 OK` - Eliminado
- `404 Not Found` - ID no existe
- `500 Internal Server Error` - Error del servidor

---

## 🎯 Códigos de estado HTTP

| Código | Significado | Causa común |
|--------|-------------|-----------|
| `200` | OK | Operación exitosa |
| `201` | Created | Recursos creado |
| `400` | Bad Request | Formato JSON inválido o validación fallida |
| `404` | Not Found | Recordatorio no existe |
| `500` | Internal Server Error | Error en el servidor |

---

## 🛠️ Ejemplos cURL

### Obtener todos los recordatorios
```bash
curl -X GET http://localhost:5000/api/reminders
```

### Obtener un recordatorio específico
```bash
curl -X GET http://localhost:5000/api/reminders/a1b2c3d4
```

### Crear un nuevo recordatorio
```bash
curl -X POST http://localhost:5000/api/reminders \
  -H "Content-Type: application/json" \
  -d '{
    "chatId": 123456789,
    "text": "Mi primer recordatorio API",
    "dueAt": "2025-02-20T15:00:00"
  }'
```

### Actualizar un recordatorio
```bash
curl -X PUT http://localhost:5000/api/reminders/a1b2c3d4 \
  -H "Content-Type: application/json" \
  -d '{
    "chatId": 123456789,
    "text": "Recordatorio actualizado",
    "dueAt": "2025-02-25T10:00:00"
  }'
```

### Eliminar un recordatorio
```bash
curl -X DELETE http://localhost:5000/api/reminders/a1b2c3d4
```

---

## 🔗 Integración

### JavaScript/Node.js (Fetch)
```javascript
// Obtener recordatorios
const reminders = await fetch('http://localhost:5000/api/reminders')
  .then(r => r.json());

// Crear recordatorio
const newReminder = await fetch('http://localhost:5000/api/reminders', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    chatId: 123456789,
    text: 'Nueva tarea',
    dueAt: '2025-02-20T15:00:00'
  })
}).then(r => r.json());
```

### Python (Requests)
```python
import requests

# Crear recordatorio
response = requests.post('http://localhost:5000/api/reminders', json={
    'chatId': 123456789,
    'text': 'Nueva tarea Python',
    'dueAt': '2025-02-20T15:00:00'
})
print(response.json())
```

### C# (HttpClient)
```csharp
var client = new HttpClient();

// Obtener todo
var response = await client.GetAsync("http://localhost:5000/api/reminders");
var json = await response.Content.ReadAsStringAsync();
```

### Postman
1. Importa esta colección: [API.postman_collection.json](./API.postman_collection.json)
2. Configura la variable `base_url`
3. Ejecuta las requests

---

## 📊 Ejemplo: Crear y listar recordatorios

```bash
# 1. Crear recordatorio
RESPONSE=$(curl -s -X POST http://localhost:5000/api/reminders \
  -H "Content-Type: application/json" \
  -d '{
    "chatId": 123456789,
    "text": "Comprar café",
    "dueAt": "2025-02-15T09:00:00"
  }')

echo "Creado: $RESPONSE"

# 2. Listar todos
curl -s -X GET http://localhost:5000/api/reminders | jq '.'

# 3. Obtener el último
ID=$(echo $RESPONSE | jq -r '.id')
curl -s -X GET http://localhost:5000/api/reminders/$ID | jq '.'
```

---

## 🔐 Seguridad

### Recomendaciones actuales:
✅ API solo accesible localmente (no expuesta públicamente)  
✅ No requiere Auth en desarrollo  

### Mejoras planificadas (Fase 2):
- [ ] JWT Bearer token
- [ ] Rate limiting
- [ ] CORS restrictivo
- [ ] HTTPS en producción

---

## 📞 Soporte

¿Problemas con la API?
- Verifica que el bot está corriendo: `dotnet run`
- Verifica el log de errores
- Abre un [Issue en GitHub](https://github.com/kudawasama/BotTelegramPersonal/issues)

---

**Última actualización:** Febrero 2025  
**API Version:** 1.0
