namespace Marketplace.Domain.Interface.Service
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream content, string originalFileName, string subFolder);
    }
}
