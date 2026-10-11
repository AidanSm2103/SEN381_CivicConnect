namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. (FR-006)</summary>
    public class StaffCategoryScopeConfiguration : IEntityTypeConfiguration<StaffCategoryScope>
    {
        public void Configure(EntityTypeBuilder<StaffCategoryScope> builder)
        {
            builder.HasIndex(s => new { s.UserId, s.CategoryId }).IsUnique();
            builder.HasOne(s => s.User).WithMany(u => u.CategoryScopes).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(s => s.Category).WithMany().HasForeignKey(s => s.CategoryId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
