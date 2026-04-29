using FMAS.API.DTOs.Auth;
using FMAS.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var token = _authService.Register(dto);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var result = _authService.Login(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(result);
        }
    }
}
