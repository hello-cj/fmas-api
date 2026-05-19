using FMAS.API.Data;
using FMAS.API.DTOs.Auth;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly FMASDbContext _context;

        public AuthController(AuthService authService, FMASDbContext context)
        {
            _authService = authService;
            _context = context;
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

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var orgIdString = User.FindFirst("organization_id")?.Value;

            Guid? orgId = null;

            string? organizationName = null;

            if (Guid.TryParse(orgIdString, out var parsedOrgId))
            {
                orgId = parsedOrgId;

                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == orgId);

                organizationName = organization?.Name;
            }

            return Ok(new
            {
                userId,
                email,
                role,
                organizationId = orgId,
                organizationName
            });
        }
    }
}
