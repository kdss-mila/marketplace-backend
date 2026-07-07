using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Common;
using Marketplace.Application.DTOs.Seller;
using Marketplace.Application.UseCases.Seller;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "seller")]
    [Route("api/seller")]
    public class SellerController(
        ListSellerProductsUseCase listProducts,
        CreateProductUseCase createProduct,
        UpdateProductUseCase updateProduct,
        DeleteProductUseCase deleteProduct,
        ListSellerSalesUseCase listSales,
        SetTrackingCodeUseCase setTracking,
        CompleteSellerOnboardingUseCase completeOnboarding) : ControllerBase
    {
        private readonly ListSellerProductsUseCase _listProducts = listProducts;
        private readonly CreateProductUseCase _createProduct = createProduct;
        private readonly UpdateProductUseCase _updateProduct = updateProduct;
        private readonly DeleteProductUseCase _deleteProduct = deleteProduct;
        private readonly ListSellerSalesUseCase _listSales = listSales;
        private readonly SetTrackingCodeUseCase _setTracking = setTracking;
        private readonly CompleteSellerOnboardingUseCase _completeOnboarding = completeOnboarding;

        [HttpGet("products")]
        public async Task<IActionResult> ListProducts()
            => Ok(await _listProducts.Execute());

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var product = await _createProduct.Execute(request);
            return StatusCode(StatusCodes.Status201Created, product);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductRequest request)
            => Ok(await _updateProduct.Execute(id, request));

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _deleteProduct.Execute(id);
            return Ok(new SuccessResponse());
        }

        [HttpGet("sales")]
        public async Task<IActionResult> ListSales()
            => Ok(await _listSales.Execute());

        [HttpPatch("sales/{id}/tracking")]
        public async Task<IActionResult> SetTracking(string id, [FromBody] SetTrackingCodeRequest request)
            => Ok(await _setTracking.Execute(id, request));

        [HttpPost("onboarding")]
        public async Task<IActionResult> Onboarding([FromBody] SellerOnboardingRequest request)
            => Ok(await _completeOnboarding.Execute(request));
    }
}
