using Marketplace.Domain.Interface.Repository;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryTokenRepository(InMemoryStore store) : ITokenRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<string> Create(string userId)
        {
            var token = Guid.NewGuid().ToString("N");
            lock (_store.SyncRoot)
            {
                _store.Tokens[token] = userId;
            }
            return Task.FromResult(token);
        }

        public Task<string?> GetUserId(string token)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Tokens.TryGetValue(token, out var userId) ? userId : null);
            }
        }

        public Task<string?> FindExistingTokenForUser(string userId)
        {
            lock (_store.SyncRoot)
            {
                var match = _store.Tokens.FirstOrDefault(kvp => kvp.Value == userId);
                return Task.FromResult<string?>(string.IsNullOrEmpty(match.Key) ? null : match.Key);
            }
        }
    }
}
