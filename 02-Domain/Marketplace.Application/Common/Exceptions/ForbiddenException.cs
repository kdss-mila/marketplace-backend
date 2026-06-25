namespace Marketplace.Application.Common.Exceptions
{
    public sealed class ForbiddenException(string message = "Acesso negado") : DomainException(message)
    {
        public override int StatusCode => 403;
    }
}
