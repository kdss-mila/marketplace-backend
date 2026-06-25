using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryUserRepository(InMemoryStore store) : IUserRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<IEnumerable<UserModel>> GetAll()
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<UserModel>>(_store.Users.ToList());
            }
        }

        public Task<UserModel?> GetById(string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Users.FirstOrDefault(u => u.Id == id));
            }
        }

        public Task<UserModel?> GetByEmail(string email)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(
                    _store.Users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public Task Add(UserModel user)
        {
            lock (_store.SyncRoot)
            {
                _store.Users.Add(user);
            }
            return Task.CompletedTask;
        }

        public Task Update(UserModel user)
        {
            lock (_store.SyncRoot)
            {
                var index = _store.Users.FindIndex(u => u.Id == user.Id);
                if (index >= 0) _store.Users[index] = user;
            }
            return Task.CompletedTask;
        }
    }
}
