using Akka.Actor;
using Akka.Hosting;
using Microsoft.AspNetCore.SignalR;
using StockHypesTracking.Actors;
using StockHypesTracking.Messsages;

namespace StockHypesTracking.Hubs
{
    public class StockHub : Hub
    {
        private readonly IRequiredActor<SocketConnectionsManager> _socketManagerARefProvider;
        private readonly ILogger<StockHub> _logger;

        public StockHub(IRequiredActor<SocketConnectionsManager> socketManagerARefProvider, ILogger<StockHub> logger)
        {
            _socketManagerARefProvider = socketManagerARefProvider;
            _logger = logger;
        }

        public async Task StartStream(StreamModel startStreamModel) 
        {
            var socketManagerARef = await _socketManagerARefProvider.GetAsync();
            var newConnectionMessage = new RegisterNewConnectionMessage(startStreamModel.Symbol, startStreamModel.Interval, Context.ConnectionId);
            _logger.LogInformation($"Starting new stream {newConnectionMessage}");
            socketManagerARef.Tell(newConnectionMessage);
        }

        public async Task UpdateStream(StreamModel updateStreamModel)
        {
            var socketManagerARef = await _socketManagerARefProvider.GetAsync();
            var updateStreamMessage = new UpdateStreamMessage(updateStreamModel.Symbol, updateStreamModel.Interval, Context.ConnectionId);
            _logger.LogInformation($"Updating stream {updateStreamMessage}");
            socketManagerARef.Tell(updateStreamMessage);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Connection {Context.ConnectionId} closed");
            var socketManagerARef = await _socketManagerARefProvider.GetAsync();
            socketManagerARef.Tell(new CloseConnectionMessage(Context.ConnectionId));
        }

        public class StreamModel 
        {
            public string Symbol { get; set; }

            public int Interval { get; set; }
        }
    }
}
