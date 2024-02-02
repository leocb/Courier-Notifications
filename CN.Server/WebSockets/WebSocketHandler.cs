using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using CN.Models.Messages;
using CN.Server.Exceptions;
using CN.Server.Options;

namespace CN.Server.WebSockets;

public class WebSocketHandler
{
    public Guid ServerId { get; } = Guid.NewGuid();
    public async Task OnConnect(WebSocket socket, Guid clientId)
    {
        ConnectionManager.Add(socket, clientId);

        Message message = new()
        {
            Date = DateTime.Now,
            FromKey = ServerId,
            FromName = "Server",
            Status = MessageStatus.Info,
            Text = "Connected"
        };
        await SendMessageAsync(socket, message);
    }

    public async Task OnDisconnect(WebSocket socket)
    {
        Message message = new()
        {
            Date = DateTime.Now,
            FromKey = ServerId,
            FromName = "Server",
            Status = MessageStatus.Info,
            Text = "Disconnected"
        };
        await SendMessageAsync(socket, message);

        await ConnectionManager.RemoveAndDisconnect(ConnectionManager.GetId(socket));
    }

    public async Task SendMessageAsync(Guid clientId, object message) =>
        await SendMessageAsync(ConnectionManager.GetSocketById(clientId), message);
    public async Task SendMessageAsync(WebSocket socket, object data)
    {
        if (socket is null)
            throw new CourierException("channel not found");

        if (socket.State != WebSocketState.Open)
            return;

        string json = JsonSerializer.Serialize(data, Option.JsonSerializer);

        ArraySegment<byte> buffer = new(Encoding.UTF8.GetBytes(json), 0, json.Length);
        await socket.SendAsync(buffer,
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }

    public async Task SendMessageToAllAsync(string message)
    {
        foreach (WebSocket ws in ConnectionManager.Sockets.Values)
        {
            if (ws.State == WebSocketState.Open)
                await SendMessageAsync(ws, message);
        }
    }

    public void Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        // do nothing
    }
}
