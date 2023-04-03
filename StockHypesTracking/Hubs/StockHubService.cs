using Microsoft.AspNetCore.SignalR;

namespace StockHypesTracking.Web.Hubs
{
    public class StockHubService
    {
        private readonly IHubContext<StockHub> _stockHub;

        public StockHubService(IHubContext<StockHub> stockHub)
        {
            _stockHub = stockHub;
        }

        public void PushStock(string message)
        {
            _stockHub.Clients.All.SendAsync("newStock", message);
        }
    }
}
