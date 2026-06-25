using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Model
{
    public class SellerProfileModel
    {
        public DocumentType DocumentType { get; set; }
        public string Document { get; set; } = string.Empty;
        public string PixKey { get; set; } = string.Empty;
        public string OriginCep { get; set; } = string.Empty;
        public string OriginAddress { get; set; } = string.Empty;
        public bool OnboardingComplete { get; set; }
    }
}
