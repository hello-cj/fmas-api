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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JournalEntry>();
            modelBuilder.Entity<JournalEntryLine>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<Organization>();

        }
    }
}