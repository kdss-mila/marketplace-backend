using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Repository
{
    public interface IRepasseRepository
    {
        Task<IEnumerable<RepasseModel>> GetAll();
        Task<RepasseModel?> GetById(string id);
        Task Add(RepasseModel repasse);
        Task Update(RepasseModel repasse);
    }
}
