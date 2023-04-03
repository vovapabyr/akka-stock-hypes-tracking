using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Actors
{
    public class SocketConnectionActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private IActorRef _pollingRActor;

        public SocketConnectionActor() 
        {
            _logger = Logging.GetLogger(Context);

            Receive<NewStockPriceMessage>((stockPrice) =>
            {
                _logger.Debug($"{stockPrice}");
            });

            Receive<PollingStartedMessage>((msg) =>
            {
                _logger.Debug($"Polling actor received.");
                _pollingRActor = Sender;
            });

            Receive<UpdateStreamMessage>((updateStream) =>
            {
                _logger.Debug($"Updating {updateStream}");
                _pollingRActor?.Tell(updateStream, Self);
            });
        }

        public static Props Props() => Akka.Actor.Props.Create(() => new SocketConnectionActor());
    }
}
