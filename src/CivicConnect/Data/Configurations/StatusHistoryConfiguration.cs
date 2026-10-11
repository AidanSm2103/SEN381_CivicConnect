namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Append-only audit table (NFR-010).</summary>
    public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
    {
        public void Configure(EntityTypeBuilder<StatusHistory> builder)
        {
            builder.Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(20);
            builder.Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(h => h.Note).HasMaxLength(1000);
            builder.HasIndex(h => h.ServiceRequestId);
            builder.HasOne(h => h.ServiceRequest).WithMany(r => r.StatusHistory).HasForeignKey(h => h.ServiceRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(h => h.ChangedBy).WithMany().HasForeignKey(h => h.ChangedByUserId).OnDelete(DeleteBehavior.Restrict);
            // TODO(Michael-John): optional - confirm no code path ever updates/deletes these rows (append-only rule).
        }
    }
}
