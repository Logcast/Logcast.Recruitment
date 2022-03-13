using Logcast.Recruitment.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logcast.Recruitment.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AudioFile> AudioFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Subscription>().HasIndex(p => p.Email).IsUnique();

            modelBuilder.Entity<AudioFile>(e =>
            {
                e.Property(a => a.Id).ValueGeneratedOnAdd();
                e.HasIndex(a => a.Id);
            });
        }
    }
}