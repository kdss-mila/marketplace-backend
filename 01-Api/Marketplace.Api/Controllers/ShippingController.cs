using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Shipping;
using Marketplace.Application.UseCases.Shipping;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/shipping")]
    public class ShippingController(
        CalculateShippingUseCase calculateShipping,
        LookupAddressByCepUseCase lookupAddress) : ControllerBase
    {
        private readonly CalculateShippingUseCase _calculateShipping = calculateShipping;
        private readonly LookupAddressByCepUseCase _lookupAddress = lookupAddress;

        [HttpPost("quote")]
        public async Task<IActionResult> Quote([FromBody] ShippingQuoteRequest request)
            => Ok(await _calculateShipping.Execute(request));

        [HttpGet("address/{cep}")]
        public async Task<IActionResult> Address(string cep)
            => Ok(await _lookupAddress.Execute(cep));
    }
}
