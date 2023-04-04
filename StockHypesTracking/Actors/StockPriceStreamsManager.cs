using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Actors
{
    public class StockPriceStreamsManager : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public StockPriceStreamsManager()
        {
            Receive<RegisterNewConnectionMessage>((newConnection) =>
            {
                var streamMediatorRActor = Context.ActorOf(StockPriceStreamMediator.Props(Sender, newConnection.Symbol, newConnection.Interval), $"stream-{newConnection.Id}");
                _logger.Info($"Add new stream mediator '{streamMediatorRActor.Path.ToStringWithAddress()}'");
            });
        }
    }
}
