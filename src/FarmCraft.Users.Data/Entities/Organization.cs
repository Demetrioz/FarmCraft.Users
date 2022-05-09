using System.ComponentModel.DataAnnotations;

namespace FarmCraft.Users.Data.Entities
{
    public class Organization
    {
        [Key]
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public ICollection<PartialUser> Members { get; set; }
    }
}
