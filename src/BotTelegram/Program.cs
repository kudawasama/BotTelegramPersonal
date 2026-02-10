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

// Inicializar API web
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
app.MapControllers();

// Ejecutar API web en background
_ = Task.Run(async () =>
{
    Console.WriteLine("\n🌐 Iniciando API web en http://localhost:5000");
    await app.RunAsync("http://localhost:5000");
});

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
Console.WriteLine("✅ API web iniciada");
Console.WriteLine("\n📱 Telegram Bot: Presiona ENTER para salir");
Console.ReadLine();
Console.WriteLine("🛑 Bot detenido");

