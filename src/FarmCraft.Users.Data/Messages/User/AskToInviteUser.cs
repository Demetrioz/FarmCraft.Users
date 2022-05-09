namespace FarmCraft.Users.Data.Messages.User
{
    public class AskToInviteUser : IUserMessage
    {
        public string RequestorId { get; private set; }
        public string OrganizationId { get; private set; }


        public AskToInviteUser(string requestorId, string orgId)
        {
            RequestorId = requestorId;
            OrganizationId = orgId;
        }
    }
}
