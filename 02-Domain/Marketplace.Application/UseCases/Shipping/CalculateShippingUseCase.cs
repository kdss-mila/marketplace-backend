using FluentValidation;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Shipping;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Application.UseCases.Shipping
{
    public class CalculateShippingUseCase(
        IShippingCalculator shippingCalculator,
        IValidator<ShippingQuoteRequest> validator)
    {
        private readonly IShippingCalculator _shippingCalculator = shippingCalculator;
        private readonly IValidator<ShippingQuoteRequest> _validator = validator;

        public async Task<ShippingQuoteResponse> Execute(ShippingQuoteRequest request)
        {
            await ValidatorRunner.EnsureValid(_validator, request);

            var result = _shippingCalculator.Calculate(request.CepOrigem, request.CepDestino, request.Peso);

            return new ShippingQuoteResponse
            {
                Valor = result.Valor,
                PrazoDias = result.PrazoDias,
                Transportadora = result.Transportadora,
            };
        }
    }
}
