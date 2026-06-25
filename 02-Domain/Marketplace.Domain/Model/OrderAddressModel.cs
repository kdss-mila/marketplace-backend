namespace Marketplace.Domain.Model
{
    public class OrderAddressModel
    {
        public string Cep { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
