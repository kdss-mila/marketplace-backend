using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class ListAdminCategoriesUseCase(
        ICurrentUserResolver currentUser,
        ICategoryRepository categoryRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<IEnumerable<CategoryModel>> Execute()
        {
            await AdminGuard.RequireAdmin(_currentUser);
            return await _categoryRepository.GetAll();
        }
    }
}
