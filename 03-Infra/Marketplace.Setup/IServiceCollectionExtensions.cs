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
using Marketplace.Repository.InMemory;

namespace Marketplace.Setup
{
    /// <summary>
    /// Composition root no estilo LolOxiBot: um único ponto público
    /// (<see cref="IocConfiguration"/>) agrega métodos privados Register* por
    /// camada/conceito. Quando o Postgres entrar, basta trocar
    /// <c>RegisterRepositoriesInMemory</c> por <c>RegisterRepositoriesPostgres</c>.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection IocConfiguration(this IServiceCollection services, IConfiguration configuration) => services
            .RegisterSettings(configuration)
            .RegisterInMemoryStore()
            .RegisterRepositoriesInMemory()
            .RegisterInfrastructureServices()
            .RegisterUseCases()
            .RegisterValidators();

        private static IServiceCollection RegisterSettings(this IServiceCollection services, IConfiguration configuration) => services
            .Configure<PlatformSettings>(configuration.GetSection("PlatformSettings"))
            .Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

        private static IServiceCollection RegisterInMemoryStore(this IServiceCollection services) => services
            .AddSingleton<InMemoryStore>();

        private static IServiceCollection RegisterRepositoriesInMemory(this IServiceCollection services) => services
            .AddScoped<IUserRepository, InMemoryUserRepository>()
            .AddScoped<IProductRepository, InMemoryProductRepository>()
            .AddScoped<ICategoryRepository, InMemoryCategoryRepository>()
            .AddScoped<IOrderRepository, InMemoryOrderRepository>()
            .AddScoped<IRepasseRepository, InMemoryRepasseRepository>()
            .AddScoped<ITokenRepository, InMemoryTokenRepository>();

        // Reservado para quando o Postgres estiver pronto.
        // private static IServiceCollection RegisterRepositoriesPostgres(this IServiceCollection services) => services
        //     .AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>()
        //     .AddScoped<IUserRepository, PostgresUserRepository>()
        //     ...

        private static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services) => services
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserResolver, CurrentUserResolver>()
            .AddSingleton<IShippingCalculator, MockShippingCalculator>()
            .AddScoped<IFileStorageService, LocalFileStorageService>();

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
            // Seller
            .AddScoped<ListSellerProductsUseCase>()
            .AddScoped<CreateProductUseCase>()
            .AddScoped<UpdateProductUseCase>()
            .AddScoped<DeleteProductUseCase>()
            .AddScoped<ListSellerSalesUseCase>()
            .AddScoped<SetTrackingCodeUseCase>()
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
