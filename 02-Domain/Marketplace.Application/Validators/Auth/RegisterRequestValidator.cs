using FluentValidation;
using Marketplace.Application.DTOs.Auth;

namespace Marketplace.Application.Validators.Auth
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido");
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).WithMessage("Nome obrigatório");
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("CPF obrigatório");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
        }
    }
}
