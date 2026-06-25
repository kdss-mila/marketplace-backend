using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Common.Exceptions;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.UseCases.Orders;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController(
        CreateOrderUseCase createOrder,
        UploadReceiptUseCase uploadReceipt,
        ListMyOrdersUseCase listMyOrders) : ControllerBase
    {
        private readonly CreateOrderUseCase _createOrder = createOrder;
        private readonly UploadReceiptUseCase _uploadReceipt = uploadReceipt;
        private readonly ListMyOrdersUseCase _listMyOrders = listMyOrders;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var response = await _createOrder.Execute(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("{id}/receipt")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadReceipt(string id, IFormFile receipt)
        {
            if (receipt is null || receipt.Length == 0)
                throw new ValidationException("Arquivo de comprovante obrigatório");

            await using var stream = receipt.OpenReadStream();
            var order = await _uploadReceipt.Execute(new UploadReceiptRequest
            {
                OrderId = id,
                FileStream = stream,
                FileName = receipt.FileName,
            });
            return Ok(order);
        }

        [HttpGet("me")]
        public async Task<IActionResult> ListMine() => Ok(await _listMyOrders.Execute());
    }
}
