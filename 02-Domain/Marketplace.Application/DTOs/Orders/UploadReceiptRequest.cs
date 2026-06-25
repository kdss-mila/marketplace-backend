namespace Marketplace.Application.DTOs.Orders
{
    public record UploadReceiptRequest
    {
        public string OrderId { get; init; } = string.Empty;
        public Stream FileStream { get; init; } = default!;
        public string FileName { get; init; } = string.Empty;
    }
}
