using Akka.Streams.Dsl;
using Akka.Streams;
using Akka;
using StockHypesTracking.Messsages;
using Flurl;
using Flurl.Http;
using HtmlAgilityPack;

namespace StockHypesTracking.Streams
{
    public static class StockStreamProcessingBuilder
    {
        public const int StocksWindowCapacity = 10;
        public const int StocksWindowSparseIndex = 3; // ex. take every third item into stocks window

        public static IGraph<FlowShape<NewStockPriceMessage, NewStockPriceMessage>, NotUsed> BuildStockProcessingFlow()
        {
            return GraphDsl.Create(builder =>
            {
                var broadcast = builder.Add(new Broadcast<NewStockPriceMessage>(2));
                var merge = builder.Add(new Merge<NewStockPriceMessage>(2));

                // source -> broadcast -> merge
                builder.From(broadcast.Out(0)).To(merge.In(0));
                // source -> broadcast -> stockHypeDetectionFlow -> merge
                builder.From(broadcast.Out(1)).Via(BuildStockHypeDetectionFlow()).To(merge.In(1));

                return new FlowShape<NewStockPriceMessage, NewStockPriceMessage>(broadcast.In, merge.Out);
            }).Named("StockFlow");
        }

        public static IGraph<FlowShape<NewStockPriceMessage, NewStockPriceMessage>, NotUsed> BuildStockHypeDetectionFlow() => Flow.Create<NewStockPriceMessage>()
            .ZipWithIndex()
            .Collect(t => (1 + t.Item2) % StocksWindowSparseIndex == 0, t => t.Item1)
            .Sliding(StocksWindowCapacity, StocksWindowCapacity - 2) // make the last item of previous window as the second item in new window to test for extremum
            .Select(stocksWindow => FindStockPriceExtremums(stocksWindow.ToList()))
            .SelectMany(s => s)
            .SelectAsync(StocksWindowCapacity - 2, async s => 
            {
                return new NewStockPriceMessage(s) { Data = new { Price = s.Data, News = await GetTopRecentNewsAsync(s.Symbol) } };
            })
            .Named("StockHypeDetectionFlow");


        private static IEnumerable<NewStockPriceMessage> FindStockPriceExtremums(IList<NewStockPriceMessage> stockPrices) 
        {
            if (stockPrices.Count <= 2)
                yield break;

            // Yield only extremums
            if ((stockPrices[0].GetMarketPrice() < stockPrices[1].GetMarketPrice() && stockPrices[1].GetMarketPrice() > stockPrices[2].GetMarketPrice()) // max
                || (stockPrices[0].GetMarketPrice() > stockPrices[1].GetMarketPrice() && stockPrices[1].GetMarketPrice() < stockPrices[2].GetMarketPrice())) // min
                yield return stockPrices[1];

            foreach (var stockPriceMessage in FindStockPriceExtremums(stockPrices.Skip(1).ToList()))
                yield return stockPriceMessage;
        }

        private static async Task<object> GetTopRecentNewsAsync(string symbol) 
        {
            var newsString = await "https://finance.yahoo.com/quote/".AppendPathSegment(symbol).GetStringAsync();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(newsString);

            var link = document.DocumentNode
                .SelectSingleNode("//div[@id='mrt-node-quoteNewsStream-0-Stream']//.//li[starts-with(@class, 'js-stream-content')]//.//a[starts-with(@class, 'js-content-viewer')]");

            return new { Title = link.InnerText, Link = $"https://finance.yahoo.com{link.GetAttributeValue<string>("href", string.Empty)}" };
        }
    }
}
