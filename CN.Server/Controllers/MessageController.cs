using CN.Models.Messages;
using CN.Server.Providers;
using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/message")]
[ApiController]
public class MessageController(WebSocketHandler wsHandler, ChannelDataProvider channelProvider) : ControllerBase
{
    [HttpPost("send")]
    public async Task<ActionResult> SendMessage(
        [FromQuery] Guid channel,
        [FromBody] Message message
        )
    {
        await channelProvider.VerifyAllowedSender(channel, message.From);
        await wsHandler.SendMessageAsync(channel, message);
        return Ok();
    }
}
