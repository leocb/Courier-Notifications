using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using CN.Models;
using CN.Models.Channels;
using CN.Models.Exceptions;
using CN.Models.Messages;

namespace CN.Server.WebSockets;

public class WebSocketHandler
{
    public Guid ServerId { get; } = Guid.NewGuid();
    public async Task OnConnect(WebSocket socket, Channel channel)
    {
        ConnectionManager.Add(socket, channel.Id ?? throw new CourierException("Invalid Channel"));

        Message message = new()
        {
            Date = DateTime.Now,
            From = this.ServerId,
            Status = MessageStatus.Info,
            Text = $"Connected to channel: {channel.Name}"
        };
        await SendMessageAsync(socket, message);
    }

    public async Task OnDisconnect(WebSocket socket)
    {
        Guid id = ConnectionManager.GetId(socket);
        await ConnectionManager.RemoveAndDisconnect(id);
    }

    public async Task SendMessageAsync(Guid clientId, object message) =>
        await SendMessageAsync(ConnectionManager.GetSocketById(clientId), message);
    public async Task SendMessageAsync(WebSocket socket, object data)
    {
        if (socket is null)
            throw new CourierException("channel not found or offline");

        if (socket.State != WebSocketState.Open)
            return;

        string json = JsonSerializer.Serialize(data, Options.JsonSerializer);

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
