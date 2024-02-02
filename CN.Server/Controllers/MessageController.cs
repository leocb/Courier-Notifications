using CN.Models.Messages;
using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MessageController(WebSocketHandler wsHandler) : ControllerBase
{
    [HttpPost("send")]
    public async Task<ActionResult> SendMessage(
        [FromQuery] Guid channel,
        [FromBody] Message message
        )
    {
        await wsHandler.SendMessageAsync(channel, message);
        return Ok();
    }
}
