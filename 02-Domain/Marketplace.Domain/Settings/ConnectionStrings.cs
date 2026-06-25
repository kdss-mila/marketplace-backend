namespace Marketplace.Domain.Settings
{
    public record ConnectionStrings
    {
        public string Postgress { get; set; } = string.Empty;
    }
}
