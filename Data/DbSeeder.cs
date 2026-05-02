using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Data
{
    public class DbSeeder
    {
        public static void SeedRoles(FMASDbContext context)
        {
            // Ensure database is created/migrated first
            context.Database.Migrate();

            // Check if roles already exist
            if (context.Roles.Any())
                return;

            var roles = new List<Role>
            {
                new Role
                {
                    RoleId = Guid.NewGuid(),
                    Name = "Admin"
                },
                new Role
                {
                    RoleId = Guid.NewGuid(),
                    Name = "Accountant"
                },
                new Role
                {
                    RoleId = Guid.NewGuid(),
                    Name = "Clerk"
                }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }
}
