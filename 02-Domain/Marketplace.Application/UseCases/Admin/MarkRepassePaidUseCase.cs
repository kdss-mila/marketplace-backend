using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class MarkRepassePaidUseCase(
        ICurrentUserResolver currentUser,
        IRepasseRepository repasseRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IRepasseRepository _repasseRepository = repasseRepository;

        public async Task<RepasseModel> Execute(string repasseId)
        {
            await AdminGuard.RequireAdmin(_currentUser);

            var repasse = await _repasseRepository.GetById(repasseId)
                ?? throw new NotFoundException("Repasse não encontrado");

            repasse.Paid = true;
            await _repasseRepository.Update(repasse);
            return repasse;
        }
    }
}
