namespace Marketplace.Application.DTOs.Shipping
{
    public record ShippingQuoteResponse
    {
        public decimal Valor { get; init; }
        public int PrazoDias { get; init; }
        public string Transportadora { get; init; } = string.Empty;
    }
}
