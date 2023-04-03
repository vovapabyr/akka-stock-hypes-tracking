using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Web.Messsages;

namespace StockHypesTracking.Web.Actors
{
    public class SocketConnectionsManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _streamsManagerRActor;

        public SocketConnectionsManagerActor(IActorRef streamsManagerRActor) 
        {
            _logger = Logging.GetLogger(Context);
            _streamsManagerRActor = streamsManagerRActor;

            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                var socketConnectionActorName = $"connection-{newConnection.Symbol}-{newConnection.Id}";
                _logger.Info($"Adding new connection: {newConnection}. Actor: {socketConnectionActorName}");
                var newConnectionRActor = Context.ActorOf(SocketConnectionActor.Props(_streamsManagerRActor), socketConnectionActorName);
                newConnectionRActor.Tell(newConnection, Self);
            });
        }

        public static Props Props(IActorRef actorRef) => Akka.Actor.Props.Create(() => new SocketConnectionsManagerActor(actorRef));
    }
}
