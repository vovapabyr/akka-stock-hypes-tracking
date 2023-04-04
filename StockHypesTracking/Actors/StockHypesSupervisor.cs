using Akka.Actor;
using Akka.Event;
using Akka.Hosting;
using Akka.Routing;

namespace StockHypesTracking.Actors
{
    public class StockHypesSupervisor : UntypedActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public StockHypesSupervisor(IActorRegistry actorRegistry)
        {
            var streamsRouterProps = new ConsistentHashingPool(5).Props(Props.Create<StockPriceStreamsManager>());
            var streamsRouterRActor = Context.ActorOf(streamsRouterProps, "streams-router");
            _logger.Info($"Start streams router '{streamsRouterRActor.Path.ToStringWithAddress()}'");

            var connectionsRouterProps = new ConsistentHashingPool(5).Props(SocketConnectionsManager.Props(streamsRouterRActor));
            var connectionsRActor = Context.ActorOf(connectionsRouterProps, "connections-router");
            _logger.Info($"Start connections router '{connectionsRActor.Path.ToStringWithAddress()}'");

            actorRegistry.TryRegister<SocketConnectionsManager>(connectionsRActor, overwrite: true);
        }

        protected override void OnReceive(object message)
        {
        }
    }
}
