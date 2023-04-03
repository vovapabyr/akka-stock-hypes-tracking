using Akka.Actor;

internal class StockStreamingService : BackgroundService
{
    private readonly ActorSystem _actorSystem;

    public StockStreamingService(ActorSystem actorSystem) 
    {
        _actorSystem = actorSystem;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //var reqActorRef = await _signalRStockDispatcherARef.GetAsync();

        //await Source.Tick(TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(5), () =>
        //{
        //    return Yahoo.Symbols("AAPL").Fields(Field.Symbol, Field.Currency, Field.RegularMarketPrice).QueryAsync();
        //}).SelectAsync(1, getStock =>
        //{
        //    return getStock();

        //}).Select(stock => 
        //{
        //    var stockValues = stock["AAPL"];
        //    return new StockModel(stockValues);
        //}).Via(StockStreamHelper.BuildDataFlow())
        //.Via(StockStreamHelper.LogFlow<StockModel>((a) => a.ToString()))
        //.SelectAsync(1, (stock) => reqActorRef.Ask<StockModel>(stock)).RunWith(Sink.Ignore<StockModel>(), _actorSystem.Materializer(ActorMaterializerSettings.Create(_actorSystem)
        //    .WithSupervisionStrategy(ex => Akka.Streams.Supervision.Directive.Resume).WithDebugLogging(true)));
    }
}