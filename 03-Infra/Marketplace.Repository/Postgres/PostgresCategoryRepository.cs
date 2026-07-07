using Dapper;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;
using Marketplace.Repository.Data;

namespace Marketplace.Repository.Postgres
{
    public class PostgresCategoryRepository(IDbConnectionFactory connectionFactory) : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private const string BaseSelect =
            "SELECT id AS Id, name AS Name, parent_id AS ParentId FROM categories";

        public async Task<IEnumerable<CategoryModel>> GetAll()
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<CategoryModel>(BaseSelect + " ORDER BY name");
            return rows.ToList();
        }

        public async Task<CategoryModel?> GetById(string id)
        {
            using var conn = _connectionFactory.Create();
            return await conn.QueryFirstOrDefaultAsync<CategoryModel>(
                BaseSelect + " WHERE id = @id", new { id });
        }

        public async Task Add(CategoryModel category)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(
                "INSERT INTO categories (id, name, parent_id) VALUES (@Id, @Name, @ParentId);",
                category);
        }

        public async Task Update(CategoryModel category)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(
                "UPDATE categories SET name = @Name, parent_id = @ParentId WHERE id = @Id;",
                category);
        }

        public async Task Delete(string id)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync("DELETE FROM categories WHERE id = @id;", new { id });
        }
    }
}
