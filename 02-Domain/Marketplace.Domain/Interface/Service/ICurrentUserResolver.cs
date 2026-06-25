using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Service
{
    public interface ICurrentUserResolver
    {
        Task<UserModel?> GetCurrent();
    }
}
