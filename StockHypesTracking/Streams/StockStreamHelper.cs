using Akka.Streams.Dsl;
using Akka.Streams;
using Akka;

namespace StockHypesTracking.Web.Streams
{
    //public static class StockStreamHelper
    //{
    //    public static IGraph<FlowShape<StockModel, StockModel>, NotUsed> BuildDataFlow()
    //    {
    //        return GraphDsl.Create(builder =>
    //        {
    //            var broadcast = builder.Add(new Broadcast<StockModel>(2));
    //            var merge = builder.Add(new Merge<StockModel>(2));

    //            builder.From(broadcast.Out(0)).To(merge.In(0));
    //            builder.From(broadcast.Out(1)).Via(BuildWindowFlow()).To(merge.In(1));

    //            return new FlowShape<StockModel, StockModel>(broadcast.In, merge.Out);
    //        }).Named("DataFlow");
    //    }

    //    public static Flow<T, T, NotUsed> LogFlow<T>(Func<T, string> formatter) => Flow.Create<T>().Select(data =>
    //    {
    //        Console.WriteLine(formatter(data));
    //        return data;
    //    });

    //    public static Flow<StockModel, StockModel, NotUsed> BuildWindowFlow() => Flow.Create<StockModel>().Sliding<StockModel, StockModel, NotUsed>(10, 10).Select(window =>
    //    {
    //        var min = double.MaxValue;
    //        var max = double.MinValue;
    //        foreach (var stock in window)
    //        {
    //            min = stock.GetMarketPrice() < min ? stock.GetMarketPrice() : min;
    //            max = stock.GetMarketPrice() > max ? stock.GetMarketPrice() : max;
    //        }

    //        return new StockModel("AAPL", "USD", min == double.MaxValue ? 0 : max - min);
    //    }).Named("WindowFlow");
    //}
}
