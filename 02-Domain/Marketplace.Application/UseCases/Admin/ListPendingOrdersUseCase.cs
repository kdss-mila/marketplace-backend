using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Admin
{
    public class ListPendingOrdersUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;

        public async Task<IEnumerable<OrderModel>> Execute()
        {
            await AdminGuard.RequireAdmin(_currentUser);
            return await _orderRepository.GetByStatus(OrderStatus.EmAnalise);
        }
    }
}
