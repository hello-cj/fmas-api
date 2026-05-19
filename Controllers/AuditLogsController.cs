using FMAS.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly FMASDbContext _context;

        public AuditLogsController(FMASDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            DateTime? from,
            DateTime? to,
            string? action,
            string? module)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(x => x.Action == action);

            if (!string.IsNullOrEmpty(module))
                query = query.Where(x => x.Module == module);

            var logs = await query
                .OrderByDescending(x => x.Timestamp)
                .Take(200)
                .ToListAsync();

            return Ok(logs);
        }
    }
}