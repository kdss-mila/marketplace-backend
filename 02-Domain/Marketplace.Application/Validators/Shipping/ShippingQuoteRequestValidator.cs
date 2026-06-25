using FluentValidation;
using Marketplace.Application.DTOs.Shipping;

namespace Marketplace.Application.Validators.Shipping
{
    public sealed class ShippingQuoteRequestValidator : AbstractValidator<ShippingQuoteRequest>
    {
        public ShippingQuoteRequestValidator()
        {
            RuleFor(x => x.CepOrigem).NotEmpty().WithMessage("CEP de origem obrigatório");
            RuleFor(x => x.CepDestino).NotEmpty().WithMessage("CEP de destino obrigatório");
            RuleFor(x => x.Peso).GreaterThan(0).WithMessage("Peso inválido");
        }
    }
}
