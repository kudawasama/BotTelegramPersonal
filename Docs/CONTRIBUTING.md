# 🤝 Guía de contribución

> Cómo contribuir a BotTelegram y ayudar a mejorarlo

---

## 📋 Tabla de contenidos
1. [Código de conducta](#código-de-conducta)
2. [Cómo contribuir](#cómo-contribuir)
3. [Reportar bugs](#reportar-bugs)
4. [Sugerir mejoras](#sugerir-mejoras)
5. [Guía de desarrollo](#guía-de-desarrollo)
6. [Estándares de código](#estándares-de-código)
7. [Proceso de Pull Request](#proceso-de-pull-request)
8. [Ambiente de desarrollo](#ambiente-de-desarrollo)

---

## 📜 Código de conducta

### Nuestro compromiso
Nos comprometemos a crear un ambiente acogedor, diverso e inclusivo para todos los colaboradores, independientemente de edad, tamaño corporal, capacidad, etnia, identidad de género, nivel de experiencia, nacionalidad, apariencia personal, raza, religión o identidad/orientación sexual.

### Comportamiento esperado
- Usa lenguaje acogedor e inclusivo
- Sé respetuoso con las opiniones diferentes
- Acepta críticas constructivas
- Enfócate en lo que es mejor para la comunidad
- Muestra empatía hacia otros miembros

### Comportamiento inaceptable
- Uso de lenguaje o imágenes sexuales
- Ataques personales o políticos
- Acoso público o privado
- Compartir información privada de otros
- Otra conducta inapropiada

---

## 🚀 Cómo contribuir

### Formas de contribuir

#### 1. **Reportar Bugs** 🐛
- Abre un Issue en GitHub
- Describe el problema claramente
- Incluye pasos para reproducir
- Adjunta logs si es posible

#### 2. **Sugerir Mejoras** 💡
- Discute grandes cambios primero (abre Issue)
- Explica el caso de uso
- Considera el impacto en otros usuarios

#### 3. **Escribir Código** 💻
- Fork el repositorio
- Crea una rama (`git checkout -b feature/mi-feature`)
- Haz commits descriptivos
- Push a tu fork
- Abre un Pull Request

#### 4. **Mejorar Documentación** 📚
- Actualiza README.md si es necesario
- Añade comentarios al código
- Crea guías útiles
- Reporta errores en documentación

#### 5. **Escribir Tests** ✅
- Añade unit tests para nuevas funciones
- Asegúrate de que tests pasen
- Mejora la cobertura de tests

---

## 🐛 Reportar Bugs

### Antes de reportar
- Verifica que el bug aún existe (última versión)
- Busca issues similares (puede estar ya reportado)
- Chequea las FAQs

### Cómo reportar
1. **Título claro:** "El bot no pasa mensajes a las 3 AM"
2. **Descripción detallada:**
   ```
   **Descripción del bug:**
   El bot no envía recordatorios después de las 3 AM.
   
   **Pasos para reproducir:**
   1. Crear un recordatorio para las 3:30 AM
   2. Esperar a esa hora
   3. Bot no envía notificación
   
   **Comportamiento esperado:**
   El bot debe enviar la notificación a las 3:30 AM
   
   **Comportamiento actual:**
   No envía notificación
   
   **Información del sistema:**
   - Sistema: Windows 10
   - .NET: 9.0
   - Hosting: Azure App Service (B1)
   
   **Logs/Screenshots:**
   [Adjunta logs si es posible]
   
   **Información adicional:**
   Solo sucede en horario madrugada (3-5 AM)
   ```

### Label checklist
- `bug` ← Etiquet como bug
- `critical` ← Si es grave
- `needs-investigation` ← Si necesitas más datos

---

## 💡 Sugerir mejoras

### Antes de sugerir
- Verifica que la funcionalidad no exista
- Busca propuestas similares
- Considera si encaja en el roadmap

### Cómo sugerir
1. **Título claro:** "Añadir notificaciones por email"
2. **Descripción:**
   ```
   **Descripción:**
   Sería útil poder recibir recordatorios también por email, no solo por Telegram.
   
   **Caso de uso:**
   Algunos usuarios prefieren email para registros, otros para integración con
   Google Calendar.
   
   **Solución propuesta:**
   - Añadir campo `emailTo` en Reminder model
   - Crear ReminderEmailService
   - Endpoint /api/reminders/{id}/send-email
   - Usar SendGrid o Mailgun para enviar
   
   **Alternativas consideradas:**
   - Usar SMTP nativo (más lento)
   - Servicio en la nube (más caro)
   
   **Contexto adicional:**
   Este feature está en ROADMAP Fase 3, así que es oportuno.
   ```

### Label checklist
- `enhancement` ← Mejora de funcionalidad
- `feature` ← Nueva funcionalidad
- `phase-1`, `phase-2`, etc. ← Fase en roadmap

---

## 💻 Guía de desarrollo

### Setup local

1. **Clone el repo**
   ```bash
   git clone https://github.com/kudawasama/BotTelegramPersonal.git
   cd BotTelegram
   ```

2. **Instala dependencias**
   ```bash
   cd src/BotTelegram
   dotnet restore
   ```

3. **Configura token**
   ```bash
   # Create appsettings.json
   {
     "Telegram": {
       "Token": "tu_token_test"
     }
   }
   ```

4. **Ejecuta el proyecto**
   ```bash
   dotnet run
   ```

5. **Tests (cuando existan)**
   ```bash
   dotnet test
   ```

### Estructura del proyecto

```
BotTelegram/
├── src/BotTelegram/
│   ├── Program.cs                 ← Punto de entrada
│   ├── appsettings.json           ← Configuración
│   ├── API/
│   │   └── RemindersController.cs ← Endpoints REST
│   ├── Commands/                  ← Lógica de comandos
│   │   ├── StartCommand.cs
│   │   ├── RememberCommand.cs
│   │   └── ...
│   ├── Core/
│   │   └── CommandRouter.cs       ← Enrutador
│   ├── Data/
│   │   └── MemoryStore.cs         ← (Futuro)
│   ├── Handlers/                  ← Manejadores Telegram
│   │   ├── CommandHandler.cs
│   │   └── MessageHandler.cs
│   ├── Models/
│   │   └── Reminder.cs            ← Modelo principal
│   └── Services/                  ← Lógica de negocio
│       ├── ReminderService.cs     ← CRUD
│       ├── ReminderScheduler.cs   ← Background
│       └── BotService.cs
├── Tests/                         ← (Futuro) Tests
├── Docs/
│   ├── README.md
│   ├── INSTALLATION.md
│   ├── API.md
│   ├── ARCHITECTURE.md
│   ├── USAGE.md
│   ├── ROADMAP.md
│   └── CONTRIBUTING.md
└── BotTelegram.sln
```

### Desarrollo de una nueva feature

**Ejemplo: Soporte para múltiples idiomas**

1. **Abre Issue:** "Soporte para idiomas multilingües"

2. **Crea rama:**
   ```bash
   git checkout -b feature/multi-language
   ```

3. **Diseña la solución:**
   - Crea `Localization/strings.en.json`
   - Crea `Localization/strings.es.json`
   - Crea `Services/LocalizationService.cs`
   - Modifica cada Command para usar service

4. **Escribe tests:** (Fase 2)
   ```bash
   # Tests de LocalizationService
   ```

5. **Commit con mensaje claro:**
   ```bash
   git commit -m "feat: Add multi-language support with localization service"
   ```

6. **Push a tu fork:**
   ```bash
   git push origin feature/multi-language
   ```

7. **Abre Pull Request:** Enlaza el Issue

---

## 📝 Estándares de código

### Convenciones C#

**Nombres:**
```csharp
// ✅ Correcto
public class RemindersController { }
private string _reminderText;
private const string MaxLength = "500";
public void SendReminder() { }

// ❌ Incorrecto
public class ReminderscontROLLER { }
private string reminder_text;
private const string MAX_LENGTH = "500";
public void SENDREMINDER() { }
```

**Indentación:** 4 espacios (estándar C#)
```csharp
public void Method()
{
    if (condition)
    {
        Code();
    }
}
```

**Comentarios:**
```csharp
// ✅ Correcto
// Obtiene todos los recordatorios del usuario
public List<Reminder> GetAll()

// ❌ Incorrecto
// Get all reminders
// from the database
// for the user
```

**Async/await:**
```csharp
// ✅ Correcto
public async Task Execute()
{
    await _bot.SendMessage(chatId, text);
}

// ❌ Incorrecto
public Task Execute()
{
    _bot.SendMessage(chatId, text);  // No awaited
}
```

**Error handling:**
```csharp
// ✅ Correcto
try
{
    await RemindersService.Save(reminder);
}
catch (IOException ex)
{
    Console.WriteLine($"Error saving reminder: {ex.Message}");
    throw;
}

// ❌ Incorrecto
try
{
    await RemindersService.Save(reminder);
}
catch (Exception) { }  // Silent fail
```

---

## 🔄 Proceso de Pull Request

### 1. Antes de hacer PR

- ✅ Branch actualizado con `main`
- ✅ Código sigue estándares
- ✅ Sin console.log o debug code
- ✅ Documentación actualizada
- ✅ Tests pasan (si existen)

### 2. Crear Pull Request

```markdown
## Descripción
Brevemente qué hace tu PR:
- Añade soporte para recordatorios SMS
- Nuevo endpoint: POST /api/reminders/{id}/send-sms
- Integración con Twilio

## Tipo de cambio
- [x] Nueva funcionalidad
- [ ] Bug fix
- [ ] Breaking change
- [ ] Documentation update

## Cambios
- Crea `Services/SmsService.cs`
- Modifica `RemindersController.cs` (+15 líneas)
- Actualiza `ROADMAP.md`

## Testing
Como se testeo esto:
1. Crear recordatorio
2. Llamar `/api/reminders/{id}/send-sms`
3. Recibir SMS en teléfono

## Screenshots/Logs
[Adjunta si es relevante]

## Checklist
- [x] Código sigue estándares del proyecto
- [x] He actualizado la documentación
- [x] Sin breaking changes
- [x] Tests pasan
```

### 3. Review y feedback

- Mantén la conversación productiva
- Responde a comentarios del reviewer
- Haz cambios solicitados
- Re-request review cuando hayas actualizado

### 4. Merge

Una vez aprobado:
- Maintainer hace merge a `main`
- Tu rama se elimina
- Tu contribution aparece en README.md

---

## 🛠️ Ambiente de desarrollo

### Herramientas recomendadas

- **IDE:** Visual Studio 2022 o Visual Studio Code
- **Git:** Última versión
- **.NET:** 9.0 SDK
- **Terminal:** PowerShell (Windows) o Bash (macOS/Linux)

### VS Code Extensions
```
ms-dotnettools.csharp         - C# support
ms-dotnettools.vscode-dotnet  - .NET CLI tools
GitHub.Copilot                - AI coding assistance
GitLens.gitlens               - Git visualization
```

### Debug

**En VS Code:**
```
1. Press F5 (Start Debugging)
2. Selecciona .NET
3. Ejecuta Program.cs
4. Breakpoints: Click en line number
```

**En Visual Studio:**
```
1. Right-click project → Properties
2. Debug → Startup project
3. F5 para debugear
4. Watch variables: Debug → Windows → Variables
```

### Useful Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Tests (cuando existan)
dotnet test

# Clean build
dotnet clean && dotnet build

# Format code
dotnet format

# Publish
dotnet publish -c Release
```

---

## 🎯 Roadmap para contribuyentes

### Tareas abiertas (Low hanging fruit)

**Perfectas para primeras contribuciones:**
- [ ] Mejorar error messages en español
- [ ] Añadir más ejemplos en USAGE.md
- [ ] Crear código de conducta (ISSUE #1)
- [ ] Documentar API en Swagger (Fase 2)

**Intermedias:**
- [ ] Agregar command `/remind-recurring` shortcut
- [ ] Validación mejorada de fechas
- [ ] Logs estructurados con Serilog

**Avanzadas:**
- [ ] Migración SQLite
- [ ] Sistema de plugins
- [ ] Dashboard web

---

## 📞 Contacto

### Community
- 💬 **Discussions:** Usa la sección Discussions de GitHub
- 🐛 **Issues:** Para bugs y features
- 📧 **Email:** [contacto email si aplica]
- 🤖 **Telegram:** [Bot link si aplica]

---

## 📜 Licencia

Al contribuir, aceptas que tus cambios serán licenciados bajo la misma licencia del proyecto (MIT).

---

## ✨ Agradecimientos

Gracias a todos los que contribuyen a mejorar BotTelegram! 🙌

### Contribuidores actuales
- [Tu nombre](https://github.com/tuusuario) - Creador principal

### Cómo aparecer aquí
1. Contribuye (PR merged)
2. Aparecerás automáticamente en GitHub Contributors

---

**Última actualización:** Febrero 2025  
**Versión:** 1.0

---

## 🎉 ¡Gracias por considerar contribuir!

Tus aportes hacen la comunidad más fuerte. 💪
