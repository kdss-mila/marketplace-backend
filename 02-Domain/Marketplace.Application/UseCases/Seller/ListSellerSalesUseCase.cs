using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class ListSellerSalesUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;

        public async Task<IEnumerable<OrderModel>> Execute()
        {
            var user = await SellerGuard.RequireSeller(_currentUser);
            return await _orderRepository.GetBySellerIdAndStatuses(
                user.Id,
                OrderStatus.Pago,
                OrderStatus.Enviado,
                OrderStatus.Entregue);
        }
    }
}
