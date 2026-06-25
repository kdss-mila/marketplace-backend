using FluentValidation;
using Marketplace.Application.DTOs.Auth;

namespace Marketplace.Application.Validators.Auth
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Senha obrigatória");
        }
    }
}
