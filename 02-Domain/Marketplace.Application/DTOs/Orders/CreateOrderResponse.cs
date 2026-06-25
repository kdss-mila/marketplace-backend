using Marketplace.Domain.Model;

namespace Marketplace.Application.DTOs.Orders
{
    public record CreateOrderResponse
    {
        public OrderModel Order { get; init; } = default!;
        public string PixKey { get; init; } = string.Empty;
    }
}
