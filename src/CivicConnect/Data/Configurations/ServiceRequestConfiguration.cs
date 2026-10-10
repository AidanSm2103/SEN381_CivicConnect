namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Adds the M3 columns/constraints for ServiceRequest. (FR-001, FR-003, FR-009, NFR-008)</summary>
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            // Structure is provided so migrations are predictable. Adjust ONLY if the migration review finds a problem.
            builder.Property(r => r.ReferenceNo).IsRequired().HasMaxLength(30);
            builder.HasIndex(r => r.ReferenceNo).IsUnique();                       // FR-001 unique ID
            builder.Property(r => r.Description).IsRequired().HasMaxLength(2000);  // DB layer of 3-layer validation
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(r => r.RowVersion).IsRowVersion();                    // FR-009 optimistic locking
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.RequesterId);

            builder.HasOne(r => r.Requester).WithMany().HasForeignKey(r => r.RequesterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.AssignedStaff).WithMany().HasForeignKey(r => r.AssignedStaffId).OnDelete(DeleteBehavior.Restrict);

            // TODO(Michael-John): CHECK constraint limiting Status to the five allowed values
            // ('Submitted','Assigned','InProgress','Resolved','Closed') via builder.ToTable(t => t.HasCheckConstraint(...)).
        }
    }
}
