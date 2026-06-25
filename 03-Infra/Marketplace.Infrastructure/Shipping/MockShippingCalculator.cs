using Marketplace.Domain.Interface.Service;

namespace Marketplace.Infrastructure.Shipping
{
    /// <summary>
    /// Replica fielmente a fórmula do mock do frontend
    /// (marketplace-frontend/src/lib/mocks/handlers/index.ts:341-350).
    /// Será substituído quando integrarmos Correios / Melhor Envio.
    /// </summary>
    public class MockShippingCalculator : IShippingCalculator
    {
        public ShippingQuoteResult Calculate(string cepOrigem, string cepDestino, decimal peso)
        {
            var origin = ParsePrefix(cepOrigem);
            var dest = ParsePrefix(cepDestino);
            var distance = Math.Abs(origin - dest);

            var valor = 15m + distance * 0.5m + peso * 10m;
            valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);

            var prazoDias = 3 + (distance / 10);

            return new ShippingQuoteResult(valor, prazoDias, "Correios Simulado");
        }

        private static int ParsePrefix(string cep)
        {
            var digits = new string((cep ?? string.Empty).Where(char.IsDigit).Take(2).ToArray());
            return int.TryParse(digits, out var n) ? n : 0;
        }
    }
}
