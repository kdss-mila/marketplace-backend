using Dapper;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;
using Marketplace.Repository.Data;

namespace Marketplace.Repository.Postgres
{
    /// <summary>
    /// Persistência de pedidos em Postgres. As colunas de endereço são
    /// achatadas na tabela <c>orders</c> e reagrupadas em
    /// <see cref="OrderAddressModel"/> na leitura.
    /// </summary>
    public class PostgresOrderRepository(IDbConnectionFactory connectionFactory) : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private const string BaseSelect = @"
SELECT
    id            AS Id,
    buyer_id      AS BuyerId,
    buyer_name    AS BuyerName,
    product_id    AS ProductId,
    product_title AS ProductTitle,
    seller_id     AS SellerId,
    seller_name   AS SellerName,
    quantity      AS Quantity,
    product_price AS ProductPrice,
    shipping_cost AS ShippingCost,
    total         AS Total,
    status        AS StatusRaw,
    receipt_url   AS ReceiptUrl,
    tracking_code AS TrackingCode,
    cep           AS Cep,
    street        AS Street,
    number        AS Number,
    complement    AS Complement,
    neighborhood  AS Neighborhood,
    city          AS City,
    state         AS State,
    created_at    AS CreatedAt,
    updated_at    AS UpdatedAt
FROM orders";

        private sealed class OrderRow
        {
            public string Id { get; set; } = string.Empty;
            public string BuyerId { get; set; } = string.Empty;
            public string BuyerName { get; set; } = string.Empty;
            public string ProductId { get; set; } = string.Empty;
            public string ProductTitle { get; set; } = string.Empty;
            public string SellerId { get; set; } = string.Empty;
            public string SellerName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal ProductPrice { get; set; }
            public decimal ShippingCost { get; set; }
            public decimal Total { get; set; }
            public string StatusRaw { get; set; } = string.Empty;
            public string? ReceiptUrl { get; set; }
            public string? TrackingCode { get; set; }
            public string Cep { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Number { get; set; } = string.Empty;
            public string? Complement { get; set; }
            public string? Neighborhood { get; set; }
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        private static OrderModel Map(OrderRow row) => new()
        {
            Id = row.Id,
            BuyerId = row.BuyerId,
            BuyerName = row.BuyerName,
            ProductId = row.ProductId,
            ProductTitle = row.ProductTitle,
            SellerId = row.SellerId,
            SellerName = row.SellerName,
            Quantity = row.Quantity,
            ProductPrice = row.ProductPrice,
            ShippingCost = row.ShippingCost,
            Total = row.Total,
            Status = ParseStatus(row.StatusRaw),
            ReceiptUrl = row.ReceiptUrl,
            TrackingCode = row.TrackingCode,
            CreatedAt = DateTime.SpecifyKind(row.CreatedAt, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(row.UpdatedAt, DateTimeKind.Utc),
            Address = new OrderAddressModel
            {
                Cep = row.Cep,
                Street = row.Street,
                Number = row.Number,
                Complement = row.Complement,
                Neighborhood = row.Neighborhood,
                City = row.City,
                State = row.State,
            },
        };

        // Preserva os literais PT-BR ("Em Análise", "Aguardando Comprovante" etc.)
        // usados em toda a paridade com o frontend.
        private static OrderStatus ParseStatus(string raw) => raw switch
        {
            "Aguardando Comprovante" => OrderStatus.AguardandoComprovante,
            "Em Análise" => OrderStatus.EmAnalise,
            "Pago" => OrderStatus.Pago,
            "Enviado" => OrderStatus.Enviado,
            "Entregue" => OrderStatus.Entregue,
            _ => throw new InvalidOperationException($"Status desconhecido: {raw}"),
        };

        private static string SerializeStatus(OrderStatus status) => status switch
        {
            OrderStatus.AguardandoComprovante => "Aguardando Comprovante",
            OrderStatus.EmAnalise => "Em Análise",
            OrderStatus.Pago => "Pago",
            OrderStatus.Enviado => "Enviado",
            OrderStatus.Entregue => "Entregue",
            _ => throw new InvalidOperationException($"Status desconhecido: {status}"),
        };

        public async Task<OrderModel?> GetById(string id)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<OrderRow>(
                BaseSelect + " WHERE id = @id", new { id });
            return row is null ? null : Map(row);
        }

        public async Task<IEnumerable<OrderModel>> GetByBuyerId(string buyerId)
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<OrderRow>(
                BaseSelect + " WHERE buyer_id = @buyerId ORDER BY created_at DESC",
                new { buyerId });
            return rows.Select(Map).ToList();
        }

