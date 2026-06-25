namespace Marketplace.Application.Common.Exceptions
{
    public sealed class UnauthorizedException(string message = "Não autorizado") : DomainException(message)
    {
        public override int StatusCode => 401;
    }
}
