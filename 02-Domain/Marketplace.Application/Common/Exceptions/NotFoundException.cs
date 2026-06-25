namespace Marketplace.Application.Common.Exceptions
{
    public sealed class NotFoundException(string message) : DomainException(message)
    {
        public override int StatusCode => 404;
    }
}
