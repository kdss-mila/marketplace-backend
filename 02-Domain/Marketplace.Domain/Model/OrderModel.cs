using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Model
{
    public class OrderModel
    {
        public string Id { get; set; } = string.Empty;
        public string BuyerId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductTitle { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public OrderAddressModel Address { get; set; } = new();
        public string? ReceiptUrl { get; set; }
        public string? TrackingCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
