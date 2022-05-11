using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Data.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Entities.User?> FindUserById(string id)
        {
            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            return await _dbContext.Users
                .Where(u => u.AzureId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Entities.User> CreateUser(Entities.User user)
        {
            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            Entities.User newUser = (await _dbContext.AddAsync(user)).Entity;
            await _dbContext.SaveChangesAsync();

            return newUser;
        }
    }
}
