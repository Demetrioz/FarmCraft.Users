namespace FarmCraft.Users.Data.Entities
{
    public class Invitation
    {
        public string InvitationId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string InvitedBy { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
    }
}
