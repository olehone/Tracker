using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class UserWorkspaceConfiguration : IEntityTypeConfiguration<UserWorkspace>
{
    public void Configure(EntityTypeBuilder<UserWorkspace> builder)
    {
        builder.ToTable("UserWorkspaces");

        builder.Property(uw => uw.Role)
            .IsRequired();

        builder.HasOne(uw => uw.User)
            .WithMany(uw => uw.UserWorkspaces)
            .HasForeignKey(uw => uw.UserId);

        builder.HasOne(uw => uw.User)
            .WithMany(uw => uw.UserWorkspaces)
            .HasForeignKey(uw=> uw.UserId);

        builder.HasKey(ub => new { ub.UserId, ub.WorkspaceId });
    }
}