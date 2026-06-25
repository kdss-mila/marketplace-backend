using Marketplace.Application.DTOs.Catalog;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Catalog
{
    public class ListProductsUseCase(IProductRepository productRepository)
    {
        private readonly IProductRepository _productRepository = productRepository;

        public Task<IEnumerable<ProductModel>> Execute(ListProductsRequest request) =>
            _productRepository.Search(request.Q, request.CategoryId);
    }
}
