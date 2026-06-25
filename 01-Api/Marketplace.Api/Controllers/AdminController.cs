using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Admin;
using Marketplace.Application.DTOs.Common;
using Marketplace.Application.UseCases.Admin;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController(
        ListUsersUseCase listUsers,
        BanUserUseCase banUser,
        ListAdminCategoriesUseCase listCategories,
        CreateCategoryUseCase createCategory,
        UpdateCategoryUseCase updateCategory,
        DeleteCategoryUseCase deleteCategory,
        ListPendingOrdersUseCase listPendingOrders,
        ApproveOrderUseCase approveOrder,
        ListRepassesUseCase listRepasses,
        MarkRepassePaidUseCase markRepassePaid) : ControllerBase
    {
        private readonly ListUsersUseCase _listUsers = listUsers;
        private readonly BanUserUseCase _banUser = banUser;
        private readonly ListAdminCategoriesUseCase _listCategories = listCategories;
        private readonly CreateCategoryUseCase _createCategory = createCategory;
        private readonly UpdateCategoryUseCase _updateCategory = updateCategory;
        private readonly DeleteCategoryUseCase _deleteCategory = deleteCategory;
        private readonly ListPendingOrdersUseCase _listPendingOrders = listPendingOrders;
        private readonly ApproveOrderUseCase _approveOrder = approveOrder;
        private readonly ListRepassesUseCase _listRepasses = listRepasses;
        private readonly MarkRepassePaidUseCase _markRepassePaid = markRepassePaid;

        [HttpGet("users")]
        public async Task<IActionResult> ListUsers()
            => Ok(await _listUsers.Execute());

        [HttpPatch("users/{id}/ban")]
        public async Task<IActionResult> BanUser(string id, [FromBody] BanUserRequest request)
            => Ok(await _banUser.Execute(id, request));

        [HttpGet("categories")]
        public async Task<IActionResult> ListCategories()
            => Ok(await _listCategories.Execute());

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            var category = await _createCategory.Execute(request);
            return StatusCode(StatusCodes.Status201Created, category);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryRequest request)
            => Ok(await _updateCategory.Execute(id, request));

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _deleteCategory.Execute(id);
            return Ok(new SuccessResponse());
        }

        [HttpGet("orders")]
        public async Task<IActionResult> ListPendingOrders()
            => Ok(await _listPendingOrders.Execute());

        [HttpPost("orders/{id}/approve")]
        public async Task<IActionResult> ApproveOrder(string id)
            => Ok(await _approveOrder.Execute(id));

        [HttpGet("repasses")]
        public async Task<IActionResult> ListRepasses()
            => Ok(await _listRepasses.Execute());

        [HttpPost("repasses/{id}/mark-paid")]
        public async Task<IActionResult> MarkRepassePaid(string id)
            => Ok(await _markRepassePaid.Execute(id));
    }
}
