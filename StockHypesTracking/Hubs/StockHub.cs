using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.SignalR;
using StockHypesTracking.Actors;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Hubs
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Connection {Context.ConnectionId} closed.");
            var socketManagerARef = await _socketManagerARefProvider.GetAsync();
            socketManagerARef.Tell(new CloseConnectionMessage(Context.ConnectionId));
        }

        public class StartStreamModel 
        {
            public string Symbol { get; set; }

            public int Interval { get; set; }
        }

    }
}
