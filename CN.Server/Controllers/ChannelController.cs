using CN.Models.Servers;
using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ChannelController(WebSocketHandler wsHandler) : ControllerBase
{
    private WebSocketHandler WsHandler { get; set; } = wsHandler;

    [HttpPost()]
    public async Task<ActionResult<Guid>> CreateChannel(
        [FromHeader] Guid serverId,
        [FromBody] Channel channelData
    )
    {
        return Ok(Guid.NewGuid());
    }

    [HttpPut()]
    public async Task<ActionResult> UpdateChannel(
        [FromHeader] Guid serverId,
        [FromQuery] Guid channelId,
        [FromBody] Channel channelData
    )
    {
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteChannel(
        [FromHeader] Guid serverId,
        [FromQuery] Guid channelId,
        [FromBody] Channel channelData
    )
    {
        return Ok();
    }

    [HttpGet()]
    public async Task<ActionResult<Channel>> GetChannel(
        [FromQuery] Guid channelId
    )
    {
        return Ok(new Channel());
    }
}
