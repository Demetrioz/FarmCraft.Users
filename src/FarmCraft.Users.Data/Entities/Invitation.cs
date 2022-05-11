using System.ComponentModel.DataAnnotations;

namespace FarmCraft.Users.Data.Entities
{
    public class Invitation
    {
        [Key]
        public string InvitationId { get; set; }
        public string InvitationOrgId { get; set; }
        public string InvitationOrgName { get; set; }
        public string? Email { get; set; }
        public string InvitedBy { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public Role Role { get; set; }
    }
}
