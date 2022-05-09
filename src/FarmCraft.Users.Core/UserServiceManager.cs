using Akka.Actor;
using Akka.DependencyInjection;
using FarmCraft.Core.Actors;
using FarmCraft.Core.Messages;
using FarmCraft.Users.Core.Actors;

namespace FarmCraft.Users.Core
{
    public class UserServiceManager : FarmCraftManager
    {
        private readonly IActorRef _graphManager;

        public UserServiceManager(IServiceProvider provider)
        {
            Props props = DependencyResolver.For(Context.System).Props<GraphManager>();
            _graphManager = Context.ActorOf(props, "GraphManager");

            Receive<FarmCraftMessage>(message => HandleMessage(message));
        }

        private void HandleMessage(FarmCraftMessage message)
        {
            switch(message.MessageType)
            {
                default:
                    break;
            }
        }
    }
}
