using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using StockHypesTracking.Web.Messsages;
using YahooFinanceApi;

namespace StockHypesTracking.Web.Actors
{
    public class StockPricePollActor : ReceiveActor, IWithTimers
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _socketConnectionRActor;
        private readonly string _symbol;
        private readonly int _interval;
        private readonly string _connectionId;

        public StockPricePollActor(IActorRef socketConnectionRActor, string symbol, int interval)
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
        }
            
        public ITimerScheduler Timers { get; set; }

        protected override void PreStart()
        {
            Timers.StartPeriodicTimer("poll", new PollStockPriceMessage(), TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(_interval));
        }

        public static Props Props(IActorRef socketConnectionRActor, string symbol, int interval) => Akka.Actor.Props.Create(() => new StockPricePollActor(socketConnectionRActor, symbol, interval));
    }
}
