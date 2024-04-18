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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSingleton<ChannelDataProvider>();
builder.Services.AddSingleton<LiteDbContext>();

builder.Services.AddCors();

builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    string[] supportedCultures = ["en-US", "pt-BR"];
    _ = opts.SetDefaultCulture(supportedCultures[1])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    opts.ApplyCurrentCultureToResponseHeaders = true;
});

WebApplication app = builder.Build();

WebSocketOptions webSocketOptions = new()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

string hostnameCors = Environment.GetEnvironmentVariable("CORS_HOSTNAME");
app.UseCors(cors =>
{
    _ = cors.AllowCredentials();
    _ = cors.AllowAnyMethod();
    _ = cors.AllowAnyHeader();

    if (string.IsNullOrEmpty(hostnameCors))
    {
        _ = cors.SetIsOriginAllowed(origin => true);
    }
    else
    {
        _ = cors.WithOrigins(hostnameCors);
    }
});

app.UseRequestLocalization();

app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
