namespace Marketplace.Application.Common.Exceptions
{
    public sealed class DependencyException(string message) : DomainException(message)
    {
        public override int StatusCode => 502;
    }
}
