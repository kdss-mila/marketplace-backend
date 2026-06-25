using FluentValidation;
using Marketplace.Application.DTOs.Seller;

namespace Marketplace.Application.Validators.Seller
{
    public sealed class SellerOnboardingRequestValidator : AbstractValidator<SellerOnboardingRequest>
    {
        public SellerOnboardingRequestValidator()
        {
            RuleFor(x => x.Document).NotEmpty().WithMessage("Documento obrigatório");
            RuleFor(x => x.PixKey).NotEmpty().WithMessage("Chave Pix obrigatória");
            RuleFor(x => x.OriginCep).NotEmpty().WithMessage("CEP de origem obrigatório");
            RuleFor(x => x.OriginAddress).NotEmpty().WithMessage("Endereço de origem obrigatório");
        }
    }
}
