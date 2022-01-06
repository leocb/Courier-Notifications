using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CN.Models.Messages;

namespace CN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpPost(Name = "Send")]
        public string SendMessage(Message message, Guid session)
        {
            return "OK";
        }
    }
}
