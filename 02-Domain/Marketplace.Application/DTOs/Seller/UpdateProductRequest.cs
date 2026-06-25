using Marketplace.Domain.Model;

namespace Marketplace.Application.DTOs.Seller
{
    public record UpdateProductRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public decimal? Price { get; init; }
        public int? Stock { get; init; }
        public string? CategoryId { get; init; }
        public List<string>? Images { get; init; }
        public decimal? Weight { get; init; }
        public ProductDimensionsModel? Dimensions { get; init; }
    }
}
