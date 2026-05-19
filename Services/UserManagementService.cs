using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;

namespace FMAS.API.Services
{
    public class UserManagementService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public UserManagementService(FMASDbContext context, CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // CREATE USER
        public void CreateUser(CreateUserDto dto)
        {
            if (!_currentUser.OrganizationId.HasValue)
                throw new Exception("Organization not found in token");

            var orgId = _currentUser.OrganizationId.Value;

            if (_context.Users.Any(u => u.Email == dto.Email && u.OrganizationId == orgId))
                throw new Exception("User already exists in this organization.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                OrganizationId = orgId,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            var role = _context.Roles.FirstOrDefault(r => r.Name == dto.Role);

            if (role == null)
                throw new Exception("Invalid role");

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            });

            _context.SaveChanges();
        }

        // GET USERS
        public List<object> GetUsers()
        {
            var orgId = _currentUser.OrganizationId;

            return (from u in _context.Users
                    join ur in _context.UserRoles on u.UserId equals ur.UserId
                    join r in _context.Roles on ur.RoleId equals r.RoleId
                    where u.OrganizationId == orgId
                    select new
                    {
                    u.UserId,
                    u.Email,
                    u.IsActive,
                    Role = r.Name
                })
                .ToList<object>();
        }

        // UPDATE USER
        public void UpdateUser(Guid userId, UpdateUserDto dto)
        {
            var orgId = _currentUser.OrganizationId;

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.OrganizationId == orgId);

            if (user == null)
                throw new Exception("User not found");

            user.Email = dto.Email;
            user.IsActive = dto.IsActive;

            _context.SaveChanges();
        }

        // DELETE USER
        public void DeleteUser(Guid userId)
        {
            var orgId = _currentUser.OrganizationId;

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.OrganizationId == orgId);

            if (user == null)
                throw new Exception("User not found");

            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
