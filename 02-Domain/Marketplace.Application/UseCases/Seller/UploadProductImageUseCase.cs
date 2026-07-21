using Marketplace.Application.DTOs.Seller;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Application.UseCases.Seller
{
    public class UploadProductImageUseCase(IFileStorageService fileStorage)
    {
        private readonly IFileStorageService _fileStorage = fileStorage;

        private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/webp"];
        private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<UploadImageResponse> Execute(Stream content, string fileName, string contentType, long size)
        {
            if (content is null || size == 0)
                throw new ArgumentException("Nenhum arquivo enviado.");

            if (!AllowedTypes.Contains(contentType))
                throw new ArgumentException("Formato inválido. Use JPEG, PNG ou WebP.");

            if (size > MaxBytes)
                throw new ArgumentException("Arquivo muito grande. Limite de 5 MB.");

            var url = await _fileStorage.SaveAsync(content, fileName, "produtos");

            return new UploadImageResponse(url);
        }
    }
}
