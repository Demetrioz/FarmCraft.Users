using Akka.Actor;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Data.Entities;
using FarmCraft.Users.Data.Messages.User;
using FarmCraft.Users.Data.Repositories.Organization;
using FarmCraft.Users.Data.Repositories.User;

namespace FarmCraft.Users.Core.Actors
{
    public class UserActor : ReceiveActor
    {
        private readonly ILogService _logger;
        private readonly IUserRepository _userRepo;
        private readonly IOrganizationRepository _orgRepo;

        public UserActor(
            IUserRepository userRepo, 
            IOrganizationRepository orgRepo, 
            ILogService logger
        )
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _logger = logger;

            Receive<AskToInviteUser>(message => HandleUserCreation(message));
            Receive<AskToRegisterUser>(message => HandleUserRegistration(message));
        }

        private void HandleUserRegistration(AskToRegisterUser message)
        {
            IActorRef sender = Sender;
            IActorRef self = Self;
            string requestId = Guid.NewGuid().ToString();

            try
            {
                RegisterUser(message.UserId, message.OrganizationName)
                    .ContinueWith(result =>
                    {
                        sender.Tell(ActorResponse.Success(requestId, result.Result));
                    });
            }
            catch (Exception ex)
            {
                _logger.LogAsync(ex).Wait();
                sender.Tell(ActorResponse.Failure(requestId, ex.Message));
            }
            finally
            {
                self.Tell(PoisonPill.Instance);
            }
        }

        private void HandleUserCreation(AskToInviteUser message)
        {
            IActorRef sender = Sender;
            IActorRef self = Self;
            string requestId = Guid.NewGuid().ToString();

            try
            {
                CreateUser(message.UserId, message.OrganizationName)
                    .ContinueWith(result =>
                    {
                        sender.Tell(ActorResponse.Success(requestId, result.Result));
                    });
            }
            catch (Exception ex)
            {
                _logger.LogAsync(ex).Wait();
                sender.Tell(ActorResponse.Failure(requestId, ex.Message));
            }
            finally
            {
                self.Tell(PoisonPill.Instance);
            }
        }

        private async Task<User> RegisterUser(string userId, string organizationName)
        {
            User? existingUser = await _userRepo.FindUserById(userId);
            
            if (existingUser != null)
                throw new Exception($"Unable to register user with id {userId}");

            string orgId = Guid.NewGuid().ToString();

            User newUser = new User
            {
                AzureId = userId,
                Preferences = new Preferences
                {
                    AlertPreference = AlertPreference.Email
                },
                Organizations = new List<PartialOrganization>
                {
                    new PartialOrganization
                    {
                        Id = orgId,
                        Name = organizationName,
                        Role = Role.Owner
                    }
                }
            };

            Organization newOrg = new Organization
            {
                OrganizationId = orgId,
                Name = organizationName,
                OwnerId = userId,
                Members = new List<PartialUser>
                {
                    new PartialUser
                    {
                        AzureId = userId
                    }
                }
            };

            var userRequest = _userRepo.CreateUser(newUser);
            var taskRequest = _orgRepo.CreateOrganization(newOrg);

            await Task.WhenAll(userRequest, taskRequest);

            return userRequest.Result;
        }

        private async Task<User> CreateUser(string userId, string organizationId)
        {
            User? existingUser = await _userRepo.FindUserById(userId);

            if (existingUser != null)
                throw new Exception($"Unable to register user with id {userId}");



            // User exists, but has only been invited
            else if (existingUser != null && existingUser.RegistrationStatus == RegistrationStatus.Invited)
            {
                existingUser.AzureId = userId;
                existingUser.RegistrationStatus = RegistrationStatus.Registered;
            }

            // User doesn't exist
            else
            {

            }

            User newUser = new User
            {
                AzureId = userId,
                RegistrationStatus
            };

            return newUser;
        }
    }
}
