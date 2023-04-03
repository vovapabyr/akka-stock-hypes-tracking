using Akka.Routing;

namespace StockHypesTracking.Messsages
{
    public class UpdateStreamMessage : IConsistentHashable
    {
        public string Id { get; }

        public string Symbol { get; }

        public int Interval { get; }

        public object ConsistentHashKey => Id;

        public UpdateStreamMessage(string symbol, int interval, string id)
        {
            Symbol = symbol;
            Interval = interval;
            Id = id;
        }

        public override string ToString() => $"Id: {Id}, Symbol: {Symbol}, Interval: {Interval}";
    }
}
