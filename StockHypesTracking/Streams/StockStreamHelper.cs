using Akka.Streams.Dsl;
using Akka.Streams;
using Akka;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Streams
{
    public static class StockStreamHelper
    {
        public static IGraph<FlowShape<NewStockPriceMessage, NewStockPriceMessage>, NotUsed> BuildDataFlow()
        {
            return GraphDsl.Create(builder =>
            {
                var broadcast = builder.Add(new Broadcast<NewStockPriceMessage>(2));
                var merge = builder.Add(new Merge<NewStockPriceMessage>(2));

                builder.From(broadcast.Out(0)).To(merge.In(0));
                builder.From(broadcast.Out(1)).Via(BuildWindowFlow()).To(merge.In(1));

                return new FlowShape<NewStockPriceMessage, NewStockPriceMessage>(broadcast.In, merge.Out);
            }).Named("DataFlow");
        }

        public static Flow<T, T, NotUsed> LogFlow<T>(Func<T, string> formatter) => Flow.Create<T>().Select(data =>
        {
            Console.WriteLine(formatter(data));
            return data;
        });

        public static Flow<NewStockPriceMessage, NewStockPriceMessage, NotUsed> BuildWindowFlow() => Flow.Create<NewStockPriceMessage>().Sliding<NewStockPriceMessage, NewStockPriceMessage, NotUsed>(10, 10).Select(window =>
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            foreach (var stock in window)
            {
                min = stock.GetMarketPrice() < min ? stock.GetMarketPrice() : min;
                max = stock.GetMarketPrice() > max ? stock.GetMarketPrice() : max;
            }

            return new NewStockPriceMessage("AAPL", "USD", min == double.MaxValue ? 0 : max - min);
        }).Named("WindowFlow");
    }
}
