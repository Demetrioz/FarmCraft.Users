namespace FarmCraft.Users.Data.Messages.User
{
    public class AskToRegisterUser : IUserMessage
    {
        public string UserId { get; set; }
        public string OrganizationName { get; set; }
        public string? InvitationId { get; set; }

        public AskToRegisterUser(string userId, string orgName, string? inviteId = null)
        {
            UserId = userId;
            OrganizationName = orgName;
            InvitationId = inviteId;
        }
    }
}
