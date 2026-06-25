using Marketplace.Domain.Enums;

namespace Marketplace.Application.DTOs.Seller
{
    public record SellerOnboardingRequest
    {
        public DocumentType DocumentType { get; init; }
        public string Document { get; init; } = string.Empty;
        public string PixKey { get; init; } = string.Empty;
        public string OriginCep { get; init; } = string.Empty;
        public string OriginAddress { get; init; } = string.Empty;
    }
}