        public async Task<IEnumerable<OrderModel>> GetBySellerIdAndStatuses(string sellerId, params OrderStatus[] statuses)
        {
            using var conn = _connectionFactory.Create();
            var statusStrings = statuses.Select(SerializeStatus).ToArray();
            var rows = await conn.QueryAsync<OrderRow>(
                BaseSelect + " WHERE seller_id = @sellerId AND status = ANY(@statuses) ORDER BY created_at DESC",
                new { sellerId, statuses = statusStrings });
            return rows.Select(Map).ToList();
        }

        public async Task<IEnumerable<OrderModel>> GetByStatus(OrderStatus status)
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<OrderRow>(
                BaseSelect + " WHERE status = @status ORDER BY created_at DESC",
                new { status = SerializeStatus(status) });
            return rows.Select(Map).ToList();
        }

        public async Task Add(OrderModel order)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
INSERT INTO orders (
    id, buyer_id, buyer_name, product_id, product_title, seller_id, seller_name,
    quantity, product_price, shipping_cost, total, status,
    receipt_url, tracking_code, cep, street, number, complement, neighborhood, city, state,
    created_at, updated_at
) VALUES (
    @Id, @BuyerId, @BuyerName, @ProductId, @ProductTitle, @SellerId, @SellerName,
    @Quantity, @ProductPrice, @ShippingCost, @Total, @Status,
    @ReceiptUrl, @TrackingCode, @Cep, @Street, @Number, @Complement, @Neighborhood, @City, @State,
    @CreatedAt, @UpdatedAt
);",
                new
                {
                    order.Id,
                    order.BuyerId,
                    order.BuyerName,
                    order.ProductId,
                    order.ProductTitle,
                    order.SellerId,
                    order.SellerName,
                    order.Quantity,
                    order.ProductPrice,
                    order.ShippingCost,
                    order.Total,
                    Status = SerializeStatus(order.Status),
                    order.ReceiptUrl,
                    order.TrackingCode,
                    order.Address.Cep,
                    order.Address.Street,
                    order.Address.Number,
                    order.Address.Complement,
                    order.Address.Neighborhood,
                    order.Address.City,
                    order.Address.State,
                    order.CreatedAt,
                    order.UpdatedAt,
                });
        }

        public async Task Update(OrderModel order)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
UPDATE orders SET
    buyer_name = @BuyerName,
    product_title = @ProductTitle,
    seller_name = @SellerName,
    quantity = @Quantity,
    product_price = @ProductPrice,
    shipping_cost = @ShippingCost,
    total = @Total,
    status = @Status,
    receipt_url = @ReceiptUrl,
    tracking_code = @TrackingCode,
    cep = @Cep,
    street = @Street,
    number = @Number,
    complement = @Complement,
    neighborhood = @Neighborhood,
    city = @City,
    state = @State,
    updated_at = @UpdatedAt
WHERE id = @Id;",
                new
                {
                    order.Id,
                    order.BuyerName,
                    order.ProductTitle,
                    order.SellerName,
                    order.Quantity,
                    order.ProductPrice,
                    order.ShippingCost,
                    order.Total,
                    Status = SerializeStatus(order.Status),
                    order.ReceiptUrl,
                    order.TrackingCode,
                    order.Address.Cep,
                    order.Address.Street,
                    order.Address.Number,
                    order.Address.Complement,
                    order.Address.Neighborhood,
                    order.Address.City,
                    order.Address.State,
                    order.UpdatedAt,
                });
        }
    }
}
