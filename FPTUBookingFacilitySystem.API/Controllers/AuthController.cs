using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPTUBookingFacilitySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var loginResponse = await _authService.LoginAsync(request);

            if (loginResponse == null)
            {
                return Unauthorized(new { message = "Invalid email or password, or account is inactive." });
            }

            return Ok(loginResponse);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Get token from Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required." });
            }

            var result = await _authService.LogoutAsync(token);

            if (result)
            {
                return Ok(new { message = "Logged out successfully." });
            }

            return BadRequest(new { message = "Logout failed." });
        }
    }
}