using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FarmCraft.Users.Data.Repositories.Invitation
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly ILogService _logger;
        private readonly UserContext? _dbContext;

        public InvitationRepository(IFarmCraftContext dbContext, ILogService logger)
        {
            _logger = logger;
            _dbContext = dbContext as UserContext;

            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            if (_logger == null)
                throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<Entities.Invitation?> FindInvitationById(string id)
        {
            if(_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            return await _dbContext.Invitations
                .Where(i => i.InvitationId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Entities.Invitation> CreateInvitation(Entities.Invitation invitation)
        {
            if (_dbContext == null)
                throw new ArgumentNullException(nameof(_dbContext));

            Entities.Invitation newInvitation = (await _dbContext.AddAsync(invitation)).Entity;
            await _dbContext.SaveChangesAsync();

            return newInvitation;
        }
    }
}
