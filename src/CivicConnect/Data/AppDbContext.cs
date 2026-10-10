namespace CivicConnect.Data
{
    /// <summary>
    /// EF Core context. OWNER: Michael-John Robinson (only person who edits this file or runs migrations).
    /// Per-entity configuration for the NEW entities lives in Data/Configurations (picked up automatically below).
    /// The Category seed data and the ServiceRequest-&gt;Category FK stay here exactly as bootstrapped,
    /// so the existing migration snapshot is unchanged.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<StaffCategoryScope> StaffCategoryScopes { get; set; } = null!;
        public DbSet<StatusHistory> StatusHistories { get; set; } = null!;
        public DbSet<RequestComment> RequestComments { get; set; } = null!;

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

            // Picks up every IEntityTypeConfiguration<T> in Data/Configurations.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
