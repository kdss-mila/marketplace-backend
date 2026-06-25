using Microsoft.OpenApi.Models;

namespace Marketplace.Api.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithFakeBearer(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Marketplace API",
                    Version = "v1",
                    Description = "API do Marketplace MVP — dados em memória, autenticação via fake token.",
                });

                // Esquema de "Bearer" apenas para permitir colar o token (ex.: token-admin)
                // no botão Authorize do Swagger UI. NÃO faz validação criptográfica.
                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "FakeToken",
                    Description = "Cole um dos tokens fixos (token-buyer / token-seller / token-admin) ou um token devolvido pelo /api/auth/login.",
                };

                options.AddSecurityDefinition("Bearer", scheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
