using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Storage
{
    public class R2StorageService : IFileStorageService, IDisposable
    {
        private readonly AmazonS3Client _client;
        private readonly R2Settings _settings;

        public R2StorageService(IOptions<R2Settings> options)
        {
            _settings = options.Value;

            var credentials = new BasicAWSCredentials(_settings.AccessKeyId, _settings.SecretAccessKey);
            var config = new AmazonS3Config
            {
                ServiceURL = $"https://{_settings.AccountId}.r2.cloudflarestorage.com",
                AuthenticationRegion = "auto",
                ForcePathStyle = true,
            };

            _client = new AmazonS3Client(credentials, config);
        }

        public async Task<string> SaveAsync(Stream content, string originalFileName, string subFolder)
        {
            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
            var key = $"{subFolder}/{Guid.NewGuid():N}{ext}";

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = content,
                ContentType = GetContentType(ext),
                AutoCloseStream = false,
                // R2 não suporta checksum em partes — desabilita validação padrão do SDK
                DisableDefaultChecksumValidation = true,
                DisablePayloadSigning = true,
            };

            await _client.PutObjectAsync(request);

            return $"{_settings.PublicUrl.TrimEnd('/')}/{key}";
        }

        public void Dispose() => _client.Dispose();

        private static string GetContentType(string ext) => ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream",
        };
    }
}
