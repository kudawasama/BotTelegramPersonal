# Solucion de Persistencia de Datos (Azure)

## Problema base

Si el bot guarda archivos JSON en un contenedor efimero, los datos pueden perderse al redeployar.

---

## Estrategia recomendada en Azure

### Opcion 1: Azure File Share montado en App Service

Ventajas:
- Persistencia entre reinicios y despliegues.
- No depende del filesystem temporal del runtime.
- Facil de respaldar.

Pasos de alto nivel:
1. Crear Storage Account.
2. Crear File Share (ejemplo: `bottelegram-data`).
3. Montar share en la Web App.
4. Configurar la ruta de datos/logs por variable de entorno.

Variables sugeridas:
- `BOT_LOGS_PATH=/home/site/bot-data/logs`
- Ruta de datos del juego en directorio persistente equivalente.

---

### Opcion 2: Migrar datos a base de datos administrada

Para crecimiento futuro:
- Azure SQL, PostgreSQL o Cosmos DB.
- Reduce riesgo por concurrencia en JSON.
- Mejora consultas y reportes.

---

## Backups

Politica recomendada:
1. Backup automatico diario de archivos persistentes.
2. Backup manual antes de cambios de modelo (ej: `RpgPlayer`).
3. Validacion de restore en entorno de prueba.

---

## Validacion operativa

Checklist:
- Reiniciar app y confirmar que los datos persisten.
- Hacer deploy y validar continuidad de jugadores/recordatorios.
- Verificar permisos de lectura/escritura en la ruta persistente.

---

## Nota de arquitectura

Mientras el proyecto siga con JSON plano:
- Centralizar accesos con locks.
- Evitar escrituras concurrentes sin sincronizacion.
- Mantener formatos estables para facilitar migraciones futuras.
