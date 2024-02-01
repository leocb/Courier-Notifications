using System.Net.WebSockets;

using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;

public class WebSocketController(WebSocketHandler wsHandler) : ControllerBase
{
    private WebSocketHandler WsHandler { get; set; } = wsHandler;

    [HttpGet("/ws")]
    public async Task Get()
    {
        WebSocketManager wsManager = this.HttpContext.WebSockets;
        if (!wsManager.IsWebSocketRequest)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using WebSocket ws = await wsManager.AcceptWebSocketAsync();

        await this.WsHandler.OnConnect(ws);

        await WsLoop(ws);
    }

    private async Task WsLoop(WebSocket ws)
    {
        byte[] buffer = new byte[1024 * 4];

        while (ws.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await ws.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await this.WsHandler.OnDisconnect(ws);
                break;
            }

            await this.WsHandler.ReceiveAsync(ws, result, buffer);
        }
    }
}