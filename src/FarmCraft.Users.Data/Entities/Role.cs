using System.ComponentModel.DataAnnotations;

namespace FarmCraft.Users.Data.Entities
{
    /// <summary>
    /// Describes a role that a FarmCraft user can have
    /// within an organization
    /// </summary>
    public enum Role
    {
        Owner,
        Admin,
        Member,
        Guest
    }
}
