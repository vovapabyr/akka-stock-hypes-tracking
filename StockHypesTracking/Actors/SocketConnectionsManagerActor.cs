using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Actors
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
                var newConnectionRActor = Context.ActorOf(SocketConnectionActor.Props(), socketConnectionActorName);
                _pollingManagerRActor.Tell(newConnection, newConnectionRActor);
                _connections.Add(newConnection.Id, newConnectionRActor);
            });

            Receive<CloseConnectionMessage>((closedConnection) =>
            {
                if (TryGetConnection(closedConnection.Id, out var connectionRActor))
                {

                    _logger.Info($"Terminating connection {closedConnection.Id}.");
                    _connections.Remove(closedConnection.Id);
                    Context.Stop(connectionRActor);
                }
            });

            Receive<UpdateStreamMessage>((updateConnection) =>
            {
                if (TryGetConnection(updateConnection.Id, out var connectionRActor))
                {
                    _logger.Info($"Updating stream {updateConnection}.");
                    connectionRActor.Tell(updateConnection, Self);
                }
            });
        }

        private bool TryGetConnection(string connectionId, out IActorRef connectionRActor) 
        {
            if (_connections.TryGetValue(connectionId, out connectionRActor))
                return true;
            else
            {
                _logger.Warning($"No connection {connectionId} found.");
                return false;
            }
        }

        public static Props Props(IActorRef actorRef) => Akka.Actor.Props.Create(() => new SocketConnectionsManagerActor(actorRef));
    }
}
