using Marketplace.Domain.Model;

namespace Marketplace.Application.DTOs.Seller
{
    public record CreateProductRequest
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Stock { get; init; }
        public string CategoryId { get; init; } = string.Empty;
        public List<string> Images { get; init; } = new();
        public decimal Weight { get; init; }
        public ProductDimensionsModel Dimensions { get; init; } = new();
    }
}
