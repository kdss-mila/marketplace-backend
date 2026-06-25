using FluentValidation;
using Marketplace.Application.DTOs.Seller;

namespace Marketplace.Application.Validators.Seller
{
    public sealed class SetTrackingCodeRequestValidator : AbstractValidator<SetTrackingCodeRequest>
    {
        public SetTrackingCodeRequestValidator()
        {
            RuleFor(x => x.TrackingCode).NotEmpty().WithMessage("Código de rastreio obrigatório");
        }
    }
}
