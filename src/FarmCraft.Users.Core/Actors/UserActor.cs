using Akka.Actor;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Core.Utilities;
using FarmCraft.Users.Data.Entities;
using FarmCraft.Users.Data.Messages.User;
using FarmCraft.Users.Data.Repositories.Invitation;
using FarmCraft.Users.Data.Repositories.Organization;
using FarmCraft.Users.Data.Repositories.User;

namespace FarmCraft.Users.Core.Actors
{
    public class UserActor : ReceiveActor
    {
        private readonly ILogService _logger;
        private readonly IUserRepository _userRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IInvitationRepository _inviteRepo;

        public UserActor(
            IUserRepository userRepo, 
            IOrganizationRepository orgRepo, 
            IInvitationRepository inviteRepo,
            ILogService logger
        )
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _inviteRepo = inviteRepo;
            _logger = logger;

            Receive<AskToInviteUser>(message => HandleUserInvitation(message));
            Receive<AskToRegisterUser>(message => HandleUserRegistration(message));
        }

        private void HandleUserInvitation(AskToInviteUser message)
        {
            IActorRef sender = Sender;
            IActorRef self = Self;
            string requestId = Guid.NewGuid().ToString();

            InviteUser(message)
                .ContinueWith(result =>
                {
                    if(result.Exception != null)
                    {
                        _logger.LogAsync(result.Exception).Wait();
                        sender.Tell(ActorResponse.Failure(requestId, result.Exception.Message));
                    }
                    else
                        sender.Tell(ActorResponse.Success(requestId, result.Result));
                });

            self.Tell(PoisonPill.Instance);
        }

        private void HandleUserRegistration(AskToRegisterUser message)
        {
            IActorRef sender = Sender;
            IActorRef self = Self;
            string requestId = Guid.NewGuid().ToString();

            RegisterUser(message)
                .ContinueWith(result =>
                {
                    if(result.Exception != null)
                    {
                        _logger.LogAsync(result.Exception).Wait();
                        sender.Tell(ActorResponse.Failure(requestId, result.Exception.Message));
                    }
                    else
                        sender.Tell(ActorResponse.Success(requestId, result.Result));
                });

            self.Tell(PoisonPill.Instance);
        }

        private async Task<User> RegisterUser(AskToRegisterUser message)
        {
            User? existingUser = await _userRepo.FindUserById(message.UserId);
            
            if (existingUser != null)
                throw new Exception($"Unable to register user with id {message.UserId}");

            string orgId = Guid.NewGuid().ToString();
            Invitation? invite = await _inviteRepo.FindInvitationById(message.InvitationId ?? "");

            User newUser = new User
            {
                AzureId = message.UserId,
                Preferences = new Preferences
                {
                    AlertPreference = AlertPreference.Email
                },

            };

            if(invite != null)
            {
                Organization? inviteOrg = await _orgRepo.FindOrganizationById(invite.InvitationOrgId);

                inviteOrg?.Members.Add(new PartialUser
                {
                    AzureId = message.UserId,
                });

                invite.RegistrationStatus = RegistrationStatus.Registered;
                newUser.Organizations = new List<PartialOrganization>
                {
                    new PartialOrganization
                    {
                        Id = invite.InvitationOrgId,
                        Name = invite.InvitationOrgName,
                        Role = invite.Role
                    }
                };
            }
            else
            {
                newUser.Organizations = new List<PartialOrganization>
                {
                    new PartialOrganization
                    {
                        Id = orgId,
                        Name = message.OrganizationName,
                        Role = Role.Owner
                    }
                };

                Organization newOrg = new Organization
                {
                    OrganizationId = orgId,
                    Name = message.OrganizationName,
                    OwnerId = message.UserId,
                    Members = new List<PartialUser>
                    {
                        new PartialUser
                        {
                            AzureId = message.UserId
                        }
                    }
                };

                await _orgRepo.CreateOrganization(newOrg);
            }

            User user = await _userRepo.CreateUser(newUser);
            return user;
        }

        private async Task<Invitation> InviteUser(AskToInviteUser message)
        {
            if (!RegexUtility.IsValidEmail(message.InvitedEmail))
                throw new ArgumentException("Invalid email");

            User? requestor = await _userRepo.FindUserById(message.RequestorId);
            if (requestor == null)
                throw new Exception("Requestor not found");

            PartialOrganization? requestorOrg = requestor.Organizations
                .Where(o => o.Id == message.OrganizationId)
                .FirstOrDefault();

            if (
                requestorOrg == null 
                || (requestorOrg.Role != Role.Owner && requestorOrg.Role != Role.Admin)
            )
                throw new Exception("Insufficient permissions");

            Invitation invite = new Invitation
            {
                InvitationId = Guid.NewGuid().ToString(),
                InvitationOrgId = requestorOrg.Id,
                InvitationOrgName = requestorOrg.Name,
                Email = message.InvitedEmail,
                InvitedBy = requestor.AzureId,
                RegistrationStatus = RegistrationStatus.Invited,
                Role = message.InvitedRole
            };

            // TODO: Send email to user
            // Publish to Service Bus -> Receive by Notification Service -> Sent to Sendgrid

            var result = await _inviteRepo.CreateInvitation(invite);
            return result;
        }
    }
}
