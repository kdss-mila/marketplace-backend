using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class ListRepassesUseCase(
        ICurrentUserResolver currentUser,
        IRepasseRepository repasseRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IRepasseRepository _repasseRepository = repasseRepository;

        public async Task<IEnumerable<RepasseModel>> Execute()
        {
            await AdminGuard.RequireAdmin(_currentUser);
            return await _repasseRepository.GetAll();
        }
    }
}
