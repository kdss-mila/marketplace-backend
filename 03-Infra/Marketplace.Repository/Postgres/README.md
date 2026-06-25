# Pasta `Postgres/` (placeholder)

Esta pasta está vazia e ficará reservada para as futuras implementações dos
repositórios em Dapper + Npgsql:

- `PostgresUserRepository.cs`
- `PostgresProductRepository.cs`
- `PostgresCategoryRepository.cs`
- `PostgresOrderRepository.cs`
- `PostgresRepasseRepository.cs`
- `PostgresTokenRepository.cs`

Quando o banco PostgreSQL estiver pronto:

1. Preencher `ConnectionStrings:Postgress` em `appsettings.json`.
2. Implementar cada repositório utilizando `IDbConnectionFactory` injetado.
3. No `Marketplace.Setup/IServiceCollectionExtensions.cs`, trocar a chamada
   `.RegisterRepositoriesInMemory()` por `.RegisterRepositoriesPostgres()`.
