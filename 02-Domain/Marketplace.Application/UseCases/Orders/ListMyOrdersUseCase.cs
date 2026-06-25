using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Orders
{
    public class ListMyOrdersUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;

        public async Task<IEnumerable<OrderModel>> Execute()
        {
            var user = await _currentUser.GetCurrent() ?? throw new UnauthorizedException();
            return await _orderRepository.GetByBuyerId(user.Id);
        }
    }
}
