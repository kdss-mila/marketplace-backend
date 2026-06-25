using FluentValidation;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Seller;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class SetTrackingCodeUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository,
        IValidator<SetTrackingCodeRequest> validator)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IValidator<SetTrackingCodeRequest> _validator = validator;

        public async Task<OrderModel> Execute(string orderId, SetTrackingCodeRequest request)
        {
            var user = await SellerGuard.RequireSeller(_currentUser);
            await ValidatorRunner.EnsureValid(_validator, request);

            var order = await _orderRepository.GetById(orderId);
            if (order is null || order.SellerId != user.Id)
                throw new NotFoundException("Venda não encontrada");

            order.TrackingCode = request.TrackingCode;
            order.Status = OrderStatus.Enviado;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.Update(order);
            return order;
        }
    }
}
