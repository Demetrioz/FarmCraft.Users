namespace FarmCraft.Users.Data.Repositories.User
{
    public interface IUserRepository
    {
        Task<Entities.User?> FindUserById(string id);
        Task<Entities.User> CreateUser(Entities.User user);
    }
}
