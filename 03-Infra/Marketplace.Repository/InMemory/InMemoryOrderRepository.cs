using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Repository.InMemory
{
    public class InMemoryOrderRepository(InMemoryStore store) : IOrderRepository
    {
        private readonly InMemoryStore _store = store;

        public Task<OrderModel?> GetById(string id)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult(_store.Orders.FirstOrDefault(o => o.Id == id));
            }
        }

        public Task<IEnumerable<OrderModel>> GetByBuyerId(string buyerId)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<OrderModel>>(
                    _store.Orders.Where(o => o.BuyerId == buyerId).ToList());
            }
        }

        public Task<IEnumerable<OrderModel>> GetBySellerIdAndStatuses(string sellerId, params OrderStatus[] statuses)
        {
            lock (_store.SyncRoot)
            {
                var set = new HashSet<OrderStatus>(statuses);
                return Task.FromResult<IEnumerable<OrderModel>>(
                    _store.Orders.Where(o => o.SellerId == sellerId && set.Contains(o.Status)).ToList());
            }
        }

        public Task<IEnumerable<OrderModel>> GetByStatus(OrderStatus status)
        {
            lock (_store.SyncRoot)
            {
                return Task.FromResult<IEnumerable<OrderModel>>(
                    _store.Orders.Where(o => o.Status == status).ToList());
            }
        }

        public Task Add(OrderModel order)
        {
            lock (_store.SyncRoot)
            {
                _store.Orders.Add(order);
            }
            return Task.CompletedTask;
        }

        public Task Update(OrderModel order)
        {
            lock (_store.SyncRoot)
            {
                var index = _store.Orders.FindIndex(o => o.Id == order.Id);
                if (index >= 0) _store.Orders[index] = order;
            }
            return Task.CompletedTask;
        }
    }
}
