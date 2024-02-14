using CN.Models.Roles;
using CN.Server.Providers;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/channel/role")]
[ApiController]
public class ChannelRoleController(ChannelDataProvider channelProvider) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AllowedSender>>> GetRoles(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId
    )
    {
        ChannelRoles roles = await channelProvider.GetChannelRoles(channelId, ownerId);
        return Ok(roles.AllowedSenders);
    }

    [HttpPost("sender")]
    public async Task<ActionResult> AllowSender(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromBody] AllowedSender newSender
    )
    {
        _ = await channelProvider.AddSenderToChannelRoles(channelId, ownerId, newSender);
        return Ok();
    }

    [HttpDelete("sender")]
    public async Task<ActionResult> RemoveSender(
        [FromHeader] Guid ownerId,
        [FromQuery] Guid channelId,
        [FromQuery] Guid senderId
    )
    {
        _ = await channelProvider.RemoveSenderFromChannelRoles(channelId, ownerId, senderId);
        return Ok();
    }
}
