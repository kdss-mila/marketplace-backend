using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Auth;
using Marketplace.Domain.Interface.Repository;

namespace Marketplace.Application.UseCases.Auth
{
    public class LoginUseCase(
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        IValidator<LoginRequest> validator)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly IValidator<LoginRequest> _validator = validator;

        public async Task<AuthResponse> Execute(LoginRequest request)
        {
            await ValidatorRunner.EnsureValid(_validator, request);

            var user = await _userRepository.GetByEmail(request.Email);
            if (user is null || user.Password != request.Password)
                throw new UnauthorizedException("Credenciais inválidas");

            if (user.Banned)
                throw new ForbiddenException("Usuário banido");

            var existing = await _tokenRepository.FindExistingTokenForUser(user.Id);
            var token = existing ?? await _tokenRepository.Create(user.Id);

            return new AuthResponse { User = user, Token = token };
        }
    }
}
