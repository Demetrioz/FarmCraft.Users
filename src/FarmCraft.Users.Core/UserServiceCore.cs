using Akka.Actor;
using FarmCraft.Core.Actors;

namespace FarmCraft.Users.Core
{
    public class UserServiceCore : FarmCraftCore<UserServiceManager>
    {
        public UserServiceCore(IServiceProvider provider) : base(provider)
        {
        }
    }
}