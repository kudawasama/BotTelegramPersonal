# 🗺️ Roadmap - Planes futuros de BotTelegram

> Plan de desarrollo a corto, medio y largo plazo para mejorar y expandir BotTelegram.

---

## 📊 Estado actual (v3.2.6 — Feb 2026)

✅ **Completado (Bot base):**
- Bot de Telegram con comandos básicos
- Sistema de recordatorios con parsing natural
- Persistencia en JSON
- API REST completa
- Deploy en Azure App Service (Basic B1)
- Recordatorios recurrentes
- ⭐ **Botones inline interactivos** (InlineKeyboardMarkup)
- ⭐ **FAQ/Manual completo** integrado en el bot

✅ **Completado (RPG — 12 fases + extras):**
- ⭐ **Fases 0-7**: Combate táctico, mapas, mazmorras, mascotas, clases, UI/UX, FSM, inventario
- ⭐ **Fase 7.5**: Bonos de clase activa + Tienda completa (consumibles, equipos, vender)
- ⭐ **Fase 8**: Sistema de Crafteo — 9 recetas Tier 1-3, `/herreria`
- ⭐ **Fase 9**: Sistema de Misiones/Quests — 8 misiones Kill/Collect/Craft/Explore, `/misiones`
- ⭐ **Fase 10**: Sistema de Gremio — crear/unirse, banco gremio, oficiales, ranking, `/gremio`
- ⭐ **Fase 11**: Arena PvP — combate simulado, ELO rating, tiers, retos, apuestas, `/arena`
- ⭐ **Fase 12**: Mundo Abierto Expandido — 100% COMPLETADA
  - v3.2.0: Sistema de Facciones (10 facciones, 7 tiers reputación)
  - v3.2.1: NPCs + Diálogos (12 NPCs interactivos)
  - v3.2.2: Quests vinculadas + Zonas desbloqueables
  - v3.2.3: Comercio NPC + Entrenamiento de atributos
  - v3.2.4: Consolidación de callbacks (Items en combate)
  - v3.2.5: Sistema de mascotas (liberar/vender)
  - v3.2.6: **Acciones expandidas** (12 nuevas acciones: pesca, meditación, investigación, entrenamientos mentales/físicos, estudio, comercio, diplomacia, taberna, aventuras riesgosas/sigilosas/sociales)
- 🔧 Patches y mejoras continuas (v3.1.5-3.2.6)

🔜 **Siguiente (RPG):**
- Fase 13: Eventos Temporales
- Fase 14: Generación de Imágenes (opcional)
- Fase 15: Telegram Mini App
- Fase 16: IA Narrativa (Dungeon Master)
- Fase 17: Más clases desbloqueables
- Fase 18: Sistema de Profesiones

---

## 🎯 Roadmap por fases

### **FASE 1: Base sólida (Próximas 2-4 semanas)**

#### 1.1 Mejoras de base de datos
- [ ] Migrar de JSON a **SQLite3**
  - Mejor performance
  - Queries más potentes
  - Transacciones ACID
  - Backups automatizados

#### 1.2 Validación y seguridad
- [ ] Input validation mejorado
- [ ] Rate limiting en API
- [ ] Autenticación opcional en API
- [ ] CORS configurables

#### 1.3 Testing
- [ ] Unit tests para Commands
- [ ] Integration tests para API
- [ ] Tests de scheduler
- [ ] Coverage > 80%

**Tiempo estimado:** 2-3 semanas

---

### **FASE 2: Experiencia de usuario (4-6 semanas)**

#### 2.1 Interfaz web
- [ ] Dashboard web en `/dashboard`
- [ ] Visualizar recordatorios en tabla
- [ ] Crear/editar/eliminar desde web
- [ ] Gráficos de estadísticas

#### 2.2 Documentación
- [ ] Swagger/OpenAPI para API
- [ ] Video tutorial
- [ ] Ejemplos en múltiples lenguajes
- [✓] FAQ completo 🎉

#### 2.3 UX del bot
- [✓] Botones inline en Telegram (`InlineKeyboardMarkup`) 🎉
- [✓] Confirmaciones interactivas 🎉
- [ ] Reacciones con emojis
- [ ] Búsqueda de recordatorios

#### 2.4 Nuevas funcionalidades multimedia 🆕
- [ ] **🎤 Transcripción de audio** (OpenAI Whisper)
  - Crear recordatorios con notas de voz
  - Procesamiento automático de voz a texto
  - Soporte multi-idioma
  - Ver detalles en [FEATURES_ROADMAP.md](FEATURES_ROADMAP.md)
- [ ] **🌐 Búsqueda web inteligente** (Bing Search API)
  - Comando `/search` para búsquedas simples
  - Comando `/ask` para respuestas con IA (opcional)
  - Integración con OpenAI GPT para respuestas inteligentes
  - Ver detalles en [FEATURES_ROADMAP.md](FEATURES_ROADMAP.md)

**Tiempo estimado:** 3-4 semanas

