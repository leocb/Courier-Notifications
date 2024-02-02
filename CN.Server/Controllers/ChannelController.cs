using CN.Models.Servers;
using CN.Server.Providers;
using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ChannelController(WebSocketHandler wsHandler, ChannelDataProvider channelProvider) : ControllerBase
{

    [HttpPost()]
    public async Task<ActionResult<Guid>> CreateChannel(
        [FromHeader] Guid ownerId,
        [FromBody] Channel channelData
    )
    {
        Guid newChannelId = await channelProvider.CreateChannel(ownerId, channelData);
        return Ok(newChannelId);
    }

    [HttpPut()]
    public async Task<ActionResult> UpdateChannel(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromBody] Channel channelData
    )
    {
        await channelProvider.UpdateChannel(ownerId, channelId, channelData);
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteChannel(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId
    )
    {
        await channelProvider.DeleteChannel(ownerId, channelId);
        return Ok();
    }

    [HttpGet()]
    public async Task<ActionResult<Channel>> GetChannel(
        [FromQuery] Guid channelId
    )
    {
        Channel channelData = await channelProvider.GetChannel(channelId);
        return Ok(channelData);
    }

    [HttpGet("bulk")]
    public async Task<ActionResult<List<Channel>>> GetAllChannels(
        [FromBody] List<Guid> channelIdList
    )
    {
        List<Channel> channelDataList = await channelProvider.GetChannelBulk(channelIdList);
        return Ok(channelDataList);
    }
}
