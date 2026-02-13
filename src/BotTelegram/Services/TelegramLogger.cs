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
        // Usar volumen persistente de Fly.io
        private static readonly string _logsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "data",
            "logs"
        );

        private static readonly object _fileLock = new();

        static TelegramLogger()
        {
            // Crear directorios si no existen
            try
            {
                if (!Directory.Exists(_logsPath))
                {
                    Directory.CreateDirectory(_logsPath);
                    Console.WriteLine($"📁 Directorio de logs creado: {_logsPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error creando directorio de logs: {ex.Message}");
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
                    var logLine = $"[{timestamp}] ✓ ACTION | {action} | User: @{username} ({chatId}) | {details}";

                    // Escribir en archivo
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                    
                    // También escribir en console para que se vea en Fly logs
                    Console.WriteLine($"📝 {logLine}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log de usuario: {ex.Message} | Stack: {ex.StackTrace}");
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
                    var logLine = $"[{timestamp}] 🎮 RPG | {eventType} | Character: {characterName} ({chatId}) | {details}";

                    // Escribir en archivo
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                    
                    // También en console
                    Console.WriteLine($"🎮 {logLine}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando log RPG: {ex.Message} | Stack: {ex.StackTrace}");
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
                    var logLine = $"[{timestamp}] ❌ ERROR | {errorType} ({chatId}) | {message}";
                    if (exception != null)
                    {
                        logLine += $" | {exception.GetType().Name}: {exception.Message}";
                    }

                    // Escribir en archivo
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                    
                    // En console
                    Console.WriteLine($"❌ {logLine}");
                    if (exception != null && exception.StackTrace != null)
                    {
                        Console.WriteLine($"   Stack: {exception.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CRITICAL - Error guardando log de error: {ex.Message}");
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
                    var logLine = $"[{timestamp}] 🔄 STATE | {oldState} → {newState} | {characterName} ({chatId}) | Reason: {reason}";

                    // Escribir en archivo
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                    
                    // En console
                    Console.WriteLine($"🔄 {logLine}");
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
