using Marketplace.Domain.Enums;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    /// <summary>
    /// Singleton thread-safe que mantém o estado dos dados mockados em memória.
    /// Seed inicial espelha exatamente marketplace-frontend/src/lib/mocks/seed.ts
    /// para que as credenciais de teste (senha 123456) e os tokens pré-cadastrados
    /// continuem válidos durante o desenvolvimento.
    /// </summary>
    public sealed class InMemoryStore
    {
        public List<UserModel> Users { get; } = new();
        public List<CategoryModel> Categories { get; } = new();
        public List<ProductModel> Products { get; } = new();
        public List<OrderModel> Orders { get; } = new();
        public List<RepasseModel> Repasses { get; } = new();
        public Dictionary<string, string> Tokens { get; } = new();

        public readonly object SyncRoot = new();

        public InMemoryStore()
        {
            Seed();
        }

        private void Seed()
        {
            const string buyerId = "user-buyer-1";
            const string sellerId = "user-seller-1";
            const string adminId = "user-admin-1";

            Categories.AddRange(new[]
            {
                new CategoryModel { Id = "cat-1", Name = "Eletrônicos", ParentId = null },
                new CategoryModel { Id = "cat-2", Name = "Celulares",   ParentId = "cat-1" },
                new CategoryModel { Id = "cat-3", Name = "Moda",        ParentId = null },
                new CategoryModel { Id = "cat-4", Name = "Casa",        ParentId = null },
            });

            Users.AddRange(new[]
            {
                new UserModel
                {
                    Id = buyerId,
                    Email = "comprador@teste.com",
                    Name = "João Comprador",
                    Cpf = "12345678901",
                    Role = UserRole.Buyer,
                    Banned = false,
                    Password = "123456",
                },
                new UserModel
                {
                    Id = sellerId,
                    Email = "vendedor@teste.com",
                    Name = "Maria Vendedora",
                    Cpf = "98765432100",
                    Role = UserRole.Seller,
                    Banned = false,
                    Password = "123456",
                    SellerProfile = new SellerProfileModel
                    {
                        DocumentType = DocumentType.Cpf,
                        Document = "98765432100",
                        PixKey = "vendedor@teste.com",
                        OriginCep = "01310100",
                        OriginAddress = "Av. Paulista, 1000 - São Paulo/SP",
                        OnboardingComplete = true,
                    },
                },
                new UserModel
                {
                    Id = adminId,
                    Email = "admin@teste.com",
                    Name = "Admin Sistema",
                    Cpf = "11122233344",
                    Role = UserRole.Admin,
                    Banned = false,
                    Password = "123456",
                },
            });

            Products.AddRange(new[]
            {
                new ProductModel
                {
                    Id = "prod-1",
                    Title = "Smartphone Pro Max",
                    Description = "Smartphone com tela AMOLED de 6.7 polegadas, 256GB de armazenamento.",
                    Price = 3499.9m,
                    Stock = 15,
                    CategoryId = "cat-2",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/phone/600/600" },
                    Weight = 0.2m,
                    Dimensions = new() { Width = 8, Height = 16, Length = 1 },
                },
                new ProductModel
                {
                    Id = "prod-2",
                    Title = "Fone Bluetooth Premium",
                    Description = "Fone com cancelamento de ruído ativo e bateria de 30h.",
                    Price = 499.9m,
                    Stock = 30,
                    CategoryId = "cat-1",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/headphone/600/600" },
                    Weight = 0.3m,
                    Dimensions = new() { Width = 20, Height = 20, Length = 8 },
                },
                new ProductModel
                {
                    Id = "prod-3",
                    Title = "Camiseta Básica Algodão",
                    Description = "Camiseta 100% algodão, disponível em várias cores.",
                    Price = 79.9m,
                    Stock = 100,
                    CategoryId = "cat-3",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/shirt/600/600" },
                    Weight = 0.25m,
                    Dimensions = new() { Width = 30, Height = 2, Length = 25 },
                },
                new ProductModel
                {
                    Id = "prod-4",
                    Title = "Luminária de Mesa LED",
                    Description = "Luminária articulada com 3 níveis de brilho.",
                    Price = 189.9m,
                    Stock = 20,
                    CategoryId = "cat-4",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/lamp/600/600" },
                    Weight = 1.2m,
                    Dimensions = new() { Width = 15, Height = 40, Length = 15 },
                },
                new ProductModel
                {
                    Id = "prod-5",
                    Title = "Notebook Ultra Slim",
                    Description = "Notebook leve com processador de última geração e SSD 512GB.",
                    Price = 5299.0m,
                    Stock = 8,
                    CategoryId = "cat-1",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/laptop/600/600" },
                    Weight = 1.5m,
                    Dimensions = new() { Width = 35, Height = 2, Length = 25 },
                },
                new ProductModel
                {
                    Id = "prod-6",
                    Title = "Relógio Esportivo",
                    Description = "Relógio resistente à água com monitor de frequência cardíaca.",
                    Price = 899.0m,
                    Stock = 12,
                    CategoryId = "cat-3",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Images = new() { "https://picsum.photos/seed/watch/600/600" },
                    Weight = 0.1m,
                    Dimensions = new() { Width = 5, Height = 5, Length = 2 },
                },
            });

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var twoDaysAgo = DateTime.UtcNow.AddDays(-2);

            Orders.AddRange(new[]
            {
                new OrderModel
                {
                    Id = "order-1",
                    BuyerId = buyerId,
                    BuyerName = "João Comprador",
                    ProductId = "prod-2",
                    ProductTitle = "Fone Bluetooth Premium",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Quantity = 1,
                    ProductPrice = 499.9m,
                    ShippingCost = 25.5m,
                    Total = 525.4m,
                    Status = OrderStatus.EmAnalise,
                    Address = new()
                    {
                        Cep = "22041080",
                        Street = "Rua das Flores",
                        Number = "123",
                        City = "Rio de Janeiro",
                        State = "RJ",
                    },
                    ReceiptUrl = "https://picsum.photos/seed/receipt/400/600",
                    CreatedAt = yesterday,
                    UpdatedAt = DateTime.UtcNow,
                },
                new OrderModel
                {
                    Id = "order-2",
                    BuyerId = buyerId,
                    BuyerName = "João Comprador",
                    ProductId = "prod-3",
                    ProductTitle = "Camiseta Básica Algodão",
                    SellerId = sellerId,
                    SellerName = "Maria Vendedora",
                    Quantity = 2,
                    ProductPrice = 159.8m,
                    ShippingCost = 18.0m,
                    Total = 177.8m,
                    Status = OrderStatus.Enviado,
                    Address = new()
                    {
                        Cep = "22041080",
                        Street = "Rua das Flores",
                        Number = "123",
                        City = "Rio de Janeiro",
                        State = "RJ",
                    },
                    ReceiptUrl = "https://picsum.photos/seed/receipt2/400/600",
                    TrackingCode = "BR123456789BR",
                    CreatedAt = twoDaysAgo,
                    UpdatedAt = DateTime.UtcNow,
                },
            });

            Repasses.Add(new RepasseModel
            {
                Id = "repasse-1",
                OrderId = "order-2",
                SellerId = sellerId,
                SellerName = "Maria Vendedora",
                ProductAmount = 159.8m,
                ShippingAmount = 18.0m,
                Commission = 15.98m,
                NetAmount = 161.82m,
                Paid = false,
                CreatedAt = yesterday,
            });

            Tokens["token-buyer"] = buyerId;
            Tokens["token-seller"] = sellerId;
            Tokens["token-admin"] = adminId;
        }
    }
}
