using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;
using YahooFinanceApi;

namespace StockHypesTracking.Actors
{
    public class StockPricePollingActor : ReceiveActor, IWithTimers
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _socketConnectionRActor;
        private string _symbol;
        private int _interval;

        public StockPricePollingActor(IActorRef socketConnectionRActor, string symbol, int interval)
        {
            _logger = Logging.GetLogger(Context);
            _socketConnectionRActor = socketConnectionRActor;
            _interval = interval;
            _symbol = symbol;

            ReceiveAsync<PollStockPriceMessage>(async (msg) =>
            {
                var stock = await Yahoo.Symbols(_symbol).Fields(Field.Symbol, Field.Currency, Field.RegularMarketPrice).QueryAsync();
                var newStockPriceMessage = new NewStockPriceMessage(stock[_symbol]);
                _logger.Debug($"{newStockPriceMessage}");
                _socketConnectionRActor.Tell(newStockPriceMessage, Self);
            });

            Receive<UpdateStreamMessage>((updateStream) =>
            {
                _logger.Debug($"Updating {updateStream}");
                _interval = updateStream.Interval;
                _symbol = updateStream.Symbol;
                StartPeriodicTimer();
            });
        }
            
        public ITimerScheduler Timers { get; set; }

        protected override void PreStart()
        {
            StartPeriodicTimer();
        }

        private void StartPeriodicTimer() => Timers.StartPeriodicTimer("poll", new PollStockPriceMessage(), TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(_interval));

        public static Props Props(IActorRef socketConnectionRActor, string symbol, int interval) => Akka.Actor.Props.Create(() => new StockPricePollingActor(socketConnectionRActor, symbol, interval));
    }
}
