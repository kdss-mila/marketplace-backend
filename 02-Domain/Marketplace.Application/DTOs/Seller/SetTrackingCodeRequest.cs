namespace Marketplace.Application.DTOs.Seller
{
    public record SetTrackingCodeRequest
    {
        public string TrackingCode { get; init; } = string.Empty;
    }
}
