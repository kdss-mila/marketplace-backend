namespace Marketplace.Application.DTOs.Admin
{
    public record CategoryRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? ParentId { get; init; }
    }
}
