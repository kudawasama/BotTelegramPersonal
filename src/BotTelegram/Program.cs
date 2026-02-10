using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using BotTelegram.Services;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var token = config["Telegram:Token"];

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("❌ Token NO encontrado");
    return;
}

Console.WriteLine("✅ Token cargado correctamente");

var bot = new TelegramBotClient(token);

Console.WriteLine("🤖 Bot iniciado correctamente");

// Inicializar scheduler en background
Console.WriteLine("📅 Inicializando ReminderScheduler...");
var scheduler = new ReminderScheduler(bot);
scheduler.Start();
Console.WriteLine("✅ ReminderScheduler iniciado");

Console.WriteLine("🔔 Iniciando StartReceiving()...");

bot.StartReceiving(
    async (botClient, update, ct) =>
    {
        try
        {
            Console.WriteLine($"📨 [UPDATE RECIBIDO] Tipo: {update.Type}");
            await BotService.HandleUpdate(botClient, update, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [ERROR EN HandleUpdate] {ex.Message}");
        }
    },
    (botClient, exception, ct) =>
    {
        Console.WriteLine($"❌ [ERROR EN POLLING] {exception.GetType().Name}: {exception.Message}");
        return Task.CompletedTask;
    }
);

Console.WriteLine("✅ StartReceiving() iniciado");
Console.WriteLine("⏳ Esperando mensajes... Presiona ENTER para salir");
Console.ReadLine();
Console.WriteLine("🛑 Bot detenido");

