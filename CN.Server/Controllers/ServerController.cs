using CN.Server.WebSockets;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ServerController(WebSocketHandler wsHandler) : ControllerBase
{
    private WebSocketHandler WsHandler { get; set; } = wsHandler;

    [HttpGet("/")]
    public ActionResult GetRoot()
    {
        return Ok("Welcome");
    }

    [HttpGet("id")]
    public ActionResult GetServerId()
    {
        return Ok(WsHandler.ServerId);
    }
}
