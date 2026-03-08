using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using BotTelegram.Services;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Prioridad: variable de entorno para producción, fallback local en appsettings.
var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") 
    ?? config["Telegram:Token"];

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("❌ Token NO encontrado");
    Console.WriteLine("💡 Configura TELEGRAM_BOT_TOKEN en variables de entorno o en appsettings.json");
    return;
}

Console.WriteLine("✅ Token cargado correctamente");

var bot = new TelegramBotClient(token);

// Identificador de versión/deploy
var buildDate = System.IO.File.GetLastWriteTimeUtc(typeof(Program).Assembly.Location);
var hostingEnvironment =
    Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")
    ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? "Local";

Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("🚀 BOT TELEGRAM RPG - INICIANDO");
Console.WriteLine($"📦 Versión Build: {buildDate:yyyy-MM-dd HH:mm:ss} UTC");
Console.WriteLine($"🔧 Versión Bot: v{BuildInfo.Version}");
Console.WriteLine($"🔖 Commit: {BuildInfo.GetCommitHash()}");
Console.WriteLine($"🌐 Entorno: {hostingEnvironment}");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("🤖 Bot iniciado correctamente");

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
    var aspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
    var port = Environment.GetEnvironmentVariable("PORT")
        ?? Environment.GetEnvironmentVariable("WEBSITES_PORT")
        ?? "5001";

    var url = string.IsNullOrWhiteSpace(aspNetCoreUrls)
        ? $"http://0.0.0.0:{port}"
        : aspNetCoreUrls;

    Console.WriteLine($"\n🌐 Iniciando API web en {url}");
    await app.RunAsync(url);
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
var isHosted = Console.IsInputRedirected
    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT"))
    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"))
    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITES_INSTANCE_ID"));

if (isHosted)
{
    await Task.Delay(System.Threading.Timeout.InfiniteTimeSpan);
}
else
{
    Console.ReadLine();
}
Console.WriteLine("🛑 Bot detenido");

