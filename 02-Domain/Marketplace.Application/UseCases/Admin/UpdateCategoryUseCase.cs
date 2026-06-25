using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Admin;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class UpdateCategoryUseCase(
        ICurrentUserResolver currentUser,
        ICategoryRepository categoryRepository,
        IValidator<CategoryRequest> validator)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IValidator<CategoryRequest> _validator = validator;

        public async Task<CategoryModel> Execute(string categoryId, CategoryRequest request)
        {
            await AdminGuard.RequireAdmin(_currentUser);
            await ValidatorRunner.EnsureValid(_validator, request);

            var category = await _categoryRepository.GetById(categoryId)
                ?? throw new NotFoundException("Categoria não encontrada");

            category.Name = request.Name;
            category.ParentId = request.ParentId;

            await _categoryRepository.Update(category);
            return category;
        }
    }
}
