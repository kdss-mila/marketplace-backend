namespace Marketplace.Application.Common.Exceptions
{
    public sealed class ConflictException(string message) : DomainException(message)
    {
        public override int StatusCode => 400;
    }
}
