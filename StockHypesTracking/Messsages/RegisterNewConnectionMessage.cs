using Akka.Routing;

namespace StockHypesTracking.Web.Messsages
{
    public class RegisterNewConnectionMessage : IConsistentHashable
    {
        public string Id { get; }

        public string Symbol { get; }

        public int Interval { get; }

        public object ConsistentHashKey => Id;

        public RegisterNewConnectionMessage(string symbol, int interval, string connectionId)
        {
            Symbol = symbol;
            Interval = interval;
            Id = connectionId;
        }

        public override string ToString() => $"Id: {Id}, Symbol: {Symbol}, Interval: {Interval}";
    }
}
