using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class WorkspaceUserConfiguration : IEntityTypeConfiguration<WorkspaceUser>
{
    public void Configure(EntityTypeBuilder<WorkspaceUser> builder)
    {
        builder.ToTable("WorkspaceUsers");

        builder.Property(uw => uw.Role)
            .IsRequired();

        builder.HasOne(uw => uw.User)
            .WithMany()
            .HasForeignKey(uw => uw.UserId);

        builder.HasKey(ub => new { ub.UserId, ub.WorkspaceId });
    }
}