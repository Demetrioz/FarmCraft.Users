namespace FarmCraft.Users.Data.Messages.User
{
    public class AskForUser : IUserMessage
    {
        public string UserId { get; private set; }

        public AskForUser(string userId)
        {
            UserId = userId;
        }
    }
}
