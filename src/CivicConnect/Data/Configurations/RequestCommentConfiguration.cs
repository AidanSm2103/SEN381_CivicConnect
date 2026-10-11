namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Append-only feedback / resolution notes (FR-005, FR-011).</summary>
    public class RequestCommentConfiguration : IEntityTypeConfiguration<RequestComment>
    {
        public void Configure(EntityTypeBuilder<RequestComment> builder)
        {
            builder.Property(c => c.Kind).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(c => c.Body).IsRequired().HasMaxLength(2000);
            builder.HasIndex(c => c.ServiceRequestId);
            builder.HasOne(c => c.ServiceRequest).WithMany(r => r.Comments).HasForeignKey(c => c.ServiceRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Author).WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
