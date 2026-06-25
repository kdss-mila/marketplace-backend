using Marketplace.Domain.Enums;
using Marketplace.Domain.Model;

namespace Marketplace.Domain.Interface.Repository
{
    public interface IOrderRepository
    {
        Task<OrderModel?> GetById(string id);
        Task<IEnumerable<OrderModel>> GetByBuyerId(string buyerId);
        Task<IEnumerable<OrderModel>> GetBySellerIdAndStatuses(string sellerId, params OrderStatus[] statuses);
        Task<IEnumerable<OrderModel>> GetByStatus(OrderStatus status);
        Task Add(OrderModel order);
        Task Update(OrderModel order);
    }
}
