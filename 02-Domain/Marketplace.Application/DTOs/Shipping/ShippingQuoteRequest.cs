namespace Marketplace.Application.DTOs.Shipping
{
    public record ShippingQuoteRequest
    {
        public string CepOrigem { get; init; } = string.Empty;
        public string CepDestino { get; init; } = string.Empty;
        public decimal Peso { get; init; }
    }
}
