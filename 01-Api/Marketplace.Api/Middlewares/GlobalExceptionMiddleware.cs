using System.Text.Json;
using Marketplace.Application.Common.Exceptions;

namespace Marketplace.Api.Middlewares
{
    /// <summary>
    /// Converte exceções da camada Application em respostas HTTP no formato
    /// que o frontend espera: { "message": "..." }.
    /// Espelha o shape de marketplace-frontend/src/lib/mocks/handlers/index.ts.
    /// </summary>
    public class GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                await WriteResponse(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);
                await WriteResponse(context, StatusCodes.Status500InternalServerError, "Erro interno");
            }
        }

        private static async Task WriteResponse(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted) return;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = JsonSerializer.Serialize(
                new { message },
                context.RequestServices.GetService<JsonSerializerOptions>() ?? new JsonSerializerOptions());

            await context.Response.WriteAsync(payload);
        }
    }
}
