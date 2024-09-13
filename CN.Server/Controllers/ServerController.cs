using CN.Server.WebSockets;

using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/server")]
[ApiController]
public class ServerController(WebSocketHandler wsHandler) : ControllerBase
{
    [HttpGet("/api")]
    public ActionResult GetAPI() => Ok("Welcome");

    [HttpGet("id")]
    public ActionResult GetServerId() => Ok(wsHandler.ServerId);
}