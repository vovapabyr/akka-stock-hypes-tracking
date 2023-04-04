using Akka.Actor;
using Akka.Event;
using StockHypesTracking.Messsages;
using YahooFinanceApi;

namespace StockHypesTracking.Actors
{
    public class StockPricePolling : ReceiveActor, IWithTimers
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private string _symbol;
        private int _interval;

        public StockPricePolling(string symbol, int interval)
        {
            _interval = interval;
            _symbol = symbol;

            ReceiveAsync<PollStockPriceMessage>(async (msg) =>
            {
                var stock = await Yahoo.Symbols(_symbol).Fields(Field.Symbol, Field.Currency, Field.RegularMarketPrice).QueryAsync();
                var newStockPriceMessage = new NewStockPriceMessage(stock[_symbol]);
                _logger.Debug($"New stock '{newStockPriceMessage}'");
                Context.Parent.Tell(newStockPriceMessage);
            });
        }
            
        public ITimerScheduler Timers { get; set; }

        protected override void PreStart()
        {
            Timers.StartPeriodicTimer("poll", new PollStockPriceMessage(), TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(_interval));
        }

        public static Props Props(string symbol, int interval) => Akka.Actor.Props.Create(() => new StockPricePolling(symbol, interval));
    }
}
