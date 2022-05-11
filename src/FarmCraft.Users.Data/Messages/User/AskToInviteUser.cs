using FarmCraft.Users.Data.Entities;

namespace FarmCraft.Users.Data.Messages.User
{
    public class AskToInviteUser : IUserMessage
    {
        public string RequestorId { get; private set; }
        public string OrganizationId { get; private set; }
        public string InvitedEmail { get; private set; }
        public Role InvitedRole { get; private set; }


        public AskToInviteUser(
            string requestorId, 
            string orgId, 
            string inviteEmail, 
            Role inviteRole
        )
        {
            RequestorId = requestorId;
            OrganizationId = orgId;
            InvitedEmail = inviteEmail;
            InvitedRole = inviteRole;
        }
    }
}
