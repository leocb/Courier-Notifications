using System.Net.WebSockets;

using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebSocketController(WebSocketHandler wsHandler) : ControllerBase
{
    [HttpGet("connect")]
    public async Task Get([FromQuery] Guid channelId)
    {
        WebSocketManager wsManager = this.HttpContext.WebSockets;
        if (!wsManager.IsWebSocketRequest)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using WebSocket ws = await wsManager.AcceptWebSocketAsync();

        await wsHandler.OnConnect(ws, channelId);

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
                await wsHandler.OnDisconnect(ws);
                break;
            }

            // for now, ignore anything we receive on ws from clients
            // await this.WsHandler.ReceiveAsync(ws, result, buffer);
        }
    }
}