---

### **FASE 3: Características avanzadas (6-8 semanas)**

#### 3.1 Notificaciones mejoradas
- [ ] Notificaciones por email
- [ ] Notificaciones por SMS
- [ ] Webhooks personalizados
- [ ] Sonidos y vibraciones en Telegram

#### 3.2 Inteligencia artificial
- [ ] Parsing mejorado con NLP
- [ ] Sugerencias automáticas basadas en historial
- [ ] Categorización automática de recordatorios
- [ ] Detección de contexto y patrones
- [ ] Integración completa de transcripción y búsqueda web (Fase 2.4)

#### 3.3 Colaboración
- [ ] Compartir recordatorios entre usuarios
- [ ] Recordatorios grupales
- [ ] Tareas asignables
- [ ] Comentarios en recordatorios

**Tiempo estimado:** 4-6 semanas

---

### **FASE 4: Integración y escalabilidad (8-12 semanas)**

#### 4.1 Integraciones
- [ ] Integración con Google Calendar
- [ ] Integración con Outlook
- [ ] Integración con Discord
- [ ] Integración con WhatsApp
- [ ] IFTTT support

#### 4.2 Sincronización
- [ ] Sincronización multi-dispositivo
- [ ] Backup a Google Drive
- [ ] Sincronización en tiempo real
- [ ] Offline-first

#### 4.3 Escalabilidad
- [ ] PostgreSQL para producción
- [ ] Redis para caché
- [ ] Microservicios
- [ ] Load balancing

**Tiempo estimado:** 6-8 semanas

---

### **FASE 5: Monetización y expansión (12+ semanas)**

#### 5.1 Modelos freemium
- [ ] Versión gratuita limitada
- [ ] Premium features
- [ ] Suscripciones
- [ ] Planes empresariales

#### 5.2 Marketplace
- [ ] Store de plugins/extensiones
- [ ] Temas personalizables
- [ ] Scripts personalizados
- [ ] Integraciones de terceros

#### 5.3 Comunidad
- [ ] Forum de usuarios
- [ ] Repositorio de templates
- [ ] Challenges semanales
- [ ] Programa de referidos

---

## 📈 Prioridades por impacto

### 🔴 **Crítico (High Impact, Easy)**
1. **SQLite** - Mejor performance
2. **Tests** - Confiabilidad
3. **Swagger** - Documentación API

### 🟡 **Alto (Medium Impact, Medium)**
1. **Dashboard web** - UX mejorada
2. **Email notifications** - Más canales
3. **Google Calendar sync** - Integración popular

### 🟢 **Medio (Good to have)**
1. **Discord bot** - Otra plataforma
2. **Categorización** - Organización
3. **Búsqueda avanzada** - Discoverability

### 🔵 **Bajo (Nice to have, Far future)**
1. **Marketplace** - Extensibilidad
2. **Modelos freemium** - Monetización
3. **Programa de referidos** - Crecimiento

---

## 📅 Timeline tentativo

```
Feb 2026  → RPG Fase 10-11 (Guild + PvP Arena)          ✅ COMPLETADO
Feb 2026  → RPG Fase 12 (Mundo Abierto Expandido)       ✅ COMPLETADO
Mar 2026  → RPG Fase 13 (Eventos Temporales)
Abr 2026  → Base bot: SQLite + Tests + Dashboard web
May 2026  → Features IA: transcripción audio + búsqueda web
Jun 2026  → RPG Fase 14-16 (Imágenes + Mini App + IA Narrativa)
Jul 2026  → RPG Fase 17-18 (Clases + Profesiones)
```

---

## 🎓 Áreas de aprendizaje

Mientras desarrollamos, aprenderemos:

- [ ] PostgreSQL y Entity Framework Core
- [ ] React.js para dashboard
- [ ] Docker y Kubernetes
- [ ] CI/CD con GitHub Actions
- [ ] Arquitectura de microservicios
- [ ] Machine Learning básico (NLP)
- [ ] Arquitectura de eventos
- [ ] Patrones de seguridad

---

## 💰 Estimación de esfuerzo

| Fase | Features | Horas | Dificultad |
|------|----------|-------|-----------|
| 1 | 3 | 30-40h | Media |
| 2 | 6 | 60-80h | Media-Alta |
| 3 | 5 | 80-100h | Alta |
| 4 | 5 | 100-120h | Muy Alta |
| 5 | 6 | 120-150h | Muy Alta |
| **Total** | **25** | **390-490h** | - |

---

## 🤝 Colaboradores

Se buscan colaboradores en:
- Backend (.NET)
- Frontend (React)
- DevOps (Docker/K8s)
- Testing
- Documentación

---

## ❓ Feedback

¿Tienes sugerencias? Abre un [Issue en GitHub](https://github.com/kudawasama/BotTelegramPersonal/issues)

---

**Última actualización:** 8 de marzo de 2026 — migración documental a Azure y checkpoint operativo actualizado
