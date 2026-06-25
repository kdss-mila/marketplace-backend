using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Application.UseCases.Admin
{
    public class DeleteCategoryUseCase(
        ICurrentUserResolver currentUser,
        ICategoryRepository categoryRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task Execute(string categoryId)
        {
            await AdminGuard.RequireAdmin(_currentUser);

            var category = await _categoryRepository.GetById(categoryId)
                ?? throw new NotFoundException("Categoria não encontrada");

            await _categoryRepository.Delete(category.Id);
        }
    }
}
