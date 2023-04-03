using Akka.Actor;
using Akka.Hosting;
using Akka.Routing;
using StockHypesTracking.Web.Actors;
using StockHypesTracking.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<StockHubService>();
builder.Services.AddAkka("stock-hypes", (builder, provider) =>
{
    builder
        .ConfigureLoggers((conf) =>
        {
            conf.LogLevel = Akka.Event.LogLevel.DebugLevel;
        })
        .WithActors((system, registry) =>
        {
            var pollingRouterProps = new ConsistentHashingPool(5).Props(Props.Create<StockPricePollingManager>());
            var pollingRouterRActor = system.ActorOf(pollingRouterProps, "polling-router");

            var connectionsRouterProps = new ConsistentHashingPool(5).Props(SocketConnectionsManagerActor.Props(pollingRouterRActor));
            var socketConnectionsRouterRActor = system.ActorOf(connectionsRouterProps, "connections-router");
            registry.Register<SocketConnectionsManagerActor>(socketConnectionsRouterRActor);
        });
});

//builder.Services.AddHostedService<StockStreamingService>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();
app.UseEndpoints(conf => 
{
    conf.MapControllers();
    conf.MapHub<StockHub>("/stockHub");
});

app.Run();
