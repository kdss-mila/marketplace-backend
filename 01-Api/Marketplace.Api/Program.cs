using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Marketplace.Api.Extensions;
using Marketplace.Api.Middlewares;
using Marketplace.Setup;

const string DevCorsPolicy = "DevCors";

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

builder.Services.AddSwaggerWithFakeBearer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(DevCorsPolicy, policy => policy
        .WithOrigins("http://localhost:5173", "http://localhost:4173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStaticFiles();

app.UseCors(DevCorsPolicy);

app.MapControllers();

app.Run();
