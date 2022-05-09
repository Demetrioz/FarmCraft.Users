using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Data.Context;

namespace FarmCraft.Users.Data.Repositories.Organization
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ILogService _logger;
        private readonly UserContext? _dbContext;

        public OrganizationRepository(IFarmCraftContext dbContext, ILogService logger)
        {
            _logger = logger;
            _dbContext = dbContext as UserContext;

            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            if (_logger == null)
                throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<Entities.Organization> CreateOrganization(Entities.Organization org)
        {
            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            Entities.Organization newOrg = (await _dbContext.Organizations.AddAsync(org)).Entity;
            await _dbContext.SaveChangesAsync();

            return newOrg;
        }
    }
}
