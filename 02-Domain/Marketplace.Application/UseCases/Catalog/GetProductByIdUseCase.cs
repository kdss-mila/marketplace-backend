using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Catalog
{
    public class GetProductByIdUseCase(IProductRepository productRepository)
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<ProductModel> Execute(string id)
        {
            var product = await _productRepository.GetById(id);
            if (product is null) throw new NotFoundException("Produto não encontrado");
            return product;
        }
    }
}
