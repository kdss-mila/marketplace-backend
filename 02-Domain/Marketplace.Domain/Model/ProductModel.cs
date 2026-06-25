namespace Marketplace.Domain.Model
{
    public class ProductModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public decimal Weight { get; set; }
        public ProductDimensionsModel Dimensions { get; set; } = new();
    }
}
