using FluentValidation;
using Marketplace.Application.Common.Validation;
using Marketplace.Application.DTOs.Seller;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class CreateProductUseCase(
        ICurrentUserResolver currentUser,
        IProductRepository productRepository,
        IValidator<CreateProductRequest> validator)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IValidator<CreateProductRequest> _validator = validator;

        public async Task<ProductModel> Execute(CreateProductRequest request)
        {
            var user = await SellerGuard.RequireSeller(_currentUser);
            await ValidatorRunner.EnsureValid(_validator, request);

            var product = new ProductModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                Images = request.Images,
                Weight = request.Weight,
                Dimensions = request.Dimensions,
                SellerId = user.Id,
                SellerName = user.Name,
            };

            await _productRepository.Add(product);
            return product;
        }
    }
}
