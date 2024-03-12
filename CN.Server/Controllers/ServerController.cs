using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/server")]
[ApiController]
public class ServerController(WebSocketHandler wsHandler) : ControllerBase
{
    [HttpGet("/")]
    public ActionResult GetRoot() => Ok("Welcome");
}
