using CN.Models.Channels;
using CN.Server.Providers;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/channel")]
[ApiController]
public class ChannelController(ChannelDataProvider channelProvider) : ControllerBase
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
        await channelProvider.UpdateChannel(channelId, ownerId, channelData);
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteChannel(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId
    )
    {
        await channelProvider.DeleteChannel(channelId, ownerId);
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
