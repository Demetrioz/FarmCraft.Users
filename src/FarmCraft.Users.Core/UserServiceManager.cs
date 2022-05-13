using FarmCraft.Core.Actors;
using FarmCraft.Core.Messages;
using FarmCraft.Users.Core.Actors;
using FarmCraft.Users.Data.Messages.User;

namespace FarmCraft.Users.Core
{
    public class UserServiceManager : FarmCraftManager
    {
        //private readonly IActorRef _graphManager;

        public UserServiceManager(IServiceProvider provider)
        {
            Receive<FarmCraftMessage>(message => HandleMessage(message));
            Receive<IUserMessage>(message => HandleWithInstanceOf<UserActor>(message));
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
