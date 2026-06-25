using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Auth;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Auth
{
    public class RegisterUseCase(
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        IValidator<RegisterRequest> validator)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly IValidator<RegisterRequest> _validator = validator;

        public async Task<AuthResponse> Execute(RegisterRequest request)
        {
            await ValidatorRunner.EnsureValid(_validator, request);

            var existing = await _userRepository.GetByEmail(request.Email);
            if (existing is not null)
                throw new ConflictException("E-mail já cadastrado");

            var user = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                Name = request.Name,
                Cpf = new string(request.Cpf.Where(char.IsDigit).ToArray()),
                Role = UserRole.Buyer,
                Banned = false,
                Password = request.Password,
            };

            await _userRepository.Add(user);
            var token = await _tokenRepository.Create(user.Id);

            return new AuthResponse { User = user, Token = token };
        }
    }
}
