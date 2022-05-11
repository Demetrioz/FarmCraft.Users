namespace FarmCraft.Users.Data.Repositories.Invitation
{
    public interface IInvitationRepository
    {
        Task<Entities.Invitation?> FindInvitationById(string id);
        Task<Entities.Invitation> CreateInvitation(Entities.Invitation invitation);
    }
}
