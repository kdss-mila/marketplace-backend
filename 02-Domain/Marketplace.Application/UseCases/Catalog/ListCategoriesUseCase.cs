using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Catalog
{
    public class ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public Task<IEnumerable<CategoryModel>> Execute() => _categoryRepository.GetAll();
    }
}
