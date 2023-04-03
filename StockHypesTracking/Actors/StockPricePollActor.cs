using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;
using YahooFinanceApi;

namespace StockHypesTracking.Actors
{
    public class StockPricePollActor : ReceiveActor, IWithTimers
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _socketConnectionRActor;
        private readonly string _symbol;
        private readonly int _interval;

        public StockPricePollActor(IActorRef socketConnectionRActor, string symbol, int interval)
        {
            _logger = Logging.GetLogger(Context);
            _socketConnectionRActor = socketConnectionRActor;
            _interval = interval;
            _symbol = symbol;
            Context.Watch(socketConnectionRActor);
            ReceiveAsync<PollStockPriceMessage>(async (msg) =>
            {
                var stock = await Yahoo.Symbols(_symbol).Fields(Field.Symbol, Field.Currency, Field.RegularMarketPrice).QueryAsync();
                var newStockPriceMessage = new NewStockPriceMessage(stock[_symbol]);
                _logger.Debug($"{newStockPriceMessage}");
                _socketConnectionRActor.Tell(newStockPriceMessage, Self);
            });

            Receive<Terminated>((terminated) =>
            {
                _logger.Debug($"Terminating.");
                Context.Stop(Self);
            });
        }
            
        public ITimerScheduler Timers { get; set; }

        protected override void PreStart()
        {
            Timers.StartPeriodicTimer("poll", new PollStockPriceMessage(), TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(_interval));
        }

        public static Props Props(IActorRef socketConnectionRActor, string symbol, int interval) => Akka.Actor.Props.Create(() => new StockPricePollActor(socketConnectionRActor, symbol, interval));
    }
}
