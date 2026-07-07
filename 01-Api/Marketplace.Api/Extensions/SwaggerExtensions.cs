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
                    Description = "API do Marketplace MVP — persistência Postgres, autenticação JWT (HS256).",
                });

                // Esquema Bearer para colar o JWT devolvido por /api/auth/login
                // no botão Authorize do Swagger UI.
                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Cole o token JWT devolvido por /api/auth/login (sem o prefixo Bearer).",
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
