using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.SignalR;
using StockHypesTracking.Web.Actors;
using StockHypesTracking.Web.Messsages;

namespace StockHypesTracking.Web.Hubs
{
    public class StockHub : Hub
    {
        private readonly IRequiredActor<SocketConnectionsManagerActor> _socketManagerARefProvider;
        private readonly ILogger<StockHub> _logger;

        public StockHub(IRequiredActor<SocketConnectionsManagerActor> socketManagerARefProvider, ILogger<StockHub> logger)
        {
            _socketManagerARefProvider = socketManagerARefProvider;
            _logger = logger;
        }

        public async Task StartStream(StartStreamModel startStreamModel) 
        {
            var socketManagerARef = await _socketManagerARefProvider.GetAsync();
            var newConnectionMessage = new RegisterNewConnectionMessage(startStreamModel.Symbol, startStreamModel.Interval, Context.ConnectionId);
            _logger.LogInformation($"Starting new stream: {newConnectionMessage}");
            socketManagerARef.Tell(newConnectionMessage);
        }

        public class StartStreamModel 
        {
            public string Symbol { get; set; }

            public int Interval { get; set; }
        }

    }
}
