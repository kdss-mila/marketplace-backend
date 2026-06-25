namespace Marketplace.Application.Common.Exceptions
{
    public abstract class DomainException(string message) : Exception(message)
    {
        public abstract int StatusCode { get; }
    }
}
