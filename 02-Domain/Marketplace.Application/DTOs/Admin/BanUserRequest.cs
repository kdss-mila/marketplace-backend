namespace Marketplace.Application.DTOs.Admin
{
    public record BanUserRequest
    {
        public bool Banned { get; init; }
    }
}
