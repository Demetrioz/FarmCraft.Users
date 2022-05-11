namespace FarmCraft.Users.Data.Repositories.Organization
{
    public interface IOrganizationRepository
    {
        Task<Entities.Organization?> FindOrganizationById(string id);
        Task<Entities.Organization> CreateOrganization(Entities.Organization org);
    }
}
