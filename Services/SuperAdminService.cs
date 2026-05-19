using BCrypt.Net;
using FMAS.API.Data;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class SuperAdminService
    {
        private readonly FMASDbContext _context;

        public SuperAdminService(FMASDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL ORGANIZATIONS
        // =====================================================
        public async Task<List<object>> GetOrganizations()
        {
            var organizations = await _context.Organizations
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return organizations.Select(x => new
            {
                organizationId = x.OrganizationId,
                name = x.Name,
                email = x.Email,
                createdAt = x.CreatedAt,
                isActive = x.IsActive
            }).Cast<object>().ToList();
        }

        // =====================================================
        // CREATE ORGANIZATION
        // =====================================================
        public async Task CreateOrganization(
            string organizationName,
            string adminEmail,
            string adminPassword)
        {
            // CHECK EXISTING EMAIL
            if (await _context.Users.AnyAsync(x => x.Email == adminEmail))
            {
                throw new Exception("Email already exists.");
            }

            var organization = new Organization
            {
                OrganizationId = Guid.NewGuid(),
                Name = organizationName,
                Email = adminEmail,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Organizations.Add(organization);

            // CREATE USER
            var user = new User
            {
                UserId = Guid.NewGuid(),
                OrganizationId = organization.OrganizationId,
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            // GET ADMIN ROLE
            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == "Admin");

            if (adminRole == null)
            {
                throw new Exception("Admin role not found.");
            }

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = adminRole.RoleId
            });

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // RESET PASSWORD
        // =====================================================
        public async Task ResetAdminPassword(
            Guid organizationId,
            string newPassword)
        {
            // FIND ADMIN USER OF ORG
            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == "Admin");

            if (adminRole == null)
            {
                throw new Exception("Admin role not found.");
            }

            var adminUser = await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.UserId equals ur.UserId
                where u.OrganizationId == organizationId
                    && ur.RoleId == adminRole.RoleId
                select u
            ).FirstOrDefaultAsync();

            if (adminUser == null)
            {
                throw new Exception("Organization admin not found.");
            }

            adminUser.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // TOGGLE STATUS
        // =====================================================
        public async Task ToggleOrganizationStatus(Guid organizationId)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(x =>
                    x.OrganizationId == organizationId);

            if (organization == null)
            {
                throw new Exception("Organization not found.");
            }

            organization.IsActive = !organization.IsActive;

            // OPTIONAL:
            // Disable all organization users too
            var users = await _context.Users
                .Where(x => x.OrganizationId == organizationId)
                .ToListAsync();

            foreach (var user in users)
            {
                user.IsActive = organization.IsActive;
            }

            await _context.SaveChangesAsync();
        }
    }
}