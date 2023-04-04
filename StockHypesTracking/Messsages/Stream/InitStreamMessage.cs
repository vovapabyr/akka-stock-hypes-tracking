using Akka.Actor;

namespace StockHypesTracking.Messsages
{
    public class InitStreamMessage
    {
        public IActorRef StreamMediatorRActor { get; private set; }

        public InitStreamMessage(IActorRef streamMediatorRActor)
        {
            StreamMediatorRActor = streamMediatorRActor;
        }
    }
}
