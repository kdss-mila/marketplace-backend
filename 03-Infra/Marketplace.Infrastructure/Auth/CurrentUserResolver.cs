using Microsoft.AspNetCore.Http;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Infrastructure.Auth
{
    /// <summary>
    /// Lê o header "Authorization: Bearer &lt;token&gt;" da request HTTP atual,
    /// faz lookup no ITokenRepository (dicionário token-&gt;userId) e devolve o
    /// usuário correspondente. Retorna null se não houver token válido.
    /// Substituirá, no futuro, a integração real com JWT.
    /// </summary>
    public class CurrentUserResolver(
        IHttpContextAccessor httpContextAccessor,
        ITokenRepository tokenRepository,
        IUserRepository userRepository) : ICurrentUserResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserModel?> GetCurrent()
        {
            var http = _httpContextAccessor.HttpContext;
            if (http is null) return null;

            var header = http.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(header)) return null;

            const string prefix = "Bearer ";
            if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;

            var token = header[prefix.Length..].Trim();
            if (string.IsNullOrEmpty(token)) return null;

            var userId = await _tokenRepository.GetUserId(token);
            if (userId is null) return null;

            return await _userRepository.GetById(userId);
        }
    }
}
