using Microsoft.AspNetCore.Mvc;
using CN.Models.Centrals;

namespace CN.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentralController : ControllerBase
    {

        [HttpGet(Name = "Central")]
        public Central GetCentral(Guid? session, Guid? central)
        {
            if (central == null) throw new ArgumentNullException(nameof(central));
            
            // if public central, return normally, if private, check if current session has permission then return

            return new Central();
        }

        [HttpPost(Name = "Central")]
        public Guid CreateCentral(Guid? session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            // Create a Central, assign it to the user in the session, return the new central GUID

            return Guid.NewGuid();
        }

        [HttpDelete(Name = "Central")]
        public bool DeleteCentral(Guid? session, Guid? central)
        {
            if (session == null) throw new ArgumentNullException(nameof (session));
            if (central == null) throw new ArgumentNullException(nameof(central));

            // delete the central if the user in the session has write permission on the central

            return true;
        }

        [HttpPut(Name = "Central")]
        public bool UpdateCentral(Guid? session, Guid? central)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (central == null) throw new ArgumentNullException(nameof(central));

            // update the central if the user in the session has write permission on the central

            return true;
        }

        [HttpPost(Name = "RequestCentralWritePermission")]
        public bool RequestCentralWritePermission(Guid session, Guid Central)
        {
            // Create a write permission request on the central
            return true;
        }
    }
}
