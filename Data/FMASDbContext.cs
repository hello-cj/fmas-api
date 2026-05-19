using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FMAS.API.Data
{
    public class FMASDbContext : DbContext
    {
        private readonly Guid? _organizationId;

        public FMASDbContext(DbContextOptions<FMASDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<ARInvoice> ARInvoices { get; set; }
        public DbSet<ARInvoiceLine> ARInvoiceLines { get; set; }
        public DbSet<ARPayment> ARPayments { get; set; }
        public DbSet<APInvoice> APInvoices { get; set; }
        public DbSet<APInvoiceLine> APInvoiceLines { get; set; }
        public DbSet<APPayment> APPayments { get; set; }

        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JournalEntry>();

            modelBuilder.Entity<JournalEntryLine>();

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(j => j.JournalEntry)
                .WithMany(j => j.Lines)
                .HasForeignKey(j => j.JournalEntryId);

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.AccountId);

            modelBuilder.Entity<User>();

            modelBuilder.Entity<Role>();

            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<Organization>();

            modelBuilder.Entity<ARInvoice>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<ARInvoiceLine>()
                .HasOne(l => l.ARInvoice)
                .WithMany(i => i.Lines)
                .HasForeignKey(l => l.ARInvoiceId);

            modelBuilder.Entity<ARPayment>()
                .HasOne(p => p.ARInvoice)
                .WithMany()
                .HasForeignKey(p => p.ARInvoiceId);

            modelBuilder.Entity<APInvoice>()
                .HasMany(x => x.Lines)
                .WithOne(x => x.APInvoice)
                .HasForeignKey(x => x.APInvoiceId);

            modelBuilder.Entity<APInvoice>()
                .HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId);
        }
    }
}