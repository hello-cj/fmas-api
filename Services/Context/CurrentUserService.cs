using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FMAS.API.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public Guid? OrganizationId =>
            Guid.TryParse(_http.HttpContext?
                .User?
                .FindFirst("organization_id")?
                .Value, out var id)
                ? id
                : null;

        public Guid? UserId =>
            Guid.TryParse(_http.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value, out var id)
                ? id
                : null;
    }
}