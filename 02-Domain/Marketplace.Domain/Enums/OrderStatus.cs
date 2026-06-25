using System.Text.Json.Serialization;

namespace Marketplace.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<OrderStatus>))]
    public enum OrderStatus
    {
        [JsonStringEnumMemberName("Aguardando Comprovante")]
        AguardandoComprovante = 0,

        [JsonStringEnumMemberName("Em Análise")]
        EmAnalise = 1,

        [JsonStringEnumMemberName("Pago")]
        Pago = 2,

        [JsonStringEnumMemberName("Enviado")]
        Enviado = 3,

        [JsonStringEnumMemberName("Entregue")]
        Entregue = 4,
    }
}
