using Akka.Actor;
using Akka.Hosting;
using Akka.Routing;

namespace StockHypesTracking.Actors
{
    public class StockHypesSupervisor : UntypedActor
    {
        public StockHypesSupervisor(IActorRegistry actorRegistry)
        {
            var pollingRouterProps = new ConsistentHashingPool(5).Props(Props.Create<StockPricePollingManager>());
            var pollingRouterRActor = Context.ActorOf(pollingRouterProps, "polling-router");

            var connectionsRouterProps = new ConsistentHashingPool(5).Props(SocketConnectionsManagerActor.Props(pollingRouterRActor));
            var connectionsRActor = Context.ActorOf(connectionsRouterProps, "connections-router");

            actorRegistry.TryRegister<SocketConnectionsManagerActor>(connectionsRActor, overwrite: true);
        }

        protected override void OnReceive(object message)
        {
        }
    }
}
