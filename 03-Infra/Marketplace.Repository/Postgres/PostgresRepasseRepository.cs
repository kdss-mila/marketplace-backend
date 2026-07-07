using Dapper;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;
using Marketplace.Repository.Data;

namespace Marketplace.Repository.Postgres
{
    public class PostgresRepasseRepository(IDbConnectionFactory connectionFactory) : IRepasseRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private const string BaseSelect = @"
SELECT
    id              AS Id,
    order_id        AS OrderId,
    seller_id       AS SellerId,
    seller_name     AS SellerName,
    product_amount  AS ProductAmount,
    shipping_amount AS ShippingAmount,
    commission      AS Commission,
    net_amount      AS NetAmount,
    paid            AS Paid,
    created_at      AS CreatedAt
FROM repasses";

        public async Task<IEnumerable<RepasseModel>> GetAll()
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<RepasseModel>(
                BaseSelect + " ORDER BY created_at DESC");
            return rows.Select(EnsureUtc).ToList();
        }

        public async Task<RepasseModel?> GetById(string id)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<RepasseModel>(
                BaseSelect + " WHERE id = @id", new { id });
            return row is null ? null : EnsureUtc(row);
        }

        public async Task Add(RepasseModel repasse)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
INSERT INTO repasses (id, order_id, seller_id, seller_name, product_amount, shipping_amount, commission, net_amount, paid, created_at)
VALUES (@Id, @OrderId, @SellerId, @SellerName, @ProductAmount, @ShippingAmount, @Commission, @NetAmount, @Paid, @CreatedAt);",
                repasse);
        }

        public async Task Update(RepasseModel repasse)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
UPDATE repasses SET
    order_id = @OrderId,
    seller_id = @SellerId,
    seller_name = @SellerName,
    product_amount = @ProductAmount,
    shipping_amount = @ShippingAmount,
    commission = @Commission,
    net_amount = @NetAmount,
    paid = @Paid,
    created_at = @CreatedAt
WHERE id = @Id;",
                repasse);
        }

        private static RepasseModel EnsureUtc(RepasseModel repasse)
        {
            repasse.CreatedAt = DateTime.SpecifyKind(repasse.CreatedAt, DateTimeKind.Utc);
            return repasse;
        }
    }
}
