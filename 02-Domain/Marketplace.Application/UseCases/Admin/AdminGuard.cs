using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    internal static class AdminGuard
    {
        public static async Task<UserModel> RequireAdmin(ICurrentUserResolver resolver)
        {
            var user = await resolver.GetCurrent() ?? throw new UnauthorizedException();
            if (user.Role != UserRole.Admin) throw new ForbiddenException();
            return user;
        }
    }
}
