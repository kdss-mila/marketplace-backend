using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryProductRepository(InMemoryStore store) : IProductRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<IEnumerable<ProductModel>> GetAll()
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<ProductModel>>(_store.Products.ToList());
            }
        }

        public Task<IEnumerable<ProductModel>> Search(string? query, string? categoryId)
        {
            lock (_store.SyncRoot)
            {
                IEnumerable<ProductModel> products = _store.Products.ToList();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    var q = query.Trim().ToLowerInvariant();
                    products = products.Where(p =>
                        p.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(q, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    products = products.Where(p => p.CategoryId == categoryId);
                }

                return Task.FromResult(products.ToList().AsEnumerable());
            }
        }

        public Task<ProductModel?> GetById(string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Products.FirstOrDefault(p => p.Id == id));
            }
        }

        public Task<IEnumerable<ProductModel>> GetBySellerId(string sellerId)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<ProductModel>>(
                    _store.Products.Where(p => p.SellerId == sellerId).ToList());
            }
        }

        public Task<ProductModel?> GetBySellerIdAndId(string sellerId, string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Products.FirstOrDefault(p => p.SellerId == sellerId && p.Id == id));
            }
        }

        public Task Add(ProductModel product)
        {
            lock (_store.SyncRoot)
            {
                _store.Products.Add(product);
            }
            return Task.CompletedTask;
        }

        public Task Update(ProductModel product)
        {
            lock (_store.SyncRoot)
            {
                var index = _store.Products.FindIndex(p => p.Id == product.Id);
                if (index >= 0) _store.Products[index] = product;
            }
            return Task.CompletedTask;
        }

        public Task Delete(string id)
        {
            lock (_store.SyncRoot)
            {
                _store.Products.RemoveAll(p => p.Id == id);
            }
            return Task.CompletedTask;
        }
    }
}
