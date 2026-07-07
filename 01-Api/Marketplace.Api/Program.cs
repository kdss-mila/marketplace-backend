using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Marketplace.Api.Extensions;
using Marketplace.Api.Middlewares;
using Marketplace.Domain.Settings;
using Marketplace.Repository.Data;
using Marketplace.Setup;

const string CorsPolicyName = "MarketplaceCors";

// Fallback usado em dev quando "Cors:AllowedOrigins" não está preenchido nem
// nos appsettings, nem em variáveis de ambiente (Cors__AllowedOrigins__0=...).
// Em produção você deve preencher a lista via env var para incluir a URL do
// Static Site (ex.: https://marketplace-frontend.onrender.com).
string[] defaultDevOrigins =
[
    "http://localhost:5173",
    "http://localhost:4173",
];

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Frontend espera camelCase em todos os DTOs (axios + types do Vite).
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Mantém campos null explícitos (parentId: null, receiptUrl: null, etc.)
        // para preservar paridade com o shape devolvido pelo mock MSW do frontend.
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        // Não escapa caracteres não-ASCII (acentos PT-BR como "Em Análise" precisam vir literais).
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.IocConfiguration(builder.Configuration);

builder.Services.AddHttpClient();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Seção 'JwtSettings' ausente do appsettings.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerWithFakeBearer();

var configuredOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

var allowedOrigins = configuredOrigins is { Length: > 0 }
    ? configuredOrigins
    : defaultDevOrigins;

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

// Executa as migrações Postgres (DbUp) antes de aceitar requests.
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    migrator.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStaticFiles();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
