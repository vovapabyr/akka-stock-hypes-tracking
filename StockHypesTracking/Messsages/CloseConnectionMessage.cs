using Akka.Routing;

namespace StockHypesTracking.Messsages
{
    public class CloseConnectionMessage : IConsistentHashable
    {
        public string Id { get; set; }

        public object ConsistentHashKey => Id;

        public CloseConnectionMessage(string id)
        {
            Id = id;
        }
    }
}
