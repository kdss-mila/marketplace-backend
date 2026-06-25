using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetAll();
        Task<IEnumerable<ProductModel>> Search(string? query, string? categoryId);
        Task<ProductModel?> GetById(string id);
        Task<IEnumerable<ProductModel>> GetBySellerId(string sellerId);
        Task<ProductModel?> GetBySellerIdAndId(string sellerId, string id);
        Task Add(ProductModel product);
        Task Update(ProductModel product);
        Task Delete(string id);
    }
}
