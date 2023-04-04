using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Hubs;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Actors
{
    public class SocketConnection : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly string _connectionId;
        private readonly StockHubService _stockHubService;
        private IActorRef _streamRActor;

        public SocketConnection(string connectionId, StockHubService stockHubService) 
        {
            _connectionId = connectionId;
            _stockHubService = stockHubService;

            Receive<UpdateStreamMessage>((updateStream) =>
            {
                _logger.Debug($"Update stream '{updateStream}'");
                _streamRActor?.Tell(updateStream, Self);
            });

            #region Stream

            ReceiveAsync<NewStockPriceMessage>(async (stockPrice) =>
            {
                _logger.Debug($"New stock '{stockPrice}'");
                await _stockHubService.PushNewStockAsync(stockPrice, _connectionId);
                Context.Sender.Tell(new StreamAckMessage(), Self);
            });

            Receive<InitStreamMessage>((msg) => 
            {
                _logger.Debug($"Stream started from '{msg.StreamMediatorRActor.Path.ToStringWithAddress()}'");
                _streamRActor = msg.StreamMediatorRActor;
                Context.Sender.Tell(new StreamAckMessage(), Self);
            });

            Receive<Status.Failure>((msg) => _logger.Error(msg.Cause, $"{msg.State}"));

            #endregion
        }

        protected override void PostStop()
        {
            _logger.Debug($"Stop '{_streamRActor.Path.ToStringWithAddress()}'");
            Context.Stop(_streamRActor);
        }
    }
}
