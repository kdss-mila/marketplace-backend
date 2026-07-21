using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Storage
{
    public class R2StorageService : IFileStorageService
    {
        private readonly R2Settings _settings;

        public R2StorageService(IOptions<R2Settings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> SaveAsync(Stream content, string originalFileName, string subFolder)
        {
            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
            var key = $"{subFolder}/{Guid.NewGuid():N}{ext}";

            var credentials = new BasicAWSCredentials(_settings.AccessKeyId, _settings.SecretAccessKey);
            var config = new AmazonS3Config
            {
                ServiceURL = $"https://{_settings.AccountId}.r2.cloudflarestorage.com",
                AuthenticationRegion = "auto",
                ForcePathStyle = true,
            };

            using var client = new AmazonS3Client(credentials, config);

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = content,
                ContentType = GetContentType(ext),
                AutoCloseStream = false,
            };

            await client.PutObjectAsync(request);

            return $"{_settings.PublicUrl.TrimEnd('/')}/{key}";
        }

        private static string GetContentType(string ext) => ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream",
        };
    }
}
