using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Data.Context;

namespace FarmCraft.Users.Data.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogService _logger;
        private readonly UserContext? _dbContext;

        public UserRepository(IFarmCraftContext dbContext, ILogService logger)
        {
            _logger = logger;
            _dbContext = dbContext as UserContext;

            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            if (_logger == null)
                throw new ArgumentNullException(nameof(_logger));
        }

        public Task<Entities.User?> FindUserById(string id)
        {
            return null;
        }

        public async Task<Entities.User> CreateUser(Entities.User user)
        {
            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            Entities.User newUser = (await _dbContext.Users.AddAsync(user)).Entity;
            await _dbContext.SaveChangesAsync();

            return newUser;
        }
    }
}
