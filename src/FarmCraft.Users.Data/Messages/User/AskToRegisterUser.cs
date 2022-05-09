namespace FarmCraft.Users.Data.Messages.User
{
    public class AskToRegisterUser : IUserMessage
    {
        public string UserId { get; set; }
        public string OrganizationName { get; set; }

        public AskToRegisterUser(string userId, string orgName)
        {
            UserId = userId;
            OrganizationName = orgName;
        }
    }
}
