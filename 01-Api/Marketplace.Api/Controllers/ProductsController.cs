using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Catalog;
using Marketplace.Application.UseCases.Catalog;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController(
        ListProductsUseCase listProducts,
        GetProductByIdUseCase getProductById) : ControllerBase
    {
        private readonly ListProductsUseCase _listProducts = listProducts;
        private readonly GetProductByIdUseCase _getProductById = getProductById;

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] string? q,
            [FromQuery] string? categoryId,
            [FromQuery] bool includeSubcategories = false)
            => Ok(await _listProducts.Execute(new ListProductsRequest
            {
                Q = q,
                CategoryId = categoryId,
                IncludeSubcategories = includeSubcategories,
            }));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => Ok(await _getProductById.Execute(id));
    }
}
