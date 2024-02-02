using CN.Models.Servers;
using CN.Server.Providers;
using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ChannelController(WebSocketHandler wsHandler, ChannelDataProvider channelProvider) : ControllerBase
{
    private WebSocketHandler WsHandler { get; set; } = wsHandler;
    private ChannelDataProvider ChannelProvider { get; set; } = channelProvider;

    [HttpPost()]
    public async Task<ActionResult<Guid>> CreateChannel(
        [FromHeader] Guid ownerId,
        [FromBody] Channel channelData
    )
    {
        Guid newChannelId = await this.ChannelProvider.CreateChannel(ownerId, channelData);
        return Ok(newChannelId);
    }

    [HttpPut()]
    public async Task<ActionResult> UpdateChannel(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromBody] Channel channelData
    )
    {
        await this.ChannelProvider.UpdateChannel(ownerId, channelId, channelData);
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteChannel(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId
    )
    {
        await this.ChannelProvider.DeleteChannel(ownerId, channelId);
        return Ok();
    }

    [HttpGet()]
    public async Task<ActionResult<Channel>> GetChannel(
        [FromQuery] Guid channelId
    )
    {
        Channel channelData = await this.ChannelProvider.GetChannel(channelId);
        return Ok(channelData);
    }

    [HttpGet("bulk")]
    public async Task<ActionResult<List<Channel>>> GetAllChannels(
        [FromBody] List<Guid> channelIdList
    )
    {
        List<Channel> channelDataList = await this.ChannelProvider.GetChannelBulk(channelIdList);
        return Ok(channelDataList);
    }
}
