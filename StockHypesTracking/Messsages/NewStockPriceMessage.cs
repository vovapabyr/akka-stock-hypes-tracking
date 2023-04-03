using YahooFinanceApi;

namespace StockHypesTracking.Messsages
{
    public class NewStockPriceMessage
    {
        public NewStockPriceMessage(Security yahooStockModel)
        {
            Symbol = yahooStockModel.Symbol;
            Currency = yahooStockModel.Currency;
            Data = yahooStockModel.RegularMarketPrice;
        }

        public NewStockPriceMessage(string symbol, string currency, object data)
        {
            Symbol = symbol;
            Currency = currency;
            Data = data;
        }

        public string Symbol { get; set; }

        public string Currency { get; set; }

        public object Data { get; set; }

        public double GetMarketPrice() => (double)Data;

        public override string ToString() => $"Symbol: {Symbol}, Currency: {Currency}, Data: {Data}";
    }
}
