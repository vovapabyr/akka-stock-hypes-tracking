using Akka.Actor;
using Microsoft.AspNetCore.Mvc;

namespace StockHypesTracking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        public IActionResult Get([FromServices] ActorSystem system) 
        {
            var actors = system.ActorSelection("/user/*");
            
            return Ok();
        }
    }
}
