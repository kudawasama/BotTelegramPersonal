---
name: Plan Fase RPG Azure
description: "Genera y ejecuta un plan técnico por fases para cambios RPG o migraciones, usando el orquestador maestro y cierre con validación."
argument-hint: "Describe la fase o la funcionalidad que quieres implementar."
agent: maestro
---

Tarea: $ARGUMENTS

Crea un plan técnico accionable y luego ejecútalo.

Requisitos:
- Divide en pasos pequeños con orden de implementación.
- Mantén comunicación y explicaciones en español.
- Asegura compatibilidad con Azure como único destino de despliegue.
- Incluye validación final (build, riesgos y próximos pasos).
