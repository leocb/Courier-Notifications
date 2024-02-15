using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace CN.Server.WebSockets;

public static class ConnectionManager
{
    public static ConcurrentDictionary<Guid, WebSocket> Sockets { get; } = new();

    public static WebSocket GetSocketById(Guid id) => Sockets.FirstOrDefault(p => p.Key == id).Value;

    public static Guid GetId(WebSocket socket) => Sockets.FirstOrDefault(p => p.Value == socket).Key;

    public static void Add(WebSocket socket, Guid clientId) => Sockets.TryAdd(clientId, socket);

    public static async Task RemoveAndDisconnect(Guid id)
    {
        _ = Sockets.TryRemove(id, out WebSocket? socket);

        if (socket is null)
            return;

        try
        {
            await socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Connection closed by the Server",
                CancellationToken.None);
        }
        catch { /* The motherfucker left before we could serve him tea */ }
    }
}