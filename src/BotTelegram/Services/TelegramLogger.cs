using System;
using System.IO;

namespace BotTelegram.Services
{
    /// <summary>
    /// Sistema de logging para registrar todas las acciones en Telegram
    /// Los logs se guardan en archivos para auditoría y pruebas
    /// </summary>
    public class TelegramLogger
    {
        private static readonly string _logsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "logs",
            "telegram"
        );

        private static readonly object _fileLock = new();

        static TelegramLogger()
        {
            // Crear directorios si no existen
            if (!Directory.Exists(_logsPath))
            {
                Directory.CreateDirectory(_logsPath);
            }
        }

        /// <summary>
        /// Registra una acción de usuario con timestamp
        /// </summary>
        public static void LogUserAction(long chatId, string username, string action, string details = "")
        {
            try
            {
                lock (_fileLock)
                {
                    var date = DateTime.Now;
                    var fileName = $"user_{chatId}_{date:yyyy-MM-dd}.log";
                    var filePath = Path.Combine(_logsPath, fileName);

                    var timestamp = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logLine = $"[{timestamp}] ACTION: {action} | User: {username} | Details: {details}";

                    File.AppendAllText(filePath, logLine + Environment.NewLine);

                    Console.WriteLine($"✓ Log guardado: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un evento de RPG (combate, progreso, etc)
        /// </summary>
        public static void LogRpgEvent(long chatId, string characterName, string eventType, string details)
        {
            try
            {
                lock (_fileLock)
                {
                    var date = DateTime.Now;
                    var fileName = $"rpg_{characterName}_{date:yyyy-MM-dd}.log";
                    var filePath = Path.Combine(_logsPath, fileName);

                    var timestamp = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logLine = $"[{timestamp}] EVENT: {eventType} | Character: {characterName} | ChatID: {chatId} | Details: {details}";

                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log RPG: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra mensajes de error o excepciones
        /// </summary>
        public static void LogError(long chatId, string errorType, string message, Exception? exception = null)
        {
            try
            {
                lock (_fileLock)
                {
                    var date = DateTime.Now;
                    var fileName = $"errors_{date:yyyy-MM-dd}.log";
                    var filePath = Path.Combine(_logsPath, fileName);

                    var timestamp = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logLine = $"[{timestamp}] ERROR: {errorType} | ChatID: {chatId} | Message: {message}";
                    if (exception != null)
                    {
                        logLine += $" | Exception: {exception.Message} | StackTrace: {exception.StackTrace}";
                    }

                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log de error: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un cambio de estado importante
        /// </summary>
        public static void LogStateChange(long chatId, string characterName, string oldState, string newState, string reason)
        {
            try
            {
                lock (_fileLock)
                {
                    var date = DateTime.Now;
                    var fileName = $"state_{date:yyyy-MM-dd}.log";
                    var filePath = Path.Combine(_logsPath, fileName);

                    var timestamp = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logLine = $"[{timestamp}] STATE_CHANGE: {characterName} | {oldState} -> {newState} | Reason: {reason} | ChatID: {chatId}";

                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log de estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Retorna la ruta de los logs para que el usuario pueda descargarlos
        /// </summary>
        public static string GetLogsPath()
        {
            return _logsPath;
        }

        /// <summary>
        /// Obtiene los logs de un usuario específico
        /// </summary>
        public static string? GetUserLogFile(long chatId, DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Now;
            var fileName = $"user_{chatId}_{targetDate:yyyy-MM-dd}.log";
            var filePath = Path.Combine(_logsPath, fileName);

            return File.Exists(filePath) ? filePath : null;
        }

        /// <summary>
        /// Obtiene todos los archivos de log del usuario (últimos 7 días)
        /// </summary>
        public static List<string> GetUserLogFiles(long chatId)
        {
            var files = new List<string>();
            var logsDir = new DirectoryInfo(_logsPath);

            if (!logsDir.Exists)
                return files;

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(-i);
                var fileName = $"user_{chatId}_{date:yyyy-MM-dd}.log";
                var filePath = Path.Combine(_logsPath, fileName);

                if (File.Exists(filePath))
                {
                    files.Add(filePath);
                }
            }

            return files;
        }

        /// <summary>
        /// Limpia logs más antiguos que X días
        /// </summary>
        public static void CleanOldLogs(int daysToKeep = 30)
        {
            try
            {
                lock (_fileLock)
                {
                    var logsDir = new DirectoryInfo(_logsPath);
                    if (!logsDir.Exists)
                        return;

                    var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                    foreach (var file in logsDir.GetFiles("*.log"))
                    {
                        if (file.LastWriteTime < cutoffDate)
                        {
                            file.Delete();
                            Console.WriteLine($"🗑️ Log antiguo eliminado: {file.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error limpiando logs: {ex.Message}");
            }
        }
    }
}
