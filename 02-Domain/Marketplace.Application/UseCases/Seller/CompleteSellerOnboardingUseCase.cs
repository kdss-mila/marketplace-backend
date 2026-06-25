using FluentValidation;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Seller;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class CompleteSellerOnboardingUseCase(
        ICurrentUserResolver currentUser,
        IUserRepository userRepository,
        IValidator<SellerOnboardingRequest> validator)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IValidator<SellerOnboardingRequest> _validator = validator;

        public async Task<UserModel> Execute(SellerOnboardingRequest request)
        {
            var user = await SellerGuard.RequireSeller(_currentUser);
            await ValidatorRunner.EnsureValid(_validator, request);

            user.SellerProfile = new SellerProfileModel
            {
                DocumentType = request.DocumentType,
                Document = request.Document,
                PixKey = request.PixKey,
                OriginCep = request.OriginCep,
                OriginAddress = request.OriginAddress,
                OnboardingComplete = true,
            };

            await _userRepository.Update(user);
            return user;
        }
    }
}
