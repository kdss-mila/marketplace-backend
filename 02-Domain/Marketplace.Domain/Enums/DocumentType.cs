using System.Text.Json.Serialization;

namespace Marketplace.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<DocumentType>))]
    public enum DocumentType
    {
        [JsonStringEnumMemberName("cpf")]
        Cpf = 0,

        [JsonStringEnumMemberName("cnpj")]
        Cnpj = 1,
    }
}
