namespace Marketplace.Domain.Interface.Repository
{
    public interface ITokenRepository
    {
        Task<string> Create(string userId);
        Task<string?> GetUserId(string token);
        Task<string?> FindExistingTokenForUser(string userId);
    }
}
