using System.Reflection;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marketplace.Domain.Settings;

namespace Marketplace.Repository.Data
{
    /// <summary>
    /// Executa as migrações versionadas em <c>Migrations/*.sql</c> (Embedded
    /// Resources) contra o Postgres configurado em
    /// <c>ConnectionStrings:Postgres</c>. Deve ser chamado uma única vez no
    /// startup do processo, antes de o app aceitar requests.
    /// </summary>
    public class DatabaseMigrator(
        IOptions<ConnectionStrings> connectionOptions,
        ILogger<DatabaseMigrator> logger)
    {
        private readonly string _connectionString = connectionOptions.Value.Postgres;
        private readonly ILogger<DatabaseMigrator> _logger = logger;

        public void Migrate()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException(
                    "ConnectionStrings:Postgres não configurada — impossível rodar migrações.");

            EnsureDatabase.For.PostgresqlDatabase(_connectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    name => name.Contains(".Migrations.", StringComparison.OrdinalIgnoreCase)
                            && name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                // Desabilita a substituição de variáveis do DbUp para não confundir
                // com literais que contenham "$" (ex.: hashes BCrypt "$2a$11$...").
                .WithVariablesDisabled()
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                _logger.LogError(result.Error, "Falha ao aplicar migrações do Postgres.");
                throw result.Error;
            }

            _logger.LogInformation(
                "Migrações Postgres aplicadas com sucesso ({Count} scripts).",
                result.Scripts.Count());
        }
    }
}
