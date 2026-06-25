using System.Text.Json.Serialization;

namespace Marketplace.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
    public enum UserRole
    {
        [JsonStringEnumMemberName("buyer")]
        Buyer = 0,

        [JsonStringEnumMemberName("seller")]
        Seller = 1,

        [JsonStringEnumMemberName("admin")]
        Admin = 2,
    }
}
