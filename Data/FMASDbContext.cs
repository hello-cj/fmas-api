using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS = GLOBAL (IMPORTANT FOR LOGIN)
            modelBuilder.Entity<User>();

            modelBuilder.Entity<Organization>();

            // TENANT-SCOPED DATA ONLY
            modelBuilder.Entity<JournalEntry>()
                .HasQueryFilter(x => x.OrganizationId == _organizationId);
        }
    }
}