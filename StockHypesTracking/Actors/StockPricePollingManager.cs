using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Web.Messsages;

namespace StockHypesTracking.Web.Actors
{
    public class StockPricePollingManager : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;

        public StockPricePollingManager()
        {
            _logger = Logging.GetLogger(Context);

            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                var pollActorName =$"poll-{newConnection.Symbol}-{newConnection.Id}";
                _logger.Info($"Adding new poller for: {newConnection}. Actor: {pollActorName}");
                Context.ActorOf(StockPricePollActor.Props(Sender, newConnection.Symbol, newConnection.Interval), pollActorName);
            });
        }
    }
}
