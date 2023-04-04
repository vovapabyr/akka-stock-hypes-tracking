using Akka.Actor;
using Akka.Hosting;
using StockHypesTracking.Actors;
using StockHypesTracking.Hubs;

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
            var rootSupervisorProps = Props.Create<StockHypesSupervisor>(registry);
            var rootSupervisor = system.ActorOf(rootSupervisorProps, "stock-hypes-supervisor");
            registry.Register<StockHypesSupervisor>(rootSupervisor);
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
