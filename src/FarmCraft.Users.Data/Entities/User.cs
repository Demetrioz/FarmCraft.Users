using System.ComponentModel.DataAnnotations;

namespace FarmCraft.Users.Data.Entities
{
    public class User
    {
        [Key]
        public string AzureId { get; set; }
        public Preferences Preferences { get; set; }
        public ICollection<PartialOrganization> Organizations { get; set; }
    }
}
