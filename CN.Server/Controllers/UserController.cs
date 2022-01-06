using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet(Name = "GetNewGuid")]
        public Guid GetNewGuid()
        {
            return new Guid();
        }

        [HttpPost(Name = "LogInUser")]
        public Guid LogIn (string user, string password)
        {
            // if user does not exist, create new Guid then create a session and return
            // if it does, mark as logged in and return a new session (timeout?)
            return Guid.NewGuid();
        }
    }
}
