using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_AUTH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService pAuthService)
        {
            _authService = pAuthService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] UserRequest registerDto)
        {
            try
            {
                TokenResponseDTO tokens = await _authService.RegisterAsync(registerDto);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] UserRequest loginDto)
        {
            var tokens = await _authService.LoginAsync(loginDto);
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                TokenResponseDTO tokens = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
