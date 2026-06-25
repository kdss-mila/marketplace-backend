using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.DTOs.Seller;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class UpdateProductUseCase(
        ICurrentUserResolver currentUser,
        IProductRepository productRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<ProductModel> Execute(string productId, UpdateProductRequest request)
        {
            var user = await SellerGuard.RequireSeller(_currentUser);

            var product = await _productRepository.GetBySellerIdAndId(user.Id, productId)
                ?? throw new NotFoundException("Produto não encontrado");

            if (request.Title is not null) product.Title = request.Title;
            if (request.Description is not null) product.Description = request.Description;
            if (request.Price is not null) product.Price = request.Price.Value;
            if (request.Stock is not null) product.Stock = request.Stock.Value;
            if (request.CategoryId is not null) product.CategoryId = request.CategoryId;
            if (request.Images is not null) product.Images = request.Images;
            if (request.Weight is not null) product.Weight = request.Weight.Value;
            if (request.Dimensions is not null) product.Dimensions = request.Dimensions;

            await _productRepository.Update(product);
            return product;
        }
    }
}
