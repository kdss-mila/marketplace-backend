using Marketplace.Domain.Model;

namespace Marketplace.Application.DTOs.Orders
{
    public record CreateOrderRequest
    {
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public OrderAddressModel Address { get; init; } = new();
        public decimal ShippingCost { get; init; }
    }
}
