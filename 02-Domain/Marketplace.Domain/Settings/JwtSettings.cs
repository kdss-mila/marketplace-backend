namespace Marketplace.Domain.Settings
{
    public record JwtSettings
    {
        public string SigningKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = "marketplace-api";
        public string Audience { get; init; } = "marketplace-web";
        public int ExpiresHours { get; init; } = 8;
    }
}
