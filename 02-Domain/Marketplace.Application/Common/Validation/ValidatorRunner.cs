using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using ApplicationValidationException = Marketplace.Application.Common.Exceptions.ValidationException;

namespace Marketplace.Application.Common.Validation
{
    public static class ValidatorRunner
    {
        public static async Task EnsureValid<T>(IValidator<T> validator, T instance, CancellationToken cancellationToken = default)
        {
            var result = await validator.ValidateAsync(instance, cancellationToken);
            if (result.IsValid) return;

            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            throw new ApplicationValidationException(errors);
        }
    }
}
