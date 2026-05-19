using FMAS.API.Data;
using FMAS.API.Entities;

namespace FMAS.API.Services
{
    public class AuditService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public AuditService(FMASDbContext context, CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task LogAsync(
            string action,
            string module,
            Guid? entityId,
            string description)
        {
            var orgId = _currentUser.OrganizationId
                ?? throw new Exception("Missing OrganizationId");

            var userId = _currentUser.UserId
                ?? throw new Exception("Missing UserId");

            var email = _currentUser.Email;

            var log = new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                OrganizationId = orgId,
                UserId = userId,

                UserEmail = email,

                Action = action,
                Module = module,              // ✅ FIXED (was EntityName mismatch)

                EntityName = module ?? "UNKNOWN",

                EntityId = entityId,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}