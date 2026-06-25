using Marketplace.Domain.Model;

namespace Marketplace.Application.DTOs.Auth
{
    public record AuthResponse
    {
        public UserModel User { get; init; } = default!;
        public string Token { get; init; } = string.Empty;
    }
}
