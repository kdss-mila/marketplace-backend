using Marketplace.Domain.Interface.Repository;
using Marketplace.Domain.Interface.Service;
using Marketplace.Domain.Model;

namespace Marketplace.Application.UseCases.Seller
{
    public class ListSellerProductsUseCase(
        ICurrentUserResolver currentUser,
        IProductRepository productRepository)
    {
        private readonly ICurrentUserResolver _currentUser = currentUser;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<IEnumerable<ProductModel>> Execute()
        {
            var user = await SellerGuard.RequireSeller(_currentUser);
            return await _productRepository.GetBySellerId(user.Id);
        }
    }
}
