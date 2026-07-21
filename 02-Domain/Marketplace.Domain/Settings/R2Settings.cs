namespace Marketplace.Domain.Settings
{
    public record R2Settings
    {
        public string AccountId { get; init; } = string.Empty;
        public string AccessKeyId { get; init; } = string.Empty;
        public string SecretAccessKey { get; init; } = string.Empty;
        public string BucketName { get; init; } = string.Empty;
        public string PublicUrl { get; init; } = string.Empty;
    }
}
