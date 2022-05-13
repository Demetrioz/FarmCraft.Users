using Akka.Actor;
using FarmCraft.Users.Data.Messages.User;
using System;

namespace FarmCraft.Users.AADIntegrations.Actors
{
    public class IntegrationActor : ReceiveActor
    {
        private readonly ActorSelection _server = Context.ActorSelection(
            Environment.GetEnvironmentVariable("AkkaServer")
        );

        public IntegrationActor()
        {
            Receive<AskForUser>(message => ForwardToServer(message));
            Receive<AskToRegisterUser>(message => ForwardToServer(message));
        }

        private async void ForwardToServer<T>(T message)
        {
            var sender = Sender;
            var result = await _server.Ask(message);
            sender.Tell(result);
        }
    }
}
