using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetAll();
        Task<UserModel?> GetById(string id);
        Task<UserModel?> GetByEmail(string email);
        Task Add(UserModel user);
        Task Update(UserModel user);
    }
}
