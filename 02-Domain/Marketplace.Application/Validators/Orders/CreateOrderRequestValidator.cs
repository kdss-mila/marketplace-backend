using FluentValidation;
using Marketplace.Application.DTOs.Orders;

namespace Marketplace.Application.Validators.Orders
{
    public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Produto obrigatório");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero");
            RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0).WithMessage("Frete inválido");
            RuleFor(x => x.Address.Cep).NotEmpty().WithMessage("CEP obrigatório");
            RuleFor(x => x.Address.Street).NotEmpty().WithMessage("Rua obrigatória");
            RuleFor(x => x.Address.Number).NotEmpty().WithMessage("Número obrigatório");
            RuleFor(x => x.Address.City).NotEmpty().WithMessage("Cidade obrigatória");
            RuleFor(x => x.Address.State).NotEmpty().WithMessage("Estado obrigatório");
        }
    }
}
