using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Auth;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Application.UseCases.Auth
{
    public class LoginUseCase(
        IUserRepository userRepository,
        IJwtService jwtService,
        IValidator<LoginRequest> validator)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IValidator<LoginRequest> _validator = validator;

        public async Task<AuthResponse> Execute(LoginRequest request)
        {
            await ValidatorRunner.EnsureValid(_validator, request);

            var user = await _userRepository.GetByEmail(request.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Credenciais inválidas");

            if (user.Banned)
                throw new ForbiddenException("Usuário banido");

            var token = _jwtService.GenerateToken(user);
            return new AuthResponse { User = user, Token = token };
        }
    }
}
