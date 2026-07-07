namespace Marketplace.Domain.Settings
{
    public record ConnectionStrings
    {
        public string Postgres { get; set; } = string.Empty;
    }
}
