using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using Marketplace.Domain.Settings;

namespace Marketplace.Repository.Data
{
    /// <summary>
    /// Implementação Postgres do <see cref="IDbConnectionFactory"/>.
    /// Não está em uso enquanto o Postgres não estiver configurado.
    /// Quando as credenciais forem fornecidas, basta:
    ///   1. Preencher "ConnectionStrings:Postgress" no appsettings.json
    ///   2. Trocar RegisterRepositoriesInMemory() por RegisterRepositoriesPostgres() no Setup.
    /// </summary>
    public class NpgsqlConnectionFactory(IOptions<ConnectionStrings> options) : IDbConnectionFactory
    {
        private readonly string _connectionString = options.Value.Postgress;

        public IDbConnection Create() => new NpgsqlConnection(_connectionString);
    }
}
