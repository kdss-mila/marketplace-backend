namespace Marketplace.Application.Common.Exceptions
{
    public sealed class ValidationException(string message) : DomainException(message)
    {
        public override int StatusCode => 400;
    }
}
