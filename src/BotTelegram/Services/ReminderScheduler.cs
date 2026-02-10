using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using BotTelegram.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotTelegram.Services
{
    public class ReminderScheduler
    {
        private readonly ITelegramBotClient _bot;
        private readonly ReminderService _service;
        private bool _isRunning = false;

        public ReminderScheduler(ITelegramBotClient bot)
        {
            _bot = bot;
            _service = new ReminderService();
        }

        public void Start()
        {
            if (_isRunning)
            {
                Console.WriteLine("⚠️ [SCHEDULER] Ya está en ejecución");
                return;
            }

            _isRunning = true;
            Console.WriteLine("[SCHEDULER] Iniciando en background...");

            // Usar ThreadPool en lugar de Task.Run para mayor confiabilidad
            System.Threading.ThreadPool.QueueUserWorkItem(async _ =>
            {
                Console.WriteLine("🔔 [SCHEDULER] ===== INICIADO EN THREAD =====");
                int ciclo = 0;

                while (_isRunning)
                {
                    ciclo++;
                    try
                    {
                        var reminders = _service.GetAll();
                        var now = DateTimeOffset.Now;

                        Console.WriteLine($"\n📊 [CICLO {ciclo}] {now:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"   Total recordatorios: {reminders.Count}");

                        var pendientes = reminders.Where(r => !r.Notified).ToList();
                        Console.WriteLine($"   Pendientes: {pendientes.Count}");

                        var vencidos = pendientes.Where(r => r.DueAt <= now).ToList();
                        Console.WriteLine($"   Vencidos: {vencidos.Count}");

                        foreach (var r in vencidos)
                        {
                            try
                            {
                                Console.WriteLine($"\n   🚀 Enviando a ChatId {r.ChatId}: {r.Text}");

                                var result = await _bot.SendMessage(
                                    r.ChatId,
                                    $"🔔 *RECORDATORIO*\n\n{r.Text}",
                                    parseMode: ParseMode.Markdown,
                                    cancellationToken: CancellationToken.None);

                                if (r.Recurrence == BotTelegram.Models.RecurrenceType.None)
                                {
                                    r.Notified = true;
                                }
                                else
                                {
                                    // Crear siguiente ocurrencia
                                    var nextDueAt = r.Recurrence switch
                                    {
                                        BotTelegram.Models.RecurrenceType.Daily => r.DueAt.AddDays(1),
                                        BotTelegram.Models.RecurrenceType.Weekly => r.DueAt.AddDays(7),
                                        BotTelegram.Models.RecurrenceType.Monthly => r.DueAt.AddMonths(1),
                                        BotTelegram.Models.RecurrenceType.Yearly => r.DueAt.AddYears(1),
                                        _ => r.DueAt.AddHours(1)
                                    };
                                    
                                    r.DueAt = nextDueAt;
                                    r.Notified = false;
                                    Console.WriteLine($"      🔄 Próxima ocurrencia: {nextDueAt:dd/MM/yyyy HH:mm}");
                                }

                                Console.WriteLine($"      ✅ Enviado (MessageId: {result.MessageId})");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"      ❌ Error: {ex.GetType().Name}: {ex.Message}");
                            }
                        }

                        if (vencidos.Any())
                        {
                            _service.UpdateAll(reminders);
                            Console.WriteLine($"   ✔️ {vencidos.Count} recordatorios actualizados");
                        }

                        await Task.Delay(30_000); // cada 30 segundos
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ [CICLO {ciclo}] ERROR: {ex}");
                        await Task.Delay(30_000);
                    }
                }
            });
        }
    }
}
