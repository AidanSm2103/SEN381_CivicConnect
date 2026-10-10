namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. (NFR-002, NFR-003, NFR-004)</summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        }
    }
}
