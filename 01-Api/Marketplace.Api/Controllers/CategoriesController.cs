using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.UseCases.Catalog;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController(ListCategoriesUseCase listCategories) : ControllerBase
    {
        private readonly ListCategoriesUseCase _listCategories = listCategories;

        [HttpGet]
        public async Task<IActionResult> List() => Ok(await _listCategories.Execute());
    }
}
