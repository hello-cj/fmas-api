using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/superadmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly SuperAdminService _service;

        public SuperAdminController(SuperAdminService service)
        {
            _service = service;
        }

        // =====================================================
        // GET ORGANIZATIONS
        // =====================================================
        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrganizations()
        {
            var result = await _service.GetOrganizations();

            return Ok(result);
        }

        // =====================================================
        // CREATE ORGANIZATION
        // =====================================================
        [HttpPost("organizations")]
        public async Task<IActionResult> CreateOrganization(
            [FromBody] CreateOrganizationDto dto)
        {
            await _service.CreateOrganization(
                dto.OrganizationName,
                dto.AdminEmail,
                dto.AdminPassword
            );

            return Ok(new
            {
                message = "Organization created successfully."
            });
        }

        // =====================================================
        // RESET PASSWORD
        // =====================================================
        [HttpPut("organizations/{organizationId}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            Guid organizationId,
            [FromBody] ResetPasswordDto dto)
        {
            await _service.ResetAdminPassword(
                organizationId,
                dto.NewPassword
            );

            return Ok(new
            {
                message = "Password reset successful."
            });
        }

        // =====================================================
        // TOGGLE STATUS
        // =====================================================
        [HttpPut("organizations/{organizationId}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid organizationId)
        {
            await _service.ToggleOrganizationStatus(organizationId);

            return Ok(new
            {
                message = "Organization status updated."
            });
        }
    }

    // =====================================================
    // DTOs
    // =====================================================

    public class CreateOrganizationDto
    {
        public string OrganizationName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
    }

    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
    }
}