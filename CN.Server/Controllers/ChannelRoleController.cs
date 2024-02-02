using CN.Models.Server;
using CN.Server.Providers;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/channel/role")]
[ApiController]
public class ChannelRoleController(ChannelDataProvider channelProvider) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ChannelRoles>> GetRoles(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId
    )
    {
        ChannelRoles roles = await channelProvider.GetChannelRoles(channelId, ownerId);
        return Ok(roles);
    }

    [HttpPost("sender")]
    public async Task<ActionResult<ChannelRoles>> AllowSender(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromBody] AllowedSender newSender
    )
    {
        ChannelRoles updatedRoles = await channelProvider.AddSenderToChannelRoles(channelId, ownerId, newSender);
        return Ok(updatedRoles);
    }

    [HttpDelete("sender")]
    public async Task<ActionResult<ChannelRoles>> RemoveSender(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromBody] Guid senderId
    )
    {
        ChannelRoles updatedRoles = await channelProvider.RemoveSenderFromChannelRoles(channelId, ownerId, senderId);
        return Ok(updatedRoles);
    }
}
