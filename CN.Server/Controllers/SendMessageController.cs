using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        [HttpPost(Name = "Send")]
        public bool SendMessage(CN.Models.Messages.Message message)
        {
            return true;
        }
    }
}
