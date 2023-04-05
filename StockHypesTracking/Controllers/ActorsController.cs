using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using StockHypesTracking.Actors;

namespace StockHypesTracking.Controllers
{
    [Route("api/[controller]")]
    public class ActorsController : ControllerBase
    {
        public async Task<IActionResult> Get([FromServices] ActorSystem system, string path = "/user") 
        {
            var treeRActor = system.ActorOf(Props.Create<ApplicationTreeTraverse>());
            var actors = await treeRActor.Ask<List<string>>(path);
            system.Stop(treeRActor);
            return Ok(string.Join('\n', actors));
        }
    }
}
