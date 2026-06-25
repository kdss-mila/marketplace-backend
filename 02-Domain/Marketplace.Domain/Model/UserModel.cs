using System.Text.Json.Serialization;
using Marketplace.Domain.Enums;

namespace Marketplace.Domain.Model
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool Banned { get; set; }
        public SellerProfileModel? SellerProfile { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
    }
}
