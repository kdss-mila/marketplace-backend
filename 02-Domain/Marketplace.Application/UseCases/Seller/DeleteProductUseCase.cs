using Marketplace.Application.Common.Exceptions;
using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;

namespace Marketplace.Application.UseCases.Seller
{
    public class DeleteProductUseCase(
        ICurrentUserResolver currentUser,
        IProductRepository productRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task Execute(string productId)
        {
            var user = await SellerGuard.RequireSeller(_currentUser);

            var product = await _productRepository.GetBySellerIdAndId(user.Id, productId)
                ?? throw new NotFoundException("Produto não encontrado");

            await _productRepository.Delete(product.Id);
        }
    }
}
