# 📚 BotTelegram - Centro de documentación

Bienvenido al centro de documentación oficial de **BotTelegram**. Aquí encontrarás todo lo que necesitas saber sobre cómo usar, instalar, desarrollar e integrar el bot.

---

## 🚀 Empezar rápidamente

### Para retomar el proyecto
1. **¿Dónde quedamos?** → [ESTADO_ACTUAL.md](./ESTADO_ACTUAL.md) - Checkpoint técnico y siguientes pasos

### Para usuarios finales
1. **Nuevo en el bot?** → [USAGE.md](./USAGE.md) - Aprende todos los comandos
2. **Instalación** → [INSTALLATION.md](./INSTALLATION.md) - Cómo configurar local o en la nube
3. **Preguntas?** → [USAGE.md#preguntas-frecuentes](./USAGE.md#preguntas-frecuentes) - FAQ

### Para desarrolladores
1. **Entender la arquitectura** → [ARCHITECTURE.md](./ARCHITECTURE.md) - Cómo funciona internamente
2. **Integración API REST** → [API.md](./API.md) - Endpoints y ejemplos
3. **Quiero contribuir** → [CONTRIBUTING.md](./CONTRIBUTING.md) - Cómo aportar

### Para el futuro
1. **¿Qué viene después?** → [ROADMAP.md](./ROADMAP.md) - Plan de desarrollo a 1 año

---

## 📖 Guías completas

### 🎯 USAGE.md - Guía de usuario
**Audiencia:** Usuarios finales, no necesita conocimiento técnico

**Contenido:**
- ✅ Descripción de todos los 7 comandos
- ✅ Formatos de tiempo soportados
- ✅ Ejemplos prácticos step-by-step
- ✅ Cómo funcionan las recurrencias (Daily, Weekly, etc.)
- ✅ Preguntas frecuentes (15+)
- ✅ Consejos y trucos

**Cuándo leer:**
- Acabas de instalar el bot y quieres aprender qué puede hacer
- Tienes una pregunta específica sobre un comando
- Quieres optimizar cómo usas el bot

---

### 🔧 INSTALLATION.md - Guía de instalación
**Audiencia:** Usuarios técnicos, devops, administradores

**Contenido:**
- ✅ Instalación local (Windows/macOS/Linux)
- ✅ Instalación con Docker
- ✅ Deploy en Azure App Service (Basic B1)
- ✅ Verificación de instalación
- ✅ Troubleshooting común

**Cuándo leer:**
- Quieres correr el bot tú mismo (local, Docker o Azure)
- Necesitas hacer deploy en tu infraestructura
- Tienes problemas durante la instalación

---

### 🏗️ ARCHITECTURE.md - Documentación técnica
**Audiencia:** Desarrolladores del proyecto, contribuyentes

**Contenido:**
- ✅ Descripción general de la arquitectura en capas
- ✅ Flujos principales (crear, listar, eliminar, notificar)
- ✅ Componentes clave con código
- ✅ Diagrama de flujo completo
- ✅ Datos y persistencia
- ✅ Manejo de errores
- ✅ Patrones de diseño (Command, Singleton, Observer)
- ✅ Medidas de seguridad

**Cuándo leer:**
- Quieres entender cómo funciona el código
- Vas a añadir una nueva feature o fix
- Necesitas debugear algo complejo

---

### 🔌 API.md - Documentación de API REST
**Audiencia:** Desarrolladores que quieren integrar el bot

**Contenido:**
- ✅ Base URL (local y Azure)
- ✅ 5 endpoints completos (GET all, GET one, POST, PUT, DELETE)
- ✅ Formatos de request/response
- ✅ Parámetros y validaciones
- ✅ Códigos HTTP
- ✅ Ejemplos cURL
- ✅ Ejemplos en JavaScript/Python/C#
- ✅ Recomendaciones de seguridad

**Cuándo leer:**
- Quieres integrar recordatorios en otra aplicación
- Necesitas automatizar crear/editar recordatorios
- Quieres hacer queries directas a la API

---

### 🤝 CONTRIBUTING.md - Guía de contribución
**Audiencia:** Desarrolladores que quieren contribuir al proyecto

**Contenido:**
- ✅ Código de conducta
- ✅ Cómo reportar bugs efectivamente
- ✅ Cómo sugerir mejoras
- ✅ Proceso de desarrollo
- ✅ Estándares de código C#
- ✅ Proceso de Pull Request
- ✅ Setup de ambiente de desarrollo
- ✅ Tareas abiertas para contribuyentes

**Cuándo leer:**
- Quieres contribuir código al proyecto
- Encontraste un bug y quieres reportarlo
- Tienes una idea para mejorar el bot

---

### 🗺️ ROADMAP.md - Plan de desarrollo
**Audiencia:** Product managers, planificadores, interesados de largo plazo

**Contenido:**
- ✅ Visión general del proyecto
- ✅ 5 fases de desarrollo (Febrero-Junio 2026)
- ✅ Estimaciones de tiempo (390-490 horas totales)
- ✅ 25+ funcionalidades planificadas
- ✅ Prioridades (Critical, High, Medium, Low)
- ✅ Áreas de aprendizaje requerido
- ✅ Necesidades de contribuyentes

**Cuándo leer:**
- Quieres saber qué funciones vendrán
- Necesitas evitar construir algo que ya está planeado
- Quieres entender la dirección del proyecto

---

### 📍 ESTADO_ACTUAL.md - Checkpoint operativo
**Audiencia:** Mantenedores, devops y desarrollo de continuidad

**Contenido:**
- ✅ Estado actual real de infraestructura y documentación
- ✅ Lista de tareas pendientes inmediatas
- ✅ Cómo retomar el trabajo en minutos

**Cuándo leer:**
- Retomas el proyecto después de una pausa
- Necesitas saber el próximo paso sin releer todo

---

## 🎯 Matriz de navegación por rol

| Rol | Inicio | Después | Profundizar |
|-----|--------|---------|-------------|
| **Usuario** | [USAGE.md](./USAGE.md) | [INSTALLATION.md](./INSTALLATION.md) | [FAQ](./USAGE.md#preguntas-frecuentes) |
| **Devops/SysAdmin** | [INSTALLATION.md](./INSTALLATION.md) | [ARCHITECTURE.md](./ARCHITECTURE.md) | [ROADMAP.md](./ROADMAP.md) |
| **Desarrollador (Integración)** | [API.md](./API.md) | [ARCHITECTURE.md](./ARCHITECTURE.md) | [CONTRIBUTING.md](./CONTRIBUTING.md) |
| **Desarrollador (Core)** | [ARCHITECTURE.md](./ARCHITECTURE.md) | [CONTRIBUTING.md](./CONTRIBUTING.md) | [ROADMAP.md](./ROADMAP.md) |
| **Contribuyente** | [CONTRIBUTING.md](./CONTRIBUTING.md) | [ARCHITECTURE.md](./ARCHITECTURE.md) | [ROADMAP.md](./ROADMAP.md) |
| **Product Manager** | [ROADMAP.md](./ROADMAP.md) | [USAGE.md](./USAGE.md) | [ARCHITECTURE.md](./ARCHITECTURE.md) |

---

## 🔍 Búsqueda de temas específicos

### ¿Cómo hago...?
- **...crear un recordatorio?** → [USAGE.md - /remember](./USAGE.md#remember---crear-recordatorio)
- **...hacer que se repita?** → [USAGE.md - /recur](./USAGE.md#recur---configurar-recurrencia)
- **...instalar en Azure?** → [INSTALLATION.md - Deploy Azure](./INSTALLATION.md#deploy-en-azure-app-service-plan-basico-b1)
- **...obtener recordatorios vía API?** → [API.md - GET /api/reminders](./API.md#1️⃣-obtener-todos-los-recordatorios)
- **...arreglarlo si no funciona?** → [INSTALLATION.md - Troubleshooting](./INSTALLATION.md#🛠️-troubleshooting)
- **...contribuir código?** → [CONTRIBUTING.md - Guía de desarrollo](./CONTRIBUTING.md#-guía-de-desarrollo)
- **...entender el código?** → [ARCHITECTURE.md - Componentes clave](./ARCHITECTURE.md#-componentes-clave)

### Tengo una pregunta sobre...
- **Comandos y funcionalidades** → [USAGE.md](./USAGE.md)
- **Instalación y configuración** → [INSTALLATION.md](./INSTALLATION.md)
- **Integración técnica** → [API.md](./API.md)
- **Código y desarrollo** → [ARCHITECTURE.md](./ARCHITECTURE.md)
- **Bugs o mejoras** → [CONTRIBUTING.md](./CONTRIBUTING.md)
- **Futuro del proyecto** → [ROADMAP.md](./ROADMAP.md)

---

## 📊 Estadísticas de documentación

| Documento | Palabras | Secciones | Ejemplos | Diagramas |
|-----------|----------|-----------|----------|-----------|
| USAGE.md | 3,500+ | 8 | 20+ | 3 |
| INSTALLATION.md | 2,800+ | 7 | 15+ | 0 |
| ARCHITECTURE.md | 4,200+ | 9 | 30+ | 5 |
| API.md | 3,100+ | 8 | 25+ | 2 |
| CONTRIBUTING.md | 3,400+ | 9 | 18+ | 2 |
| ROADMAP.md | 3,000+ | 6 | 5+ | 3 |
| **Total** | **20,000+** | **47** | **113+** | **15** |

---

## 🎓 Tutoriales step-by-step

### Tutorial 1: Primer recordatorio (5 minutos)
1. Abre Telegram
2. Busca tu bot
3. Envía: `/start`
4. Espera respuesta: "¡Hola! 👋 Bienvenido..."
5. Envía: `/remember "Beber agua" en 5 minutos`
6. Espera: Deberías recibir `✅ Recordatorio creado`
7. En 5 minutos: Recibirás `💬 RECORDATORIO ⏰ - Beber agua`

**Documentación:** [USAGE.md - Ejemplo 1](./USAGE.md#ejemplo-1-recordatorio-simple)

### Tutorial 2: Recurrencia diaria (10 minutos)
1. Crea recordatorio: `/remember "Meditación" en 08:00`
2. Nota el ID que recibes (ej: `a1b2c3d4`)
3. Configura recurrencia: `/recur a1b2c3d4 daily`
4. Verifica: `/list` debe mostrar `🔄 Daily`
5. Al día siguiente a las 08:00: ¡Recibirás recordatorio automáticamente!

**Documentación:** [USAGE.md - Ejemplo 2](./USAGE.md#ejemplo-2-recordatorio-recurrente)

### Tutorial 3: Integración API (15 minutos)
1. El bot está corriendo en `http://localhost:5000`
2. Abre terminal/PowerShell
3. Crea recordatorio:
   ```bash
   curl -X POST http://localhost:5000/api/reminders \
     -H "Content-Type: application/json" \
     -d '{"chatId":123456789,"text":"Tarea API","dueAt":"2025-02-20T15:00:00"}'
   ```
4. Obtén lista: `curl http://localhost:5000/api/reminders`
5. Verifica en Telegram que aparece el recordatorio

**Documentación:** [API.md - Ejemplo cURL](./API.md#-ejemplos-curl)

---

## 🆘 Support & Ayuda

### Recursos
- 📖 **Documentación:** Este sitio
- 🐛 **Issues/Bugs:** [GitHub Issues](https://github.com/kudawasama/BotTelegramPersonal/issues)
- 💬 **Preguntas:** [GitHub Discussions](https://github.com/kudawasama/BotTelegramPersonal/discussions)
- 📧 **Email:** [contact@example.com] (si aplica)

### Antes de pedir help
1. ✅ Lee la sección relevante en USAGE.md
2. ✅ Chequea FAQ en [USAGE.md#preguntas-frecuentes](./USAGE.md#preguntas-frecuentes)
3. ✅ Busca en Issues existentes
4. ✅ Lee Troubleshooting en [INSTALLATION.md#troubleshooting](./INSTALLATION.md#troubleshooting)

---

## 📝 Historial de documentación

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 1.0 | Feb 2025 | Documentación inicial completa (6 archivos) |
| Próxima | Mar 2025 | Video tutoriales, ejemplos avanzados |

---

## 🎯 Próximos pasos recomendados

```
Eres usuario final?
    → Lee: USAGE.md
    → Luego: Empieza a usar el bot

Eres DevOps/SysAdmin?
    → Lee: INSTALLATION.md
    → Luego: Deploy y monitorea

Eres desarrollador (integración)?
    → Lee: API.md
    → Luego: Integra con tu app

Eres desarrollador (core)?
    → Lee: ARCHITECTURE.md
    → Luego: CONTRIBUTING.md
    → Finalmente: Abre un PR

Eres contribuyente?
    → Lee: CONTRIBUTING.md
    → Luego: Ayuda en Issues or PRs
    → Finalmente: Aparece aquí 👏
```

---

## 📞 Cómo citar esta documentación

**APA:**
```
BotTelegram Contributors. (2025). BotTelegram Documentation. 
Retrieved from https://github.com/kudawasama/BotTelegramPersonal/tree/main/Docs
```

**Markdown:**
```markdown
[BotTelegram Documentation](./README.md)
```

---

## ✨ Agradecimientos
Gracias a todos los que han contribuido a esta documentación. 🙌

---

**Última actualización:** Febrero 2025  
**Versión de documentación:** 1.0  
**Compatible con:** BotTelegram v1.0 (.NET 9.0)

---

**¿Sugerencias para mejorar la documentación?** [Abre un Issue](https://github.com/kudawasama/BotTelegramPersonal/issues/new)
