using Dapper;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;
using Marketplace.Repository.Data;

namespace Marketplace.Repository.Postgres
{
    /// <summary>
    /// Persistência de produtos em Postgres. Imagens ficam na tabela
    /// <c>product_images</c> e são agregadas por produto usando um único
    /// SELECT com <c>array_agg</c> ordenado por <c>position</c>.
    /// </summary>
    public class PostgresProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private const string BaseSelect = @"
SELECT
    p.id           AS Id,
    p.title        AS Title,
    p.description  AS Description,
    p.price        AS Price,
    p.stock        AS Stock,
    p.category_id  AS CategoryId,
    p.seller_id    AS SellerId,
    p.seller_name  AS SellerName,
    p.weight       AS Weight,
    p.width        AS Width,
    p.height       AS Height,
    p.length       AS Length,
    COALESCE(
        (SELECT array_agg(pi.url ORDER BY pi.position)
         FROM product_images pi WHERE pi.product_id = p.id),
        ARRAY[]::text[]
    ) AS Images
FROM products p";

        private sealed class ProductRow
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string CategoryId { get; set; } = string.Empty;
            public string SellerId { get; set; } = string.Empty;
            public string SellerName { get; set; } = string.Empty;
            public decimal Weight { get; set; }
            public decimal Width { get; set; }
            public decimal Height { get; set; }
            public decimal Length { get; set; }
            public string[] Images { get; set; } = Array.Empty<string>();
        }

        private static ProductModel Map(ProductRow row) => new()
        {
            Id = row.Id,
            Title = row.Title,
            Description = row.Description,
            Price = row.Price,
            Stock = row.Stock,
            CategoryId = row.CategoryId,
            SellerId = row.SellerId,
            SellerName = row.SellerName,
            Weight = row.Weight,
            Dimensions = new ProductDimensionsModel
            {
                Width = row.Width,
                Height = row.Height,
                Length = row.Length,
            },
            Images = row.Images?.ToList() ?? new List<string>(),
        };

        public async Task<IEnumerable<ProductModel>> GetAll()
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<ProductRow>(BaseSelect + " ORDER BY p.title");
            return rows.Select(Map).ToList();
        }

        public async Task<IEnumerable<ProductModel>> Search(string? query, string? categoryId, bool includeSubcategories)
        {
            using var conn = _connectionFactory.Create();

            var whereParts = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query))
            {
                whereParts.Add("(LOWER(p.title) LIKE @q OR LOWER(p.description) LIKE @q)");
                parameters.Add("q", "%" + query.Trim().ToLowerInvariant() + "%");
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                if (includeSubcategories)
                {
                    var ids = new List<string> { categoryId };
                    var children = await conn.QueryAsync<string>(
                        "SELECT id FROM categories WHERE parent_id = @categoryId",
                        new { categoryId });
                    ids.AddRange(children);
                    whereParts.Add("p.category_id = ANY(@categoryIds)");
                    parameters.Add("categoryIds", ids.ToArray());
                }
                else
                {
                    whereParts.Add("p.category_id = @categoryId");
                    parameters.Add("categoryId", categoryId);
                }
            }

            var sql = BaseSelect;
            if (whereParts.Count > 0)
                sql += " WHERE " + string.Join(" AND ", whereParts);
            sql += " ORDER BY p.title";

            var rows = await conn.QueryAsync<ProductRow>(sql, parameters);
            return rows.Select(Map).ToList();
        }

        public async Task<ProductModel?> GetById(string id)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<ProductRow>(
                BaseSelect + " WHERE p.id = @id", new { id });
            return row is null ? null : Map(row);
        }

        public async Task<IEnumerable<ProductModel>> GetBySellerId(string sellerId)
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<ProductRow>(
                BaseSelect + " WHERE p.seller_id = @sellerId ORDER BY p.title",
                new { sellerId });
            return rows.Select(Map).ToList();
        }

        public async Task<ProductModel?> GetBySellerIdAndId(string sellerId, string id)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<ProductRow>(
                BaseSelect + " WHERE p.seller_id = @sellerId AND p.id = @id",
                new { sellerId, id });
            return row is null ? null : Map(row);
        }

        public async Task Add(ProductModel product)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
INSERT INTO products (id, title, description, price, stock, category_id, seller_id, seller_name, weight, width, height, length)
VALUES (@Id, @Title, @Description, @Price, @Stock, @CategoryId, @SellerId, @SellerName, @Weight, @Width, @Height, @Length);",
                new
                {
                    product.Id,
                    product.Title,
                    product.Description,
                    product.Price,
                    product.Stock,
                    product.CategoryId,
                    product.SellerId,
                    product.SellerName,
                    product.Weight,
                    product.Dimensions.Width,
                    product.Dimensions.Height,
                    product.Dimensions.Length,
                });
            await ReplaceImages(conn, product);
        }

        public async Task Update(ProductModel product)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
UPDATE products
SET title = @Title,
    description = @Description,
    price = @Price,
    stock = @Stock,
    category_id = @CategoryId,
    seller_name = @SellerName,
    weight = @Weight,
    width = @Width,
    height = @Height,
    length = @Length
WHERE id = @Id;",
                new
                {
                    product.Id,
                    product.Title,
                    product.Description,
                    product.Price,
                    product.Stock,
                    product.CategoryId,
                    product.SellerName,
                    product.Weight,
                    product.Dimensions.Width,
                    product.Dimensions.Height,
                    product.Dimensions.Length,
                });
            await ReplaceImages(conn, product);
        }

        public async Task Delete(string id)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync("DELETE FROM products WHERE id = @id;", new { id });
        }

        private static async Task ReplaceImages(System.Data.IDbConnection conn, ProductModel product)
        {
            await conn.ExecuteAsync(
                "DELETE FROM product_images WHERE product_id = @productId;",
                new { productId = product.Id });

            if (product.Images is null || product.Images.Count == 0) return;

            var rows = product.Images
                .Select((url, position) => new { ProductId = product.Id, Position = position, Url = url })
                .ToList();

            await conn.ExecuteAsync(
                "INSERT INTO product_images (product_id, position, url) VALUES (@ProductId, @Position, @Url);",
                rows);
        }
    }
}
