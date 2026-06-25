using Microsoft.Extensions.Options;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;
using Marketplace.Domain.Settings;

namespace Marketplace.Application.UseCases.Admin
{
    public class ApproveOrderUseCase(
        ICurrentUserResolver currentUser,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IRepasseRepository repasseRepository,
        IOptions<PlatformSettings> platformSettings)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IRepasseRepository _repasseRepository = repasseRepository;
        private readonly PlatformSettings _platformSettings = platformSettings.Value;

        public async Task<OrderModel> Execute(string orderId)
        {
            await AdminGuard.RequireAdmin(_currentUser);

            var order = await _orderRepository.GetById(orderId)
                ?? throw new NotFoundException("Pedido não encontrado");

            var product = await _productRepository.GetById(order.ProductId);
            if (product is not null)
            {
                product.Stock = Math.Max(0, product.Stock - order.Quantity);
                await _productRepository.Update(product);
            }

            order.Status = OrderStatus.Pago;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.Update(order);

            var commission = order.ProductPrice * _platformSettings.CommissionRate;
            var repasse = new RepasseModel
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = order.Id,
                SellerId = order.SellerId,
                SellerName = order.SellerName,
                ProductAmount = order.ProductPrice,
                ShippingAmount = order.ShippingCost,
                Commission = commission,
                NetAmount = order.ProductPrice + order.ShippingCost - commission,
                Paid = false,
                CreatedAt = DateTime.UtcNow,
            };

            await _repasseRepository.Add(repasse);
            return order;
        }
    }
}
