using CN.Server.WebSockets;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(new WebSocketHandler());

WebApplication app = builder.Build();

WebSocketOptions webSocketOptions = new()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // nothing for now
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
