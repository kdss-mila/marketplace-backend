using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> GetAll();
        Task<CategoryModel?> GetById(string id);
        Task Add(CategoryModel category);
        Task Update(CategoryModel category);
        Task Delete(string id);
    }
}
