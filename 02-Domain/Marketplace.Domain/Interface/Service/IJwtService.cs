using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Service
{
    public interface IJwtService
    {
        string GenerateToken(UserModel user);
    }
}
