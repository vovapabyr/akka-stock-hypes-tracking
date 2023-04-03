using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Web.Messsages;

namespace StockHypesTracking.Web.Actors
{
    public class SocketConnectionActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _streamsManagerRActor;

        public SocketConnectionActor(IActorRef streamsManagerRActor) 
        {
            _logger = Logging.GetLogger(Context);

            _streamsManagerRActor = streamsManagerRActor;

            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                _logger.Debug(newConnection.ToString());
                _streamsManagerRActor.Tell(newConnection);
            });

            Receive<NewStockPriceMessage>((stockPrice) =>
            {
                _logger.Debug(stockPrice.ToString());
            });
        }

        public static Props Props(IActorRef actorRef) => Akka.Actor.Props.Create(() => new SocketConnectionActor(actorRef));
    }
}
