using Microsoft.AspNetCore.SignalR;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Hubs
{
    public class StockHubService
    {
        private readonly IHubContext<StockHub> _stockHub;
        private readonly ILogger<StockHubService> _logger;

        public StockHubService(IHubContext<StockHub> stockHub, ILogger<StockHubService> logger)
        {
            _stockHub = stockHub;
            _logger = logger;
        }

        public async Task PushNewStockAsync(NewStockPriceMessage newStockPrice, string connectionId)
        {
            _logger.LogDebug($"New stock '{newStockPrice} 'for client '{connectionId}'");
            await _stockHub.Clients.Client(connectionId).SendAsync("newStock", newStockPrice);
        }
    }
}
