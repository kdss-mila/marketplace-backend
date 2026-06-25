using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Orders
{
    public class UploadReceiptUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository,
        IFileStorageService fileStorage)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IFileStorageService _fileStorage = fileStorage;

        public async Task<OrderModel> Execute(UploadReceiptRequest request)
        {
            var user = await _currentUser.GetCurrent() ?? throw new UnauthorizedException();

            var order = await _orderRepository.GetById(request.OrderId)
                ?? throw new NotFoundException("Pedido não encontrado");

            if (order.BuyerId != user.Id) throw new ForbiddenException();

            var url = await _fileStorage.SaveAsync(request.FileStream, request.FileName, "receipts");

            order.ReceiptUrl = url;
            order.Status = OrderStatus.EmAnalise;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.Update(order);
            return order;
        }
    }
}
