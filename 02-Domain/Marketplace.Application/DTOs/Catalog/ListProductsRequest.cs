namespace Marketplace.Application.DTOs.Catalog
{
    public record ListProductsRequest
    {
        public string? Q { get; init; }
        public string? CategoryId { get; init; }
        public bool IncludeSubcategories { get; init; }
    }
}
