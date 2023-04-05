using Akka.Actor;
using Akka.Event;

namespace StockHypesTracking.Actors
{
    public class ApplicationTreeTraverse : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private List<string> _actors = new List<string>();
        private IActorRef _sender;

        public ApplicationTreeTraverse() 
        {
            // TODO Come up with better way.
            Task.Delay(2000).ContinueWith(t => { _sender.Tell(_actors); });


            Receive<string>(path =>
            {
                var actors = Context.System.ActorSelection($"{path}/*");
                actors.Tell(new Identify(Guid.NewGuid()), Self);

                _sender = Sender;
            });

            Receive<ActorIdentity>(identity =>
            {
                if (identity == null || identity.Subject == null)
                    return;

                var actorRef = identity.Subject;
                _logger.Debug($"Actor: {actorRef.Path.ToStringWithAddress()}");
                _actors.Add(actorRef.Path.ToStringWithAddress());
                Self.Tell(actorRef.Path);
            });

            Receive<ActorPath>(path =>
            {
                _logger.Debug($"Path: {path}");
                Context.ActorSelection($"{path}/*").Tell(new Identify(Guid.NewGuid()), Self);
            });
        }
    }
}
