using BCrypt.Net;
using FMAS.API.Data;
using FMAS.API.DTOs.Auth;
using FMAS.API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FMAS.API.Services
{
    public class AuthService
    {
        private readonly FMASDbContext _context;
        private readonly IConfiguration _config;

        // Constructor injection for DbContext and IConfiguration
        public AuthService(FMASDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string Register(RegisterDto dto)
        {
            // 1. Create Organization
            var org = new Organization
            {
                OrganizationId = Guid.NewGuid(),
                Name = dto.organization_name,
                Email = dto.email,
                CreatedAt = DateTime.UtcNow
            };

            // Check if email already exists in Users table
            if (_context.Users.Any(u => u.Email == dto.email))
            {
                throw new Exception("Email already exists");
            }

            _context.Organizations.Add(org);

            // 2. Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            // 3. Create User
            var user = new User
            {
                UserId = Guid.NewGuid(),
                OrganizationId = org.OrganizationId,
                Email = dto.email,
                PasswordHash = hashedPassword,
                IsActive = true,        
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            // 4. Assign Admin role to the user
            var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole == null)
            {
                throw new Exception("Admin role not found. Seed roles first.");
            }

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = adminRole.RoleId
            });

            _context.SaveChanges();

            // 4. Generate JWT (reuse your login logic)
            return GenerateToken(user, "Admin");
        }

        public LoginResponseDto Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.email);

            if (user == null)
                throw new Exception("Invalid credentials");

            var isValid = BCrypt.Net.BCrypt.Verify(dto.password, user.PasswordHash);

            if (!isValid)
                throw new Exception("Invalid credentials");

            var role = (from ur in _context.Set<UserRole>()
                        join r in _context.Set<Role>()
                        on ur.RoleId equals r.RoleId
                        where ur.UserId == user.UserId
                        select r.Name)
                        .FirstOrDefault();

            var token = GenerateToken(user, role);

            return new LoginResponseDto
            {
                token = token
            };
        }

        // Private method to generate JWT token
        private string GenerateToken(Entities.User user, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("organization_id", user.OrganizationId.ToString()),
                new Claim("role", role ?? "Clerk")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}