using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Infrastructure.Storage
{
    /// <summary>
    /// Persiste arquivos enviados em <c>wwwroot/uploads/{subFolder}</c> e devolve uma URL
    /// relativa servível pelo middleware <c>UseStaticFiles</c>. Sucessor natural: AWS S3.
    /// </summary>
    public class LocalFileStorageService(
        IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor) : IFileStorageService
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<string> SaveAsync(Stream content, string originalFileName, string subFolder)
        {
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
            }

            var folder = Path.Combine(webRoot, "uploads", subFolder);
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(originalFileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            await using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs);
            }

            var relativeUrl = $"/uploads/{subFolder}/{fileName}";

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is not null)
            {
                return $"{request.Scheme}://{request.Host}{relativeUrl}";
            }

            return relativeUrl;
        }
    }
}
