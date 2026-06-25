namespace Marketplace.Domain.Settings
{
    public record PlatformSettings
    {
        public string PixKey { get; init; } = "marketplace@pix.com.br";
        public decimal CommissionRate { get; init; } = 0.10m;
    }
}
