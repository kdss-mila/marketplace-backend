using FluentValidation;
using Marketplace.Application.DTOs.Seller;

namespace Marketplace.Application.Validators.Seller
{
    public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Título obrigatório");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Descrição obrigatória");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Preço deve ser maior que zero");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Estoque inválido");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Categoria obrigatória");
            RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Peso deve ser maior que zero");
            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Pelo menos uma imagem é obrigatória.")
                .Must(imgs => imgs.All(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)))
                .WithMessage("Todas as imagens devem ser URLs válidas.");
        }
    }
}
