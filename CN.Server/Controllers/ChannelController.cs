using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
public class ChannelController : Controller
{

    [HttpGet("/")]
    public async Task<ActionResult> GetRoot()
    {
        return Ok("Welcome");
    }
}
