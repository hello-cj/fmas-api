using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FMAS.API.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? OrganizationId
        {
            get
            {
                var value = _http.HttpContext?
                    .User?
                    .FindFirst("organization_id")?
                    .Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public Guid? UserId
        {
            get
            {
                var value = _http.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? _httpContextAccessor.HttpContext?.User?
                .FindFirst("email")?.Value;

        public string? Role =>
            _http.HttpContext?
                .User?
                .FindFirst(ClaimTypes.Role)?
                .Value;

        public bool IsSuperAdmin =>
            Role == "SuperAdmin";
    }
}