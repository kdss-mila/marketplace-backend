using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Application.UseCases.Admin;
using Marketplace.Application.UseCases.Auth;
using Marketplace.Application.UseCases.Catalog;
using Marketplace.Application.UseCases.Orders;
using Marketplace.Application.UseCases.Seller;
using Marketplace.Application.UseCases.Shipping;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Settings;
using Marketplace.Infrastructure.Auth;
using Marketplace.Infrastructure.Shipping;
using Marketplace.Infrastructure.Storage;
using Marketplace.Repository.Data;
using Marketplace.Repository.Postgres;

namespace Marketplace.Setup
{
    /// <summary>
    /// Composition root: um único ponto público (<see cref="IocConfiguration"/>)
    /// agrega métodos privados Register* por camada. Persistência é 100%
    /// Postgres (Dapper) — a pasta InMemory/ foi retirada com a migração.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection IocConfiguration(this IServiceCollection services, IConfiguration configuration) => services
            .RegisterSettings(configuration)
            .RegisterRepositoriesPostgres()
            .RegisterInfrastructureServices()
            .RegisterUseCases()
            .RegisterValidators();

        private static IServiceCollection RegisterSettings(this IServiceCollection services, IConfiguration configuration) => services
            .Configure<PlatformSettings>(configuration.GetSection("PlatformSettings"))
            .Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"))
            .Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
            .Configure<R2Settings>(configuration.GetSection("R2Settings"));

        private static IServiceCollection RegisterRepositoriesPostgres(this IServiceCollection services) => services
            .AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>()
            .AddSingleton<DatabaseMigrator>()
            .AddScoped<IUserRepository, PostgresUserRepository>()
            .AddScoped<IProductRepository, PostgresProductRepository>()
            .AddScoped<ICategoryRepository, PostgresCategoryRepository>()
            .AddScoped<IOrderRepository, PostgresOrderRepository>()
            .AddScoped<IRepasseRepository, PostgresRepasseRepository>();

        private static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services) => services
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserResolver, CurrentUserResolver>()
            .AddSingleton<IJwtService, JwtService>()
            .AddSingleton<IShippingCalculator, MockShippingCalculator>()
            .AddSingleton<IFileStorageService, R2StorageService>();

        private static IServiceCollection RegisterUseCases(this IServiceCollection services) => services
            // Auth
            .AddScoped<LoginUseCase>()
            .AddScoped<RegisterUseCase>()
            .AddScoped<GetMeUseCase>()
            // Catalog
            .AddScoped<ListProductsUseCase>()
            .AddScoped<GetProductByIdUseCase>()
            .AddScoped<ListCategoriesUseCase>()
            // Orders
            .AddScoped<CreateOrderUseCase>()
            .AddScoped<UploadReceiptUseCase>()
            .AddScoped<ListMyOrdersUseCase>()
            // Shipping
            .AddScoped<CalculateShippingUseCase>()
            .AddScoped<LookupAddressByCepUseCase>()
            // Seller
            .AddScoped<ListSellerProductsUseCase>()
            .AddScoped<CreateProductUseCase>()
            .AddScoped<UpdateProductUseCase>()
            .AddScoped<DeleteProductUseCase>()
            .AddScoped<ListSellerSalesUseCase>()
            .AddScoped<SetTrackingCodeUseCase>()
            .AddScoped<UploadProductImageUseCase>()
            .AddScoped<CompleteSellerOnboardingUseCase>()
            // Admin
            .AddScoped<ListUsersUseCase>()
            .AddScoped<BanUserUseCase>()
            .AddScoped<ListAdminCategoriesUseCase>()
            .AddScoped<CreateCategoryUseCase>()
            .AddScoped<UpdateCategoryUseCase>()
            .AddScoped<DeleteCategoryUseCase>()
            .AddScoped<ListPendingOrdersUseCase>()
            .AddScoped<ApproveOrderUseCase>()
            .AddScoped<ListRepassesUseCase>()
            .AddScoped<MarkRepassePaidUseCase>();

        private static IServiceCollection RegisterValidators(this IServiceCollection services) => services
            .AddValidatorsFromAssemblyContaining<LoginUseCase>();
    }
}
