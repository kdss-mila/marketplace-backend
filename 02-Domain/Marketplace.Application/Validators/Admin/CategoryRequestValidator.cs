using FluentValidation;
using Marketplace.Application.DTOs.Admin;

namespace Marketplace.Application.Validators.Admin
{
    public sealed class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nome obrigatório");
        }
    }
}
