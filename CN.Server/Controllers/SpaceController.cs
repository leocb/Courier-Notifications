using Microsoft.AspNetCore.Mvc;

namespace CN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceController : ControllerBase
    {
        [HttpPost(Name = "Space")]
        public Guid CreateSpace(Guid session, Guid central)
        {
            return Guid.NewGuid();
        }

        [HttpDelete(Name = "Space")]
        public bool DeleteSpace(Guid session, Guid central, Guid space)
        {
            return true;
        }

        [HttpPost(Name = "RequestSpaceWritePermission")]
        public bool RequestSpaceWritePermission(Guid session, Guid Space)
        {
            return true;
        }
    }
}
