using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.DTOs.Admin;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class BanUserUseCase(
        ICurrentUserResolver currentUser,
        IUserRepository userRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserModel> Execute(string userId, BanUserRequest request)
        {
            await AdminGuard.RequireAdmin(_currentUser);

            var target = await _userRepository.GetById(userId)
                ?? throw new NotFoundException("Usuário não encontrado");

            target.Banned = request.Banned;
            await _userRepository.Update(target);
            return target;
        }
    }
}
