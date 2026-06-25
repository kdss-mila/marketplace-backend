using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.DTOs.Auth;
using Marketplace.Application.UseCases.Auth;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
        LoginUseCase loginUseCase,
        RegisterUseCase registerUseCase,
        GetMeUseCase getMeUseCase) : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase = loginUseCase;
        private readonly RegisterUseCase _registerUseCase = registerUseCase;
        private readonly GetMeUseCase _getMeUseCase = getMeUseCase;

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
            => Ok(await _loginUseCase.Execute(request));

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var response = await _registerUseCase.Execute(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me() => Ok(await _getMeUseCase.Execute());
    }
}
