using System.Text.Json.Serialization;

using CN.Models;
using CN.Server.Database;
using CN.Server.Providers;
using CN.Server.WebSockets;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = Options.JsonSerializer.PropertyNamingPolicy;
    foreach (JsonConverter item in Options.JsonSerializer.Converters)
    {
        options.JsonSerializerOptions.Converters.Add(item);
    }
});

// Swagger
string? enableSwaggerEnv = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
bool enableSwagger = !string.IsNullOrEmpty(enableSwaggerEnv) && enableSwaggerEnv.Equals("true");
if (enableSwagger)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Courier
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSingleton<ChannelDataProvider>();
builder.Services.AddSingleton<LiteDbContext>();

// Localization
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    string[] supportedCultures = ["en-US", "pt-BR"];
    _ = opts.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    opts.ApplyCurrentCultureToResponseHeaders = true;
});

//CORS
builder.Services.AddCors();


// ----------------------------------------------- APP

WebApplication app = builder.Build();

WebSocketOptions webSocketOptions = new()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

// CORS Protection
string? hostnameCors = Environment.GetEnvironmentVariable("CORS_HOSTNAME");
app.UseCors(cors =>
{
    _ = cors.AllowCredentials();
    _ = cors.AllowAnyMethod();
    _ = cors.AllowAnyHeader();

    // Anyone
    if (string.IsNullOrEmpty(hostnameCors))
    {
        _ = cors.SetIsOriginAllowed(origin => true);
    }
    // Only specified origins
    else
    {
        _ = cors.WithOrigins(hostnameCors);
    }
});

app.UseRequestLocalization();

app.UseWebSockets(webSocketOptions);

// Swagger
if (enableSwagger)
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
