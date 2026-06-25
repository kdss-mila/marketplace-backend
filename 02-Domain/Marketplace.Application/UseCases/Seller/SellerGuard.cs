using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    internal static class SellerGuard
    {
        public static async Task<UserModel> RequireSeller(ICurrentUserResolver resolver)
        {
            var user = await resolver.GetCurrent() ?? throw new UnauthorizedException();
            if (user.Role != UserRole.Seller) throw new ForbiddenException();
            return user;
        }
    }
}
