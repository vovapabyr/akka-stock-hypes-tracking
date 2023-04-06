using YahooFinanceApi;

namespace StockHypesTracking.Messsages
{
    public class NewStockPriceMessage
    {
        public NewStockPriceMessage(Security yahooStockModel) : this(yahooStockModel.Symbol, yahooStockModel.Currency, yahooStockModel.RegularMarketTime, yahooStockModel.RegularMarketPrice)
        {
        }

        public NewStockPriceMessage(NewStockPriceMessage newStockPrice) : this(newStockPrice.Symbol, newStockPrice.Currency, newStockPrice.Time, newStockPrice.Data) 
        {
        }

        public NewStockPriceMessage(string symbol, string currency, long time, object data)
        {
            Symbol = symbol;
            Currency = currency;
            Time = time;
            Data = data;
        }

        public string Symbol { get; set; }

        public string Currency { get; set; }

        public long Time { get; set; }

        public object Data { get; set; }

        public double GetMarketPrice() => (double)Data;

        public override string ToString() => $"Symbol: {Symbol}, Currency: {Currency}, Data: {Data}";
    }
}
