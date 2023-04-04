using Akka.Actor;
using Akka.Event;
using Akka.Streams;
using Akka.Streams.Dsl;
using StockHypesTracking.Messsages;
using StockHypesTracking.Streams;

namespace StockHypesTracking.Actors
{
    public class StockPriceStreamMediator : ReceiveActor
    {
        private string _symbol;
        private int _interval;
        private readonly ISourceQueueWithComplete<NewStockPriceMessage> _sourceQueue;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private IActorRef _pollingRActor;

        public StockPriceStreamMediator(IActorRef connectionRActor, string symbol, int interval)
        {
            _symbol = symbol;
            _interval = interval;

            _sourceQueue = Source.Queue<NewStockPriceMessage>(0, overflowStrategy: OverflowStrategy.Backpressure).Via(StockStreamHelper.BuildDataFlow()).Via(StockStreamHelper.LogFlow<NewStockPriceMessage>(d => $"Yoooooo. Stream is working {d}"))
                .To(Sink.ActorRefWithAck<NewStockPriceMessage>(connectionRActor, new InitStreamMessage(Self), new StreamAckMessage(), new StreamCompletedMessage())).Run(Context.System.Materializer());

            InitPolling();

            Receive<UpdateStreamMessage>((updateStream) =>
            {
                _logger.Debug($"Update stream '{updateStream}'");
                _interval = updateStream.Interval;
                _symbol = updateStream.Symbol;

                _logger.Debug($"Stop polling '{_pollingRActor.Path.ToStringWithUid()}'");
                Context.Stop(_pollingRActor);
            });

            Receive<Terminated>((terminated) =>
            {
                if(terminated.ActorRef == _pollingRActor)
                    InitPolling();
            });

            ReceiveAsync<NewStockPriceMessage>(async (newStock) => await _sourceQueue.OfferAsync(newStock));
        }
        private void InitPolling() 
        {
            _pollingRActor = Context.ActorOf(StockPricePolling.Props(_symbol, _interval), $"polling-{_symbol}-{_interval}");
            _logger.Debug($"Start polling '{_pollingRActor.Path.ToStringWithUid()}'");
            Context.Watch(_pollingRActor);
        }

        public static Props Props(IActorRef connectionRActor, string symbol, int interval) => Akka.Actor.Props.Create(() => new StockPriceStreamMediator(connectionRActor, symbol, interval));
    }
}
