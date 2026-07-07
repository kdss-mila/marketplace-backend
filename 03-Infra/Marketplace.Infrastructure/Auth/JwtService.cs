using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;
using Marketplace.Domain.Settings;

namespace Marketplace.Infrastructure.Auth
{
    /// <summary>
    /// Emite JWTs assinados HS256 com claims mínimas para o frontend saber o
    /// papel do usuário logado. O CurrentUserResolver lê o mesmo id/role de
    /// HttpContext.User para revalidar o UserModel a cada request.
    /// </summary>
    public class JwtService(IOptions<JwtSettings> options) : IJwtService
    {
        private readonly JwtSettings _settings = options.Value;

        public string GenerateToken(UserModel user)
        {
            if (string.IsNullOrWhiteSpace(_settings.SigningKey))
                throw new InvalidOperationException("JwtSettings:SigningKey não configurada.");

            var keyBytes = Encoding.UTF8.GetBytes(_settings.SigningKey);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, MapRole(user.Role)),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddHours(Math.Max(1, _settings.ExpiresHours)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Reflete os valores literais serializados via JsonStringEnumMemberName
        // ("buyer" / "seller" / "admin") para bater com o payload esperado no
        // frontend e com o [Authorize(Roles="...")] dos controllers.
        private static string MapRole(UserRole role) => role switch
        {
            UserRole.Buyer => "buyer",
            UserRole.Seller => "seller",
            UserRole.Admin => "admin",
            _ => role.ToString().ToLowerInvariant(),
        };
    }
}
