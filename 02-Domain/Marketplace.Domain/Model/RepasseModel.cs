namespace Marketplace.Domain.Model
{
    public class RepasseModel
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal ProductAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal NetAmount { get; set; }
        public bool Paid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
