using System.Net.WebSockets;
using System.Text;

namespace CN.Server.WebSockets;

public class WebSocketHandler
{
    public async Task OnConnect(WebSocket socket)
    {
        ConnectionManager.Add(socket);

        Guid socketId = ConnectionManager.GetId(socket);
        await SendMessageToAllAsync($"{socketId} is now connected");
    }

    public async Task OnDisconnect(WebSocket socket)
    {
        Guid socketId = ConnectionManager.GetId(socket);
        await SendMessageToAllAsync($"{socketId} left");

        await ConnectionManager.RemoveAndDisconnect(ConnectionManager.GetId(socket));
    }

    public async Task SendMessageAsync(WebSocket socket, string message)
    {
        if (socket.State != WebSocketState.Open)
            return;

        ArraySegment<byte> buffer = new(Encoding.UTF8.GetBytes(message), 0, message.Length);
        await socket.SendAsync(buffer,
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }

    public async Task SendMessageAsync(Guid socketId, string message) =>
        await SendMessageAsync(ConnectionManager.GetSocketById(socketId), message);

    public async Task SendMessageToAllAsync(string message)
    {
        foreach (WebSocket ws in ConnectionManager.Sockets.Values)
        {
            if (ws.State == WebSocketState.Open)
                await SendMessageAsync(ws, message);
        }
    }

    public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        Guid socketId = ConnectionManager.GetId(socket);
        string message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

        await SendMessageToAllAsync(message);
    }
}
