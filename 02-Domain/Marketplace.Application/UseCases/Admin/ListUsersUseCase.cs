using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class ListUsersUseCase(
        ICurrentUserResolver currentUser,
        IUserRepository userRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IEnumerable<UserModel>> Execute()
        {
            await AdminGuard.RequireAdmin(_currentUser);
            return await _userRepository.GetAll();
        }
    }
}
