using System.Diagnostics;
using System.Reflection;

namespace BotTelegram.Services
{
    /// <summary>
    /// Información de build/versión del bot.
    /// Lee el commit actual de git en runtime para mostrarlo en /start.
    /// </summary>
    public static class BuildInfo
    {
        private static string? _cachedCommit;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        // Fallback: se actualiza manualmente al hacer deploy
        private const string FallbackCommit = "65bcf3b";
        private const string FallbackDate   = "2025-02-19";
        private const string BotVersion     = "3.2.3";

        /// <summary>
        /// Obtiene el hash corto del último commit git (7 chars).
        /// Lee del proceso git si está disponible, sino usa el fallback.
        /// </summary>
        public static string GetCommitHash()
        {
            // Usar caché para no llamar git en cada request
            if (_cachedCommit != null && DateTime.UtcNow - _cacheTime < CacheDuration)
                return _cachedCommit;

            try
            {
                var psi = new ProcessStartInfo("git", "rev-parse --short HEAD")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd().Trim();
                    proc.WaitForExit(3000);
                    if (!string.IsNullOrEmpty(output) && output.Length >= 6)
                    {
                        _cachedCommit = output[..7];
                        _cacheTime = DateTime.UtcNow;
                        return _cachedCommit;
                    }
                }
            }
            catch
            {
                // Git no disponible (contenedor sin git, etc.)
            }

            _cachedCommit = FallbackCommit;
            _cacheTime = DateTime.UtcNow;
            return FallbackCommit;
        }

        /// <summary>
        /// Obtiene la fecha del último commit (formato corto).
        /// </summary>
        public static string GetCommitDate()
        {
            try
            {
                var psi = new ProcessStartInfo("git", "log -1 --format=%ci")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd().Trim();
                    proc.WaitForExit(3000);
                    if (!string.IsNullOrEmpty(output) && output.Length >= 10)
                        return output[..10]; // Solo YYYY-MM-DD
                }
            }
            catch { }

            return FallbackDate;
        }

        /// <summary>
        /// Obtiene el mensaje del último commit.
        /// </summary>
        public static string GetCommitMessage()
        {
            try
            {
                var psi = new ProcessStartInfo("git", "log -1 --format=%s")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd().Trim();
                    proc.WaitForExit(3000);
                    if (!string.IsNullOrEmpty(output))
                    {
                        // Limitar longitud para Telegram
                        return output.Length > 60 ? output[..60] + "..." : output;
                    }
                }
            }
            catch { }

            return "Fases 1-6 completadas";
        }

        /// <summary>
        /// Versión del bot
        /// </summary>
        public static string Version => BotVersion;

        /// <summary>
        /// Genera el bloque de texto de versión para /start
        /// </summary>
        public static string GetVersionBlock()
        {
            var hash = GetCommitHash();
            var date = GetCommitDate();
            var msg  = GetCommitMessage();
            return $"🔧 `v{BotVersion}` · commit `{hash}` · {date}\n📝 _{msg}_";
        }
    }
}
