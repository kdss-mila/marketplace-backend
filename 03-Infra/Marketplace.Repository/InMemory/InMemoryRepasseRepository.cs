using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryRepasseRepository(InMemoryStore store) : IRepasseRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<IEnumerable<RepasseModel>> GetAll()
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<RepasseModel>>(_store.Repasses.ToList());
            }
        }

        public Task<RepasseModel?> GetById(string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Repasses.FirstOrDefault(r => r.Id == id));
            }
        }

        public Task Add(RepasseModel repasse)
        {
            lock (_store.SyncRoot)
            {
                _store.Repasses.Add(repasse);
            }
            return Task.CompletedTask;
        }

        public Task Update(RepasseModel repasse)
        {
            lock (_store.SyncRoot)
            {
                var index = _store.Repasses.FindIndex(r => r.Id == repasse.Id);
                if (index >= 0) _store.Repasses[index] = repasse;
            }
            return Task.CompletedTask;
        }
    }
}
