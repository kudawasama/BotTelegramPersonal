# Azure Setup (Plan Basico B1)

Esta guia reemplaza los despliegues legacy y deja el proyecto estandarizado en Azure.

## Objetivo

Desplegar `BotTelegram` en **Azure App Service Linux (Basic B1)** con CI/CD desde GitHub Actions.

## Checkpoint de continuidad (2026-03-08)

### Ya completado

- Workflow de Azure creado: `.github/workflows/azure-deploy.yml`.
- Referencias activas de deploy legacy removidas del proyecto.
- Documentación principal migrada a Azure.

### Pendiente para cerrar despliegue

1. Crear `AZURE_CREDENTIALS` en GitHub Secrets.
2. Crear `AZURE_WEBAPP_NAME` en GitHub Secrets.
3. Configurar `TELEGRAM_BOT_TOKEN` en Azure Web App.
4. Verificar `Always On` en App Service Plan B1.
5. Ejecutar primer deploy por `push` a `master`.

## 1) Crear recursos en Azure

1. Crea un **Resource Group** (ej: `rg-bottelegram-prod`).
2. Crea un **App Service Plan**:
   - SKU: `B1`
   - OS: Linux
3. Crea una **Web App** en ese plan.
4. Configura runtime .NET (si no aparece .NET 9, usa runtime soportado por tu suscripcion y revisa compatibilidad del proyecto).

## 2) Variables de entorno en Azure Web App

En `Settings > Environment variables`, agrega al menos:

- `TELEGRAM_BOT_TOKEN` = token real de BotFather.
- `ASPNETCORE_ENVIRONMENT` = `Production`.

Opcional:

- `BOT_LOGS_PATH` = ruta de logs persistentes si deseas personalizarla.

## 3) Configuracion recomendada del plan B1

- Habilitar **Always On** para evitar pausas del proceso.
- Configurar zona horaria segun tu operacion.
- Activar logs de aplicacion para diagnostico.

## 4) GitHub Secrets para despliegue automatico

En el repositorio de GitHub, crea:

- `AZURE_CREDENTIALS` (JSON de Service Principal).
- `AZURE_WEBAPP_NAME` (nombre exacto de la Web App).

## 5) Workflow de despliegue

El workflow vive en:

- `.github/workflows/azure-deploy.yml`

Acciones principales:

1. Restore/build/publish del proyecto .NET.
2. Login en Azure con `AZURE_CREDENTIALS`.
3. Deploy del paquete a la Web App.

## 6) Verificacion post-deploy

1. Revisa que el workflow finalice en verde.
2. Valida logs de la Web App.
3. En Telegram, prueba `/start` y un comando RPG (`/rpg` o `/menu`) para confirmar polling activo.

## 7) Operacion diaria recomendada

- Despliegues por `push` a `master`.
- Verifica estado de la app tras cada release.
- Mantener un solo destino de infraestructura: **Azure**.
