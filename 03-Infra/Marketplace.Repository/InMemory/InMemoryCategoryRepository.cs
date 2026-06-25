using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryCategoryRepository(InMemoryStore store) : ICategoryRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<IEnumerable<CategoryModel>> GetAll()
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<CategoryModel>>(_store.Categories.ToList());
            }
        }

        public Task<CategoryModel?> GetById(string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Categories.FirstOrDefault(c => c.Id == id));
            }
        }

        public Task Add(CategoryModel category)
        {
            lock (_store.SyncRoot)
            {
                _store.Categories.Add(category);
            }
            return Task.CompletedTask;
        }

        public Task Update(CategoryModel category)
        {
            lock (_store.SyncRoot)
            {
                var index = _store.Categories.FindIndex(c => c.Id == category.Id);
                if (index >= 0) _store.Categories[index] = category;
            }
            return Task.CompletedTask;
        }

        public Task Delete(string id)
        {
            lock (_store.SyncRoot)
            {
                _store.Categories.RemoveAll(c => c.Id == id);
            }
            return Task.CompletedTask;
        }
    }
}
