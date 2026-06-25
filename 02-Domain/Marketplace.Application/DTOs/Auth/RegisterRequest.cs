namespace Marketplace.Application.DTOs.Auth
{
    public record RegisterRequest
    {
        public string Email { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Cpf { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
