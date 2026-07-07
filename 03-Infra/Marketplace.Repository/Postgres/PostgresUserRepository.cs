using Dapper;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;
using Marketplace.Repository.Data;

namespace Marketplace.Repository.Postgres
{
    /// <summary>
    /// Persistência de usuários em Postgres via Dapper. Retorna
    /// <see cref="UserModel"/> com <see cref="SellerProfileModel"/> agregado
    /// quando existir linha em seller_profiles.
    /// </summary>
    public class PostgresUserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private const string BaseSelect = @"
SELECT
    u.id            AS Id,
    u.email         AS Email,
    u.name          AS Name,
    u.cpf           AS Cpf,
    u.role          AS RoleRaw,
    u.banned        AS Banned,
    u.password_hash AS PasswordHash,
    sp.document_type       AS DocumentTypeRaw,
    sp.document            AS Document,
    sp.pix_key             AS PixKey,
    sp.origin_cep          AS OriginCep,
    sp.origin_address      AS OriginAddress,
    sp.onboarding_complete AS OnboardingComplete
FROM users u
LEFT JOIN seller_profiles sp ON sp.user_id = u.id";

        private sealed class UserRow
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string RoleRaw { get; set; } = "buyer";
            public bool Banned { get; set; }
            public string PasswordHash { get; set; } = string.Empty;
            public string? DocumentTypeRaw { get; set; }
            public string? Document { get; set; }
            public string? PixKey { get; set; }
            public string? OriginCep { get; set; }
            public string? OriginAddress { get; set; }
            public bool? OnboardingComplete { get; set; }
        }

        private static UserModel Map(UserRow row)
        {
            var user = new UserModel
            {
                Id = row.Id,
                Email = row.Email,
                Name = row.Name,
                Cpf = row.Cpf,
                Role = ParseRole(row.RoleRaw),
                Banned = row.Banned,
                PasswordHash = row.PasswordHash,
            };

            if (row.Document is not null)
            {
                user.SellerProfile = new SellerProfileModel
                {
                    DocumentType = ParseDocumentType(row.DocumentTypeRaw ?? "cpf"),
                    Document = row.Document,
                    PixKey = row.PixKey ?? string.Empty,
                    OriginCep = row.OriginCep ?? string.Empty,
                    OriginAddress = row.OriginAddress ?? string.Empty,
                    OnboardingComplete = row.OnboardingComplete ?? false,
                };
            }

            return user;
        }

        private static UserRole ParseRole(string raw) => raw?.ToLowerInvariant() switch
        {
            "seller" => UserRole.Seller,
            "admin" => UserRole.Admin,
            _ => UserRole.Buyer,
        };

        private static DocumentType ParseDocumentType(string raw) =>
            string.Equals(raw, "cnpj", StringComparison.OrdinalIgnoreCase)
                ? DocumentType.Cnpj
                : DocumentType.Cpf;

        private static string SerializeRole(UserRole role) => role switch
        {
            UserRole.Seller => "seller",
            UserRole.Admin => "admin",
            _ => "buyer",
        };

        private static string SerializeDocumentType(DocumentType type) => type switch
        {
            DocumentType.Cnpj => "cnpj",
            _ => "cpf",
        };

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            using var conn = _connectionFactory.Create();
            var rows = await conn.QueryAsync<UserRow>(BaseSelect + " ORDER BY u.name");
            return rows.Select(Map).ToList();
        }

        public async Task<UserModel?> GetById(string id)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<UserRow>(
                BaseSelect + " WHERE u.id = @id", new { id });
            return row is null ? null : Map(row);
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            using var conn = _connectionFactory.Create();
            var row = await conn.QueryFirstOrDefaultAsync<UserRow>(
                BaseSelect + " WHERE LOWER(u.email) = LOWER(@email)", new { email });
            return row is null ? null : Map(row);
        }

        public async Task Add(UserModel user)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
INSERT INTO users (id, email, name, cpf, role, banned, password_hash)
VALUES (@Id, @Email, @Name, @Cpf, @Role, @Banned, @PasswordHash);",
                new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Cpf,
                    Role = SerializeRole(user.Role),
                    user.Banned,
                    user.PasswordHash,
                });

            if (user.SellerProfile is not null)
                await UpsertSellerProfile(conn, user);
        }

        public async Task Update(UserModel user)
        {
            using var conn = _connectionFactory.Create();
            await conn.ExecuteAsync(@"
UPDATE users
SET email = @Email,
    name = @Name,
    cpf = @Cpf,
    role = @Role,
    banned = @Banned,
    password_hash = @PasswordHash
WHERE id = @Id;",
                new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Cpf,
                    Role = SerializeRole(user.Role),
                    user.Banned,
                    user.PasswordHash,
                });

            if (user.SellerProfile is not null)
                await UpsertSellerProfile(conn, user);
            else
                await conn.ExecuteAsync("DELETE FROM seller_profiles WHERE user_id = @Id;", new { user.Id });
        }

        private static Task UpsertSellerProfile(System.Data.IDbConnection conn, UserModel user)
        {
            var profile = user.SellerProfile!;
            return conn.ExecuteAsync(@"
INSERT INTO seller_profiles (user_id, document_type, document, pix_key, origin_cep, origin_address, onboarding_complete)
VALUES (@UserId, @DocumentType, @Document, @PixKey, @OriginCep, @OriginAddress, @OnboardingComplete)
ON CONFLICT (user_id) DO UPDATE SET
    document_type = EXCLUDED.document_type,
    document = EXCLUDED.document,
    pix_key = EXCLUDED.pix_key,
    origin_cep = EXCLUDED.origin_cep,
    origin_address = EXCLUDED.origin_address,
    onboarding_complete = EXCLUDED.onboarding_complete;",
                new
                {
                    UserId = user.Id,
                    DocumentType = SerializeDocumentType(profile.DocumentType),
                    profile.Document,
                    profile.PixKey,
                    profile.OriginCep,
                    profile.OriginAddress,
                    profile.OnboardingComplete,
                });
        }
    }
}
