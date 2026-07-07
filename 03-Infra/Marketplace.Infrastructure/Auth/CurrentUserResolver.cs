using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Infrastructure.Auth
{
    /// <summary>
    /// Lê o JWT já validado pelo middleware JwtBearer (HttpContext.User) e devolve
    /// o UserModel atual a partir do repositório. Isso mantém a paridade com o
    /// mock antigo que resolvia o usuário por token e permite que UseCases
    /// continuem chamando GetCurrent() sem tocar em HttpContext.
    /// </summary>
    public class CurrentUserResolver(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository) : ICurrentUserResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserModel?> GetCurrent()
        {
            var http = _httpContextAccessor.HttpContext;
            var principal = http?.User;
            if (principal?.Identity is null || !principal.Identity.IsAuthenticated) return null;

            var userId =
                principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                principal.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _userRepository.GetById(userId);
        }
    }
}
