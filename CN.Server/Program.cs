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
builder.Services.Configure<LiteDbOptions>(builder.Configuration.GetSection(LiteDbOptions.LiteDb));

WebApplication app = builder.Build();

WebSocketOptions webSocketOptions = new()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();