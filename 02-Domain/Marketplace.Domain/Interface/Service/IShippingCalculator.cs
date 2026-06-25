namespace Marketplace.Domain.Interface.Service
{
    public interface IShippingCalculator
    {
        ShippingQuoteResult Calculate(string cepOrigem, string cepDestino, decimal peso);
    }

    public record ShippingQuoteResult(decimal Valor, int PrazoDias, string Transportadora);
}
