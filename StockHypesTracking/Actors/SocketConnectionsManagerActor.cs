using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Web.Messsages;

namespace StockHypesTracking.Web.Actors
{
    public class SocketConnectionsManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _pollingManagerRActor;
        private Dictionary<string, IActorRef> _connections = new Dictionary<string, IActorRef>();

        public SocketConnectionsManagerActor(IActorRef streamsManagerRActor) 
        {
            _logger = Logging.GetLogger(Context);
            _pollingManagerRActor = streamsManagerRActor;

            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                if (_connections.ContainsKey(newConnection.Id)) 
                {
                    _logger.Warning($"Trying to start polling for exsiting connectio {newConnection.Id}.");
                    return;
                }

                var socketConnectionActorName = $"connection-{newConnection.Symbol}-{newConnection.Id}";
                _logger.Info($"Adding new connection: {newConnection}. Actor: {socketConnectionActorName}");
                var newConnectionRActor = Context.ActorOf(SocketConnectionActor.Props(_pollingManagerRActor), socketConnectionActorName);
                newConnectionRActor.Tell(newConnection, Self);
                _connections.Add(newConnection.Id, newConnectionRActor);
            });
        }

        public static Props Props(IActorRef actorRef) => Akka.Actor.Props.Create(() => new SocketConnectionsManagerActor(actorRef));
    }
}
