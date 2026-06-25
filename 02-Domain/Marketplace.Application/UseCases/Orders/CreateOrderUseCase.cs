using FluentValidation;
using Microsoft.Extensions.Options;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;
using Marketplace.Domain.Settings;

namespace Marketplace.Application.UseCases.Orders
{
    public class CreateOrderUseCase(
        ICurrentUserResolver currentUser,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IValidator<CreateOrderRequest> validator,
        IOptions<PlatformSettings> platformSettings)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IValidator<CreateOrderRequest> _validator = validator;
        private readonly PlatformSettings _platformSettings = platformSettings.Value;

        public async Task<CreateOrderResponse> Execute(CreateOrderRequest request)
        {
            await ValidatorRunner.EnsureValid(_validator, request);

            var user = await _currentUser.GetCurrent() ?? throw new UnauthorizedException();

            var product = await _productRepository.GetById(request.ProductId)
                ?? throw new NotFoundException("Produto não encontrado");

            if (product.Stock < request.Quantity)
                throw new ConflictException("Estoque insuficiente");

            var productPrice = product.Price * request.Quantity;
            var now = DateTime.UtcNow;

            var order = new OrderModel
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = user.Id,
                BuyerName = user.Name,
                ProductId = product.Id,
                ProductTitle = product.Title,
                SellerId = product.SellerId,
                SellerName = product.SellerName,
                Quantity = request.Quantity,
                ProductPrice = productPrice,
                ShippingCost = request.ShippingCost,
                Total = productPrice + request.ShippingCost,
                Status = OrderStatus.AguardandoComprovante,
                Address = request.Address,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _orderRepository.Add(order);

            return new CreateOrderResponse { Order = order, PixKey = _platformSettings.PixKey };
        }
    }
}
