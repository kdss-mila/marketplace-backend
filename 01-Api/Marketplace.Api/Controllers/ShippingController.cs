using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Shipping;
using Marketplace.Application.UseCases.Shipping;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/shipping")]
    public class ShippingController(CalculateShippingUseCase calculateShipping) : ControllerBase
    {
        private readonly CalculateShippingUseCase _calculateShipping = calculateShipping;

        [HttpPost("quote")]
        public async Task<IActionResult> Quote([FromBody] ShippingQuoteRequest request)
            => Ok(await _calculateShipping.Execute(request));
    }
}
