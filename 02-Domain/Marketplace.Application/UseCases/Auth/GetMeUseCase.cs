using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Auth
{
    public class GetMeUseCase(ICurrentUserResolver currentUser)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;

        public async Task<UserModel> Execute()
        {
            var user = await _currentUser.GetCurrent();
            if (user is null) throw new UnauthorizedException();
            return user;
        }
    }
}
