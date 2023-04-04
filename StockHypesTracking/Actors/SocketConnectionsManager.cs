using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Actors
{
    public class SocketConnectionsManager : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly IActorRef _pollingManagerRActor;
        private Dictionary<string, IActorRef> _connections = new Dictionary<string, IActorRef>();

        public SocketConnectionsManager(IActorRef streamsManagerRActor)
        {
            _pollingManagerRActor = streamsManagerRActor;

            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                if (_connections.ContainsKey(newConnection.Id))
                {
                    _logger.Warning($"Try to start polling for exsiting connection '{newConnection.Id}'");
                    return;
                }

                var newConnectionRActor = Context.ActorOf(Akka.Actor.Props.Create<SocketConnection>(), $"connection-{newConnection.Id}");
                _logger.Info($"Add new connection '{newConnectionRActor.Path.ToStringWithAddress()}'");
                _pollingManagerRActor.Tell(newConnection, newConnectionRActor);
                _connections.Add(newConnection.Id, newConnectionRActor);
            });

            Receive<CloseConnectionMessage>((closedConnection) =>
            {
                if (TryGetConnection(closedConnection.Id, out var connectionRActor))
                {
                    _logger.Info($"Terminating connection '{connectionRActor.Path.ToStringWithAddress()}'");
                    _connections.Remove(closedConnection.Id);
                    Context.Stop(connectionRActor);
                }
            });

            Receive<UpdateStreamMessage>((updateConnection) =>
            {
                if (TryGetConnection(updateConnection.Id, out var connectionRActor))
                {
                    _logger.Info($"Update stream '{connectionRActor.Path.ToStringWithAddress()}'");
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
                _logger.Warning($"No connection '{connectionId}' found");
                return false;
            }
        }

        public static Props Props(IActorRef actorRef) => Akka.Actor.Props.Create(() => new SocketConnectionsManager(actorRef));
    }
}
