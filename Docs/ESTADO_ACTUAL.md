# Estado Actual del Proyecto

Ultima actualizacion: **2026-03-08**

Este documento resume exactamente donde quedamos para retomar trabajo sin perder contexto.

## Resumen rapido

- Hosting objetivo unico: **Azure App Service Linux (Plan B1)**.
- Workflow activo de despliegue: `.github/workflows/azure-deploy.yml`.
- Infraestructura legacy eliminada del repo (Fly/Render/Replit y variantes).
- Agentes y prompts nuevos para orquestacion en español ya creados.

## Completado

1. Migracion de documentacion y despliegue a Azure.
2. Limpieza de archivos legacy de deploy.
3. Ajustes de codigo para entorno cloud neutral/Azure:
   - `src/BotTelegram/Program.cs`
   - `src/BotTelegram/Services/TelegramLogger.cs`
4. Estructura de trabajo para siguientes sesiones:
   - `.github/agents/*.agent.md`
   - `.github/prompts/*.prompt.md`
   - `.github/instructions/*.instructions.md`

## Pendiente inmediato (siguiente paso)

1. Crear secrets en GitHub:
   - `AZURE_CREDENTIALS`
   - `AZURE_WEBAPP_NAME`
2. Configurar variables en Azure Web App:
   - `TELEGRAM_BOT_TOKEN`
   - `ASPNETCORE_ENVIRONMENT=Production`
3. Confirmar `Always On` habilitado en el plan B1.
4. Hacer `push` a `master` y validar deploy en GitHub Actions.

## Como retomar en 5 minutos

1. Revisar `deploy/AZURE_SETUP.md`.
2. Completar secrets/variables pendientes.
3. Ejecutar deploy con push.
4. Validar en Telegram con `/start` y `/rpg`.

## Riesgos abiertos

- Si faltan secrets, el workflow de Azure fallara en login/deploy.
- Si `TELEGRAM_BOT_TOKEN` no esta en Azure, el bot no iniciara en produccion.
