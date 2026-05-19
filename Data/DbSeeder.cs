using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Data
{
    public class DbSeeder
    {
        public static void Seed(FMASDbContext context)
        {
            context.Database.Migrate();

            SeedRoles(context);
            SeedFinanceData(context);
        }

        public static void SeedRoles(FMASDbContext context)
        {
            if (context.Roles.Any())
                return;

            var roles = new List<Role>
            {
                new Role { RoleId = Guid.NewGuid(), Name = "SuperAdmin" },
                new Role { RoleId = Guid.NewGuid(), Name = "Admin" },
                new Role { RoleId = Guid.NewGuid(), Name = "Accountant" },
                new Role { RoleId = Guid.NewGuid(), Name = "Clerk" }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        // ================= CLEAN FINANCE SEED =================
        public static void SeedFinanceData(FMASDbContext context)
        {
            if (context.Accounts.Any())
                return;

            // ACCOUNTS

            var orgId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var cash = new Account
            {
                AccountId = Guid.NewGuid(),
                Code = "1010",
                Name = "Cash",
                AccountType = "Asset",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                OrganizationId = orgId
            };

            var ar = new Account
            {
                AccountId = Guid.NewGuid(),
                Code = "1200",
                Name = "Accounts Receivable",
                AccountType = "Asset",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                OrganizationId = orgId
            };

            var ap = new Account
            {
                AccountId = Guid.NewGuid(),
                Code = "2000",
                Name = "Accounts Payable",
                AccountType = "Liability",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                OrganizationId = orgId
            };

            var revenue = new Account
            {
                AccountId = Guid.NewGuid(),
                Code = "4000",
                Name = "Service Revenue",
                AccountType = "Revenue",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                OrganizationId = orgId
            };

            var expense = new Account
            {
                AccountId = Guid.NewGuid(),
                Code = "6000",
                Name = "Office Expense",
                AccountType = "Expense",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                OrganizationId = orgId
            };

            context.Accounts.AddRange(cash, ar, ap, revenue, expense);
            context.SaveChanges();

            // ================= AR INVOICE =================
            var arInvoice = new ARInvoice
            {
                ARInvoiceId = Guid.NewGuid(),
                OrganizationId = orgId,
                CustomerId = Guid.NewGuid(),
                InvoiceNumber = "AR-0001",
                InvoiceDate = DateTime.UtcNow,
                Status = InvoiceStatus.Posted,
                TotalAmount = 1000
            };

            context.ARInvoices.Add(arInvoice);
            context.SaveChanges();

            context.ARInvoiceLines.Add(new ARInvoiceLine
            {
                ARInvoiceLineId = Guid.NewGuid(),
                ARInvoiceId = arInvoice.ARInvoiceId,
                Description = "Development Service",
                Quantity = 1,
                UnitPrice = 1000,
                //Total = 1 * 1000

            });

            context.SaveChanges();

            // ================= AP INVOICE =================
            var apInvoice = new APInvoice
            {
                APInvoiceId = Guid.NewGuid(),
                VendorId = Guid.NewGuid(), // temporary seed vendor id
                InvoiceDate = DateTime.UtcNow,
                Reference = "AP-0001",
                Description = "Office Supplies",
                Status = InvoiceStatus.Posted,
                TotalAmount = 500
            };

            context.APInvoices.Add(apInvoice);
            context.SaveChanges();

            context.APInvoiceLines.Add(new APInvoiceLine
            {
                APInvoiceLineId = Guid.NewGuid(),
                APInvoiceId = apInvoice.APInvoiceId,
                Description = "Office Materials",
                Quantity = 5,
                UnitPrice = 100,
                //Total = 5 * 100

            });

            context.SaveChanges();
        }

        public static void SeedSuperAdmin(FMASDbContext context)
        {
            var existing = context.Users
                .FirstOrDefault(x => x.Email == "superadmin@fmas.com");

            if (existing != null)
                return;

            var superAdminRole = context.Roles
                .First(r => r.Name == "SuperAdmin");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                OrganizationId = null,
                Email = "superadmin@fmas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("superadmin123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = superAdminRole.RoleId
            });

            context.SaveChanges();
        }
    }
}