using Microsoft.EntityFrameworkCore;
using CivicConnect.Models;

namespace CivicConnect.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();

                // Initial predefined categories (seed data). Add new ones here and create a migration.
                entity.HasData(
                    new Category { Id = 1, Name = "Maintenance", IsActive = true },
                    new Category { Id = 2, Name = "IT Support", IsActive = true },
                    new Category { Id = 3, Name = "Facilities", IsActive = true },
                    new Category { Id = 4, Name = "Cleaning", IsActive = true },
                    new Category { Id = 5, Name = "Security", IsActive = true },
                    new Category { Id = 6, Name = "Other", IsActive = true });
            });

            // Database-level safety net: every request must reference an existing category,
            // and a category that is in use cannot be deleted.
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
