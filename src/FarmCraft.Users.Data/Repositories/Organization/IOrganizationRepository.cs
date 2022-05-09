namespace FarmCraft.Users.Data.Repositories.Organization
{
    public interface IOrganizationRepository
    {
        Task<Entities.Organization> CreateOrganization(Entities.Organization org);
    }
}
