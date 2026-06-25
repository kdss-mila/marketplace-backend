using System.Data;

namespace Marketplace.Repository.Data
{
    /// <summary>
    /// Abstrai a criação de conexões com o banco. Permite trocar a implementação
    /// de Postgres (Npgsql) por outra (ex.: SqlServer, SQLite) sem afetar Repositórios.
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
